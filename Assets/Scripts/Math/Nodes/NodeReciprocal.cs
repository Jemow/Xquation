public class NodeReciprocal : MathNode
{
    public NodeReciprocal(MathOp op) : base(op) { }
    
    public override float Value(float x) => 1f / x;
    public override string Formula() => "1/x";
}
