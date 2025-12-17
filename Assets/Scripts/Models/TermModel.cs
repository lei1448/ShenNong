using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public struct TermData
{
    public int Id;
    public string Name;
    public string Description;
    public int TempMin;
    public int TempMax;
    public int Light;
    public int Moisture;
    public int WeatherEffectId; // 1: Rain/Growth?, 2: Snow/Cold?
    public string WeatherParams;
}

public interface ITermModel : IModel
{
    public BindableProperty<int> CurrentTermId { get; set; }
    public string CurrentTermName { get; }
    public string CurrentTermDescription { get; }
    public int CurrentTemp { get; } // Average Temp
    public int CurrentTermTempMin { get; }
    public int CurrentTermTempMax { get; }
    
    public string CurrentLight{ get; } // Description, maybe remove if not in table, or keep legacy
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

    public TermData CurrentTermData => _termDatas[Mathf.Clamp(CurrentTermId.Value, 0, 23)];

    public string CurrentTermName => CurrentTermData.Name;
    public string CurrentTermDescription => CurrentTermData.Description;
    public int CurrentTemp => (CurrentTermData.TempMin + CurrentTermData.TempMax) / 2;
    public int CurrentTermTempMin => CurrentTermData.TempMin;
    public int CurrentTermTempMax => CurrentTermData.TempMax;
    
    public string CurrentLight => CurrentTermData.Description; // Placeholder, legacy string desc not in table
    public int CurrentLightValue => CurrentTermData.Light;
    public int CurrentMoistureValue => CurrentTermData.Moisture;
    public int CurrentFertilityValue => 50; // Default fertility

    private readonly List<TermData> _termDatas = new()
    {
        // Spring (0-5)
        new TermData { Id=101, Name="立春", Description="立，始建也。春气始而建立也。", TempMin=0, TempMax=10, Light=5, Moisture=4, WeatherEffectId=2, WeatherParams="0°C" },
        new TermData { Id=102, Name="雨水", Description="天街小雨润如酥，草色遥看近却无", TempMin=5, TempMax=12, Light=5, Moisture=5, WeatherEffectId=1, WeatherParams="水分+1, 光照0" },
        new TermData { Id=103, Name="惊蛰", Description="微雨众卉新，一雷惊蛰始", TempMin=5, TempMax=12, Light=5, Moisture=5, WeatherEffectId=1, WeatherParams="水分+1, 光照0" },
        new TermData { Id=104, Name="春分", Description="仲春初四日，春色正中分", TempMin=8, TempMax=15, Light=6, Moisture=5, WeatherEffectId=1, WeatherParams="水分+1, 光照0" },
        new TermData { Id=105, Name="清明", Description="清明时节雨纷纷，路上行人欲断魂", TempMin=12, TempMax=20, Light=6, Moisture=5, WeatherEffectId=1, WeatherParams="水分+2, 光照0" },
        new TermData { Id=106, Name="谷雨", Description="谷雨前后，种瓜点豆", TempMin=15, TempMax=22, Light=8, Moisture=6, WeatherEffectId=1, WeatherParams="水分+2, 光照0" },

        // Summer (6-11)
        new TermData { Id=107, Name="立夏", Description="陇亩日长蒸翠麦，园林雨过熟黄梅", TempMin=14, TempMax=26, Light=8, Moisture=5, WeatherEffectId=1, WeatherParams="水分+2, 光照0" },
        new TermData { Id=108, Name="小满", Description="最爱垄头麦，迎风笑落红", TempMin=17, TempMax=29, Light=8, Moisture=5, WeatherEffectId=1, WeatherParams="水分+2, 光照0" },
        new TermData { Id=109, Name="芒种", Description="时雨及芒种，四野皆插秧", TempMin=20, TempMax=32, Light=9, Moisture=8, WeatherEffectId=1, WeatherParams="水分+3, 光照0" },
        new TermData { Id=110, Name="夏至", Description="昼晷已云极，宵漏自此长", TempMin=28, TempMax=35, Light=10, Moisture=8, WeatherEffectId=1, WeatherParams="水分+3, 光照0" },
        new TermData { Id=111, Name="小暑", Description="小暑大暑，上蒸下煮", TempMin=25, TempMax=32, Light=9, Moisture=6, WeatherEffectId=1, WeatherParams="水分+3, 光照0" },
        new TermData { Id=112, Name="大暑", Description="大暑热不透，大热在秋后", TempMin=30, TempMax=36, Light=9, Moisture=9, WeatherEffectId=1, WeatherParams="水分+3, 光照0" },

        // Autumn (12-17)
        new TermData { Id=113, Name="立秋", Description="立秋凉风至，白露生寒蝉", TempMin=25, TempMax=30, Light=8, Moisture=8, WeatherEffectId=1, WeatherParams="水分+2, 光照0" },
        new TermData { Id=114, Name="处暑", Description="处暑出暑天渐凉，五谷丰登秋收忙", TempMin=20, TempMax=26, Light=6, Moisture=4, WeatherEffectId=1, WeatherParams="水分+1, 光照0" },
        new TermData { Id=115, Name="白露", Description="露从今夜白，月是故乡明", TempMin=17, TempMax=26, Light=6, Moisture=3, WeatherEffectId=1, WeatherParams="水分+1, 光照0" },
        new TermData { Id=116, Name="秋分", Description="秋分昼夜均，寒暑平", TempMin=15, TempMax=20, Light=6, Moisture=3, WeatherEffectId=1, WeatherParams="水分+1, 光照0" },
        new TermData { Id=117, Name="寒露", Description="寒露惊秋晚，朝看菊渐黄", TempMin=10, TempMax=15, Light=5, Moisture=2, WeatherEffectId=1, WeatherParams="0°C, 光照0" },
        new TermData { Id=118, Name="霜降", Description="霜降见霜，米谷满仓", TempMin=5, TempMax=10, Light=4, Moisture=2, WeatherEffectId=2, WeatherParams="0°C, 光照0" },
        
        // Winter (18-23)
        new TermData { Id=119, Name="立冬", Description="醉看墨花月白，恍疑雪满前村", TempMin=5, TempMax=14, Light=5, Moisture=4, WeatherEffectId=2, WeatherParams="温度-3°C, 光照0" },
        new TermData { Id=120, Name="小雪", Description="花雪随风不厌看，更多还肯失林峦", TempMin=0, TempMax=5, Light=2, Moisture=4, WeatherEffectId=2, WeatherParams="温度-3°C, 光照0" },
        new TermData { Id=121, Name="大雪", Description="节气今朝逢大雪，清晨瓦上雪微凝", TempMin=-10, TempMax=-3, Light=2, Moisture=4, WeatherEffectId=2, WeatherParams="温度-3°C, 光照0" },
        new TermData { Id=122, Name="冬至", Description="天街晓色瑞烟浓，名纸相传尽贺冬", TempMin=-5, TempMax=3, Light=2, Moisture=2, WeatherEffectId=2, WeatherParams="温度-3°C, 光照0" },
        new TermData { Id=123, Name="小寒", Description="花外东风作小寒，轻红淡白满阑干", TempMin=-9, TempMax=-5, Light=2, Moisture=2, WeatherEffectId=2, WeatherParams="温度-3°C, 光照0" },
        new TermData { Id=124, Name="大寒", Description="旧雪未及消，新雪又拥户", TempMin=-5, TempMax=3, Light=2, Moisture=2, WeatherEffectId=2, WeatherParams="温度-3°C, 光照0" },
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
