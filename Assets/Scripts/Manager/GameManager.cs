using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] _spawnPoints;
    
    [Header("Parameters")]
    [SerializeField] Wave[] _waves;

    [Header("Events")] 
    [SerializeField] private UnityEvent _onWaveStarted;
    [SerializeField] private UnityEvent _onWaveEnded;
    
    [Serializable]
    struct Wave
    {
        public EnemyController[] enemies;
        public int enemyNumber;
        public float spawnInterval;
    }

    public int WaveIndex => _waveIndex;

    private EnemyManager _enemyManager;
    
    private int _waveIndex;
    private int _currentSpawnCount;
    
    public static GameManager Instance { get; private set; }
    
    public bool Spawning { get; private set; }

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        _enemyManager = EnemyManager.Instance;
    }

    private void Start() => StartWave();

    #region Wave

    public void StartWave()
    {
        Spawning = true;
        StartCoroutine(SpawnRoutine());
        
        _onWaveStarted?.Invoke();
    }

    public void StopWave()
    {
        _waveIndex++;
        _currentSpawnCount = 0;

        if (_waveIndex >= _waves.Length)
        {
            Debug.LogWarning("Finish Wave");
        }
        else _onWaveEnded?.Invoke();
    }

    #endregion

    private void SpawnEnemy()
    {
        if(!Spawning) return;
        
        EnemyController[] enemyList = _waves[_waveIndex].enemies;
        EnemyController enemyController = Instantiate(enemyList[Random.Range(0, enemyList.Length)], _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity, _enemyManager.transform);
        _enemyManager.AddController(enemyController);
        
        _currentSpawnCount++;
        
        if(_currentSpawnCount >= _waves[_waveIndex].enemyNumber) Spawning = false;
    }

    private IEnumerator SpawnRoutine()
    {
        while (Spawning)
        {
            yield return new WaitForSeconds(_waves[_waveIndex].spawnInterval);
            SpawnEnemy();
        }
    }

    public void GameOver()
    {
        Spawning = false;
        _enemyManager.Clear();
    }
}
