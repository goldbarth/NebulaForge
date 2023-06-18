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
    
    [Header("Orbit Settings")]
    [SerializeField, Range(0f, 100f)] private float _orbitSpeed = 50f;
    [SerializeField, Range(0f, 50f)] private float _orbitRadius = 20f;
    
    [Header("Object References")]
    [SerializeField] private Transform _center;
    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _child;

    private readonly Vector3 _childAxis = Vector3.forward;
    
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = _child.localPosition;
    }

    private void Update()
    {
        RotateAroundCenter();
        OrbitAroundParent();
    }

    private void RotateAroundCenter()
    {
        _parent.RotateAround(_center.position, SetRotationAngle(_orbitAngle), _rotationSpeed * Time.deltaTime);
    }

    private void OrbitAroundParent()
    {
        var orbitPosition = Quaternion.Euler(SetChildRotationAngle()) * (_childAxis * _orbitRadius);
        _child.localPosition = _initialPosition + orbitPosition;
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
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_center.position, _orbitRadius);
    }
#endif
}