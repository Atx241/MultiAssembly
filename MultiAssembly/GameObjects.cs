using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MultiAssembly
{
    internal static class GameObjects
    {
        public static PlaneContainer? Player;
        public static void PrintComponents(GameObject obj)
        {
            foreach (Component c in obj.GetComponents<Component>())
            {
                Console.WriteLine(c.GetType().ToString());
            }
        }
    }
}
