using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection { Right, Left }
public enum CharacterState { Idle, Moving, Shooting, Grappling, Crouching }
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> idleSprites;
    [SerializeField] List<Sprite> jumpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> shootSprites;

    SpriteAnimator walkRightAnim;
    SpriteAnimator jumpAnim;
    SpriteAnimator shootAnim;
    SpriteAnimator idleAnim;
    SpriteAnimator currentAnim;
    SpriteAnimator previousAnim;
    public CharacterState State { get; private set; }

    bool wasPreviouslyMoving;

    SpriteRenderer spriteRenderer;

    public bool IsMoving { get; set; }
    public bool IsPlaying { get; private set; }
    public FacingDirection FacingDirection { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init()
    {
        
        idleAnim = new SpriteAnimator(idleSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        shootAnim = new SpriteAnimator(shootSprites, spriteRenderer);
        jumpAnim = new SpriteAnimator(jumpSprites, spriteRenderer);

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

            if (IsMoving)
                currentAnim.HandleUpdate();
            else
                if (currentAnim.Frames.Count > 2)
                    spriteRenderer.sprite = currentAnim.Frames[1];
                else
                    spriteRenderer.sprite = null;

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
}
