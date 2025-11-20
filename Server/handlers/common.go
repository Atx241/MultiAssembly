package handlers

import (
	"bytes"
	"fmt"
)

const UUIDLength = 16

// Takes a buffer derived from a client request, and returns two strings and a boolean: one string corresponds to the private UUID of the sender and the other corresponds to the four-character function identifier (FCFI). The boolean indicates whether the data was successfully parsed or was corrupted.
func GetMeta(buf *bytes.Buffer) (string, string, bool) {
	uuid, ok := ReadString(buf, UUIDLength)
	if !ok {
		return "", "", false
	}

	fcfi, ok := ReadString(buf, 4)
	if !ok {
		return "", "", false
	}

	return string(uuid), string(fcfi), true
}

// NOTE: Using a size of -1 will read the rest of the buffer
func ReadString(buf *bytes.Buffer, size int) (string, bool) {
	_size := size
	if _size == -1 {
		_size = buf.Available()
	}

	str := make([]byte, _size)
	n, err := buf.Read(str)
	if err != nil {
		fmt.Println("Error reading string:", err.Error())
		return "", false
	}
	if size != -1 && n < _size {
		fmt.Print("Error reading string: not enough data (read ", n, " want ", _size, ")\n")
		return "", false
	}
	return string(str), true
}
