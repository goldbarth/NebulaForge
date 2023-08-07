using SolarSystem;
using UnityEngine;

public class VirtualBody 
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