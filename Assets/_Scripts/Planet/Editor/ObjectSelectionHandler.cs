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
                // Check if the selected GameObject has the desired script attached.
                var component = Selection.activeGameObject.GetComponent<ObjectGenerator>();
                var componentInChildren = Selection.activeGameObject.GetComponentInChildren<ObjectGenerator>();
                if (component || componentInChildren)
                {
                    ObjectSelectionEventManager.RaiseObjectSelectedEvent();
                    Debug.Log("Selected GameObject has the desired script attached.");
                }
            }

            if (Selection.activeGameObject == null)
            {
                ObjectSelectionEventManager.RaiseNoObjectSelectedEvent();
                Debug.Log("Selected GameObject has no desired script attached.");
            }
        }
    }
}

#endif
