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
    public static bool IsFull = true;

    public delegate void PlayerDamagedHandler(float currentHealth, float maxHealth);
    public static event PlayerDamagedHandler OnPlayerDamaged;

    new void OnEnable()
    {
        base.OnEnable();
        IsAlive = true;
        
        transform.position = GameManager.Instance.CurrentLevelPlayerPos;

        UIManager.Instance.SetHealthIndex(currentHealth);
        
        GameManager.Instance.OnGameRestart -= PlayerReset;
        OnPlayerDamaged += UIManager.Instance.ReduceHealthUI;
        EnemyMechanics.OnEnemyAttack += ReduceHealth;
        GameManager.Instance.OnHealthPickup += AddHealth;

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

        GameManager.Instance.OnGameRestart += PlayerReset;
        GameManager.Instance.OnLevelComplete += () => gameObject.SetActive(true);
        GameManager.Instance.OnHealthPickup -= AddHealth;
    }

    void AddHealth()
    {
        if (currentHealth < maxHealth)
            currentHealth++;

        IsFull = currentHealth == maxHealth;
    }

    private void ReduceHealth(PlayerHealth playerHealth)
    {
        if (playerHealth == this)
        {
            currentHealth--;
            IsFull = false;
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
        gameObject.transform.position = GameManager.Instance.GameData.GetPlayerStartPos(this); 
        gameObject.SetActive(true);
    }

    void DisableObj() => gameObject.SetActive(false);
}
