package handlers

import (
	"bytes"
	"fmt"
	"net"

	"atxmedia.us/multiassembly/player"
)

func HandleConn(conn *net.Conn) {
	buf := make([]byte, 2048)
	size := 0
	var err error
	for {
		size, err = (*conn).Read(buf)

		if err != nil {
			fmt.Println("TCP connection error occured: ", err.Error())
			fmt.Print("Closing connection for ", (*conn).RemoteAddr().String(), "...\n")
			break
		}

		HandleRequest(bytes.NewBuffer(buf[:size]))
	}
	(*conn).Close()
}

func HandleRequest(buf *bytes.Buffer) {
	uuid, fcfi, ok := GetMeta(buf)
	if !ok {
		goto Corrupted
	}
	fmt.Println("TCP Message:\nUUID: ", uuid, "\nFCFI: ", fcfi)

	switch fcfi {
	case "REG_":
		puuid, ok := ReadString(buf, 16)
		if !ok {
			goto Corrupted
		}

		username, ok := ReadString(buf, -1)
		if !ok {
			goto Corrupted
		}

		player.Players = append(player.Players, player.NewPlayer(username, uuid, puuid))
		fmt.Print("Registered new player:\nUsername: ", username, "\nPrivate UUID: ", uuid, "\nPublic UUID: ", puuid)
	case "UREG":

	}
	return
Corrupted:
	fmt.Println("Data corrupted for TCP connection")
}
