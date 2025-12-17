using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SeasonTilemapController : MonoBehaviour, IController
{
    public DualGridTilemap targetTilemap;
    
    // Assign these in Inspector
    public Tile[] SpringTiles;
    public Tile[] SummerTiles;
    public Tile[] AutumnTiles;
    public Tile[] WinterTiles;

    private ITermModel _termModel;
    private Season _currentSeason;

    public IArchitecture GetArchitecture()
    {
        return ShennongAlmanac.Interface;
    }

    void Start()
    {
        _termModel = this.GetModel<ITermModel>();
        
        if (targetTilemap == null)
        {
            targetTilemap = FindObjectOfType<DualGridTilemap>();
            if (targetTilemap != null)
            {
                Debug.Log("[SeasonTilemapController] Auto-found DualGridTilemap reference.");
            }
            else
            {
                Debug.LogError("[SeasonTilemapController] DualGridTilemap reference missing and could not be found in scene!");
            }
        }
        
        this.RegisterEvent<OnTermChange>(OnTermChange);
        
        // Initialize with current season
        _currentSeason = _termModel.GetSeason();
        UpdateTilesForSeason(_currentSeason);
    }

    private void OnTermChange(OnTermChange e)
    {
        var newSeason = _termModel.GetSeason();
        if (newSeason != _currentSeason)
        {
            _currentSeason = newSeason;
            UpdateTilesForSeason(newSeason);
        }
    }

    private void UpdateTilesForSeason(Season season)
    {
        if (targetTilemap == null)
        {
            Debug.LogError("[SeasonTilemapController] Target Tilemap is NOT assigned!");
            return;
        }

        Tile[] selectedTiles = null;
        string debugSeasonName = season.ToString();

        switch (season)
        {
            case Season.Spring:
                selectedTiles = SpringTiles;
                break;
            case Season.Summer:
                selectedTiles = SummerTiles;
                break;
            case Season.Autumn:
                selectedTiles = AutumnTiles;
                break;
            case Season.Winter:
                selectedTiles = WinterTiles;
                break;
        }

        Debug.Log($"[SeasonTilemapController] Updating for Season: {debugSeasonName}. TermID: {_termModel.CurrentTermId.Value}");

        if (selectedTiles != null && selectedTiles.Length == 16)
        {
            targetTilemap.SetTiles(selectedTiles);
            Debug.Log($"[SeasonTilemapController] Successfully called SetTiles for {debugSeasonName}. Tiles[0]: {selectedTiles[0]?.name}");
        }
        else
        {
            int len = selectedTiles == null ? 0 : selectedTiles.Length;
            Debug.LogWarning($"[SeasonTilemapController] Setup missing or invalid for season: {debugSeasonName}. Tiles array length: {len}. Ensure 16 tiles are assigned in Inspector.");
        }
    }

    private void OnDestroy()
    {
        this.UnRegisterEvent<OnTermChange>(OnTermChange);
    }
}
