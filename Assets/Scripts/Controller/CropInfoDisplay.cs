using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class CropInfoDisplay : MonoBehaviour, IController
{
    private ICropModel _cropModel;
    private ICursorModel _cursorModel;

    public IArchitecture GetArchitecture()
    {
        return ShennongAlmanac.Interface;
    }

    void Start()
    {
        _cropModel = this.GetModel<ICropModel>();
        _cursorModel = this.GetModel<ICursorModel>();
    }

    void OnGUI()
    {
        if (_cropModel == null || _cursorModel == null) return;

        Vector3Int cursorPos = _cursorModel.CursorPos;
        CropData crop = _cropModel.GetCrop(cursorPos);

        if (crop != null)
        {
            // Position the box near the mouse
            Vector2 mousePos = Event.current.mousePosition;
            
            // Define styles for larger text
            GUIStyle headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.fontSize = 24;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = Color.white;
            headerStyle.alignment = TextAnchor.UpperCenter;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 20;
            labelStyle.normal.textColor = Color.white;
            
            // Increase box size
            float width = 400;
            float height = 450;
            Rect rect = new Rect(mousePos.x + 20, mousePos.y + 20, width, height);

            GUI.Box(rect, "Crop Info", headerStyle);

            GUILayout.BeginArea(rect);
            GUILayout.Space(40); // Header space

            GUILayout.BeginVertical();
            GUILayout.Label($"Name: {crop.CropId}", labelStyle);
            GUILayout.Label($"Growth: {crop.GrowthDays}/{crop.Config.TotalGrowthDays} Days", labelStyle);
            GUILayout.Label($"HP: {crop.CurrentHP}/{crop.Config.MaxHP}", labelStyle);
            
            var stage = crop.Config.GetCurrentStageData(crop.GrowthDays);
            GUILayout.Label($"Stage: {stage.StageName}", labelStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("--- Environment ---", labelStyle);
            GUILayout.Label($"Temp: {crop.CurrentState.Temp}", labelStyle);
            GUILayout.Label($"Light: {crop.CurrentState.Light}", labelStyle);
            GUILayout.Label($"Moisture: {crop.CurrentState.Moisture}", labelStyle);
            GUILayout.Label($"Fertility: {crop.CurrentState.Fertility}", labelStyle);
            GUILayout.EndVertical();
            
            GUILayout.EndArea();
        }
    }
}
