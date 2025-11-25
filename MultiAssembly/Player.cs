using System;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

namespace MultiAssembly
{
    internal class Player
    {
        public static List<Player> Players = new List<Player>();

        public string Username;
        public string UUID;

        private GameObject gameObject;
        private Player(string uuid, string username)
        {
            UUID = uuid;
            Username = username;
            Players.Add(this);

            gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.transform.localScale = Vector3.one * 4;
            UnityEngine.Object.Destroy(gameObject.GetComponent<SphereCollider>());

            //GameObject unText = new GameObject();
            //unText.transform.SetParent(gameObject.transform, false);
            //unText.transform.localPosition = Vector3.up * 5;
            //TextMeshPro textComp = unText.AddComponent<TextMeshPro>();
            //textComp.text = Username;
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
        public static Player New(string uuid, string username)
        {
            Player ret = new Player(uuid, username);
            Players.Add(ret);
            Console.WriteLine("New player");
            return ret;
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void Destroy()
        {
            Players.Remove(this);
            UnityEngine.Object.Destroy(gameObject);
        }

        ~Player() {
            Destroy();
        }
    }
}
