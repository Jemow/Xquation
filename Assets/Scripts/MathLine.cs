using UnityEngine;
using System.Collections.Generic;

public class MathLine : MonoBehaviour
{
    public LineRenderer Line { get; private set; }
    public EdgeCollider2D Collider { get; private set; }

    [Header("Settings")]
    [SerializeField] private float damagePerTick = 10f;
    [SerializeField] private float damageTickRate = 0.2f;

    private readonly Dictionary<int, float> _nextDamageTime = new();

    void Awake()
    {
        Line = GetComponent<LineRenderer>();
        Collider = GetComponent<EdgeCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        int id = other.GetInstanceID();

        if (_nextDamageTime.ContainsKey(id) && Time.time < _nextDamageTime[id]) return;

        if (IsTouchingEnemy(other))
        {
            Debug.Log($"Damage : {damagePerTick} (Touché !)");

            _nextDamageTime[id] = Time.time + damageTickRate;
        }
    }

    private bool IsTouchingEnemy(Collider2D enemyCollider)
    {
        int positionsCount = Line.positionCount;
        
        for (int i = 0; i < positionsCount; i++)
        {
            if (enemyCollider.bounds.Contains(Line.GetPosition(i)))
                return true; 
        }
        
        return false;
    }

    private void OnDisable()
    {
        _nextDamageTime.Clear();
    }
}