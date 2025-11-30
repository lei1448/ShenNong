using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class FarmSystem : AbstractSystem
{
    private ICropModel _cropModel;

    protected override void OnInit()
    {
        _cropModel = this.GetModel<ICropModel>();
        this.RegisterEvent<OnDayPass>(OnDayPass);
    }

    private void OnDayPass(OnDayPass e)
    {
        List<Vector3Int> positions = new List<Vector3Int>(_cropModel.CropMap.Keys);

        foreach (var pos in positions)
        {
            var crop = _cropModel.CropMap[pos];
            if (!crop.IsMature)
            {
                crop.GrowthDays++;
                if (crop.GrowthDays >= crop.Config.DaysToMature)
                {
                    crop.IsMature = true;
                }
                this.SendEvent(new OnCropUpdated { Position = pos });
            }
        }
    }

    public void PlantCrop(Vector3Int pos, CropConfig config)
    {
        if (config == null) return;

        var termModel = this.GetModel<ITermModel>();
        string currentTerm = termModel.CurrentTermName;
        
        if (config.SuitableSolarTerms != null && config.SuitableSolarTerms.Count > 0)
        {
            if (!config.SuitableSolarTerms.Contains(currentTerm))
            {
                Debug.Log($"[FarmSystem] Cannot plant {config.CropName} in {currentTerm}. Suitable: {string.Join(",", config.SuitableSolarTerms)}");
                return;
            }
            else
            {
                Debug.Log($"[FarmSystem] Seasonal check passed for {config.CropName} in {currentTerm}.");
            }
        }
        else
        {
            Debug.Log($"[FarmSystem] No seasonal restrictions for {config.CropName}.");
        }

        if (_cropModel.GetCrop(pos) == null)
        {
            _cropModel.AddCrop(pos, config);
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

            _cropModel.RemoveCrop(pos);
            this.SendEvent(new OnCropUpdated { Position = pos }); 
        }
    }
}