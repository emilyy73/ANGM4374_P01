using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyFSM : MonoBehaviour
{
    private enum State
    {
        Wander,
        Chase,
        Attack
    }

    private State currentState;

    void Update()
    {
        switch(currentState)
        {
            case State.Wander:
            {
                UpdateWander();
                break;
            }
            case State.Chase:
            {
                UpdateChase();
                break;
            }
            case State.Attack:
            {
                UpdateAttack();
                break;
            }
        }
    }

    public abstract void UpdateWander();
    public abstract void UpdateChase();
    public abstract void UpdateAttack();
}
