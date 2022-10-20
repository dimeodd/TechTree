using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PerkSystem {

    [CreateAssetMenu (fileName = "Perk", menuName = "TechTree/Perk", order = 0)]
    public class Perk : ScriptableObject {
        public int Cost;
        public string PerkName; //Translate
        public string Description; //Translate

        //Buildings UnlockBuilds
        //Buildings UnlockUnits
        //BuildBonus BonusesForBuilds
        //UnitBonus BonusesForUnits
    }
}