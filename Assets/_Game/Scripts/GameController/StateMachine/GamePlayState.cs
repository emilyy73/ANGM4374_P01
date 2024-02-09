using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameCondition
{
    Play,
    Win,
    Lose
}

public class GamePlayState : State
{
    private GameFSM _stateMachine;
    public GameController _controller;

    public GameCondition _gameCondition = GameCondition.Play;

    public GamePlayState(GameFSM stateMachine, GameController controller)
    {
        _stateMachine = stateMachine;
        _controller = controller;
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("State: Game Play");

        //_controller.SceneChanger.ChangeScene("Level");
        _controller.UnitSpawner.Spawn(_controller.PlayerUnitPrefab, _controller.PlayerUnitSpawnLocation);

        Debug.Log("Listen for Player Inputs");
        Debug.Log("Disaply Player HUD");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedTick()
    {
        base.FixedTick();
    }

    public override void Tick()
    {
        base.Tick();
        
        if (_gameCondition == GameCondition.Win)
        {
            // go to win screen
        }
        if (_gameCondition == GameCondition.Lose)
        {
            // go to lose screen
        }
    }


}
