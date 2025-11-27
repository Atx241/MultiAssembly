using System;
using BepInEx;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace MultiAssembly
{
    [BepInPlugin("atxmedia.globiassembly", "GlobiAssembly", "1.0.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        //Increment this for every major version (should only be incrememented at most every six months and only for large reworks or additions)
        public const int VersionMajor = 1;
        //Increment this for every minor version (should be incremented at most every month and only for important, but not essential reworks or additions)
        public const int VersionMinor = 0;
        //Increment this for every patch (patches are announced changes that don't fit into the major or minor categories)
        public const int VersionPatch = 0;

        //Increment this every time a non-announced/small change has been made
        public const int VersionInc = 1;

        private static TextMeshProUGUI? creditText;
        private static Canvas? menuCanvas;

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
                print(scene.name);
                if (scene.name == "Flying")
                {
                    GameLoad();
                } else if (scene.name == "Menu")
                {
                    menuCanvas = GameObject.Find("Menu Canvas").GetComponent<Canvas>();

                    if (menuCanvas == null)
                    {
                        print("Cannot find menu canvas");
                        return;
                    }

                    creditText = new GameObject("MultiAssemblyCreditText", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                    creditText.transform.parent = menuCanvas.transform;
                    creditText.rectTransform.anchorMin = Vector2.zero;
                    creditText.rectTransform.anchorMax = Vector2.zero;
                    creditText.rectTransform.anchoredPosition = new Vector2(700, 0);
                    creditText.rectTransform.sizeDelta = new Vector2(600, 200);
                    creditText.text = "Multiplayer powered by AtxMedia\nVersion " + VersionMajor + "." + VersionMinor + "." + VersionPatch + " inc " + VersionInc;
                    creditText.fontSize = 18;
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
