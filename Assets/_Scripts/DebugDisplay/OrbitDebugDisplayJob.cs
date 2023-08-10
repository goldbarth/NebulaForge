using Unity.Collections;
using UnityEngine;
using SolarSystem;
using Unity.Jobs;
using Jobs;

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

        // "Center of the universe" related variables.
        var referenceFrameIndex = 0;
        var referenceBodyInitialPosition = Vector3.zero;

        // Initialize virtual bodies (don't want to move the actual bodies).
        for (int i = 0; i < virtualBodies.Length; i++)
        {
            virtualBodies[i] = new VirtualBody(bodies[i]);
            pathColors[i] = bodies[i].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;

            // Set the center body´s index and start position.
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
            RelativeToCenterBody = _relativeToBody,
            ReferenceFrameIndex = referenceFrameIndex,
            ReferenceBodyInitialPosition = referenceBodyInitialPosition
        };

        var handle = job.Schedule();

        handle.Complete();

        virtualBodies.Dispose();
        pathColors.Dispose();
        drawPoints.Dispose();
    }
}

