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

        public static void Initialize()
        {
            NetworkingCanvas = GameObject.FindFirstObjectByType<FlyingUIController>().gameObject.GetComponent<Canvas>();
            DataText = new GameObject("NetworkingCanvas", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();

            DataText.transform.parent = NetworkingCanvas.transform;
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
    }
}
