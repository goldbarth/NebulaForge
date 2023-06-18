using UnityEngine;

public class RotateAroundPoint : Rotate
{
    [Header("Object Reference")]
    [SerializeField] private Transform _transform;
    
    private void Update()
    {
        transform.RotateAround(_transform.position, SetRotationAngle(), _rotationSpeed * Time.deltaTime);
    }
}