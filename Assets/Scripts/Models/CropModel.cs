// Models/CropModel.cs
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class CropData
{
    public string CropId; // 对应 Config 的名字
    public int PlantedDay; // 种植时的累计总天数
    public int GrowthDays; // 已经生长了几天
    public bool IsMature;  // 是否成熟
    public CropConfig Config; // 引用配置
}

public interface ICropModel : IModel
{
    Dictionary<Vector3Int, CropData> CropMap { get; }
    void AddCrop(Vector3Int pos, CropConfig config);
    BindableProperty<CropConfig> SelectedSeedConfig { get; }
    CropData GetCrop(Vector3Int pos);
    void RemoveCrop(Vector3Int pos);
}

public class CropModel : AbstractModel, ICropModel
{
    public Dictionary<Vector3Int, CropData> CropMap { get; } = new();
    public BindableProperty<CropConfig> SelectedSeedConfig { get; } = new();

    protected override void OnInit() { }

    public void AddCrop(Vector3Int pos, CropConfig config)
    {
        if (CropMap.ContainsKey(pos)) return;

        CropMap.Add(pos, new CropData
        {
            CropId = config.CropName,
            PlantedDay = 0, 
            GrowthDays = 0,
            IsMature = false,
            Config = config
        });
    }

    public CropData GetCrop(Vector3Int pos)
    {
        return CropMap.TryGetValue(pos, out var data) ? data : null;
    }

    public void RemoveCrop(Vector3Int pos)
    {
        if (CropMap.ContainsKey(pos)) CropMap.Remove(pos);
    }
}