using QFramework;
using UnityEngine;
// using UnityEngine.Rendering.Universal; // Uncomment if using URP Light2D

public class DayNightController : MonoBehaviour, IController
{
    public Light globalLight; // Assign Directional Light here
    // public Light2D globalLight2D; // Assign 2D Global Light here if URP

    public Gradient dayNightColor; // Gradient for light color
    public AnimationCurve dayNightIntensity; // Curve for light intensity

    private TimeSystem _timeSystem;

    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    void Start()
    {
        _timeSystem = this.GetSystem<TimeSystem>();
        
        // Default Gradient if not set
        if (dayNightColor == null || dayNightColor.colorKeys.Length == 0)
        {
            dayNightColor = new Gradient();
            dayNightColor.colorKeys = new GradientColorKey[] {
                new GradientColorKey(new Color(0.1f, 0.1f, 0.2f), 0.0f), // Night
                new GradientColorKey(new Color(0.1f, 0.1f, 0.2f), 0.2f), // Night
                new GradientColorKey(new Color(1.0f, 0.95f, 0.8f), 0.3f), // Sunrise
                new GradientColorKey(Color.white, 0.5f), // Noon
                new GradientColorKey(new Color(1.0f, 0.8f, 0.6f), 0.7f), // Sunset
                new GradientColorKey(new Color(0.1f, 0.1f, 0.2f), 0.8f), // Night
                new GradientColorKey(new Color(0.1f, 0.1f, 0.2f), 1.0f) // Night
            };
            dayNightColor.alphaKeys = new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 1.0f)
            };
        }
        
        // Default Intensity if not set
        if (dayNightIntensity == null || dayNightIntensity.length == 0)
        {
            dayNightIntensity = new AnimationCurve(
                new Keyframe(0.0f, 0.2f),
                new Keyframe(0.2f, 0.2f),
                new Keyframe(0.3f, 0.8f),
                new Keyframe(0.5f, 1.0f),
                new Keyframe(0.7f, 0.8f),
                new Keyframe(0.8f, 0.2f),
                new Keyframe(1.0f, 0.2f)
            );
        }

        _timeSystem.CurrentTimeOfDay.Register(UpdateLighting).UnRegisterWhenGameObjectDestroyed(gameObject);
        UpdateLighting(_timeSystem.CurrentTimeOfDay.Value);
    }

    private void UpdateLighting(float time)
    {
        Color color = dayNightColor.Evaluate(time);
        float intensity = dayNightIntensity.Evaluate(time);

        if (globalLight != null)
        {
            globalLight.color = color;
            globalLight.intensity = intensity;
        }
        
        // if (globalLight2D != null)
        // {
        //     globalLight2D.color = color;
        //     globalLight2D.intensity = intensity;
        // }
    }
}
