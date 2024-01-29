using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private LayerMask heartsLayer;

    private PlayerAnimations playerAnimations;
    private InputManager inputManager;
    private bool hitObstacle = false;

    public static bool IsAlive = false;

    public delegate void PlayerDamagedHandler(float currentHealth, float maxHealth);
    public static event PlayerDamagedHandler OnPlayerDamaged;

    public delegate void PlayerPickup();
    public static event PlayerPickup OnPlayerPickup;

    new void OnEnable()
    {
        base.OnEnable();
        IsAlive = true;

        GameManager.Instance.OnGameRestart -= PlayerReset;
        transform.position = GameManager.Instance.CurrentLevelPlayerPos;

        UIManager.Instance.SetHealthIndex(currentHealth);
        EnemyMechanics.OnEnemyAttack += ReduceHealth;

        OnPlayerDamaged += UIManager.Instance.ReduceHealthUI;
        OnPlayerPickup += UIManager.Instance.AddHealthUI;

        GameManager.Instance.GameData.SetPlayerStartPos(this, transform.position);
    }

    new void Start()
    {
        base.Start();
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    void Update()
    {
        if (!IsAlive)
            return;

        if (capsuleCollider.IsTouchingLayers(hazardsLayer) && !hitObstacle)
        {
            ReduceHealth(this);
            hitObstacle = true;
        }
        else if (!capsuleCollider.IsTouchingLayers(hazardsLayer))
        {
            hitObstacle = false;
        }
    }

    void OnDisable()
    {
        EnemyMechanics.OnEnemyAttack -= ReduceHealth;
        OnPlayerDamaged -= UIManager.Instance.ReduceHealthUI;
        OnPlayerPickup -= UIManager.Instance.AddHealthUI;

        GameManager.Instance.OnGameRestart += PlayerReset;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentHealth < maxHealth && collision.gameObject.tag == "Heart" && !hitObstacle)
        {
            collision.gameObject.SetActive(false);
            OnPlayerPickup?.Invoke();
            currentHealth++;
            hitObstacle = true;
        }
    }

    private void ReduceHealth(PlayerHealth playerHealth)
    {
        if (playerHealth == this)
        {
            currentHealth--;
            if (currentHealth < 1)
            {
                playerAnimations?.PlayDeathAnim(false);
                IsAlive = false;
                GameManager.Instance.OnGamerOver();
                Invoke("DisableObj", 3f);
            }
            OnPlayerDamaged?.Invoke(currentHealth, maxHealth);
        }
    }

    void PlayerReset()
    {
        gameObject.transform.position = GameManager.Instance.GameData.GetPlayerStartPos(this); // use data from SO
        gameObject.SetActive(true);
    }

    void DisableObj() => gameObject.SetActive(false);
}
