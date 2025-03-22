using UnityEngine;

namespace GreatGames.CaseLib.Game
{
    public class VFXSpawner : Spawner
    {
        public void SpawnObject(Transform spawnTransform)
        {
            GameObject spawnObj = _spawnPool.Retrieve();

            spawnObj.transform.position = spawnTransform.position;
            spawnObj.SetActive(true);
        }
    }
}
