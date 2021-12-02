using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Create SpriteAnimationParams", fileName = "SpriteAnimationParams", order = 0)]
public class SpriteAnimationParams : ScriptableObject
{
    
}

public class SpriteAnimator
{
    readonly SpriteRenderer _renderer;

    public SpriteAnimator(SpriteRenderer rend)
    {
        _renderer = rend;
    }
}

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimatorTester : MonoBehaviour
{
    SpriteAnimator _anim;
    SpriteAnimationParams _params;

    void Awake()
    {
        _anim = new SpriteAnimator(GetComponent<SpriteRenderer>());
    }
}
