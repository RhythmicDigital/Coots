using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatTreat : Entity
{
    [SerializeField] float healAmount = 1;
    public float HealthAmount => healAmount;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (State != EntityState.Active) return;
    }
}