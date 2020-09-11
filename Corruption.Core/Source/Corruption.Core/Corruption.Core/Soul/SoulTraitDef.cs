using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core.Soul
{
    public class SoulTraitDef : TraitDef
    {
        public List<SoulTraitDegreeOptions> soulTraitDegreeOptions;
    }

    public class SoulTraitDegreeOptions : TraitDegreeData
    {
        public List<AbilityDef> abilityUnlocks = new List<AbilityDef>();
    }
}
