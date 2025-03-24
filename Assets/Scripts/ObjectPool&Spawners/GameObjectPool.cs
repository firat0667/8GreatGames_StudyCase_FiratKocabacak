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

        private readonly Queue<GameObject> _pool = new();


        private void Start()
        {
            for (int i = 0; i < PoolCapacity; i++)
            {
                var go = InstantiatePrefab();
                go.SetActive(false);
                _pool.Enqueue(go);
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
            GameObject go = null;

            if (_pool.Count > 0)
            {
                go = _pool.Dequeue();
            }
            else
            {
                go = Instantiate(Prefab, transform);
            }

            go.SetActive(true);
            return go;
        }

        public void ReturnToPool(GameObject go)
        {
            if (_pool.Count > PoolCapacity)
            {
                Destroy(go);
            }
            else
            {
                go.SetActive(false);
                _pool.Enqueue(go);
            }
        }
    }
}
