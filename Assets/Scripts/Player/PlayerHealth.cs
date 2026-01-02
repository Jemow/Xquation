using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : EntityHealth
{
    [Header("Player")]
    [SerializeField] private float invisibleTime = 0.2f;
    
    [Header("UI")]
    [SerializeField] private Slider healthSlider;
    
    private bool _isInvincible;

    public override void ChangeHealth(int amount)
    {
        if(amount < 0 && _isInvincible) return;
        base.ChangeHealth(amount);
        StartCoroutine(Invincibility());
        UpdateHeathSlider();
    }

    private IEnumerator Invincibility()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(invisibleTime);
        _isInvincible = false;
    }

    public override void ResetHealth()
    {
        base.ResetHealth();
        UpdateHeathSlider();
    }
    
    private void UpdateHeathSlider() => healthSlider.value = HealthRatio;

    protected override void Death()
    {
        base.Death();
        MathLine.IsAttacking = false;
    }
}