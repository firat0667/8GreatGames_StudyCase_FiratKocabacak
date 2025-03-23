using GreatGames.CaseLib.Grid;
using UnityEngine;

namespace GreatGames.CaseLib.Key
{
    public class LongKey : GameKey
    {
        public LongKey(string value) : base(value)
        {
            _value = value.GetHashCode() + Random.Range(-10000, 10000);
        }
    }

    public class GameKey
    {
        public int Value => _value;
        protected int _value;
        public string ValueAsString => _valueAsString;
        protected string _valueAsString;
        public GameKey(string value)
        {
            _valueAsString = value;
            _value = value.GetHashCode();
        }

        public GameKey(int x, int y)
        {
            _valueAsString = $"{x},{y}";
            _value = _valueAsString.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is GameKey other)
            {
                return ValueAsString == other.ValueAsString;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ValueAsString.GetHashCode();
        }

        public static bool operator ==(GameKey first, GameKey second)
        {
            if (ReferenceEquals(first, second)) return true;
            if (ReferenceEquals(first, null) || ReferenceEquals(second, null)) return false;

            return first._valueAsString == second._valueAsString;
        }

        public static bool operator !=(GameKey first, GameKey second)
        {
            return !(first == second);
        }
        public Vector2Int ToVector2Int()
        {
            if (string.IsNullOrEmpty(ValueAsString)) return Vector2Int.zero;

            string[] parts = ValueAsString.Split('_');
            if (parts.Length > 1)
            {
                string[] coords = parts[1].Split(',');
                if (coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))
                {
                    return new Vector2Int(x, y);
                }
            }

            Debug.LogError($"[ERROR] GameKey.ToVector2Int() başarısız: {ValueAsString}");
            return Vector2Int.zero;
        }
    }
    public static class GameKeyExtensions
    {
        public static bool IsLower(this GameKey key) => key.ValueAsString.StartsWith("L_");
        public static bool IsUpper(this GameKey key) => key.ValueAsString.StartsWith("U_");
        public static string GetPrefix(bool isUpper)
        {
            return isUpper ? "U_" : "L_";
        }
    }

}
