package handlers

import (
	"bytes"
	"errors"
	"fmt"
	"net"
	"sync"
	"time"

	"atxmedia.us/multiassembly/globals"
	"atxmedia.us/multiassembly/handlers/bit"
	udpfuncs "atxmedia.us/multiassembly/handlers/funcs/udp"
	"atxmedia.us/multiassembly/player"
)

const UDPClientTimeout float64 = 10.0

var udp *net.UDPConn

type UDPClient struct {
	Addr    *net.UDPAddr
	Timeout float64
	UUID    string
}

var clients map[string]*UDPClient = make(map[string]*UDPClient)

func Handle(buf *bytes.Buffer, client *net.UDPAddr) {
	uuid, fcfi, ok := GetMeta(buf)
	if !ok {
		fmt.Println("Data corrupted for UDP connection")
		return
	}
	//fmt.Println("UDP Message:\nUUID: ", uuid, "\nFCFI: ", fcfi)

	c, found := clients[uuid]

	if !found {
		c = &UDPClient{Addr: client, UUID: uuid}
		clients[uuid] = c
		fmt.Println("Registered UDP client", uuid)
	}

	c.Timeout = UDPClientTimeout

	udpfuncs.Run(uuid, fcfi, buf)
}

func UDPLoop(wg *sync.WaitGroup, conn *net.UDPConn) {
	defer wg.Done()
	udp = conn
	for {
		for k, v := range clients {
			//Timing out
			v.Timeout -= 1.0 / float64(globals.Hertz)
			if v.Timeout <= 0 {
				fmt.Println("Timed out client", k)
				delete(clients, k)
			}

			player.Mutex.Lock()

			p := player.GetByID(v.UUID)

			if p == nil {
				player.Mutex.Unlock()
				break
			}
			//Data sending
			for _, rec := range clients {
				UDPWrite(rec.Addr, bit.String("PTUP"), bit.String(p.PublicUUID), bit.Float64(p.Position.X), bit.Float64(p.Position.Y), bit.Float64(p.Position.Z))
				UDPWrite(rec.Addr, bit.String("PTUR"), bit.String(p.PublicUUID), bit.Float64(p.Rotation.X), bit.Float64(p.Rotation.Y), bit.Float64(p.Rotation.Z))
			}

			player.Mutex.Unlock()
		}
		time.Sleep(time.Duration(1e+9 / globals.Hertz))
	}
}

func UDPWrite(addr *net.UDPAddr, bytes ...[]byte) error {
	if udp == nil {
		return errors.New("UDP server not assigned or initialized")
	}
	_, err := udp.WriteTo(bit.M(bytes...), addr)
	if err != nil {
		return err
	}
	return nil
}
