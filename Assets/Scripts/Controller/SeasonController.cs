using QFramework;
using UnityEngine;

public class SeasonController : MonoBehaviour, IController
{
    public DualGridTilemap dualGridTilemap;
    public SeasonTileConfig seasonTileConfig;

    private ITermModel _termModel;
    private Season _currentSeason;

    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    void Start()
    {
        _termModel = this.GetModel<ITermModel>();
        
        // Initial update
        _currentSeason = _termModel.GetSeason();
        UpdateSeasonTiles(_currentSeason);

        // Listen for term changes to detect season change
        _termModel.CurrentTermId.Register(termId =>
        {
            Season newSeason = _termModel.GetSeason();
            if (newSeason != _currentSeason)
            {
                _currentSeason = newSeason;
                UpdateSeasonTiles(newSeason);
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void UpdateSeasonTiles(Season season)
    {
        if (dualGridTilemap != null && seasonTileConfig != null)
        {
            var tiles = seasonTileConfig.GetTilesForSeason(season);
            dualGridTilemap.SetTiles(tiles);
            Debug.Log($"[Season] Changed to {season}");
        }
    }
}
