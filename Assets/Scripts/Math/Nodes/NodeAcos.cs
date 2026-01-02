using UnityEngine;

public class NodeAcos : MathNode
{
    public NodeAcos(MathOp op) : base(op) { }

    public override float Value(float x) => Mathf.Acos(x);

    public override string Formula() => "acos(x)";
}
