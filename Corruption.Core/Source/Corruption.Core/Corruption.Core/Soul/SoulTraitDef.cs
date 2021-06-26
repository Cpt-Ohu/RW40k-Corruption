using Corruption.Core.Abilities;
using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Soul
{
    public class SoulTraitDef : TraitDef
    {
        public Type tabWindowClass;

        internal Texture2D Icon;
        public string iconPath;
        public GodDef associatedGod;
        public TaleDef progressionTaleDef;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                if (!this.iconPath.NullOrEmpty())
                {
                    this.Icon = ContentFinder<Texture2D>.Get(this.iconPath);
                }
            });
        }

    }

    public class SoulTraitDegreeOptions : TraitDegreeData
    {
        public List<AbilityDef> abilityUnlocks = new List<AbilityDef>();
        public List<LearnableAbility> learnableAbilities = new List<LearnableAbility>();
        public TraitDef associatedTrait;
        public int associatedTraitDegree = -1;
    }
}
