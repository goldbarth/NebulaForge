using UnityEngine;
using TMPro;

namespace UserUI
{
    public class UserInput : MonoBehaviour
    {
        [SerializeField] private GameObject _inputPanel;
        
        [SerializeField] private TMP_InputField _magnitudeInputField;
        [SerializeField] private TMP_InputField _gravityInputField;
        [SerializeField] private TMP_InputField _massInputField;
        
        private SelectionManager _selectionManager;

        private void Awake()
        {
            _selectionManager = SelectionManager.Instance;
        }
        
        private void Start()
        {
            _magnitudeInputField.onEndEdit.AddListener(ReadMagnitudeInput);
            _gravityInputField.onEndEdit.AddListener(ReadGravityInput);
            _massInputField.onEndEdit.AddListener(ReadMassInput);
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
                SelectionManager.Instance.SelectedObject().Velocity = SelectionManager.Instance.SelectedObject().Velocity.normalized * magnitude;
        }

        private void ReadGravityInput(string input)
        {
            if (float.TryParse(input, out var gravity))
                SelectionManager.Instance.SelectedObject().SurfaceGravity = gravity;
        }

        private void ReadMassInput(string input)
        {
            if (float.TryParse(input, out var mass))
                SelectionManager.Instance.SelectedObject().ObjectMass = mass;
        }

        private void SetPanelActive()
        {
            _inputPanel.SetActive(!_inputPanel.activeSelf);
        }
    }
}