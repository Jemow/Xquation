using UnityEngine;
using System.Collections.Generic;

public class MathLine : MonoBehaviour
{
    public LineRenderer Line { get; private set; }
    public EdgeCollider2D Collider { get; private set; }

    [Header("References")] 
    [SerializeField] private Material _attackMaterial;
    [SerializeField] private Material _chaosMaterial;

    [Header("Settings")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float damageTickRate = 0.2f;
    [SerializeField] private float chaosThreshold = 3f;
    [SerializeField] private ContactFilter2D filter; 

    [Header("Visuals")]
    [SerializeField] private Gradient normalColor;
    [SerializeField] private Gradient attackColor;
    [SerializeField] private Gradient chaosColor;

    public static bool IsAttacking;

    private Material _defaultMaterial;
    private bool _wasAttacking;
    
    private bool _isChaotic;

    private readonly Dictionary<int, float> _nextDamageTime = new();
    private readonly List<Collider2D> _results = new();
    private Vector3[] _positionsBuffer = new Vector3[500];

    private void Awake()
    {
        Line = GetComponent<LineRenderer>();
        Collider = GetComponent<EdgeCollider2D>();
        
        if (Line)
        {
            _defaultMaterial = Line.sharedMaterial;
            Line.colorGradient = normalColor;
        }
    }

    private void OnEnable()
    {
        _wasAttacking = !IsAttacking;
        UpdateVisuals(true);
    }

    private void Update()
    {
        if (_wasAttacking != IsAttacking)
        {
            _wasAttacking = IsAttacking;
            _isChaotic = false; 
            UpdateVisuals(true);
        }

        if(!IsAttacking) return;

        CheckChaos();

        if (!Collider) return;

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
    }

    private void UpdateVisuals(bool forceUpdate = false)
    {
        if (!Line) return;

        if (!IsAttacking)
        {
            if (Line.sharedMaterial != _defaultMaterial || forceUpdate)
            {
                Line.sharedMaterial = _defaultMaterial;
                Line.colorGradient = normalColor;
            }
        }
        else
        {
            if (_isChaotic)
            {
                if (Line.sharedMaterial != _chaosMaterial || forceUpdate)
                {
                    Line.sharedMaterial = _chaosMaterial;
                    Line.colorGradient = chaosColor;
                }
            }
            else
            {
                if (Line.sharedMaterial != _attackMaterial || forceUpdate)
                {
                    Line.sharedMaterial = _attackMaterial;
                    Line.colorGradient = attackColor;
                }
            }
        }
    }

    private void CheckChaos()
    {
        if (!Line || Line.positionCount < 2) return;

        int count = Line.positionCount;
        if (_positionsBuffer.Length < count) _positionsBuffer = new Vector3[count];

        Line.GetPositions(_positionsBuffer);

        float totalLength = 0f;
        for (int i = 0; i < count - 1; i++)
        {
            totalLength += Vector3.Distance(_positionsBuffer[i], _positionsBuffer[i + 1]);
        }

        float directDistance = Vector3.Distance(_positionsBuffer[0], _positionsBuffer[count - 1]);
        if (directDistance < 0.01f) directDistance = 0.01f;

        float ratio = totalLength / directDistance;
        bool currentlyChaotic = ratio > chaosThreshold;

        if (currentlyChaotic != _isChaotic)
        {
            _isChaotic = currentlyChaotic;
            UpdateVisuals();
        }
    }

    private void OnDisable()
    {
        _nextDamageTime.Clear();
    }
}