package handlers

import (
	"bytes"
	"errors"
	"fmt"
	"io"
	"net"
	"slices"

	"atxmedia.us/multiassembly/handlers/bit"
	"atxmedia.us/multiassembly/player"
)

var conns []*net.Conn

func Disconnect(conn *net.Conn, associatedPlayer *player.Player) {
	fmt.Print("Closing connection for ", (*conn).RemoteAddr().String(), "...\n")

	for i := 0; i < len(conns); i++ {
		if conns[i] == conn {
			conns = slices.Delete(conns, i, i+1)
		}
	}

	if associatedPlayer == nil {
		return
	}
	player.RemoveByID(associatedPlayer.PrivateUUID)
	for _, c := range conns {
		TCPWrite(c, bit.String("UREG"), bit.String((*associatedPlayer).PublicUUID), bit.String((*associatedPlayer).Username))
	}

	ClientsMutex.Lock()
	delete(clients, associatedPlayer.PrivateUUID)
	ClientsMutex.Unlock()

	(*conn).Close()
}

func HandleConn(conn *net.Conn) {
	var associatedPlayer *player.Player

	conns = append(conns, conn)
	for {
		msgLength, ok := bit.ReadUint16(bytes.NewBuffer(bit.TCPReadExactly(conn, 2)))
		if !ok {
			fmt.Println("TCP connection error occured")
			Disconnect(conn, associatedPlayer)
			break
		}
		if HandleRequest(bytes.NewBuffer(bit.TCPReadExactly(conn, int(msgLength))), &associatedPlayer, conn) {
			break
		}
	}
}

func HandleRequest(buf *bytes.Buffer, aPlayer **player.Player, conn *net.Conn) bool {
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

		unLength, err := buf.ReadByte()
		if err != nil {
			goto Corrupted
		}
		username, ok := bit.ReadString(buf, int(unLength))
		if !ok {
			goto Corrupted
		}
		vehicle, err := io.ReadAll(buf)
		if err != nil {
			goto Corrupted
		}

		(*aPlayer) = player.New(username, uuid, puuid)
		(*aPlayer).Vehicle = vehicle

		fmt.Print("Registered new player:\nUsername: ", username, "\nPrivate UUID: ", uuid, "\nPublic UUID: ", puuid, "\n")

		for _, c := range conns {
			err := TCPWrite(c, bit.String("REG_"), bit.String((*aPlayer).PublicUUID), []byte{byte(len((*aPlayer).Username))}, bit.String((*aPlayer).Username), (*aPlayer).Vehicle)
			if err != nil {
				fmt.Println(err.Error())
			}
		}
		for _, p := range player.Players {
			err := TCPWrite(conn, bit.String("REG_"), bit.String(p.PublicUUID), []byte{byte(len(p.Username))}, bit.String(p.Username), p.Vehicle)
			if err != nil {
				fmt.Println(err.Error())
			}
		}
	case "UREG":
		Disconnect(conn, *aPlayer)
		return true
	default:

	}
	return false
Corrupted:
	fmt.Println("Data corrupted for TCP connection")
	return false
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
