using HelpersAndExtensions;
using UnityEngine.UI;
using SolarSystem;
using UnityEngine;
using TMPro;

namespace UserUI
{
    public class UserInput : MonoBehaviour
    {
        [SerializeField] private GameObject _planetValueInputPanel;
        
        [SerializeField] private TMP_InputField _magnitudeInputField;
        [SerializeField] private TMP_InputField _gravityInputField;
        [SerializeField] private TMP_InputField _massInputField;
        
        [SerializeField] private Slider _timeScaleSlider;
        [SerializeField] private Toggle _manualTimeScaleToggle;

        private SelectionManager _selectionManager;
        private OrbitSimulation _orbitSimulation;

        private void Awake()
        {
            _selectionManager = SelectionManager.Instance;
            _orbitSimulation = OrbitSimulation.Instance;
        }
        
        private void Start()
        {
            _magnitudeInputField.onEndEdit.AddListener(ReadMagnitudeInput);
            _gravityInputField.onEndEdit.AddListener(ReadGravityInput);
            _massInputField.onEndEdit.AddListener(ReadMassInput);
            
            _timeScaleSlider.onValueChanged.AddListener(SetTimeScale);
            _manualTimeScaleToggle.onValueChanged.AddListener(SetManualTimeScale);
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

        private void ReadMagnitudeInput(string input)
        {
            if (float.TryParse(input, out var magnitude))
            {
                if(magnitude <= 0f) return;
                SelectionManager.Instance.SelectedObject().Velocity = 
                    SelectionManager.Instance.SelectedObject().Velocity.normalized * magnitude;
            }

            _magnitudeInputField.Clear();
        }

        private void ReadGravityInput(string input)
        {
            if (float.TryParse(input, out var gravity))
            {
                if(gravity <= 0f) return;
                SelectionManager.Instance.SelectedObject().SurfaceGravity = gravity;
            }
            
            _gravityInputField.Clear();
        }

        private void ReadMassInput(string input)
        {
            if (float.TryParse(input, out var mass))
            {
                if(mass <= 0f) return;
                SelectionManager.Instance.SelectedObject().ObjectMass = mass;
            }
            
            _massInputField.Clear();
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