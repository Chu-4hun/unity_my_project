using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public float AnimationSnoothTime = 5;
    private Vector2 CurrentMoveAnimationBlend = new Vector2();
    private Vector2 CurrentBlendVelocity;

    private Animator _Animator;

    public void Awake()
    {
        _Animator = GetComponent<Animator>();
    }

    public void SetAnimationMove(Vector2 InputVector)
    {
        CurrentMoveAnimationBlend = Vector2.SmoothDamp(CurrentMoveAnimationBlend, InputVector,
                   ref CurrentBlendVelocity, AnimationSnoothTime * Time.fixedDeltaTime);
        _Animator.SetFloat("X", CurrentMoveAnimationBlend.x);
        _Animator.SetFloat("Z", CurrentMoveAnimationBlend.y);
    }
 
}
