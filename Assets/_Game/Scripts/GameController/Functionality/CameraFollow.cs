using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;
    private Transform targetTransform;

    [SerializeField]
    private float xFollowDist = 0;
    [SerializeField]
    private float yFollowDist = 5f;
    [SerializeField]
    private float zFollowDist = -5f;

    private void Awake()
    {        
        player = GameObject.FindGameObjectWithTag("Player");
        targetTransform = player.GetComponent<Transform>();
    }

    private void Update()
    {
        float xDist = transform.position.x - targetTransform.position.x;
        float zDist = transform.position.z - targetTransform.position.z;

        Vector3 newPosition = targetTransform.position;
        newPosition.x += xFollowDist;
        newPosition.z += zFollowDist;
        newPosition.y += yFollowDist;

        transform.position = newPosition;
    }
}
