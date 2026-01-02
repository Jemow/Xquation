using UnityEngine;

public class NodeLog : MathNode
{
    public NodeLog(MathOp op) : base(op) { }

    public override float Value(float x) => Mathf.Log(x);

    public override string Formula() => "log(x)";
}
