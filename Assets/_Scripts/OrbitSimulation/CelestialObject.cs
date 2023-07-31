using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class CelestialObject : MonoBehaviour
{
    [BoxGroup("Conditions")]
    [Space, SerializeField] private float _surfaceGravity;
    [BoxGroup("Conditions")]
    [ReadOnly] public float Mass;
    [Foldout("Velocity Status")]
    [SerializeField] public Vector3 InitialVelocity = new(0f, 0.5f, 0f);
    [Foldout("Velocity Status")]
    [ReadOnly] public Vector3 CurrentVelocity;
    
    public Vector3 Position => _rigidbody.position;

    private Rigidbody _rigidbody;
    private float _radius;

    private void Awake()
    {
        _radius = GetComponentInChildren<ObjectGenerator>().Radius;
        _rigidbody = GetComponent<Rigidbody>();
        CurrentVelocity = InitialVelocity;
        
        MassCalculation();
    }

    private void OnValidate()
    {
        _rigidbody = GetComponent<Rigidbody>();
        MassCalculation();
    }

    private void MassCalculation()
    {
        Mass = _surfaceGravity * _radius * _radius / Universe.GravitationalConstant;
        _rigidbody.mass = Mass;
    }

    public void UpdateVelocity(Vector3 acceleration, float timeStep)
    {
        CurrentVelocity += acceleration * timeStep;
    }

    public void UpdatePosition(float timeStep)
    {
        _rigidbody.MovePosition(_rigidbody.position + CurrentVelocity * timeStep);
    }
}
