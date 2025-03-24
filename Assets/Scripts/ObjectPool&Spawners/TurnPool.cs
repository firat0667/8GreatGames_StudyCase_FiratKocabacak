using DG.Tweening;
using GreatGames.CaseLib.Pool;
using UnityEngine;


public class TurnPool : MonoBehaviour
{
    [SerializeField] private float _turnPoolTime;
    private GameObjectPool _pool;

    private void OnEnable()
    {
        _pool = PoolManager.Instance.GetMergeVFXPool();
        DOVirtual.DelayedCall(_turnPoolTime, () =>
        {
            if (_pool != null)
            {
                _pool.ReturnToPool(gameObject);
            }
        });
    }
}
