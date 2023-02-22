using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {Playing, Paused}
public class GameController : MonoBehaviour
{
    [SerializeField] InputController inputController;
    [SerializeField] CharacterController2D characterController;
    [SerializeField] GrappleController grappleController;
    [SerializeField] GrabController grabController;
    [SerializeField] CameraController cameraController;
    [SerializeField] CharacterAnimator playerAnimator;
    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject endScreen;
    
    GameState state;
    GameState stateBeforePause;

    public static GameController i { get; private set; }

    public CharacterAnimator PlayerAnimator => playerAnimator;
    
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
            grabController.HandleUpdate();
            cameraController.HandleUpdate();
            
            if (Input.GetButtonDown("Pause"))
            {
                PauseGame(true);
            }
        }

        else if (state == GameState.Paused)
        {
            if (Input.GetButtonDown("Pause"))
            {
                PauseGame(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void FixedUpdate() 
    {
        if (state == GameState.Playing)
        {
            inputController.HandleFixedUpdate();
            characterController.HandleFixedUpdate();
            grabController.HandleFixedUpdate();
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

            Physics.autoSimulation = false;
            Time.timeScale = 0;
            
            AudioManager.i.PlaySfx(SfxId.UIPause);
        }
        else
        {
            pauseScreen.SetActive(false);
            state = stateBeforePause;

            Physics.autoSimulation = true;
            Time.timeScale = 1;

            AudioManager.i.PlaySfx(SfxId.UIUnpause);
        }
    }
}
