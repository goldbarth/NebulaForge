using UnityEngine;

public enum ChildOrbitAngle
{
    X,
    Y,
    Z
}

public class OrbitController : Rotate
{
    [SerializeField, Space(3)] private ChildOrbitAngle _childOrbitAngle = ChildOrbitAngle.Y;

    [SerializeField] private float[] _rotationSpeeds;
    [Header("Orbit Settings")]
    [SerializeField, Range(0f, 100f)] private float _orbitSpeed = 50f;
    [SerializeField, Range(0f, 500f)] private float _orbitRadius = 20f;
    
    [Header("Object References")]
    [SerializeField] private Transform _center;
    [SerializeField] private Transform[] _parents;
    [SerializeField] private Transform[] _children;
    

    private readonly Vector3 _childAxis = Vector3.forward;
    
    private Vector3 _initialPosition;

    private void Start()
    {
        foreach (var child in _children)
            _initialPosition = child.localPosition;
    }

    private void Update()
    {
        RotateAroundCenter();
        OrbitAroundParent();
    }

    private void RotateAroundCenter()
    {
        for (int parentIndex = 0; parentIndex < _parents.Length; parentIndex++)
        {
            var parent = _parents[parentIndex];
            var rotationSpeed = _rotationSpeeds[parentIndex];
            parent.RotateAround(_center.position, SetRotationAngle(_orbitAngle), rotationSpeed * Time.deltaTime);
        }
    }

    private void OrbitAroundParent()
    {
        var orbitPosition = Quaternion.Euler(SetChildRotationAngle()) * (_childAxis * _orbitRadius);
        foreach (var child in _children)
            child.localPosition = _initialPosition + orbitPosition;
    }
    
    private Vector3 SetChildRotationAngle()
    {
        var orbitAngle = _orbitSpeed * Time.time;
        switch (_childOrbitAngle)
        {
            case ChildOrbitAngle.X:
                return new Vector3(orbitAngle, 0f, 0f);
            case ChildOrbitAngle.Y:
                return new Vector3(0f, orbitAngle, 0f);
            case ChildOrbitAngle.Z:
                return new Vector3(0f, 0f, orbitAngle);
            default:
                return Vector3.zero;
        }
    }
}