public class NodeConstant : MathNode
{
    private readonly float _value;

    public NodeConstant(float value, MathOp op) : base(op)
    {
        _value = value;
    }

    public override float Value(float x, float t) => _value;
    public override string Formula() => _value.ToString();
}