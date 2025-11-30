using QFramework;
using UnityEngine;

public class WeatherSystem : AbstractSystem
{
    public BindableProperty<WeatherType> CurrentWeather { get; set; } = new(WeatherType.Sunny);
    
    private WeatherConfig _config;
    private ITermModel _termModel;

    protected override void OnInit()
    {
        _termModel = this.GetModel<ITermModel>();
        
        // Load Config (In a real project, this might be injected or loaded from Resources)
        _config = Resources.Load<WeatherConfig>("WeatherConfig");
        
        this.RegisterEvent<OnDayPass>(e =>
        {
            UpdateWeather();
        });
    }

    private void UpdateWeather()
    {
        if (_config == null) return;
        
        string term = _termModel.CurrentTermName;
        WeatherType newWeather = _config.GetWeatherForTerm(term);
        
        // Special logic for specific terms as requested
        // "Qingming Rain" - High chance of rain if not configured
        if (term == "清明" && Random.value < 0.8f) newWeather = WeatherType.Rainy;
        // "Lidong Leaves" - Windy/Leaves
        if (term == "立冬" && Random.value < 0.8f) newWeather = WeatherType.Windy;

        CurrentWeather.Value = newWeather;
        Debug.Log($"[Weather] Term: {term}, Weather: {newWeather}");
    }
}
