using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class CorruptionPawnKindDef : PawnKindDef
    {
        public AfflictionProperty affliction;

        public bool renamePawns = false;

        public bool useFixedGender = false;

        public Gender fixedGender;

        public RulePackDef overridingNameRulePack;

        public IntRange additionalImplantCount = new IntRange(0, 0);

        public List<HediffDef> forcedStartingHediffs = new List<HediffDef>();

        public List<HediffDef> disallowedStartingHediffs = new List<HediffDef>();

        public List<RecipeDef> forcedStartingImplantRecipes = new List<RecipeDef>();

        public List<TraitDef> forcedStartingTraits = new List<TraitDef>();

        public List<string> additionalTags = new List<string>();

        public bool isServitor;
    }
}
