using System.Collections.Generic;
using System.Text;
using _root.Scripts.Game;
using _root.Scripts.Utils;
using UnityEngine;

namespace _root.Scripts.UI.InGameMenuView
{
    public static class ClickerData
    {
        private static readonly float[] InfectWeight = { 0.5f, 1.5f };
        private static readonly float[] Infectivity = { 0.5f, 2f };
        private static readonly float[] InfectPower = { 0.03f, 0.05f };
        private static readonly float[] ModificationDecrease = { 1.5f, 4f };
        private static readonly int[] StudyPlus = { 5, 15 };
        private static readonly int[] StudyMinus = { -5, -25 };
        private static readonly int[] ConcentrationPlus = { 0, 5 };
        private static readonly int[] ConcentrationMinus = { -999, -3 };
        private static readonly int[] MaskPlus = { 10, 25 };
        private static readonly int[] MaskMinus = { -3, -25 };
        private static readonly int[] AnnoyPlus = { 5, 10 };
        private static readonly int[] AnnoyMinus = { -10, -25 };
        private static readonly string[] StrMinusArr = { "소폭 감소", "감소", "대폭 감소" };
        private static readonly string[] StrPlusArr = { "소폭 증가", "증가", "대폭 증가" };

        private static string Disease(this float value, string prefix, IReadOnlyList<float> arr)
        {
            if (value == 0) return "";
            for (var i = 0; i < arr.Count; i++)
                if (value <= arr[i])
                    return $"{prefix}{StrMinusArr[i]}\n".SetColor(Color.cyan);
            return $"{prefix}{StrMinusArr[arr.Count]}\n".SetColor(Color.cyan);
        }

        private static string Authority(this int value, string prefix, IReadOnlyList<int> plus, IReadOnlyList<int> minus)
        {
            if (value == 0) return "";
            for (var i = 0; i < minus.Count; i++)
                if (value <= minus[i])
                    return $"{prefix}{StrMinusArr[i]}\n".SetColor(Color.green);
            if (value < 0) return $"{prefix}{StrMinusArr[minus.Count]}\n".SetColor(Color.green);
            for (var i = 0; i < plus.Count; i++)
                if (value <= plus[i])
                    return $"{prefix}{StrPlusArr[i]}\n".SetColor(Color.red);
            return $"{prefix}{StrPlusArr[plus.Count]}\n".SetColor(Color.red);
        }

        public static string GetClickerData(this LocalDataManager.GridData gridData) =>
            new StringBuilder()
                .Append(gridData.disease.infectWeight.Disease("전염 확률 ", InfectWeight))
                .Append(gridData.disease.infectivity.Disease("전염력 ", Infectivity))
                .Append(gridData.disease.infectPower.Disease("감염 확률 ", InfectPower))
                .Append(((float)gridData.disease.modificationDecrease).Disease("공기 감염률 ", ModificationDecrease))
                .Append(gridData.authority.study.Authority("교육 방해 ", StudyPlus, StudyMinus))
                .Append((-gridData.authority.concentration).Authority("수업 집중 방해 ", ConcentrationPlus, ConcentrationMinus))
                .Append(gridData.authority.mask.Authority("마스크 반발도 ", MaskPlus, MaskMinus))
                .Append(gridData.authority.annoy.Authority("짜증 ", AnnoyPlus, AnnoyMinus))
                .ToString();
    }
}