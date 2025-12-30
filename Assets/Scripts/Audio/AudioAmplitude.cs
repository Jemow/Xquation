using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioAmplitude : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("On remet 256 ou 512 pour mieux capter les basses (Drums)")]
    [SerializeField] private int sampleSize = 256; 
    [SerializeField] private float sensitivity = 10f;
    [Tooltip("On check plus souvent (0.05 = 20 fois/sec) pour ne pas rater le beat")]
    [SerializeField] private float updateInterval = 0.05f; 
    
    [Header("Punchiness")]
    [Tooltip("Vitesse à laquelle la vague redescend après un coup (plus c'est bas, plus ça reste en l'air)")]
    [SerializeField] private float decaySpeed = 5f;

    public static AudioAmplitude Instance { get; private set; }

    private AudioSource _audioSource;
    private float[] _samples;
    private float _timer;
    private float _currentAmplitude; // La valeur brute calculée
    
    // La valeur finale que les autres scripts utilisent
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
        Amplitude = _currentAmplitude > Amplitude ? _currentAmplitude : // INSTANT ATTACK
            Mathf.Lerp(Amplitude, _currentAmplitude, Time.deltaTime * decaySpeed); // SLOW DECAY

        if (!_audioSource.clip || !_audioSource.isPlaying)
        {
            _currentAmplitude = 0f;
            return;
        }
        
        // --- 2. RECUPERATION DES DONNEES ---
        _timer += Time.deltaTime;
        if (_timer < updateInterval) return;
        _timer = 0f;

        int currentSamplePos = _audioSource.timeSamples;
        int totalSamples = _audioSource.clip.samples;
        
        if (currentSamplePos + sampleSize >= totalSamples) return; 
        
        _audioSource.clip.GetData(_samples, currentSamplePos);

        float sum = 0f;
        for (int i = 0; i < sampleSize; i += 4)
        {
            float val = _samples[i];
            sum += val * val;
        }
        
        _currentAmplitude = Mathf.Sqrt(sum / (sampleSize / 4f)) * sensitivity;
    }
}