using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // Check bounds
        Vector3 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (Mathf.Abs(viewportPos.x - 0.5f) > cursorModel.ViewRatioX / 2f ||
            Mathf.Abs(viewportPos.y - 0.5f) > cursorModel.ViewRatioY / 2f)
        {
            return;
        }

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
