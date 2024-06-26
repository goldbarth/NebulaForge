using HelpersAndExtensions;
using SolarSystem;
using UnityEngine;
using System;
using Planet;

// Source: @ Infallible Code https://www.youtube.com/watch?v=_yf5vzZ2sYE&t=35s
// Modified the basics and expanded a lot.
namespace UX
{
    public class SelectionManager : GenericSingleton<SelectionManager>
    {
        [Tooltip("If true, the user can only select objects by looking at them.")]
        [SerializeField] private bool _usingCenterDotInteraction;
        [SerializeField] private Texture _selectedTexture;
        [SerializeField] private LayerMask _layerMask = 1 << 8;

        private Texture _objectTexture;
        private Transform _selection;
        private Camera _camera;
        
        private bool _isObjectSelected;
        
        public bool IsGamePaused;
        
        public event Action OnObjectSelected;
        public event Action OnObjectDeselectedReady;
        public event Action OnHoverOverObject;
        

        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
        }

        private void Update()
        {
            if (IsGamePaused) return;
            GameObjectSelectionHandler();
            HandleTabInput();
        }

        private void GameObjectSelectionHandler()
        {
            if (IsObjectNotSelectedYet())
                SetHighlightMaterialToObjectTexture();

            if (IsObjectSelected())
                return;
            
            if (_usingCenterDotInteraction)
                CenterDotInteraction();
            else
                MouseInteraction();
        }

        private void MouseInteraction()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask))
                Selection(hit);
        }
        
        private void CenterDotInteraction()
        {
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask))
                Selection(hit);
        }

        private void Selection(RaycastHit hit)
        {
            var selection = hit.transform;

            var highlightMaterial = SetHighlightMaterial(selection);
            SetHighlightMaterialToObjectTexture(highlightMaterial);

            if (highlightMaterial != null)
            {
                SetSelectionTextureToHighlightMaterial(highlightMaterial);

                if (Input.GetMouseButtonDown(0))
                    ActivateUserInterface();
            }

            _selection = selection;
        }

        private void SetSelectionTextureToHighlightMaterial(Material highlightMaterial)
        {
            highlightMaterial.mainTexture = _selectedTexture;
        }

        private void SetHighlightMaterialToObjectTexture(Material highlightMaterial)
        {
            OnHoverOverObject?.Invoke();
            _objectTexture = highlightMaterial.mainTexture;
        }

        private void SetHighlightMaterialToObjectTexture()
        {
            var highlightMaterial = SetHighlightMaterial(_selection);
            highlightMaterial.mainTexture = _objectTexture;
        }

        private bool IsObjectSelected()
        {
            return _selection != null && _isObjectSelected;
        }

        private bool IsObjectNotSelectedYet()
        {
            return _selection != null && !_isObjectSelected;
        }

        private static Material SetHighlightMaterial(Transform selection)
        {
            return selection.GetComponentInChildren<ObjectGenerator>().ObjectSettings.Material;
        }

        private void HandleTabInput()
        {
            if (_isObjectSelected && Input.GetKeyDown(KeyCode.Tab))
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