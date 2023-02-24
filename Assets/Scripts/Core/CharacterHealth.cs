using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] CharacterAnimator animator;
    [SerializeField] float maxHealth = 3;
    [SerializeField] float currentHealth;
    [SerializeField] LayerMask harmfulLayers;
    [SerializeField] LayerMask healingLayers;
    [SerializeField] float invincibleTime = 1;
    [SerializeField] SpriteRenderer renderer;

    float invincibleTimer;
    
    public event Action OnDeath;
    public event Action OnHurt;
    public event Action OnHeal;

    void Start() 
    {
        Init();
    }

    public void Init() 
    {
        currentHealth = maxHealth;
        GameController.i.SetPlayerHealth(currentHealth);

        animator.HurtAnim.OnEnd += () => { animator.SetState(animator.PreviousState); };
        
        OnHurt += () => {
            HurtSprite();
            animator.SetState(CharacterState.Hurt);
            if (gameObject.CompareTag("Player")) AudioManager.i.PlaySfx(SfxId.PlayerHurt);
            else if (gameObject.CompareTag("Dog")) AudioManager.i.PlaySfx(SfxId.DogHurt);
            else AudioManager.i.PlaySfx(SfxId.BossHurt);
        };

        OnDeath += () => { 
            animator.SetState(CharacterState.Dead);
        };

        OnHeal += () => {
            HealSprite();
        };
    }

    public void HurtSprite()
    {
        renderer.DOColor(Color.red, 0.1f).OnComplete(() => {
            renderer.DOColor(Color.white, 0.1f); 
            });
    }

    public void HealSprite()
    {
        renderer.DOColor(Color.yellow, 0.1f).OnComplete(() => {
            renderer.DOColor(Color.white, 0.1f); 
            });
    }

    public void AddHealth(float health=1)
    {
        currentHealth += health;
        OnHeal?.Invoke();
        
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        GameController.i.SetPlayerHealth(currentHealth);
        
    }

    public void SubHealth(float health=1)
    {
        currentHealth -= health;

        OnHurt?.Invoke();
        
        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth == 0)
        {
            if (gameObject.CompareTag("Player")) GameController.i.OnPlayerDeath(); 

            OnDeath?.Invoke();
        }
       
        GameController.i.SetPlayerHealth(currentHealth);
        GameController.i.CameraController.ShakeCamera();

    }

    void OnTriggerEnter2D(Collider2D collider) 
    {
        if ((harmfulLayers & (1 << collider.gameObject.layer)) != 0)
        {
            if (invincibleTimer >= invincibleTime)
            {
                SubHealth();
                invincibleTimer = 0;
            }
        }

        else if ((healingLayers & (1 << collider.gameObject.layer)) != 0)
        {
            AddHealth();
        }
    }

    void HandleInvincibleTimer()
    {
        invincibleTimer += Time.deltaTime;
    }

    public void HandleUpdate()
    {
        HandleInvincibleTimer();
    }
}