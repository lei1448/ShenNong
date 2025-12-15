using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class NextRoundController : MonoBehaviour, IController
{
    private TimeSystem _timeSystem;
    private ITermModel _termModel;

    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    void Start()
    {
        _timeSystem = this.GetSystem<TimeSystem>();
        _termModel = this.GetModel<ITermModel>();

        // Find the button in the scene
        var nextRoundObj = GameObject.Find("NextRound");
        if (nextRoundObj != null)
        {
            var btn = nextRoundObj.GetComponent<Button>();
            if (btn != null)
            {
                // Remove existing listeners to be safe/clean reloads
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnNextRoundClicked);
            }
            else
            {
                Debug.LogWarning("[NextRoundController] GameObject 'NextRound' found but has no Button component.");
            }
        }
        else
        {
            Debug.LogWarning("[NextRoundController] Could not find GameObject named 'NextRound'.");
        }
    }

    private void OnNextRoundClicked()
    {
        Debug.Log("[NextRound] Advance Term clicked.");
        _timeSystem.AdvanceTerm();
    }
}
