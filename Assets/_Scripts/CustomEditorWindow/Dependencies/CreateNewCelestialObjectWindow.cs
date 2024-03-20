using UnityEditor;
using UnityEngine;
using Planet;

namespace CustomEditorWindow.Dependencies
{
    public class CreateNewCelestialObjectWindow : View
    {
        private string _objectName = string.Empty;
        private Vector3 _initialVelocity = new(5f, 0.0f, 0f);
        private int _resolution = 128;
        private float _radius = 60f;
        private float _mass = 1000f;
        
        private Gradient _gradient = new();
        private Texture2D _separationLine;
        private ObjectType _objectType;

        private readonly CreateNewCelestialObject _createNewCelestialObject = new();
        
        public static void ShowWindow()
        {
            var window = GetWindow<CreateNewCelestialObjectWindow>(TextHolder.CreateObjectWindowHeader);
            window.minSize = new Vector2(280, 450);
            window.maxSize = new Vector2(280, 450);
        }
        
        private void OnGUI()
        {
            const float buttonBorderWidth = 15f;
            
            GUILayout.Label("", LabelStyle.SeparationLineColor(), GUILayout.Height(1));
            EditorGUILayout.Space(4);
            _objectName = EditorGUILayout.TextField("Name:", _objectName);
            EditorGUILayout.Space(2);
            GUILayout.Label("", LabelStyle.SeparationLineColor(), GUILayout.Height(1));
            EditorGUILayout.Space(4);
            _objectType = (ObjectType) EditorGUILayout.EnumPopup("Type:", _objectType);
            EditorGUILayout.Space(2);
            GUILayout.Label("", LabelStyle.SeparationLineColor(), GUILayout.Height(1));
            EditorGUILayout.Space(4);
            _resolution = EditorGUILayout.IntField("Resolution:", _resolution);
            EditorGUILayout.Space(2);
            GUILayout.Label("", LabelStyle.SeparationLineColor(), GUILayout.Height(1));
            EditorGUILayout.Space(4);
            _radius = EditorGUILayout.FloatField("Radius:", _radius);
            EditorGUILayout.Space(2);
            GUILayout.Label("", LabelStyle.SeparationLineColor(), GUILayout.Height(1));
            EditorGUILayout.Space(4);
            _gradient = EditorGUILayout.GradientField("Gradient:", _gradient);
            EditorGUILayout.Space(2);
            GUILayout.Label("", LabelStyle.SeparationLineColor(), GUILayout.Height(1));
            EditorGUILayout.Space(4);
            _mass = EditorGUILayout.FloatField("Mass:", _mass);
            EditorGUILayout.Space(2);
            GUILayout.Label("", LabelStyle.SeparationLineColor(), GUILayout.Height(1));
            EditorGUILayout.Space(4);
            _initialVelocity = EditorGUILayout.Vector3Field("Initial Velocity:", _initialVelocity);
            EditorGUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(TextHolder.CreateButtonName, LabelStyle.SetDefaultButtonStyle(LabelStyle.MaxButtonWidth(TextHolder.CreateButtonName), buttonBorderWidth)))
                _createNewCelestialObject.CreateObject(_objectType, _gradient, _radius, _resolution, _mass, _initialVelocity, _objectName);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(4);
            
            if (_objectType == ObjectType.TerrestrialBody)
                EditorGUILayout.HelpBox("The Terrestrial Body is good for eg. a planets with elevation (and habitat)." + 
                                        "The surface can be modified and adjusted at the 'Surface' tab in the editor.", MessageType.Info);
            
            if (_objectType == ObjectType.SolidSphere)
                EditorGUILayout.HelpBox("The Solid Sphere is good for eg. plain surfaces (and using shaders for water and clouds for example)." + 
                                        "There is no elevation to modify. The gradient choice converts into the surface color.", MessageType.Info);
        
            if (_createNewCelestialObject.IsObjectNameEmpty)
                EditorGUILayout.HelpBox("The name can´t be empty! " +
                                        "You need to enter a name for the celestial object.", MessageType.Warning);
        
            if (!_createNewCelestialObject.IsObjectNameValid)
                EditorGUILayout.HelpBox("An asset with the same name already exists. " +
                                        "Please enter a new name.", MessageType.Warning);

            GUILayout.FlexibleSpace();
        
            EditorGUILayout.HelpBox("For now, the celestial object creation is in development. It is possible to run into bugs. " +
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