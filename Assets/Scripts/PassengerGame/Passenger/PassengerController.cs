using DG.Tweening;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerController : MonoBehaviour,IMatchable
{
    public Transform Root => transform;
    public List<GameKey> OccupiedGridKeys { get; private set; } = new();
    public GameKey SlotIndex { get; set; }

    public ItemColor ItemColor { get; set; }

    public bool IsMovable => throw new System.NotImplementedException();

    public bool IsMarkedForMatch { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private Renderer _renderer;

    [Header("Line Settings")]
    [SerializeField] private float _startOffset = 0.5f;
    [SerializeField] private float _spacing = 0.5f;

    public void Initialize(GameKey key, ItemColor itemColor)
    {
        SlotIndex = key;
        OccupiedGridKeys.Clear();
        OccupiedGridKeys.Add(key);
        ItemColor = itemColor; 

        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null)
            _renderer.material.color = ItemColorUtility.GetColor(itemColor);
    }


    public void MoveTo(GameKey key)
    {
        throw new System.NotImplementedException();
    }
    public void SpawnAtOffset(Vector3 finalPos, ItemColor color)
    {
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null)
            _renderer.material.color = ItemColorUtility.GetColor(color);

        transform.position = finalPos;
    }

    public void JumpToSeat(Transform seatTransform, PassengerMoveSettingsSO moveSettings, System.Action onComplete)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(moveSettings.scaleUp, moveSettings.scaleDuration).SetEase(moveSettings.scaleEase));
        seq.Append(transform.DOJump(
            seatTransform.position,
            moveSettings.jumpPower,
            moveSettings.jumpCount,
            moveSettings.jumpDuration
        ).SetEase(moveSettings.jumpEase));
        seq.Append(transform.DOScale(Vector3.one, moveSettings.scaleDuration).SetEase(Ease.InOutSine));

        seq.OnComplete(() =>
        {
            transform.SetParent(seatTransform, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            GetComponentInChildren<PassengerAnim>()?.PlayPassengerSit();

            onComplete?.Invoke();
        });
    }

    public bool MatchesWith(IMatchable other)
    {
        throw new System.NotImplementedException();
    }

    public void OnMatchedAsTarget()
    {
    }

    public void OnMatchedAsMover(GameKey targetSlot)
    {
    }

}
