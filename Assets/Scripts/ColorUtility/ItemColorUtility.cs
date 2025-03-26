using System.Collections.Generic;
using UnityEngine;

namespace GreatGames.CaseLib.Utility
{
    public static class ItemColorUtility
    {
        private static readonly Dictionary<ItemColor, Color> _colorMap = new()
        {
            { ItemColor.Red, Color.red },
            { ItemColor.Blue, Color.blue },
            { ItemColor.Green, Color.green },
            { ItemColor.Pink, new Color(1f, 0.41f, 0.71f) },
            { ItemColor.Orange, new Color(1f, 0.5f, 0f) }
        };

        private static readonly Dictionary<Color, ItemColor> _reverseColorMap = new();

        static ItemColorUtility()
        {
            foreach (var pair in _colorMap)
            {
                _reverseColorMap[pair.Value] = pair.Key;
            }
        }

        public static bool TryGetColor(ItemColor color, out Color result)
        {
            return _colorMap.TryGetValue(color, out result);
        }

        public static Color GetColor(ItemColor color)
        {
            return _colorMap.ContainsKey(color) ? _colorMap[color] : Color.white;
        }

        public static bool TryGetSlinkyColorEnum(Color color, out ItemColor slinkyColor)
        {
            return _reverseColorMap.TryGetValue(color, out slinkyColor);
        }

        public static ItemColor GetSlinkyColorEnum(Color color)
        {
            return _reverseColorMap.TryGetValue(color, out var slinkyColor) ? slinkyColor : ItemColor.Red;
        }
    }

    public enum ItemColor
    {
        Red,
        Blue,
        Green,
        Pink,
        Orange
    }
}
