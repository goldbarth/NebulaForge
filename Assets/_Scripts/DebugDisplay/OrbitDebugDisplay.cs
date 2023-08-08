using SolarSystem;
using UnityEngine;

// Straight copy pasta from Sebastian Lague's Solar System series.
// To adjust the orbit simulation and save a lot of time to set it up.
[ExecuteInEditMode]
public class OrbitDebugDisplay : MonoBehaviour
{
    [SerializeField] private bool _drawOrbits = true;
    [SerializeField] private int _numSteps = 1000;
    [SerializeField] private float _timeStep = 0.1f;
    [SerializeField] private bool _usePhysicsTimeStep;

    [SerializeField] private bool _relativeToBody;
    [SerializeField] private CelestialObject _centralBody;
    [SerializeField] private float _width = 100;
    [SerializeField] private bool _useThickLines;

    private void Update()
    {
        if (_drawOrbits) DrawOrbits();
        else HideOrbits();
    }

    private void DrawOrbits()
    {
        var bodies = FindObjectsOfType<CelestialObject>();
        var virtualBodies = new VirtualBody[bodies.Length];
        var drawPoints = new Vector3[bodies.Length][];
        var referenceFrameIndex = 0;
        var referenceBodyInitialPosition = Vector3.zero;

        // Initialize virtual bodies (don't want to move the actual bodies)
        for (int i = 0; i < virtualBodies.Length; i++)
        {
            virtualBodies[i] = new VirtualBody(bodies[i]);
            drawPoints[i] = new Vector3[_numSteps];

            if (bodies[i] == _centralBody && _relativeToBody)
            {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualBodies[i].Position;
            }
        }

        // Simulate
        for (int step = 0; step < _numSteps; step++)
        {
            var referenceBodyPosition = (_relativeToBody) ? virtualBodies[referenceFrameIndex].Position : Vector3.zero;
            // Update velocities
            for (int i = 0; i < virtualBodies.Length; i++)
            {
                virtualBodies[i].Velocity += CalculateAcceleration(i, virtualBodies) * _timeStep;
            }

            // Update positions
            for (int i = 0; i < virtualBodies.Length; i++)
            {
                var newPos = virtualBodies[i].Position + virtualBodies[i].Velocity * _timeStep;
                virtualBodies[i].Position = newPos;
                if (_relativeToBody)
                {
                    var referenceFrameOffset = referenceBodyPosition - referenceBodyInitialPosition;
                    newPos -= referenceFrameOffset;
                }

                if (_relativeToBody && i == referenceFrameIndex)
                {
                    newPos = referenceBodyInitialPosition;
                }

                drawPoints[i][step] = newPos;
            }
        }

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < virtualBodies.Length; bodyIndex++)
        {
            var pathColour = bodies[bodyIndex].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;

            if (_useThickLines)
            {
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                lineRenderer.enabled = true;
                lineRenderer.positionCount = drawPoints[bodyIndex].Length;
                lineRenderer.SetPositions(drawPoints[bodyIndex]);
                lineRenderer.startColor = pathColour;
                lineRenderer.endColor = pathColour;
                lineRenderer.widthMultiplier = _width;
            }
            else
            {
                for (int i = 0; i < drawPoints[bodyIndex].Length - 1; i++)
                {
                    Debug.DrawLine(drawPoints[bodyIndex][i], drawPoints[bodyIndex][i + 1], pathColour);
                }

                // Hide renderer
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                if (lineRenderer)
                {
                    lineRenderer.enabled = false;
                }
            }
        }
    }

    private static Vector3 CalculateAcceleration(int bodyIndex, VirtualBody[] virtualBodies)
    {
        var acceleration = Vector3.zero;
        for (int j = 0; j < virtualBodies.Length; j++)
        {
            if (bodyIndex == j) continue;

            var forceDir = (virtualBodies[j].Position - virtualBodies[bodyIndex].Position).normalized;
            var sqrDst = (virtualBodies[j].Position - virtualBodies[bodyIndex].Position).sqrMagnitude;
            acceleration += forceDir * (Universe.GravitationalConstant * virtualBodies[j].Mass) / sqrDst;
        }

        return acceleration;
    }

    private static void HideOrbits()
    {
        var bodies = FindObjectsOfType<CelestialObject>();

        // Draw paths
        foreach (var t in bodies)
        {
            var lineRenderer = t.gameObject.GetComponentInChildren<LineRenderer>();
            lineRenderer.positionCount = 0;
        }
    }

    private void OnValidate()
    {
        if (_usePhysicsTimeStep)
            _timeStep = Universe.PhysicsTimeStep;
    }
}