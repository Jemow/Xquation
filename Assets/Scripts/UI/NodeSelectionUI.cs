using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NodeSelectionUI : MonoBehaviour
{
    [Serializable]
    public struct OpButtonSetup
    {
        public string name; 
        public MathOp op;
        public SelectButton button;
    }

    [Header("References")] 
    [SerializeField] private MathWave mathWave; 

    [Header("Data")]
    [SerializeField] private NodeData[] nodes;

    [Header("Rarity Weights")]
    [SerializeField] private int commonWeight = 60;
    [SerializeField] private int rareWeight = 30;
    [SerializeField] private int epicWeight = 10;

    [Header("Constant Generation Settings")]
    [SerializeField] private float minConstantValue = 1f;
    [SerializeField] private float maxConstantValue = 10f;

    [Header("UI References")]
    [SerializeField] private SelectButton[] nodeButtons;
    [SerializeField] private OpButtonSetup[] operationButtons;
    [SerializeField] private TextMeshProUGUI _formulaPlaceholderTmp;

    [Header("Events")] 
    [SerializeField] private UnityEvent onComposeValid;
    [SerializeField] private UnityEvent onComposeInvalid;

    private NodeData _selectedNodeData;
    private MathOp _selectedOp;

    private string _currentFormula;
    
    private float _selectedConstantValue;

    private void Start()
    {
        InitOperationButtons();
        // Initialisation visuelle si c'est null
        if (_formulaPlaceholderTmp) _formulaPlaceholderTmp.text = "y = ?";
    }

    private void InitOperationButtons()
    {
        for (int i = 0; i < operationButtons.Length; i++)
        {
            OpButtonSetup setup = operationButtons[i];
            
            setup.button.Button.onClick.RemoveAllListeners();
            setup.button.Tmp.SetText(setup.name);
            setup.button.Button.onClick.AddListener(() => OnOpSelected(setup.button, setup.op));

            if (i == 0) OnOpSelected(setup.button, setup.op);
        }
    }

    private void OnOpSelected(SelectButton clickedButton, MathOp op)
    {
        _selectedOp = op;

        foreach (var setup in operationButtons)
        {
            setup.button.SetSelected(setup.button == clickedButton);
        }
        
        CheckCompose();
        UpdateFormulaPlaceholder();
    }

    public void GenerateNodes()
    {
        List<NodeData> availableNodes = new List<NodeData>(nodes);
        _selectedNodeData = null;

        for (int i = 0; i < nodeButtons.Length; i++)
        {
            if (availableNodes.Count == 0) break;
            
            NodeData pickedData = PickWeightedNode(availableNodes);
            availableNodes.Remove(pickedData);

            SelectButton selectButton = nodeButtons[i];
            
            float generatedValue = 0f;
            string buttonText = pickedData.nodeName;

            if (pickedData.nodeType == NodeType.Constant)
            {
                generatedValue = (float)Math.Round(UnityEngine.Random.Range(minConstantValue, maxConstantValue) * 2f) / 2f;
                buttonText = generatedValue.ToString();
            }
            
            selectButton.SetSelected(false);
            selectButton.Button.onClick.RemoveAllListeners();
            selectButton.Tmp.SetText(buttonText);
            selectButton.SetRarity(pickedData.rarity);
            
            selectButton.Button.onClick.AddListener(() => OnNodeSelected(selectButton, pickedData, generatedValue));
            
            if(i == 0) OnNodeSelected(selectButton, pickedData, generatedValue);
        }
        
        CheckCompose();

        if(mathWave) _currentFormula = mathWave.GetFormula();
        UpdateFormulaPlaceholder();
    }

    private NodeData PickWeightedNode(List<NodeData> pool)
    {
        int totalWeight = 0;
        foreach (var node in pool)
        {
            totalWeight += GetRarityWeight(node.rarity);
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var node in pool)
        {
            currentWeight += GetRarityWeight(node.rarity);
            if (randomValue < currentWeight)
            {
                return node;
            }
        }

        return pool[0];
    }

    private int GetRarityWeight(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return commonWeight;
            case Rarity.Rare: return rareWeight;
            case Rarity.Epic: return epicWeight;
            default: return commonWeight;
        }
    }

    private void OnNodeSelected(SelectButton clickedButton, NodeData data, float constantValue)
    {
        _selectedNodeData = data;
        _selectedConstantValue = constantValue;

        foreach (var button in nodeButtons)
        {
            button.SetSelected(button == clickedButton);
        }
        
        CheckCompose();
        UpdateFormulaPlaceholder();
    }
    
    public void ApplySelection()
    {
        if (_selectedNodeData == null) return;

        // Utilise la ref 'mathWave' ou '_mathWave' selon ton setup, j'utilise mathWave ici comme dans ton snippet
        if (mathWave != null)
        {
            switch (_selectedNodeData.nodeType)
            {
                case NodeType.X: mathWave.AddNode(new NodeX(_selectedOp)); break;
                case NodeType.Sin: mathWave.AddNode(new NodeSin(_selectedOp)); break;
                case NodeType.Tan: mathWave.AddNode(new NodeTan(_selectedOp)); break;
                case NodeType.Asin: mathWave.AddNode(new NodeAsin(_selectedOp)); break;
                case NodeType.T: mathWave.AddNode(new NodeT(_selectedOp, mathWave)); break;
                case NodeType.Constant: mathWave.AddNode(new NodeConstant(_selectedConstantValue, _selectedOp)); break;
                case NodeType.Reciprocal: mathWave.AddNode(new NodeReciprocal(_selectedOp)); break;
                case NodeType.Cos: mathWave.AddNode(new NodeCos(_selectedOp)); break;
                case NodeType.Acos: mathWave.AddNode(new NodeAcos(_selectedOp)); break;
                case NodeType.Log: mathWave.AddNode(new NodeLog(_selectedOp)); break;
                case NodeType.Abs: mathWave.AddNode(new NodeAbs(_selectedOp)); break;
                case NodeType.Sign: mathWave.AddNode(new NodeSign(_selectedOp)); break;
                case NodeType.Atan: mathWave.AddNode(new NodeAtan(_selectedOp)); break;
                case NodeType.Round: mathWave.AddNode(new NodeRound(_selectedOp)); break;
            }
        }
        
        GameManager.Instance.StartWave();
        gameObject.SetActive(false);
    }

    private void CheckCompose()
    {
        if (_selectedOp != MathOp.Compose)
        {
            onComposeValid?.Invoke();
            return;
        }
        
        if(_selectedNodeData != null && !_selectedNodeData.canCompose)
            onComposeInvalid?.Invoke();
        else 
            onComposeValid?.Invoke();
    }

    private void UpdateFormulaPlaceholder()
    {
        if (_selectedNodeData == null) return;

        MathNode tempNode = null;
        
        switch (_selectedNodeData.nodeType)
        {
            case NodeType.X: tempNode = new NodeX(_selectedOp); break;
            case NodeType.Sin: tempNode = new NodeSin(_selectedOp); break;
            case NodeType.Tan: tempNode = new NodeTan(_selectedOp); break;
            case NodeType.Asin: tempNode = new NodeAsin(_selectedOp); break;
            case NodeType.T: tempNode = new NodeT(_selectedOp, mathWave); break;
            case NodeType.Constant: tempNode = new NodeConstant(_selectedConstantValue, _selectedOp); break;
            case NodeType.Reciprocal: tempNode = new NodeReciprocal(_selectedOp); break;
            case NodeType.Cos: tempNode = new NodeCos(_selectedOp); break;
            case NodeType.Acos: tempNode = new NodeAcos(_selectedOp); break;
            case NodeType.Log: tempNode = new NodeLog(_selectedOp); break;
            case NodeType.Abs: tempNode = new NodeAbs(_selectedOp); break;
            case NodeType.Sign: tempNode = new NodeSign(_selectedOp); break;
            case NodeType.Atan: tempNode = new NodeAtan(_selectedOp); break;
            case NodeType.Round: tempNode = new NodeRound(_selectedOp); break;
        }

        if (tempNode != null && _formulaPlaceholderTmp)
        {
            string preview;
            
            if (string.IsNullOrEmpty(_currentFormula) || _currentFormula == "0")
                preview = tempNode.Formula();
            else
                preview = tempNode.ApplyFormula(_currentFormula);

            _formulaPlaceholderTmp.SetText($"y = {preview}");
        }
    }
}