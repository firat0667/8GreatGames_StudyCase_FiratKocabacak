using UnityEngine;
using System.Collections.Generic;

namespace GreatGames.CaseLib.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        [HideInInspector] public int CurrentLevelIndex = 0;
        [HideInInspector] public int CurrentLevelCount = 0;
        public List<LevelConfigSO> Levels;
    }
}
