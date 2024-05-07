using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : StateMachineMB
{
    public WanderState wanderState;
    public ChaseState chaseState;
    public AttackState attackState;
    public Stats stats;

    void Awake ()
    {
        wanderState = new WanderState(stats);
        chaseState = new ChaseState(stats);
        attackState = new AttackState(stats);
    }

}
