using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum ShootType { Single, Circle }
public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] SfxId shootSound;
    [SerializeField] ShootType shootType;
    [SerializeField] List<string> projectileNames;
    [SerializeField] bool automaticFiring = true;
    [SerializeField] bool targetPlayer;
    [SerializeField] float delayBetweenShots = 1;
    [SerializeField] float projectileSpeed = 10;
    [SerializeField] Vector3 shootDirection;
    [SerializeField] float angleIncrement = 10;
    [SerializeField] float radius = 1;

    float timeSinceLastShot = 0;
    int currentProjectileIndex;

    public event Action OnShoot;

    [SerializeField] CharacterAnimator animator;
    Transform mainCameraTransform;

    void Awake() 
    {
        animator = GetComponent<CharacterAnimator>();
        mainCameraTransform = Camera.main.transform;
    }
    
    void Start() 
    {
        animator.ShootAnim.Init();
        animator.ShootAnim.OnEnd += () => {
            animator.SetState(CharacterState.Idle);
        };
        
        OnShoot += () => {
            var yDist = Mathf.Abs(mainCameraTransform.position.y - transform.position.y);
            if(yDist > 15)
            {
                // If the shooter is far off the screen, don't play the sound
                return;
            }

            if (shootSound != SfxId.Null) AudioManager.i.PlaySfx(shootSound);
            animator.SetState(CharacterState.Shooting);
        };
    }
    public void Init()
    {
        OnShoot = null;
        currentProjectileIndex = 0;
        timeSinceLastShot = 0;
    }

    void Update() 
    {
        if (automaticFiring) HandleUpdate();
    }

    public void HandleUpdate()
    {
    
        timeSinceLastShot += Time.deltaTime;
        if (timeSinceLastShot >= delayBetweenShots)
        {
            switch (shootType)
            {
                case ShootType.Single:
                    Shoot(GetCurrentProjectileName());
                    break;
                case ShootType.Circle:
                    ShootCircle(GetCurrentProjectileName());
                    break;
            }
            OnShoot?.Invoke();
            timeSinceLastShot = 0;
        }
    }

    string GetCurrentProjectileName()
    {
        return projectileNames.ElementAt(currentProjectileIndex);
    }

    void Shoot(string name)
    {
        Entity projectile = ObjectPool.i.GetObject(GetCurrentProjectileName()).GetComponent<Entity>();
        projectile.SetActive(true);
        projectile.SetPosition(transform.position);
        projectile.SetMoveSpeed(projectileSpeed);
        projectile.SetRotation(transform.rotation.z);

        if (targetPlayer)
            projectile.SetMoveDirection(GameController.i.Player.transform.position - transform.position);
        else
            projectile.SetMoveDirection(shootDirection);
        
        MoveToNextProjectile();
    }

    void ShootCircle(string bullet="Bullet")
    {
        float tempAngle = 0;
        int i = 0;

        while (tempAngle < 360)
        {
            float bulDirX = transform.position.x + radius +  Mathf.Sin(((tempAngle) * Mathf.Deg2Rad));
            float bulDirY = transform.position.y + radius + Mathf.Cos(((tempAngle ) * Mathf.Deg2Rad));

            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            Vector2 bulDir = (bulMoveVector - transform.position).normalized;
            
            GameObject bul = ObjectPool.i.GetObject(bullet);
            Entity currentEntity = bul.GetComponent<Entity>();

            currentEntity.SetPosition(transform.position);
            currentEntity.SetActive(true);
            currentEntity.SetMoveSpeed(projectileSpeed);
            currentEntity.SetMoveDirection(Vector3.up);
            currentEntity.SetRotation(tempAngle);
            tempAngle += angleIncrement;
            i++;
        }
    }

    public void MoveToNextProjectile()
    {
        currentProjectileIndex++;
        if (currentProjectileIndex >= projectileNames.Count()) currentProjectileIndex = 0;
    }
}