using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NodeSelectionUI : MonoBehaviour
{
    [SerializeField] private NodeData[] nodes;
    [SerializeField] private Button[] selectButtons;
    [SerializeField] private MathWave mathWave;
    
    public void GenerateNodes()
    {
        List<NodeData> randomNodes = nodes
            .OrderBy(x => Random.value)
            .Take(selectButtons.Length)
            .ToList();

        for (int i = 0; i < selectButtons.Length; i++)
        {
            NodeData data = randomNodes[i];
            Button btn = selectButtons[i];
            
            btn.onClick.RemoveAllListeners();
            btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = data.nodeName;
            btn.onClick.AddListener(() => SelectNode(data));
            btn.onClick.AddListener(() => GameManager.Instance.StartWave());
        }
    }
    
    private void SelectNode(NodeData data)
    {
        switch (data.nodeType)
        {
            case NodeType.X: mathWave.AddNode(new NodeX(MathOp.Add)); break;
            case NodeType.Sin: mathWave.AddNode(new NodeSin(MathOp.Add)); break;
            case NodeType.Tan: mathWave.AddNode(new NodeTan(MathOp.Add)); break;
            case NodeType.Asin: mathWave.AddNode(new NodeAsin(MathOp.Add)); break;
            case NodeType.T: mathWave.AddNode(new NodeT(MathOp.Add, mathWave)); break;
            case NodeType.Constant: mathWave.AddNode(new NodeConstant(data.defaultValue, MathOp.Add)); break;
        }
        
        gameObject.SetActive(false);
    }
}