using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using UnityEngine;


public interface ISlotItem
{
    List<GameKey> OccupiedGridKeys { get; }
    void MoveTo(GameKey key); 
    GameObject Root { get; }
    GameKey SlotIndex { get; set; }

}

