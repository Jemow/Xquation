using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private readonly List<EnemyController> _enemies = new();
    
    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    
    public void AddController(EnemyController controller) => _enemies.Add(controller);

    public void RemoveController(EnemyController controller)
    {
        if(_enemies.Count == 0) return;
        
        GameManager instance = GameManager.Instance;
        
        _enemies.Remove(controller);
        
        if(_enemies.Count == 0 && !instance.Spawning) instance.StopWave();
    }

    public void Clear()
    {
        foreach (EnemyController enemy in _enemies)
        {
            enemy.gameObject.SetActive(false);
        }
        
        _enemies.Clear();
    }
}
