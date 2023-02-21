using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState { Playing, Paused }
public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;
    bool looping = true;
    int currentFrame;
    float timer;
    public AnimState State { get; private set; }
    public int CurrentFrame => currentFrame;
    public float FrameRate => frameRate;
    public List<Sprite> Frames => frames;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.08f)
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0f;
        spriteRenderer.sprite = frames[0];
    }

    public void HandleUpdate()
    {
        if (State == AnimState.Playing)
        {
            HandlePlayingUpdate();
        }
        else if (State == AnimState.Paused)
        {
            HandlePausedUpdate();
        }
    }
    void HandlePlayingUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            if (currentFrame + 1 >= frames.Count)
            {
                if (!looping)
                {
                    SetState(AnimState.Paused);
                }
            }
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
            
        }
    }
    void HandlePausedUpdate()
    {
    }
    public void SetLooping(bool looping)
    {
        this.looping = looping;
    }
    
    public void SetState(AnimState State)
    {
        this.State = State;
    }
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
    public void SetCurrentFrame(int frame)
    {
        currentFrame = frame;
    }
}