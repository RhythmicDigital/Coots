using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public float Gravity = -10;
    public float FrameRate = 0.16f;
    public float CameraDepth = -10f;
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
