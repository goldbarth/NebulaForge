using UnityEngine.UI;
using SolarSystem;
using UnityEngine;

namespace UIAndUX
{
    public class UserInput : MonoBehaviour
    {
        [SerializeField] private GameObject _planetValueInputPanel;
        
        [SerializeField] private Slider _magnitudeSlider;
        [SerializeField] private Slider _gravitySlider;
        [SerializeField] private Slider _massSlider;
        
        [SerializeField] private Toggle _manualTimeScaleToggle;
        [SerializeField] private Slider _timeScaleSlider;

        private SelectionManager _selectionManager;
        private OrbitSimulation _orbitSimulation;

        private bool _isMagnitudeInitialized;
        private bool _isGravityInitialized;
        private bool _isMassInitialized;

        private void Awake()
        {
            _selectionManager = SelectionManager.Instance;
            _orbitSimulation = OrbitSimulation.Instance;
        }
        
        private void Start()
        {
            _magnitudeSlider.onValueChanged.AddListener(ReadMagnitudeInput);
            _gravitySlider.onValueChanged.AddListener(ReadGravityInput);
            _massSlider.onValueChanged.AddListener(ReadMassInput);
            
            _manualTimeScaleToggle.onValueChanged.AddListener(SetManualTimeScale);
            _timeScaleSlider.onValueChanged.AddListener(SetTimeScale);
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
            if (SelectionManager.Instance.SelectedObject() == null)
                _isMagnitudeInitialized = false;
            
            if (!_isMagnitudeInitialized)
            {
                value = SelectionManager.Instance.SelectedObject().Velocity.magnitude;
                _isMagnitudeInitialized = true;
            }
            
            SelectionManager.Instance.SelectedObject().Velocity = 
                SelectionManager.Instance.SelectedObject().Velocity.normalized * value;
        }

        private void ReadGravityInput(float value)
        {
            if (SelectionManager.Instance.SelectedObject() == null)
                _isGravityInitialized = false;
            
            if (!_isGravityInitialized)
            {
                value = SelectionManager.Instance.SelectedObject().SurfaceGravity;
                _isGravityInitialized = true;
            }
            SelectionManager.Instance.SelectedObject().SurfaceGravity = value;
        }

        private void ReadMassInput(float value)
        {
            if (SelectionManager.Instance.SelectedObject() == null)
                _isMassInitialized = false;
            
            if (!_isMassInitialized)
            {
                value = SelectionManager.Instance.SelectedObject().ObjectMass;
                _isMassInitialized = true;
            }
            
            SelectionManager.Instance.SelectedObject().ObjectMass = value;
        }
        
        private void SetManualTimeScale(bool value)
        {
            _orbitSimulation.ManualTimeScale = value;
        }
        
        private void SetTimeScale(float value)
        {
            _orbitSimulation.TimeScale = _timeScaleSlider.value;
        }

        private void SetPanelActive()
        {
            _planetValueInputPanel.SetActive(!_planetValueInputPanel.activeSelf);
        }
        
    }
}