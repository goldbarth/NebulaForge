// a class that holds all the settings for the noise to manipulate the shape/surface of the planet
[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Rigid }
    public FilterType Filter;
    
    [ConditionalHide("Filter", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("Filter", 1)]
    public RidgidNoiseSettings ridgidNoiseSettings;
}