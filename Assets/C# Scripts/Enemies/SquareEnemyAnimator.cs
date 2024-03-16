using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareEnemyAnimator : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.Play("EnemyFullShieldSquare");
    }
}
