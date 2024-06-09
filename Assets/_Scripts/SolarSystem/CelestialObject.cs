using HelpersAndExtensions;
using UnityEngine;
using Planet;

namespace SolarSystem
{
    // Source: Sebastian Lague's "Coding Adventure: Solar System"
    // I have modified the basics, simplified it and expanded it with properties
    // and calculations.
    [ExecuteAlways, RequireComponent(typeof(Rigidbody))]
    public class CelestialObject : MonoBehaviour
    {
        [Space, SerializeField] private float _surfaceGravity;
        [Space(5), SerializeField] public float Mass;
        [Space(5), SerializeField] public Vector3 InitialVelocity = new(0f, 0.5f, 0f);
        [Space(5), SerializeField, ReadOnly] private Vector3 _currentVelocity;

        public float SurfaceGravity
        {
            get => _surfaceGravity;
            set
            {
                _surfaceGravity = value;
                MassCalculation();
            }
        }

        public Vector3 Velocity
        {
            
            get => _currentVelocity;
            set => _currentVelocity = value;
        }

        public float ObjectMass
        {
            get => Mass;
            set
            {
                Mass = value;
                _rigidbody.mass = Mass;
                GravityCalculation();
            }
        }

        public Vector3 Position => _rigidbody.position;

        private Rigidbody _rigidbody;
        private float _radius;

        private void Awake()
        {
            if (GetComponentInChildren<ObjectGenerator>() != null)
                _radius = GetComponentInChildren<ObjectGenerator>().Radius;
            
            _rigidbody = GetComponent<Rigidbody>();
            _currentVelocity = InitialVelocity;
        
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
        
        private void GravityCalculation()
        {
            _surfaceGravity = Universe.GravitationalConstant * Mass / (_radius * _radius);
        }

        public void UpdateVelocity(Vector3 acceleration, float timeStep)
        {
            _currentVelocity += acceleration * timeStep;
        }

        public void UpdatePosition(float timeStep)
        {
            
            if (gameObject.CompareTag("BlackHole"))
                _currentVelocity = Vector3.zero;
             
            _rigidbody.MovePosition(_rigidbody.position + _currentVelocity * timeStep);
        }
    }
}
