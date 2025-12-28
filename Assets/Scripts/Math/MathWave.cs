using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathWave : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRendererPrefab;
    [SerializeField] private TextMeshProUGUI functionText;
    [SerializeField] private AudioAmplitude audioAmplitude;
    
    [Header("Function")]
    [SerializeField] private int numPoints = 50;
    [SerializeField] private float maxX = 20f;
    [SerializeField] private float maxY = 100f;
    [SerializeField] private float speed = 1f;
    
    [Header("Audio Influence")]
    [SerializeField] private float audioAmplitudeStrength = 0.5f;

    [Header("Debug")] 
    [SerializeField] private MathOp op;

    private readonly List<MathNode> _nodes = new();
    private readonly List<LineRenderer> _activeLines = new();
    private readonly Queue<LineRenderer> _linePool = new();

    private float _audioTime;
    private string _currentFormula;
    private bool _hasBase;

    private void Update() => UpdateWave();

    private void UpdateWave()
    {
        if (_nodes.Count == 0) return;
        
        Vector3 playerPos = transform.position;
        
        foreach (var line in _activeLines)
        {
            line.positionCount = 0;
            line.gameObject.SetActive(false);
            _linePool.Enqueue(line);
        }
        _activeLines.Clear();

        float targetSpeed = speed * (1f + audioAmplitude.Amplitude);
        _audioTime += targetSpeed * Time.deltaTime;

        float maxJump = 10f;
        float prevY = 0f;

        LineRenderer currentLine = GetLineFromPool();
        int pointIndex = 0;

        for (int i = 0; i < numPoints; i++)
        {
            float tNorm = i / (float)(numPoints - 1);
            float x = Mathf.Lerp(-maxX, maxX, tNorm);

            float y = 0f;
            bool hasValue = false;
            foreach (var node in _nodes)
            {
                float nodeY = hasValue ? node.Apply(y, x, _audioTime) : node.Value(x, _audioTime);
                
                if (!float.IsFinite(nodeY)) nodeY = 0f;
                nodeY = Mathf.Clamp(nodeY, -maxY*10f, maxY*10f);

                y = !hasValue ? nodeY : nodeY;

                hasValue = true;
            }

            y *= 1f + audioAmplitude.Amplitude * audioAmplitudeStrength;
            y = Mathf.Clamp(y, -maxY, maxY);

            if (!float.IsFinite(y) || Mathf.Abs(y - prevY) > maxJump)
            {
                currentLine = GetLineFromPool();
                pointIndex = 0;
            }

            currentLine.positionCount = pointIndex + 1;
            currentLine.SetPosition(pointIndex, playerPos + new Vector3(x, y, 0));
            pointIndex++;
            prevY = y;
        }
    }

    private LineRenderer GetLineFromPool()
    {
        LineRenderer line;
        if (_linePool.Count > 0)
        {
            line = _linePool.Dequeue();
            line.gameObject.SetActive(true);
        }
        else
        {
            line = Instantiate(lineRendererPrefab, transform);
        }

        _activeLines.Add(line);
        return line;
    }

    public void AddNodeX() => AddNode(new NodeX(op));
    public void AddNodeSin() => AddNode(new NodeSin(op));
    public void AddNodeTan() => AddNode(new NodeTan(op));
    public void AddNodeAsin() => AddNode(new NodeAsin(op));
    public void AddNodeConstant(float value) => AddNode(new NodeConstant(value, op));

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
        _audioTime = 0f;
        
        foreach (var line in _activeLines)
        {
            line.positionCount = 0;
            line.gameObject.SetActive(false);
            _linePool.Enqueue(line);
        }
        _activeLines.Clear();
    }
}
