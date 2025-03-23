using GreatGames.CaseLib.Game;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Pool;
using UnityEngine;

public class VFXManager : FoundationSingleton<VFXManager>, IFoundationSingleton
{
    public bool Initialized { get; set; }
    [SerializeField] private GameObjectPool _pool;
    [SerializeField] private VFXSpawner _mergeVFXSpawner;
    [SerializeField] private VFXSpawner _levelDoneVFXSpawner;
    [SerializeField] private Transform _levelSuccesParticlePos;

    public void PlayMergeParticle(Vector3 transformSpawner)
    {
        _mergeVFXSpawner.SpawnVFX(transformSpawner);
    }
    public void PlayLevelDoneParticle()
    {
       // _levelDoneVFXSpawner.SpawnVFX(_levelSuccesParticlePos.position);
    }
}
