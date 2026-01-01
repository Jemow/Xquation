using UnityEngine;

public class EntityAnimation : MonoBehaviour
{
    protected Animator animator;
    protected EntityMovement entityMovement;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        entityMovement = GetComponentInParent<EntityMovement>();
    }

    private void Update() => MoveAnimation();
    
    protected virtual void MoveAnimation() => animator.SetFloat("Speed", entityMovement.VelocityMagnitude);

    public void Death() => animator.SetTrigger("Death");

    public virtual void OnDeathAnimationEnded()
    {
        Destroy(transform.parent.gameObject);
    }
}