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
    
    private Dictionary<Vector3Int, GameObject> _spawnedCrops = new Dictionary<Vector3Int, GameObject>();

    private ICursorModel _cursorModel;
    private FarmSystem _farmSystem;

    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    void Start()
    {
        _cursorModel = this.GetModel<ICursorModel>();
        _cropModel = this.GetModel<ICropModel>();
        _farmSystem = this.GetSystem<FarmSystem>();

        this.RegisterEvent<OnCropUpdated>(e => UpdateCropGameObject(e.Position)).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlantAtCursor();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            HarvestAtCursor();
        }
    }

    private void PlantAtCursor()
    {
        if (_cropModel.SelectedSeedConfig.Value == null) 
        {
            Debug.Log("[FarmView] No seed selected.");
            return;
        }
        
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

    private void UpdateCropGameObject(Vector3Int pos)
    {
        CropData data = _cropModel.GetCrop(pos);
        if (data == null) 
        {
            // If data is null but we have a spawned object, destroy it (removed crop)
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
    }
}