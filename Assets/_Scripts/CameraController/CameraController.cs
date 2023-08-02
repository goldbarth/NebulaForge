using UnityEngine;

public class CameraController: MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _sensitivity = 5.0f;
    
    private void Update()
    {
        transform.position += transform.forward * (Input.GetAxis("Vertical") * _speed * Time.deltaTime);
        transform.position += transform.right * (Input.GetAxis("Horizontal") * _speed * Time.deltaTime);
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.eulerAngles += new Vector3(-mouseY * _sensitivity, mouseX * _sensitivity, 0);
    }
}