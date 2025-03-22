using GreatGames.CaseLib.Game;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : FoundationSingleton<VFXManager>, IFoundationSingleton
{
    public bool Initialized { get; set; }
    [SerializeField] private GameObjectPool _pool;
    [SerializeField] private VFXSpawner _vfxSpawner;

    public void PlayMergeParticle(Transform transformSpawner)
    {
        _vfxSpawner.SpawnVFX(transformSpawner);
    }

}
