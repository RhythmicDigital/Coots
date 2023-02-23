using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField]
    private Collider2D _platformCollider;

    private void FixedUpdate()
    {
        if (_platformCollider.enabled)
        {
            _platformCollider.enabled = CharacterController2D.FixedPosition.y > transform.position.y;
        }
        else
        {
            _platformCollider.enabled = CharacterController2D.FixedPosition.y > transform.TransformPoint(_platformCollider.offset).y;
        }
    }
}
