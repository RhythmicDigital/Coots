using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public enum UpgradeType { Health, Ammo, Freeze }
public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem i;
    List<UpgradeType> upgradesAcquired = new List<UpgradeType>();

    void Awake() 
    {
        i = this;
    }

    void Init()
    {
        ClearUpgrades();
    }

    public void AcquireUpgrade(UpgradeType upgrade)
    {
        upgradesAcquired.Add(upgrade);
    }

    public bool HasUpgrade(UpgradeType upgrade)
    {
        return upgradesAcquired.Contains(upgrade);
    }
    
    void ClearUpgrades()
    {
        upgradesAcquired.Clear();
    }

    public int GetUpgradeAmount()
    {
        return upgradesAcquired.Count;
    }

}