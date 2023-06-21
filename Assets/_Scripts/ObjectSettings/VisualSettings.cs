using UnityEngine;

[System.Serializable]
public class VisualSettings
{
    [SerializeField, Range(2, 255)] public int Resolution = 2;
    [SerializeField, Range(0, 5000)] public float PlanetRadius = 1000f;
    [Space] public Gradient Gradient;
    [Space] public ObjectType ObjectType;
}