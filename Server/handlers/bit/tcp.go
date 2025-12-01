package bit

import (
	"fmt"
	"net"
)

func TCPReadExactly(conn *net.Conn, length int) []byte {
	ret := make([]byte, 0, length)
	n := 0

	for n < length {
		tmpBuf := make([]byte, length-n)
		read, err := (*conn).Read(tmpBuf)
		if err != nil {
			fmt.Println("TCPReadExactly encountered a TCP error:", err.Error())
			return nil
		}
		n += read
		ret = append(ret, tmpBuf...)
	}
	return ret
}
