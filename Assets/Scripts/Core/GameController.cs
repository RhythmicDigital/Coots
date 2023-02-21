using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {Playing, Paused}
public class GameController : MonoBehaviour
{
    [SerializeField] InputController inputController;
    [SerializeField] CharacterController2D characterController;
    [SerializeField] GrappleController grappleController;
    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject endScreen;
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
            inputController.HandleUpdate();
            grappleController.HandleUpdate();
        }
        else if (state == GameState.Paused)
        {
            
        }
    }

    void FixedUpdate() 
    {
        if (state == GameState.Playing)
        {
            inputController.HandleFixedUpdate();
            characterController.HandleFixedUpdate();
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
            pauseScreen.SetActive(true);
            state = GameState.Paused;
        }
        else 
        {
            pauseScreen.SetActive(false);
            state = stateBeforePause;
        }
    }
}
