using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace MultiAssembly
{
    internal static class UI
    {
        public static Canvas? NetworkingCanvas;
        public static TextMeshProUGUI? DataText;

        public static Camera? MainCamera;

        public static void Initialize()
        {
            MainCamera = GameObject.FindObjectsByType<CameraController>(FindObjectsSortMode.None)[0].GetComponent<Camera>();
            NetworkingCanvas = GameObject.FindFirstObjectByType<FlyingUIController>().gameObject.GetComponent<Canvas>();
            DataText = new GameObject("ConnectionText", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();

            DataText.transform.SetParent(NetworkingCanvas.transform, false);
            DataText.rectTransform.anchorMin = new Vector2(1, 0.5f);
            DataText.rectTransform.anchorMax = DataText.rectTransform.anchorMin;
            DataText.rectTransform.anchoredPosition = new Vector2(-700, 0);
            DataText.rectTransform.sizeDelta = new Vector2(600, 200);
            DataText.horizontalAlignment = HorizontalAlignmentOptions.Right;
            DataText.text = "Connected to server";
            DataText.color = Color.green;
            DataText.fontSize = 18;
        }
        public static void Cleanup()
        {
            NetworkingCanvas = null;
            if (DataText != null) GameObject.Destroy(DataText);
        }
        public static void Loop()
        {
            foreach (Player p in Player.Players)
            {
                if (p.LabelTMP == null || MainCamera == null)
                {
                    continue;
                }
                var sp = MainCamera.WorldToScreenPoint(p.GetGameObject().transform.position);
                if (sp.z < 0)
                {
                    p.LabelTMP.alpha = 0;
                }
                else
                {
                    p.LabelTMP.alpha = 1;
                }
                p.LabelTMP.rectTransform.position = new Vector3(sp.x, sp.y, 10);
                p.LabelTMP.text = p.Username + "\n" + MathF.Floor(Vector3.Distance(p.GetGameObject().transform.position, MainCamera.transform.position)).ToString() + "M";
            }
        }
    }
}
