using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadFish : Entity
{
    [SerializeField] float damageAmount = 1;
    public float DamageAmount => damageAmount;

    void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.CompareTag("Player"))
        {
            SetActive(false);
        }
    }
}