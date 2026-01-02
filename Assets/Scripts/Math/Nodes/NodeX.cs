public class NodeX : MathNode
{
    public NodeX(MathOp op) : base(op) {}

    public override float Value(float x) => x;

    public override string Formula() => "x";
}