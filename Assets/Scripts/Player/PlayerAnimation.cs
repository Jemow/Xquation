public class PlayerAnimation : EntityAnimation
{
    
    protected override void MoveAnimation()
    {
        bool isInverted = entityMovement.Rb.linearVelocity.x < -0.1f && transform.localScale.x > 0.1f ||
                          entityMovement.Rb.linearVelocity.x > 0.1f && transform.localScale.x < -0.1f;
        animator.SetBool("Inverted", isInverted);
        
        base.MoveAnimation();
    }
}