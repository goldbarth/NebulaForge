using System.Linq;
using UnityEngine;

public enum ObjectType
{
    SolidSphere,
    TerrestrialBody,
}
/// <summary>
/// The ObjectGenerator is a class to generate planets and other objects.
/// It is the main class of the project and communicates tightly with the ObjectSettings.
/// </summary>
[ExecuteAlways, ImageEffectAllowedInSceneView]
public class ObjectGenerator : MonoBehaviour
{
    [field: SerializeField, Header("Asset Debug Field")] public ObjectSettings ObjectSettings { get; set; }

    [SerializeField, HideInInspector] private MeshRenderer[] _meshRenderer;

    [HideInInspector] public bool ShapeSettingsFoldout;
    
    public SurfaceShapeDispatcher SurfaceShapeDispatcher { get; } = new();
    public SurfaceShape SurfaceShape { get; } = new();
    public NoiseLayer[] NoiseLayers => ObjectSettings.NoiseLayers;
    public ObjectType ObjectType => ObjectSettings.ObjectType;
    public Gradient Gradient => ObjectSettings.Gradient;
    public Material Material => ObjectSettings.Material;
    public float Radius => ObjectSettings.Radius;

    private const int CubeFaces = 6;
    
    private readonly SurfaceElevationGradient _surfaceElevationGradient = new();

    private TerrainFace[] _terrainFaces;

    private void Awake()
    {
        _terrainFaces = new TerrainFace[CubeFaces];
        _meshRenderer = new MeshRenderer[CubeFaces];
    }

    private void Start()
    {
        GeneratePlanet();
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        UpdateElevationGradient();
    }

    // First search attempts: https://stackoverflow.com/questions/38120084/how-can-we-destroy-child-objects-in-edit-modeunity3d
    // and https://stackoverflow.com/questions/1211608/possible-to-iterate-backwards-through-a-foreach
    // Sources: https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.cast?view=net-7.0
    // https:https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.reverse?view=net-7.0
    public void RemovePlanet()
    {
        // we using linq cast to avoid allocate memory. we also want to use reverse to destroy child objects in reverse order
        // to prevent to destroy only even numbered child objects(as in my testing). preventing a double foreach with a list.
        var children = transform.Cast<Transform>().Reverse();
        foreach (var child in children)
            DestroyImmediate(child.gameObject);
    }

    private void Initialize()
    {
        UpdateSurfaceSettings();

        if (_meshRenderer == null || _meshRenderer.Length == 0)
            _meshRenderer = new MeshRenderer[CubeFaces];

        _terrainFaces = new TerrainFace[CubeFaces];

        // cube face directions
        var directions = new[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        
        // create a new mesh renderer and mesh filter for each cube face
        for (var cubeFaceIndex = 0; cubeFaceIndex < CubeFaces; cubeFaceIndex++)
        {
            if (_terrainFaces[cubeFaceIndex] == null)
            {
                // create a new gameobject for each cube face
                // if we have a child object with the same name, we use it. otherwise we create a new one. this is to prevent to create more than 6 child objects.
                var newFace = cubeFaceIndex < transform.childCount ? transform.GetChild(cubeFaceIndex).gameObject : new GameObject($"TerrainFace_{cubeFaceIndex}");
                newFace.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                newFace.transform.SetParent(transform);

                // add mesh renderer, mesh filter components to the child object.
                _meshRenderer[cubeFaceIndex] = newFace.GetOrAddComponent2<MeshRenderer>();
                var meshFilter = newFace.GetOrAddComponent2<MeshFilter>();
                var mesh = new Mesh { name = $"TerrainMesh_{cubeFaceIndex}" };
                meshFilter.sharedMesh = mesh;

                _terrainFaces[cubeFaceIndex] = new TerrainFace(this, mesh, directions[cubeFaceIndex]);
            }
            
            _meshRenderer[cubeFaceIndex].sharedMaterial = Material;
            
            //var renderFace = ObjectSettings.FaceRenderMask == FaceRenderMask.All || (int) ObjectSettings.FaceRenderMask - 1 == cubeFaceIndex;
            //_meshRenderer[cubeFaceIndex].enabled = renderFace;
        }
    }
    
    private void GenerateMesh()
    {
        if(_terrainFaces == null) return;

        for (int pageIndex = 0; pageIndex < CubeFaces; pageIndex++)
        {
            if (_meshRenderer[pageIndex].enabled)
                _terrainFaces[pageIndex].GenerateSphereMesh(ObjectSettings.Resolution);
        }
        
        UpdateSurfaceShapeDispatcherSettings();
        UpdateElevation();
    }

    private void UpdateSurfaceSettings()
    {
        UpdateSurfaceElevationGradientSettings();
        UpdateSurfaceShapeDispatcherSettings();
        UpdateSurfaceShapeSetting();
        UpdateElevationGradient();
    }

    public void OnPlanetSettingsUpdated()
    {
        GenerateMesh();
    }
            
    public void OnColorSettingsUpdated()
    {
        UpdateElevationGradient();
    }
    
    private void UpdateSurfaceShapeSetting()
    {
        SurfaceShape.UpdateSettings(this);
    }

    private void UpdateSurfaceElevationGradientSettings()
    {
        _surfaceElevationGradient.UpdateSettings(this);
    }

    private void UpdateElevation()
    {
        _surfaceElevationGradient.UpdateElevation(SurfaceShape.ElevationMinMax);
    }

    private void UpdateElevationGradient()
    { 
        _surfaceElevationGradient.UpdateGradient();
    }
    
    private void UpdateSurfaceShapeDispatcherSettings()
    {
        SurfaceShapeDispatcher.UpdateSettings(this);
    }
}