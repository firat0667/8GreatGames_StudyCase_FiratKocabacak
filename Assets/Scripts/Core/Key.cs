using UnityEngine;

namespace GreatGames.CaseLib.Key
{
    // LongKey ensures no conflicts between keys.
    public class LongKey : GameKey
    {
        public LongKey(string value) : base(value)
        {
            _value = value.GetHashCode() + Random.Range(-10000, 10000);
        }
    }

    // Basic ID generated from a string value.
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

        public override bool Equals(object obj)
        {
            return this == obj as GameKey;
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(GameKey first, GameKey second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }
            else if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.Value == second.Value;
        }

        public static bool operator !=(GameKey first, GameKey second)
        {
            return !(first == second);
        }
    }
}
