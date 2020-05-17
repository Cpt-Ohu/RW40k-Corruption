using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_GiveHediffSeverity : CompAbilityEffect_GiveHediff
    {
        public new CompProperties_AbilityGiveHediffSeverity Props => this.props as CompProperties_AbilityGiveHediffSeverity;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (target.Pawn != null)
            {
                Hediff givenHediff = target.Pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediffDef);
                if (givenHediff != null)
                {
                    givenHediff.Severity = this.Props.severity;
                }
            }
        }
    }

    public class CompProperties_AbilityGiveHediffSeverity : CompProperties_AbilityGiveHediff
    {
        public float severity;

        public bool onlyToSelf;

        public CompProperties_AbilityGiveHediffSeverity()
        {
            this.compClass = typeof(CompAbilityEffect_GiveHediffSeverity);
        }
    }
}
