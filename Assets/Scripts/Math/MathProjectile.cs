using UnityEngine;
using System.Collections.Generic;

public class MathProjectile : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private int damage = 1;
    [SerializeField] private int funcDamage = 2;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float funcSpeed = 10f;
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
    [SerializeField] private float cameraShakeAmplitude = 1f;
    [SerializeField] private float cameraShakeFrequency = 0.5f;
    [SerializeField] private float cameraShakeDuration = 0.2f;
    
    [Header("Visual")]
    [SerializeField] private Material funcProjectileMaterial;
    [SerializeField] private Color funcColor = Color.yellow;
    
    [Header("Sounds")]
    [SerializeField] AudioClip projectileHitClip;

    private SpriteRenderer _spriteRenderer;
    private TrailRenderer _trailRenderer;
    
    private Material _originalMaterial;
    private Color _originalColor;
    private Material _originalTrailMaterial;
    private Gradient _originalTrailGradient;

    private Vector3 _startPosition;
    private Vector3 _direction;
    private Vector3 _perpendicularDir;
    
    private List<MathNode> _nodes;
    private float _distanceTraveled;
    private float _mathMinX;
    private float _mathScale;
    private float _currentLifeTime;
    
    private bool _followFunction;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();

        if (_spriteRenderer)
        {
            _originalMaterial = _spriteRenderer.sharedMaterial;
            _originalColor = _spriteRenderer.color;
        }

        if (_trailRenderer)
        {
            _originalTrailMaterial = _trailRenderer.sharedMaterial;
            _originalTrailGradient = _trailRenderer.colorGradient;
        }
    }
    
    public void Init(Vector3 direction, List<MathNode> nodes, float minX, float scale, float limitY, bool followFunction)
    {
        _distanceTraveled = 0f;
        _currentLifeTime = lifeTime;
        _startPosition = transform.position; 
        
        if (_trailRenderer) _trailRenderer.Clear();

        _direction = direction.normalized;
        _perpendicularDir = new Vector3(-_direction.y, _direction.x, 0f);
        
        _nodes = new List<MathNode>(nodes);
        _mathMinX = minX;
        _mathScale = scale;
        _followFunction = followFunction;
        
        maxMathY = limitY;

        if (_followFunction)
        {
            if (_spriteRenderer)
            {
                _spriteRenderer.material = funcProjectileMaterial;
                _spriteRenderer.color = funcColor;
            }

            if (_trailRenderer)
            {
                _trailRenderer.material = funcProjectileMaterial;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new[] { new GradientColorKey(funcColor, 0.0f), new GradientColorKey(funcColor, 1.0f) },
                    new [] { new GradientAlphaKey(funcColor.a, 0.0f), new GradientAlphaKey(funcColor.a, 1.0f) }
                );
                _trailRenderer.colorGradient = gradient;
            }
        }
        else
        {
            if (_spriteRenderer)
            {
                _spriteRenderer.material = _originalMaterial;
                _spriteRenderer.color = _originalColor;
            }

            if (_trailRenderer)
            {
                _trailRenderer.material = _originalTrailMaterial;
                _trailRenderer.colorGradient = _originalTrailGradient;
            }
        }
        
        transform.right = _direction;
    }

    private void Update()
    {
        float currentSpeed = _followFunction ? funcSpeed : speed;
        
        float moveStep = currentSpeed * Time.deltaTime;
        float nextDistance = _distanceTraveled + moveStep;
        nextDistance = Mathf.Min(nextDistance, maxDistanceTraveled);

        Vector3 currentPos = transform.position;
        float mathY = 0f;

        if (_followFunction)
        {
            float mathX = _mathMinX + (nextDistance * _mathScale);
            float rawMathY = CalculateMathY(mathX);
            
            float blendFactor = Mathf.Clamp01(nextDistance / startTransitionDistance);
            blendFactor = Mathf.SmoothStep(0f, 1f, blendFactor);
            mathY = Mathf.Clamp(rawMathY * blendFactor, -maxMathY, maxMathY);
            
            UpdateRotation(nextDistance, currentSpeed, blendFactor);
        }
        else
        {
            transform.right = _direction;
        }

        Vector3 nextPos = _startPosition + (_direction * nextDistance) + (_perpendicularDir * mathY);

        float stepDistance = Vector3.Distance(currentPos, nextPos);
        
        transform.position = nextPos;
        _distanceTraveled = nextDistance;

        if (stepDistance > projectileRadius && stepDistance <= maxJumpDistance)
        {
            RaycastHit2D hit = Physics2D.Linecast(currentPos, nextPos, hitLayers);
            
            if (hit.collider && hit.collider.CompareTag(targetTag))
                ApplyDamage(hit.collider);
        }
        else if (stepDistance > maxJumpDistance && _trailRenderer && _distanceTraveled > 0.1f)
        {
            _trailRenderer.Clear();
        }

        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime <= 0f) gameObject.SetActive(false);
    }

    private void ApplyDamage(Collider2D other)
    {
        if (other.TryGetComponent(out EntityHealth entityHealth))
        {
            entityHealth.ChangeHealth(-(_followFunction ? funcDamage : damage));
            SFXManager.Instance.PlaySFX(projectileHitClip, transform.position);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            ApplyDamage(other);
            CameraShakeManager.Instance.Shake(cameraShakeAmplitude, cameraShakeFrequency, cameraShakeDuration);
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