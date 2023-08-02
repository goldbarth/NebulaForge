using System;

namespace Extensions
{
    /// <summary>
    /// The ObjectSelectionEventManager is communicating between the WindowView and the ObjectSelectionHandler(because it is an) and the ).
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