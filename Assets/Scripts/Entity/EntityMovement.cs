using System.Collections;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Adjust the speed of the entity")]
    [SerializeField] [Min(1)] private float speed = 5f;
    
    [Header("KnockBack")]
    [SerializeField] private float _knockBackForce = 5f;
    [SerializeField] private float _knockBackDuration = 0.5f;
    [Tooltip("Adjust the temporary damping when the entity is having a knock back")]
    [SerializeField] private float _knockBackDamping = 5f;
    
    public Vector2 Direction { get; set; }
    
    private Rigidbody2D _rb;
    
    private Coroutine _knockBackCoroutine;
    
    private float _initialLinearDamping;
    
    private bool _knockBack;

    private void Start() => _rb = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        if(!_knockBack)
            Move();
    }

    private void Move() => _rb.linearVelocity = Direction * speed;

    #region KnockBack

    public void KnockBack(Vector2 direction)
    {
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        _knockBackCoroutine = StartCoroutine(KnockBackCoroutine(direction, _knockBackForce, _knockBackDuration));
    }


    protected virtual void KnockBackStart(Vector2 direction, float force)
    {
        _rb.linearDamping = _knockBackDamping;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void KnockBackEnd()
    {
        _rb.linearDamping = _initialLinearDamping;
    }
    
    private IEnumerator KnockBackCoroutine(Vector2 direction, float force, float duration)
    {
        _knockBack = true;
        KnockBackStart(direction, force);
        yield return new WaitForSeconds(duration);
        KnockBackEnd();
        _knockBack = false;
    }

    #endregion
}