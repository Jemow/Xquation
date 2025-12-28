using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathWave : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
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
    
    private float _audioTime;

    private string _currentFormula;
    
    private bool _hasBase;

    private void Update() => UpdateWave();

    private void UpdateWave()
    {
        Vector3 playerPos = transform.position;
        lineRenderer.positionCount = numPoints;
        
        float targetSpeed = speed * (1f + audioAmplitude.Amplitude);
        _audioTime += targetSpeed * Time.deltaTime;

        for (int i = 0; i < numPoints; i++)
        {
            float tNorm = i / (float)(numPoints - 1);
            float x = Mathf.Lerp(-maxX, maxX, tNorm);

            float y = 0f;
            bool hasValue = false;

            foreach (var node in _nodes)
            {
                if (!hasValue)
                {
                    y = node.Value(x, _audioTime);
                    hasValue = true;
                }
                else y = node.Apply(y, x, _audioTime);
            }
            
            if (!float.IsFinite(y)) y = 0f;
            
            y *= 1f + audioAmplitude.Amplitude * audioAmplitudeStrength;
            
            y = Mathf.Clamp(y, -maxY, maxY);

            lineRenderer.SetPosition(i, playerPos + new Vector3(x, y, 0));
        }
    }
    
    public void AddNodeX() => AddNode(new NodeX(op));
    public void AddNodeSin() => AddNode(new NodeSin(op));
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
        lineRenderer.positionCount = 0;
    }
}