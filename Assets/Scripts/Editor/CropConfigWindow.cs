using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CropConfigWindow : EditorWindow
{
    [MenuItem("Shennong/Crop Config Window")]
    public static void ShowWindow()
    {
        GetWindow<CropConfigWindow>("Crop Config");
    }

    private ListView _cropList;
    private ScrollView _detailView;
    private List<CropConfig> _allCrops;
    private const string CONFIG_PATH = "Assets/Config";

    private void OnEnable()
    {
        RefreshCropList();
    }

    private void CreateGUI()
    {
        var root = rootVisualElement;

        // Main Split View
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        root.Add(splitView);

        // -- Left Pane --
        var leftPane = new VisualElement();
        splitView.Add(leftPane);

        // Toolbar
        var toolbar = new VisualElement();
        toolbar.style.flexDirection = FlexDirection.Row;
        toolbar.style.paddingTop = 5;
        toolbar.style.paddingBottom = 5;
        
        var createBtn = new Button(CreateNewCrop) { text = "Create New Crop" };
        createBtn.style.flexGrow = 1;
        toolbar.Add(createBtn);
        
        var refreshBtn = new Button(RefreshUI) { text = "Refresh" };
        toolbar.Add(refreshBtn);

        leftPane.Add(toolbar);

        // List
        _cropList = new ListView();
        _cropList.style.flexGrow = 1;
        _cropList.makeItem = () => new Label();
        _cropList.bindItem = (element, i) => 
        {
            var label = element as Label;
            if (_allCrops != null && i < _allCrops.Count && _allCrops[i] != null)
                label.text = string.IsNullOrEmpty(_allCrops[i].CropName) ? _allCrops[i].name : _allCrops[i].CropName;
            else
                label.text = "null";
        };
        
        // Handle selection
        _cropList.selectionChanged += OnCropSelected;
        
        leftPane.Add(_cropList);

        // -- Right Pane --
        _detailView = new ScrollView();
        _detailView.style.paddingLeft = 10;
        _detailView.style.paddingRight = 10;
        _detailView.style.paddingTop = 10;
        splitView.Add(_detailView);

        RefreshUI();
    }

    private void RefreshUI()
    {
        RefreshCropList();
        if (_cropList != null)
        {
            _cropList.itemsSource = _allCrops;
            _cropList.Rebuild();
        }
    }

    private void RefreshCropList()
    {
        _allCrops = new List<CropConfig>();
        // Ensure path exists or fallback to searching everywhere if strict path fails
        string[] searchPaths = AssetDatabase.IsValidFolder(CONFIG_PATH) ? new[] { CONFIG_PATH } : null;
        
        var guids = AssetDatabase.FindAssets("t:CropConfig", searchPaths);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var crop = AssetDatabase.LoadAssetAtPath<CropConfig>(path);
            if (crop != null)
            {
                _allCrops.Add(crop);
            }
        }
    }

    private void OnCropSelected(IEnumerable<object> selectedItems)
    {
        _detailView.Clear();
        var selectedInfo = selectedItems.FirstOrDefault() as CropConfig;
        if (selectedInfo == null) return;

        // Use InspectorElement to show the default inspector UI
        var inspector = new InspectorElement(selectedInfo);
        _detailView.Add(inspector);
    }

    private void CreateNewCrop()
    {
        if (!AssetDatabase.IsValidFolder(CONFIG_PATH))
        {
            // If main config path missing, try to create it or default to Assets logic (omitted for brevity, assuming existing project structure)
             Debug.LogWarning($"Path {CONFIG_PATH} might not exist.");
        }

        var newCrop = ScriptableObject.CreateInstance<CropConfig>();
        newCrop.CropName = "New Crop";
        
        string path = AssetDatabase.GenerateUniqueAssetPath($"{CONFIG_PATH}/NewCrop.asset");
        
        AssetDatabase.CreateAsset(newCrop, path);
        AssetDatabase.SaveAssets();
        
        RefreshUI();
        
        // Select the new one
        int index = _allCrops.IndexOf(newCrop);
        if (index >= 0)
        {
            _cropList.selectedIndex = index;
        }
    }
}
