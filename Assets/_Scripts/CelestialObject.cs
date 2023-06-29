using UnityEngine;

// ----------------------------------------------------------------------------------------------------------
// First I tried a simple approach to simulate gravity, but it didn't work out as intended. After some research
// I found a solution that works better. The code is based on the following sources:
// https://en.wikipedia.org/wiki/Gravitational_constant, https://en.wikipedia.org/wiki/Orbital_speed
// https://en.wikipedia.org/wiki/Two-body_problem, https://en.wikipedia.org/wiki/Three-body_problem
// https://en.wikipedia.org/wiki/N-body_simulation, https://arxiv.org/pdf/math/0011268.pdf
// And of course Sebastian Lague's tutorial series coding Adventure: https://www.youtube.com/watch?v=7axImc1sxa0
// I don't use exactly the same approach as he does, but I got the idea from him.
// ----------------------------------------------------------------------------------------------------------
[RequireComponent(typeof(Rigidbody))]
public class CelestialObject : MonoBehaviour
{
    [SerializeField] private Transform _centerObject;
    [SerializeField] private Vector3 _initialVelocity = new(0f, 0.5f, 0f);
    
    [field: SerializeField] public Vector3 Velocity { get; private set; }
    [field: SerializeField] public float Mass { get; set; }
    [field: SerializeField] public float SurfaceGravity { get; set; }
    
    private const float GravityConstant = 0.01f;
    private const float PhysicsTimeStep = 0.01f;
    
    private Rigidbody _rigidbody;
    private float _radius;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Time.fixedDeltaTime = PhysicsTimeStep;
        Velocity = _initialVelocity;
        
        _radius = GetComponentInChildren<ObjectGenerator>().PlanetRadius;
        
        RecalculateMass();
    }

    private void OnValidate()
    {
        RecalculateMass();
    }

    private void FixedUpdate()
    {
        UpdateVelocity(VelocityChange());
        UpdatePosition();
    }
    
    private Vector3 GravitationalAcceleration()
    {
        // F = G * (m1 * m2) / r^2 | Newton's law of universal gravitation
        // a = G * m1 / r^2 | Gravitational acceleration

        var centerPosition = _centerObject.position;
        var orbitingPosition = transform.position;
        
        // Gravitational constant (G).
        var G = GravityConstant;
        // Mass of the orbiting object (m1).
        var mass = _rigidbody.mass;
        // Distance squared (r^2).
        var sqrDistance = (centerPosition - orbitingPosition).sqrMagnitude;
        // Directional force.
        var forceDirection = (centerPosition - orbitingPosition).normalized;
        
        // a = G * m1 / r^2 * direction | Acceleration (directional)
        return forceDirection * (G * mass / sqrDistance);
    }
    
    private void RecalculateMass()
    {
        Mass = SurfaceGravity * _radius * _radius / GravityConstant;
        _rigidbody.mass = Mass;
    }
    
    private Vector3 VelocityChange()
    {
        return GravitationalAcceleration() * PhysicsTimeStep;
    }
    
    private void UpdateVelocity(Vector3 velocityChange)
    {
        Velocity += velocityChange;
    }

    private void UpdatePosition()
    {
        _rigidbody.MovePosition(_rigidbody.position + Velocity * PhysicsTimeStep);
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
