using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MathWave : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private MathLine linePrefab;
    [SerializeField] private TextMeshProUGUI functionText;
    [SerializeField] private Transform linesParent;
    
    [Header("Parameters")]
    [SerializeField] private float speed = 5f;

    [Tooltip("The speed when the user is attacking")]
    [SerializeField] private float speedAttack = 10f;

    [Header("Visual Settings")] 
    [SerializeField] private int numPoints = 500;
    [SerializeField] private float startTransitionDistance = 3f;

    [Header("Juice")]
    [SerializeField] private float attackAmplitudeVibration = 0.3f;
    [SerializeField] private float attackPhaseVibration = 0.5f;

    [Header("Math Domain")]
    [SerializeField] private float mathMinX = -10f; 
    [SerializeField] private float mathScale = 1f; 
    
    [Header("Limits (The Scissors)")]
    [SerializeField] private float maxX = 20f; 
    [SerializeField] private float maxY = 20f;
    [SerializeField] private float maxJump = 20f;

    public float TimeValue { get; private set; }
    public float NodeCount => _nodes.Count;

    private readonly List<MathNode> _nodes = new();
    private readonly List<MathLine> _activeLines = new();
    private readonly Queue<MathLine> _linePool = new();
    private readonly List<Vector2> _currentColliderPoints = new();
    
    private Camera _mainCamera;

    private string _currentFormula;
    private bool _hasBase;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        TimeValue += (MathLine.IsAttacking ? speedAttack : speed) * Time.deltaTime;
        UpdateWave();
    }

    private void UpdateWave()
    {
        if (_nodes.Count == 0) return;

        Vector3 playerPos = transform.position;
        if (Mouse.current == null) return;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        
        Vector3 forwardDir = (mouseWorldPos - playerPos).normalized;
        Vector3 upDir = new Vector3(-forwardDir.y, forwardDir.x, 0f);
        
        float prevY = 0f;
        int activeLineIndex = 0;
        MathLine currentLineObj = GetLineOrReuse(activeLineIndex);

        int pointIndex = 0;
        _currentColliderPoints.Clear();

        float currentAmpVibration = 1f;
        float currentPhaseVibration = 0f;

        if (MathLine.IsAttacking)
        {
            currentAmpVibration = Random.Range(1f - attackAmplitudeVibration, 1f + attackAmplitudeVibration);
            currentPhaseVibration = Random.Range(-attackPhaseVibration, attackPhaseVibration);
        }

        for (int i = 0; i < numPoints; i++)
        {
            float tNorm = i / (float)(numPoints - 1);
            
            float visualX = Mathf.Lerp(0f, maxX, tNorm); 
            float mathX = mathMinX + (visualX * mathScale) + currentPhaseVibration;

            float y = 0f;
            bool hasValue = false;
            
            foreach (var node in _nodes)
            {
                float nodeY = hasValue ? node.Apply(y, mathX) : node.Value(mathX);
                if (!float.IsFinite(nodeY)) nodeY = 0f;
                y = !hasValue ? nodeY : nodeY;
                hasValue = true;
            }

            y *= currentAmpVibration;
            
            float blendFactor = Mathf.Clamp01(visualX / startTransitionDistance);
            blendFactor = Mathf.SmoothStep(0f, 1f, blendFactor);
            y *= blendFactor;

            float clampedY = Mathf.Clamp(y, -maxY, maxY);

            bool isBigJump = Mathf.Abs(clampedY - prevY) > maxJump;
            bool isSignChange = Mathf.Sign(clampedY) != Mathf.Sign(prevY);
            
            bool isCut = !float.IsFinite(y) || (isBigJump && isSignChange);

            if (i > 0 && isCut && pointIndex > 0)
            {
                if (currentLineObj.Collider)
                {
                    if (_currentColliderPoints.Count >= 2)
                    {
                        currentLineObj.Collider.enabled = true;
                        currentLineObj.Collider.SetPoints(_currentColliderPoints);
                    }
                    else currentLineObj.Collider.enabled = false;
                }

                activeLineIndex++;
                currentLineObj = GetLineOrReuse(activeLineIndex);
                pointIndex = 0;
                _currentColliderPoints.Clear();
            }

            if (float.IsFinite(clampedY))
            {
                currentLineObj.Line.positionCount = pointIndex + 1;
                Vector3 finalPos = playerPos + (forwardDir * visualX) + (upDir * clampedY);
                currentLineObj.Line.SetPosition(pointIndex, finalPos);
                _currentColliderPoints.Add(transform.InverseTransformPoint(finalPos));
                
                pointIndex++;
                prevY = clampedY;
            }
        }

        if (currentLineObj.Collider)
        {
            if (_currentColliderPoints.Count >= 2)
            {
                currentLineObj.Collider.enabled = true;
                currentLineObj.Collider.SetPoints(_currentColliderPoints);
            }
            else currentLineObj.Collider.enabled = false;
        }

        activeLineIndex++;
        CleanupUnusedLines(activeLineIndex);
    }
    
    private MathLine GetLineOrReuse(int index)
    {
        MathLine lineObj;
        if (index < _activeLines.Count) lineObj = _activeLines[index];
        else if (_linePool.Count > 0)
        {
            lineObj = _linePool.Dequeue();
            lineObj.gameObject.SetActive(true);
        }
        else lineObj = Instantiate(linePrefab, linesParent);
        
        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localRotation = Quaternion.identity;

        if (index >= _activeLines.Count) _activeLines.Add(lineObj);
        return lineObj;
    }

    private void CleanupUnusedLines(int countToKeep)
    {
        for (int i = _activeLines.Count - 1; i >= countToKeep; i--)
        {
            MathLine lineObj = _activeLines[i];
            lineObj.Line.positionCount = 0;
            lineObj.gameObject.SetActive(false);
            _linePool.Enqueue(lineObj);
            _activeLines.RemoveAt(i);
        }
    }

    public void AddNode(MathNode node)
    {
        if (!_hasBase)
        {
            _nodes.Add(node);
            _currentFormula = node.Formula();
            _hasBase = true;
        }
        else
        {
            _nodes.Add(node);
            _currentFormula = node.ApplyFormula(_currentFormula);
        }
        functionText.text = $"y = {_currentFormula}";
    }

    public void Clear()
    {
        _nodes.Clear();
        _hasBase = false;
        _currentFormula = "";
        functionText.SetText("y = ?");
        CleanupUnusedLines(0);
    }

    public List<MathNode> GetNodes() => _nodes;
    public string GetFormula() => _currentFormula;
    public float GetMinX() => mathMinX;
    public float GetMathScale() => mathScale;
    public float GetMaxY() => maxY;
}