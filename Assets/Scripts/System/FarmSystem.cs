using System.Collections.Generic;
using QFramework;
using UnityEngine;


public class FarmSystem : AbstractSystem
{
    private ICropModel _cropModel;
    private ITermModel _termModel;

    protected override void OnInit()
    {
        _cropModel = this.GetModel<ICropModel>();
        _termModel = this.GetModel<ITermModel>();
        
        this.RegisterEvent<OnDayPass>(OnDayPass);
        this.RegisterEvent<OnTermChange>(OnTermChange);
        this.RegisterEvent<OnTermSettlement>(OnTermSettlement);
    }

    private void OnDayPass(OnDayPass e)
    {
        // Day pass mainly impacts growth
        AdvanceCropGrowth(1);
    }

    private void OnTermSettlement(OnTermSettlement e)
    {
        // 1. Calculate how many days are remaining in this solar term that we are skipping
        // CurrentDayInTerm is 1-indexed. If it's day 1, and we jump, we simulate passing the remaining 14 days (Total 15).
        // If DaysPerTerm is 15.
        // Days to grow = 15 - CurrentDayInTerm + 1? No.
        // If I am on Day 1 (Morning), and I skip to Next Term, I am effectively finishing Day 1 to Day 15.
        // So I grow for (DaysPerTerm - CurrentInTerm + 1) days?
        // Let's assume standard logic: Remaining days = DaysPerTerm - CurrentDayInTerm + 1. 
        // e.g. Day 1: 15 - 1 + 1 = 15 days.
        // e.g. Day 15: 15 - 15 + 1 = 1 day.
        // Actually, AdvanceTerm resets CurrentDay to 1 for NEXT term.
        // So we are simulating the PASSAGE of the REST of the current term.
        // Let's use simple math: DaysToSimulate = _termModel.DaysPerTerm - _termModel.CurrentDayInTerm.Value + 1;
        
        int daysToSimulate = _termModel.DaysPerTerm - _termModel.CurrentDayInTerm.Value + 1;
        // Safety check
        if (daysToSimulate < 0) daysToSimulate = 0;
        
        Debug.Log($"[FarmSystem] Simulating {daysToSimulate} days growth for Term Settlement.");
        AdvanceCropGrowth(daysToSimulate);

        // End of Turn: Calculate Blood / HP Damage based on deviation
        List<Vector3Int> positions = new List<Vector3Int>(_cropModel.CropMap.Keys);
        
        foreach (var pos in positions)
        {
            var crop = _cropModel.CropMap[pos];
            if (crop.Config == null) continue;

            int damage = CalculateDamage(crop);
            if (damage > 0)
            {
                // Damage is applied once per term skip? Or per day skipped?
                // Visual Novels usually simplify this to "Turn-based damage".
                // Let's keep it as is (Once per Term Settlement).
                
                crop.CurrentHP -= damage;
                if (crop.CurrentHP < 0) crop.CurrentHP = 0;
                
                Debug.Log($"[FarmSystem] Crop at {pos} took {damage} damage. Current HP: {crop.CurrentHP}");
            }
            
            this.SendEvent(new OnCropUpdated { Position = pos });
        }
    }

    private void AdvanceCropGrowth(int days)
    {
        List<Vector3Int> positions = new List<Vector3Int>(_cropModel.CropMap.Keys);

        foreach (var pos in positions)
        {
            var crop = _cropModel.CropMap[pos];
            if (!crop.IsMature)
            {
                crop.GrowthDays += days;
                if (crop.GrowthDays >= crop.Config.DaysToMature)
                {
                    crop.IsMature = true;
                }
                this.SendEvent(new OnCropUpdated { Position = pos });
            }
        }
    }

    private void OnTermChange(OnTermChange e)
    {
        // Start of Turn: Reset Environment Params
        ResetCropEnvironment();
    }

    private void ResetCropEnvironment()
    {
        // Set all crops' state to current Term parameters
        int termTemp = _termModel.CurrentTemp;
        int termLight = _termModel.CurrentLightValue;
        int termMoisture = _termModel.CurrentMoistureValue;
        int termFertility = _termModel.CurrentFertilityValue;

        foreach (var kvp in _cropModel.CropMap)
        {
            var crop = kvp.Value;
            crop.CurrentState.Temp = termTemp;
            crop.CurrentState.Light = termLight;
            crop.CurrentState.Moisture = termMoisture;
            crop.CurrentState.Fertility = termFertility;
            
            // Check suitability immediately to update icons
            CheckSuitability(crop);
            
            this.SendEvent(new OnCropUpdated { Position = kvp.Key });
        }
        
        Debug.Log($"[FarmSystem] Environment Reset for Term: {_termModel.CurrentTermName}");
    }

    private int CalculateDamage(CropData crop)
    {
        int damage = 0;
        damage += GetDeviation(crop.CurrentState.Temp, crop.Config.TempRange);
        damage += GetDeviation(crop.CurrentState.Light, crop.Config.LightRange);
        damage += GetDeviation(crop.CurrentState.Moisture, crop.Config.MoistureRange);
        damage += GetDeviation(crop.CurrentState.Fertility, crop.Config.FertilityRange);
        return damage;
    }

    private int GetDeviation(int value, CropParameterRange range)
    {
        if (value < range.Min) return range.Min - value;
        if (value > range.Max) return value - range.Max;
        return 0;
    }

    private void CheckSuitability(CropData crop)
    {
        // Logic for checking which icon to show could be in View, 
        // but System can mark the state if needed.
        // For now, View can calculate this based on value vs Config range.
    }

    public void ModifyCropState(Vector3Int pos, string paramType, int value)
    {
        var crop = _cropModel.GetCrop(pos);
        if (crop == null) return;

        switch (paramType)
        {
            case "Moisture":
                crop.CurrentState.Moisture += value;
                break;
            case "Fertility":
                crop.CurrentState.Fertility += value;
                break;
            case "Light":
                crop.CurrentState.Light += value;
                break;
            case "Temp":
                crop.CurrentState.Temp += value;
                break;
        }

        // Clamp values? Maybe. For now allow overflow.
        this.SendEvent(new OnCropUpdated { Position = pos });
    }

    public void PlantCrop(Vector3Int pos, CropConfig config)
    {
        if (config == null) return;
        
        // Check Term Restrictions
        string currentTerm = _termModel.CurrentTermName;
        if (config.SuitableSolarTerms != null && config.SuitableSolarTerms.Count > 0)
        {
            if (!config.SuitableSolarTerms.Contains(currentTerm))
            {
                Debug.Log($"[FarmSystem] Cannot plant {config.CropName} in {currentTerm}.");
                return;
            }
        }

        if (_cropModel.GetCrop(pos) == null)
        {
            _cropModel.AddCrop(pos, config);
            
            // Initialize with current environment
            var crop = _cropModel.GetCrop(pos);
            crop.CurrentState.Temp = _termModel.CurrentTemp;
            crop.CurrentState.Light = _termModel.CurrentLightValue;
            crop.CurrentState.Moisture = _termModel.CurrentMoistureValue;
            crop.CurrentState.Fertility = _termModel.CurrentFertilityValue;
            
            this.SendEvent(new OnCropUpdated { Position = pos });
        }
    }

    public void HarvestCrop(Vector3Int pos)
    {
        var crop = _cropModel.GetCrop(pos);
        if (crop == null) return;

        if (crop.IsMature)
        {
            var knowledgeModel = this.GetModel<IKnowledgeModel>();
            if (!string.IsNullOrEmpty(crop.Config.KnowledgeDescription))
            {
                knowledgeModel.Unlock(crop.Config.CropName);
            }
            
            // Score Calculation
            int score = crop.CurrentHP;
            _cropModel.TotalScore.Value += score;
            Debug.Log($"[FarmSystem] Harvested {crop.CropId}. Score: {score}. Total: {_cropModel.TotalScore.Value}");

            _cropModel.RemoveCrop(pos);
            this.SendEvent(new OnCropUpdated { Position = pos }); 
        }
    }
    public void ApplyTool(Vector3Int pos, ToolType tool)
    {
        var crop = _cropModel.GetCrop(pos);
        
        switch (tool)
        {
            case ToolType.WateringCan:
                if (crop != null)
                {
                    crop.CurrentState.Moisture += 20;
                    Debug.Log($"[FarmSystem] Watered crop at {pos}. Moisture: {crop.CurrentState.Moisture}");
                    this.SendEvent(new OnCropUpdated { Position = pos });
                }
                break;
                
            case ToolType.Fertilizer:
                if (crop != null)
                {
                    crop.CurrentState.Fertility += 20;
                    Debug.Log($"[FarmSystem] Fertilized crop at {pos}. Fertility: {crop.CurrentState.Fertility}");
                    this.SendEvent(new OnCropUpdated { Position = pos });
                }
                break;
                
            case ToolType.Hoe:
                if (crop != null)
                {
                    _cropModel.RemoveCrop(pos);
                    Debug.Log($"[FarmSystem] Hoed/Removed crop at {pos}.");
                    this.SendEvent(new OnCropUpdated { Position = pos });
                }
                break;
        }
    }
}