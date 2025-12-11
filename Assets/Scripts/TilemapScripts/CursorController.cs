using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

using QFramework;

public partial class CursorController : MonoBehaviour {
    public DualGridTilemap dualGridTilemap;
    private ICursorModel _cursorModel;

    void Start()
    {
        _cursorModel = ShennongAlmanac.Interface.GetModel<ICursorModel>();
    }

    void Update() {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Check bounds
        if (_cursorModel != null)
        {
            Vector3 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (Mathf.Abs(viewportPos.x - 0.5f) > _cursorModel.ViewRatioX / 2f ||
                Mathf.Abs(viewportPos.y - 0.5f) > _cursorModel.ViewRatioY / 2f)
            {
                return;
            }
        }

        Vector3Int tilePos = GetWorldPosTile(mouseWorldPos);
        transform.position = tilePos + new Vector3(0.5f, 0.5f, -1);

        if (Input.GetMouseButton(0)) {
            dualGridTilemap.SetCell(tilePos, dualGridTilemap.dirtPlaceholderTile);
        } else if (Input.GetMouseButton(1)) {
            dualGridTilemap.SetCell(tilePos, dualGridTilemap.grassPlaceholderTile);
        }
    }

    public static Vector3Int GetWorldPosTile(Vector3 worldPos) {
        int xInt = Mathf.FloorToInt(worldPos.x);
        int yInt = Mathf.FloorToInt(worldPos.y);
        return new(xInt, yInt, 0);
    }
}
