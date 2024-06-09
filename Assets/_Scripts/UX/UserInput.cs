using UnityEngine.UI;
using SolarSystem;
using UnityEngine;
using System;

namespace UX
{
    public class UserInput : MonoBehaviour
    {
        [SerializeField] private GameObject _planetValueInputPanel;
        
        [SerializeField] private Slider _magnitudeSlider;
        [SerializeField] private Slider _gravitySlider;
        [SerializeField] private Slider _massSlider;
        
        [SerializeField] private Toggle _manualTimeScaleToggle;
        [SerializeField] private Slider _timeScaleSlider;
        
        [SerializeField] private GameObject _quitPanel;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        
        private SelectionManager _selectionManager;
        private OrbitSimulation _orbitSimulation;
        
        public event Action OnSliderChanged;

        private void Awake()
        {
            _selectionManager = SelectionManager.Instance;
            _orbitSimulation = OrbitSimulation.Instance;
        }
        
        private void Start()
        {
            _planetValueInputPanel.SetActive(false);
            _quitPanel.SetActive(false);
            
            _magnitudeSlider.onValueChanged.AddListener(ReadMagnitudeInput);
            _gravitySlider.onValueChanged.AddListener(ReadGravityInput);
            _massSlider.onValueChanged.AddListener(ReadMassInput);
            
            _manualTimeScaleToggle.onValueChanged.AddListener(SetManualTimeScale);
            _timeScaleSlider.onValueChanged.AddListener(SetTimeScale);
            
            _confirmButton.onClick.AddListener(QuitSimulation);
            _cancelButton.onClick.AddListener(CancelQuit);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _quitPanel.SetActive(!_quitPanel.activeSelf);
            }
            
            _selectionManager.IsGamePaused = _quitPanel.activeSelf;
            Time.timeScale = _selectionManager.IsGamePaused ? 0 : 1;
        }

        private void OnEnable()
        {
            _selectionManager.OnObjectSelected += SetPanelActive;
            _selectionManager.OnObjectDeselectedReady += SetPanelActive;
        }
        
        private void OnDisable()
        {
            _selectionManager.OnObjectSelected -= SetPanelActive;
            _selectionManager.OnObjectDeselectedReady -= SetPanelActive;
        }

        private void ReadMagnitudeInput(float value)
        {
            if (Math.Abs(SelectionManager.Instance.SelectedObject().Velocity.magnitude - value) > 0.01f)
                OnSliderChanged?.Invoke();
            
            SelectionManager.Instance.SelectedObject().Velocity = 
                SelectionManager.Instance.SelectedObject().Velocity.normalized * value;
        }

        private void ReadGravityInput(float value)
        {
            if (Math.Abs(SelectionManager.Instance.SelectedObject().SurfaceGravity - value) > 0.01f)
                OnSliderChanged?.Invoke();
            
            SelectionManager.Instance.SelectedObject().SurfaceGravity = value;
        }

        private void ReadMassInput(float value)
        {
            if (Math.Abs(SelectionManager.Instance.SelectedObject().ObjectMass - value) > 0.01f)
                OnSliderChanged?.Invoke();
            
            SelectionManager.Instance.SelectedObject().ObjectMass = value;
        }
        
        private void SetManualTimeScale(bool value)
        {
            _orbitSimulation.ManualTimeScale = value;
        }
        
        private void SetTimeScale(float value)
        {
            if(!_orbitSimulation.ManualTimeScale)
                _timeScaleSlider.value = _orbitSimulation.TimeScale;
                
            _orbitSimulation.TimeScale = _timeScaleSlider.value;
        }

        private void SetPanelActive()
        {
            if (_selectionManager.SelectedObject() != null)
            {
                _magnitudeSlider.value = _selectionManager.SelectedObject().Velocity.magnitude;
                _gravitySlider.value = _selectionManager.SelectedObject().SurfaceGravity;
                _massSlider.value = _selectionManager.SelectedObject().ObjectMass;
            }
            
            _planetValueInputPanel.SetActive(!_planetValueInputPanel.activeSelf);
        }
        
        private void QuitSimulation()
        {
            Application.Quit();
        }
        
        private void CancelQuit()
        {
            _quitPanel.SetActive(false);
        }
    }
}