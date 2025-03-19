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
            { SlinkyColor.Pink, new Color(1f, 0.41f, 0.71f) },
            { SlinkyColor.Orange, new Color(1f, 0.5f, 0f) }
        };

        private static readonly Dictionary<Color, SlinkyColor> _reverseColorMap = new();

        static SlinkyColorUtility()
        {
            foreach (var pair in _colorMap)
            {
                _reverseColorMap[pair.Value] = pair.Key;
            }
        }

        public static bool TryGetColor(SlinkyColor color, out Color result)
        {
            return _colorMap.TryGetValue(color, out result);
        }

        public static Color GetColor(SlinkyColor color)
        {
            return _colorMap.ContainsKey(color) ? _colorMap[color] : Color.white;
        }

        public static bool TryGetSlinkyColorEnum(Color color, out SlinkyColor slinkyColor)
        {
            return _reverseColorMap.TryGetValue(color, out slinkyColor);
        }

        public static SlinkyColor GetSlinkyColorEnum(Color color)
        {
            return _reverseColorMap.TryGetValue(color, out var slinkyColor) ? slinkyColor : SlinkyColor.Red;
        }
    }

    public enum SlinkyColor
    {
        Red,
        Blue,
        Green,
        Pink,
        Orange
    }
}
