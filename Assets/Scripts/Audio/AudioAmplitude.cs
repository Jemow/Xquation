using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioAmplitude : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int sampleSize = 256;
    [SerializeField] private float sensitivity = 10f;
    
    public static AudioAmplitude Instance { get; private set; }

    private AudioSource _audioSource;
    private float[] _samples;

    public float Amplitude { get; private set; }

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        _audioSource = GetComponent<AudioSource>();
        _samples = new float[sampleSize];
    }

    private void Update()
    {
        _audioSource.GetOutputData(_samples, 0);

        float sum = 0f;
        for (int i = 0; i < sampleSize; i++)
            sum += _samples[i] * _samples[i];

        Amplitude = Mathf.Sqrt(sum / sampleSize) * sensitivity;
    }
}