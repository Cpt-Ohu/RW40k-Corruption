using Corruption.Core;
using Corruption.Core.Gods;
using Corruption.Core.Soul;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Worship.Abilities
{
    public class CompAbilityEffect_GainCorruption : CompAbilityEffect
    {
        public override void Apply(GlobalTargetInfo target)
        {
            base.Apply(target);
            CompSoul soul = this.parent.pawn.Soul();
            GodDef casterGod = soul.ChosenPantheon.GodsListForReading.RandomElementByWeight(x => 1f + soul.FavourTracker.FavourValueFor(x));
            Pawn pawn = target.Thing as Pawn;
            if (pawn != null)
            {
                pawn.Soul()?.GainCorruption(casterGod.favourCorruptionFactor * 1500, casterGod, false);
                MoteMaker.MakeStaticMote(pawn.Position, pawn.Map, CoreThingDefOf.Mote_HolyExorcise);
            }
        }
    }

    public class CompProperties_GainCorruption : CompProperties_AbilityEffect
    {
        public CompProperties_GainCorruption()
        {
            this.compClass = typeof(CompAbilityEffect_GainCorruption);
        }
    }
}
