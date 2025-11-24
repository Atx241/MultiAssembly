package bit

import (
	"bytes"
	"fmt"
)

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
