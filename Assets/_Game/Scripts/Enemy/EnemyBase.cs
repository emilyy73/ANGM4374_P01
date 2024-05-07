using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBase : EnemyFSM
{
    [SerializeField]
    private float detectionRadius = 10f;
    [SerializeField]
    private float moveSpeed = 2f;
    [SerializeField]
    private float wanderRadius;

    private Vector3 startingPosition;

    void Start()
    {
        startingPosition = this.transform.position;
        this.transition(State.Wander);
    }

    override void UpdateAttack()
    {
        throw new System.NotImplementedException();
    }

    override void UpdateChase()
    {
        throw new System.NotImplementedException();
    }

    override void UpdateWander()
    {
        throw new System.NotImplementedException();
    }

    Vector3 GetRoamingPosition()
    {
        return startingPosition + GetRandomDirec() * Random.Range(10f, 50f);
    }

    static Vector3 GetRandomDirec()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
