using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;


public enum E_GameType
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public interface IGameModel : IModel
{
    public BindableProperty<E_GameType> gameType { get; set; }
}

public class GameModel : AbstractModel, IGameModel
{
    public BindableProperty<E_GameType> gameType { get ; set; } = new(E_GameType.MainMenu);

    protected override void OnInit()
    {
        gameType.Value = E_GameType.Playing;
        gameType.Register(gameType =>
        {
            this.SendEvent<OnGameStart>();
        });
    }
}
