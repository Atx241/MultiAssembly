using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;
//using TMPro;

namespace MultiAssembly
{
    internal class Player
    {
        public static List<Player> Players = new List<Player>();

        public string Username;
        public string UUID;

        private GameObject gameObject;

        public void CleanParts(object obj)
        {
            var part = (GameObject)obj;
            //Run twice in case of dependencies
            for (int rpt = 0; rpt < 2; rpt++)
            {
                Component[] components = part.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    Component comp = components[i];
                    //if (!(comp is MeshRenderer) && !(comp is MeshFilter) && !(comp is Transform) && !(comp is PaintableRenderer) && !(comp is WingVisual))
                    if ((comp is Collider) || (comp is Wheel))
                    {
                        if (comp is Behaviour b)
                        {
                            b.enabled = false;
                        }
                        else
                        {
                            GameObject.Destroy(comp);
                        }
                    }
                }
            }
            foreach (Transform c in part.transform)
            {
                CleanParts(c.gameObject);
            }
        }
        private Player(string uuid, string username, MemoryStream vehicle)
        {
            UUID = uuid;
            Username = username;
            Players.Add(this);

            //gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //gameObject.transform.localScale = Vector3.one * 4;
            //UnityEngine.Object.Destroy(gameObject.GetComponent<SphereCollider>());
            gameObject = new GameObject();

            while (vehicle.Position < vehicle.Length)
            {
                var name = Bit.ReadString(vehicle, vehicle.ReadByte());
                var px = Bit.ReadFloat(vehicle);
                var py = Bit.ReadFloat(vehicle);
                var pz = Bit.ReadFloat(vehicle);
                var rx = Bit.ReadFloat(vehicle);
                var ry = Bit.ReadFloat(vehicle);
                var rz = Bit.ReadFloat(vehicle);
                Console.WriteLine("Create player part " + name);
                if (name == "Body")
                {
                    continue;
                }
                GameObject child = MonoBehaviour.Instantiate(PartPrefabs.GetPartPrefab(name));

                child.transform.parent = gameObject.transform;

                child.transform.localPosition = new Vector3(px, py, pz);
                child.transform.localEulerAngles = new Vector3(rx, ry, rz);

                Thread t = new Thread(CleanParts);
                t.Start(child);
                t.Join();
            }
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
        public static Player New(string uuid, string username, byte[] vehicle)
        {
            Player ret = new Player(uuid, username, new MemoryStream(vehicle));
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
