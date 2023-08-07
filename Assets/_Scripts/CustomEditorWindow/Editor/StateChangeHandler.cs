using HelpersAndExtensions;
using UnityEditor;
using UnityEngine;

namespace CustomEditorWindow
{
    public class StateChangeHandler : Editor
    {
        static StateChangeHandler()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            Debug.Log($"Play mode state changed: {stateChange}");
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    EnteredPlayMode();
                    Debug.Log("Entered Play Mode");
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    ExitedPlayMode();
                    Debug.Log("Exiting Play Mode");
                    break;
            }
        }
        
        private static void EnteredPlayMode()
        {
            StateChangeEventManager.RaiseModeStateChangeEvent();
        }
        
        private static void ExitedPlayMode()
        {
            StateChangeEventManager.RaiseModeStateChangeEvent();
        }
    }
}