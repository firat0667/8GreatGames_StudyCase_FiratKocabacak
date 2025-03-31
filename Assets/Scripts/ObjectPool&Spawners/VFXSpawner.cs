using UnityEngine;

namespace GreatGames.CaseLib.Game
{
    public enum ParticleType
    {
        Merge,
        Success,
        Hole
    }
    public  class VFXSpawner : Spawner
    {
        public void SpawnMergeVFX(Vector3 position)
        {
            var obj = _spawnPool.Retrieve();
            obj.transform.position = position;

            var ps = obj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
        public void SpawnHole(Vector3 position)
        {
            var obj = _spawnPool.Retrieve();
            obj.transform.position = position;

            var ps = obj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
        public void SpawnSuccessVFX(Transform transform, Vector3 position)
        {
            var obj = _spawnPool.Retrieve();
            obj.transform.parent = transform;
            obj.transform.localPosition = position;
            obj.transform.localEulerAngles = new Vector3(0, 0, 0);
            var ps = obj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
    }
}
