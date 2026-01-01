using UnityEngine;

public class EntityAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Animator animator;
    
    protected EntityMovement entityMovement;
    
    private void Start() => entityMovement = GetComponent<EntityMovement>();

    private void Update() => MoveAnimation();
    
    protected virtual void MoveAnimation() => animator.SetFloat("Speed", entityMovement.VelocityMagnitude);
}