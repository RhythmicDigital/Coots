using UnityEngine;

class InputController : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D _characterController;

    private float _xDir;
    private bool _crouch, _jump;

    private void Reset()
    {
        _characterController = GetComponent<CharacterController2D>();
    }

    private void Update()
    {
        _xDir = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        _crouch = y < 0 || Input.GetKey(KeyCode.LeftShift);
        _jump = y > 0 || Input.GetKey(KeyCode.Space);
    }

    private void FixedUpdate()
    {
        _characterController.Move(_xDir, _crouch, _jump);
    }
}
