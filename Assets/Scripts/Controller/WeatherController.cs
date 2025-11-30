using QFramework;
using UnityEngine;

public class WeatherController : MonoBehaviour, IController
{
    public ParticleSystem rainParticle;
    public ParticleSystem snowParticle;
    public ParticleSystem leavesParticle; // Windy
    
    private WeatherSystem _weatherSystem;

    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    void Start()
    {
        _weatherSystem = this.GetSystem<WeatherSystem>();
        
        _weatherSystem.CurrentWeather.Register(UpdateWeatherEffects).UnRegisterWhenGameObjectDestroyed(gameObject);
        UpdateWeatherEffects(_weatherSystem.CurrentWeather.Value);
    }

    private void UpdateWeatherEffects(WeatherType weather)
    {
        if (rainParticle) rainParticle.Stop();
        if (snowParticle) snowParticle.Stop();
        if (leavesParticle) leavesParticle.Stop();

        switch (weather)
        {
            case WeatherType.Rainy:
                if (rainParticle) rainParticle.Play();
                break;
            case WeatherType.Snowy:
                if (snowParticle) snowParticle.Play();
                break;
            case WeatherType.Windy:
                if (leavesParticle) leavesParticle.Play();
                break;
        }
    }
}
