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
            if (obj is GameKey otherKey)
            {
                return _valueAsString == otherKey._valueAsString;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _valueAsString.GetHashCode();
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
    }
}
