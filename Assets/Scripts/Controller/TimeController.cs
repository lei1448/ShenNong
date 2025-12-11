using System.Collections;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour, IController
{
    public Button nextTermButton; 
    private TimeSystem _timeSystem;

    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    void Start()
    {
        _timeSystem = this.GetSystem<TimeSystem>();

        if (nextTermButton != null)
        {
            nextTermButton.onClick.AddListener(() =>
            {
                _timeSystem.AdvanceTerm(); 
            });
        }
    }
}
