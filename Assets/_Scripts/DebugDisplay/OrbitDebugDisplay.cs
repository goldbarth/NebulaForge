using System;
using System.Collections.Generic;
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
     // [SerializeField] private float _lineThickness = 0.2f;

     [SerializeField] private bool _relativeToBody;
     [SerializeField] private CelestialObject _centralBody;

     private readonly Dictionary<CelestialObject, LineRenderer> _lineRenderers = new();
     // private void Start()
     // {
     //     InitializeLineRenderers();
     // }

     private void Update()
     {
         if (_drawOrbits) 
             DrawOrbits();
     }
     
     // private void InitializeLineRenderers()
     // {
     //     var bodies = FindObjectsOfType<CelestialObject>();
     //     foreach (var body in bodies)
     //     {
     //         LineRenderer lr = body.gameObject.GetComponent<LineRenderer>();
     //         if (lr == null)
     //         {
     //             lr = body.gameObject.AddComponent<LineRenderer>();
     //             // Konfiguriere hier den LineRenderer nach Bedarf
     //             lr.widthMultiplier = _lineThickness; // Beispielbreite
     //             lr.material = body.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial; // Beispielmaterial
     //             lr.startColor = lr.endColor = body.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color; // Beispiel für Farbzuweisung
     //         }
     //         lr.positionCount = _numSteps;
     //         _lineRenderers[body] = lr;
     //     }
     // }

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
         
         // foreach (var pair in _lineRenderers)
         // {
         //     var body = pair.Key;
         //     var lr = pair.Value;
         //
         //     // Berechne die Bahnkurvenpunkte für dieses CelestialObject
         //     var orbitPoints = CalculateOrbitPoints(body, _numSteps); // Du musst diese Funktion basierend auf deiner aktuellen Logik implementieren
         //
         //     lr.SetPositions(orbitPoints);
         // }
     }
     
     private Vector3[] CalculateOrbitPoints(CelestialObject body, int numSteps)
     {
         var points = new Vector3[numSteps];
         // TODO: Calculate the orbit points based on the body's position, velocity, and acceleration
         var position = body.Position;
         var velocity = body.Velocity;
     
         for (int i = 0; i < numSteps; i++)
         {
             var acceleration = CalculateAcceleration(body, position, velocity);
             velocity += acceleration * _timeStep;
             position += velocity * _timeStep;
             points[i] = position;
         }
     
         return points;
     }
     
     private Vector3 CalculateAcceleration(CelestialObject body, Vector3 position, Vector3 velocity)
     {
         var acceleration = Vector3.zero;
         var bodies = FindObjectsOfType<CelestialObject>();

         foreach (var otherBody in bodies)
         {
             if (otherBody == body) continue;

             var direction = otherBody.Position - position;
             var distance = direction.magnitude;
             var forceMagnitude = Universe.GravitationalConstant * otherBody.Mass / (distance * distance);
             acceleration += direction.normalized * forceMagnitude;
         }

         return acceleration;
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

// [ExecuteAlways]
// public class OrbitDebugDisplay : MonoBehaviour
// {
//     [SerializeField] private bool _drawOrbits = true;
//     [SerializeField] private int _numSteps = 1000;
//     [SerializeField] private float _timeStep = 0.1f;
//     [SerializeField] private bool _relativeToBody;
//     [SerializeField] private CelestialObject _centralBody;
//
//     private void Update()
//     {
//         if (_drawOrbits)
//             DrawOrbits();
//     }
//
//     private void DrawOrbits()
//     {
//         var bodies = FindObjectsOfType<CelestialObject>();
//         var drawPoints = new Vector3[bodies.Length * _numSteps];
//         var virtualBodies = InitializeVirtualBodies(bodies);
//
//         SimulateOrbits(virtualBodies, drawPoints);
//         DrawPaths(virtualBodies, drawPoints, bodies);
//     }
//
//     private VirtualBody[] InitializeVirtualBodies(CelestialObject[] bodies)
//     {
//         var virtualBodies = new VirtualBody[bodies.Length];
//         for (int i = 0; i < virtualBodies.Length; i++)
//             virtualBodies[i] = new VirtualBody(bodies[i]);
//         return virtualBodies;
//     }
//
//     private void SimulateOrbits(VirtualBody[] virtualBodies, Vector3[] drawPoints)
//     {
//         for (int step = 0; step < _numSteps; step++)
//         {
//             UpdateVelocities(virtualBodies);
//             UpdatePositions(virtualBodies, drawPoints, step);
//         }
//     }
//
//     private void UpdateVelocities(VirtualBody[] virtualBodies)
//     {
//         for (int i = 0; i < virtualBodies.Length; i++)
//             virtualBodies[i].Velocity += CalculateAcceleration(i, virtualBodies) * _timeStep;
//     }
//
//     private void UpdatePositions(VirtualBody[] virtualBodies, Vector3[] drawPoints, int step)
//     {
//         for (int i = 0; i < virtualBodies.Length; i++)
//         {
//             virtualBodies[i].Position += virtualBodies[i].Velocity * _timeStep;
//             drawPoints[i * _numSteps + step] = virtualBodies[i].Position;
//         }
//     }
//
//     private void DrawPaths(VirtualBody[] virtualBodies, Vector3[] drawPoints, CelestialObject[] bodies)
//     {
//         for (int bodyIndex = 0; bodyIndex < virtualBodies.Length; bodyIndex++)
//         {
//             var pathColour = bodies[bodyIndex].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;
//             for (int steps = 0; steps < _numSteps - 1; steps++)
//             {
//                 var startPoint = drawPoints[bodyIndex * _numSteps + steps];
//                 var endPoint = drawPoints[bodyIndex * _numSteps + steps + 1];
//                 Debug.DrawLine(startPoint, endPoint, pathColour);
//             }
//         }
//     }
//
//     private static Vector3 CalculateAcceleration(int bodyIndex, VirtualBody[] virtualBodies)
//     {
//         var acceleration = Vector3.zero;
//         for (int j = 0; j < virtualBodies.Length; j++)
//         {
//             if (bodyIndex == j) continue;
//             var forceDir = (virtualBodies[j].Position - virtualBodies[bodyIndex].Position).normalized;
//             var sqrDst = (virtualBodies[j].Position - virtualBodies[bodyIndex].Position).sqrMagnitude;
//             acceleration += forceDir * (Universe.GravitationalConstant * virtualBodies[j].Mass) / sqrDst;
//         }
//         return acceleration;
//     }
// }