using System;
using UnityEngine;

namespace CameraController
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Camera _topDowCamera;
        [SerializeField] private Camera _freeViewCamera;
        
        [SerializeField] private GameObject _topdownCanvas;
        [SerializeField] private GameObject _freeViewCanvas;

        private void Start()
        {
            _topDowCamera.enabled = true;
            _freeViewCamera.enabled = false;
            
            _topdownCanvas.SetActive(true);
            _freeViewCanvas.SetActive(false);
        }

        private void Update()
        {
            SwitchCamera();
        }

        private void SwitchCamera()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _topDowCamera.enabled = !_topDowCamera.enabled;
                _freeViewCamera.enabled = !_freeViewCamera.enabled;
                
                _topdownCanvas.SetActive(!_topdownCanvas.activeSelf);
                _freeViewCanvas.SetActive(!_freeViewCanvas.activeSelf);
            }
        }
    }
}