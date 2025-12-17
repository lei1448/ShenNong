using QFramework;
using UnityEngine;
using TMPro;

public class ScoreViewController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    private TMP_Text _scoreText;

    private void Awake()
    {
        _scoreText = GetComponent<TMP_Text>();
        if (_scoreText == null)
        {
            // Fallback: Try to find "Grade" directly or "Canvas/Grade"
            var gradeObj = GameObject.Find("Grade"); // Try finding by unique name first
            if (gradeObj == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    var t = canvas.transform.Find("Grade");
                    if (t != null) gradeObj = t.gameObject;
                }
            }

            if (gradeObj != null)
            {
                _scoreText = gradeObj.GetComponent<TMP_Text>();
            }
        }
        
        if (_scoreText == null)
        {
            Debug.LogError("[ScoreViewController] Could not find TMP_Text component on this object or 'Canvas/Grade'.");
        }
    }

    private void Start()
    {
        this.RegisterEvent<OnTermScoreSettled>(OnTermScoreSettled);
        UpdateScoreDisplay(0); // Init with 0 or empty
    }

    private void OnTermScoreSettled(OnTermScoreSettled e)
    {
        UpdateScoreDisplay(e.Score);
    }

    private void UpdateScoreDisplay(int score)
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"{score}";
        }
    }
    
    private void OnDestroy()
    {
        this.UnRegisterEvent<OnTermScoreSettled>(OnTermScoreSettled);
    }
}
