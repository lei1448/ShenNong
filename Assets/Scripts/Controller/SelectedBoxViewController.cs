using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class SelectedBoxViewController : MonoBehaviour, IController
{
    [SerializeField] private GameObject selectedBox;
    private ICursorModel cursorModel;
    public IArchitecture GetArchitecture()
    {
        return ShennongAlmanac.Interface;
    }

    void Start()
    {
        cursorModel = this.GetModel<ICursorModel>();
    }
    [Header("Cursor Settings")]
    [Range(0f, 1f)] public float viewRatioX = 0.8f;
    [Range(0f, 1f)] public float viewRatioY = 0.8f;

    void Update()
    {
        // Sync inspector values to model
        cursorModel.ViewRatioX = viewRatioX;
        cursorModel.ViewRatioY = viewRatioY;

        // 1. Get Viewport Position
        Vector3 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        // 2. Clamp Viewport Position
        float clampedViewportX = Mathf.Clamp(viewportPos.x, 0.5f - viewRatioX / 2f, 0.5f + viewRatioX / 2f);
        float clampedViewportY = Mathf.Clamp(viewportPos.y, 0.5f - viewRatioY / 2f, 0.5f + viewRatioY / 2f);
        Vector3 clampedViewportPos = new Vector3(clampedViewportX, clampedViewportY, viewportPos.z);

        // 3. Raycast to Ground (Z=0)
        Ray ray = Camera.main.ViewportPointToRay(clampedViewportPos);
        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);
        
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            
            // 4. Convert to Tile Position
            int tileX = Mathf.FloorToInt(worldPos.x);
            int tileY = Mathf.FloorToInt(worldPos.y);
            
            cursorModel.CursorPos = new Vector3Int(tileX, tileY, 0);
            selectedBox.transform.position = cursorModel.CursorPos + new Vector3(0.5f, 0.5f, 0);
        }
    }
}
