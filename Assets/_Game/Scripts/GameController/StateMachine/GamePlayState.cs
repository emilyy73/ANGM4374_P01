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

    // remove after testing
    public SimpleRandomWalkMapGenerator srwmg;
    public GameObject _srwmg;
    public float _time;

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

        Debug.Log("Listen for Player Inputs");

        // remove after testing
        _srwmg = GameObject.Find("SimpleRandomWalkDungeonGenerator");
        srwmg = _srwmg.GetComponent<SimpleRandomWalkMapGenerator>();
        srwmg.RunProceduralGeneration();
        _controller.UnitSpawner.Spawn(_controller.PlayerUnitPrefab, _controller.PlayerUnitSpawnLocation);

        Debug.Log("Display Player HUD");
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

        _time += Time.deltaTime;
        Debug.Log(_controller.TouchInput.TouchIsHeld);

        if (_controller.TouchInput.TouchIsHeld == true)
        {
            _stateMachine.ChangeState(_stateMachine.WinState);
        }
        if (_time >= 5)
        {
            _stateMachine.ChangeState(_stateMachine.LoseState);
        }
    }


}
