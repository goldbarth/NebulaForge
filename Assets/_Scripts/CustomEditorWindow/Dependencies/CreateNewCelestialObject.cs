#if UNITY_EDITOR

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
        
        private const string Line = "LineRenderer";
        private const string Collider = "ColliderHolder";
        private const string MeshHolder = "MeshHolder";
        private const string Mesh = "Mesh";
        private const string ObjectGenerator = "Surface";
        
        public bool IsObjectNameEmpty;
        public bool IsObjectNameValid = true;

        public void CreateObject(ObjectType objectType, Gradient gradient, float radius, int resolution, float mass, float surfaceGravity, Vector3 initialVelocity, string objectName = "")
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
                    var newObject = new GameObject(objectName) { transform = {parent = OrbitSimulation.Instance.transform } };
                    newObject.GetOrAddComponent2<Rigidbody>().useGravity = false;
                    newObject.GetOrAddComponent2<CelestialObject>();
                    newObject.GetOrAddComponent2<CelestialObject>().Mass = mass;
                    newObject.GetOrAddComponent2<CelestialObject>().SurfaceGravity = surfaceGravity;
                    newObject.GetOrAddComponent2<CelestialObject>().InitialVelocity = initialVelocity;
                    
                    // add a child object with the collider holder script attached to it.
                    var newColliderHolder = new GameObject(Collider) { transform = { parent = newObject.transform } };
                    newColliderHolder.GetOrAddComponent2<SphereCollider>().radius = radius;
                    
                    // add a child object with the name mesh holder.
                    var newMeshHolder = new GameObject(MeshHolder) { transform = { parent = newObject.transform } };

                    // add a child object to the mesh holder with the mesh script attached to it.
                    var newMesh = new GameObject(Mesh) { transform = { parent = newMeshHolder.transform} };

                    // add a child object with the object generator script attached to it.
                    var newObjectGenerator = new GameObject(ObjectGenerator) { transform = { parent = newObject.transform } };

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
                    
                    var newLineRenderer = new GameObject(Line) { transform = { parent = newObject.transform } };
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

#endif