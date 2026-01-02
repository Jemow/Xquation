using UnityEngine;

public class NodeSign : MathNode
{
    public NodeSign(MathOp op) : base(op) { }

    public override float Value(float x) => Mathf.Sin(x);

    public override string Formula() => "sign(x)";
}
