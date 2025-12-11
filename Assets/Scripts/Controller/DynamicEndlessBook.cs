using echo17.EndlessBook;
using UnityEngine;
using System.Collections.Generic;

public class DynamicEndlessBook : EndlessBook
{
    public void AddPage(Material material)
    {
        if (pageData == null)
        {
            pageData = new List<PageData>();
        }

        pageData.Add(new PageData { material = material });
    }

    public void ClearPages()
    {
        if (pageData != null)
        {
            pageData.Clear();
        }
    }

    public void SetPageData(int index, Material material)
    {
        if (pageData != null && index >= 0 && index < pageData.Count)
        {
            pageData[index].material = material;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Auto Assign References")]
    public void AutoAssignReferences()
    {
        // 1. Animator
        bookController = GetComponentInChildren<Animator>();
        
        // 2. Transforms
        standinsTransform = transform.Find("Standins");
        pagesTransform = transform.Find("Pages");

        // 3. Standins Array
        if (standinsTransform != null)
        {
            standins = new GameObject[5];
            // Assuming standard naming or order: ClosedFront, OpenFront, OpenMiddle, OpenBack, ClosedBack
            // But usually they are children. Let's try to find by name or index.
            // The enum order is: ClosedFront=0, OpenFront=1, OpenMiddle=2, OpenBack=3, ClosedBack=4
            
            standins[0] = FindChildRecursively(standinsTransform, "ClosedFront");
            standins[1] = FindChildRecursively(standinsTransform, "OpenFront");
            standins[2] = FindChildRecursively(standinsTransform, "OpenMiddle"); // Usually OpenMiddle_High
            standins[3] = FindChildRecursively(standinsTransform, "OpenBack");
            standins[4] = FindChildRecursively(standinsTransform, "ClosedBack");

            // Fallback: just take children in order if names don't match exactly
            if (standins[2] == null) standins[2] = FindChildRecursively(standinsTransform, "OpenMiddle_High");
        }

        // 4. Pages List
        if (pagesTransform != null)
        {
            pages = new List<Page>();
            foreach (Transform child in pagesTransform)
            {
                var page = child.GetComponent<Page>();
                if (page != null) pages.Add(page);
            }
        }

        // 5. Materials
        // Paths: Assets/Plugin/EndlessBook/Book/Materials/
        string matPath = "Assets/Plugin/EndlessBook/Book/Materials/";
        materials = new Material[5];
        materials[0] = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(matPath + "BookCover.mat");
        materials[1] = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(matPath + "BookPageBack.mat");
        materials[2] = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(matPath + "BookPageFront.mat");
        materials[3] = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(matPath + "BookPageLeft.mat");
        materials[4] = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(matPath + "BookPageRight.mat");

        // 6. Standin Qualities
        standinQualities = new StandinQualityEnum[5];
        for(int i=0; i<5; i++) standinQualities[i] = StandinQualityEnum.High;

        // 7. Other Settings
        maxPagesTurningCount = 5;
        deltaTime = DeltaTimeEnum.deltaTime;
        pageFillerMaterial = materials[2]; // Default to Front

        Debug.Log("DynamicEndlessBook: Auto Assignment Complete!");
        UnityEditor.EditorUtility.SetDirty(this);
    }

    private GameObject FindChildRecursively(Transform parent, string partOfName)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(partOfName)) return child.gameObject;
            var res = FindChildRecursively(child, partOfName);
            if (res != null) return res;
        }
        return null;
    }
#endif
}
