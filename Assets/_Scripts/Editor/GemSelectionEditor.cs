using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class GemSelectionEditor : Editor
{

    static GemSelectionEditor()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnSelectionChanged()
    {
        if (Selection.activeGameObject)
        {
            // Check if the selected GameObject has the desired script attached.
            var scriptComponent = Selection.activeGameObject.GetComponent<ObjectGenerator>();
            var scriptComponentInChildren = Selection.activeGameObject.GetComponentInChildren<ObjectGenerator>();
            if (scriptComponent || scriptComponentInChildren)
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
        {
            
        }
    }
}