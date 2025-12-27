using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Adjust the speed of the player")]
    [SerializeField] [Min(1)] private float _speed = 5f;
    
    private Rigidbody2D _rb;
    
    private Vector2 _direction;

    private void Start() => _rb = GetComponent<Rigidbody2D>();

    private void FixedUpdate() => Move();

    private void Move() => _rb.linearVelocity = _direction * _speed;

    public void OnMove(InputAction.CallbackContext context) => _direction = context.ReadValue<Vector2>();
}