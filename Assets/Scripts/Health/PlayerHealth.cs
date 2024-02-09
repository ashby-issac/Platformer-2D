using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private LayerMask heartsLayer;

    private PlayerAnimations playerAnimations;
    private bool hitObstacle = false;

    public static bool IsAlive = false;
    public static bool IsFull = true;

    public delegate void PlayerDamagedHandler(float currentHealth, float maxHealth);
    public static event PlayerDamagedHandler OnPlayerDamaged;

    new void OnEnable()
    {
        base.OnEnable();
        IsAlive = true;
        
        transform.position = GameController.Instance.CurrentLevelPlayerPos;
        AudioManager.Instance.LevelStartAudio();

        UIManager.Instance.SetHealthIndex(currentHealth);
        
        GameController.Instance.OnGameRestart -= PlayerReset;
        OnPlayerDamaged += UIManager.Instance.ReduceHealthUI;
        EnemyMechanics.OnEnemyAttack += ReduceHealth;
        GameController.Instance.OnHealthPickup += AddHealth;

        GameController.Instance.GameData.SetPlayerStartPos(this, transform.position);
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
            AudioManager.Instance.SpikesClip();
            hitObstacle = true;
        }
        else if (!capsuleCollider.IsTouchingLayers(hazardsLayer))
        {
            hitObstacle = false;
        }
    }

    new void OnDisable()
    {
        EnemyMechanics.OnEnemyAttack -= ReduceHealth;
        OnPlayerDamaged -= UIManager.Instance.ReduceHealthUI;

        GameController.Instance.OnGameRestart += PlayerReset;
        GameController.Instance.OnLevelComplete += () => gameObject.SetActive(true);
        GameController.Instance.OnHealthPickup -= AddHealth;
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
                GameController.Instance.OnGamerOver();
                rb.bodyType = RigidbodyType2D.Static;
                Invoke("DisableObj", 3f);
            }
            OnPlayerDamaged?.Invoke(currentHealth, maxHealth);
        }
    }

    void PlayerReset()
    {
        gameObject.transform.position = GameController.Instance.GameData.GetPlayerStartPos(this); 
        gameObject.SetActive(true);
    }

    void DisableObj() => gameObject.SetActive(false);
}
