using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {Playing, Paused}
public class GameController : MonoBehaviour
{
    [SerializeField] Camera worldCamera;

    GameState state;
    GameState stateBeforePause;

    public static GameController i { get; private set; }
    
    // Start is called before the first frame update
    void Awake()
    {
        i = this; 
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Playing)
        {

        }
        else if (state == GameState.Paused)
        {

        }
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforePause = state;
            state = GameState.Paused;
        }
        else 
        {
            state = stateBeforePause;
        }
    }
}
