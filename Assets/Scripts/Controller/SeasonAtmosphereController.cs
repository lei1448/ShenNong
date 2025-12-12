using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class SeasonAtmosphereController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    private Image _leftBranch;
    private Image _rightBranch;
    private ITermModel _termModel;

    private void Start()
    {
        _termModel = this.GetModel<ITermModel>();
        
        SetupUI();
        
        // Initial update
        UpdateAtmosphere(_termModel.GetSeason());

        // Listen for term changes
        _termModel.CurrentTermId.Register(termId =>
        {
             UpdateAtmosphere(_termModel.GetSeason());
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
            
        _leftBranch = CreateBranchImage("LeftImage", leftMask.transform);
        // Align Top-Center of the mask, Scale up
        var leftRect = _leftBranch.rectTransform;
        leftRect.anchorMin = new Vector2(0.5f, 1f);
        leftRect.anchorMax = new Vector2(0.5f, 1f);
        leftRect.pivot = new Vector2(0.5f, 1f); // Top Pivot
        leftRect.anchoredPosition = new Vector2(0, 0); // Top of container
        leftRect.sizeDelta = new Vector2(imageSize, imageSize);

        // Right Side
        var rightMask = CreateMaskContainer("RightMask", container.transform, 
            new Vector2(1f - sideWidthPercent, 0f), 
            new Vector2(1f, 1f));

        _rightBranch = CreateBranchImage("RightImage", rightMask.transform);
        var rightRect = _rightBranch.rectTransform;
        rightRect.anchorMin = new Vector2(0.5f, 1f);
        rightRect.anchorMax = new Vector2(0.5f, 1f);
        rightRect.pivot = new Vector2(0.5f, 1f);
        rightRect.anchoredPosition = new Vector2(0, 0);
        rightRect.sizeDelta = new Vector2(imageSize, imageSize);
        
        // Mirror Right
        rightRect.localScale = new Vector3(-1, 1, 1);
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
        
        // Add Mask
        var maskImage = go.AddComponent<Image>();
        maskImage.color = new Color(0,0,0,0); // Invisible but needed for mask? 
        // Actually RectMask2D doesn't need an image, but it clips children to rect.
        // RectMask2D is more performant and doesn't require an image component usually, 
        // but standard Mask does. Let's use RectMask2D.
        DestroyImmediate(maskImage); // removal just in case
        
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
        return img;
    }

    private void UpdateAtmosphere(Season season)
    {
        string resourceName = GetSeasonResourceName(season);
        var sprite = Resources.Load<Sprite>($"Nongzuowu/树枝/{resourceName}");
        
        if (sprite != null)
        {
            _leftBranch.sprite = sprite;
            _rightBranch.sprite = sprite;
            _leftBranch.gameObject.SetActive(true);
            _rightBranch.gameObject.SetActive(true);
            
            // Adjust to native size but keep within reasonable bounds if needed
            // _leftBranch.SetNativeSize(); 
            // _rightBranch.SetNativeSize();
        }
        else
        {
            // If resource missing, hide to avoid white boxes
            _leftBranch.gameObject.SetActive(false);
            _rightBranch.gameObject.SetActive(false);
            Debug.LogWarning($"Could not find season branch sprite: Nongzuowu/树枝/{resourceName}");
        }
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
