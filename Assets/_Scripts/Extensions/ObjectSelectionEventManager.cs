using System;

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