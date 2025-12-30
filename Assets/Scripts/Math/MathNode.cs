using UnityEngine;

public enum MathOp
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Power,
    Compose
}

public abstract class MathNode
{
    public MathOp Operation { get; }

    protected MathNode(MathOp op)
    {
        Operation = op;
    }

    public abstract float Value(float x);
    public abstract string Formula();

    public float Apply(float currentY, float x)
    {
        float v = Value(x);

        return Operation switch
        {
            MathOp.Add      => currentY + v,
            MathOp.Subtract => currentY - v,
            MathOp.Multiply => currentY * v,
            MathOp.Divide   => Mathf.Abs(v) > Mathf.Epsilon ? currentY / v : 0f,
            MathOp.Power    => Mathf.Pow(currentY, v),
            MathOp.Compose  => Value(currentY),
            _ => currentY
        };
    }

    public string ApplyFormula(string current)
    {
        return Operation switch
        {
            MathOp.Add      => $"{current} + {Formula()}",
            MathOp.Subtract => $"{current} - {Formula()}",
            MathOp.Multiply => $"({current}) * {Formula()}",
            MathOp.Divide   => $"({current}) / {Formula()}",
            MathOp.Power    => $"({current})<sup>{Formula()}</sup>",
            MathOp.Compose  => Formula().Replace("x", $"{current}"),
            _ => current
        };
    }
}