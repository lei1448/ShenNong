using System.Collections.Generic;
using UnityEngine;

public enum WeatherType
{
    Sunny,
    Rainy,
    Snowy,
    Cloudy,
    Windy // Falling leaves
}

[System.Serializable]
public class WeatherEntry
{
    public string TermName; // Solar Term Name
    public WeatherType Weather;
    public float Probability; // 0.0 - 1.0
}

[CreateAssetMenu(fileName = "WeatherConfig", menuName = "Shennong/WeatherConfig")]
public class WeatherConfig : ScriptableObject
{
    public List<WeatherEntry> WeatherEntries;

    public WeatherType GetWeatherForTerm(string termName)
    {
        // Default to Sunny
        
        // Find all entries for this term
        List<WeatherEntry> entries = WeatherEntries.FindAll(e => e.TermName == termName);
        
        if (entries.Count == 0) return WeatherType.Sunny;

        float roll = Random.value;
        float cumulative = 0f;

        foreach (var entry in entries)
        {
            cumulative += entry.Probability;
            if (roll <= cumulative)
            {
                return entry.Weather;
            }
        }
        
        return WeatherType.Sunny;
    }
}
