#if UNITY_EDITOR

using System.Collections.Generic;
using HelpersAndExtensions;
using SolarSystem;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow
{
    [ExecuteInEditMode]
    public class CelestialObjectUpdater : MonoBehaviour
    {
        private CelestialObjectManager _celestialObjectManager;
    
        private void OnEnable()
        {
            _celestialObjectManager = CelestialObjectManager.Instance;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
    
        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        }

        private void OnHierarchyChanged()
        {
            
            
            // using a hash set to check if the objects are still in the scene. prevents duplicates and is faster.
            var sceneObjects = new HashSet<CelestialObject>(FindObjectsOfType<CelestialObject>());
            var objectsToRemove = new List<CelestialObject>();
            
            foreach (var obj in _celestialObjectManager.CelestialObjects)
            {
                if (!sceneObjects.Contains(obj))
                {
                    objectsToRemove.Add(obj);
                }
                else
                {
                    sceneObjects.Remove(obj);
                }
            }
            
            foreach (var obj in objectsToRemove)
            {
                _celestialObjectManager.RemoveCelestialObject(obj);
            }
            
            foreach (var obj in sceneObjects)
            {
                _celestialObjectManager.AddCelestialObject(obj);
                _celestialObjectManager.AddAssetPathToCelestialObject(obj, FolderPath.GetAssetFolder(obj.name));
            }
        }
    }
}

#endif