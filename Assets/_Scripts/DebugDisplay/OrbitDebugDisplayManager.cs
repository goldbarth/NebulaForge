// using System.Collections.Generic;
// using Unity.Collections;
// using UnityEngine;
// using SolarSystem;
// using Unity.Jobs;
//
// [ExecuteInEditMode]
// public class OrbitDebugDisplayManager : MonoBehaviour
// {
//     [SerializeField] private bool _drawOrbits = true;
//     [SerializeField, Space] private int _numSteps = 1000;
//     [SerializeField] private float _timeStep = 0.1f;
//     [SerializeField, Space] private bool _usePhysicsTimeStep;
//     [SerializeField] private bool _relativeToBody;
//     [SerializeField, Space] private CelestialObject _centralBody;
//     [SerializeField] private float _width = 100;
//     [SerializeField, Space] private bool _useThickLines;
//
//     [SerializeField, Space] private List<VirtualBodyOrbit> _virtualBodies;
//     [SerializeField] private CelestialObject[] _celestialObjects;
//
//     private void Update()
//     {
//         if (!_drawOrbits) return;
//
//         if (_virtualBodies == null || _virtualBodies.Count == 0) return;
//         
//         var orbitDataArray = new NativeArray<OrbitData>(_virtualBodies.Count, Allocator.TempJob);
//         for (int i = 0; i < _virtualBodies.Count; i++)
//         {
//             orbitDataArray[i] = new OrbitData
//             {
//                 Steps = _numSteps,
//                 TimeStep = _timeStep,
//                 UsePhysicsTimeStep = _usePhysicsTimeStep,
//                 RelativeToBody = _relativeToBody,
//                 Width = _width,
//                 UseThickLines = _useThickLines
//             };
//         }
//
//         var job = new OrbitDebugDisplayJob
//         {
//             OrbitDataArray = orbitDataArray,
//             CelestialObjects = _celestialObjects,
//             CentralBody = _centralBody,
//         };
//         
//         // Call DrawOrbit for each virtual body after the job has completed
//         foreach (var virtualBodyOrbit in _virtualBodies)
//         {
//             virtualBodyOrbit.DrawOrbit(_numSteps, _timeStep, _width, _relativeToBody, _useThickLines, _celestialObjects,
//                 _centralBody);
//         }
//         
//         var jobHandle = job.Schedule(_virtualBodies.Count, 1);
//         jobHandle.Complete();
//         orbitDataArray.Dispose();
//     }
//
//     public struct OrbitData
//     {
//         // blittable fields
//         public int Steps;
//         public float TimeStep;
//         public bool UsePhysicsTimeStep;
//         public bool RelativeToBody;
//         public float Width;
//         public bool UseThickLines;
//     }
// }