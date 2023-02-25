using UnityEngine;

public class GrabController : MonoBehaviour
{
    [SerializeField]
    private DistanceJoint2D _ropeJointPrefab;

    private LineRenderer _lr;

    private DistanceJoint2D _ropeJoint;
    private Transform _ropeJointTransform;

    private bool _connected;
    public bool Grabbing => _connected;

    private float _distance;

    private void Awake()
    {
        _ropeJoint = Instantiate(_ropeJointPrefab);
        _ropeJoint.enabled = false;
        _ropeJointTransform = _ropeJoint.transform;
        _lr = GetComponent<LineRenderer>();
    }

    public void ConnectToItem(RaycastHit2D hit)
    {
        if (_connected)
        {
            Disconnect();
        }
        _connected = true;

        _lr.positionCount = 2;
        _lr.enabled = true;

        _distance = Vector3.Distance(hit.point, transform.position);

        _ropeJointTransform.position = transform.position;
        _ropeJoint.connectedBody = hit.rigidbody;
        _ropeJoint.connectedAnchor = hit.rigidbody.transform.InverseTransformPoint(hit.point);
        _ropeJoint.distance = _distance;
        _ropeJoint.enabled = true;
    }

    public void Disconnect()
    {
        _ropeJoint.enabled = false;
        _lr.enabled = false;
        _connected = false;
    }

    public void HandleUpdate()
    {
        if (!_connected) return;
        var connectedTo = _ropeJoint.connectedBody.transform.TransformPoint(_ropeJoint.connectedAnchor);
        _lr.SetPosition(0, transform.position);
        _lr.SetPosition(1, connectedTo);
    }

    public void HandleFixedUpdate()
    {
        if (!_connected) return;
        _distance -= Time.fixedDeltaTime * 5;
        if (_distance <= 1)
        {
            Disconnect();

            AudioManager.i.PlaySfx(SfxId.Ungrapple);

            return;
        }

        _ropeJointTransform.position = transform.position;
        _ropeJoint.distance = _distance;
    }
}