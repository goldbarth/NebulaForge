using SolarSystem;
using UnityEngine;
using TMPro;

namespace UX
{
    public class DrawProperties : MonoBehaviour
    {
        [SerializeField] private TMP_Text _positionText;
        [SerializeField] private TMP_Text _magnitudeText;
        [SerializeField] private TMP_Text _gravityText;
        [SerializeField] private TMP_Text _massText;
        [SerializeField] private TMP_Text _timeScaleText;

        private CelestialObject _celestialObject;
        private OrbitSimulation _orbitSimulation;

        private const string FixedTimeScale = "10"; 

        private void Awake()
        {
            _orbitSimulation = OrbitSimulation.Instance;
        }

        private void Update()
        {
            SetSelectedPlanet();
            DrawTimeScaleValue();
            DrawPlanetValues();
        }

        private void DrawTimeScaleValue()
        {
            if (!_orbitSimulation.ManualTimeScale)
                _timeScaleText.text = FixedTimeScale;
            
            _timeScaleText.text = $"{_orbitSimulation.TimeScale}";
        }

        private void DrawPlanetValues()
        {
            SetPlanetStats();
        }
        
        private void SetPlanetStats()
        {
            if (_celestialObject == null) return;
            
            _magnitudeText.text = $"{_celestialObject.Velocity.magnitude}";
            _gravityText.text = $"{_celestialObject.SurfaceGravity}";
            _massText.text = $"{_celestialObject.ObjectMass}";
            _positionText.text = $"{_celestialObject.Position}";
        }

        private void SetSelectedPlanet()
        {
            _celestialObject = SelectionManager.Instance.SelectedObject();
        }
    }
}