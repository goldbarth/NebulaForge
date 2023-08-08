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

    private void Update()
    {
        if (_drawOrbits)
            DrawOrbits();
    }

    private void DrawOrbits()
    {
        var bodies = FindObjectsOfType<CelestialObject>();
        var drawPoints = new NativeArray<Vector3>(bodies.Length * _numSteps, Allocator.TempJob);
        var virtualBodies = new NativeArray<VirtualBody>(bodies.Length, Allocator.TempJob);
        var pathColors = new NativeArray<Color>(bodies.Length, Allocator.TempJob);

        var referenceFrameIndex = 0;
        var referenceBodyInitialPosition = Vector3.zero;

        // Initialize virtual bodies (don't want to move the actual bodies).
        for (int i = 0; i < virtualBodies.Length; i++)
        {
            virtualBodies[i] = new VirtualBody(bodies[i]);
            pathColors[i] = bodies[i].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;

            if (bodies[i] == _centralBody && _relativeToBody)
            {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualBodies[i].Position;
            }
        }

        // Create the job.
        var job = new CalculateOrbitsJob
        {
            NumSteps = _numSteps,
            TimeStep = _timeStep,
            PathColors = pathColors,
            DrawPoints = drawPoints,
            VirtualBodies = virtualBodies,
            RelativeToBody = _relativeToBody,
            ReferenceFrameIndex = referenceFrameIndex,
            ReferenceBodyInitialPosition = referenceBodyInitialPosition
        };

        // Schedule and execute the job in parallel (with the separated logic).
        job.Schedule(virtualBodies.Length, 64).Complete();

        virtualBodies.Dispose();
        pathColors.Dispose();
        drawPoints.Dispose();
    }

    //TODO: Extract later, when debugging is done.
    [BurstCompile]
    private struct CalculateOrbitsJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> DrawPoints;
        public NativeArray<VirtualBody> VirtualBodies;
        [ReadOnly] public NativeArray<Color> PathColors;

        [ReadOnly] public Vector3 ReferenceBodyInitialPosition;
        [ReadOnly] public int ReferenceFrameIndex;
        [ReadOnly] public bool RelativeToBody;
        [ReadOnly] public float TimeStep;
        [ReadOnly] public int NumSteps;
        [ReadOnly] public float Width;

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
                virtualBody.Velocity += CalculateAcceleration(index, VirtualBodies) * TimeStep;

                // Update positions
                var newPosition = VirtualBodies[index].Position + VirtualBodies[index].Velocity * TimeStep;
                
                var body = VirtualBodies[index];
                body.Position = newPosition;
                VirtualBodies[index] = body;

                if (RelativeToBody)
                {
                    var referenceFrameOffset = referenceBodyPosition - ReferenceBodyInitialPosition;
                    newPosition -= referenceFrameOffset;
                }

                if (RelativeToBody && index == ReferenceFrameIndex)
                {
                    newPosition = ReferenceBodyInitialPosition;
                }

                // TODO: Can´t use: DrawPoints[startIndex * NumSteps + step] = newPosition;
                // like in the non-job version, because of the NativeArray restrictions. I guess.
                DrawPoints[startIndex + step] = newPosition;
            }

            // Draw the path
            for (int steps = 0; steps < NumSteps - 1; steps++)
            {
                var numSteps = index * NumSteps;
                var startPoint = DrawPoints[numSteps + steps];
                var endPoint = DrawPoints[numSteps + steps + 1];
                Debug.DrawLine(startPoint, endPoint, PathColors[index]);
            }
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
}