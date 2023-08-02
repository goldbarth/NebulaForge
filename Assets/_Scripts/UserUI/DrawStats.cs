using SolarSystem;
using UnityEngine;
using TMPro;

namespace UserUI
{
    public class DrawStats : MonoBehaviour
    {
        [SerializeField] private TMP_Text _positionText;
        [SerializeField] private TMP_Text _magnitudeText;
        [SerializeField] private TMP_Text _gravityText;
        [SerializeField] private TMP_Text _massText;
        
        private CelestialObject _celestialObject;

        private void Update()
        {
            _celestialObject = SelectionManager.Instance.SelectedObject();
            
            SetStats();
            ResetStats();
        }

        private void SetStats()
        {
            if (_celestialObject == null) return;
            
            _magnitudeText.text = $"{_celestialObject.Velocity.magnitude}";
            _gravityText.text = $"{_celestialObject.SurfaceGravity}";
            _massText.text = $"{_celestialObject.ObjectMass}";
            _positionText.text = $"{_celestialObject.Position}";
        }
        
        private void ResetStats()
        {
            if(_celestialObject != null) return;
            
            _magnitudeText.text = "0.0";
            _gravityText.text = "0.0";
            _massText.text = "0.0";
            _positionText.text = "0.0";
        }
    }
}