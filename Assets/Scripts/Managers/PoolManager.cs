using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Pool;
using UnityEngine;


public class PoolManager : FoundationSingleton<PoolManager>, IFoundationSingleton
{
    public bool Initialized { get; set; }
    [SerializeField] private GameObjectPool _mergeVfxPool;

    public GameObjectPool GetMergeVFXPool()
    {
       return _mergeVfxPool;
    }
}
