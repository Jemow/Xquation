using UnityEngine;

public class EnemyController : MonoBehaviour
{
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
