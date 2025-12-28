using UnityEngine;

public class NodeT : MathNode
{
    private readonly MathWave _wave;

    public NodeT(MathOp op, MathWave wave) : base(op)
    {
        _wave = wave;
    }

    public override float Value(float x)
    {
        float L = Mathf.PI * 2f;
        return Mathf.PingPong(_wave.TimeValue, L) - L * 0.5f;
    }

    public override string Formula() => "t";
}