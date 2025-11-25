package handlers

import (
	"bytes"
	"errors"
	"fmt"
	"net"
	"slices"

	"atxmedia.us/multiassembly/handlers/bit"
	"atxmedia.us/multiassembly/player"
)

var conns []*net.Conn

func HandleConn(conn *net.Conn) {
	buf := make([]byte, 2048)
	size := 0

	var associatedPlayer *player.Player
	var err error

	connIdx := len(conns)
	conns = append(conns, conn)

	for {
		size, err = (*conn).Read(buf)

		if err != nil {
			fmt.Println("TCP connection error occured: ", err.Error())
			fmt.Print("Closing connection for ", (*conn).RemoteAddr().String(), "...\n")

			conns = slices.Delete(conns, connIdx, connIdx+1)

			if associatedPlayer == nil {
				break
			}
			player.RemoveByID(associatedPlayer.PrivateUUID)
			for _, c := range conns {
				TCPWrite(c, bit.String("UREG"), bit.String((*associatedPlayer).PublicUUID), bit.String((*associatedPlayer).Username))
			}

			ClientsMutex.Lock()
			delete(clients, associatedPlayer.PrivateUUID)
			ClientsMutex.Unlock()

			break
		}

		byteBuf := bytes.NewBuffer(buf[:size])

		for byteBuf.Len() > 0 {
			size, ok := bit.ReadUint16(byteBuf)

			if !ok {
				fmt.Println("Failed to read buffer size")
			}

			tmpBuf := make([]byte, size)

			byteBuf.Read(tmpBuf)

			HandleRequest(bytes.NewBuffer(tmpBuf), &associatedPlayer, conn)
		}
	}
	associatedPlayer.Remove()
	(*conn).Close()
}

func HandleRequest(buf *bytes.Buffer, aPlayer **player.Player, conn *net.Conn) {
	uuid, fcfi, ok := GetMeta(buf)
	if !ok {
		goto Corrupted
	}
	//fmt.Println("TCP Message:\nUUID: ", uuid, "\nFCFI: ", fcfi)

	player.Mutex.Lock()
	defer player.Mutex.Unlock()
	switch fcfi {
	case "REG_":
		puuid, ok := bit.ReadString(buf, 16)
		if !ok {
			goto Corrupted
		}

		username, ok := bit.ReadString(buf, -1)
		if !ok {
			goto Corrupted
		}

		(*aPlayer) = player.New(username, uuid, puuid)

		fmt.Print("Registered new player:\nUsername: ", username, "\nPrivate UUID: ", uuid, "\nPublic UUID: ", puuid, "\n")

		for _, c := range conns {
			err := TCPWrite(c, bit.String("REG_"), bit.String((*aPlayer).PublicUUID), bit.String((*aPlayer).Username))
			if err != nil {
				fmt.Println(err.Error())
			}
		}
		for _, p := range player.All() {
			err := TCPWrite(conn, bit.String("REG_"), bit.String(p.PublicUUID), bit.String(p.Username))
			if err != nil {
				fmt.Println(err.Error())
			}
		}
	case "UREG":
		player.RemoveByID(uuid)
		for _, c := range conns {
			err := TCPWrite(c, bit.String("UREG"), bit.String((*aPlayer).PublicUUID), bit.String((*aPlayer).Username))
			if err != nil {
				fmt.Println(err.Error())
			}
		}
	default:

	}
	return
Corrupted:
	fmt.Println("Data corrupted for TCP connection")
}

func TCPWrite(conn *net.Conn, bytes ...[]byte) error {
	if conn == nil {
		return errors.New("TCP connection is nil")
	}
	data := bit.M(bytes...)
	_, err := (*conn).Write(bit.M(bit.Uint16(uint16(len(data))), data))
	if err != nil {
		return err
	}
	return nil
}
