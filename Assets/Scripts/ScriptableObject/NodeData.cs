using UnityEngine;

public enum NodeType { X, Sin, Tan, Asin, Constant, T }

[CreateAssetMenu(fileName = "NodeData", menuName = "Scriptable Objects/NodeData")]
public class NodeData : ScriptableObject
{
    public string nodeName;
    public NodeType nodeType;
}