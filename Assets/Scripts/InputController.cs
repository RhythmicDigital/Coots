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
    [SerializeField]
    private GrabController _grabController;

    private Camera _camera;

    private float _xDir;
    private bool _crouch, _jump;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void HandleUpdate()
    {
        MoveUpdate();
        GrappleUpdate();
    }

    private void MoveUpdate()
    {
        _xDir = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        _crouch = y < 0 || Input.GetKey(KeyCode.LeftShift);
        _jump = y > 0 || Input.GetKey(KeyCode.Space);
    }

    private void GrappleUpdate()
    {
        var mousePos = (Vector3)Input.mousePosition;
        mousePos.z = transform.position.z - _camera.transform.position.z;
        var mouseWorldPos = _camera.ScreenToWorldPoint(mousePos);

        var aimDirection = mouseWorldPos - _aimParent.position;
        _aimParent.rotation = Quaternion.LookRotation(Vector3.forward, aimDirection);


        if (Input.GetButtonUp("Fire1"))
        {
            _grappleController.Disconnect();
            _grabController.Disconnect();

            if (_grappleController.State == GrappleState.HasInstantPull)
            {
                _grappleController.SetState(GrappleState.UsedInstantPull);
            }

            return;
        }
        if (_grappleController.Grappling || _grabController.Grabbing) return;

        if (!Input.GetButtonDown("Fire1")) return;

        var hit = Physics2D.Raycast(_aimParent.position, aimDirection, _maxGrappleDistance, _canBeGrappled);
        if (!hit) return;
        var hitGo = hit.rigidbody ? hit.rigidbody.gameObject : hit.collider.gameObject;
        var grappleObj = hitGo.GetComponent<GrappleObject>();

        if (grappleObj == null || grappleObj.Interaction == GrappleObject.GrappleInteraction.Connect)
        {
            if (_characterController.Grounded == false)
            {   
                if (_grappleController.State == GrappleState.Jumping || _grappleController.State == GrappleState.UsedInstantPull
                    && CameraController.i.CheckVisibility(hitGo))
                {
                    if (_grappleController.State == GrappleState.Jumping)
                        _grappleController.SetState(GrappleState.HasInstantPull);

                    _grappleController.ConnectToPoint(hit.transform, hit.point);

                    GameController.i.PlayerAnimator.SetState(CharacterState.Shooting);
                    AudioManager.i.PlaySfx(SfxId.Shoot);
                }
            }
        }
        else if (grappleObj.Interaction == GrappleObject.GrappleInteraction.Pull && CameraController.i.CheckVisibility(grappleObj))
        {
            _grabController.ConnectToItem(hit);

            GameController.i.PlayerAnimator.SetState(CharacterState.Shooting);
            AudioManager.i.PlaySfx(SfxId.Shoot);
        }
        else if (CameraController.i.CheckVisibility(grappleObj))
        {
            grappleObj.Interact?.Invoke(hit);
        }
    }

    public void HandleFixedUpdate()
    {
        if (_grappleController.Grappling) return;
        _characterController.Move(_xDir, _crouch, _jump);
    }
}
