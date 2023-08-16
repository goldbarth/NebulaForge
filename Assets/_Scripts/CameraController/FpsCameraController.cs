using UnityEngine;

namespace CameraController
{
    public class FpsCameraController: MonoBehaviour
    {
        [SerializeField] private float _speed = 400f;
        [SerializeField] private float _sensitivity = 2f;
        
        private void Update()
        {
            MouseMovement();
            KeyboardMovement();
        }

        private void KeyboardMovement()
        {
            transform.position += transform.forward * (Input.GetAxis("Vertical") * _speed * Time.deltaTime);
            transform.position += transform.right * (Input.GetAxis("Horizontal") * _speed * Time.deltaTime);
        }

        private void MouseMovement()
        {
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            transform.eulerAngles += new Vector3(-mouseY * _sensitivity, mouseX * _sensitivity, 0);
        }
    }
}