using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileType;
using System.Collections;

public class DualGridTilemap : MonoBehaviour {
    protected static Vector3Int[] NEIGHBOURS = new Vector3Int[] {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0)
    };

    // Dictionary to store rules. Now instance-based to allow swapping/updates without static issues
    protected Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> _currentRules;

    // References
    public Tilemap placeholderTilemap;
    public Tilemap displayTilemap;
    private Tilemap _displayTilemapNext; // The secondary buffer

    public Tile grassPlaceholderTile;
    public Tile dirtPlaceholderTile;
    public Tile[] tiles;

    private Coroutine _crossfadeCoroutine;
    private const float FADE_DURATION = 1.0f;
    
    // Map Dimensions
    public int MapWidth = 100;
    public int MapHeight = 50; // -25 to 25
    
    // Origin is centered (e.g. -Width/2 to Width/2)

    void Awake() {
        _currentRules = GenerateRules(tiles);
    }

    void Start() {
        GeneratePlaceholderMap();
        EnsureSecondaryTilemap();
        RefreshDisplayTilemap(displayTilemap, _currentRules);
        
        SetTilemapAlpha(displayTilemap, 1f);
        SetTilemapAlpha(_displayTilemapNext, 0f);
    }
    
    private void GeneratePlaceholderMap()
    {
        if (placeholderTilemap == null) return;
        
        placeholderTilemap.ClearAllTiles();
        
        int startX = -MapWidth / 2;
        int endX = MapWidth / 2;
        int startY = -MapHeight / 2;
        int endY = MapHeight / 2;
        
        // Fill with Grass
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                placeholderTilemap.SetTile(new Vector3Int(x, y, 0), grassPlaceholderTile);
            }
        }
    }
    
    // Create or find the secondary tilemap for double buffering
    private void EnsureSecondaryTilemap()
    {
        if (_displayTilemapNext != null) return;
        
        // Look for existing
        var child = transform.Find("DisplayTilemap_Next");
        if (child != null)
        {
            _displayTilemapNext = child.GetComponent<Tilemap>();
            return;
        }

        // Create new by cloning
        if (displayTilemap != null)
        {
            var go = Instantiate(displayTilemap.gameObject, displayTilemap.transform.parent);
            go.name = "DisplayTilemap_Next";
            _displayTilemapNext = go.GetComponent<Tilemap>();
            
            // Clear it
            _displayTilemapNext.ClearAllTiles();
            
            // Adjust sorting order? Usually same order + z-fighting might occur if not handled.
            // But since we fade alpha, z-fighting is less of an issue if sorted same.
            // Better to have Next slightly behind or in front? similar to atmosphere.
            // Let's keep same Order in Layer.
            var rend = _displayTilemapNext.GetComponent<TilemapRenderer>();
            var prevRend = displayTilemap.GetComponent<TilemapRenderer>();
            if (rend != null && prevRend != null)
            {
                rend.sortingOrder = prevRend.sortingOrder; // Same layer
            }
        }
    }

    // Helper to generate rules from a specific tileset
    private Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> GenerateRules(Tile[] tileSet)
    {
        if (tileSet == null || tileSet.Length != 16) return new Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile>();
        
        return new Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> {
            {new (Grass, Grass, Grass, Grass), tileSet[6]},
            {new (Dirt, Dirt, Dirt, Grass), tileSet[13]},
            {new (Dirt, Dirt, Grass, Dirt), tileSet[0]},
            {new (Dirt, Grass, Dirt, Dirt), tileSet[8]},
            {new (Grass, Dirt, Dirt, Dirt), tileSet[15]},
            {new (Dirt, Grass, Dirt, Grass), tileSet[1]},
            {new (Grass, Dirt, Grass, Dirt), tileSet[11]},
            {new (Dirt, Dirt, Grass, Grass), tileSet[3]},
            {new (Grass, Grass, Dirt, Dirt), tileSet[9]},
            {new (Dirt, Grass, Grass, Grass), tileSet[5]},
            {new (Grass, Dirt, Grass, Grass), tileSet[2]},
            {new (Grass, Grass, Dirt, Grass), tileSet[10]},
            {new (Grass, Grass, Grass, Dirt), tileSet[7]},
            {new (Dirt, Grass, Grass, Dirt), tileSet[14]},
            {new (Grass, Dirt, Dirt, Grass), tileSet[4]},
            {new (Dirt, Dirt, Dirt, Dirt), tileSet[12]},
        };
    }

    public void SetTiles(Tile[] newTiles)
    {
        if (newTiles == null || newTiles.Length != 16)
        {
            Debug.LogError("DualGridTilemap: Invalid tiles array.");
            return;
        }

        if (_crossfadeCoroutine != null) StopCoroutine(_crossfadeCoroutine);
        _crossfadeCoroutine = StartCoroutine(CrossfadeRoutine(newTiles));
    }

    private IEnumerator CrossfadeRoutine(Tile[] newTiles)
    {
        Debug.Log("[DualGridTilemap] Starting Crossfade...");
        EnsureSecondaryTilemap();

        // 1. Generate new rules
        var nextRules = GenerateRules(newTiles);
        
        // 2. Prepare Next Tilemap (Hidden)
        SetTilemapAlpha(_displayTilemapNext, 0f);
        _displayTilemapNext.ClearAllTiles();
        RefreshDisplayTilemap(_displayTilemapNext, nextRules);
        
        // Ensure Next renders ON TOP of Current
        var currentRend = displayTilemap.GetComponent<TilemapRenderer>();
        var nextRend = _displayTilemapNext.GetComponent<TilemapRenderer>();
        if (currentRend != null && nextRend != null)
        {
            nextRend.sortingOrder = currentRend.sortingOrder + 1;
        }
        
        // Ensure Current is fully visible
        SetTilemapAlpha(displayTilemap, 1f);
        
        // 3. Fade
        float timer = 0f;
        while (timer < FADE_DURATION)
        {
            timer += Time.deltaTime;
            float t = timer / FADE_DURATION;
            
            // Fade NEXT in (0 -> 1)
            SetTilemapAlpha(_displayTilemapNext, t);
            
            // Keep CURRENT at 1
            SetTilemapAlpha(displayTilemap, 1f);
            
            yield return null;
        }
        
        // 4. Finalize
        SetTilemapAlpha(displayTilemap, 0f); // Now hide old
        SetTilemapAlpha(_displayTilemapNext, 1f); // Show new
        
        // 5. Swap logic
        // We want 'displayTilemap' to always refer to the visible active one
        // so that existing 'SetCell' calls continue to work on the correct map.
        var tempMap = displayTilemap;
        displayTilemap = _displayTilemapNext;
        _displayTilemapNext = tempMap;
        
        // Update current rules and tiles reference so SetCell works correctly
        this.tiles = newTiles;
        _currentRules = nextRules;
        
        Debug.Log("[DualGridTilemap] Crossfade Complete.");
    }

    private void SetTilemapAlpha(Tilemap map, float alpha)
    {
        if (map == null) return;
        var c = map.color;
        c.a = alpha;
        map.color = c;
    }

    public void SetCell(Vector3Int coords, Tile tile) {
        placeholderTilemap.SetTile(coords, tile);
        // Only update the currently active display map
        setDisplayTile(coords, displayTilemap, _currentRules);
    }

    private TileType getPlaceholderTileTypeAt(Vector3Int coords) {
        if (placeholderTilemap.GetTile(coords) == grassPlaceholderTile)
            return Grass;
        else if (placeholderTilemap.GetTile(coords) == dirtPlaceholderTile)
            return Dirt;
        else
            return Grass;
    }

    // Updated to be stateless/pure if possible
    protected Tile calculateDisplayTile(Vector3Int coords, Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> rules) {
        TileType topRight = getPlaceholderTileTypeAt(coords - NEIGHBOURS[0]);
        TileType topLeft = getPlaceholderTileTypeAt(coords - NEIGHBOURS[1]);
        TileType botRight = getPlaceholderTileTypeAt(coords - NEIGHBOURS[2]);
        TileType botLeft = getPlaceholderTileTypeAt(coords - NEIGHBOURS[3]);

        Tuple<TileType, TileType, TileType, TileType> neighbourTuple = new(topLeft, topRight, botLeft, botRight);

        if (rules.TryGetValue(neighbourTuple, out Tile tile))
        {
            return tile;
        }
        return null; // Should not happen with complete rules
    }

    protected void setDisplayTile(Vector3Int pos, Tilemap targetMap, Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> rules) {
         if (targetMap == null || rules == null) return;
         
        for (int i = 0; i < NEIGHBOURS.Length; i++) {
            Vector3Int newPos = pos + NEIGHBOURS[i];
            targetMap.SetTile(newPos, calculateDisplayTile(newPos, rules));
        }
    }

    public void RefreshDisplayTilemap() {
        // Overload for default/legacy usage
        RefreshDisplayTilemap(displayTilemap, _currentRules);
    }

    public void RefreshDisplayTilemap(Tilemap targetMap, Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> rules) {
        if (targetMap == null || rules == null) return;

        // Iterate a reasonable range or bounds. 
        // For efficiency, maybe get bounds of placeholder?
        // Original code used hardcoded -50 to 50
        for (int i = -50; i < 50; i++) {
            for (int j = -50; j < 50; j++) {
                // Use invalid pos to trigger updates without modifying logic much?
                // setDisplayTile uses POS to update neighbors.
                // Just calculating strictly by cell might be cleaner for full refresh.
                // But setDisplayTile logic affects 4 cells per pos.
                // Let's stick to the grid scan.
                
                // Directly set the tile for this position? 
                // Wait, calculateDisplayTile(newPos) takes a coord and converts it using rules.
                
                targetMap.SetTile(new Vector3Int(i,j,0), calculateDisplayTile(new Vector3Int(i,j,0), rules));
            }
        }
    }
}

public enum TileType {
    None,
    Grass,
    Dirt
}
