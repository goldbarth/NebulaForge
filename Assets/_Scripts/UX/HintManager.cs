using UnityEngine;
using TMPro;

namespace UX
{
    public class HintManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hintText;
        
        [SerializeField] private string _startHintText = "Hover over a planet to see its stats.";
        [SerializeField] private string _selectObjectHintText = "Click to select a planet.";
        [SerializeField] private string _adjustSettingsHintText = "In the left upper corner. Use the sliders to change the planet's properties.";
        [SerializeField] private string _deselectHintText = "Press ESC to deselect the planet and hide the planet settings.";
        
        private UserInput _userInput;
        
        private bool _hasTutorialEnded = false;

        private void Awake()
        {
            _userInput = FindObjectOfType<UserInput>();
        }

        private void Start()
        {
            SetHintText(_startHintText);
        }
        
        private void OnEnable()
        {
            SelectionManager.Instance.OnHoverOverObject += HoverOverObject;
            SelectionManager.Instance.OnObjectSelected += ActiveObjectSelected;
            SelectionManager.Instance.OnObjectDeselectedReady += HideHintText;
            _userInput.OnSliderChanged += ActiveUserInterface;
        }

        private void OnDisable()
        {
            SelectionManager.Instance.OnHoverOverObject -= HoverOverObject;
            SelectionManager.Instance.OnObjectSelected -= ActiveObjectSelected;
            SelectionManager.Instance.OnObjectDeselectedReady -= HideHintText;
            _userInput.OnSliderChanged -= ActiveUserInterface;
        }

        private void HoverOverObject()
        {
            if (_hasTutorialEnded) return;
            SetHintText(_selectObjectHintText);
        }
        
        private void ActiveObjectSelected()
        {
            if (_hasTutorialEnded) return;
            SetHintText(_adjustSettingsHintText);
        }
        
        private void ActiveUserInterface()
        {
            if (_hasTutorialEnded) return;
            SetHintText(_deselectHintText);
        }
        
        private void HideHintText()
        {
            SetHintText(string.Empty);
            _hasTutorialEnded = true;
        }

        private void SetHintText(string text)
        {
            _hintText.text = text;
        }
    }
}