using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] protected CapsuleCollider2D capsuleCollider;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected LayerMask hazardsLayer;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float damageHit;

    protected float currentHealth;
    protected bool isOnHazard = false;

    protected void OnEnable()
    {
        currentHealth = maxHealth;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    protected void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    protected bool IsTouchingHazard()
    {
        isOnHazard = true;
        return capsuleCollider.IsTouchingLayers(hazardsLayer);
    }
}
