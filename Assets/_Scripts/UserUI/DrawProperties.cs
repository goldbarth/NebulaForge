using System;
using SolarSystem;
using UnityEngine;
using TMPro;

namespace UserUI
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
            _timeScaleText.text = $"{_orbitSimulation.TimeScale}";
        }

        private void DrawPlanetValues()
        {
            SetPlanetStats();
            ResetPlanetStats();
        }
        
        private void SetPlanetStats()
        {
            if (_celestialObject == null) return;
            
            _magnitudeText.text = $"{_celestialObject.Velocity.magnitude}";
            _gravityText.text = $"{_celestialObject.SurfaceGravity}";
            _massText.text = $"{_celestialObject.ObjectMass}";
            _positionText.text = $"{_celestialObject.Position}";
        }
        
        private void ResetPlanetStats()
        {
            if(_celestialObject != null) return;
            
            _magnitudeText.text = "0.0";
            _gravityText.text = "0.0";
            _massText.text = "0.0";
            _positionText.text = "0.0";
        }

        private void SetSelectedPlanet()
        {
            _celestialObject = SelectionManager.Instance.SelectedObject();
        }
    }
}