using System;
using UnityEngine;

namespace _root.Scripts.Game
{
    [Serializable]
    public struct Disease
    {
        public float infectWeight;
        public int infectivity;
        public float infectPower;

        public static Disease operator *(Disease disease, float value) =>
            new()
            {
                infectWeight = disease.infectWeight * value,
                infectivity = Mathf.RoundToInt(disease.infectivity * value)
            };
    }

    [Serializable]
    public struct Person
    {
        public int totalPerson;
        public int healthyPerson;
        public int deathPerson;
        public int infectedPerson;
    }

    [Serializable]
    public class PersonData
    {
        public Player.Person personObject;
        public int catchDate;
        public bool isInfected;

        public SymptomType symptomType;

        public float deathWeight;
        public float recoverWeight;
    }

    public enum SymptomType
    {
        Nothing,
        Weak,
        Normal,
        Strong,
        Emergency
    }
}