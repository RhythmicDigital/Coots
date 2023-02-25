using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public enum EntityState { Active, Inactive }

public class Entity : MonoBehaviour
{
    [SerializeField] float hitRadius;
    [SerializeField] LayerMask hittableLayer;
    [SerializeField] bool isBouncy;
    
    int maxBounces = 10;
    int numBounces;

    public EntityState State { get; private set; } = EntityState.Active;
    public bool IsMoving { get; private set; } = true;
    public Vector3 MoveDirection { get; private set; }
    public float MoveSpeed { get; private set; }

    void Start() 
    {
        SetActive(true);

        if (isBouncy) numBounces = maxBounces;
    }

    public void Update()
    {
        if (State == EntityState.Active)
        {
            HandleActiveUpdate();
        }
    }

    void HandleActiveUpdate()
    {
        if (IsMoving)
            MoveEntity();
        
        HandleCollisions();
    }

    void HandleCollisions()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, hitRadius, hittableLayer);
        bool collided = hitColliders.Length > 0;

        if (collided)
        {
            if (!isBouncy)
                SetActive(false);
            else
            {
                List<RaycastHit2D> hits = new List<RaycastHit2D>();
                ContactFilter2D filter = new ContactFilter2D {useTriggers = true};
                int hit = Physics2D.Raycast(transform.position, new Vector2(MoveDirection.x, MoveDirection.y), filter, hits);

                if ( hit != 0 )
                {
                    if (numBounces > 0)
                    {
                        var contact = hits.ElementAt(0);
                        var dot = Vector3.Dot(contact.normal, -MoveDirection);
                        dot *= 2;
                        var reflection = contact.normal * dot;
                        reflection = reflection + new Vector2(MoveDirection.x, MoveDirection.y);
                        SetMoveDirection(reflection);
                        numBounces -= 1;
                    }
                    else 
                        SetActive(false);
                }
            }
        }
    }

    public void SetState(EntityState state)
    {
        State = state;
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            if (isBouncy) numBounces = maxBounces;
            gameObject.SetActive(active);
            State = EntityState.Active;
        }

        else
        {
            transform.position = ObjectPool.i.transform.position;
            State = EntityState.Inactive;
        }
        IsMoving = active;
        GetComponent<Entity>().enabled = active;
        GetComponent<SpriteRenderer>().enabled = active;
        GetComponent<Collider2D>().enabled = active;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        MoveSpeed = moveSpeed;
    }

    public void SetMoveDirection(Vector3 moveDirection)
    {
        MoveDirection = moveDirection;
    }

    public void SetIsMoving(bool isMoving)
    {
        IsMoving = isMoving;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetRotation(float rotation)
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, rotation);
    }

    void MoveEntity()
    {
        transform.Translate(MoveDirection.normalized * MoveSpeed * Time.deltaTime);
    }
}