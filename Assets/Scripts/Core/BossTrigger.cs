using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] int waypointIndex;

    void Start() 
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.CompareTag("Player"))
        {
            GameController.i.Boss.MoveToWaypoint(waypointIndex);
        }
    }
}