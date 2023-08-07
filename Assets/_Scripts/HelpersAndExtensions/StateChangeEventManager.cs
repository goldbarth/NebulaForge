using System;

namespace HelpersAndExtensions
{
    /// <summary>
    /// The StateChangeEventManager is communicating between the View
    /// and the StateChangeHandler, because classes in the editor folder can´t directly do it.
    /// </summary>
    public static class StateChangeEventManager
    {
        public static event Action OnPlayModeStateChanged;
        public static void RaiseModeStateChangeEvent()
        {
            OnPlayModeStateChanged?.Invoke();
        }
    }
}