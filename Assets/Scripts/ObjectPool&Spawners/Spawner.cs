
using GreatGames.CaseLib.Pool;
using UnityEngine;

namespace GreatGames.CaseLib.Game
{
    public abstract class BatchSpawner : Spawner
    {
        public void BatchSpawn(int batchCount)
        {
            for (int i = 0; i < batchCount; i++)
            {
                Spawn();
            }
        }
    }

    public abstract class Spawner : MonoBehaviour
    {
        [SerializeField] protected GameObjectPool _spawnPool;
         protected Transform _spawnTransform;

        protected GameObject Spawn()
        {
            var go = _spawnPool.Retrieve();
            go.transform.SetPositionAndRotation(_spawnTransform.position, _spawnTransform.rotation);
            return go;
        }
        protected GameObject Spawn(Vector3 _selfSpawnTransform)
        {
            var go = _spawnPool.Retrieve();
            go.transform.SetPositionAndRotation(_selfSpawnTransform, Quaternion.identity);
            return go;
        }

    }
}