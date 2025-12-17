using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CropParameterRange
{
    public int Min;
    public int Max;
}

[System.Serializable]
public struct CropStageData
{
    public string StageName;
    public int DurationDays; // Days required to complete this stage
    public Sprite StageIcon;
    
    [Header("Environment Requirements")]
    public CropParameterRange TempRange;
    public CropParameterRange LightRange;
    public CropParameterRange MoistureRange;
    public CropParameterRange FertilityRange;
}

[CreateAssetMenu(fileName = "NewCrop", menuName = "Shennong/CropConfig")]
public class CropConfig : ScriptableObject
{
    public string CropName;
    public int CropId; // Matches ID in table (e.g. 101)
    
    public Sprite Icon; // Seed/UI Icon
    public int MaxHP = 20; // Default max HP
    
    [Header("Growth Stages")]
    public List<CropStageData> Stages = new List<CropStageData>();
    
    [Header("Planting Restrictions")]
    public List<string> SuitableSolarTerms; // List of term names allowed for planting

    [Header("Knowledge Unlock")]
    public string KnowledgeDescription;
    public Sprite KnowledgeIcon; // Icon for the encyclopedia/unlocks
    
    // Helper to calculate total days
    public int TotalGrowthDays
    {
        get
        {
            if (Stages == null) return 0;
            int sum = 0;
            foreach (var s in Stages) sum += s.DurationDays;
            return sum;
        }
    }

    public Sprite GetSpriteByProgress(int currentGrowthDays)
    {
        if (Stages == null || Stages.Count == 0) return null;
        
        int daysAccumulator = 0;
        
        // Find which stage we are in
        for (int i = 0; i < Stages.Count; i++)
        {
            daysAccumulator += Stages[i].DurationDays;
            // If strictly less, we are in this stage
            // Or if it's the last stage, stay there
            if (currentGrowthDays < daysAccumulator)
            {
                return Stages[i].StageIcon;
            }
        }
        
        // If exceeded total days, return the last stage (Mature)
        return Stages[Stages.Count - 1].StageIcon;
    }
    
    public CropStageData GetCurrentStageData(int currentGrowthDays)
    {
        if (Stages == null || Stages.Count == 0) return default;
        
        int daysAccumulator = 0;
        for (int i = 0; i < Stages.Count; i++)
        {
            daysAccumulator += Stages[i].DurationDays;
            if (currentGrowthDays < daysAccumulator)
            {
                return Stages[i];
            }
        }
        return Stages[Stages.Count - 1]; // Return last stage if mature/overgrown
    }
}