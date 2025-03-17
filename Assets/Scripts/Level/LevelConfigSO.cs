using GreatGames.CaseLib.Grid;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/LevelConfig")]
public class LevelConfigSO : ScriptableObject
{
    [Header("Grid Settings")]
    public Vector2Int UpperGridSize = new(5, 5);
    public Vector2Int LowerGridSize = new(6, 1);

    public List<SlinkyData> Slinkies = new();

    
    //TODO: you should change here
    public Transform LevelInstance { get; set; }
}
