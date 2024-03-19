# if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public struct LabelStyle
    {
        public static GUIStyle SetCenteredLabel()
        {
            return new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
        }
    
        public static GUIStyle SetCenteredBoldLabel()
        {
            return new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
        }
    
        public static GUIStyle SetCenteredMiniBoldLabel()
        {
            return new GUIStyle(EditorStyles.miniBoldLabel) { alignment = TextAnchor.MiddleCenter };
        }
    
        public static GUIStyle SetCenteredMiniLabel()
        {
            return new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
        }
    
        public static GUIStyle SetDefaultButtonStyle(float maxButtonWidth, float buttonBorderWith)
        {
            var defaultStyle = new GUIStyle(GUI.skin.button);
            defaultStyle.normal.textColor = Color.white;
            defaultStyle.fixedWidth = maxButtonWidth + buttonBorderWith;
        
            defaultStyle.normal.background = CreateColoredTexture(2, 2, Color.gray);
            defaultStyle.active.background = CreateColoredTexture(2, 2, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            return defaultStyle;
        }

        public static Texture2D CreateColoredTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (int colorsIndex = 0; colorsIndex < pixels.Length; colorsIndex++)
                pixels[colorsIndex] = color;
        
            var texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    
        public static float MaxButtonWidth(string buttonName)
        {
            return GUI.skin.button.CalcSize(new GUIContent(buttonName)).x;
        }
        
        /// <summary>
        /// If no color is provided, the default color is a dark gray.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Separation Line Style</returns>
        public static GUIStyle SeparationLineColor(Color color = default)
        {
            
            if (color == default)
                color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            var separationLine = new Texture2D(1, 1);
            separationLine.SetPixel(0,0, color);
            separationLine.Apply();
            var style = new GUIStyle { normal = { background = separationLine } };
            return style;
        }
    }
}

#endif