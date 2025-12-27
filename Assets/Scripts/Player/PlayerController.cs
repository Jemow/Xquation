using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private MathWave _mw;

    private void Start()
    {
        _mw = GetComponent<MathWave>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // if (context.started) _mw.ShootWave();
    }
}
