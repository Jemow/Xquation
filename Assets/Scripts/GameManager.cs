using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyController _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    
    [Header("Parameters")]
    [SerializeField] [Min(0.1f)] private float _spawnInterval = 1f;
    
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        InvokeRepeating(nameof(SpawnEnemy), _spawnInterval, _spawnInterval);
    }

    private void SpawnEnemy() => Instantiate(_enemyPrefab, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
}
