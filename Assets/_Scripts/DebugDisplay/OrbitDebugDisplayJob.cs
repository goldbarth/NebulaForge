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
    
    [SerializeField] private bool _useJob;
    [SerializeField] private bool _useJobFor;
    [SerializeField] private bool _useJobParallelFor;

    private void Update()
    {
        if (_drawOrbits)
            DrawOrbits();
    }

    //TODO: Refactor later, when debugging is done.
    private void DrawOrbits()
    {
        var bodies = FindObjectsOfType<CelestialObject>();
        var drawPoints = new NativeArray<Vector3>(bodies.Length * _numSteps, Allocator.TempJob);
        var virtualBodies = new NativeArray<VirtualBody>(bodies.Length, Allocator.TempJob);
        var pathColors = new NativeArray<Color>(bodies.Length, Allocator.TempJob);

        // "Center of the universe" related variables.
        var referenceFrameIndex = 0;
        var referenceBodyInitialPosition = Vector3.zero;

        // Initialize virtual bodies (don't want to move the actual bodies).
        for (int i = 0; i < virtualBodies.Length; i++)
        {
            virtualBodies[i] = new VirtualBody(bodies[i]);
            pathColors[i] = bodies[i].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;

            // Set the reference frame. This is the body that the other bodies will be relative to(center of the universe).
            if (bodies[i] == _centralBody && _relativeToBody)
            {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualBodies[i].Position;
            }
        }
        
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

        var jobFor = new CalculateOrbitsJobFor
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
        
        var parallelJob = new CalculateOrbitsJobParallel
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

        var handle = new JobHandle();
        
        if (_useJob)
        {
            handle = job.Schedule();
        }
        if (_useJobFor)
        {
            jobFor.Run(virtualBodies.Length);
            var scheduleJobDependency = new JobHandle();
            var scheduleJobHandle = jobFor.Schedule(virtualBodies.Length, scheduleJobDependency);
            var scheduleParallelJobHandle = jobFor.ScheduleParallel(virtualBodies.Length, 64, scheduleJobHandle);
            scheduleParallelJobHandle.Complete();
        }
        if (_useJobParallelFor)
        {
            handle = parallelJob.Schedule(virtualBodies.Length, 64);
        }

        handle.Complete();

        virtualBodies.Dispose();
        pathColors.Dispose();
        drawPoints.Dispose();
    }

    //TODO: Extract later, when debugging is done.
    [BurstCompile]
    private struct CalculateOrbitsJob : IJob
    {
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> DrawPoints;
        [ReadOnly] public NativeArray<Color> PathColors;
        public NativeArray<VirtualBody> VirtualBodies;

        [ReadOnly] public Vector3 ReferenceBodyInitialPosition;
        [ReadOnly] public int ReferenceFrameIndex;
        [ReadOnly] public bool RelativeToBody;
        [ReadOnly] public float TimeStep;
        [ReadOnly] public int NumSteps;
        [ReadOnly] public float Width;

        public void Execute()
        {
            // Simulate
            for (int step = 0; step < NumSteps; step++)
            {
                var referenceBodyPosition =
                    (RelativeToBody) ? VirtualBodies[ReferenceFrameIndex].Position : Vector3.zero;
                // Update velocities
                for (int i = 0; i < VirtualBodies.Length; i++)
                {
                    var body = VirtualBodies[i];
                    body.Velocity += CalculateAcceleration(i, VirtualBodies) * TimeStep;
                    VirtualBodies[i] = body;
                }

                // Update positions
                for (int i = 0; i < VirtualBodies.Length; i++)
                {
                    var newPos = VirtualBodies[i].Position + VirtualBodies[i].Velocity * TimeStep;
                    var body = VirtualBodies[i];
                    body.Position = newPos;
                    VirtualBodies[i] = body;

                    if (RelativeToBody)
                    {
                        var referenceFrameOffset = referenceBodyPosition - ReferenceBodyInitialPosition;
                        newPos -= referenceFrameOffset;
                    }

                    if (RelativeToBody && i == ReferenceFrameIndex)
                    {
                        newPos = ReferenceBodyInitialPosition;
                    }

                    DrawPoints[i * NumSteps + step] = newPos;
                }
            }

            // Draw paths
            for (int bodyIndex = 0; bodyIndex < VirtualBodies.Length; bodyIndex++)
            {
                var startIndex = bodyIndex * NumSteps;
                for (int steps = 0; steps < NumSteps - 1; steps++)
                {
                    var startPoint = DrawPoints[startIndex + steps];
                    var endPoint = DrawPoints[startIndex + steps + 1];
                    Debug.DrawLine(startPoint, endPoint, PathColors[bodyIndex]);
                }
            }
        }
    }

    //TODO: Extract later, when debugging is done.
    [BurstCompile]
    private struct CalculateOrbitsJobFor : IJobFor
    {
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> DrawPoints;
        [ReadOnly] public NativeArray<Color> PathColors;
        public NativeArray<VirtualBody> VirtualBodies;

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
                // Calculate the center body position (if needed).
                var referenceBodyPosition =
                    (RelativeToBody) ? VirtualBodies[ReferenceFrameIndex].Position : Vector3.zero;

                // Update velocities.
                virtualBody.Velocity += CalculateAcceleration(index, VirtualBodies) * TimeStep;


                // // Update positions.
                var newPosition = VirtualBodies[index].Position + VirtualBodies[index].Velocity * TimeStep;

                // Update the virtual body position.
                var body = VirtualBodies[index];
                body.Position = newPosition;
                VirtualBodies[index] = body;

                if (RelativeToBody)
                {
                    // If the center body is moving, we need to move the other bodies as well.
                    var referenceFrameOffset = referenceBodyPosition - ReferenceBodyInitialPosition;
                    newPosition -= referenceFrameOffset;
                }

                // If the current body is the center body and we are 
                // drawing relative to it, we need to reset the position.
                if (RelativeToBody && index == ReferenceFrameIndex)
                    newPosition = ReferenceBodyInitialPosition;

                DrawPoints[startIndex + step] = newPosition;
            }

            // Draw the path.
            var startPoint = DrawPoints[startIndex + NumSteps - 1];
            var endPoint = DrawPoints[startIndex];
            Debug.DrawLine(startPoint, endPoint, PathColors[index]);

            // Update the virtual body.
            VirtualBodies[index] = virtualBody;
        }
    }
    
    //TODO: Extract later, when debugging is done.
    [BurstCompile]
    private struct CalculateOrbitsJobParallel : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> DrawPoints;
        [ReadOnly] public NativeArray<Color> PathColors;
        public NativeArray<VirtualBody> VirtualBodies;

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
                // Calculate the center body position (if needed).
                var referenceBodyPosition =
                    (RelativeToBody) ? VirtualBodies[ReferenceFrameIndex].Position : Vector3.zero;

                // Update velocities.
                virtualBody.Velocity += CalculateAcceleration(index, VirtualBodies) * TimeStep;


                // // Update positions.
                var newPosition = VirtualBodies[index].Position + VirtualBodies[index].Velocity * TimeStep;

                // Update the virtual body position.
                var body = VirtualBodies[index];
                body.Position = newPosition;
                VirtualBodies[index] = body;

                if (RelativeToBody)
                {
                    // If the center body is moving, we need to move the other bodies as well.
                    var referenceFrameOffset = referenceBodyPosition - ReferenceBodyInitialPosition;
                    newPosition -= referenceFrameOffset;
                }

                // If the current body is the center body and we are 
                // drawing relative to it, we need to reset the position.
                if (RelativeToBody && index == ReferenceFrameIndex)
                    newPosition = ReferenceBodyInitialPosition;

                DrawPoints[startIndex + step] = newPosition;
            }

            // Draw the path.
            var startPoint = DrawPoints[startIndex + NumSteps - 1];
            var endPoint = DrawPoints[startIndex];
            Debug.DrawLine(startPoint, endPoint, PathColors[index]);

            // Update the virtual body.
            VirtualBodies[index] = virtualBody;
        }
    }

    private static Vector3 CalculateAcceleration(int bodyIndex, NativeArray<VirtualBody> virtualBodies)
    {
        var acceleration = Vector3.zero;
        for (int virtualBodyIndex = 0; virtualBodyIndex < virtualBodies.Length; virtualBodyIndex++)
        {
            if (bodyIndex == virtualBodyIndex) continue;

            var forceDir = (virtualBodies[virtualBodyIndex].Position - virtualBodies[bodyIndex].Position)
                .normalized;
            var sqrDst = (virtualBodies[virtualBodyIndex].Position - virtualBodies[bodyIndex].Position)
                .sqrMagnitude;
            acceleration += forceDir * (Universe.GravitationalConstant * virtualBodies[virtualBodyIndex].Mass) /
                            sqrDst;
        }

        return acceleration;
    }
}