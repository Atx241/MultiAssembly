using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

namespace MultiAssembly.Handlers
{
    internal class UDP
    {
        public static Dictionary<string, Action<MemoryStream>> functions = new Dictionary<string, Action<MemoryStream>>();

        static UDP()
        {
            functions.Add("PTUP", playerTransformUpdatePosition);
            functions.Add("PTUR", playerTransformUpdateRotation);
        }

        public static void Run(string fcfi, MemoryStream stream)
        {
            if (!functions.TryGetValue(fcfi, out Action<MemoryStream> func))
            {
                throw new HandlerNotFoundException(fcfi);
            }
            func(stream);
        }

        private static void playerTransformUpdatePosition(MemoryStream stream)
        {
            string uuid = Bit.ReadString(stream, UUID.UUIDLength);
            float x = (float)Bit.ReadDouble(stream);
            float y = (float)Bit.ReadDouble(stream);
            float z = (float)Bit.ReadDouble(stream);
            Player? p = Player.Find(uuid);
            //Check for null transform because of Unity cleanup
            if (p == null || p.GetGameObject().transform == null)
            {
                return;
                //throw new PlayerNotFoundException(uuid);
            }
            p.GetGameObject().transform.position = new Vector3(x, y, z);
        }
        private static void playerTransformUpdateRotation(MemoryStream stream)
        {
            string uuid = Bit.ReadString(stream, UUID.UUIDLength);
            float x = (float)Bit.ReadDouble(stream);
            float y = (float)Bit.ReadDouble(stream);
            float z = (float)Bit.ReadDouble(stream);
            Player? p = Player.Find(uuid);
            if (p == null || p.GetGameObject().transform == null)
            {
                return;
                //throw new PlayerNotFoundException(uuid);
            }
            p.GetGameObject().transform.eulerAngles = new Vector3(x, y, z);
        }
    }
}
