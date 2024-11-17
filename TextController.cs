using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnifromEngine
{
    public class TextController : MonoBehaviour
    {
        private static Canvas globalCanvas;

        public static TextMeshProUGUI CreateText(string text, string objectName = "UITextObject", float xOffset = 0, float yOffset = 0, int fontSize = 36, Color? color = null, float width = 500)
        {
            if (globalCanvas == null)
            {
                GameObject canvasObject = new GameObject("UnifromCanvas");

                globalCanvas = canvasObject.AddComponent<Canvas>();
                globalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();

                DontDestroyOnLoad(canvasObject);
            }
            
            GameObject UITextObject = new GameObject(objectName);

            TextMeshProUGUI textMesh = UITextObject.AddComponent<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color ?? Color.white;

            UITextObject.transform.SetParent(globalCanvas.transform);

            RectTransform rectTransform = textMesh.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(width, fontSize * 2);
            
            textMesh.enableWordWrapping = false;

            return textMesh;
        }
    }
}