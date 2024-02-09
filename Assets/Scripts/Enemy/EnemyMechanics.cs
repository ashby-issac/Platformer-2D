using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMechanics : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform point;

    [SerializeField] float attackDist = 1.5f;
    [SerializeField] float rayDistance = 1f;

    private EnemyAnimations enemyAnimations;
    private EnemyHealth enemyHealth;

    private bool isOneShotComplete;
    private float distToPlayer;
    private float oneShotTimer = 1f;
    private Vector3 targetPos = default;

    public delegate void EnemyAttackHandler(PlayerHealth playerHealth);

    public static event EnemyAttackHandler OnEnemyAttack;

    public float DistToPlayer => Vector3.Distance(transform.position, playerTransform.position);

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAnimations = GetComponent<EnemyAnimations>();
    }

    void Update()
    {
        if (!enemyHealth.IsAlive || !PlayerHealth.IsAlive) return;

        distToPlayer = DistToPlayer;
        var yDist = Mathf.Abs(playerTransform.position.y - transform.position.y);
        if (distToPlayer < 10f && yDist < 0.5f)
        {
            AttackPlayer();
        }
        else
        {
            enemyAnimations.PlayMoveAnim(false);
        }
    }


    private void AttackPlayer()
    {
        var dirToPlayer = playerTransform.position - transform.position;
        transform.localScale = new Vector2(-Mathf.Sign(dirToPlayer.x), transform.localScale.y);
        targetPos = dirToPlayer.x < 0 ? playerTransform.position + new Vector3(attackDist, 0, 0) : playerTransform.position - new Vector3(attackDist, 0, 0);

        if (distToPlayer < attackDist)
        {
            if (!isOneShotComplete)
            {
                enemyAnimations.PlayMoveAnim(false);
                enemyAnimations.PlayAttackAnim();
                isOneShotComplete = true;
                oneShotTimer = 1.5f;
            }
        }
        else
        {
            enemyAnimations.PlayMoveAnim(true);
        }

        if (oneShotTimer > 0f)
        {
            oneShotTimer -= Time.deltaTime;
        }
        else
        {
            isOneShotComplete = false;
        }

        transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime);
    }

    // Binded to Attack.anim event
    public void OnAttackAnimComplete()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(point.position, new Vector2(-transform.localScale.x, 0), rayDistance);
        if (hitInfo && hitInfo.transform.tag == "Player")
        {
            var playerHealth = hitInfo.transform?.gameObject?.GetComponent<PlayerHealth>();
            if (PlayerHealth.IsAlive)
                OnEnemyAttack?.Invoke(playerHealth);
        }
    }
}
