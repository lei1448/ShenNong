using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CropParameterRange
{
    public int Min;
    public int Max;
}

[CreateAssetMenu(fileName = "NewCrop", menuName = "Shennong/CropConfig")]
public class CropConfig : ScriptableObject
{
    public string CropName;
    public Sprite Icon; // [新增] UI 显示用的图标
    public int DaysToMature; 
    public Sprite[] GrowthStages; 
    public int MaxHP = 100;

    [Header("Environment Requirements")]
    public CropParameterRange TempRange;
    public CropParameterRange LightRange;
    public CropParameterRange MoistureRange;
    public CropParameterRange FertilityRange;

    [Header("Planting Restrictions")]
    public List<string> SuitableSolarTerms; // List of term names allowed for planting

    [Header("Knowledge Unlock")]
    public string KnowledgeDescription;
    public Sprite KnowledgeIcon;

    public Sprite GetSpriteByProgress(int growthDays)
    {
        if (GrowthStages == null || GrowthStages.Length == 0) return null;
        float progress = (float)growthDays / DaysToMature;
        int index = Mathf.FloorToInt(progress * (GrowthStages.Length - 1));
        index = Mathf.Clamp(index, 0, GrowthStages.Length - 1);
        if (growthDays >= DaysToMature) index = GrowthStages.Length - 1;
        return GrowthStages[index];
    }
}