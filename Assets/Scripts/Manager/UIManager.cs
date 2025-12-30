using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TextMeshProUGUI _waveCountTmp;
    public static UIManager Instance { get; private set; }
    
    private GameManager _gameManager;

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public void UpdateWaveCount() => _waveCountTmp.SetText($"Wave {_gameManager.WaveIndex + 1}");
}