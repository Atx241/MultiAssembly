package bit

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"math"
)

func ReadFloat64(buf *bytes.Buffer) (float64, bool) {
	f64 := make([]byte, 8)
	n, err := buf.Read(f64)
	if err != nil {
		fmt.Println("Error reading float64:", err.Error())
		return math.NaN(), false
	}
	if n < 8 {
		fmt.Print("Error reading float64: not enough data (read ", n, " want 8)\n")
		return math.NaN(), false
	}
	return math.Float64frombits(binary.LittleEndian.Uint64(f64)), true
}

func Float64(float float64) []byte {
	return Uint64(math.Float64bits(float))
}

func Uint64(u uint64) []byte {
	ret := make([]byte, 8)
	binary.LittleEndian.PutUint64(ret, u)
	return ret
}

func Uint16(u uint16) []byte {
	bit1 := byte(u)
	bit2 := byte(u >> 8)
	return []byte{bit1, bit2}
}

func ReadUint16(buf *bytes.Buffer) (uint16, bool) {
	u16 := make([]byte, 2)
	n, err := buf.Read(u16)
	if err != nil {
		fmt.Println("Error reading uint16:", err.Error())
		return 0, false
	}
	if n < 2 {
		fmt.Print("Error reading string: not enough data (read ", n, " want 2)\n")
		return 0, false
	}
	return binary.LittleEndian.Uint16(u16), true
}
