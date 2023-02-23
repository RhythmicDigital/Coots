using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatTreat : Entity
{
    [SerializeField] float healAmount = 1;
    public float HealthAmount => healAmount;

    void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.CompareTag("Player"))
        {
            var boostForce = (transform.position - collider.transform.position).normalized * GlobalSettings.i.BoostSpeed;
            collider.GetComponent<Rigidbody2D>().AddForce(boostForce);
            SetActive(false);
        }
    }
}