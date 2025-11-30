using System.Collections;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour, IController
{
    public TextMeshProUGUI dayText;
    public Button nextDayButton; 
    public Button speedButton; 
    public TextMeshProUGUI speedText; 
    
    public bool autoPassTime = true; 
    public float secondsPerDay = 5.0f; 

    private ITermModel _termModel;
    private TimeSystem _timeSystem;

    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    void Start()
    {
        _termModel = this.GetModel<ITermModel>();
        _timeSystem = this.GetSystem<TimeSystem>();

        UpdateUI();

        _termModel.CurrentDayInTerm.Register(_ => UpdateUI()).UnRegisterWhenGameObjectDestroyed(gameObject);
        _timeSystem.CurrentTimeOfDay.Register(_ => UpdateUI()).UnRegisterWhenGameObjectDestroyed(gameObject);

        if (nextDayButton != null)
        {
            nextDayButton.onClick.AddListener(() =>
            {
                _timeSystem.AdvanceDay();
            });
        }

        if (speedButton != null)
        {
            speedButton.onClick.AddListener(() =>
            {
                float currentScale = _timeSystem.TimeScale;
                if (currentScale >= 4.0f) _timeSystem.TimeScale = 0.0f; // Pause
                else if (currentScale == 0.0f) _timeSystem.TimeScale = 1.0f;
                else _timeSystem.TimeScale *= 2.0f;
                
                UpdateSpeedUI();
            });
        }
        
        UpdateSpeedUI();
    }

    void Update()
    {
        if (autoPassTime)
        {
            float dayFraction = Time.deltaTime / secondsPerDay;
            _timeSystem.AdvanceTime(dayFraction);
        }
    }

    private void UpdateUI()
    {
        float time = _timeSystem.CurrentTimeOfDay.Value;
        int hour = Mathf.FloorToInt(time * 24);
        int minute = Mathf.FloorToInt((time * 24 - hour) * 60);
        
        dayText.text = $"{_termModel.CurrentTermName} 第{_termModel.CurrentDayInTerm.Value}天 {hour:00}:{minute:00}";
    }

    private void UpdateSpeedUI()
    {
        if (speedText != null)
        {
            float scale = _timeSystem.TimeScale;
            if (scale == 0) speedText.text = "暂停";
            else speedText.text = $"x{scale}";
        }
    }
}