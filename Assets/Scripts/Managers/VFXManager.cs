using GreatGames.CaseLib.Game;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Pool;
using UnityEngine;

public class VFXManager : FoundationSingleton<VFXManager>, IFoundationSingleton
{
    public bool Initialized { get; set; }
    [SerializeField] private GameObjectPool _pool;
    [SerializeField] private VFXSpawner _vfxSpawner;

    public void PlayMergeParticle(Vector3 transformSpawner)
    {
        _vfxSpawner.SpawnVFX(transformSpawner);
    }

}
