using GreatGames.CaseLib.Game;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Pool;
using UnityEngine;

public class VFXManager : FoundationSingleton<VFXManager>, IFoundationSingleton
{
    public bool Initialized { get; set; }
    [SerializeField] private VFXSpawner _mergeVFXSpawner;
    [SerializeField] private VFXSpawner _levelDoneVFXSpawner;
    [SerializeField] private VFXSpawner _holeMergeVFXSpawner;
    [SerializeField] private Transform _levelSuccesParticlePos;

    public void PlayMergeParticle(Vector3 transformSpawner)
    {
        _mergeVFXSpawner.SpawnMergeVFX(transformSpawner);
    }
    public void PlayLevelDoneParticle()
    {
        _levelDoneVFXSpawner.SpawnSuccessVFX(_levelSuccesParticlePos, new Vector3(0,0,0));
    }
    public void HoleMergeSpawner(Vector3 transformSpawner)
    {
        _holeMergeVFXSpawner.SpawnHole(transformSpawner);
    }
}
