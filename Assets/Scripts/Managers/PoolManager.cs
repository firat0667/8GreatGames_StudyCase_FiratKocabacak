using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Pool;
using UnityEngine;


public class PoolManager : FoundationSingleton<PoolManager>, IFoundationSingleton
{
    public bool Initialized { get; set; }
    [SerializeField] private GameObjectPool _mergeVfxPool;
    [SerializeField] private GameObjectPool _succesVfxPool;

    public GameObjectPool GetMergeVFXPool()
    {
       return _mergeVfxPool;
    }
    public GameObjectPool GetSuccessVFXPool()
    {
        return _succesVfxPool;
    }
}
