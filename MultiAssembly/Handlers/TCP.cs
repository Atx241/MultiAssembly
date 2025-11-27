using System;
using System.Collections.Generic;
using System.IO;

namespace MultiAssembly.Handlers
{
    internal static class TCP
    {
        public static Dictionary<string, Action<MemoryStream>> functions = new Dictionary<string, Action<MemoryStream>>();

        static TCP()
        {
            functions["REG_"] = registerPlayer;
            functions["UREG"] = unregisterPlayer;
        }

        public static void Run(string fcfi, MemoryStream stream)
        {
            if (!functions.TryGetValue(fcfi, out Action<MemoryStream> func)) {
                throw new HandlerNotFoundException(fcfi);
            }
            func(stream);
        }

        private static void registerPlayer(MemoryStream stream)
        {
            string uuid = Bit.ReadString(stream, UUID.UUIDLength);
            string username = Bit.ReadString(stream, stream.ReadByte());
            byte[] vehicle = new byte[stream.Length - stream.Position];
            stream.Read(vehicle, 0, vehicle.Length);
            Console.WriteLine("Vehicle length " + vehicle.Length);
            if (Player.Find(uuid) != null) return;
            Console.WriteLine("Registered new player:\nUUID: " + uuid + "\nUsername: \"" + username + "\"");
            Player.New(uuid, username, vehicle);
        }
        private static void unregisterPlayer(MemoryStream stream)
        {
            string uuid = Bit.ReadString(stream, UUID.UUIDLength);
            Console.WriteLine("Unregistered player:\nUUID: " + uuid);

            Player? p = Player.Find(uuid);
            if (p != null)
            {
                Player.Players.Remove(p);
            }
        }
    }
}
