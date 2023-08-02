using SolarSystem;
using UnityEngine;
using Extensions;
using Planet;
using System;

// Source: @ Infallible Code https://www.youtube.com/watch?v=_yf5vzZ2sYE&t=35s
// Modified by me. TODO: Refactor GameObjectSelectionHandler()
namespace UserUI
{
    public class SelectionManager : GenericSingleton<SelectionManager>
    {
        [SerializeField] private Texture _selectedTexture;
        [SerializeField] private LayerMask _layerMask;

        private Texture _objectTexture;
        private Transform _selection;
        private Camera _camera;
        
        private bool _isObjectSelected = false;
        
        public event Action OnObjectSelected;
        public event Action OnObjectDeselectedReady;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            GameObjectSelectionHandler();
            HandleEscapeInput();
        }

        private void GameObjectSelectionHandler()
        {
            if (_selection != null && !_isObjectSelected)
            {
                var highlightMaterial = SetHighlightMaterial(_selection);
                highlightMaterial.mainTexture = _objectTexture;
            }

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _layerMask))
            {
                var selection = hitInfo.transform;

                var highlightMaterial = SetHighlightMaterial(selection);
                _objectTexture = highlightMaterial.mainTexture;
                
                if (highlightMaterial != null)
                {
                    highlightMaterial.mainTexture = _selectedTexture;
                    if (Input.GetMouseButtonDown(0))
                        ActivateUserInterface();
                }

                _selection = selection;
            }
        }

        private static Material SetHighlightMaterial(Transform selection)
        {
            return selection.GetComponentInChildren<ObjectGenerator>().ObjectSettings.Material;
        }

        private void HandleEscapeInput()
        {
            if (_isObjectSelected && Input.GetKeyDown(KeyCode.Escape))
                DeselectObject();
        }
        
        private void DeselectObject()
        {
            _isObjectSelected = false;
            var selectionMaterial = SetHighlightMaterial(_selection);
            selectionMaterial.mainTexture = _objectTexture;
            _selection = null;
            OnObjectDeselectedReady?.Invoke();
            
        }
        
        private void ActivateUserInterface()
        { 
            _isObjectSelected = true;
            OnObjectSelected?.Invoke();
        }
        
        public CelestialObject SelectedObject()
        {
            return _selection != null ? _selection.GetComponent<CelestialObject>() : null;
        }
    }
}