using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Boss : MonoBehaviour
{
    [SerializeField] ProjectileShooter shooter;
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float moveSpeed = 10;

    public void MoveToWaypoint(int point)
    {
        transform.DOMove(waypoints.ElementAt(point).position, moveSpeed);
        MoveToNextProjectile();
    }

    public void HandleUpdate()
    {
        shooter.HandleUpdate();
    }

    public void MoveToNextProjectile()
    {
        shooter.MoveToNextProjectile();
    }
}