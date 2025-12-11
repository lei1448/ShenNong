// Controller/SeedBarViewController.cs
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class SeedBarViewController : MonoBehaviour, IController
{
    [Header("Settings")]
    public List<CropConfig> allSeeds; // 在Inspector里拖入所有可用的种子配置
    public GameObject slotPrefab;     // 拖入上面制作的 SeedSlot Prefab
    public Transform slotContainer;   // 拖入挂载了 HorizontalLayoutGroup 的父物体

    private ICropModel _cropModel;
    private readonly List<GameObject> _spawnedSlots = new();

    public IArchitecture GetArchitecture() => ShennongAlmanac.Interface;

    void Start()
    {
        _cropModel = this.GetModel<ICropModel>();

        // 1. 生成所有种子按钮
        foreach (var seedConfig in allSeeds)
        {
            GenerateSlot(seedConfig);
        }

        // 2. 监听选中变化，更新UI高亮状态
        _cropModel.SelectedSeedConfig.Register(OnSelectionChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);

        // 3. 默认选中第一个
        if (allSeeds.Count > 0)
        {
            _cropModel.SelectedSeedConfig.Value = allSeeds[0];
        }
    }

    private void GenerateSlot(CropConfig config)
    {
        GameObject slotObj = Instantiate(slotPrefab, slotContainer);
        _spawnedSlots.Add(slotObj);

        // 设置图标 (假设 Prefab 里有一个名为 "Icon" 的子物体 Image)
        if (slotObj.transform.Find("Icon").TryGetComponent<Image>(out var iconImg)) iconImg.sprite = config.Icon;

        // 绑定点击事件
        Button btn = slotObj.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            // 点击时，修改 Model 数据
            _cropModel.SelectedSeedConfig.Value = config;
        });
    }

    private void OnSelectionChanged(CropConfig newConfig)
    {
        // 遍历所有按钮，更新高亮状态
        for (int i = 0; i < allSeeds.Count; i++)
        {
            // 假设 Prefab 里有一个名为 "Highlight" 的子物体
            Transform highlight = _spawnedSlots[i].transform.Find("Highlight");
            
            if (highlight != null)
            {
                bool isSelected = (allSeeds[i] == newConfig);
                highlight.gameObject.SetActive(isSelected);
            }
        }
    }
}