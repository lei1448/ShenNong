using UnityEditor;
using UnityEngine;

public class NextRoundInstaller
{
    [MenuItem("Shennong/Scene/Setup Next Round Controller")]
    public static void SetupNextRoundController()
    {
        var existing = Object.FindObjectOfType<NextRoundController>();
        if (existing != null)
        {
            Debug.Log("NextRoundController already exists in the scene.");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        var go = new GameObject("NextRoundController");
        go.AddComponent<NextRoundController>();
        Undo.RegisterCreatedObjectUndo(go, "Create Next Round Controller");
        Selection.activeGameObject = go;
        Debug.Log("Created NextRoundController.");
    }
}
