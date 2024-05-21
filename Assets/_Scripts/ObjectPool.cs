using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [SerializeField]
    private int poolSize = 10;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public GameObject GetObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            InitializePool(prefab);

        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);

        GameObject ObjectToGet = poolDictionary[prefab].Dequeue();
        ObjectToGet.SetActive(true);
        ObjectToGet.transform.parent = null;

        return ObjectToGet;
    }

    public void ReturnObject(GameObject objectToReturn, float delay = 0.001f)
    {
        StartCoroutine(DelayReturn(delay, objectToReturn));
    }

    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(objectToReturn);
    }

    private void ReturnToPool(GameObject objectToReturn)
    {
        GameObject origianlPrefab = objectToReturn.GetComponent<PooledObject>().OriginalPrefab;
        objectToReturn.SetActive(false);
        objectToReturn.transform.parent = transform;
        poolDictionary[origianlPrefab].Enqueue(objectToReturn);
    }

    private void InitializePool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().OriginalPrefab = prefab;
        newObject.SetActive(false);
        poolDictionary[prefab].Enqueue(newObject);
    }
}
