using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MultiAssembly.Handlers
{
    internal static class TCP
    {
        public static Dictionary<string, Action<MemoryStream>> functions = new Dictionary<string, Action<MemoryStream>>();

        static TCP()
        {
            functions["REG_"] = registerPlayer;
        }

        public static void Run(string fcfi, MemoryStream stream)
        {
            Action<MemoryStream> func = functions[fcfi];
            if (func == null) {
                throw new HandlerNotFoundException(fcfi);
            }
            func(stream);
        }

        private static void registerPlayer(MemoryStream stream)
        {
            Console.WriteLine("Registered new player:\nUUID: " + Bit.ReadString(stream, UUID.UUIDLength) + "\nUsername: " + Bit.ReadString(stream, -1));
        }
    }
}
