using Unity.Collections;
using UnityEngine;
using SolarSystem;
using Unity.Burst;
using Unity.Jobs;

namespace Jobs
{
    [BurstCompile]
    public struct CalculateOrbitsJob : IJob
    {
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> DrawPoints;
        [ReadOnly] public NativeArray<Color> PathColors;
        public NativeArray<VirtualBody> VirtualBodies;

        [ReadOnly] public Vector3 ReferenceBodyInitialPosition;
        [ReadOnly] public bool RelativeToCenterBody;
        [ReadOnly] public int ReferenceFrameIndex;
        [ReadOnly] public float TimeStep;
        [ReadOnly] public int NumSteps;

        public void Execute()
        {
            // Simulate.
            for (int step = 0; step < NumSteps; step++)
            {
                var referenceBodyPosition = SetReferenceBodyPosition();
                
                // Update velocities.
                for (int bodyIndex = 0; bodyIndex < VirtualBodies.Length; bodyIndex++)
                    AddAccelerationToBodyVelocity(bodyIndex);
                

                // Update positions. Separate the position update from the velocity seems to be better for performance.
                for (int bodyIndex = 0; bodyIndex < VirtualBodies.Length; bodyIndex++)
                {
                    var startIndex = bodyIndex * NumSteps;
                    
                    var newPosition = SetNewPosition(bodyIndex);
                    SetNewPositionToBody(bodyIndex, newPosition);

                    if (RelativeToCenterBody)
                        newPosition = SubtractNewPosWithCenterToBodyOldPosDirection(referenceBodyPosition, newPosition);

                    if (IsThisBodyCenterAndOthersRelativeTo(bodyIndex))
                        newPosition = ReferenceBodyInitialPosition;

                    DrawPoints[startIndex + step] = newPosition;
                }
            }

            // Draw paths
            for (int bodyIndex = 0; bodyIndex < VirtualBodies.Length; bodyIndex++)
            {
                var startIndex = bodyIndex * NumSteps;
                for (int steps = 0; steps < NumSteps - 1; steps++)
                    DrawPaths(startIndex, steps, bodyIndex);
            }
        }

        private bool IsThisBodyCenterAndOthersRelativeTo(int bodyIndex)
        {
            return RelativeToCenterBody && bodyIndex == ReferenceFrameIndex;
        }

        private void DrawPaths(int startIndex, int steps, int bodyIndex)
        {
            var startPoint = DrawPoints[startIndex + steps];
            var endPoint = DrawPoints[startIndex + steps + 1];
            Debug.DrawLine(startPoint, endPoint, PathColors[bodyIndex]);
        }

        private Vector3 SetReferenceBodyPosition()
        {
            var referenceBodyPosition =
                (RelativeToCenterBody) ? VirtualBodies[ReferenceFrameIndex].Position : Vector3.zero;
            return referenceBodyPosition;
        }

        private Vector3 SubtractNewPosWithCenterToBodyOldPosDirection(Vector3 referenceBodyPosition, Vector3 newPos)
        {
            var referenceFrameOffset = referenceBodyPosition - ReferenceBodyInitialPosition;
            newPos -= referenceFrameOffset;
            return newPos;
        }

        private Vector3 SetNewPosition(int bodyIndex)
        {
            var newPos = VirtualBodies[bodyIndex].Position + VirtualBodies[bodyIndex].Velocity * TimeStep;
            return newPos;
        }

        private void SetNewPositionToBody(int bodyIndex, Vector3 newPos)
        {
            var virtualBody = VirtualBodies[bodyIndex];
            virtualBody.Position = newPos;
            VirtualBodies[bodyIndex] = virtualBody;
        }

        private void AddAccelerationToBodyVelocity(int bodyIndex)
        {
            var virtualBody = VirtualBodies[bodyIndex];
            virtualBody.Velocity += CalculateAcceleration(bodyIndex, VirtualBodies) * TimeStep;
            VirtualBodies[bodyIndex] = virtualBody;
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
}