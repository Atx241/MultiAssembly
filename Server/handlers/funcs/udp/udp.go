package udpfuncs

import (
	"bytes"
	"fmt"

	"atxmedia.us/multiassembly/handlers/bit"
	"atxmedia.us/multiassembly/player"
	"atxmedia.us/multiassembly/util"
)

var funcs map[string]func(*player.Player, *bytes.Buffer) bool = make(map[string]func(*player.Player, *bytes.Buffer) bool)

func init() {
	funcs["PTUP"] = playerTransformUpdatePosition
	funcs["PTUR"] = playerTransformUpdateRotation
}

func Run(uuid string, fcfi string, buf *bytes.Buffer) bool {
	fn, ok := funcs[fcfi]
	if !ok {
		fmt.Println("Invalid FCFI for UDP function")
		return false
	}
	p := player.GetByID(uuid)
	if p == nil {
		return false
	}
	return fn(p, buf)
}

func playerTransformUpdatePosition(p *player.Player, buf *bytes.Buffer) bool {
	x, ok := bit.ReadFloat64(buf)
	if !ok {
		return false
	}
	y, ok := bit.ReadFloat64(buf)
	if !ok {
		return false
	}
	z, ok := bit.ReadFloat64(buf)
	if !ok {
		return false
	}
	p.Position = util.Vector3{X: x, Y: y, Z: z}
	return true
}
func playerTransformUpdateRotation(p *player.Player, buf *bytes.Buffer) bool {
	x, ok := bit.ReadFloat64(buf)
	if !ok {
		return false
	}
	y, ok := bit.ReadFloat64(buf)
	if !ok {
		return false
	}
	z, ok := bit.ReadFloat64(buf)
	if !ok {
		return false
	}
	p.Rotation = util.Vector3{X: x, Y: y, Z: z}
	return true
}
