using UnityEngine;
using System.Collections.Generic;

public class MathProjectile : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private int damage = 1;
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float minSpeed = 0.5f; 
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float startTransitionDistance = 3f;
    [SerializeField] private float maxJumpDistance = 5f; 
    [SerializeField] private float maxMathY = 50f;          // <-- Limite pour mathY
    [SerializeField] private float maxDistanceTraveled = 1000f; // <-- Limite pour distance totale

    [Header("Audio")]
    [SerializeField] private float audioStrength = 5f;
    [SerializeField] private float audioScaleStrength = 1f;
    [SerializeField] private float maxScaleMultiplier = 5f; // <-- Limite du scale

    private TrailRenderer _trailRenderer;

    private Vector3 _startPosition;
    private Vector3 _direction;
    private Vector3 _perpendicularDir;
    
    private List<MathNode> _nodes;
    private float _distanceTraveled;
    
    private float _mathMinX;
    private float _mathScale;

    private Vector3 _initialScale;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _initialScale = transform.localScale;
    }
    
    public void Init(Vector3 direction, List<MathNode> nodes, float minX, float length, float scale)
    {
        _direction = direction.normalized;
        _perpendicularDir = new Vector3(-_direction.y, _direction.x, 0f);
        _startPosition = transform.position;
        
        _nodes = new List<MathNode>(nodes);
        _mathMinX = minX;
        _mathScale = scale;
    }

    private void Update()
    {
        float currentSpeed = minSpeed;
        float amp = 0f;

        if (AudioAmplitude.Instance)
        {
            amp = AudioAmplitude.Instance.Amplitude;
            currentSpeed += baseSpeed * amp * audioStrength;

            float scaleMultiplier = 1f + (amp * audioScaleStrength);
            scaleMultiplier = Mathf.Min(scaleMultiplier, maxScaleMultiplier);
            transform.localScale = _initialScale * scaleMultiplier;
        }
        else
        {
            currentSpeed = baseSpeed;
            transform.localScale = _initialScale;
        }

        _distanceTraveled += currentSpeed * Time.deltaTime;
        _distanceTraveled = Mathf.Min(_distanceTraveled, maxDistanceTraveled); // limiter la distance totale

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
            return;
        }
        
        float mathX = _mathMinX + (_distanceTraveled * _mathScale);
        float mathY = CalculateMathY(mathX);

        // Appliquer blend et limiter Y
        float blendFactor = Mathf.Clamp01(_distanceTraveled / startTransitionDistance);
        blendFactor = Mathf.SmoothStep(0f, 1f, blendFactor);
        mathY *= blendFactor;
        mathY = Mathf.Clamp(mathY, -maxMathY, maxMathY); // <-- clamp Y

        Vector3 straightPos = _startPosition + (_direction * _distanceTraveled);
        Vector3 offset = _perpendicularDir * mathY;
        
        Vector3 targetPosition = straightPos + offset;

        // Limiter la distance pour le TrailRenderer
        float dist = Vector3.Distance(transform.position, targetPosition);
        if (_trailRenderer && dist > maxJumpDistance && _distanceTraveled > 0.1f)
            _trailRenderer.Clear();

        transform.position = targetPosition;

        // Calcul de la direction suivante
        float nextDist = _distanceTraveled + (currentSpeed * 0.05f);
        nextDist = Mathf.Min(nextDist, maxDistanceTraveled);
        float nextMathX = _mathMinX + (nextDist * _mathScale);
        float nextMathY = Mathf.Clamp(CalculateMathY(nextMathX) * blendFactor, -maxMathY, maxMathY);
        Vector3 nextPos = (_startPosition + (_direction * nextDist)) + (_perpendicularDir * nextMathY);
        
        Vector3 lookDir = (nextPos - transform.position).normalized;
        if (lookDir != Vector3.zero) transform.right = lookDir;
    }

    private float CalculateMathY(float x)
    {
        if (_nodes == null || _nodes.Count == 0) return 0f;

        float y = 0f;
        bool hasValue = false;

        foreach (var node in _nodes)
        {
            float nodeY = hasValue ? node.Apply(y, x) : node.Value(x);
            if (!float.IsFinite(nodeY)) nodeY = 0f;
            y = !hasValue ? nodeY : nodeY;
            hasValue = true;
        }
        return y;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent(out EntityHealth entityHealth))
            entityHealth.ChangeHealth(-damage);
    }
}
