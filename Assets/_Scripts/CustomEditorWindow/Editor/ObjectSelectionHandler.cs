using HelpersAndExtensions;
using UnityEditor;
using UnityEngine;
using Planet;

namespace CustomEditorWindow
{
    [InitializeOnLoad]
    public class ObjectSelectionHandler : Editor
    {
        static ObjectSelectionHandler()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            if (Selection.activeGameObject)
            {
                var component = Selection.activeGameObject.GetComponent<ObjectGenerator>();
                var componentInChildren = Selection.activeGameObject.GetComponentInChildren<ObjectGenerator>();
                
                if (component || componentInChildren)
                    RaiseSuccessEvent();
            }

            if (Selection.activeGameObject == null)
                RaiseNoSuccessEvent();
        }

        private static void RaiseSuccessEvent()
        {
            ObjectSelectionEventManager.RaiseObjectSelectedEvent();
        }

        private static void RaiseNoSuccessEvent()
        {
            ObjectSelectionEventManager.RaiseNoObjectSelectedEvent();
        }
    }
}
