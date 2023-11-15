using System;
using _root.Scripts.Game;
using Random = UnityEngine.Random;

namespace _root.Scripts.Utils
{
    public static class Utils
    {
        public static bool IsPositive(this int value) => value >= 0;

        public static bool IsNatural(this int value) => value > 0;

        public static bool Chance(this float value, float weight = 1) => Random.value < value * weight / 100d;

        public static bool Chance(this double value, float weight = 1) => Random.value < value * weight / 100d;

        public static bool Chance(this int value, float weight = 1) => Random.value < value * weight / 100d;

        public static float SymptomPcrDate(this SymptomType symptomType)
        {
            return symptomType switch
            {
                SymptomType.Nothing => 26,
                SymptomType.Weak => 8,
                SymptomType.Normal => 3f,
                SymptomType.Strong => 1.5f,
                SymptomType.Emergency => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(symptomType), symptomType, null)
            };
        }
    }
}