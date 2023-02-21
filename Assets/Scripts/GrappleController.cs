using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    [SerializeField]
    private LayerMask _ropeLayerMask;

    [SerializeField]
    private DistanceJoint2D _ropeJointPrefab;

    private DistanceJoint2D _ropeJoint;
    private GameObject _anchorPosition;
    private Transform _rbTransform;


    private Vector3 _connectedPoint;
    private float _maxDistance;

    private LineRenderer _lr;
    private readonly List<(Vector2 pos, Vector2 normal)> _ropePositions = new();
    private float _currentDistance;

    private bool _connected;

    public void ConnectToPoint(Vector3 point)
    {
        if (_connected)
        {
            Disconnect();
        }

        _connectedPoint = point;
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

    public void HandleUpdate()
    {
        if (!_connected) return;

        RemoveObsoleteRopePoints();
        CreateNewRopePoints();
        UpdateRopeVisuals();
    }

    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
            position => polyCollider.transform.TransformPoint(position));

        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
    }

    private void CreateNewRopePoints()
    {
        var lastRopePoint = _ropePositions.Count == 0 ? (Vector2)_connectedPoint : _ropePositions[^1].pos;
        var dir = lastRopePoint - (Vector2)transform.position;
        var playerToCurrentNextHit = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude - 0.1f, _ropeLayerMask);
        if (playerToCurrentNextHit)
        {
            var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
            if (colliderWithVertices != null)
            {
                var closestPointToHit =
                    GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

                var cc = Vector2.SignedAngle(closestPointToHit - lastRopePoint, dir) > 0;
                var normal = closestPointToHit - lastRopePoint;
                if (cc)
                {
                    (normal.x, normal.y) = (-normal.y, normal.x);
                }
                else
                {
                    (normal.x, normal.y) = (normal.y, -normal.x);
                }

                _ropePositions.Add((closestPointToHit, normal));
                _currentDistance += (closestPointToHit - lastRopePoint).magnitude;
                lastRopePoint = closestPointToHit;
            }
        }

        if (_ropePositions.Count == 0)
        {
            _currentDistance = 0;
        }

        _anchorPosition.transform.position = lastRopePoint;
        _ropeJoint.distance = _maxDistance - _currentDistance;
    }

    private void RemoveObsoleteRopePoints()
    {
        if (_ropePositions.Count >= 1)
        {
            for (var i = _ropePositions.Count - 1; i >= 0; i--)
            {
                var (pos, normal) = _ropePositions[i];
                if (Vector2.Dot(pos - (Vector2)transform.position, normal) >= 0)
                {
                    break;
                }

                var prevPosition = i == 0 ? (Vector2)_connectedPoint : _ropePositions[i - 1].pos;
                _currentDistance -= (prevPosition - pos).magnitude;
                _ropePositions.RemoveAt(i);
            }
        }
    }

    private void UpdateRopeVisuals()
    {
        _lr.positionCount = _ropePositions.Count + 2;
        _lr.SetPosition(0, _connectedPoint);
        _lr.SetPosition(_lr.positionCount - 1, transform.position);
        for (var i = 0; i < _ropePositions.Count; i++)
        {
            _lr.SetPosition(i + 1, _ropePositions[i].pos);
        }
    }
}