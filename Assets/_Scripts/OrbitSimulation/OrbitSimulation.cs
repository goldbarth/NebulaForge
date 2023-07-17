using NaughtyAttributes;
using UnityEngine;

// ----------------------------------------------------------------------------------------------------------
// First I tried a simple approach to simulate gravity, but it didn't work out as intended. After some research
// I found a solution that works better. The code is based on the following sources:
// https://en.wikipedia.org/wiki/Gravitational_constant, https://en.wikipedia.org/wiki/Orbital_speed
// https://en.wikipedia.org/wiki/Two-body_problem, https://en.wikipedia.org/wiki/Three-body_problem
// https://en.wikipedia.org/wiki/N-body_simulation, https://arxiv.org/pdf/math/0011268.pdf
// And of course Sebastian Lague's tutorial series coding Adventure: https://www.youtube.com/watch?v=7axImc1sxa0
// I don't use exactly the same approach as he does, but I got the idea from him. 
// I thought it was funny to use the original gravity constant (G = 6.67408f * Mathf.Pow(10, -11)),
// but all values would have had to be increased unnecessarily very strongly.
// ----------------------------------------------------------------------------------------------------------
public class OrbitSimulation : MonoBehaviour
{
    [SerializeField] private bool _manualTimeScale = false;
    [EnableIf("_manualTimeScale")]
    [SerializeField] private float _timeScale = 1f;
    
    [SerializeField] private CelestialObject[] _objects;

    private void Awake()
    {
        Time.fixedDeltaTime = Universe.PhysicsTimeStep;
    }

    private void FixedUpdate()
    {
        Time.timeScale = _manualTimeScale ? _timeScale : 1f;
        
        foreach (var obj in _objects)
            obj.UpdateVelocity(GravitationalAcceleration(obj.Position, obj), Universe.PhysicsTimeStep);

        foreach (var obj in _objects)
            obj.UpdatePosition(Universe.PhysicsTimeStep);
    }

    private Vector3 GravitationalAcceleration(Vector3 otherPosition, CelestialObject @object)
    {
        // F = G * (m1 * m2) / r^2 | Newton's law of universal gravitation
        // a = G * m1 / r^2 | Gravitational acceleration
        
        var acceleration = Vector3.zero;
        
        foreach (var obj in _objects)
        {
            if (obj == @object) continue;
            
            // Gravitational constant (G).
            var G = Universe.GravitationalConstant;
            // Mass of the orbiting object (m1).
            var mass = obj.Mass;
            // Distance squared (r^2).
            var sqrDistance = (obj.Position - otherPosition).sqrMagnitude;
            // Directional force.
            var forceDirection = (obj.Position - otherPosition).normalized;
        
            // a = G * m1 / r^2 * direction | Acceleration (directional)
            acceleration += (G * mass / sqrDistance) * forceDirection;
        }

        return acceleration;
    }
    
    // First attempt at simulating gravity.
    
    // // Newton's law of universal gravitation.
    // private Vector3 CalculateGravitationalForce()
    // {
    //     // F = G * (m1 * m2) / r^2
    //
    //     var centerPosition = _centerObject.position;
    //     var orbitingPosition = _rigidbody.position;
    //     
    //     // Distance between the two objects (r).
    //     var distance = Vector3.Distance(centerPosition, orbitingPosition);
    //     // Distance squared (r^2).
    //     var distanceSquared = distance * distance;
    //     // Gravitational constant (G).
    //     var G = 6.67408f * Mathf.Pow(10, -11);
    //     // Force (F).
    //     var force = G * (_rigidbody.mass * other.rigidbody.mass) / distanceSquared;
    //     
    //     // Multiply the direction by the force to get the force vector.
    //     var forceVector = force * (centerPosition - orbitingPosition);
    //     
    //     return forceVector;
    // }
}