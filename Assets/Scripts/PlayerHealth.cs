using System.Collections;
using UnityEngine;

public class PlayerHealth : EntityHealth
{
    [Header("Player")]
    [SerializeField] private float invisibleTime = 0.2f;
    
    private bool _isInvincible;

    public override void ChangeHealth(int amount)
    {
        if(amount < 0 && _isInvincible) return;
        base.ChangeHealth(amount);
        StartCoroutine(Invincibility());
    }

    protected override void Death()
    {
        GameManager.Instance.GameOver();
    }

    private IEnumerator Invincibility()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(invisibleTime);
        _isInvincible = false;
    }
}