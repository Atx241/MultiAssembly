package handlers

import (
	"bytes"
	"fmt"
	"net"
	"sync"
	"time"

	"atxmedia.us/multiassembly/globals"
	udpfuncs "atxmedia.us/multiassembly/handlers/funcs/udp"
)

type UDPClient struct {
	addr    *net.UDPAddr
	timeout float64
}

var clients []*UDPClient

func Handle(buf *bytes.Buffer, client *net.UDPAddr) {
	uuid, fcfi, ok := GetMeta(buf)
	if !ok {
		goto Corrupted
	}
	fmt.Println("UDP Message:\nUUID: ", uuid, "\nFCFI: ", fcfi)
	udpfuncs.Run(uuid, fcfi, buf)
	return
Corrupted:
	fmt.Println("Data corrupted for UDP connection")
}

func UDPLoop(wg *sync.WaitGroup) {
	for {
		time.Sleep(time.Duration(1000000000 / globals.Hertz))
	}
	wg.Done()
}
