using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadFish : Entity
{
    [SerializeField] float damageAmount = 1;
    public float DamageAmount => damageAmount;

    private void Awake()
    {
        var pr = GetComponent<GrappleObject>();
        pr.Interact = Deflect;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (State != EntityState.Active) return;
        if (collider.CompareTag("Player"))
        {
            AudioManager.i.PlaySfx(SfxId.FishShot);
            SetActive(false);
        }
    }

    private void Deflect(RaycastHit2D hit)
    {
        // SetMoveDirection(-MoveDirection);
        transform.rotation *= Quaternion.Euler(0, 0, 180);
    }
}