using UnityEditor;
using UnityEngine;

public class AtmosphereInstaller
{
    [MenuItem("Shennong/Scene/Setup Atmosphere")]
    public static void SetupAtmosphere()
    {
        var existing = Object.FindObjectOfType<SeasonAtmosphereController>();
        if (existing != null)
        {
            Debug.Log("SeasonAtmosphereController already exists in the scene.");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        var go = new GameObject("SeasonAtmosphereController");
        go.AddComponent<SeasonAtmosphereController>();
        Undo.RegisterCreatedObjectUndo(go, "Create Atmosphere Controller");
        Selection.activeGameObject = go;
        Debug.Log("Created SeasonAtmosphereController.");
    }
}
