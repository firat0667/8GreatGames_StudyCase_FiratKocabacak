using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using UnityEngine;

public class PassengerController : MonoBehaviour, ISlotItem
{
    public Transform Root => transform;
    public List<GameKey> OccupiedGridKeys { get; private set; } = new();
    public GameKey SlotIndex { get; set; }

    GameObject ISlotItem.Root => throw new System.NotImplementedException();

    public ItemColor ItemColor { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private Renderer _renderer;

    [Header("Line Settings")]
    [SerializeField] private float _startOffset = 0.5f;
    [SerializeField] private float _spacing = 0.5f;

    public void Initialize(GameKey key, Color color)
    {
        SlotIndex = key;
        OccupiedGridKeys.Clear();
        OccupiedGridKeys.Add(key);

        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null)
            _renderer.material.color = color;
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
}
