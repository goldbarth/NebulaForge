using UnityEditor;
using UnityEngine;

public struct LabelStyle
{
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
}