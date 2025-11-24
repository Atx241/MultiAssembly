package main

import (
	"bytes"
	"fmt"
	"net"
	"sync"

	"atxmedia.us/multiassembly/handlers"
)

func main() {
	var wg sync.WaitGroup
	wg.Add(1)
	go tcpMain(&wg)
	wg.Add(1)
	go udpMain(&wg)
	wg.Wait()
}

func tcpMain(wg *sync.WaitGroup) {
	defer wg.Done()
	fmt.Println("Initializing TCP...")
	tcp, err := net.Listen("tcp", ":33333")
	if err != nil {
		fmt.Println("Error binding TCP listener: ", err.Error())
		return
	}

	fmt.Println("TCP successfully initialized")
	for {
		conn, err := tcp.Accept()
		if err != nil {
			fmt.Println("TCP Error occured: ", err.Error())
		}
		handlers.HandleConn(&conn)
	}
}

func udpMain(wg *sync.WaitGroup) {
	defer wg.Done()
	fmt.Println("Initializing UDP...")
	addr, err := net.ResolveUDPAddr("udp", ":33334")
	if err != nil {
		fmt.Println("Error resolving UDP address: ", err.Error())
		return
	}
	udp, err := net.ListenUDP("udp", addr)
	if err != nil {
		fmt.Println("Error binding UDP listener: ", err.Error())
		return
	}
	fmt.Println("UDP successfully initialized")

	wg.Add(1)
	go handlers.UDPLoop(wg, udp)

	buf := make([]byte, 2048)

	for {
		var err error
		var client *net.UDPAddr
		size := 0

		size, client, err = udp.ReadFromUDP(buf)

		if err != nil {
			fmt.Println("UDP Error occured: ", err.Error())
		}

		cbuf := make([]byte, size)
		copy(cbuf, buf[:size])

		go handlers.Handle(bytes.NewBuffer(cbuf[:size]), client)
	}
}
