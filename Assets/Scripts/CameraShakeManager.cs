/***************************************************************
 * File : CameraShakeManager.cs
 * Author : Jemoel Ablay
 * Date : 20-01-2025
 * Version : 1.0.0
 * Project : Any
 * 
 * Description:
 * This script allows dynamic camera shake effects using Cinemachine.
 * It provides a simple interface to trigger camera shakes with customizable
 * amplitude, frequency, and duration. The shake effect automatically resets
 * after the specified duration.
 *
 * Usage:
 * - Attach this script to an empty GameObject in your scene.
 * - Assign a Cinemachine Virtual Camera with a CinemachineBasicMultiChannelPerlin
 *   component in the inspector.
 * - Call Shake(amplitude, frequency, duration) from other scripts using the static Instance.
 *
 * Example:
 * CameraShakeManager.Instance.Shake(2.0f, 1.0f, 0.5f);
 **************************************************************/

using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The cinemachine to manage shakes")]
    [SerializeField] private CinemachineBasicMultiChannelPerlin _cameraShake;
    
    public static CameraShakeManager Instance { get; private set; }

    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;

        ResetShake();
    }

    public void Shake(float amplitude, float frequency, float duration)
    {
        _cameraShake.AmplitudeGain = amplitude;
        _cameraShake.FrequencyGain = frequency;
        
        if(_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeTime(duration));
    }

    private void ResetShake()
    {
        _cameraShake.AmplitudeGain = 0;
        _cameraShake.FrequencyGain = 0;
    }

    private IEnumerator ShakeTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        ResetShake();
    }
}