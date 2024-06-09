using System;
using UnityEngine;

namespace CameraController
{
    public class FpsCameraController: MonoBehaviour
    {
        [SerializeField] private float _speed = 400f;
        [SerializeField] private float _sensitivity = 2f;

        private const int ScrollSpeedMultiplier = 100;
        private const int MinSpeed = 400;
        private const int MaxSpeed = 1400;

        private void Update()
        {
            MouseMovement();
            KeyboardMovement();
            MouseScroll();
        }

        private void MouseScroll()
        {
            _speed = Mathf.Clamp(_speed + Input.mouseScrollDelta.y * ScrollSpeedMultiplier, MinSpeed, MaxSpeed);
        }

        private void KeyboardMovement()
        {
            transform.position += transform.forward * (Input.GetAxis("Vertical") * _speed * Time.deltaTime);
            transform.position += transform.right * (Input.GetAxis("Horizontal") * _speed * Time.deltaTime);
            
            if (Input.GetKey(KeyCode.Space))
                transform.position += Vector3.up * (_speed * Time.deltaTime);
            
            if (Input.GetKey(KeyCode.C))
                transform.position += Vector3.down * (_speed * Time.deltaTime);
            
            if(Input.GetKey(KeyCode.Q))
                transform.eulerAngles += new Vector3(0, 0, .1f);
            
            if(Input.GetKey(KeyCode.E))
                transform.eulerAngles += new Vector3(0, 0, -.1f);
        }

        private void MouseMovement()
        {
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            transform.eulerAngles += new Vector3(-mouseY * _sensitivity, mouseX * _sensitivity, 0);
        }
    }
}