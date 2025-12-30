using UnityEngine;
using System.Collections.Generic;

public class MathLine : MonoBehaviour
{
    public LineRenderer Line { get; private set; }
    public EdgeCollider2D Collider { get; private set; }

    [Header("Settings")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float damageTickRate = 0.2f;
    [SerializeField] private ContactFilter2D filter; 

    private readonly Dictionary<int, float> _nextDamageTime = new();
    private readonly List<Collider2D> _results = new();

    void Awake()
    {
        Line = GetComponent<LineRenderer>();
        Collider = GetComponent<EdgeCollider2D>();
    }

    private void Update()
    {
        if (Collider == null) return;

        int count = Collider.Overlap(filter, _results);

        for (int i = 0; i < count; i++)
        {
            Collider2D other = _results[i];
            
            if (!other.CompareTag("Enemy")) continue;

            int id = other.GetInstanceID();

            if (_nextDamageTime.ContainsKey(id) && Time.time < _nextDamageTime[id]) continue;

            if (other.TryGetComponent(out EntityHealth health))
            {
                health.ChangeHealth(-damagePerTick);
                _nextDamageTime[id] = Time.time + damageTickRate;
            }
        }
    }//

    private void OnDisable()
    {
        _nextDamageTime.Clear();
    }
}