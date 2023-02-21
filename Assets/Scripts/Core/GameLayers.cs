using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask canBeGrappledLayer;
    public static GameLayers i;

    void Awake() 
    {
        i = this;
    }

    public LayerMask PlayerLayer => playerLayer;
    public LayerMask GroundLayer => groundLayer;
    public LayerMask CanBeGrappledLayer => canBeGrappledLayer;
}
