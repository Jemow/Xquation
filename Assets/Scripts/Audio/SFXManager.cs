using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int defaultPoolSize = 5;
    public static SFXManager Instance { get; private set; }

    private readonly List<AudioSource> _pool = new();
    private AudioSource _beamSource;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        for (int i = 0; i < defaultPoolSize; i++)
            CreateNewSource();
        
        _beamSource = gameObject.AddComponent<AudioSource>();
    }

    private AudioSource CreateNewSource()
    {
        GameObject sourceObject = new GameObject("PooledAudioSource");
        sourceObject.transform.SetParent(transform);
        
        AudioSource newSource = sourceObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        
        _pool.Add(newSource);
        return newSource;
    }

    public void PlaySFX(AudioClip clip, Vector2 position, float volume = 1.0f)
    {
        if (!clip ) return;

        AudioSource sourceToUse = null;

        foreach (AudioSource source in _pool)
        {
            if (!source.isPlaying)
            {
                sourceToUse = source;
                break;
            }
        }

        if (!sourceToUse) sourceToUse = CreateNewSource();

        sourceToUse.transform.position = position;
        sourceToUse.volume = volume;
        sourceToUse.spatialBlend = 1f;
        sourceToUse.PlayOneShot(clip);
    }

    public void PlayBeamSFX(AudioClip clip, float volume = 1f)
    {
        if (!clip) return;

        if (_beamSource.isPlaying) return;

        _beamSource.clip = clip;
        _beamSource.volume = volume;
        _beamSource.loop = true;
        _beamSource.spatialBlend = 1f;
        _beamSource.Play();
    }

    public void StopBeamSFX()
    {
        if (_beamSource.isPlaying)
            _beamSource.Stop();
    }
}