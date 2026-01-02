using UnityEngine;

public class NodeCos : MathNode
{
    public NodeCos(MathOp op) : base(op) { }

    public override float Value(float x) => Mathf.Cos(x);

    public override string Formula() => "cos(x)";
}
