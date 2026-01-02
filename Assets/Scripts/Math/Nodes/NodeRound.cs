using UnityEngine;

public class NodeRound : MathNode
{
    public NodeRound(MathOp op) : base(op) { }

    public override float Value(float x) => Mathf.Round(x);

    public override string Formula() => "round(x)";
}
