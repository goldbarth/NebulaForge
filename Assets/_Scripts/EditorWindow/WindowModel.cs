using UnityEditor;

public class WindowModel
{
    private readonly ObjectSettings _data;
    private SerializedObject _serializedObject;

    public WindowModel(ObjectGenerator objectGenerator)
    {
        _data = objectGenerator.ObjectSettings;
    }

    public void UpdateSerializedObject()
    {
        _serializedObject = new SerializedObject(_data);
    }
}