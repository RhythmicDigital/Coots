using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrappleObject : MonoBehaviour
{
    public enum GrappleInteraction
    {
        Connect,
        Pull,
        Interact,
    }

    [field: SerializeField] public GrappleInteraction Interaction { get; private set; }
    public UnityAction<RaycastHit2D> Interact;
}
