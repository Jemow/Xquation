using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathWave : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private MathLine linePrefab;
    [SerializeField] private TextMeshProUGUI functionText;
    [SerializeField] private AudioAmplitude audioAmplitude;

    [Header("Visual Settings")] 
    [SerializeField] private int numPoints = 100;
    [SerializeField] private float beamLength = 20f;
    [SerializeField] private float startTransitionDistance = 3f;

    [Header("Math Domain")]
    [SerializeField] private float mathMinX = -10f; 
    [SerializeField] private float mathMaxX = 10f;
    
    [Header("Limits")]
    [SerializeField] private float maxY = 100f;
    [SerializeField] private float speed = 1f;

    [Header("Audio Influence")] 
    [SerializeField] private float audioAmplitudeStrength = 0.5f;
    
    [Header("Debug")] 
    [SerializeField] private MathOp op;

    public float TimeValue { get; private set; }

    private readonly List<MathNode> _nodes = new();
    private readonly List<MathLine> _activeLines = new();
    private readonly Queue<MathLine> _linePool = new();
    private readonly List<Vector2> _currentColliderPoints = new();

    private string _currentFormula;
    private bool _hasBase;

    private void Update()
    {
        float audioAmp = audioAmplitude.Amplitude;
        if (audioAmp > 0.01f) TimeValue += speed * audioAmp * Time.deltaTime;

        UpdateWave();
    }

    private void UpdateWave()
    {
        if (_nodes.Count == 0) return;

        Vector3 playerPos = transform.position;
        float maxJump = 10f;
        float prevY = 0f;

        int activeLineIndex = 0;
        MathLine currentLineObj = GetLineOrReuse(activeLineIndex);

        int pointIndex = 0;
        _currentColliderPoints.Clear();

        for (int i = 0; i < numPoints; i++)
        {
            float tNorm = i / (float)(numPoints - 1);
            float visualX = Mathf.Lerp(0f, beamLength, tNorm); 
            float mathX = Mathf.Lerp(mathMinX, mathMaxX, tNorm);

            float y = 0f;
            bool hasValue = false;
            
            foreach (var node in _nodes)
            {
                float nodeY = hasValue ? node.Apply(y, mathX) : node.Value(mathX);
                if (!float.IsFinite(nodeY)) nodeY = 0f;
                nodeY = Mathf.Clamp(nodeY, -maxY * 10f, maxY * 10f);
                y = !hasValue ? nodeY : nodeY;
                hasValue = true;
            }

            y *= 1f + audioAmplitude.Amplitude * audioAmplitudeStrength;
            
            float blendFactor = Mathf.Clamp01(visualX / startTransitionDistance);
            blendFactor = Mathf.SmoothStep(0f, 1f, blendFactor);
            y *= blendFactor;

            y = Mathf.Clamp(y, -maxY, maxY);

            bool isCut = !float.IsFinite(y) || Mathf.Abs(y - prevY) > maxJump;

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

            if (float.IsFinite(y))
            {
                currentLineObj.Line.positionCount = pointIndex + 1;
                
                currentLineObj.Line.SetPosition(pointIndex, playerPos + new Vector3(visualX, y, 0));
                _currentColliderPoints.Add(new Vector2(visualX, y));
                
                pointIndex++;
                prevY = y;
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
        else lineObj = Instantiate(linePrefab, transform);

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

    public void AddNodeX() => AddNode(new NodeX(op));
    public void AddNodeSin() => AddNode(new NodeSin(op));
    public void AddNodeTan() => AddNode(new NodeTan(op));
    public void AddNodeAsin() => AddNode(new NodeAsin(op));
    public void AddNodeConstant(float value) => AddNode(new NodeConstant(value, op));
    public void AddNodeT() => AddNode(new NodeT(op, this));

    private void AddNode(MathNode node)
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
        functionText.text = "y = ?";
        CleanupUnusedLines(0);
    }
}