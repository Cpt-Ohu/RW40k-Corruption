using RimWorld;
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
        private sealed class PotentialMutation
        {
            public PotentialMutation(HediffDef hediff, BodyPartRecord bodyPart)
            {
                Def = hediff;
                Part = bodyPart;
            }

            public HediffDef Def { get; set; }
            public BodyPartRecord Part { get; set; }
        }

        public static void ApplyMutation(Pawn pawn, List<HediffDef> mutations, float severityChange, float newMutationChance = 0.01f)
        {
            var existingHediffs = pawn.health.hediffSet.hediffs.FindAll(x => mutations.Exists(y => y == x.def && x.Severity < x.def.maxSeverity));
            var selectedHediff = existingHediffs.Count > 0 ? existingHediffs.RandomElement() : null;

            float severity = 0f;
            if (selectedHediff != null && Rand.Value > newMutationChance)
            {
                selectedHediff.Severity += severityChange;
            }
            else
            {
                var availableMutation = GetNewMutation(mutations, pawn);
                var potentialMutation = availableMutation;
                if (potentialMutation != null)
                {
                    severity = severityChange;
                    Hediff hediff = HediffMaker.MakeHediff(potentialMutation.Def, pawn);
                    hediff.Part = potentialMutation.Part;
                    pawn.health.AddHediff(hediff);
                    hediff.Severity += severity;
                }
            }
        }

        private static PotentialMutation GetNewMutation(List<HediffDef> allMutations, Pawn pawn)
        {
            var availables = AvailableMutations(allMutations, pawn);
            if (availables.Count() > 0)
            {
                return availables.RandomElement();
            }
            return null;
        }

        private static IEnumerable<PotentialMutation> AvailableMutations(List<HediffDef> allMutations, Pawn pawn)
        {
            foreach (var bodyPart in pawn.RaceProps.body.AllParts.Where(x => !pawn.health.hediffSet.PartIsMissing(x)))
            {
                foreach (var mutation in allMutations.Where(x => x.HasComp(typeof(HediffComp_ReplacePart))))
                {
                    var replacePart = mutation.CompProps<HediffCompProperties_ReplacePart>();
                    if (replacePart != null && bodyPart.def == replacePart.partToReplace)
                    {
                        yield return new PotentialMutation(mutation, bodyPart);
                    }
                }
            }

            foreach (var hediff in allMutations.Where(x => x.addedPartProps == null && x.HasComp(typeof(HediffComp_ReplacePart)) == false))
            {
                yield return new PotentialMutation(hediff, null);
            }
        }
    }
}
