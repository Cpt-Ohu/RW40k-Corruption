using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class RecipeWorker_Exorcise : RecipeWorker
    {
        public override bool AvailableOnNow(Thing thing)
        {
            Pawn pawn = thing as Pawn;
            if (pawn == null) return false;
            return pawn.health.hediffSet.GetHediffs<HediffDemonicPossession>().Count() > 0;            
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            float baseChance = 0.2f;
            var exorcistSoul = billDoer.Soul();
            if (exorcistSoul != null)
            {
                baseChance *= 1 + (1 - exorcistSoul.CorruptionLevel);
            }
            PossessionUtiltiy.TryRemovePossession(pawn, baseChance);
        }

        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            return;
        }
    }
}
