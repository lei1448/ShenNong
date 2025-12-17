using QFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SeasonAtmosphereController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    // Dual layers for crossfade
    private Image _leftBranchCurrent;
    private Image _leftBranchNext;
    
    private Image _rightBranchCurrent;
    private Image _rightBranchNext;
    
    private ITermModel _termModel;
    private Coroutine _transitionCoroutine;
    private Season _currentSeason;
    
    private const float FADE_DURATION = 1.0f;

    private void Start()
    {
        _termModel = this.GetModel<ITermModel>();
        
        SetupUI();
        
        // Initial set (no fade)
        _currentSeason = _termModel.GetSeason();
        SetSeasonImmediate(_currentSeason);

        // Listen for term changes
        _termModel.CurrentTermId.Register(termId =>
        {
             var newSeason = _termModel.GetSeason();
             if (newSeason != _currentSeason)
             {
                 _currentSeason = newSeason;
                 UpdateAtmosphere(newSeason);
             }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void SetupUI()
    {
        // Find or create Canvas
        var canvas = FindObjectOfType<Canvas>();
        
        GameObject canvasGo = GameObject.Find("AtmosphereCanvas");
        if (canvasGo == null)
        {
            canvasGo = new GameObject("AtmosphereCanvas");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
        }
        else
        {
            canvas = canvasGo.GetComponent<Canvas>();
        }
        
        // Sorting order -10 to be behind everything
        canvas.sortingOrder = -10;

        foreach (Transform child in canvas.transform)
        {
            if (child.name == "SeasonalAtmosphere")
            {
                DestroyImmediate(child.gameObject);
            }
        }

        // Main Container
        var container = new GameObject("SeasonalAtmosphere");
        container.transform.SetParent(canvas.transform, false);
        var rect = container.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        
        var group = container.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        // Configuration
        float sideWidthPercent = 0.28f; // Approx 28% width for the "Red Box" area
        float imageSize = 1800f; // Large size to Ensure we zoom in and push trunk off-screen

        // Left Side
        var leftMask = CreateMaskContainer("LeftMask", container.transform, 
            new Vector2(0f, 0f), 
            new Vector2(sideWidthPercent, 1f));
            
        // Create Two Layers for Left
        _leftBranchCurrent = CreateBranchImage("LeftImage_Current", leftMask.transform);
        _leftBranchNext = CreateBranchImage("LeftImage_Next", leftMask.transform);
        
        ConfigureBranchRect(_leftBranchCurrent, imageSize);
        ConfigureBranchRect(_leftBranchNext, imageSize);

        // Right Side
        var rightMask = CreateMaskContainer("RightMask", container.transform, 
            new Vector2(1f - sideWidthPercent, 0f), 
            new Vector2(1f, 1f));

        // Create Two Layers for Right
        _rightBranchCurrent = CreateBranchImage("RightImage_Current", rightMask.transform);
        _rightBranchNext = CreateBranchImage("RightImage_Next", rightMask.transform);
        
        ConfigureBranchRect(_rightBranchCurrent, imageSize);
        ConfigureBranchRect(_rightBranchNext, imageSize);
        
        // Mirror Right
        _rightBranchCurrent.rectTransform.localScale = new Vector3(-1, 1, 1);
        _rightBranchNext.rectTransform.localScale = new Vector3(-1, 1, 1);
    }
    
    private void ConfigureBranchRect(Image img, float size)
    {
        var rt = img.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f); // Top Pivot
        rt.anchoredPosition = new Vector2(0, 0); // Top of container
        rt.sizeDelta = new Vector2(size, size);
    }

    private GameObject CreateMaskContainer(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        
        var mask = go.AddComponent<RectMask2D>();
        mask.softness = new Vector2Int(30, 0);
        
        return go;
    }

    private Image CreateBranchImage(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.raycastTarget = false;
        img.preserveAspect = true;
        
        // Default transparent
        var c = img.color;
        c.a = 0;
        img.color = c;
        
        return img;
    }

    private void SetSeasonImmediate(Season season)
    {
        string resourceName = GetSeasonResourceName(season);
        var sprite = Resources.Load<Sprite>($"Nongzuowu/树枝/{resourceName}");
        
        if (sprite != null)
        {
            _leftBranchCurrent.sprite = sprite;
            _rightBranchCurrent.sprite = sprite;
            
            SetAlpha(_leftBranchCurrent, 1f);
            SetAlpha(_rightBranchCurrent, 1f);
            SetAlpha(_leftBranchNext, 0f);
            SetAlpha(_rightBranchNext, 0f); // FIXED: Was _rightBranchCurrent
        }
    }

    private void UpdateAtmosphere(Season season)
    {
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
        }
        _transitionCoroutine = StartCoroutine(TransitionRoutine(season));
    }

    private IEnumerator TransitionRoutine(Season targetSeason)
    {
        string resourceName = GetSeasonResourceName(targetSeason);
        var newSprite = Resources.Load<Sprite>($"Nongzuowu/树枝/{resourceName}");

        if (newSprite == null)
        {
            Debug.LogWarning($"Could not find season branch sprite: Nongzuowu/树枝/{resourceName}");
            yield break;
        }

        // Prepare Next layer: Alpha 0, correct sprite
        _leftBranchNext.sprite = newSprite;
        _rightBranchNext.sprite = newSprite;
        SetAlpha(_leftBranchNext, 0f);
        SetAlpha(_rightBranchNext, 0f);
        
        // Ensure Next is rendered ON TOP of Current so the fade-in is visible
        _leftBranchNext.rectTransform.SetAsLastSibling();
        _rightBranchNext.rectTransform.SetAsLastSibling();
        
        // Ensure Current is fully visible (Constant background)
        SetAlpha(_leftBranchCurrent, 1f);
        SetAlpha(_rightBranchCurrent, 1f);
        
        float timer = 0f;
        while (timer < FADE_DURATION)
        {
            timer += Time.deltaTime;
            float t = timer / FADE_DURATION;
            
            // Fade NEXT in (0 -> 1)
            SetAlpha(_leftBranchNext, t);
            SetAlpha(_rightBranchNext, t);
            
            // Keep CURRENT at 1
            SetAlpha(_leftBranchCurrent, 1f);
            SetAlpha(_rightBranchCurrent, 1f);
            
            yield return null;
        }

        // Finalize state
        // Set Next to 1
        SetAlpha(_leftBranchNext, 1f);
        SetAlpha(_rightBranchNext, 1f);
        
        // Disable Current (it's now covered)
        SetAlpha(_leftBranchCurrent, 0f);
        SetAlpha(_rightBranchCurrent, 0f);
        
        // Swap references
        var tempL = _leftBranchCurrent;
        _leftBranchCurrent = _leftBranchNext;
        _leftBranchNext = tempL;
        
        var tempR = _rightBranchCurrent;
        _rightBranchCurrent = _rightBranchNext;
        _rightBranchNext = tempR;
    }

    private void SetAlpha(Image img, float alpha)
    {
        if (img == null) return;
        var c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private string GetSeasonResourceName(Season season)
    {
        return season switch
        {
            Season.Spring => "春",
            Season.Summer => "夏",
            Season.Autumn => "秋",
            Season.Winter => "冬",
            _ => "春"
        };
    }
}
