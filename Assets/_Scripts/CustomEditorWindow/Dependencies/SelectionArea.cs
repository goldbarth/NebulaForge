using System.Linq;
using SolarSystem;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class SelectionArea
    {
        private readonly WindowLayout _layout;
        private string[] _planetNamesArray;
        private int _currentPlanetIndex;

        public SelectionArea(WindowLayout layout)
        {
            _layout = layout;
        }
        
        public void DrawSelectionArea()
        {
            DrawSelectionField();
        }
        
        private void DrawSelectionField()
        {
            var celestialObjectManager = CelestialObjectManager.Instance;
            _planetNamesArray = celestialObjectManager.GetCelestialBodyNames();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TextHolder.SelectionHeader);
            var selectedPlanetIndex = EditorGUILayout.Popup(_currentPlanetIndex, _planetNamesArray);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
    
            if (selectedPlanetIndex != _currentPlanetIndex)
            {
                _currentPlanetIndex = selectedPlanetIndex;
                var selectedBody = celestialObjectManager.GetCelestialObject(_currentPlanetIndex);
                Selection.activeGameObject = selectedBody.gameObject;
            }
        }
    }
}