using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private EnemyAnimations enemyAnimations;
    private bool hitObstacle = false;

    public bool IsAlive { get; private set; }

    new void OnEnable()
    {
        base.OnEnable();
        IsAlive = true;
        
        PlayerMechanics.OnPlayerAttack += ReduceHealth;
        GameController.Instance.OnRestartEnemy -= ResetEnemy;

        GameController.Instance.GameData.SetEnemyPos(this, gameObject.transform.position);
    }

    new void Start()
    {
        base.Start();
        enemyAnimations = GetComponent<EnemyAnimations>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Spike" && !hitObstacle)
        {
            ReduceHealth(this);
            hitObstacle = true;
        }
    }

    void Update()
    {
        if (!capsuleCollider.IsTouchingLayers(hazardsLayer))
            hitObstacle = false;
    }

    void OnDisable()
    {
        PlayerMechanics.OnPlayerAttack -= ReduceHealth;
        GameController.Instance.OnRestartEnemy += ResetEnemy;
    }

    private void ReduceHealth(EnemyHealth enemyHealth)
    {
        if (enemyHealth == this)
        {
            currentHealth--;
            if (currentHealth < 1)
            {
                rb.bodyType = RigidbodyType2D.Static;
                enemyAnimations?.PlayDeathAnim(false);
                Invoke("DisableObj", 3f);
                IsAlive = false;
            }
        }
    }

    void DisableObj() => gameObject.SetActive(false);

    void ResetEnemy()
    {
        transform.position = GameController.Instance.GameData.GetEnemyPos(this);
        enemyAnimations.ResetDeathAnim();
        rb.bodyType = RigidbodyType2D.Dynamic;
        gameObject.SetActive(true);
    }
}
