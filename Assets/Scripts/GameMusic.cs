using UnityEngine;

public class GameMusic : MonoBehaviour
{
    public AudioClip introClip;
    public AudioClip loopClip;
    private AudioSource _sourceIntro;
    private AudioSource _sourceLoop;

    private void Start()
    {
        _sourceIntro = gameObject.AddComponent<AudioSource>();
        _sourceLoop = gameObject.AddComponent<AudioSource>();

        double introDuration = (double)introClip.samples / introClip.frequency;
        double startTime = AudioSettings.dspTime + 0.1;

        _sourceIntro.clip = introClip;
        _sourceIntro.PlayScheduled(startTime);

        _sourceLoop.clip = loopClip;
        _sourceLoop.loop = true;
        _sourceLoop.PlayScheduled(startTime + introDuration);
    }
}