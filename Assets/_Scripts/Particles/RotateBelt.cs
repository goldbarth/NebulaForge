using UnityEngine;

public class RotateBelt : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.forward * 0.1f);
    }
}
