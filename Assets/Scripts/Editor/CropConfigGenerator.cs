using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class CropConfigGenerator : EditorWindow
{
    [MenuItem("Shennong/Generate Crop Configs")]
    public static void GenerateConfigs()
    {
        // 1. Wheat (XiaoMai)
        CreateCropConfig(101, "Wheat", "Crop/Wheat/1",
            new List<CropStageData> {
                new CropStageData { 
                    StageName = "Seedling", 
                    DurationDays = 5, 
                    StageIcon = LoadSprite("Crop/Wheat/1"),
                    TempRange = new CropParameterRange { Min = 15, Max = 20},
                    LightRange = new CropParameterRange { Min = 0, Max = 0}, // 0 means any? Or specific? Table says 0. Usually implies Low/None requirement or "Don't Care"?
                    // Actually table says "0" for Light. In gaming logic this might mean "No Light Needed" or "Darkness".
                    // But for growth, usually means "Not sensitive" or "0". Let's use 0-0 range?
                    // Wait, table col "Light Condition" says "0", "4~8", "6~8".
                    // Max is 10 usually? 
                    // Let's assume 0 is a valid value.
                    // TempRange = new CropParameterRange { Min = 15, Max = 20},
                    // LightRange = new CropParameterRange { Min = 0, Max = 0}, // Strict 0? Or 0+?
                    // Let's assume strictly 0 if range is not given, or usually "0" means >= 0?
                    // But subsequent stages have "4~8". 
                    // Let's implement literally as Min 0, Max 0 for now unless user clarifies.
                    
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 6, Max = 9}
                },
                new CropStageData { 
                    StageName = "Growing", 
                    DurationDays = 5, 
                    StageIcon = LoadSprite("Crop/Wheat/2"),
                    TempRange = new CropParameterRange { Min = 12, Max = 20},
                    LightRange = new CropParameterRange { Min = 4, Max = 8},
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 4, Max = 6}
                },
                new CropStageData { 
                    StageName = "Mature", 
                    DurationDays = 5, // Last stage duration? Table has 3 rows. Last row is IDs 103, 203 etc.
                    // Table structure: ID 101, 102, 103 are STAGES of Wheat?
                    // Yes: "Crop/Wheat/1", "Crop/Wheat/2", "Crop/Wheat/3".
                    // 101: 20HP, State 0?
                    // 102: 20HP, State 1?
                    // 103: 20HP, State 2?
                    // I will treat them as stages.
                    StageIcon = LoadSprite("Crop/Wheat/3"),
                    TempRange = new CropParameterRange { Min = 20, Max = 25},
                    LightRange = new CropParameterRange { Min = 6, Max = 8},
                    FertilityRange = new CropParameterRange { Min = 3, Max = 4},
                    MoistureRange = new CropParameterRange { Min = 5, Max = 8}
                }
            },
            new List<string> { "寒露", "霜降" } // Wheat: Hanlu, Shuangjiang
        );

        // 2. Corn (YuMi)
        CreateCropConfig(201, "Corn", "Crop/Corn/1",
            new List<CropStageData> {
                new CropStageData { 
                    StageName = "Seedling", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Corn/1"),
                    TempRange = new CropParameterRange { Min = 10, Max = 12}, // 10-12
                    LightRange = new CropParameterRange { Min = 0, Max = 0}, // 0
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 4, Max = 6}
                },
                new CropStageData { 
                    StageName = "Growing", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Corn/2"),
                    TempRange = new CropParameterRange { Min = 20, Max = 28},
                    LightRange = new CropParameterRange { Min = 7, Max = 9},
                    FertilityRange = new CropParameterRange { Min = 6, Max = 9},
                    MoistureRange = new CropParameterRange { Min = 6, Max = 9}
                },
                new CropStageData { 
                    StageName = "Mature", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Corn/3"),
                    TempRange = new CropParameterRange { Min = 22, Max = 25},
                    LightRange = new CropParameterRange { Min = 7, Max = 9},
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 4, Max = 6}
                }
            }, new List<string> { "谷雨", "立夏" } // Corn: Guyu, Lixia
        );

        // 3. Cabbage (JuanXinCai)
        CreateCropConfig(301, "Cabbage", "Crop/Cabbage/1",
             new List<CropStageData> {
                new CropStageData { 
                    StageName = "Seedling", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Cabbage/1"),
                    TempRange = new CropParameterRange { Min = 10, Max = 15},
                    LightRange = new CropParameterRange { Min = 0, Max = 0},
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 4, Max = 6}
                },
                new CropStageData { 
                    StageName = "Growing", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Cabbage/2"),
                    TempRange = new CropParameterRange { Min = 15, Max = 20},
                    LightRange = new CropParameterRange { Min = 4, Max = 8},
                    FertilityRange = new CropParameterRange { Min = 6, Max = 9},
                    MoistureRange = new CropParameterRange { Min = 5, Max = 8}
                },
                new CropStageData { 
                    StageName = "Mature", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Cabbage/3"),
                    TempRange = new CropParameterRange { Min = 15, Max = 18},
                    LightRange = new CropParameterRange { Min = 4, Max = 8},
                    FertilityRange = new CropParameterRange { Min = 5, Max = 8},
                    MoistureRange = new CropParameterRange { Min = 4, Max = 6}
                }
            }, new List<string> { "立秋", "处暑" } // Cabbage: Liqiu, Chushu
        );
        
        // 4. Soybean (DaDou)
        CreateCropConfig(401, "Soybean", "Crop/Bean/1",
             new List<CropStageData> {
                new CropStageData { 
                    StageName = "Seedling", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Bean/1"),
                    TempRange = new CropParameterRange { Min = 10, Max = 15},
                    LightRange = new CropParameterRange { Min = 0, Max = 0},
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 6, Max = 8}
                },
                new CropStageData { 
                    StageName = "Growing", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Bean/2"),
                    TempRange = new CropParameterRange { Min = 20, Max = 25},
                    LightRange = new CropParameterRange { Min = 6, Max = 8},
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 6, Max = 8}
                },
                new CropStageData { 
                    StageName = "Mature", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Bean/3"),
                    TempRange = new CropParameterRange { Min = 18, Max = 22},
                    LightRange = new CropParameterRange { Min = 7, Max = 9},
                    FertilityRange = new CropParameterRange { Min = 2, Max = 4},
                    MoistureRange = new CropParameterRange { Min = 6, Max = 8}
                }
            }, new List<string> { "夏至", "小暑" } // Soybean: Xiazhi, Xiaoshu
        );
        
        // 5. Peanut (HuaSheng)
        CreateCropConfig(501, "Peanut", "Crop/Peaunt/1",
             new List<CropStageData> {
                new CropStageData { 
                    StageName = "Seedling", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Peaunt/1"),
                    TempRange = new CropParameterRange { Min = 12, Max = 15},
                    LightRange = new CropParameterRange { Min = 0, Max = 0},
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 7, Max = 9}
                },
                new CropStageData { 
                    StageName = "Growing", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Peaunt/2"),
                    TempRange = new CropParameterRange { Min = 22, Max = 28},
                    LightRange = new CropParameterRange { Min = 6, Max = 8},
                    FertilityRange = new CropParameterRange { Min = 5, Max = 8},
                    MoistureRange = new CropParameterRange { Min = 4, Max = 6}
                },
                new CropStageData { 
                    StageName = "Mature", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Peaunt/3"),
                    TempRange = new CropParameterRange { Min = 20, Max = 25},
                    LightRange = new CropParameterRange { Min = 7, Max = 9},
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 4, Max = 6}
                }
            }, new List<string> { "谷雨", "立夏" } // Peanut: Guyu, Lixia
        );
        
        // 6. Turnip/WhiteRadish (BaiLuoBo)
        CreateCropConfig(601, "Turnip", "Crop/Carrot/1",
             new List<CropStageData> {
                new CropStageData { 
                    StageName = "Seedling", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Carrot/1"),
                    TempRange = new CropParameterRange { Min = 8, Max = 12}, // 8-12
                    LightRange = new CropParameterRange { Min = 0, Max = 0},
                    FertilityRange = new CropParameterRange { Min = 4, Max = 6},
                    MoistureRange = new CropParameterRange { Min = 3, Max = 6}
                },
                new CropStageData { 
                    StageName = "Growing", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Carrot/2"),
                    TempRange = new CropParameterRange { Min = 15, Max = 20},
                    LightRange = new CropParameterRange { Min = 4, Max = 8},
                    FertilityRange = new CropParameterRange { Min = 5, Max = 8},
                    MoistureRange = new CropParameterRange { Min = 4, Max = 8}
                },
                new CropStageData { 
                    StageName = "Mature", 
                    DurationDays = 5,
                    StageIcon = LoadSprite("Crop/Carrot/3"),
                    TempRange = new CropParameterRange { Min = 10, Max = 15},
                    LightRange = new CropParameterRange { Min = 3, Max = 6},
                    FertilityRange = new CropParameterRange { Min = 2, Max = 4},
                    MoistureRange = new CropParameterRange { Min = 3, Max = 6}
                }
            }, new List<string> { "立秋", "处暑" } // Turnip: Liqiu, Chushu
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Crop Configs Generated Successfully!");
    }

    private static Sprite LoadSprite(string path)
    {
        // "Crop/Wheat/1" -> Assets/Resources/Crop/Wheat/1.png ?
        // Or if using Resources.Load, need to be in Resources folder.
        // Assuming they are in Assets/Resources/
        // If not, we might need direct loading via AssetDatabase with full path.
        // Let's assume standard Resources for now, BUT CreateAssetMenu creates assets in Project.
        // We'll try loading from "Assets/Resources/..." 
        // Better: Use AssetDatabase to search.
        
        // User table says "Crop/Wheat/1".
        // Let's assume this is relative to a known root or Resources.
        return Resources.Load<Sprite>(path);
    }

    private static void CreateCropConfig(int id, string name, string iconPath, List<CropStageData> stages, List<string> terms)
    {
        CropConfig config = ScriptableObject.CreateInstance<CropConfig>();
        config.CropName = name;
        config.CropId = id;
        config.Icon = LoadSprite(iconPath);
        config.Stages = stages;
        config.SuitableSolarTerms = terms;
        config.MaxHP = 20;

        string path = $"Assets/Resources/CropConfigs/{name}.asset";
        
        // Ensure directory
        System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/CropConfigs");
        
        AssetDatabase.CreateAsset(config, path);
    }
}
#endif
