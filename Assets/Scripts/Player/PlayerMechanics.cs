using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class PlayerMechanics : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] float rayDistance = 1f;

    [SerializeField] LayerMask platformsLayer;
    [SerializeField] Transform point; 

    [SerializeField] private float jumpDuration = 0.06f;

    Rigidbody2D playerRb;
    InputManager inputManager;
    PlayerAnimations playerAnimations;
    CapsuleCollider2D capsuleCollider;

    public delegate void AttackEventHandler(EnemyHealth enemyHealth);

    public static event AttackEventHandler OnPlayerAttack;

    float jumpTimer = 0;
    bool isInGround = true;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        inputManager = GetComponent<InputManager>();
        playerAnimations = GetComponent<PlayerAnimations>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void FixedUpdate()
    {
        if (!inputManager) return;
        
        ProcessMovement();
        ProcessJump();
        ProcessAttack();
    }

    private void ProcessMovement()
    {
        playerRb.velocity = new Vector2(inputManager.HorizontalAxis * moveSpeed, playerRb.velocity.y);
        playerAnimations.PlayRunAnim(inputManager.HorizontalAxis != 0);
        if (inputManager.HorizontalAxis != 0)
            transform.localScale = new Vector3(-Mathf.Sign(inputManager.HorizontalAxis), transform.localScale.y, transform.localScale.z);
    }

    private void ProcessJump()
    {
        if (inputManager.IsJumpPressed)
        {
            if (isInGround)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
                isInGround = false;
                playerAnimations.PlayJumpAnim();
            }
            else
                if (jumpTimer < jumpDuration)
                    playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
        }

        if (IsGrounded())
        {
            isInGround = true;
            jumpTimer = 0;
        }
        else
        {
            jumpTimer += Time.deltaTime;
        }
    }

    private void ProcessAttack()
    {
        if (inputManager.IsAttackClicked)
            playerAnimations.PlayAttackAnim();
    }

    // Binded to Attack.anim event
    public void OnAttackAnimComplete()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(point.position, new Vector2(-transform.localScale.x, 0), rayDistance);
        if (hitInfo && hitInfo.transform.tag == "Enemy")
        {
            var enemyHealth = hitInfo.transform?.gameObject?.GetComponent<EnemyHealth>();
            OnPlayerAttack?.Invoke(enemyHealth);
        }
    }

    private bool IsGrounded() => capsuleCollider.IsTouchingLayers(platformsLayer);
}

