using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GrappleState { Idle, Jumping, HasInstantPull, UsedInstantPull }
public class GrappleController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidbody;

    [SerializeField]
    private LayerMask _ropeLayerMask;

    [SerializeField]
    private DistanceJoint2D _ropeJointPrefab;

    [SerializeField]
    private float _angularVelocity = 100;

    private DistanceJoint2D _ropeJoint;
    private GameObject _anchorPosition;
    private Transform _rbTransform;

    private Transform _connectedTo;
    private Vector3 _connectedToOffset, _startPosition;

    private float _maxDistance;

    private LineRenderer _lr;
    private readonly List<(Vector2 pos, bool clockwise)> _ropePositions = new();
    private float _currentDistance, _startDistance;

    private bool _connected;

    public GrappleState State { get; private set; }

    public bool Grappling => _connected;
    public Transform ConnectedTo => _connectedTo;
    public float Distance => _currentDistance;

    public void ConnectToPoint(Transform obj, Vector3 point)
    {
        if (_connected)
        {
            Disconnect();
        }

        _startPosition = point;
        _connectedTo = obj;
        _connectedToOffset = obj.InverseTransformPoint(point);
        _startDistance = 0;

        _maxDistance = Vector3.Distance(point, transform.position);

        _anchorPosition.transform.position = point;
        _ropeJoint.distance = _maxDistance;
        _ropeJoint.connectedAnchor = _rbTransform.InverseTransformPoint(transform.position);
        _ropeJoint.enabled = true;

        _lr.positionCount = 0;
        _lr.enabled = true;

        _ropePositions.Clear();

        _connected = true;
    }

    public void Disconnect()
    {
        _ropeJoint.enabled = false;
        _lr.enabled = false;
        _connected = false;

        AudioManager.i.PlaySfx(SfxId.Ungrapple);

        Vector2 ungrappleForce = Vector2.zero;
        if (_connected) ungrappleForce = (_anchorPosition.transform.position - _rigidbody.transform.position).normalized * GlobalSettings.i.UngrappleFlySpeed;
        _rigidbody.AddForce(ungrappleForce);
    }

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _ropeJoint = Instantiate(_ropeJointPrefab);
        _ropeJoint.enabled = false;
        _ropeJoint.connectedBody = GetComponentInParent<Rigidbody2D>();
        _anchorPosition = _ropeJoint.gameObject;
        _rbTransform = _ropeJoint.connectedBody.transform;
    }

    public void HandleFixedUpdate()
    {
        if (!_connected) return;
        var entity = _connectedTo != null ? _connectedTo.GetComponent<Entity>() : null;
        if (_connectedTo == null || !_connectedTo.gameObject.activeInHierarchy || (entity && entity.State == EntityState.Inactive))
        {
            // If the object we're connected to disappears we need to disconnect
            Disconnect();
            return;
        }

        if (_maxDistance - _currentDistance > 1 && State == GrappleState.HasInstantPull)
        {
            _maxDistance = Mathf.MoveTowards(_maxDistance, _currentDistance + 1, Time.deltaTime * GlobalSettings.i.GrappleSpeed);
            _rigidbody.AddForce((_connectedTo.position - transform.position).normalized * Time.deltaTime * GlobalSettings.i.GrappleSpeed);
        }

        var connectedToPos = _connectedTo.TransformPoint(_connectedToOffset);
        var startMoved = connectedToPos != _startPosition;
        _startPosition = connectedToPos;

        RemoveObsoleteRopePoints();
        if (startMoved && _ropePositions.Count > 0)
        {
            RemoveObsoleteRopePoints(true);

            _currentDistance -= _startDistance;
            // Should always be larger than 0
            if (_ropePositions.Count > 0)
            {
                _startDistance = Vector2.Distance(_ropePositions[0].pos, _startPosition);
                _currentDistance += _startDistance;
            }

            CreateNewRopePoints(true);
        }

        CreateNewRopePoints();
    }

    public void HandleUpdate()
    {
        UpdateRopeVisuals();
    }

    public void Move(float x)
    {
        var dir = (_startPosition - transform.position).normalized;
        (dir.x, dir.y) = (dir.y, -dir.x);

        _rigidbody.AddForce(dir * _angularVelocity * x);
    }

    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        if (polyCollider.points.Length == 0) return Vector2.zero;

        return polyCollider.points
            .Select(point => polyCollider.transform.TransformPoint(point))
            .OrderBy(point => Vector2.SqrMagnitude((Vector2)point - hit.point))
            .First();
    }
    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, BoxCollider2D boxCollider)
    {
        var hitLocal = (Vector2)boxCollider.transform.InverseTransformPoint(hit.point);
        var compareTo = boxCollider.size / 2;

        var closestPoint = Vector2.zero;
        var closestDistance = float.MaxValue;
        for (var i = 0; i < 4; i++)
        {
            var distance = Vector2.SqrMagnitude(hitLocal - (compareTo + boxCollider.offset));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = compareTo;
            }
            compareTo = new Vector2(-compareTo.y, compareTo.x);
        }

        return boxCollider.transform.TransformPoint(closestPoint);
    }

    private void CreateNewRopePoints(bool inverse = false)
    {
        if (inverse && _ropePositions.Count == 0)
        {
            // Handled by regular check
            return;
        }

        Vector2 yourEnd;
        Vector2 lastRopePoint;
        if (inverse)
        {
            yourEnd = _ropePositions.Count == 0 ? transform.position : _ropePositions[0].pos;
            // last rope point is always start position when inserting to beginning of list
            lastRopePoint = _startPosition;
        }
        else
        {
            yourEnd = transform.position;
            lastRopePoint = _ropePositions.Count == 0 ? _startPosition : _ropePositions[^1].pos;
        }

        var dir = lastRopePoint - yourEnd;
        var dirNormalized = dir.normalized;

        var hit = inverse ?
            Physics2D.Raycast(lastRopePoint + dirNormalized * -0.05f, -dirNormalized, dir.magnitude - 0.1f, _ropeLayerMask) :
        Physics2D.Raycast(yourEnd + dirNormalized * 0.05f, dirNormalized, dir.magnitude - 0.1f, _ropeLayerMask);

        Vector2 closestPointToHit = Vector2.zero;
        var foundPoint = false;

        if (hit)
        {
            foundPoint = true;
            if (hit.collider is PolygonCollider2D colliderWithVertices)
            {
                closestPointToHit =
                    GetClosestColliderPointFromRaycastHit(hit, colliderWithVertices);
            }
            else if (hit.collider is BoxCollider2D boxCollider)
            {
                closestPointToHit =
                    GetClosestColliderPointFromRaycastHit(hit, boxCollider);
            }
            else
            {
                foundPoint = false;
            }

            foundPoint &= closestPointToHit != lastRopePoint && closestPointToHit != yourEnd;
        }


        if (foundPoint)
        {
            var clockwise = Vector2.SignedAngle(closestPointToHit - lastRopePoint, dir) <= 0;

            var value = (closestPointToHit, clockwise);
            if (inverse)
            {
                _ropePositions.Insert(0, value);
                _currentDistance -= _startDistance;
                _startDistance -= hit.distance;
                _currentDistance += Vector2.Distance(_ropePositions[1].pos, _ropePositions[0].pos);
            }
            else
            {
                _ropePositions.Add(value);
            }

            _currentDistance += (closestPointToHit - lastRopePoint).magnitude;
            lastRopePoint = closestPointToHit;

            if (_ropePositions.Count == 1)
            {
                _startDistance = _currentDistance;
            }
        }

        if (_ropePositions.Count == 0)
        {
            _currentDistance = 0;
        }
        else if (_ropePositions.Count == 1)
        {
            _currentDistance = Vector2.Distance(_ropePositions[0].pos, _startPosition);
        }

        _anchorPosition.transform.position = lastRopePoint;
        _ropeJoint.distance = _maxDistance - _currentDistance;
    }

    private void RemoveObsoleteRopePoints(bool inverse = false)
    {
        if (_ropePositions.Count < 1)
        {
            return;
        }
        var yourEnd = inverse ? (Vector2)_startPosition : (Vector2)transform.position;

        bool CheckPoint(int i, Vector2 prevPosition)
        {
            var (pos, clockwise) = _ropePositions[i];

            var normal = pos - prevPosition;
            if (clockwise != inverse)
            {
                (normal.x, normal.y) = (normal.y, -normal.x);
            }
            else
            {
                (normal.x, normal.y) = (-normal.y, normal.x);
            }

            if (Vector2.Dot(pos - yourEnd, normal) >= 0)
            {
                return true;
            }

            _currentDistance -= (prevPosition - pos).magnitude;
            return false;
        }

        var removeFrom = 0;
        var removeTo = _ropePositions.Count;
        if (inverse)
        {
            removeTo = removeFrom;
            for (var i = 0; i < _ropePositions.Count; i++)
            {
                var prevPosition = i == _ropePositions.Count - 1 ? (Vector2)transform.position : _ropePositions[i + 1].pos;
                var end = CheckPoint(i, prevPosition);

                if (end) break;
                removeTo = i + 1;
            }
        }
        else
        {
            removeFrom = removeTo;
            for (var i = _ropePositions.Count - 1; i >= 0; i--)
            {
                var prevPosition = i == 0 ? (Vector2)_startPosition : _ropePositions[i - 1].pos;
                var end = CheckPoint(i, prevPosition);

                if (end) break;
                removeFrom = i;
            }
        }

        if (removeFrom != removeTo)
        {
            _ropePositions.RemoveRange(removeFrom, removeTo - removeFrom);
        }
    }

    private void UpdateRopeVisuals()
    {
        _lr.positionCount = _ropePositions.Count + 2;
        _lr.SetPosition(0, _startPosition);
        _lr.SetPosition(_lr.positionCount - 1, transform.position);
        for (var i = 0; i < _ropePositions.Count; i++)
        {
            _lr.SetPosition(i + 1, _ropePositions[i].pos);
        }
    }

    public void SetState(GrappleState state)
    {
        State = state;
    }
}