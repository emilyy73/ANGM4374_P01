using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [field: SerializeField]
    public Unit PlayerUnitPrefab { get; private set; }
    [field: SerializeField]
    public Unit CameraUnitPrefab { get; private set; }
    [field: SerializeField]
    public Unit BossUnitPrefab { get; private set; }
    [field: SerializeField]
    public Unit WitchUnitPrefab { get; private set; }
    [field: SerializeField]
    public List<Unit> EnemyUnitsPrefabs { get; private set; }
    public Transform PlayerUnitSpawnLocation { get; private set; }
    [field: SerializeField]
    public UnitSpawner UnitSpawner { get; private set; }
    [field: SerializeField]
    public SceneChanger SceneChanger { get; private set; }
    [field: SerializeField]
    public TouchInput TouchInput { get; private set; }

    public void CloseApplication()
    {
        Application.Quit();
    }
}