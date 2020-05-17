using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public static class MutationUtility 
    {
        public static void ApplyMutation(Pawn pawn, List<HediffDef> mutations, float severityChange, float newMutationChance = 0.1f)
        {
            var existingHediffs = pawn.health.hediffSet.hediffs.FindAll(x => mutations.Contains(x.def));

            var selectedHediff = existingHediffs.FindAll(x => x.Severity < x.def.maxSeverity).RandomElement();

            if (selectedHediff != null && Rand.Value > newMutationChance)
            {
                selectedHediff.Severity += severityChange;
            }
            else
            {
                var randomDef = mutations.RandomElement();
                Hediff hediff = HediffMaker.MakeHediff(randomDef, pawn);
                pawn.health.AddHediff(hediff);
            }
        }

        internal static void ApplyMutation(Pawn pawn, List<HediffDef> mutationHediffs, object gainPerSecond)
        {
            throw new NotImplementedException();
        }
    }
}
