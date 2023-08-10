using SolarSystem;
using UnityEngine;

// With the VirtualBody we copy the necessary data
// from the CelestialObject as a reference
public struct VirtualBody 
{
    public readonly float Mass;
        
    public Vector3 Position;
    public Vector3 Velocity;

    public VirtualBody (CelestialObject body)
    {
        Position = body.transform.position;
        Velocity = body.InitialVelocity;
        Mass = body.Mass;
    }
}