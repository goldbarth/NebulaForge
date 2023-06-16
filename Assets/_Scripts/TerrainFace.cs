using UnityEngine;

public class TerrainFace
{
    private const int PlaneSize = 2;
    
    private readonly SurfaceShape _surfaceShape;
    private readonly Mesh _mesh;
    private readonly Vector3 _upDirection;
    private readonly Vector3 _axisA;
    private readonly Vector3 _axisB;

    public TerrainFace(SurfaceShape surfaceShape, Mesh mesh, Vector3 upDirection)
    {
        _mesh = mesh;
        _upDirection = upDirection;
        _surfaceShape = surfaceShape;
        _axisA = new Vector3(upDirection.y, upDirection.z, upDirection.x);
        _axisB = Vector3.Cross(_axisA, _upDirection);
    }

    public void GenerateSphereMesh(int resolution)
    {
        var vertices = new Vector3[resolution * resolution];
        var uv = new Vector2[resolution * resolution];
        var triangles = new int[(resolution - 1) * (resolution - 1) * 6]; // 6 bc we have 2 triangles(2 * 3 vertex) per quad
        var startPosition = _upDirection - _axisA - _axisB;

        var triangleIndex = 0;
        for (int y = 0, index = 0; y < resolution; y++)
        for (var x = 0; x < resolution; x++, index++)
        {
            // calculating the normalized position of each vertex and uv coordinate for consistent mapping
            var percent = new Vector2(x, y) / (resolution - 1); 
            // calculating the position of each vertex on the unit cube
            var cubePosition = startPosition + (_axisA * percent.x + _axisB * percent.y) * PlaneSize;
            // calculating the position of each vertex on the unit sphere, instead of normalizing the cube position
            var spherePosition = _surfaceShape.CalculateSeamlessEdges(cubePosition);
            // calculating the position of each vertex on the planet surface
            var planetPosition = spherePosition + _surfaceShape.CalculateSphereSurface(spherePosition);
            
            vertices[index] = planetPosition; // assign vertex position
            uv[index] = new Vector2(percent.x, percent.y); // assign UV coordinates

            if (x < resolution - 1 && y < resolution - 1)
            {
                // we create a quad with 2 triangles (0, 1, 2) and (0, 2, 3)
                triangles[triangleIndex] = index;
                triangles[triangleIndex + 1] = index + resolution;
                triangles[triangleIndex + 2] = index + resolution + 1;

                triangles[triangleIndex + 3] = index;
                triangles[triangleIndex + 4] = index + resolution + 1;
                triangles[triangleIndex + 5] = index + 1;

                // +6 because we generated 2 triangles
                triangleIndex += 6;
            }
        }

        _mesh.Clear();
        _mesh.SetVertices(vertices);
        _mesh.SetTriangles(triangles, 0);
        _mesh.SetUVs(0, uv);
        _mesh.RecalculateNormals();
    }
}