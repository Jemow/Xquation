using UnityEngine;

public enum NodeType { X, Sin, Tan, Asin, Constant, T }
public enum Rarity { Common, Rare, Epic }

[CreateAssetMenu(fileName = "NodeData", menuName = "Scriptable Objects/NodeData")]
public class NodeData : ScriptableObject
{
    public string nodeName;
    public NodeType nodeType;
    public Rarity rarity;
    public bool canCompose;
}