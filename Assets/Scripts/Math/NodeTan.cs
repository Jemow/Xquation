using UnityEngine;

public class NodeTan : MathNode
{
    public NodeTan(MathOp op) : base(op) {}
    
    public override float Value(float x) => Mathf.Tan(x);

    public override string Formula() => "tan(x)";
}
