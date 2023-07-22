using UnityEditor;
using UnityEngine;

/// <summary>
/// The NoiseLayerDrawer is only responsible for customizing the layout and GUI for individual elements of the NoiseLayers array.
/// In this case, it prevents the foldout overlapping with the other elements(NoiseLayers) and not draw it correctly.
/// </summary>
[CustomPropertyDrawer(typeof(NoiseLayer))]
public class NoiseLayerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position.height = EditorGUIUtility.singleLineHeight;
        
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            
            // Draw individual properties of NoiseLayer
            var enabledProperty = property.FindPropertyRelative("Enabled");
            var useFirstLayerAsMaskProperty = property.FindPropertyRelative("UseFirstLayerAsMask");
            var noiseSettingsProperty = property.FindPropertyRelative("NoiseSettings");

            var rect = position;
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, enabledProperty);

            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, useFirstLayerAsMaskProperty);

            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, noiseSettingsProperty, true);

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded)
        {
            height += 3 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("NoiseSettings"), true);
        }

        return height;
    }
}