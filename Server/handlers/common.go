package handlers

import (
	"bytes"

	"atxmedia.us/multiassembly/handlers/bit"
)

const UUIDLength = 16

// Takes a buffer derived from a client request, and returns two strings and a boolean: one string corresponds to the private UUID of the sender and the other corresponds to the four-character function identifier (FCFI). The boolean indicates whether the data was successfully parsed or was corrupted.
func GetMeta(buf *bytes.Buffer) (string, string, bool) {
	uuid, ok := bit.ReadString(buf, UUIDLength)
	if !ok {
		return "", "", false
	}

	fcfi, ok := bit.ReadString(buf, 4)
	if !ok {
		return "", "", false
	}

	return string(uuid), string(fcfi), true
}
