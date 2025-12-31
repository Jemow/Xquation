using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] [Min(1)] private int damage = 1;
    
    public int Damage => damage;
    
    private EntityMovement _movement;
    private Transform _playerTransform;

    private void Start()
    {
        _movement = GetComponent<EntityMovement>();
        _playerTransform = PlayerController.PlayerTransform;
    }

    private void Update()
    {
        Vector2 direction = _playerTransform.position - transform.position;
        direction.Normalize();
        _movement.Direction = direction;
    }
}
