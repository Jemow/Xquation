using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Adjust the speed of the entity")]
    [SerializeField] [Min(1)] private float speed = 5f;
    
    public Vector2 Direction { get; set; }
    
    private Rigidbody2D _rb;

    private void Start() => _rb = GetComponent<Rigidbody2D>();

    private void FixedUpdate() => Move();

    private void Move() => _rb.linearVelocity = Direction * speed;
}