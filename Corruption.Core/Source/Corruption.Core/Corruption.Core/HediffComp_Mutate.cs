using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_Mutate : HediffComp
    {
        private int ticksToMutationTick = 60;

        public HediffCompProperties_Mutate Props => this.props as HediffCompProperties_Mutate;

        public override void CompPostTick(ref float severityAdjustment)
        {
            float newMutationChance = this.ticksToMutationTick == 0 ? this.Props.newMutationsChance : 0f;
            MutationUtility.ApplyMutation(this.Pawn, this.Props.mutationHediffs, this.Props.severityGainPerSecond * GenTicks.TicksPerRealSecond, newMutationChance);
            base.CompPostTick(ref severityAdjustment);
            ticksToMutationTick--;
            if (ticksToMutationTick <= 0) ticksToMutationTick = this.Props.newMutationInterval.RandomInRange * GenTicks.TicksPerRealSecond;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.ticksToMutationTick, "lastNewMutationTick");
        }
    }

    public class HediffCompProperties_Mutate : HediffCompProperties
    {
        public List<HediffDef> mutationHediffs = new List<HediffDef>();
        public float severityGainPerSecond;
        public float newMutationsChance = 0.05f;
        public IntRange newMutationInterval = IntRange.one;

        public BodyTypeDef changedBodyType;

        public HediffCompProperties_Mutate()
        {
            this.compClass = typeof(HediffComp_Mutate);
        }
    }
}
