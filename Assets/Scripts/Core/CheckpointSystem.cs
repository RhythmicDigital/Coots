using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem i;

    void Awake() 
    {
        i = this;
    }
}