using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathWave : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private TextMeshProUGUI functionText;
    
    [Header("Function")]
    [SerializeField] private int numPoints = 50;
    [SerializeField] private float maxX = 20f;
    [SerializeField] private float speed = 1f;

    [Header("Debug")] 
    [SerializeField] private MathOp op;
    
    private readonly List<MathNode> _nodes = new();
    
    private string _currentFormula = "x";

    private void Start()
    {
        functionText.SetText(_currentFormula);
    }

    private void Update() => UpdateWave();

    private void UpdateWave()
    {
        Vector3 playerPos = transform.position;
        lineRenderer.positionCount = numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float tNorm = i / (float)(numPoints - 1);
            float x = Mathf.Lerp(-maxX, maxX, tNorm);

            float y = x;
            foreach (var node in _nodes)
                y = node.Apply(y, x, Time.time * speed);
            
            if (!float.IsFinite(y)) y = 0f;

            lineRenderer.SetPosition(i, playerPos + new Vector3(x, y, 0));
        }
    }

    public void AddNodeSin() => AddNode(new NodeSin(op));
    public void AddNodeConstant(float value) => AddNode(new NodeConstant(value, op));

    private void AddNode(MathNode node)
    {
        _nodes.Add(node);
        
        _currentFormula = node.ApplyFormula(_currentFormula);
        functionText.text = $"y = {_currentFormula}";
    }
}