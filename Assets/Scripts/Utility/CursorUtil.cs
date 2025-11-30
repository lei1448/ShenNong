using UnityEngine;

public static class CursorUtil
{
    // 创建一个代表世界坐标Z=0的平面
    private static readonly Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);

    /// <summary>
    /// 获取鼠标指向世界坐标Z=0平面的精确位置
    /// </summary>
    /// <returns>世界坐标</returns>
    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
    
    /// <summary>
    /// 获取鼠标指向的世界坐标对应的格子坐标 (Tile Position)
    /// </summary>
    /// <returns>格子坐标</returns>
    public static Vector3Int GetWorldPosTile()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        int xInt = Mathf.FloorToInt(mouseWorldPos.x);
        int yInt = Mathf.FloorToInt(mouseWorldPos.y);
        
        return new Vector3Int(xInt, yInt, 0);
    }
}