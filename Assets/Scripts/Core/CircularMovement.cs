using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    [SerializeField] GrappleController grappleController;
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] float angularSpeed = 1f;
    
    private Vector2 fixedPoint;
    int direction = 1;
    
    void Start ()
    {
        fixedPoint = transform.position;
    }
    
    void Update ()
    {
        if (Input.GetButton("Horizontal") && (grappleController.Grappling))
        {
            int direction = 1;
            fixedPoint = grappleController.ConnectedTo.position;

            if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
                direction = 1;

            else if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
                direction = -1;
        }
    }
    void FixedUpdate() 
    {
        rigidbody.AddForce(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * angularSpeed);
    }
}