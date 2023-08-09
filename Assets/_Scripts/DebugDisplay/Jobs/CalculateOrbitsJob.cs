using Unity.Collections;
using UnityEngine;
using SolarSystem;
using Unity.Burst;
using Unity.Jobs;

namespace Jobs
{
    //TODO: Refactor later, when debugging is done.
    [BurstCompile]
    public struct CalculateOrbitsJob : IJob
    {
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> DrawPoints;
        [ReadOnly] public NativeArray<Color> PathColors;
        public NativeArray<VirtualBody> VirtualBodies;

        [ReadOnly] public Vector3 ReferenceBodyInitialPosition;
        [ReadOnly] public int ReferenceFrameIndex;
        [ReadOnly] public bool RelativeToBody;
        [ReadOnly] public float TimeStep;
        [ReadOnly] public int NumSteps;

        public void Execute()
        {
            // Simulate
            for (int step = 0; step < NumSteps; step++)
            {
                var referenceBodyPosition =
                    (RelativeToBody) ? VirtualBodies[ReferenceFrameIndex].Position : Vector3.zero;
                
                // Update velocities
                for (int bodyIndex = 0; bodyIndex < VirtualBodies.Length; bodyIndex++)
                {
                    var body = VirtualBodies[bodyIndex];
                    body.Velocity += CalculateAcceleration(bodyIndex, VirtualBodies) * TimeStep;
                    VirtualBodies[bodyIndex] = body;
                }

                // Update positions
                for (int bodyIndex = 0; bodyIndex < VirtualBodies.Length; bodyIndex++)
                {
                    var startIndex = bodyIndex * NumSteps;
                    
                    var newPos = VirtualBodies[bodyIndex].Position + VirtualBodies[bodyIndex].Velocity * TimeStep;
                    
                    var body = VirtualBodies[bodyIndex];
                    body.Position = newPos;
                    VirtualBodies[bodyIndex] = body;

                    if (RelativeToBody)
                    {
                        var referenceFrameOffset = referenceBodyPosition - ReferenceBodyInitialPosition;
                        newPos -= referenceFrameOffset;
                    }

                    if (RelativeToBody && bodyIndex == ReferenceFrameIndex)
                    {
                        newPos = ReferenceBodyInitialPosition;
                    }

                    DrawPoints[startIndex + step] = newPos;
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
}