using System.Collections.Generic;
using UnityEngine;

namespace GreatGames.CaseLib.Utility
{
    public static class SlinkyColorUtility
    {
        private static readonly Dictionary<SlinkyColor, Color> _colorMap = new()
        {
            { SlinkyColor.Red, Color.red },
            { SlinkyColor.Blue, Color.blue },
            { SlinkyColor.Green, Color.green },
            { SlinkyColor.Yellow, new Color(1f, 0.92f, 0.016f) },
            { SlinkyColor.Orange, new Color(1f, 0.5f, 0f) }

        };

        public static bool TryGetColor(SlinkyColor color, out Color result)
        {
            return _colorMap.TryGetValue(color, out result);
        }

        public static Color GetColor(SlinkyColor color)
        {
            return _colorMap.ContainsKey(color) ? _colorMap[color] : Color.white;
        }
    }

    public enum SlinkyColor
    {
        Red,
        Blue,
        Green,
        Yellow,
        Orange 
    }
}
