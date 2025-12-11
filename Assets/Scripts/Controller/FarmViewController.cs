using System.Collections.Generic;
using QFramework;

using UnityEngine;
using UnityEngine.Tilemaps; 

public class FarmViewController : MonoBehaviour, IController
{
    [Header("References")]
    public GameObject cropPrefabTemplate; 
    public DualGridTilemap groundTilemap; 
    public Transform cropContainer;

    private ICropModel _cropModel;
    private IToolModel _toolModel; // New Reference
    
    private Dictionary<Vector3Int, GameObject> _spawnedCrops = new Dictionary<Vector3Int, GameObject>();

    private ICursorModel _cursorModel;
    private FarmSystem _farmSystem;

    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    void Start()
    {
        _cursorModel = this.GetModel<ICursorModel>();
        _cropModel = this.GetModel<ICropModel>();
        _cursorModel = this.GetModel<ICursorModel>();
        _cropModel = this.GetModel<ICropModel>();
        _toolModel = this.GetModel<IToolModel>(); // New Reference
        _farmSystem = this.GetSystem<FarmSystem>();

        this.RegisterEvent<OnCropUpdated>(e => UpdateCropGameObject(e.Position)).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void Update()
    {
        // Mouse Click Interaction
        if (Input.GetMouseButtonDown(0) && IsCursorInBounds())
        {
            HandleMouseClick();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (IsCursorInBounds()) PlantAtCursor();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (IsCursorInBounds()) HarvestAtCursor();
        }
        // Debug Tools
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (IsCursorInBounds()) _farmSystem.ModifyCropState(_cursorModel.CursorPos, "Moisture", 20);
            Debug.Log("Watering Crop...");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (IsCursorInBounds()) _farmSystem.ModifyCropState(_cursorModel.CursorPos, "Fertility", 20);
            Debug.Log("Fertilizing Crop...");
        }
    }

    private bool IsCursorInBounds()
    {
        Vector3 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        return Mathf.Abs(viewportPos.x - 0.5f) <= _cursorModel.ViewRatioX / 2f &&
               Mathf.Abs(viewportPos.y - 0.5f) <= _cursorModel.ViewRatioY / 2f;
    }

    private void HandleMouseClick()
    {
        Vector3Int mousePos = _cursorModel.CursorPos;
        Debug.Log($"[FarmView] Clicked at GridPos: {mousePos}. Current Tool: {_toolModel.CurrentTool.Value}");
        
        // 1. Check Tool
        if (_toolModel.CurrentTool.Value != ToolType.None)
        {
            Debug.Log($"[FarmView] Applying Tool {_toolModel.CurrentTool.Value} at {mousePos}");
            _farmSystem.ApplyTool(mousePos, _toolModel.CurrentTool.Value);
            return;
        }

        // 2. Default interaction
        if (_cropModel.SelectedSeedConfig.Value != null)
        {
            Debug.Log($"[FarmView] Planting {_cropModel.SelectedSeedConfig.Value.CropName} at {mousePos}");
            PlantAtCursor();
        }
    }

    private void PlantAtCursor()
    {
        if (_cropModel.SelectedSeedConfig.Value == null) 
        {
            Debug.Log("[FarmView] No seed selected.");
            return;
        }
        
        // Check if Tool is selected, prevent planting if Tool is active? 
        // Logic above handles priority: Tool > Plant
        
        CropConfig currentSeed = _cropModel.SelectedSeedConfig.Value;
        Vector3Int mousePos = _cursorModel.CursorPos;

        if (_cropModel.GetCrop(mousePos) != null) 
        {
            Debug.Log($"[FarmView] Position {mousePos} already has a crop.");
            return;
        }

        TileBase groundTile = groundTilemap.placeholderTilemap.GetTile(mousePos);
        if (groundTile == groundTilemap.dirtPlaceholderTile) 
        {
            _farmSystem.PlantCrop(mousePos, currentSeed);
        }
        else
        {
            Debug.Log($"[FarmView] Tile at {mousePos} is not dirt. Found: {groundTile?.name}, Expected: {groundTilemap.dirtPlaceholderTile?.name}");
        }
    }

    private void HarvestAtCursor()
    {
        Vector3Int mousePos = _cursorModel.CursorPos;
        _farmSystem.HarvestCrop(mousePos);
    }

    [Header("Alert Reference")]
    public GameObject iconPrefab; // Prefab with SpriteRenderer
    public Vector3 iconOffset = new Vector3(0.5f, 0.8f, 0); // Top Right

    [Header("Alert Sprites")]
    public Sprite iconTempHigh;
    public Sprite iconTempLow;
    public Sprite iconLightHigh;
    public Sprite iconLightLow;
    public Sprite iconMoistureHigh;
    public Sprite iconMoistureLow;
    public Sprite iconFertilityLow; // Usually no "High" fertility penalty?

    private void UpdateCropGameObject(Vector3Int pos)
    {
        CropData data = _cropModel.GetCrop(pos);
        if (data == null) 
        {
            if (_spawnedCrops.TryGetValue(pos, out GameObject oldObj))
            {
                Destroy(oldObj);
                _spawnedCrops.Remove(pos);
            }
            return;
        }

        GameObject cropObj;
        if (!_spawnedCrops.TryGetValue(pos, out cropObj))
        {
            Vector3 worldPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
            cropObj = Instantiate(cropPrefabTemplate, worldPos, Quaternion.identity, cropContainer);
            _spawnedCrops.Add(pos, cropObj);
        }

        var sr = cropObj.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = data.Config.GetSpriteByProgress(data.GrowthDays);
        }

        UpdateAlertIcon(cropObj, data);
    }

    private void UpdateAlertIcon(GameObject cropObj, CropData data)
    {
        Transform iconTrans = cropObj.transform.Find("AlertIcon");
        GameObject iconObj;
        SpriteRenderer iconSr;

        if (iconTrans == null)
        {
            if (iconPrefab == null) return;
            iconObj = Instantiate(iconPrefab, cropObj.transform);
            iconObj.name = "AlertIcon";
            iconObj.transform.localPosition = iconOffset;
            iconSr = iconObj.GetComponent<SpriteRenderer>();
        }
        else
        {
            iconObj = iconTrans.gameObject;
            iconSr = iconObj.GetComponent<SpriteRenderer>();
        }

        Sprite alertSprite = GetAlertSprite(data);
        if (alertSprite != null)
        {
            iconObj.SetActive(true);
            iconSr.sprite = alertSprite;
        }
        else
        {
            iconObj.SetActive(false);
        }
    }

    private Sprite GetAlertSprite(CropData data)
    {
        // Priority: Temp > Light > Fertility > Moisture
        
        // Temp
        if (data.CurrentState.Temp < data.Config.TempRange.Min) return iconTempLow;
        if (data.CurrentState.Temp > data.Config.TempRange.Max) return iconTempHigh;

        // Light
        if (data.CurrentState.Light < data.Config.LightRange.Min) return iconLightLow;
        if (data.CurrentState.Light > data.Config.LightRange.Max) return iconLightHigh;

        // Fertility
        if (data.CurrentState.Fertility < data.Config.FertilityRange.Min) return iconFertilityLow;

        // Moisture
        if (data.CurrentState.Moisture < data.Config.MoistureRange.Min) return iconMoistureLow;
        if (data.CurrentState.Moisture > data.Config.MoistureRange.Max) return iconMoistureHigh;

        return null;
    }
}