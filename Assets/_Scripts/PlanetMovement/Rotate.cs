using UnityEngine;

public enum OrbitAngle
{
    X,
    Y,
    Z
}

public class Rotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField, Range(0f, 50f), Space(3)] protected float _rotationSpeed;
    [SerializeField] protected OrbitAngle _orbitAngle = OrbitAngle.Y;

    private void Update()
    {
        RotateAroundSelf();
    }

    private void RotateAroundSelf()
    {
        transform.Rotate(SetRotationAngle(_orbitAngle), _rotationSpeed * Time.deltaTime);
    }

    protected Vector3 SetRotationAngle(OrbitAngle orbitAngle = OrbitAngle.Y)
    {
        switch (orbitAngle)
        {
            case OrbitAngle.X:
                return Vector3.right;
            case OrbitAngle.Y:
                return Vector3.up;
            case OrbitAngle.Z:
                return Vector3.forward;
            default:
                return Vector3.zero;
        }
    }
}