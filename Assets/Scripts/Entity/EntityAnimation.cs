using UnityEngine;

public class EntityAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    
    private EntityMovement _entityMovement;
    
    private void Start() => _entityMovement = GetComponent<EntityMovement>();

    private void Update()
    {
        _animator.SetFloat("Speed", _entityMovement.VelocityMagnitude);
    }
}