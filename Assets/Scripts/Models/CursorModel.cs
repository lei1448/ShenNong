using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public interface ICursorModel : IModel
{
    public Vector3Int CursorPos { get; set; }
    public float ViewRatioX { get; set; }
    public float ViewRatioY { get; set; }
}

public class CursorModel : AbstractModel, ICursorModel
{
    public Vector3Int CursorPos { get; set; }
    public float ViewRatioX { get; set; }
    public float ViewRatioY { get; set; }

    protected override void OnInit()
    {
        ViewRatioX = 0.8f;
        ViewRatioY = 0.8f;
    }
}