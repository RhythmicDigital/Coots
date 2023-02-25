using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using TMPro;
using System;
using UnityEngine.UI;

public enum GameState { Playing, Paused, PreGame, Win, Loss }
public class GameController : MonoBehaviour
{
    [SerializeField] Transform playerStartPoint;
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

    [SerializeField] Screen pauseScreen;
    [SerializeField] Screen titleScreen;
    [SerializeField] Screen deathScreen;
    [SerializeField] Screen endScreen;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    
    GameState state;
    GameState stateBeforePause;

    float timer;
    bool isTiming;
    public bool BossDefeated = false;

    public static GameController i { get; private set; }

    public CharacterAnimator PlayerAnimator => playerAnimator;
    public GameObject Player => player;
    public Boss Boss => boss;
    public CameraController CameraController => cameraController;

    public event Action OnPreGame;
    public event Action OnStartPlaying;
    public event Action OnLoss;
    public event Action OnPause;
    public event Action OnUnpause;
    public event Action OnWin;
    
    // Start is called before the first frame update
    void Awake()
    {
        i = this;
        PauseGame(false);
    }

    void Start() 
    {
        OnPreGame += () => {
            AudioManager.i.PlayMusic(MusicId.Title);
            titleScreen.SetActive(true);
            player.transform.position = playerStartPoint.position;
        };
        
        OnStartPlaying += () => {
            ObjectPool.i.ResetPool();
            boss.Init();
            grappleController.Disconnect();
            player.GetComponent<CharacterHealth>().Init();
            titleScreen.SetActive(false);
            player.transform.position = playerStartPoint.position;
            AudioManager.i.PlayMusic(MusicId.Gameplay);
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            StartTimer();
        };

        OnPause += () => {
            AudioManager.i.PauseMusic();
            AudioManager.i.PlaySfx(SfxId.UIPause);
            pauseScreen.SetActive(true);
            SetState(GameState.Paused);
        };

        OnUnpause += () => {
            AudioManager.i.PlayMusic();
            AudioManager.i.PlaySfx(SfxId.UIUnpause);
            pauseScreen.SetActive(false);
            SetState(GameState.Playing);
        };

        OnLoss += () => {
            AudioManager.i.PlaySfx(SfxId.PlayerDeath);
            AudioManager.i.StopMusic();
            deathScreen.SetActive(true);
            grappleController.Disconnect();
        };

        OnWin += () => {
            endScreen.SetActive(true);
            SetTimerText(timer.ToString());
            StopTimer();
        };

        pauseScreen.OnSelected += (int selection) => {
            if (selection == 0)
            {
                OnUnpause?.Invoke();
            }
            else if (selection == 1)
            {
                SetState(GameState.PreGame);
            }
        };

        deathScreen.OnSelected += (int selection) => {
            if (selection == 0)
            {
                SetState(GameState.Playing);
            }
            else if (selection == 1)
            {
                SetState(GameState.PreGame);
            }
        };

        endScreen.OnSelected += (int selection) => {
            if (selection == 0)
            {
                SetState(GameState.Playing);
            }
            else if (selection == 1)
            {
                SetState(GameState.PreGame);
            }
        };

        SetState(GameState.PreGame);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        ResetTimer();
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
            HandleTimerUpdate();
            
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

            pauseScreen.HandleUpdate();
        }

        else if (state == GameState.PreGame)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                SetState(GameState.Playing);
            }
        }

        else if (state == GameState.Loss)
        {
            deathScreen.HandleUpdate();
        }

        else if (state == GameState.Win)
        {
            endScreen.HandleUpdate();
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
            OnPause?.Invoke();
        }
        else
        {
            SetState(stateBeforePause);
            OnUnpause?.Invoke();
        }
    }

    public void WinGame() => SetState(GameState.Win);

    private void SetState(GameState newState)
    {
        if (state == newState) return;

        if (newState == GameState.Playing)
        {
            Physics.autoSimulation = true;
            Time.timeScale = 1;

            if (state == GameState.PreGame || state == GameState.Loss || state == GameState.Win)
            {
                OnStartPlaying?.Invoke();
            }
        }
        else if (newState == GameState.Paused)
        {
            Physics.autoSimulation = false;
            Time.timeScale = 0;
        }

        else if (newState == GameState.PreGame)
        {
            OnPreGame?.Invoke();
        }

        else if (newState == GameState.Loss)
        {
            OnLoss?.Invoke();
        }
        else if (newState == GameState.Win)
        {
            OnWin?.Invoke();
        }

        titleScreen.SetActive(newState == GameState.PreGame);
        pauseScreen.SetActive(newState == GameState.Paused);
        endScreen.SetActive(newState == GameState.Win);
        deathScreen.SetActive(newState == GameState.Loss);

        state = newState;
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

    void StartTimer()
    {
        isTiming = true;
    }   
    public void ResetTimer()
    {
        timer = 0;
        StopTimer();
    }

    void StopTimer()
    {
        isTiming = false;
    }

    public void HandleTimerUpdate()
    {
        if (isTiming) timer += Time.deltaTime;
    }

    void SetTimerText(string text)
    {
        timerText.text = text;
    }

    public void OnPlayerDeath()
    {
        SetState(GameState.Loss);
    }

}
