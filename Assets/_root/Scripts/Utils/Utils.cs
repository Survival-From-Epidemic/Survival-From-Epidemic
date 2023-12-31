﻿using System;
using System.Collections.Generic;
using _root.Scripts.Game;
using _root.Scripts.SingleTon;
using _root.Scripts.UI.InGameMenuView;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _root.Scripts.Utils
{
    public static class Utils
    {
        private static readonly Vector3 SubVector = new(0.5f, 0.5f);
        public static bool IsPositive(this int value) => value >= 0;

        public static bool IsNatural(this int value) => value > 0;

        public static bool Chance(this float value, float weight = 1) => Random.value < value * weight / 100d;

        public static bool Chance(this double value, float weight = 1) => Random.value < value * weight / 100d;

        public static bool Chance(this int value, float weight = 1) => Random.value < value * weight / 100d;

        public static Vector3 CenterAnchorPosition()
        {
            var pos = MainCamera.Component.ScreenToViewportPoint(Input.mousePosition);
            return (pos - SubVector) * 50;
        }

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

        public static string GetGraphName(this InGameMenuGraph graph)
        {
            return graph switch
            {
                InGameMenuGraph.Person => "전염 상태",
                InGameMenuGraph.State => "학생 상태",
                InGameMenuGraph.Disease => "질병 정보",
                _ => throw new ArgumentOutOfRangeException(nameof(graph), graph, null)
            };
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static string SetColor(this string text, Color col) => $"<color={ColorHexFromUnityColor(col)}>{text}</color>";
        public static string ColorHexFromUnityColor(this Color unityColor) => $"#{ColorUtility.ToHtmlStringRGBA(unityColor)}";
    }
}