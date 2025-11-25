using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MultiAssembly.Handlers
{
    internal class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(string fcfi) : base("Handler not found for FCFI" + fcfi) { 
        }
    }
    internal class NotEnoughDataForHandlerException : Exception
    {
        public NotEnoughDataForHandlerException(string fcfi) : base("Recieved stream does not contain the required amount of data for the input FCFI (" + fcfi + ")")
        {
        }
    }
    internal class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(string player) : base("Player not found (attempted to search for player " + player + ")")
        {
        }
    }
    internal static class Utility {
        public static string ReadFCFI(MemoryStream stream)
        {
            return Bit.ReadString(stream, 4);
        }
    }
}
