package player

import "atxmedia.us/multiassembly/util"

type Player struct {
	Username    string
	PrivateUUID string
	PublicUUID  string
	Position    util.Vector3
	Rotation    util.Vector3
}

func NewPlayer(Username string, PrivateUUID string, PublicUUID string) Player {
	return Player{Username: Username, PrivateUUID: PrivateUUID, PublicUUID: PublicUUID}
}

var Players []Player = make([]Player, 0)
