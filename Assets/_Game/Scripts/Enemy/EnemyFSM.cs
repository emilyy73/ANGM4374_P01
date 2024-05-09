using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyFSM : MonoBehaviour
{
    public enum EnemyState
    {
        Wander,
        Chase,
        Attack,
        Dead
    }

    protected EnemyState currentState;
    protected EnemyState previousState;

    protected void transition(EnemyState nextState)
    {
        if (currentState != null)
        {
            previousState = currentState;
            currentState = nextState;
            OnExitState(previousState);
        }
        OnEnterState(currentState);
    }

    protected void UpdateCurrentState() {

    }

    protected virtual void OnEnterState(EnemyState state) { }
    protected virtual void OnExitState(EnemyState state) { }
    protected virtual void OnUpdateState(EnemyState state) { }

    protected virtual void Start()
    {
        currentState = EnemyState.Wander;
    }

    void Update()
    {
        OnUpdateState(currentState);
    }
}
