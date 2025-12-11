using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public interface ITermModel : IModel
{
    public BindableProperty<int> CurrentTermId { get; set; }
    public string CurrentTermName { get; }
    public int CurrentTemp { get; }
    public string CurrentLight{ get; }
    public int CurrentLightValue { get; }
    public int CurrentMoistureValue { get; }
    public int CurrentFertilityValue { get; }
    BindableProperty<int> CurrentDayInTerm { get; } // 当前节气的第几天
    int DaysPerTerm { get; }
    Season GetSeason();
}

public class TermModel : AbstractModel, ITermModel
{
    public BindableProperty<int> CurrentTermId { get; set; } = new();
    public BindableProperty<int> CurrentDayInTerm { get; set; } = new(1);
    public int DaysPerTerm => 15;
    
    public string CurrentTermName { get => _solorTermsNameInfo[CurrentTermId.Value]; }
    public int CurrentTemp { get => _solorTermsTempretureInfo[CurrentTermName]; }
    public string CurrentLight { get => _solorTermsLightInfo[CurrentTermName]; }
    public int CurrentLightValue { get => _solorTermsLightValInfo[CurrentTermName]; }
    public int CurrentMoistureValue { get => _solorTermsMoistureValInfo[CurrentTermName]; }
    public int CurrentFertilityValue { get => 50; } // Default fertility

    /// <summary>
    /// 二十四节气中文名信息
    /// </summary>
    private readonly List<string> _solorTermsNameInfo = new()
    {
        "立春", "雨水", "惊蛰", "春分", "清明", "谷雨",
        "立夏", "小满", "芒种", "夏至", "小暑", "大暑",
        "立秋", "处暑", "白露", "秋分", "寒露", "霜降",
        "立冬", "小雪", "大雪", "冬至", "小寒", "大寒"
    };
    
    /// <summary>
    /// 存储二十四节气及其对应的象征性平均温度。
    /// </summary>
    private readonly Dictionary<string, int> _solorTermsTempretureInfo = new()
    {
        // 春季
        { "立春", 5 },   { "雨水", 8 },   { "惊蛰", 12 },  { "春分", 15 },  { "清明", 18 },  { "谷雨", 20 },
        // 夏季
        { "立夏", 25 },  { "小满", 28 },  { "芒种", 30 },  { "夏至", 33 },  { "小暑", 35 },  { "大暑", 38 },
        // 秋季
        { "立秋", 32 },  { "处暑", 28 },  { "白露", 22 },  { "秋分", 18 },  { "寒露", 12 },  { "霜降", 8 },
        // 冬季
        { "立冬", 5 },   { "小雪", 0 },   { "大雪", -5 },  { "冬至", -8 },  { "小寒", -6 },  { "大寒", -10 }
    };

    /// <summary>
    /// 存储二十四节气及其对应的象征。
    /// </summary>
    private readonly Dictionary<string, string> _solorTermsLightInfo = new()
    {
        // 春季
        { "立春", "阳光和煦万物复苏" }, { "雨水", "春雨绵绵滋润大地" }, { "惊蛰", "春雷阵阵生机勃勃" },
        { "春分", "昼夜平分春光明媚" }, { "清明", "天清地明春意盎然" }, { "谷雨", "雨水充沛暖意融融" },
        // 夏季
        { "立夏", "日光渐盛绿荫渐浓" }, { "小满", "阳光充足生机旺盛" }, { "芒种", "时有雷雨天气闷热" },
        { "夏至", "日照最长烈日炎炎" }, { "小暑", "骄阳似火热浪袭人" }, { "大暑", "赤日当空酷暑难耐" },
        // 秋季
        { "立秋", "天高云淡秋意初现" }, { "处暑", "暑气渐消秋风送爽" }, { "白露", "秋高气爽丹桂飘香" },
        { "秋分", "昼夜平分秋色宜人" }, { "寒露", "秋意深浓晨雾弥漫" }, { "霜降", "秋晚风凉霜染枫林" },
        // 冬季
        { "立冬", "寒风乍起日光偏斜" }, { "小雪", "阴云密布偶有飞雪" }, { "大雪", "千里冰封白雪皑皑" },
        { "冬至", "日照最短寒气逼人" }, { "小寒", "天寒地冻阳光微弱" }, { "大寒", "冰天雪地寒风刺骨" }
    };

    // Light Value (0-100)
     private readonly Dictionary<string, int> _solorTermsLightValInfo = new()
    {
        { "立春", 50 }, { "雨水", 40 }, { "惊蛰", 50 }, { "春分", 60 }, { "清明", 70 }, { "谷雨", 60 },
        { "立夏", 80 }, { "小满", 85 }, { "芒种", 80 }, { "夏至", 100 }, { "小暑", 95 }, { "大暑", 90 },
        { "立秋", 80 }, { "处暑", 75 }, { "白露", 60 }, { "秋分", 50 }, { "寒露", 40 }, { "霜降", 30 },
        { "立冬", 20 }, { "小雪", 15 }, { "大雪", 10 }, { "冬至", 5 }, { "小寒", 10 }, { "大寒", 15 }
    };

    // Moisture Value (0-100)
    private readonly Dictionary<string, int> _solorTermsMoistureValInfo = new()
    {
        { "立春", 40 }, { "雨水", 80 }, { "惊蛰", 60 }, { "春分", 50 }, { "清明", 60 }, { "谷雨", 90 },
        { "立夏", 60 }, { "小满", 70 }, { "芒种", 80 }, { "夏至", 50 }, { "小暑", 40 }, { "大暑", 30 },
        { "立秋", 30 }, { "处暑", 40 }, { "白露", 50 }, { "秋分", 40 }, { "寒露", 30 }, { "霜降", 20 },
        { "立冬", 20 }, { "小雪", 40 }, { "大雪", 50 }, { "冬至", 40 }, { "小寒", 30 }, { "大寒", 20 }
    };

    protected override void OnInit()
    {
        CurrentTermId.Register(newId =>
        {
            if (newId > 23)
            {
                CurrentTermId.Value = 0;
            }
            else if(newId < 0)
            {
                CurrentTermId.Value = 23;
            }
        });
        CurrentTermId.Value = 0;
    }

    public Season GetSeason()
    {
        // 0-5 Spring, 6-11 Summer, 12-17 Autumn, 18-23 Winter
        int termId = CurrentTermId.Value;
        if (termId >= 0 && termId <= 5) return Season.Spring;
        if (termId >= 6 && termId <= 11) return Season.Summer;
        if (termId >= 12 && termId <= 17) return Season.Autumn;
        return Season.Winter;
    }
}

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter
}
