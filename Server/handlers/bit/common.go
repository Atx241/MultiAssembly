package bit

//Shorthand for merging byte arrays
func M(bytes ...[]byte) []byte {
	ret := make([]byte, 0)
	for _, b := range bytes {
		ret = append(ret, b...)
	}
	return ret
}
