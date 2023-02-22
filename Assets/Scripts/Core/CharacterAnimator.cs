using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection { Right, Left }
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> idleSprites;
    [SerializeField] List<Sprite> jumpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> shootSprites;
     [SerializeField] List<Sprite> grapplingSprites;
    [SerializeField] List<Sprite> hurtSprites;
    [SerializeField] List<Sprite> crouchSprites;

    SpriteAnimator walkRightAnim;
    SpriteAnimator jumpAnim;
    SpriteAnimator shootAnim;
    SpriteAnimator idleAnim;
    SpriteAnimator hurtAnim;
    SpriteAnimator crouchAnim;
    SpriteAnimator grapplingAnim;
    SpriteAnimator currentAnim;
    SpriteAnimator previousAnim;

    public CharacterState State { get; private set; }
    CharacterState previousState;

    bool wasPreviouslyMoving;

    SpriteRenderer spriteRenderer;

    public bool IsMoving { get; set; }
    public bool IsPlaying { get; private set; }
    public FacingDirection FacingDirection { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start() 
    {
        Init();
    }

    public void Init()
    {
        SetPlaying(true);
        float frameRate = GlobalSettings.i.FrameRate;
        idleAnim = new SpriteAnimator(idleSprites, spriteRenderer, frameRate);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer, frameRate);
        shootAnim = new SpriteAnimator(shootSprites, spriteRenderer, frameRate, false);
        jumpAnim = new SpriteAnimator(jumpSprites, spriteRenderer, frameRate, false);
        hurtAnim = new SpriteAnimator(hurtSprites, spriteRenderer, frameRate);
        crouchAnim = new SpriteAnimator(crouchSprites, spriteRenderer, frameRate);
        grapplingAnim = new SpriteAnimator(grapplingSprites, spriteRenderer, frameRate);

        previousState = State;

        if (previousAnim != null)
            currentAnim = previousAnim;
        else
            currentAnim = idleAnim;
    }
    
    private void Update()
    {
        
        if (IsPlaying)
        {
            var prevAnim = currentAnim;

            if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
                currentAnim.Start();

            currentAnim.HandleUpdate();                

            previousAnim = currentAnim;
            wasPreviouslyMoving = IsMoving;
        }
        
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        FacingDirection = dir;
    }

    public void SetPlaying(bool playing) 
    {
        IsPlaying = playing;
    }

    public void SetState(CharacterState state)
    {
        previousState = State;
        State = state;

        switch (state)
        {
            case CharacterState.Idle:
                currentAnim = idleAnim;
                break;
            case CharacterState.Grappling:
                currentAnim = grapplingAnim;
                break;
            case CharacterState.Shooting:
                currentAnim = shootAnim;
                break;
            case CharacterState.Moving:
                currentAnim = walkRightAnim;
                break;
            case CharacterState.Crouching:
                currentAnim = crouchAnim;
                break;
            case CharacterState.Jumping:
                currentAnim = jumpAnim;
                break;
        }
        currentAnim.Start();
    }
    
    public void OnCrouch(bool crouch)
    {
        if (crouch)
            SetState(CharacterState.Crouching);
        else
            SetState(previousState);
    }

    public void OnMove()
    {
        SetState(CharacterState.Moving);
    }

    public void OnJump()
    {
        SetState(CharacterState.Jumping);
    }

    public void OnIdle()
    {
        SetState(CharacterState.Idle);
    }

    public void OnLand() 
    {
        Debug.Log("Landed");
        SetState(CharacterState.Idle);
    }

    public void SetState(string state)
    {
        switch (state.ToLower())
        {
            case "idle":
                SetState(CharacterState.Idle);
                break;
            case "grappling":
                SetState(CharacterState.Grappling);
                break;
            case "shooting":
                SetState(CharacterState.Shooting);
                break;
            case "moving":
                SetState(CharacterState.Moving);
                break;
            case "crouching":
                SetState(CharacterState.Crouching);
                break;
            case "jumping":
                SetState(CharacterState.Jumping);
                break;
        }
    }
}
