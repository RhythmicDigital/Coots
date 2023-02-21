using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    public static GameLayers i;

    void Awake() 
    {
        i = this;
    }

    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }
}
