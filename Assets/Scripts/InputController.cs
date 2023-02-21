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

        var hit = Physics2D.Raycast(_aimParent.position, aimDirection, _maxGrappleDistance, _canBeGrappled);
        if (!hit) return;
        var hitGo = hit.rigidbody ? hit.rigidbody.gameObject : hit.collider.gameObject;
        var grappleObj = hitGo.GetComponent<GrappleObject>();

        if (grappleObj == null || grappleObj.Interaction == GrappleObject.GrappleInteraction.Connect)
        {
            _grappleController.ConnectToPoint(hit.point);
        }
        else if (grappleObj.Interaction == GrappleObject.GrappleInteraction.Pull)
        {
            hit.rigidbody.AddForceAtPosition(-aimDirection.normalized * 5, hit.point, ForceMode2D.Impulse);
        }
        else
        {
            grappleObj.Interact?.Invoke(hit);
        }
    }

    private void FixedUpdate()
    {
        _characterController.Move(_xDir, _crouch, _jump);
    }
}
