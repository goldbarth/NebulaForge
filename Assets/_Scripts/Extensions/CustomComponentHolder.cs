using UnityObject = UnityEngine.Object;
using Unity.VisualScripting;
using UnityEngine;

public static class CustomComponentHolder
{
    // You can't use ?. and ?? operators on the Unity Object instances.
    // This is due to the fact that == and != operator is overriden by Unity.
    // Whereas compiler uses default == / != operators by default when using ?. / ?? operators.
    // Source: https://blogs.unity3d.com/ru/2014/05/16/custom-operator-should-we-keep-it/

    public static T GetOrAddComponent2<T>(this UnityObject uo) where T : Component
    {
        var temp = uo.GetComponent<T>();
        if (temp == null) temp = uo.AddComponent<T>();

        return temp;
    }
}