using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Boss : MonoBehaviour
{
    [SerializeField] CharacterAnimator animator;
    [SerializeField] List<ProjectileShooter> shooters;
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float moveSpeed = 10;

    int currentShooter = 0; 

    void Start() 
    {
        animator.ShootAnim.OnEnd += () => {
            animator.SetState(CharacterState.Idle);
        };
        
        foreach(ProjectileShooter shooter in shooters)
        {
            shooter.OnShoot += () => {
                AudioManager.i.PlaySfx(SfxId.BossShoot);
                animator.SetState(CharacterState.Shooting);
            };
        }
    }

    public void MoveToWaypoint(int point)
    {
        transform.DOMove(waypoints.ElementAt(point).position, moveSpeed);
        MoveToNextProjectile();
    }

    public void HandleUpdate()
    {
        GetCurrentShooter().HandleUpdate();
    }

    ProjectileShooter GetCurrentShooter()
    {
        return shooters.ElementAt(currentShooter);
    }
    public void MoveToNextProjectile()
    {
        GetCurrentShooter().MoveToNextProjectile();
    }
}