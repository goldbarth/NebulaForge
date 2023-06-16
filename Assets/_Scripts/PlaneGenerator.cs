using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneGenerator : MonoBehaviour
{
    [SerializeField] private Material _meshMaterial;
    [Range(2, 255)] [SerializeField] private int _resolution;
    [SerializeField] private float _planeSize;
    [SerializeField] private Vector2 _noiseCenter;
    [SerializeField] private float _noiseScale;
    [SerializeField] private float _noiseStrength;
    private Mesh _mesh;
    private MeshFilter _meshFilter;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _mesh = new Mesh { name = "PlaneGeneratorMesh" };
        _meshRenderer.sharedMaterial = _meshMaterial;
        _meshFilter.sharedMesh = _mesh;
    }

    private void Update()
    {
        GeneratePlane();
    }

    private void GeneratePlane()
    {
        var vertices = new Vector3[_resolution * _resolution];
        //(resolution - 1) * (resolution - 1) = Anzahl der Quads 
        // *2 = Anzahl der Triangles pro Quad
        // *3 = Anzahl der Vertices pro Triangle
        var triangles = new int[(_resolution - 1) * (_resolution - 1) * 2 * 3];

        var startPosition = (Vector3.left + Vector3.back) * (_planeSize * 0.5f);
        var triangleIndex = 0;
        for (int y = 0, i = 0; y < _resolution; y++)
        for (var x = 0; x < _resolution; x++, i++)
        {
            var percent = new Vector2(x, y) / (_resolution - 1);
            var vertexPosition = startPosition + (Vector3.right * percent.x + Vector3.forward * percent.y) * _planeSize;

            // Noise
            var noisePosition = _noiseCenter + new Vector2(vertexPosition.x, vertexPosition.z) * _noiseScale;
            var vertNoisePosition = vertexPosition +
                                    Vector3.up * (Mathf.PerlinNoise(noisePosition.x, noisePosition.y) * _noiseStrength);
            vertices[i] = vertNoisePosition;

            if (x < _resolution - 1 && y < _resolution - 1)
            {
                // Vertex kann ein Quad generieren
                triangles[triangleIndex] = i;
                triangles[triangleIndex + 1] = i + _resolution;
                triangles[triangleIndex + 2] = i + _resolution + 1;

                triangles[triangleIndex + 3] = i;
                triangles[triangleIndex + 4] = i + _resolution + 1;
                triangles[triangleIndex + 5] = i + 1;

                // +6 weil wir 2 Triangles generiert haben
                triangleIndex += 6;
            }
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
    }
}