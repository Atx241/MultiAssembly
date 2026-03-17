using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;
using System.Collections;
using TMPro;
using System.Reflection.Emit;
using System.Text;
//using TMPro;

namespace MultiAssembly
{
    internal class Player
    {
        public static List<Player> Players = new List<Player>();

        public string Username;
        public string UUID;

        public TextMeshProUGUI? LabelTMP;

        private GameObject gameObject;

        public void CleanParts(object obj)
        {
            var part = (GameObject)obj;
            Component[] components = part.GetComponents<Component>();
            List<Component> componentsToDestroy = new List<Component>();

            foreach (var comp in components)
            {
                if ((comp is Collider) || (comp is Wheel))
                {
                    componentsToDestroy.Add(comp);
                }
            }

            foreach (var comp in componentsToDestroy)
            {
                GameObject.Destroy(comp);
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

            gameObject = new GameObject("Player" + UUID, typeof(Rigidbody));
            gameObject.GetComponent<Rigidbody>().isKinematic = true;

            while (vehicle.Position < vehicle.Length)
            {
                var name = Bit.ReadString(vehicle, vehicle.ReadByte());
                GameObject prefab = PartPrefabs.GetPartPrefab(name);
                if (prefab == null)
                {
                    Console.WriteLine("Invalid vehicle for " + uuid + ", aborting creation (last part name:" + name + ". Next 64 bytes: [" + Encoding.UTF8.GetString(Bit.ReadExactly(vehicle, 64)) + "])");
                    break;
                }
                var px = Bit.ReadFloat(vehicle);
                var py = Bit.ReadFloat(vehicle);
                var pz = Bit.ReadFloat(vehicle);
                var rx = Bit.ReadFloat(vehicle);
                var ry = Bit.ReadFloat(vehicle);
                var rz = Bit.ReadFloat(vehicle);
                Console.WriteLine("Create player part " + name);
                GameObject child = GameObject.Instantiate(prefab);

                child.transform.parent = gameObject.transform;

                child.transform.localPosition = new Vector3(px, py, pz);
                child.transform.localEulerAngles = new Vector3(rx, ry, rz);

                if (name == "Body")
                {
                    var radius1 = Bit.ReadVector2(vehicle);
                    var radius2 = Bit.ReadVector2(vehicle);
                    var lengthOffset1 = Bit.ReadVector3(vehicle);
                    var lengthOffset2 = Bit.ReadVector3(vehicle);
                    var roundness1 = Bit.ReadVector4(vehicle);
                    var roundness2 = Bit.ReadVector4(vehicle);
                    var pFuselage = child.GetComponent<ProceduralFuselage>();
                    pFuselage.appliedTransform.side1 = new ProceduralFuselageSide();
                    pFuselage.appliedTransform.side2 = new ProceduralFuselageSide();

                    pFuselage.appliedTransform.side1.radius = radius1;
                    pFuselage.appliedTransform.side2.radius = radius2;

                    pFuselage.appliedTransform.side1.lengthOffset = lengthOffset1;
                    pFuselage.appliedTransform.side2.lengthOffset = lengthOffset2;

                    pFuselage.appliedTransform.side1.roundness = roundness1;
                    pFuselage.appliedTransform.side2.roundness = roundness2;

                    child.GetComponent<ProceduralFuselageMesh>().UpdateMesh(pFuselage.AppliedTransform);


                }

                //Thread t = new Thread(CleanParts);
                //t.Start(child);
                //t.Join();
                CleanParts(child);
            }
            initUI();
        }
        private void initUI()
        {
            LabelTMP = new GameObject("PLabel" + UUID, typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            LabelTMP.transform.SetParent(UI.NetworkingCanvas!.transform, false);
            LabelTMP.text = Username;
            LabelTMP.rectTransform.anchorMin = new Vector2(0, 0);
            LabelTMP.rectTransform.anchorMax = LabelTMP.rectTransform.anchorMin;
            LabelTMP.rectTransform.anchoredPosition = new Vector2(0, 0);
            LabelTMP.rectTransform.sizeDelta = new Vector2(600, 200);
            LabelTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
            LabelTMP.color = Color.cyan;
            LabelTMP.fontSize = 30;
            LabelTMP.fontWeight = FontWeight.Heavy;
        }

        public static Player? Find(string uuid)
        {
            foreach (Player p in Players)
            {
                if (p.UUID == uuid)
                {
                    return p;
                }
            }
            return null;
        }
        //NOTE: Ensure that this runs on the main thread, otherwise it has a chance to crash the game.
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
            UnityEngine.Object.Destroy(LabelTMP);
        }

        ~Player()
        {
            Destroy();
        }
    }
}
