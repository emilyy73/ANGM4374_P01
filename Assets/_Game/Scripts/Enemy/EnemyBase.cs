using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBase : EnemyFSM
{
    [SerializeField]
    protected float detectionRadius = 10f;
    [SerializeField]
    protected float attackRadius = 2f;
    [SerializeField]
    protected float moveSpeed = 2f;

    protected Collider wanderArea;
    protected Vector2 destination;
    protected Transform target;

    public void SetWanderArea(Collider wanderArea)
    {
        this.wanderArea = wanderArea;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Start()
    {
        Debug.Log("Hola?");
        SetTarget(GameObject.FindWithTag("Player").transform);
        transition(EnemyState.Wander);
    }

    private Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }

    protected override void OnEnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Wander:
                destination = GetRandomPointInBounds(wanderArea.bounds);
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Dead:
                break;
        }
    }

    protected override void OnExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Wander:
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Dead:
                break;
        }
    }

    protected override void OnUpdateState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Wander:
                Wander();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Dead:
                Dead();
                break;
        }
    }

    protected void Wander()
    {
        Debug.Log("Destination: " + target.position);
        // If the enemy is close to the destination, get a new random destination
        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            destination = GetRandomPointInBounds(wanderArea.bounds);
        }

        // Move towards the destination
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        // Check if the player is within the detection radius
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Player"));
        if (player != null)
        {
            target = player.transform;
            transition(EnemyState.Chase);
        }
    }

    protected void Chase()
    {
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Check if the player is within the detection radius
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Player"));
        if (player == null)
        {
            transition(EnemyState.Wander);
        }

        // Check if the player is within the attack radius
        if (Vector3.Distance(transform.position, target.position) < attackRadius)
        {
            transition(EnemyState.Attack);
        }
    }


    protected virtual void Attack()
    {
        transition(EnemyState.Chase);
    }

    protected virtual void Dead()
    {
        // Die
    }

}
