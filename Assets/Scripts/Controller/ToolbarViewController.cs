using QFramework;
using UnityEngine;

using UnityEngine.UI;

public class ToolbarViewController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    private IToolModel _toolModel;
    
    private GameObject _panel;

    void Start()
    {
        _toolModel = this.GetModel<IToolModel>();
        
        // Setup UI
        CreateToolbarUI();
    }

    private void CreateToolbarUI()
    {
        // Create Canvas if not exists (or find existing)
        // For simplicity, we create a small panel at bottom center
        
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        _panel = new GameObject("ToolbarPanel");
        _panel.transform.SetParent(canvasObj.transform, false);
        
        RectTransform rt = _panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f); // Left Center
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.pivot = new Vector2(0, 0.5f);
        rt.anchoredPosition = new Vector2(50, 50); // Moved Left (80->50) and Up (0->50)
        rt.sizeDelta = new Vector2(100, 300);

        VerticalLayoutGroup layout = _panel.AddComponent<VerticalLayoutGroup>(); // Vertical
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 20;
        layout.childControlHeight = false;
        layout.childControlWidth = false;

        CreateToolButton("WateringCan", ToolType.WateringCan, "Icons/WateringCan");
        CreateToolButton("Fertilizer", ToolType.Fertilizer, "Icons/Fertilizer");
        CreateToolButton("Hoe", ToolType.Hoe, "Icons/Hoe");
        
        // Add a "None" / Deselect button or handle it differently?
        // Let's add a clear button or just right click to clear?
        // For now 3 tools.
    }

    private void CreateToolButton(string name, ToolType toolType, string iconPath)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(_panel.transform, false);
        
        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0.5f); // Semi transparent default
        
        RectTransform rt = buttonObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);
        
        Button btn = buttonObj.AddComponent<Button>();
        
        // Load Icon
        Sprite icon = Resources.Load<Sprite>(iconPath);
        if (icon == null)
        {
            Texture2D tex = Resources.Load<Texture2D>(iconPath);
            if (tex != null)
            {
                icon = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        
        if (icon != null)
        {
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(buttonObj.transform, false);
            Image iconImg = iconObj.AddComponent<Image>();
            iconImg.sprite = icon;
            RectTransform iconRt = iconObj.GetComponent<RectTransform>();
            iconRt.anchorMin = Vector2.zero;
            iconRt.anchorMax = Vector2.one;
            iconRt.offsetMin = new Vector2(10, 10);
            iconRt.offsetMax = new Vector2(-10, -10);
            iconImg.raycastTarget = false; // Let button handle click
        }
        
        // Select logic
        btn.onClick.AddListener(() =>
        {
            if (_toolModel.CurrentTool.Value == toolType)
            {
                _toolModel.SelectTool(ToolType.None); // Toggle off
            }
            else
            {
                _toolModel.SelectTool(toolType);
            }
        });

        // Reactive State for visual
        _toolModel.CurrentTool.RegisterWithInitValue(current => 
        {
            if (current == toolType)
            {
                img.color = Color.green; // Selected highlight
            }
            else
            {
                img.color = new Color(1, 1, 1, 0.5f); // Normal
            }
        }).UnRegisterWhenGameObjectDestroyed(buttonObj);
    }
}
