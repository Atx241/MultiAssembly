package player

import (
	"sync"

	"atxmedia.us/multiassembly/util"
)

var Mutex sync.Mutex

type Player struct {
	Username    string
	PrivateUUID string
	PublicUUID  string
	Position    util.Vector3
	Rotation    util.Vector3
	Vehicle     []byte
}

var Players map[string]*Player = make(map[string]*Player, 0)

func (p Player) Remove() {
	RemoveByID(p.PrivateUUID)
}

func New(Username string, PrivateUUID string, PublicUUID string) *Player {
	p := Player{Username: Username, PrivateUUID: PrivateUUID, PublicUUID: PublicUUID}
	Players[p.PrivateUUID] = &p
	return &p

}
func GetByID(privateUUID string) *Player {
	return Players[privateUUID]
}
func RemoveByID(privateUUID string) {
	delete(Players, privateUUID)
}
