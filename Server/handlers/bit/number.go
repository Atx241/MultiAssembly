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
		fmt.Print("Error reading string: not enough data (read ", n, " want 8)\n")
		return math.NaN(), false
	}
	return math.Float64frombits(binary.LittleEndian.Uint64(f64)), true
}

func Float64(float float64) []byte {
	return Uint64(math.Float64bits(float))
}
