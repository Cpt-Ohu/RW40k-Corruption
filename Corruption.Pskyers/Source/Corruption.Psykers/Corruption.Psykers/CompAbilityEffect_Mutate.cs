using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_Mutate : CompAbilityEffect
    {
        public new CompProperties_AbilityMutate Props => this.props as CompProperties_AbilityMutate;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            ApplyMutation(target.Pawn);
        }

        protected void ApplyMutation(Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }

            var existingHediffs = pawn.health.hediffSet.hediffs.FindAll(x => this.Props.mutationHediffs.Contains(x.def));

            var selectedHediff = existingHediffs.FindAll(x => x.Severity < x.def.maxSeverity).RandomElement();

            if (selectedHediff != null && Rand.Value > 0.1f)
            {
                selectedHediff.Severity += (float)(this.Props.severityFactorFromEntropy * this.parent.def.EntropyGain / (100 * (Math.Sqrt(Math.Max(1,this.parent.def.EffectRadius * 0.5f)))));
            }
            else
            {
                var randomDef = this.Props.mutationHediffs.RandomElement();
                Hediff hediff = HediffMaker.MakeHediff(randomDef, pawn);
                pawn.health.AddHediff(hediff);
            }
        }
    }

    public class CompAbilityEffect_MutateSelf : CompAbilityEffect_Mutate
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            this.ApplyMutation(this.parent.pawn);
        }
    }

    public class CompProperties_AbilityMutate : CompProperties_AbilityEffect
    {
        public List<HediffDef> mutationHediffs = new List<HediffDef>();

        public float severityFactorFromEntropy = 1f;

        public CompProperties_AbilityMutate()
        {
            this.compClass = typeof(CompAbilityEffect_Mutate);
        }        
    }

}
