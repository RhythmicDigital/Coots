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
        if (collider.CompareTag("Player"))
        {
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();

            rb.AddForce(GlobalSettings.i.BoostSpeed * Vector2.up, ForceMode2D.Impulse);

            AudioManager.i.PlaySfx(SfxId.TreatShot);
            SetActive(false);
        }
    }
}