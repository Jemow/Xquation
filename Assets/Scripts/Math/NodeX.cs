public class NodeX : MathNode
{
    public NodeX(MathOp op) : base(op) {}

    public override float Value(float x, float t) => x;

    public override string Formula() => "x";
}