using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using TMPro;

public enum GameState { Playing, Paused, End, TitleScreen }
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
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject endScreen;
    [SerializeField] TMP_Text timerText;
    [SerializeField] List<GameObject> healthImages;
    
    GameState state;
    GameState stateBeforePause;

    float timer;

    public static GameController i { get; private set; }

    public CharacterAnimator PlayerAnimator => playerAnimator;
    public GameObject Player => player;
    public CameraController CameraController => cameraController;
    public Boss Boss => boss;
    
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
            boss.HandleUpdate();
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

    public void EndGame()
    {
        endScreen.SetActive(true);
        SetState(GameState.End);
    }

    public void SetState(GameState state)
    {
        this.state = state;
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
    }
}
