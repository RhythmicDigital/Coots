using UnityEngine;

class InputController : MonoBehaviour
{
    [SerializeField]
    private float _maxGrappleDistance;

    [SerializeField]
    private CharacterController2D _characterController;

    [SerializeField]
    private Transform _aimParent;

    [SerializeField]
    private LayerMask _canBeGrappled;

    [SerializeField]
    private LayerMask _ground;

    [SerializeField]
    private GrappleController _grappleController;

    private Camera _camera;

    private float _xDir;
    private bool _crouch, _jump;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        MoveUpdate();
        GrapplUpdate();
    }

    private void MoveUpdate()
    {
        _xDir = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        _crouch = y < 0 || Input.GetKey(KeyCode.LeftShift);
        _jump = y > 0 || Input.GetKey(KeyCode.Space);
    }

    private void GrapplUpdate()
    {
        var mousePos = (Vector3)Input.mousePosition;
        mousePos.z = transform.position.z - _camera.transform.position.z;
        var mouseWorldPos = _camera.ScreenToWorldPoint(mousePos);

        var aimDirection = mouseWorldPos - _aimParent.position;
        _aimParent.rotation = Quaternion.LookRotation(Vector3.forward, aimDirection);


        if (Input.GetButtonUp("Fire1"))
        {
            _grappleController.Disconnect();
            return;
        }
        if (!Input.GetButtonDown("Fire1")) return;

        var hit = Physics2D.Raycast(_aimParent.position, aimDirection, _maxGrappleDistance, _canBeGrappled | _ground);
        if (!hit) return;

        _grappleController.ConnectToPoint(hit.point);
    }

    private void FixedUpdate()
    {
        _characterController.Move(_xDir, _crouch, _jump);
    }
}
