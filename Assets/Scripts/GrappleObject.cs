using System;
using UnityEngine;

public class GrappleObject : MonoBehaviour
{
    public enum GrappleInteraction
    {
        Connect,
        Pull,
        Interact,
    }

    [field: SerializeField] public GrappleInteraction Interaction { get; private set; }
    public Action<RaycastHit2D> Interact;
}
