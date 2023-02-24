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
            if (waypointIndex == 4)
            {
                GameController.i.EndGame();
            }
            else
            {
                GameController.i.Boss.MoveToWaypoint(waypointIndex);
            }
        }
    }
}