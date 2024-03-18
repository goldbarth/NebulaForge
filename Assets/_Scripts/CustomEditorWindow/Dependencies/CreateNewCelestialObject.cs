using HelpersAndExtensions;
using SolarSystem;
using UnityEngine;
using Planet;
using PlanetSettings;
using PlanetSettings.NoiseSettings;

namespace CustomEditorWindow.Dependencies
{
    public class CreateNewCelestialObject
    {
        public bool IsAssetNameEmpty;

        private string _objectName = string.Empty;
        
        private float _radius = 60f;
        private Gradient _lineRendererGradient;

        public void CreateObject(string objectName = "")
        {
            _objectName = objectName;
            if (!string.IsNullOrEmpty(_objectName))
            {
                IsAssetNameEmpty = false;
                if (GameObject.Find(_objectName) != null)
                {
                    Debug.LogError("An object with the same name already exists.");
                    return;
                }
                
                if (GameObject.Find(_objectName) == null)
                {
                    // create new object in the scene and attach the celestial object script to it.
                    Debug.Log("Creating new celestial object with the name: " + _objectName);
                    var newObject = new GameObject(_objectName);
                    newObject.GetOrAddComponent2<Rigidbody>();
                    newObject.GetOrAddComponent2<CelestialObject>();
                    
                    var newColliderHolder = new GameObject("ColliderHolder");
                    newColliderHolder.transform.parent = newObject.transform;
                    newColliderHolder.GetOrAddComponent2<SphereCollider>().radius = _radius + 10f;
                    
                    var newMeshHolder = new GameObject("MeshHolder");
                    newMeshHolder.transform.parent = newObject.transform;
                    
                    // add the mesh renderer to the mesh holder.
                    var newMesh = new GameObject("Mesh");
                    newMesh.transform.parent = newMeshHolder.transform;
                    newMesh.GetOrAddComponent2<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    
                    // add a child object with the object generator script attached to it.
                    var newObjectGenerator = new GameObject("Surface");
                    newObjectGenerator.transform.parent = newObject.transform;
                    var newObjectSettings = ScriptableObject.CreateInstance<ObjectSettings>();
                    newObjectSettings.Radius = _radius;
                    newObjectGenerator.GetOrAddComponent2<ObjectGenerator>().ObjectSettings = newObjectSettings;
                    newObjectSettings.ObjectType = ObjectType.TerrestrialBody;
                    newObjectSettings.Resolution = 128;
                    newObjectSettings.Gradient = new Gradient { colorKeys = new[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) } };
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
                                    NoiseStrength = 0.5f,
                                    NumberOfLayers = 2,
                                    BaseRoughness = 0.5f,
                                    Roughness = 0.5f,
                                    Persistence = 0.5f
                                }
                            }
                        }
                    };
                    newObjectSettings.Material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    newObjectGenerator.GetOrAddComponent2<ObjectGenerator>();
                    newObjectGenerator.GetOrAddComponent2<ObjectGenerator>().ObjectSettings = newObjectSettings;
                    newObjectGenerator.GetOrAddComponent2<ObjectGenerator>().GenerateObject();
                    
                    // add a child object with a LineRenderer component attached to it.
                    _lineRendererGradient = new Gradient { colorKeys = new[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) } };
                    var newLineRenderer = new GameObject("Line");
                    newLineRenderer.transform.parent = newObject.transform;
                    newLineRenderer.GetOrAddComponent2<LineRenderer>().colorGradient = _lineRendererGradient;
                }
            }
            else
            {
                IsAssetNameEmpty = true;
            }
        }
    }
}