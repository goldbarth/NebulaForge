using System;

namespace Extensions
{
    /// <summary>
    /// The ObjectSelectionEventManager is communicating between the WindowView
    /// and the ObjectSelectionHandle, because classes in the editor folder can´t directly do it.
    /// </summary>
    public static class ObjectSelectionEventManager
    {
        public static event Action OnObjectSelected;
        public static event Action OnNoObjectSelected;

        public static void RaiseObjectSelectedEvent()
        {
            OnObjectSelected?.Invoke();
        }

        public static void RaiseNoObjectSelectedEvent()
        {
            OnNoObjectSelected?.Invoke();
        }
    }
}