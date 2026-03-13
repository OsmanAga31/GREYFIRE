[System.Serializable]
public class SpreadSettings
{
    public bool enabled; // Whether spread shot is enabled
    public int count; // Number of projectiles in the spread
    public float angle; // Angle of the spread; Use values lower than 1...
}

[System.Serializable]
public class AOESettings
{
    public bool enabled; // Whether area of effect damage is enabled
    public float radius; // Radius of the area of effect damage
}