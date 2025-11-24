package handlers

import (
	"bytes"
	"fmt"
	"net"

	"atxmedia.us/multiassembly/handlers/bit"
	"atxmedia.us/multiassembly/player"
)

func HandleConn(conn *net.Conn) {
	buf := make([]byte, 2048)
	size := 0
	var associatedPlayer *player.Player
	var err error
	for {
		size, err = (*conn).Read(buf)

		if err != nil {
			fmt.Println("TCP connection error occured: ", err.Error())
			fmt.Print("Closing connection for ", (*conn).RemoteAddr().String(), "...\n")
			break
		}

		HandleRequest(bytes.NewBuffer(buf[:size]), &associatedPlayer)
	}
	associatedPlayer.Remove()
	(*conn).Close()
}

func HandleRequest(buf *bytes.Buffer, aPlayer **player.Player) {
	uuid, fcfi, ok := GetMeta(buf)
	if !ok {
		goto Corrupted
	}
	fmt.Println("TCP Message:\nUUID: ", uuid, "\nFCFI: ", fcfi)

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

		fmt.Print("Registered new player:\nUsername: ", username, "\nPrivate UUID: ", uuid, "\nPublic UUID: ", puuid)
	case "UREG":
		player.RemoveByID(uuid)
	default:

	}
	return
Corrupted:
	fmt.Println("Data corrupted for TCP connection")
}
