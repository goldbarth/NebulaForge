using System.Collections.Generic;
using HelpersAndExtensions;
using UnityEditor;
using UnityEngine;

namespace SolarSystem
{
    [ExecuteInEditMode]
    public class CelestialObjectManager : GenericSingleton<CelestialObjectManager>
    {
        [SerializeField] private List<CelestialObject> _celestialObjects = new();

        private readonly Dictionary<CelestialObject, List<string>> _celestialObjectAssets = new();
        public List<CelestialObject> CelestialObjects => _celestialObjects;
        
        
        public void AddCelestialObject(CelestialObject celestialObject)
        {
            if (!_celestialObjects.Contains(celestialObject))
            {
                _celestialObjects.Add(celestialObject);
                _celestialObjectAssets[celestialObject] = new List<string>();
            }
        }
        
#if UNITY_EDITOR
    
        public void RemoveCelestialObject(CelestialObject celestialObject)
        {
            if (_celestialObjects.Remove(celestialObject))
            {
                if (_celestialObjectAssets.TryGetValue(celestialObject, out var assetPaths))
                {
                    foreach (var path in assetPaths)
                    {
                        AssetDatabase.DeleteAsset(path);
                    }
                    
                    _celestialObjectAssets.Remove(celestialObject);
                }
            }
        }
        
#endif

        public void AddAssetPathToCelestialObject(CelestialObject celestialObject, string assetPath)
        {
            if (_celestialObjectAssets.TryGetValue(celestialObject, out var asset))
            {
                asset.Add(assetPath);
            }
            else
            {
                Debug.LogError("CelestialObjectManager: CelestialObject not found");
            }
        }
        
        public void ClearCelestialObjects()
        {
            _celestialObjects.Clear();
        }
    
        public CelestialObject GetCelestialObject(int index)
        {
            return _celestialObjects[index];
        }
        
        public int GetCelestialObjectIndex(CelestialObject celestialObject)
        {
            return _celestialObjects.IndexOf(celestialObject);
        }
        
    
        public string[] GetCelestialBodyNames()
        {
            var names = new string[_celestialObjects.Count];
            for (var i = 0; i < _celestialObjects.Count; i++)
            {
                names[i] = _celestialObjects[i].name;
            }
            
            return names;
        }
    }
}