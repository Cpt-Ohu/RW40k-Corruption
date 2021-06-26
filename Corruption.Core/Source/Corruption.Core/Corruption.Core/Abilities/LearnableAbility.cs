using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Abilities
{

    public class LearnableAbility
    {
        public AbilityDef ability;

        public AbilityDef perequesiteAbility;

        public bool replacesPerequisite = false;

        public List<AbilityDef> conflictsWith = new List<AbilityDef>();

        public float cost;

        public Vector2 position = Vector2.zero;

        public GodDef associatedGod;
    }
}

