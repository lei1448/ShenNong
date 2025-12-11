using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;


public class ShennongAlmanac : Architecture<ShennongAlmanac>
{
   // ShennongAlmanac.cs
    protected override void Init()
    {
        this.RegisterModel<ITermModel>(new TermModel());
        this.RegisterModel<ICursorModel>(new CursorModel());
        this.RegisterModel<IGameModel>(new GameModel());
        this.RegisterModel<ICropModel>(new CropModel()); 
        this.RegisterModel<IToolModel>(new ToolModel()); // Toolbar System
        this.RegisterModel<IKnowledgeModel>(new KnowledgeModel()); // 新增
        this.RegisterSystem<InputSystem>(new InputSystem());
        this.RegisterSystem<TimeSystem>(new TimeSystem()); // 下面会写
        this.RegisterSystem<FarmSystem>(new FarmSystem()); // 下面会写
        this.RegisterSystem<WeatherSystem>(new WeatherSystem()); // 新增
    }
}
