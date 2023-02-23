using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum CameraState { Fixed, Following }
public class CameraController : MonoBehaviour
{
    [SerializeField] List<Transform> targets;
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothTime = .5f;
    [SerializeField] float minZoom = 40f;
    [SerializeField] float maxZoom = 10f;
    [SerializeField] float zoomLimiter = 50f;  
    [SerializeField] float shakeAmount = 0.1f;
    [SerializeField] float shakeDuration = 1;
    [SerializeField] int shakeVibrato = 1;
    [SerializeField] int cameraMoveSpeed = 10;
    [SerializeField] Transform player;
    [SerializeField] float depth = -10;
    [SerializeField] bool lockYAxis;
    [SerializeField] bool lockXAxis;
    
    Vector3 velocity;
    Camera cam;
    CameraState state;

    public static CameraController i;

    public CameraState State => state;

    void Awake() 
    {
        i = this;
    }
    
    void Start() 
    {
        Init();
    }

    public IEnumerator MoveCamera()
    {
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, depth);

        if (lockXAxis)
            targetPos.x = transform.position.x;

        if (lockYAxis)
            targetPos.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, targetPos, cameraMoveSpeed * Time.deltaTime);
        yield return null;
    }

    public void SetState(CameraState state) 
    {
        this.state = state;
        if (state == CameraState.Fixed)
        {
            cam.fieldOfView = 45;
        }
    }

    public void Init()
    {
        cam = GetComponent<Camera>();
        state = CameraState.Following;
    }

    public void SetPosition(Vector3 position)
    {
        Vector3 targetPos = new Vector3(position.x, position.y, depth);
        transform.position = targetPos;
    }

    public IEnumerator SetPositionAsync(Vector3 position)
    {
        Vector3 targetPos = position;
        transform.position = Vector3.Lerp(transform.position, targetPos, cameraMoveSpeed * Time.deltaTime);
        yield return null;
    }

    public void HandleUpdate()
    {
        if (State == CameraState.Following)
        {
            if (targets.Count == 0) return;
            
            else if (targets.Count == 1) StartCoroutine(MoveCamera());
            
            else if (targets.Count > 1)
            {
                Move();
                Zoom();
            }
        }
    }

    public void ShakeCamera()
    {
        transform.DOShakePosition(shakeDuration, shakeAmount, shakeVibrato, 0.5f);
    }

    public void AddTarget(Transform target)
    {
        targets.Add(target);
    }

    public void RemoveTarget(Transform target)
    {
        targets.Remove(target);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }
    
    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    } 

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
