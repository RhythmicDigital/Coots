using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum FacingDirection { Right, Left }

public enum CharacterState { Idle, Moving, Shooting, Grappling, Crouching, Jumping, Landing, Hurt, Dead }

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> idleSprites;
    [SerializeField] List<Sprite> jumpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> shootSprites;
    [SerializeField] List<Sprite> deadSprites;
    [SerializeField] List<Sprite> grapplingSprites;
    [SerializeField] List<Sprite> hurtSprites;
    [SerializeField] List<Sprite> crouchSprites;
    [SerializeField] List<Sprite> landSprites;

    SpriteAnimator walkRightAnim;
    SpriteAnimator jumpAnim;
    SpriteAnimator shootAnim;
    SpriteAnimator idleAnim;
    SpriteAnimator hurtAnim;
    SpriteAnimator crouchAnim;
    SpriteAnimator grapplingAnim;
    SpriteAnimator currentAnim;
    SpriteAnimator previousAnim;
    SpriteAnimator landAnim;
    SpriteAnimator deadAnim;

    public CharacterState State { get; private set; }
    CharacterState previousState;

    bool wasPreviouslyMoving;

    SpriteRenderer spriteRenderer;

    public bool IsMoving { get; set; }
    public bool IsPlaying { get; private set; }
    public FacingDirection FacingDirection { get; private set; }
    public SpriteAnimator ShootAnim => shootAnim;
    public SpriteAnimator HurtAnim => hurtAnim;
    public CharacterState PreviousState => previousState;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetPlaying(true);
        float frameRate = 0.16f;
        idleAnim = new SpriteAnimator(idleSprites, spriteRenderer, frameRate);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer, frameRate);
        shootAnim = new SpriteAnimator(shootSprites, spriteRenderer, frameRate, false);
        jumpAnim = new SpriteAnimator(jumpSprites, spriteRenderer, frameRate, false);
        hurtAnim = new SpriteAnimator(hurtSprites, spriteRenderer, frameRate);
        crouchAnim = new SpriteAnimator(crouchSprites, spriteRenderer, frameRate);
        grapplingAnim = new SpriteAnimator(grapplingSprites, spriteRenderer, frameRate);
        landAnim = new SpriteAnimator(landSprites, spriteRenderer, frameRate);
        deadAnim = new SpriteAnimator(landSprites, spriteRenderer, frameRate);

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
            case CharacterState.Landing:
                currentAnim = jumpAnim;
                break;
            case CharacterState.Dead:
                currentAnim = deadAnim;
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
        if (GameController.i.Player.GetComponent<CharacterController2D>().Grounded) SetState(CharacterState.Moving);
        else 
        {
            SetState(CharacterState.Grappling);
        }
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
        if (GameController.i.Player.GetComponent<CharacterController2D>().m_Moving)
            SetState(CharacterState.Moving);
        else
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
            case "landing":
                SetState(CharacterState.Landing);
                break;
        }
    }
}
