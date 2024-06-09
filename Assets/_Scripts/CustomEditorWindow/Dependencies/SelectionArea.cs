#if UNITY_EDITOR

using SolarSystem;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow.Dependencies
{
    public class SelectionArea
    {
        private readonly CelestialObjectManager _celestialObjectManager = CelestialObjectManager.Instance;
        
        private string[] _planetNamesArray;
        private int _selectedPlanetIndex;
        private int _currentPlanetIndex;
        
        public void DrawSelectionArea()
        {
            SynchronizeDropdownWithSelection();
            DrawSelectionField();
            UpdateSelectedGameObject();
        }
        
        private void DrawSelectionField()
        {
            _planetNamesArray = _celestialObjectManager.GetCelestialBodyNames();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _selectedPlanetIndex = EditorGUILayout.Popup(TextHolder.SelectionHeader, 
                                                            _currentPlanetIndex, _planetNamesArray, GUILayout.Width(300));
            GUILayout.EndHorizontal();
        }

        private void UpdateSelectedGameObject()
        {
            if (_selectedPlanetIndex != _currentPlanetIndex)
            {
                _currentPlanetIndex = _selectedPlanetIndex;
                var selectedBody = _celestialObjectManager.GetCelestialObject(_currentPlanetIndex);
                Selection.activeGameObject = selectedBody.gameObject;
            }
        }
        
        private void SynchronizeDropdownWithSelection()
        {
            var currentSelection = Selection.activeGameObject;
            if (currentSelection != null)
            {
                var currentCelestialObject = currentSelection.GetComponent<CelestialObject>();
                if (currentCelestialObject != null)
                {
                    var newIndex = _celestialObjectManager.GetCelestialObjectIndex(currentCelestialObject);
                    if (newIndex != -1 && newIndex != _currentPlanetIndex)
                    {
                        _currentPlanetIndex = newIndex;
                    }
                }
            }
        }
    }
}

#endif