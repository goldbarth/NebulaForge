using UnityEngine;

namespace CameraController
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject  _topDownCamera;
        [SerializeField] private GameObject  _freeViewCamera;
        
        [SerializeField] private GameObject _topdownCanvas;
        [SerializeField] private GameObject _freeViewCanvas;

        private void Start()
        {
            _topDownCamera.SetActive(true);
            _freeViewCamera.SetActive(false);
            
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
                _topDownCamera.SetActive(!_topDownCamera.activeSelf);
                _freeViewCamera.SetActive(!_freeViewCamera.activeSelf);
                
                _topdownCanvas.SetActive(!_topdownCanvas.activeSelf);
                _freeViewCanvas.SetActive(!_freeViewCanvas.activeSelf);
            }
        }
    }
}