using System.Linq;
using UnityEngine;

public enum FaceRenderMask
{
    All,
    Top,
    Bottom,
    Left,
    Right,
    Front,
    Back
}

public enum ObjectType
{
    SolidSphere,
    TerrestrialBody,
}

[ExecuteInEditMode]
public class PlanetGenerator : MonoBehaviour
{
    [SerializeField,Space] private PlanetSettings _objectSettings;
    
    [Header("Material")]
    [SerializeField] private Material _objectMaterial;
    
    [Header("Visuals")]
    [SerializeField, Range(2, 255)] private int _resolution = 2;
    [SerializeField, Range(0, 10000)] private float _planetRadius = 10f;
    [SerializeField, Space] private Gradient _gradient;
    
    
    [Header("Planet Surface")]
    [SerializeField, Space(5)] private ObjectType _objectType;
    [SerializeField, Space(3)] private FaceRenderMask _faceRenderMask;
    
    [SerializeField, HideInInspector] private MeshRenderer[] _meshRenderer;
    [HideInInspector] public bool ShapeSettingsFoldout;
    public PlanetSettings ObjectSettings => _objectSettings;
    public Material ObjectMaterial => _objectMaterial;
    public Gradient Gradient => _gradient;
    public float PlanetRadius => _planetRadius;

    private readonly SurfaceElevationGradient _surfaceElevationGradient = new();
    private readonly SurfaceSettingsDispatcher _surfaceSettings = new();
    private readonly SurfaceShape _surfaceShape = new();
    
    private readonly int _cubeFaces = 6;

    private TerrainFace[] _terrainFaces;

    private void Awake()
    {
        UpdateSurfaceSettings();
        
        _terrainFaces = new TerrainFace[_cubeFaces];
        _meshRenderer = new MeshRenderer[_cubeFaces];
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
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
            _meshRenderer = new MeshRenderer[_cubeFaces];

        _terrainFaces = new TerrainFace[_cubeFaces];
        
        var directions = new[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        // create a new mesh renderer and mesh filter for each cube face
        for (var cubeFaceIndex = 0; cubeFaceIndex < _cubeFaces; cubeFaceIndex++)
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

                _terrainFaces[cubeFaceIndex] = new TerrainFace(_objectType, _surfaceShape, mesh, directions[cubeFaceIndex], _surfaceSettings);
            }
            
            _meshRenderer[cubeFaceIndex].sharedMaterial = _objectMaterial;
            
            var renderFace = _faceRenderMask == FaceRenderMask.All || (int) _faceRenderMask - 1 == cubeFaceIndex;
            _meshRenderer[cubeFaceIndex].enabled = renderFace;
        }
    }
    
    private void UpdateSurfaceSettings()
    {
        _surfaceElevationGradient.UpdateSettings(this);
        _surfaceShape.UpdateSettings(this, _objectSettings);
        UpdateObjectSettings();
        UpdateGradientColors();
    }

    public void OnPlanetSettingsUpdated()
    {
        GenerateMesh();
    }
            
    public void OnColorSettingsUpdated()
    {
        UpdateGradientColors();
    }
    
    private void GenerateMesh()
    {
        if(_terrainFaces == null) return;

        for (int pageIndex = 0; pageIndex < _cubeFaces; pageIndex++)
        {
            if (_meshRenderer[pageIndex].enabled)
                _terrainFaces[pageIndex].GenerateSphereMesh(_resolution);
        }
        
        UpdateObjectSettings();
        _surfaceElevationGradient.UpdateElevation(_surfaceShape.ElevationMinMax);
    }

    private void UpdateGradientColors()
    {
        _surfaceElevationGradient.UpdateGradient();
    }
    
    private void UpdateObjectSettings()
    {
        _surfaceSettings.UpdateObjectSettings(_objectType, _surfaceShape);
    }
}