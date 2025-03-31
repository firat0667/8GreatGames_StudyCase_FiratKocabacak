using DG.Tweening;
using GreatGames.CaseLib.Game;
using GreatGames.CaseLib.Pool;
using UnityEngine;




public class TurnPool : MonoBehaviour
{
    [SerializeField] private ParticleType _type;
    [SerializeField] private float _turnPoolTime;
    private GameObjectPool _pool;
    private void OnEnable()
    {
        if(_type==ParticleType.Merge) _pool = PoolManager.Instance.GetMergeVFXPool();
        else if (_type == ParticleType.Success) _pool = PoolManager.Instance.GetSuccessVFXPool();
        else if(_type == ParticleType.Hole) _pool = PoolManager.Instance.GetMergeHolePool();
        DOVirtual.DelayedCall(_turnPoolTime, () =>
        {
            if (_pool != null)
            {
                _pool.ReturnToPool(gameObject);
            }
        });
    }
}
