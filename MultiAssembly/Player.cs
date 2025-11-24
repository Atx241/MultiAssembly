using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiAssembly
{
    internal class Player
    {
        public static List<Player> Players = new List<Player>();

        public string Username;
        public string UUID;

        private GameObject gameObject;
        public Player(string uuid, string username)
        {
            UUID = uuid;
            Username = username;
            Players.Add(this);

            gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.transform.localScale = Vector3.one * 4;
            UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
        }

        public static Player? Find(string uuid)
        {
            foreach (Player p in Players)
            {
                if (p.UUID == uuid) {
                    return p;
                }
            }
            return null;
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        ~Player() {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
