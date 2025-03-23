using GreatGames.CaseLib.Behavior.GameSystem;
using System.Collections.Generic;
using UnityEngine;

namespace GreatGames.CaseLib.Pool
{
    public enum PoolType { NormalPool, RandomGameObjectPool } 

    public class GameObjectPool : MonoBehaviour, IPool
    {
        public PoolType PoolType; 
        public GameObject Prefab; 
        public List<GameObject> Prefabs; 
        public int PoolCapacity = 30; 

        private readonly Queue<GameObject> pool = new();


        private void Start()
        {
            for (int i = 0; i < PoolCapacity; i++)
            {
                var go = InstantiatePrefab();
                go.SetActive(false);
                pool.Enqueue(go);
            }
        }

        private GameObject InstantiatePrefab()
        {
            if (PoolType == PoolType.NormalPool)
            {
                return Instantiate(Prefab);
            }
            else
            {
                int randomIndex = Random.Range(0, Prefabs.Count);
                return Instantiate(Prefabs[randomIndex]);
            }

        }

        public GameObject Retrieve()
        {
            if (pool.Count == 0 || pool.Count < 10)
            {
                return InstantiatePrefab();
            }

            var go = pool.Dequeue();
            if (go == null)
            {
                go = InstantiatePrefab();
            }
            go.SetActive(true);
            return go;
        }

        public void ReturnToPool(GameObject go)
        {
            if (pool.Count >= PoolCapacity)
            {
                Destroy(go);
            }
            else
            {
                go.SetActive(false);
                pool.Enqueue(go);
            }
        }
    }
}
