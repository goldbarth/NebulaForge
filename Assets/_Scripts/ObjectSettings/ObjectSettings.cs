using UnityEngine;

[CreateAssetMenu(fileName = "Object Settings", menuName = "ScriptableObjects/Object Settings")]
public class ObjectSettings : ScriptableObject
{
    [Space] public Material ObjectMaterial;
    [Space(3)] public VisualSettings VisualSettings;
    [Space(3)] public FaceRenderMask FaceRenderMask;
    public NoiseLayer[] NoiseLayers;
}
