using System.Collections;
using System.Collections.Generic;
using QFramework;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetTilemapViewController : MonoBehaviour,IController
{
    public DualGridTilemap dualGridTilemap;
    private ICursorModel cursorModel;
    private Vector3Int mousePos;
    public IArchitecture GetArchitecture()
    {
        return ShennongAlmanac.Interface;
    }

/// <summary>
/// Start is called on the frame when a script is enabled just before
/// any of the Update methods is called the first time.
/// </summary>
    void Start()
    {
        cursorModel = this.GetModel<ICursorModel>();
    }
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            mousePos = cursorModel.CursorPos;
            dualGridTilemap.SetCell(mousePos,dualGridTilemap.dirtPlaceholderTile);
        }
        else if(Input.GetMouseButton(1))
        {
            mousePos = cursorModel.CursorPos;
            dualGridTilemap.SetCell(mousePos,dualGridTilemap.grassPlaceholderTile);
        }
    }
}
