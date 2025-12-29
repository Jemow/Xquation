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
    [Header("Health Configuration")]
    [Tooltip("The maximum that the player can have")]
    [SerializeField] [Min(1)] private int maxHealth = 100;

    int _currentHealth;

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

    protected virtual void Start()
    {
        // Initialize the entity with the maximum health
        CurrentHealth = maxHealth;
    }
    
    /// <summary>
    /// Handles the entity taking damage. Override for specific behavior.
    /// </summary>
    protected virtual void TakeDamage()
    {
        Debug.Log("Entity took damage");
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
    private void Death()
    {
        Debug.Log("Entity is dead");
        Destroy(gameObject);
    }
}