using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerkSystem
{

    [CreateAssetMenu(fileName = "PerkTree", menuName = "TechTree/PerkTree", order = 0)]
    public class PerkTree : ScriptableObject
    {
        public PerkObject[] PerkList = new PerkObject[0];

        public PerkObject this[int index] => PerkList[index];
    }

    [Serializable]
    public class PerkObject
    {
#if UNITY_EDITOR
        [HideInInspector] public Vector2Int Pos;
#endif

        public int[] ParentsIndex = new int[0];
        public int[] ChildsIndex = new int[0];

        public Perk Perk;
    }
}