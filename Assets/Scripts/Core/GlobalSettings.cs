using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public float MaxFallVelocity = -30f;
    public float Gravity = -10;
    public float GrappleSpeed = 15;
    public float UngrappleFlySpeed = 1000;
    public float MaxVelocity = 30;
    public float MaxVerticalVelocity = 90;
    public float BoostSpeed = 5;
    public float FrameRate = 0.16f;
    public Color HighlightedColor = Color.grey;
    public Color UnhighlightedColor = Color.white;
    public static GlobalSettings i;

    // Start is called before the first frame update
    void Awake()
    {
        i = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
