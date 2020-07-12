using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_DemonicPossession : HediffComp_SeverityPerDay
    {
        public HediffCompProperties_DemonicPossession Props => this.props as HediffCompProperties_DemonicPossession;


        private float growthPerTick
        {
            get
            {
                var comp = this.parent.def.CompProps<HediffCompProperties_SeverityPerDay>();
                if (comp != null)
                {
                    return comp.severityPerDay / GenDate.TicksPerDay;
                }
                return 1f;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
        }

        public override string CompLabelInBracketsExtra
        {
            get
            {
                int totalTicksRemaining = (int)((this.parent.def.lethalSeverity - this.parent.Severity) / this.growthPerTick);
                if (totalTicksRemaining > GenDate.TicksPerDay)
                {
                    return totalTicksRemaining.ToStringTicksToDays();
                }
                return totalTicksRemaining.TicksToSeconds().ToString("0.0");
            }
        }

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            if (this.Props.demonToSpawn != null && this.Pawn.Corpse != null)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(this.Props.demonToSpawn);
                GenSpawn.Spawn(pawn, this.Pawn.Corpse.Position, this.Pawn.Corpse.Map);
                MoteMaker.ThrowExplosionCell(pawn.Position, pawn.Map, ThingDefOf.Mote_Bombardment, UnityEngine.Color.red);
                if (this.Props.spawningMentalState != null)
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(this.Props.spawningMentalState);
                }
                
                this.Pawn.Corpse.Destroy(DestroyMode.KillFinalize);
            }
        }
    }

    public class HediffCompProperties_DemonicPossession: HediffCompProperties_SeverityPerDay
    {
        public PawnKindDef demonToSpawn;

        public MentalStateDef spawningMentalState;

        public HediffCompProperties_DemonicPossession()
        {
            this.compClass = typeof(HediffComp_DemonicPossession);
        }
    }
}
