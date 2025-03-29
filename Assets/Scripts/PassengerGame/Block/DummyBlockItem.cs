using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using UnityEngine;

public class DummyBlockItem : ISlotItem
{
    public GameObject Root { get; }
    public GameKey SlotIndex { get; set; }
    public List<GameKey> OccupiedGridKeys { get; } = new();
    public ItemColor ItemColor => ItemColor.Red;
    public bool IsMovable => false;
    ItemColor ISlotItem.ItemColor { get; set; }

    public DummyBlockItem(GameObject root)
    {
        Root = root;
    }

    public void MoveTo(GameKey targetKey) { }
}
