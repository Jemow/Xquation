using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TextMeshProUGUI _waveCountTmp;
    [SerializeField] private Image projectileImg;

    [Header("Parameters")]
    [SerializeField] private Color projectileColor;
    [SerializeField] private Color funcProjectileColor;
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
    
    public void UpdateProjectileImageColor(bool func) => projectileImg.color = func ? funcProjectileColor : projectileColor;
}