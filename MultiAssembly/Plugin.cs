using System;
using BepInEx;
using MultiAssembly;
using UnityEngine.SceneManagement;

namespace GlobiAssembly
{
    [BepInPlugin("atxmedia.globiassembly", "GlobiAssembly", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static string Username = "N/A";

        public new void print(object obj)
        {
            Logger.LogInfo(obj);
        }
        private void Awake()
        {
            Username = Environment.UserName;
            print("PublicKey: " + UUID.LocalKP.Public);
            print("PrivateKey: " + UUID.LocalKP.Private);
            Run();
        }
        public void Run()
        {
            print("Hello, " + Username);
            
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Flying")
                {
                    GameLoad();
                }
            };

            SceneManager.sceneUnloaded += (scene) =>
            {
                if (scene.name == "Flying")
                {
                    GameUnload();
                }
            };
        }

        public void GameLoad()
        {
            if (!Network.IsInitialized())
            {
                Network.Initialize();
            }
            print("Game loaded!");
        }

        public void GameUnload()
        {
            print("Game unloaded!");
            if (Network.IsInitialized())
            {
                Network.Disconnect();
            }
        }

        public static void PlayerLoop()
        {
            PlaneContainer player = GameObjects.Player!;
            Network.SendUDP("PTUP", (double)player.transform.position.x, (double)player.transform.position.y, (double)player.transform.position.z);
            Network.SendUDP("PTUR", (double)player.transform.eulerAngles.x, (double)player.transform.eulerAngles.y, (double)player.transform.eulerAngles.z);
        }
    }
}
