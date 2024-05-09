using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyBase
{
    [SerializeField]
    private float attackRate = 1f;
    [SerializeField]
    private float dashForce = 10f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void Attack()
    {
        Vector3 dirToPlayer = (target.position - transform.position).normalized; 
        rb.AddForce(dirToPlayer * dashForce, ForceMode.Impulse);

        transition(EnemyState.Chase);
    }
}
