using UnityEditor;
using UnityEngine;
using Planet;

namespace CustomEditorWindow.Dependencies
{
    public class CreateNewCelestialObjectWindow : View
    {
        private string _objectName = string.Empty;
        
        private ObjectGenerator _objectGenerator;
        private WindowLayout _layout;
        
        private readonly CreateNewCelestialObject _createNewCelestialObject = new();
        
        private void OnEnable()
        {
            _layout = CreateInstance<WindowLayout>();
            _objectGenerator = _layout.ObjectGenerator;
        }
        
        public static void ShowWindow()
        {
            var window = GetWindow<CreateNewCelestialObjectWindow>(TextHolder.CreateAssetWindowHeader);
            window.minSize = new Vector2(400, 450);
            window.maxSize = new Vector2(400, 450);
        }
        
        private void OnGUI()
        {
            const float buttonBorderWidth = 15f;
            EditorGUILayout.LabelField("Enter a name for the new celestial object:");
            EditorGUILayout.Space(10);
            _objectName = EditorGUILayout.TextField("Celestial Object Name", _objectName);
            EditorGUILayout.Space(7);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(TextHolder.CreateButtonName, LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(TextHolder.CreateButtonName), buttonBorderWidth)))
                _createNewCelestialObject.CreateObject(_objectName);
            GUILayout.EndHorizontal();
        
            if (_createNewCelestialObject.IsAssetNameEmpty)
                EditorGUILayout.HelpBox("The name can´t be empty! " +
                                        "You need to enter a name for the celestial object.", MessageType.Warning);
        
        
            // if (!_createNewCelestialObject.IsAssetNameValid)
            //     EditorGUILayout.HelpBox("An asset with the same name already exists. " +
            //                             "Please enter a new name.", MessageType.Warning);

            GUILayout.FlexibleSpace();
        
            EditorGUILayout.HelpBox("For now, the new created asset is a copy of the current selected asset. " +
                                    "In the future it will be possible to define properties. Stay tuned. (ﾉ◕ヮ◕)ﾉ*:・ﾟ✧", MessageType.Info, true);
        
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(TextHolder.CloseButtonName, LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(TextHolder.CloseButtonName), buttonBorderWidth)))
                Close();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}