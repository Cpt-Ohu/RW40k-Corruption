using Corruption.Core.Gods;
using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Corruption.Worship.Wonders
{
    public class WonderWorker_AddSpecialTrait : WonderWorker_Targetable
    {
        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters()
            {
                canTargetPawns = true,
                canTargetBuildings = false,
                canTargetItems = false,
                canTargetLocations = false
            };
        }

        protected override void TryDoEffectOnTarget(GodDef god, int worshipPoints)
        {
            Pawn pawn = this.target.Thing as Pawn;
            if (pawn != null)
            {
                foreach (var incompatible in this.Def.traitToGive.conflictingTraits)
                {
                    Trait incTrait = pawn.story.traits.GetTrait(incompatible);
                    if (incTrait != null)
                    {
                        pawn.story.traits.allTraits.Remove(incTrait);
                    }
                }

                Trait trait = new Trait(this.Def.traitToGive, 0, true);
                pawn.story.traits.GainTrait(trait);
            }
        }
    }
}
