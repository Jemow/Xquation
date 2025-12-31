using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NodeSelectionUI : MonoBehaviour
{
    [System.Serializable]
    public struct OpButtonSetup
    {
        public string name; 
        public MathOp op;
        public SelectButton button;
    }

    [Header("Data")]
    [SerializeField] private NodeData[] nodes;
    [SerializeField] private MathWave mathWave;

    [Header("UI References")]
    [SerializeField] private SelectButton[] nodeButtons;
    [SerializeField] private OpButtonSetup[] operationButtons;

    private NodeData _selectedNodeData;
    private MathOp _selectedOp;

    private void Start()
    {
        InitOperationButtons();
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
    }

    public void GenerateNodes()
    {
        List<NodeData> randomNodes = nodes
            .OrderBy(x => UnityEngine.Random.value)
            .Take(nodeButtons.Length)
            .ToList();

        _selectedNodeData = null;

        for (int i = 0; i < nodeButtons.Length; i++)
        {
            NodeData data = randomNodes[i];
            SelectButton selectButton = nodeButtons[i];
            
            selectButton.SetSelected(false);
            selectButton.Button.onClick.RemoveAllListeners();
            selectButton.Tmp.SetText(data.nodeName);
            
            selectButton.Button.onClick.AddListener(() => OnNodeSelected(selectButton, data));
            
            if(i == 0) OnNodeSelected(selectButton, data);
        }
    }

    private void OnNodeSelected(SelectButton clickedButton, NodeData data)
    {
        _selectedNodeData = data;

        foreach (var button in nodeButtons)
        {
            button.SetSelected(button == clickedButton);
        }
    }
    
    public void ApplySelection()
    {
        if (_selectedNodeData == null) return;

        switch (_selectedNodeData.nodeType)
        {
            case NodeType.X: mathWave.AddNode(new NodeX(_selectedOp)); break;
            case NodeType.Sin: mathWave.AddNode(new NodeSin(_selectedOp)); break;
            case NodeType.Tan: mathWave.AddNode(new NodeTan(_selectedOp)); break;
            case NodeType.Asin: mathWave.AddNode(new NodeAsin(_selectedOp)); break;
            case NodeType.T: mathWave.AddNode(new NodeT(_selectedOp, mathWave)); break;
            case NodeType.Constant: mathWave.AddNode(new NodeConstant(_selectedNodeData.defaultValue, _selectedOp)); break;
        }
        
        GameManager.Instance.StartWave();
        gameObject.SetActive(false);
    }
}