using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Projectile projectilePrefab;
    
    private MathWave _mw;

    private void Start()
    {
        _mw = GetComponent<MathWave>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!context.started) return;
        
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 dir = (mouseWorldPos - transform.position).normalized;

        Projectile p = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        p.Init(dir);
    }
}