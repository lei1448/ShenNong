using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileType;

public class DualGridTilemap : MonoBehaviour {
    protected static Vector3Int[] NEIGHBOURS = new Vector3Int[] {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0)
    };

    protected static Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> neighbourTupleToTile;

    // Provide references to each tilemap in the inspector
    public Tilemap placeholderTilemap;
    public Tilemap displayTilemap;

    // Provide the dirt and grass placeholder tiles in the inspector
    public Tile grassPlaceholderTile;
    public Tile dirtPlaceholderTile;

    // Provide the 16 tiles in the inspector
    public Tile[] tiles;

    void Awake() {
        InitializeDictionary();
    }

    void Start() {
        RefreshDisplayTilemap();
    }

    private void InitializeDictionary() {
        if (neighbourTupleToTile != null) return;

        // This dictionary stores the "rules", each 4-neighbour configuration corresponds to a tile
        // |_1_|_2_|
        // |_3_|_4_|
        neighbourTupleToTile = new() {
            {new (Grass, Grass, Grass, Grass), tiles[6]},
            {new (Dirt, Dirt, Dirt, Grass), tiles[13]}, // OUTER_BOTTOM_RIGHT
            {new (Dirt, Dirt, Grass, Dirt), tiles[0]}, // OUTER_BOTTOM_LEFT
            {new (Dirt, Grass, Dirt, Dirt), tiles[8]}, // OUTER_TOP_RIGHT
            {new (Grass, Dirt, Dirt, Dirt), tiles[15]}, // OUTER_TOP_LEFT
            {new (Dirt, Grass, Dirt, Grass), tiles[1]}, // EDGE_RIGHT
            {new (Grass, Dirt, Grass, Dirt), tiles[11]}, // EDGE_LEFT
            {new (Dirt, Dirt, Grass, Grass), tiles[3]}, // EDGE_BOTTOM
            {new (Grass, Grass, Dirt, Dirt), tiles[9]}, // EDGE_TOP
            {new (Dirt, Grass, Grass, Grass), tiles[5]}, // INNER_BOTTOM_RIGHT
            {new (Grass, Dirt, Grass, Grass), tiles[2]}, // INNER_BOTTOM_LEFT
            {new (Grass, Grass, Dirt, Grass), tiles[10]}, // INNER_TOP_RIGHT
            {new (Grass, Grass, Grass, Dirt), tiles[7]}, // INNER_TOP_LEFT
            {new (Dirt, Grass, Grass, Dirt), tiles[14]}, // DUAL_UP_RIGHT
            {new (Grass, Dirt, Dirt, Grass), tiles[4]}, // DUAL_DOWN_RIGHT
            {new (Dirt, Dirt, Dirt, Dirt), tiles[12]},
        };
    }

    public void SetTiles(Tile[] newTiles)
    {
        if (newTiles == null || newTiles.Length != 16)
        {
            Debug.LogError("DualGridTilemap: Invalid tiles array. Must be 16 tiles.");
            return;
        }
        
        this.tiles = newTiles;
        
        InitializeDictionary();

        // Re-populate dictionary with new tiles
        neighbourTupleToTile[new (Grass, Grass, Grass, Grass)] = tiles[6];
        neighbourTupleToTile[new (Dirt, Dirt, Dirt, Grass)] = tiles[13];
        neighbourTupleToTile[new (Dirt, Dirt, Grass, Dirt)] = tiles[0];
        neighbourTupleToTile[new (Dirt, Grass, Dirt, Dirt)] = tiles[8];
        neighbourTupleToTile[new (Grass, Dirt, Dirt, Dirt)] = tiles[15];
        neighbourTupleToTile[new (Dirt, Grass, Dirt, Grass)] = tiles[1];
        neighbourTupleToTile[new (Grass, Dirt, Grass, Dirt)] = tiles[11];
        neighbourTupleToTile[new (Dirt, Dirt, Grass, Grass)] = tiles[3];
        neighbourTupleToTile[new (Grass, Grass, Dirt, Dirt)] = tiles[9];
        neighbourTupleToTile[new (Dirt, Grass, Grass, Grass)] = tiles[5];
        neighbourTupleToTile[new (Grass, Dirt, Grass, Grass)] = tiles[2];
        neighbourTupleToTile[new (Grass, Grass, Dirt, Grass)] = tiles[10];
        neighbourTupleToTile[new (Grass, Grass, Grass, Dirt)] = tiles[7];
        neighbourTupleToTile[new (Dirt, Grass, Grass, Dirt)] = tiles[14];
        neighbourTupleToTile[new (Grass, Dirt, Dirt, Grass)] = tiles[4];
        neighbourTupleToTile[new (Dirt, Dirt, Dirt, Dirt)] = tiles[12];

        RefreshDisplayTilemap();
    }

    public void SetCell(Vector3Int coords, Tile tile) {
        placeholderTilemap.SetTile(coords, tile);
        setDisplayTile(coords);
    }

    private TileType getPlaceholderTileTypeAt(Vector3Int coords) {
        if (placeholderTilemap.GetTile(coords) == grassPlaceholderTile)
            return Grass;
        else if (placeholderTilemap.GetTile(coords) == dirtPlaceholderTile)
            return Dirt;
        else
            return Grass;
    }

    protected Tile calculateDisplayTile(Vector3Int coords) {
        // 4 neighbours
        TileType topRight = getPlaceholderTileTypeAt(coords - NEIGHBOURS[0]);
        TileType topLeft = getPlaceholderTileTypeAt(coords - NEIGHBOURS[1]);
        TileType botRight = getPlaceholderTileTypeAt(coords - NEIGHBOURS[2]);
        TileType botLeft = getPlaceholderTileTypeAt(coords - NEIGHBOURS[3]);

        Tuple<TileType, TileType, TileType, TileType> neighbourTuple = new(topLeft, topRight, botLeft, botRight);

        return neighbourTupleToTile[neighbourTuple];
    }

    protected void setDisplayTile(Vector3Int pos) {
        for (int i = 0; i < NEIGHBOURS.Length; i++) {
            Vector3Int newPos = pos + NEIGHBOURS[i];
            displayTilemap.SetTile(newPos, calculateDisplayTile(newPos));
        }
    }

    // The tiles on the display tilemap will recalculate themselves based on the placeholder tilemap
    public void RefreshDisplayTilemap() {
        for (int i = -50; i < 50; i++) {
            for (int j = -50; j < 50; j++) {
                setDisplayTile(new Vector3Int(i, j, 0));
            }
        }
    }
}

public enum TileType {
    None,
    Grass,
    Dirt
}
