/***************************************************************
 * File        : ObjectPool.cs
 * Author      : Jemoel Ablay
 * Date        : 08-01-2025
 * Version     : 1.1.0
 * Project     : Third Person Controller
 * 
 * Description:
 * This script provides a simple and flexible object pooling system 
 * for any type of GameObject. It helps to optimize performance by 
 * reusing objects instead of creating and destroying them repeatedly.
 *
 * Usage:
 * 1. Create an empty GameObject in your scene and attach this script to it.
 * 2. In the Inspector, configure the pools:
 *    - Add a tag to identify each pool.
 *    - Assign a prefab to use for that pool.
 *    - Specify the number of objects to instantiate initially.
 * 3. To spawn an object, call the `SpawnFromPool` method with the desired tag, 
 *    position, and rotation.
 *
 * Requirements:
 * - All prefabs used in the pools must be assigned in the Inspector.
 **************************************************************/

using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPool Instance;

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> _poolDictionary;

    private void Awake()
    {
        if (Instance && Instance != this) Destroy(gameObject);
        else Instance = this;

        _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = _poolDictionary[tag].Dequeue();
        
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        
        objectToSpawn.SetActive(true);
        
        _poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public T SpawnFromPool<T>(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject obj = SpawnFromPool(tag, position, rotation);
    
        T component = obj.GetComponent<T>();
    
        return component;
    }
}