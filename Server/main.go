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
	wg.Add(1)
	go handlers.UDPLoop(&wg)
	wg.Wait()
}

func tcpMain(wg *sync.WaitGroup) {
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
	wg.Done()
}

func udpMain(wg *sync.WaitGroup) {
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

	buf := make([]byte, 2048)
	size := 0
	for {
		var err error
		var client *net.UDPAddr
		size, client, err = udp.ReadFromUDP(buf)
		if err != nil {
			fmt.Println("UDP Error occured: ", err.Error())
		}
		go handlers.Handle(bytes.NewBuffer(buf[:size]), client)
	}
	wg.Done()
}
