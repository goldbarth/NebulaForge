using SolarSystem;
using UnityEngine;

// Original from Sebastian Lague's Solar System series. To draw the orbits with foresight.
// I have changed it so that it is as close as possible to the job script logic.
[ExecuteAlways]
public class OrbitDebugDisplay : MonoBehaviour
{
    [SerializeField] private bool _drawOrbits = true;
    [SerializeField] private int _numSteps = 1000;
    [SerializeField] private float _timeStep = 0.1f;

    [SerializeField] private bool _relativeToBody;
    [SerializeField] private CelestialObject _centralBody;

    private void Update()
    {
        if (_drawOrbits) 
            DrawOrbits();
    }

    private void DrawOrbits()
    {
        var bodies = FindObjectsOfType<CelestialObject>();
        var drawPoints = new Vector3[bodies.Length * _numSteps];
        var virtualBodies = new VirtualBody[bodies.Length];
        
        var referenceFrameIndex = 0;
        var referenceBodyInitialPosition = Vector3.zero;

        // Initialize virtual bodies (don't want to move the actual bodies)
        for (int i = 0; i < virtualBodies.Length; i++)
        {
            virtualBodies[i] = new VirtualBody(bodies[i]);

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
                
                drawPoints[i * _numSteps + step] = newPos;
            }
        }

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < virtualBodies.Length; bodyIndex++)
        {
            var startIndex = bodyIndex * _numSteps;
            var pathColour = bodies[bodyIndex].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;

            for (int steps = 0; steps < _numSteps - 1; steps++)
            {
                var startPoint = drawPoints[startIndex + steps];
                var endPoint = drawPoints[startIndex + steps + 1];
                Debug.DrawLine(startPoint, endPoint, pathColour);
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
}