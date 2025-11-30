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
    BindableProperty<int> CurrentDayInTerm { get; } // 当前节气的第几天
    int DaysPerTerm { get; }
    Season GetSeason();
}
public class TermModel : AbstractModel,ITermModel
{
    public BindableProperty<int> CurrentTermId { get; set; } = new();
    public BindableProperty<int> CurrentDayInTerm { get; set; } = new(1);
    public int DaysPerTerm => 15;
    public string CurrentTermName { get => _solorTermsNameInfo[CurrentTermId.Value]; }
    public int CurrentTemp { get => _solorTermsTempretureInfo[CurrentTermName]; }
    public string CurrentLight { get => _solorTermsLightInfo[CurrentTermName]; }

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
    /// <remarks>
    /// Key: 节气名
    /// Value: 平均温度
    /// </remarks>
    private readonly Dictionary<string, int> _solorTermsTempretureInfo = new()
    {
        // 春季
        { "立春", 5 },   // 春始，乍暖还寒
        { "雨水", 8 },   // 气温回升，降水增多
        { "惊蛰", 12 },  // 天气转暖，春雷始鸣
        { "春分", 15 },  // 昼夜平分，气候温和
        { "清明", 18 },  // 清新明朗，气温渐升
        { "谷雨", 20 },  // 雨生百谷，温暖湿润

        // 夏季
        { "立夏", 25 },  // 夏日伊始，气温显著升高
        { "小满", 28 },  // 夏熟作物开始饱满
        { "芒种", 30 },  // 炎热多雨，农忙时节
        { "夏至", 33 },  // 白昼最长，炎热至极
        { "小暑", 35 },  // 暑气渐盛，进入伏天
        { "大暑", 38 },  // 一年中最热的时期

        // 秋季
        { "立秋", 32 },  // 秋季开始，暑气未消
        { "处暑", 28 },  // 暑气渐退，天气转凉
        { "白露", 22 },  // 气温下降快，晨有露水
        { "秋分", 18 },  // 昼夜平分，秋高气爽
        { "寒露", 12 },  // 气温更低，露水有寒意
        { "霜降", 8 },   // 天气渐冷，开始有霜

        // 冬季
        { "立冬", 5 },   // 冬季开始，气温下降
        { "小雪", 0 },   // 开始降雪，天气寒冷
        { "大雪", -5 },  // 降雪范围广，天气更冷
        { "冬至", -8 },  // 白昼最短，数九寒天
        { "小寒", -6 },  // 气候寒冷，尚未至极
        { "大寒", -10 }  // 一年中最冷的时期
    };

    /// <summary>
    /// 存储二十四节气及其对应的象征。
    /// </summary>
    /// <remarks>
    /// Key: 节气名
    /// Value: 光照/描述
    /// </remarks>
    private readonly Dictionary<string, string> _solorTermsLightInfo = new()
    {
        // 春季
        { "立春", "阳光和煦，万物复苏" },
        { "雨水", "春雨绵绵，滋润大地" },
        { "惊蛰", "春雷阵阵，生机勃勃" },
        { "春分", "昼夜平分，春光明媚" },
        { "清明", "天清地明，春意盎然" },
        { "谷雨", "雨水充沛，暖意融融" },

        // 夏季
        { "立夏", "日光渐盛，绿荫渐浓" },
        { "小满", "阳光充足，生机旺盛" },
        { "芒种", "时有雷雨，天气闷热" },
        { "夏至", "日照最长，烈日炎炎" },
        { "小暑", "骄阳似火，热浪袭人" },
        { "大暑", "赤日当空，酷暑难耐" },

        // 秋季
        { "立秋", "天高云淡，秋意初现" },
        { "处暑", "暑气渐消，秋风送爽" },
        { "白露", "秋高气爽，丹桂飘香" },
        { "秋分", "昼夜平分，秋色宜人" },
        { "寒露", "秋意深浓，晨雾弥漫" },
        { "霜降", "秋晚风凉，霜染枫林" },

        // 冬季
        { "立冬", "寒风乍起，日光偏斜" },
        { "小雪", "阴云密布，偶有飞雪" },
        { "大雪", "千里冰封，白雪皑皑" },
        { "冬至", "日照最短，寒气逼人" },
        { "小寒", "天寒地冻，阳光微弱" },
        { "大寒", "冰天雪地，寒风刺骨" }
    };

    protected override void OnInit()
    {
        CurrentTermId.Register(newId =>
        {
            if (newId > 23)
            {
                CurrentTermId.Value = 0;
                return;
            }
            else if(newId < 0)
            {
                CurrentTermId.Value = 23;
                return;
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
