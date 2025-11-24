package bit

import "encoding/binary"

func Uint64(u uint64) []byte {
	ret := make([]byte, 8)
	binary.LittleEndian.PutUint64(ret, u)
	return ret
}

//Shorthand for merging byte arrays
func M(bytes ...[]byte) []byte {
	ret := make([]byte, 0)
	for _, b := range bytes {
		ret = append(ret, b...)
	}
	return ret
}
