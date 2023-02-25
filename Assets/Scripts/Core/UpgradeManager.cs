using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.CompareTag("Upgrade"))
        {
            UpgradeType upgrade = collider.GetComponent<Ability>().Type;
            if (UpgradeSystem.i.HasUpgrade(upgrade));
            {
                UpgradeSystem.i.AcquireUpgrade(upgrade);
            }
            collider.gameObject.SetActive(false);
        }
    }
}