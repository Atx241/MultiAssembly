using System;
using BepInEx;
using UnityEngine.SceneManagement;

namespace MultiAssembly
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
            AppDomain.CurrentDomain.ProcessExit += OnExit;
            Username = Environment.UserName.Trim();
            print("PublicKey: " + UUID.LocalKP.Public);
            print("PrivateKey: " + UUID.LocalKP.Private);
            Run();
        }
        private void OnExit(object sender, EventArgs e)
        {
            Network.Disconnect();
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
            Network.SendUDP("PTUR", (double)player.transform.eulerAngles.x, (double)player.transform.eulerAngles.y, (double)player.transform.eulerAngles.z);
            Network.SendUDP("PTUP", (double)player.transform.position.x, (double)player.transform.position.y, (double)player.transform.position.z);
        }
    }
}
