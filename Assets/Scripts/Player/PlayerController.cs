using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private MathProjectile projectilePrefab;
    
    [Header("Parameters")]
    [SerializeField] private float fireRate = 0.2f;

    [Header("Beam Energy")]
    [SerializeField] private float maxBeamEnergy = 100f;
    [SerializeField] private float energyConsumption = 20f;
    [SerializeField] private float energyRecover = 15f;
    [SerializeField] private float recoverDelay = 1f;

    public static Transform PlayerTransform { get; private set; }
    
    private PlayerHealth _playerHealth;
    private EntityMovement _movement;
    private MathWave _mw;
    private Camera _mainCamera;

    private bool _isFiring;
    private bool _canFire = true;
    private bool _funcProjectile;
    
    private float _currentBeamEnergy;
    private float _lastConsumTime;

    private void Start()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        _movement = GetComponent<EntityMovement>();
        _mw = GetComponent<MathWave>();
        _mainCamera = Camera.main;
        
        PlayerTransform = transform;
        _currentBeamEnergy = maxBeamEnergy;
    }

    private void Update()
    {
        HandleBeamEnergy();

        if(!_isFiring || !_canFire) return;

        StartCoroutine(FireCoroutine());
        Fire();
    }

    private void HandleBeamEnergy()
    {
        if(_mw.NodeCount == 0) return;
        
        if (MathLine.IsAttacking)
        {
            _currentBeamEnergy -= energyConsumption * Time.deltaTime;
            _lastConsumTime = Time.time;
            
            if (_currentBeamEnergy <= 0)
            {
                _currentBeamEnergy = 0;
                MathLine.IsAttacking = false;
            }
        }
        else
        {
            if (_currentBeamEnergy < maxBeamEnergy && Time.time > _lastConsumTime + recoverDelay)
            {
                _currentBeamEnergy += energyRecover * Time.deltaTime;
                _currentBeamEnergy = Mathf.Min(_currentBeamEnergy, maxBeamEnergy);
            }
        }
    }

    #region Inputs

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if(context.started) _isFiring = true;
        else if(context.canceled) _isFiring = false;
    }

    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        if (context.started && _currentBeamEnergy > 0) 
            MathLine.IsAttacking = true;
        else if (context.canceled) 
            MathLine.IsAttacking = false;
    }

    public void OnFuncMode(InputAction.CallbackContext context)
    {
        if(context.started) _funcProjectile = true;
        else if(context.canceled) _funcProjectile = false;
    }
    
    public void OnMove(InputAction.CallbackContext context) => _movement.Direction = context.ReadValue<Vector2>();

    #endregion

    private void Fire()
    {
        if(MathLine.IsAttacking) return;
        
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 dir = (mouseWorldPos - transform.position).normalized;

        MathProjectile p = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        p.Init(
            dir, 
            _mw.GetNodes(), 
            _mw.GetMinX(), 
            _mw.GetMathScale(),
            _mw.NodeCount != 0 && _funcProjectile
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
        if(!other.gameObject.CompareTag("Enemy")) return;

        if (other.gameObject.TryGetComponent(out EnemyController enemyController))
        {
            _playerHealth.ChangeHealth(-enemyController.Damage);
            Vector2 direction = (transform.position - other.transform.position);
            direction.Normalize();
            _movement.KnockBack(direction);
        }
    }
}