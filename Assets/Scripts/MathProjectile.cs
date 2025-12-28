using UnityEngine;
using System.Collections.Generic;

public class MathProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float minSpeed = 0.5f; 
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float startTransitionDistance = 3f;

    [Header("Audio")]
    [SerializeField] private float audioStrength = 5f; // C'est ici maintenant !

    private Vector3 _startPosition;
    private Vector3 _direction;
    private Vector3 _perpendicularDir;
    
    private List<MathNode> _nodes;
    private float _distanceTraveled;
    
    private float _mathMinX;
    private float _mathMaxX;
    private float _beamLength;

    // On a retiré "float audioStrength" des arguments
    public void Init(Vector3 direction, List<MathNode> nodes, float minX, float maxX, float length)
    {
        _direction = direction.normalized;
        _perpendicularDir = new Vector3(-_direction.y, _direction.x, 0f);
        _startPosition = transform.position;
        
        _nodes = new List<MathNode>(nodes);
        _mathMinX = minX;
        _mathMaxX = maxX;
        _beamLength = length;
    }

    private void Update()
    {
        float currentSpeed = minSpeed;
        
        if (AudioAmplitude.Instance)
        {
            // On utilise la variable locale audioStrength définie dans l'inspecteur du prefab
            currentSpeed += baseSpeed * AudioAmplitude.Instance.Amplitude * audioStrength;
        }
        else
        {
            currentSpeed = baseSpeed;
        }

        _distanceTraveled += currentSpeed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        float tNorm = _distanceTraveled / _beamLength;
        float mathX = Mathf.LerpUnclamped(_mathMinX, _mathMaxX, tNorm);
        float mathY = CalculateMathY(mathX);

        float blendFactor = Mathf.Clamp01(_distanceTraveled / startTransitionDistance);
        blendFactor = Mathf.SmoothStep(0f, 1f, blendFactor);
        mathY *= blendFactor;

        Vector3 straightPos = _startPosition + (_direction * _distanceTraveled);
        Vector3 offset = _perpendicularDir * mathY;
        
        transform.position = straightPos + offset;
        
        float nextDist = _distanceTraveled + (currentSpeed * 0.05f);
        float nextT = nextDist / _beamLength;
        float nextMathX = Mathf.LerpUnclamped(_mathMinX, _mathMaxX, nextT);
        float nextMathY = CalculateMathY(nextMathX) * blendFactor;
        
        Vector3 nextPos = (_startPosition + (_direction * nextDist)) + (_perpendicularDir * nextMathY);
        
        Vector3 lookDir = (nextPos - transform.position).normalized;
        if(lookDir != Vector3.zero) transform.right = lookDir;
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
            nodeY = Mathf.Clamp(nodeY, -500f, 500f); 
            y = !hasValue ? nodeY : nodeY;
            hasValue = true;
        }
        return y;
    }
}