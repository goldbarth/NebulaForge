using System.Collections.Generic;
using HelpersAndExtensions;
using UnityEngine;

namespace SolarSystem
{
    public class CelestialObjectManager : GenericSingleton<CelestialObjectManager>
    {
        [SerializeField] private List<CelestialObject> _celestialObjects = new();
        
        public void AddCelestialObject(CelestialObject celestialObject)
        {
            _celestialObjects.Add(celestialObject);
        }
    
        public void RemoveCelestialObject(CelestialObject celestialObject)
        {
            _celestialObjects.Remove(celestialObject);
        }
    
        public CelestialObject GetCelestialObject(int index)
        {
            return _celestialObjects[index];
        }
        
        public int GetCelestialObjectIndex(CelestialObject celestialObject)
        {
            return _celestialObjects.IndexOf(celestialObject);
        }
        
        public CelestialObject SetCelestialObject(int index, CelestialObject celestialObject)
        {
            return _celestialObjects[index] = celestialObject;
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