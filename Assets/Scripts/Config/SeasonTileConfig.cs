using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "SeasonTileConfig", menuName = "Shennong/SeasonTileConfig")]
public class SeasonTileConfig : ScriptableObject
{
    [Header("Spring")]
    public Tile[] SpringTiles; // 16 tiles

    [Header("Summer")]
    public Tile[] SummerTiles;

    [Header("Autumn")]
    public Tile[] AutumnTiles;

    [Header("Winter")]
    public Tile[] WinterTiles;

    public Tile[] GetTilesForSeason(Season season)
    {
        switch (season)
        {
            case Season.Spring: return SpringTiles;
            case Season.Summer: return SummerTiles;
            case Season.Autumn: return AutumnTiles;
            case Season.Winter: return WinterTiles;
            default: return SpringTiles;
        }
    }
}
