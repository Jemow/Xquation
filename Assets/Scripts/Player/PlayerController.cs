using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private MathProjectile projectilePrefab;
    
    [Header("Parameters")]
    [SerializeField] private float fireRate = 0.2f;

    public static Transform PlayerTransform { get; private set; }
    
    private EntityMovement _movement;
    private MathWave _mw;
    private Camera _mainCamera;

    private bool _isFiring;
    private bool _canFire = true;

    private void Start()
    {
        _movement = GetComponent<EntityMovement>();
        _mw = GetComponent<MathWave>();
        _mainCamera = Camera.main;
        
        PlayerTransform = transform;
    }

    private void Update()
    {
        if(!_isFiring || !_canFire) return;

        StartCoroutine(FireCoroutine());
        Fire();
    }

    #region Inputs

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if(context.started) _isFiring = true;
        else if(context.canceled) _isFiring = false;
    }

    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        if(context.started) MathLine.IsAttacking = true;
        else if(context.canceled) MathLine.IsAttacking = false;
    }
    
    public void OnMove(InputAction.CallbackContext context) => _movement.Direction = context.ReadValue<Vector2>();

    #endregion

    private void Fire()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 dir = (mouseWorldPos - transform.position).normalized;

        MathProjectile p = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        p.Init(
            dir, 
            _mw.GetNodes(), 
            _mw.GetMinX(), 
            _mw.GetMaxX(), 
            _mw.GetMathScale()
        );
    }

    private IEnumerator FireCoroutine()
    {
        _canFire = false;
        yield return new WaitForSeconds(fireRate);
        _canFire = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Enemy"))
            Debug.Log("Player is dead");
    }
}