#if UNITY_EDITOR

using Extensions;
using UnityEditor;
using UnityEngine;

namespace Planet
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
            Debug.Log("Selected GameObject in Hierarchy has the generator script attached.");
        }

        private static void RaiseNoSuccessEvent()
        {
            ObjectSelectionEventManager.RaiseNoObjectSelectedEvent();
            Debug.Log("There is no GameObject in the Hierarchy selected or the GameObject has no generator script attached.");
        }
    }
}

#endif
