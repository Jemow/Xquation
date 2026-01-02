using UnityEngine;

public class GameMusic : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float intiVolume = 0.5f;
    public AudioClip introClip;
    public AudioClip loopClip;
    private AudioSource _sourceIntro;
    private AudioSource _sourceLoop;

    private void Start()
    {
        _sourceIntro = gameObject.AddComponent<AudioSource>();
        _sourceLoop = gameObject.AddComponent<AudioSource>();

        _sourceIntro.volume = intiVolume;
        _sourceLoop.volume = intiVolume;

        double introDuration = (double)introClip.samples / introClip.frequency;
        double startTime = AudioSettings.dspTime + 0.1;

        _sourceIntro.clip = introClip;
        _sourceIntro.PlayScheduled(startTime);

        _sourceLoop.clip = loopClip;
        _sourceLoop.loop = true;
        _sourceLoop.PlayScheduled(startTime + introDuration);
    }
}