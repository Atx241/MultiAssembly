package bit

import "encoding/binary"

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

//Shorthand for merging byte arrays
func M(bytes ...[]byte) []byte {
	ret := make([]byte, 0)
	for _, b := range bytes {
		ret = append(ret, b...)
	}
	return ret
}
