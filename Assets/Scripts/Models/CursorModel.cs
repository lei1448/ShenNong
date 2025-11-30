using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public interface ICursorModel : IModel
{
    public Vector3Int CursorPos { get; set; }
}

public class CursorModel : AbstractModel, ICursorModel
{
    public Vector3Int CursorPos { get; set; }

    protected override void OnInit()
    {
        
    }
}