using UnityEngine;

public class NodeTan : MathNode
{
    public NodeTan(MathOp op) : base(op) {}
    
    public override float Value(float x, float t) => Mathf.Tan(x + t);

    public override string Formula() => "tan(x)";
}
