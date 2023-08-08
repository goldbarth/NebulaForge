using Unity.Collections;
using UnityEngine;
using SolarSystem;
using Unity.Burst;
using Unity.Jobs;

[ExecuteAlways]
public class OrbitDebugDisplayJob : MonoBehaviour
{
    [SerializeField] private bool _drawOrbits = true;
    [SerializeField] private int _numSteps = 1000;
    [SerializeField] private float _timeStep = 0.1f;

    [SerializeField] private bool _relativeToBody;
    [SerializeField] private CelestialObject _centralBody;
    [SerializeField] private float _width = 100;
    [SerializeField] private bool _useThickLines;

    private void Update()
    {
        if (_drawOrbits)
            CalculateOrbits();
    }
    
     #region Jobs and Native Containers

    // TESTING JOBS

    private void CalculateOrbits()
    {
        var bodies = FindObjectsOfType<CelestialObject>();
        var virtualBodies = new NativeArray<VirtualBody>(bodies.Length, Allocator.TempJob);
        var drawPoints = new NativeArray<Vector3>(bodies.Length * _numSteps, Allocator.TempJob);
        
        var referenceBodyInitialPosition = Vector3.zero;
        var useThickLines = _useThickLines;
        var referenceFrameIndex = 0;

        // Initialize virtual bodies (don't want to move the actual bodies).
        for (int i = 0; i < virtualBodies.Length; i++)
        {
            virtualBodies[i] = new VirtualBody(bodies[i]);

            if (bodies[i] == _centralBody && _relativeToBody)
            {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualBodies[i].Position;
            }
        }

        // Create the job.
        var job = new CalculateOrbitsJob
        {
            VirtualBodies = virtualBodies,
            DrawPoints = drawPoints,
            NumSteps = _numSteps,
            TimeStep = _timeStep,
            RelativeToBody = _relativeToBody,
            ReferenceFrameIndex = referenceFrameIndex,
            ReferenceBodyInitialPosition = referenceBodyInitialPosition
        };

        // Schedule and execute the job in parallel (with the separated logic).
        job.Schedule(virtualBodies.Length, 64).Complete();

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < virtualBodies.Length; bodyIndex++)
        {
            var startIndex = bodyIndex * _numSteps;
            var pathColour = bodies[bodyIndex].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;

            if (useThickLines)
            {
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                lineRenderer.enabled = true;
                lineRenderer.positionCount = _numSteps;
                lineRenderer.SetPositions(drawPoints.GetSubArray(startIndex, _numSteps));
                lineRenderer.startColor = pathColour;
                lineRenderer.endColor = pathColour;
                lineRenderer.widthMultiplier = _width;
            }
            else
            {
                for (int steps = 0; steps < _numSteps - 1; steps++)
                {
                    var startPoint = drawPoints[startIndex + steps];
                    var endPoint = drawPoints[startIndex + steps + 1];
                    Debug.DrawLine(startPoint, endPoint, pathColour);
                }

                // Hide renderer
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                if (lineRenderer)
                {
                    lineRenderer.enabled = false;
                }
            }
        }

        virtualBodies.Dispose();
        drawPoints.Dispose();
    }

    [BurstCompile]
    private struct CalculateOrbitsJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> DrawPoints;
        public NativeArray<VirtualBody> VirtualBodies;

        public float Width;
        public int NumSteps;
        public float TimeStep;
        public bool RelativeToBody;
        public int ReferenceFrameIndex;
        public Vector3 ReferenceBodyInitialPosition;

        public void Execute(int index)
        {
            var virtualBody = VirtualBodies[index];
            var startIndex = index * NumSteps;

            // Simulate orbits
            for (int step = 0; step < NumSteps; step++)
            {
                var referenceBodyPosition =
                    (RelativeToBody) ? VirtualBodies[ReferenceFrameIndex].Position : Vector3.zero;

                // Update velocities
                for (int virtualBodyIndex = 0; virtualBodyIndex < VirtualBodies.Length; virtualBodyIndex++)
                {
                    virtualBody.Velocity += CalculateAcceleration(virtualBodyIndex, VirtualBodies) * TimeStep;
                }

                // Update positions
                for (int virtualBodyIndex = 0; virtualBodyIndex < VirtualBodies.Length; virtualBodyIndex++)
                {
                    var newPosition = VirtualBodies[virtualBodyIndex].Position + VirtualBodies[virtualBodyIndex].Velocity * TimeStep;
                    
                    // ------------------------------
                    var body = VirtualBodies[virtualBodyIndex];
                    body.Position = newPosition;
                    VirtualBodies[virtualBodyIndex] = body;
                    // ------------------------------
                    
                    if (RelativeToBody)
                    {
                        var referenceFrameOffset = referenceBodyPosition - ReferenceBodyInitialPosition;
                        newPosition -= referenceFrameOffset;
                    }

                    if (RelativeToBody && virtualBodyIndex == ReferenceFrameIndex)
                    {
                        newPosition = ReferenceBodyInitialPosition;
                    }
                    
                    DrawPoints[startIndex + step] = newPosition;
                }
            }

            VirtualBodies[index] = virtualBody;
        }
    }

    private static Vector3 CalculateAcceleration(int bodyIndex, NativeArray<VirtualBody> virtualBodies)
    {
        var acceleration = Vector3.zero;
        for (int virtualBodyIndex = 0; virtualBodyIndex < virtualBodies.Length; virtualBodyIndex++)
        {
            if (bodyIndex == virtualBodyIndex) continue;

            var forceDir = (virtualBodies[virtualBodyIndex].Position - virtualBodies[bodyIndex].Position).normalized;
            var sqrDst = (virtualBodies[virtualBodyIndex].Position - virtualBodies[bodyIndex].Position).sqrMagnitude;
            acceleration += forceDir * (Universe.GravitationalConstant * virtualBodies[virtualBodyIndex].Mass) / sqrDst;
        }

        return acceleration;
    }

    #endregion
}