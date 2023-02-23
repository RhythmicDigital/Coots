using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : Entity
{
    void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.CompareTag("Player"))
        {
            SetActive(false);
        }
    }
}