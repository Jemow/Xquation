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
    
    [Header("Detection")]
    [SerializeField] private float projectileRadius = 0.5f; 
    [SerializeField] private float maxJumpDistance = 5f; 
    [SerializeField] private float maxMathY = 50f;
    [SerializeField] private float maxDistanceTraveled = 1000f;

    [Header("Collision")]
    [SerializeField] private LayerMask hitLayers; 
    [SerializeField] private string targetTag = "Enemy"; 

    [Header("Audio")]
    [SerializeField] private float audioStrength = 5f;
    [SerializeField] private float audioScaleStrength = 1f;
    [SerializeField] private float maxScaleMultiplier = 5f;

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
        if (AudioAmplitude.Instance)
        {
            float amp = AudioAmplitude.Instance.Amplitude;
            currentSpeed += baseSpeed * amp * audioStrength;
            float scaleMultiplier = Mathf.Min(1f + (amp * audioScaleStrength), maxScaleMultiplier);
            transform.localScale = _initialScale * scaleMultiplier;
        }
        else
        {
            currentSpeed = baseSpeed;
            transform.localScale = _initialScale;
        }

        float moveStep = currentSpeed * Time.deltaTime;
        float nextDistance = _distanceTraveled + moveStep;
        nextDistance = Mathf.Min(nextDistance, maxDistanceTraveled);

        Vector3 currentPos = transform.position;

        float mathX = _mathMinX + (nextDistance * _mathScale);
        float rawMathY = CalculateMathY(mathX);
        
        float blendFactor = Mathf.Clamp01(nextDistance / startTransitionDistance);
        blendFactor = Mathf.SmoothStep(0f, 1f, blendFactor);
        float mathY = Mathf.Clamp(rawMathY * blendFactor, -maxMathY, maxMathY);

        Vector3 nextPos = _startPosition + (_direction * nextDistance) + (_perpendicularDir * mathY);

        float stepDistance = Vector3.Distance(currentPos, nextPos);

        if (stepDistance > projectileRadius && stepDistance <= maxJumpDistance)
        {
            RaycastHit2D hit = Physics2D.Linecast(currentPos, nextPos, hitLayers);
            
            if (hit.collider != null && hit.collider.CompareTag(targetTag))
            {
                ApplyDamage(hit.collider);
            }
        }
        else if (stepDistance > maxJumpDistance && _trailRenderer && _distanceTraveled > 0.1f)
        {
            _trailRenderer.Clear();
        }

        transform.position = nextPos;
        _distanceTraveled = nextDistance;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f) Destroy(gameObject);

        UpdateRotation(nextDistance, currentSpeed, blendFactor);
    }

    private void ApplyDamage(Collider2D other)
    {
        if (other.TryGetComponent(out EntityHealth entityHealth))
        {
            entityHealth.ChangeHealth(-damage);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            ApplyDamage(other);
        }
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

    private void UpdateRotation(float currentDist, float speed, float blend)
    {
        float futureDist = currentDist + (speed * 0.02f);
        float futureMathX = _mathMinX + (futureDist * _mathScale);
        float futureY = Mathf.Clamp(CalculateMathY(futureMathX) * blend, -maxMathY, maxMathY);
        Vector3 futurePos = _startPosition + (_direction * futureDist) + (_perpendicularDir * futureY);
        
        Vector3 lookDir = (futurePos - transform.position).normalized;
        if (lookDir != Vector3.zero) transform.right = lookDir;
    }
}