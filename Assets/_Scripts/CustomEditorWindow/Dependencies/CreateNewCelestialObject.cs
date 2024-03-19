using PlanetSettings.NoiseSettings;
using HelpersAndExtensions;
using PlanetSettings;
using SolarSystem;
using UnityEngine;
using Planet;

namespace CustomEditorWindow.Dependencies
{
    public class CreateNewCelestialObject
    {
        private readonly CreateNewAsset _createNewAsset = new();
        
        public bool IsObjectNameEmpty;
        public bool IsObjectNameValid = true;

        public void CreateObject(ObjectType objectType, Gradient gradient, float radius, int resolution, string objectName = "")
        {
            if (!string.IsNullOrEmpty(objectName))
            {
                IsObjectNameEmpty = false;
                if (GameObject.Find(objectName) != null)
                {
                    Debug.LogError("An object with the same name already exists.");
                    return;
                }
                
                if (GameObject.Find(objectName) == null)
                {
                    IsObjectNameValid = true;

                    // create the new object
                    var newObject = new GameObject(objectName);
                    newObject.transform.parent = OrbitSimulation.Instance.transform;
                    newObject.GetOrAddComponent2<Rigidbody>();
                    newObject.GetOrAddComponent2<CelestialObject>();
                    
                    // add a child object with the collider holder script attached to it.
                    var newColliderHolder = new GameObject("ColliderHolder");
                    newColliderHolder.transform.parent = newObject.transform;
                    newColliderHolder.GetOrAddComponent2<SphereCollider>().radius = radius + 10f;
                    
                    var newMeshHolder = new GameObject("MeshHolder");
                    newMeshHolder.transform.parent = newObject.transform;
                    
                    // add a child object with the mesh script attached to it.
                    var newMesh = new GameObject("Mesh");
                    newMesh.transform.parent = newMeshHolder.transform;
                    
                    // add a child object with the object generator script attached to it.
                    var newObjectGenerator = new GameObject("Surface");
                    newObjectGenerator.transform.parent = newObject.transform;
                    
                    // attach the object settings
                    var newObjectSettings = ScriptableObject.CreateInstance<ObjectSettings>();
                    newObjectSettings.Resolution = resolution;
                    newObjectSettings.ObjectType = objectType;
                    newObjectSettings.Gradient = gradient;
                    newObjectSettings.Radius = radius;
                    
                    // add default noise layer. it is necessary to have at least one noise layer
                    // to avoid null reference exceptions and generate the object.
                    newObjectSettings.NoiseLayers = new[]
                    {
                        new NoiseLayer
                        {
                            Enabled = true,
                            NoiseSettings = new NoiseSettings
                            {
                                SimpleNoiseSettings = new SimpleNoiseSettings
                                {
                                    Seed = 0,
                                    NoiseStrength = 0f,
                                    NumberOfLayers = 1,
                                    BaseRoughness = 0f,
                                    Roughness = 0f,
                                    Persistence = 0f
                                }
                            }
                        }
                    };
                    
                    // create the asset, add the material and the object settings
                    var newAsset = _createNewAsset.CreateAsset(newObjectSettings, objectType, objectName);
                    
                    newMesh.GetOrAddComponent2<MeshRenderer>().material = newAsset.Material;
                    
                    newObjectGenerator.GetOrAddComponent2<ObjectGenerator>();
                    newObjectGenerator.GetOrAddComponent2<ObjectGenerator>().ObjectSettings = newAsset;
                    newObjectGenerator.GetOrAddComponent2<ObjectGenerator>().GenerateObject();
                    
                    var newLineRenderer = new GameObject("Line");
                    newLineRenderer.transform.parent = newObject.transform;
                    newLineRenderer.GetOrAddComponent2<LineRenderer>().colorGradient = gradient;
                }
            }
            else
            {
                Debug.LogError("Object name cannot be empty.");
                IsObjectNameEmpty = true;
            }
        }
    }
}