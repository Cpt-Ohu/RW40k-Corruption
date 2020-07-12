using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public static class PossessionUtiltiy
    {
        public static bool TryRemovePossession(Pawn pawn, float survivalChance = 0.2f)
        {
            var possession = pawn.health.hediffSet.GetHediffs<HediffDemonicPossession>().FirstOrDefault();

            if (possession != null)
            {
                pawn.health.RemoveHediff(possession);
                MoteMaker.MakeStaticMote(pawn.Position, pawn.Map, Corruption.Core.CoreThingDefOf.Mote_DemonicPossessionBlast);

                if (Rand.Value < survivalChance)
                {
                    pawn.health.AddHediff(HediffDefOf.Exorcised);
                    Find.LetterStack.ReceiveLetter("PossessionSurvived".Translate(), "PossessionSurvivedDesc".Translate(pawn.Name.ToStringShort), LetterDefOf.PositiveEvent);
                }
                else
                {
                    pawn.Kill(null, possession);
                    Find.LetterStack.ReceiveLetter("PossessionDied".Translate(), "PossessionDiedDesc".Translate(pawn.Name.ToStringShort), LetterDefOf.NegativeEvent);
                }

                return true;
            }
            return false;
        }
    }
}
