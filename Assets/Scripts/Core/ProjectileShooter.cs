using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] bool targetPlayer;
    [SerializeField] float delayBetweenShots = 1;
    [SerializeField] float projectileSpeed = 10;
    [SerializeField] Vector3 shootDirection;
    [SerializeField] string projectileName;

    float timeSinceLastShot = 0;

    void Update() 
    {
        timeSinceLastShot += Time.deltaTime;
        if (timeSinceLastShot >= delayBetweenShots)
        {
            Shoot(projectileName);
            timeSinceLastShot = 0;
        }
    }

    void Shoot(string name)
    {
        Entity projectile = ObjectPool.i.GetObject(name).GetComponent<Entity>();
        projectile.SetActive(true);
        projectile.SetPosition(transform.position);
        projectile.SetRotation(transform.rotation);
        projectile.SetMoveSpeed(projectileSpeed);

        if (targetPlayer)
            projectile.SetMoveDirection(GameController.i.Player.transform.position);
        else
            projectile.SetMoveDirection(shootDirection);
    }
}