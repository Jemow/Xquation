using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private MathProjectile projectilePrefab;
    [SerializeField] private Transform projectileSpawn;
    
    [Header("Parameters")]
    [SerializeField] private float fireRate = 0.2f;

    [Header("Beam Energy")]
    [SerializeField] private float maxBeamEnergy = 100f;
    [SerializeField] private float energyConsumption = 20f;
    [SerializeField] private float energyRecover = 15f;
    [SerializeField] private float recoverDelay = 1f;
    [SerializeField] private float beamShakeAmplitude = 10f;
    [SerializeField] private float beamShakeFrequency = 5f;
    
    [Header("UI")]
    [SerializeField] private Slider beamSlider;

    public static Transform PlayerTransform { get; private set; }
    
    private PlayerHealth _playerHealth;
    private EntityMovement _movement;
    private PlayerAnimation _playerAnimation;
    private MathWave _mw;
    private Camera _mainCamera;

    private Vector2 _playerSpawn;

    private bool _isFiring;
    private bool _canFire = true;
    private bool _funcProjectile;
    
    private float _currentBeamEnergy;
    private float _lastConsumTime;

    #region Init

    private void Awake() => PlayerTransform = transform;

    private void Start()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        _movement = GetComponent<EntityMovement>();
        _playerAnimation = GetComponentInChildren<PlayerAnimation>();
        _mw = GetComponentInChildren<MathWave>();
        _mainCamera = Camera.main;
        
        _playerSpawn = transform.position;
        _currentBeamEnergy = maxBeamEnergy;
    }

    #endregion

    private void Update()
    {
        UpdateXScale();
        HandleBeamEnergy();

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
        if (GameManager.Instance.IsPaused)
        {
            MathLine.IsAttacking = false;
            CameraShakeManager.Instance.StopShake();
            return;
        }
        
        if (context.started && _currentBeamEnergy > 0)
        {
            _mw.AttackDisplay();
            if(_mw.NodeCount == 0) return;
            
            MathLine.IsAttacking = true;
            CameraShakeManager.Instance.StartShake(beamShakeAmplitude, beamShakeFrequency);
        }
        else if (context.canceled)
        {
            _mw.NormalDisplay();
            if(_mw.NodeCount == 0) return;
            
            MathLine.IsAttacking = false;
            CameraShakeManager.Instance.StopShake();
        }
    }

    public void OnFuncMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _funcProjectile = !_funcProjectile;
            UIManager.Instance.UpdateProjectileImageColor(_funcProjectile);
        }
    }
    
    public void OnMove(InputAction.CallbackContext context) => _movement.Direction = context.ReadValue<Vector2>();

    public void OnPause(InputAction.CallbackContext context)
    {
        GameManager gameManager = GameManager.Instance;
        

        if (context.started && (gameManager.IsPaused || gameManager.IsPlaying))
            gameManager.PauseTrigger();
    }

    #endregion

    #region Fire

    private void Fire()
    {
        if(MathLine.IsAttacking || !GameManager.Instance.IsPlaying) return;
        
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 dir = (mouseWorldPos - projectileSpawn.position).normalized;

        MathProjectile projectile = ObjectPool.Instance.SpawnFromPool<MathProjectile>("Projectile", projectileSpawn.position, Quaternion.identity);
        projectile.Init(
            dir, 
            _mw.GetNodes(), 
            _mw.GetMinX(), 
            _mw.GetMathScale(),
            _mw.GetMaxY(),
            _mw.NodeCount != 0 && _funcProjectile
        );
    }

    private IEnumerator FireCoroutine()
    {
        _canFire = false;
        yield return new WaitForSeconds(fireRate);
        _canFire = true;
    }

    #endregion

    #region Beam Energy

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
            
            UpdateBeamSlider();
        }
        else
        {
            if (_currentBeamEnergy < maxBeamEnergy && Time.time > _lastConsumTime + recoverDelay)
            {
                _currentBeamEnergy += energyRecover * Time.deltaTime;
                _currentBeamEnergy = Mathf.Min(_currentBeamEnergy, maxBeamEnergy);
                
                UpdateBeamSlider();
            }
        }
    }
    
    private void UpdateBeamSlider() => beamSlider.value = _currentBeamEnergy / maxBeamEnergy;

    public void ResetEnergy()
    {
        _currentBeamEnergy = maxBeamEnergy;
        UpdateBeamSlider();
    }

    #endregion
    
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

    public void Respawn()
    {
        transform.position = _playerSpawn;
        
        _playerAnimation.Restart();
    }

    private void UpdateXScale()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(transform.position);
        
        float direction = mouseScreenPos.x >= screenPos.x ? 1f : -1f;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }
}