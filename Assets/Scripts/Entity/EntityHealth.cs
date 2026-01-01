using System.Collections;
using UnityEngine;

/**
 * @jemoelablay
 * EntityHealth.sc
 * 09.09.2024
 * Description : This script simulate the health of an entity, it can be
 * a player, an enemy, or even an object. To deal damage or heal the entity
 * simply call the ChangeHealth() function with the desired amount.
 */

public class EntityHealth : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Collider2D[] _collider2Ds;
    
    [Header("Health Configuration")]
    [Tooltip("The maximum that the player can have")]
    [SerializeField] [Min(1)] private int maxHealth = 100;
    
    [Tooltip("How much time the hit effect will be displayed")]
    [SerializeField] private float _hitEffectDuration = 0.1f;
    
    protected float HealthRatio => (float)_currentHealth / maxHealth;

    /// <summary>
    /// Gets or sets the current health of the entity.
    /// Health is clamped between 0 and MaxHealth.
    /// </summary>
    private int CurrentHealth 
    { 
        get => _currentHealth;
        set
        {
            if (value > maxHealth) _currentHealth = maxHealth;
            else if (value <= 0) { _currentHealth = 0; Death(); }
            else _currentHealth = value;
        }
    }

    private EntityMovement _entityMovement;
    private EntityAnimation _entityAnimation;
    private Material _material;
    
    private int _currentHealth;

    protected virtual void Start()
    {
        _entityMovement = GetComponent<EntityMovement>();
        _entityAnimation = GetComponentInChildren<EntityAnimation>();
        _material = GetComponentInChildren<Renderer>().material;
        
        // Initialize the entity with the maximum health
        CurrentHealth = maxHealth;
    }
    
    /// <summary>
    /// Handles the entity taking damage. Override for specific behavior.
    /// </summary>
    protected virtual void TakeDamage()
    {
        StartCoroutine(HitRoutine());
    }
    
    /// <summary>
    /// This function allows to change the entity's health by a specific amount.
    /// </summary>
    /// <param name="amount">The amount to add to the entity's health. A negative value will deal damage.</param>
    public virtual void ChangeHealth(int amount)
    {
        CurrentHealth += amount;
        if (amount < 0) TakeDamage();
    }

    /// <summary>
    /// Handles the entity's death. Override to customize death behavior.
    /// </summary>
    protected virtual void Death()
    {
        foreach (var collider2D in _collider2Ds)
            collider2D.enabled = false;
        
        _entityMovement.ResetDirection();
        _entityMovement.enabled = false;
        _entityAnimation.Death();
    }

    private IEnumerator HitRoutine()
    {
        _material.SetFloat("_HitEffectBlend", 1f);
        yield return new WaitForSeconds(_hitEffectDuration);
        _material.SetFloat("_HitEffectBlend", 0f);
    }
    
    public virtual void ResetHealth() => CurrentHealth = maxHealth;
}