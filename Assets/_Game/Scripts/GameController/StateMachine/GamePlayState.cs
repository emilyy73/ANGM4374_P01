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
    public CorridorFirstDungeonGenerator cfdg;
    public GameObject _cfdg;
    public float _time;
    public AutoTiler autoTiler;
    public GameObject _autoTiler;

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

        _cfdg = GameObject.Find("CorridorFirstDungeonGenerator");
        cfdg = _cfdg.GetComponent<CorridorFirstDungeonGenerator>();
        cfdg.RunProceduralGeneration();

        _controller.transform.position = new Vector3(cfdg.playerSpawn.x, 0.5f, cfdg.playerSpawn.y);
        _controller.transform.eulerAngles = new Vector3(_controller.transform.eulerAngles.x,
            _controller.transform.eulerAngles.y, _controller.transform.eulerAngles.z);
        _controller.UnitSpawner.Spawn(_controller.PlayerUnitPrefab, _controller.transform);
        _controller.transform.position = new Vector3(0, 4.5f, -5);
        _controller.transform.eulerAngles = new Vector3(_controller.transform.eulerAngles.x + 30,
            _controller.transform.eulerAngles.y, _controller.transform.eulerAngles.z);
        _controller.UnitSpawner.Spawn(_controller.CameraUnitPrefab, _controller.transform);
        _controller.transform.position = new Vector3(cfdg.bossSpawn.x, 1.75f, cfdg.bossSpawn.y);
        _controller.transform.rotation = Quaternion.identity;
        _controller.UnitSpawner.Spawn(_controller.BossUnitPrefab, _controller.transform);
        _controller.transform.position = new Vector3(cfdg.witchSpawn01.x, 0.6f, cfdg.witchSpawn01.y);
        _controller.transform.rotation = Quaternion.identity;
        _controller.UnitSpawner.Spawn(_controller.WitchUnitPrefab, _controller.transform);
        _controller.transform.position = new Vector3(cfdg.witchSpawn02.x, 0.6f, cfdg.witchSpawn02.y);
        _controller.transform.rotation = Quaternion.identity;
        _controller.UnitSpawner.Spawn(_controller.WitchUnitPrefab, _controller.transform);


        _autoTiler = GameObject.Find("AutoTiler");
        autoTiler = _autoTiler.GetComponent<AutoTiler>();
        autoTiler.RunAutoTiler();

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
        /*
        if (_controller.TouchInput.TouchIsHeld == true)
        {
            _stateMachine.ChangeState(_stateMachine.WinState);
        }
        if (_time >= 30)
        {
            _stateMachine.ChangeState(_stateMachine.LoseState);
        }
        */
    }


}
