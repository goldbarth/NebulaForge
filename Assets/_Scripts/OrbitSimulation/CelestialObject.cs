using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CelestialObject : MonoBehaviour
{
    [SerializeField] private Vector3 _initialVelocity = new(0f, 0.5f, 0f);
    
    [field: SerializeField] public Vector3 Velocity { get; private set; }
    [field: SerializeField] public float Mass { get; private set; }
    [field: SerializeField] public float SurfaceGravity { get; set; }

    //private Rigidbody Rigidbody => _rigidbody ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();
    public Vector3 Position => _rigidbody.position;

    private Rigidbody _rigidbody;
    private float _radius;

    private void Awake()
    {
        _radius = GetComponentInChildren<ObjectGenerator>().PlanetRadius;
        _rigidbody = GetComponent<Rigidbody>();
        Velocity = _initialVelocity;
        
        MassCalculation();
    }

    private void OnValidate()
    {
        _rigidbody = GetComponent<Rigidbody>();
        MassCalculation();
    }

    private void MassCalculation()
    {
        Mass = SurfaceGravity * _radius * _radius / Universe.GravitationalConstant;
        _rigidbody.mass = Mass;
    }

    public void UpdateVelocity(Vector3 acceleration, float timeStep)
    {
        Velocity += acceleration * timeStep;
    }

    public void UpdatePosition(float timeStep)
    {
        _rigidbody.MovePosition(_rigidbody.position + Velocity * timeStep);
    }
}
