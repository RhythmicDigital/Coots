using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CharacterHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 3;
    [SerializeField] float currentHealth;
    [SerializeField] LayerMask harmfulLayers;
    [SerializeField] LayerMask healingLayers;
    [SerializeField] float invincibleTime = 1;

    float invincibleTimer;
    SpriteRenderer renderer;
    
    void Start() 
    {
        Init();
    }

    public void Init() 
    {
        currentHealth = maxHealth;
        GameController.i.SetPlayerHealth(currentHealth);
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
        HealSprite();
        
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        GameController.i.SetPlayerHealth(currentHealth);
    }

    public void SubHealth(float health=1)
    {
        currentHealth -= health;
        HurtSprite();
        
        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth == 0) GameController.i.OnDeath();
       
        GameController.i.SetPlayerHealth(currentHealth);
        GameController.i.CameraController.ShakeCamera();

    }

    void OnTriggerEnter2D(Collider2D collider) 
    {
        if (harmfulLayers == (harmfulLayers | (1 << collider.gameObject.layer)));
        {
            if (invincibleTimer >= invincibleTime)
            {
                SubHealth();
                invincibleTimer = 0;
            }
        }

        if (healingLayers == (healingLayers | (1 << collider.gameObject.layer)));
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