using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using TMPro;

public enum GameState { Playing, Paused, PreGame, Win, Loss }
public class GameController : MonoBehaviour
{
    [SerializeField] Boss boss;
    [SerializeField] InputController inputController;
    [SerializeField] CharacterController2D characterController;
    [SerializeField] GrappleController grappleController;
    [SerializeField] GrabController grabController;
    [SerializeField] CameraController cameraController;
    [SerializeField] CharacterAnimator playerAnimator;
    [SerializeField] GameObject player;
    [SerializeField] Camera worldCamera;
    [SerializeField] List<GameObject> healthImages;
    [SerializeField] TMP_Text timerText;

    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject endScreen;
    
    GameState state;
    GameState stateBeforePause;

    float timer;

    public static GameController i { get; private set; }

    public CharacterAnimator PlayerAnimator => playerAnimator;
    public GameObject Player => player;
    public Boss Boss => boss;
    public CameraController CameraController => cameraController;
    
    // Start is called before the first frame update
    void Awake()
    {
        i = this;
        PauseGame(false);
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
            player.GetComponent<CharacterHealth>().HandleUpdate();
            
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
        if (pause != (state != GameState.Paused))
        {
            return;
        }

        if (pause)
        {
            stateBeforePause = state;
            SetState(GameState.Paused);
            AudioManager.i.PlaySfx(SfxId.UIPause);
        }
        else
        {
            SetState(stateBeforePause);
            AudioManager.i.PlaySfx(SfxId.UIUnpause);
        }
    }

    public void WinGame() => SetState(GameState.Win);

    private void SetState(GameState newState)
    {
        if (state == newState) return;

        if (state == GameState.Playing)
        {
            Physics.autoSimulation = true;
            Time.timeScale = 1;
        }
        else
        {
            Physics.autoSimulation = false;
            Time.timeScale = 0;
        }

        pauseScreen.SetActive(newState == GameState.Paused);
        titleScreen.SetActive(newState == GameState.PreGame);
        endScreen.SetActive(newState == GameState.Win);
        deathScreen.SetActive(newState == GameState.Loss);

    }

    public void EndGame()
    {
        endScreen.SetActive(true);
        SetState(GameState.Win);
    }

    public void SetPlayerHealth(float health)
    {
        foreach(GameObject image in healthImages) image.SetActive(false);
        
        healthImages.ElementAt((int)health).SetActive(true);
    }

    public void ResetTimer()
    {
        timer = 0;
    }

    public void HandleTimerUpdate()
    {
        timer += Time.deltaTime;
    }

    void SetTimerText(string text)
    {
        timerText.text = text;
    }

    public void OnDeath()
    {
        deathScreen.SetActive(true);
        SetState(GameState.Loss);
    }
}
