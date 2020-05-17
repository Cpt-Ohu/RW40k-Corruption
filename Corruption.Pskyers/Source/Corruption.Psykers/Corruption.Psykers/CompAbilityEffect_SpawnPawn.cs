using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_SpawnPawn : CompAbilityEffect_WithDuration
    {
        public new CompProperties_SpawnPawn Props => this.props as CompProperties_SpawnPawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            for (int i = 0; i < this.Props.spawnCount; i++)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(Props.kindDef, this.Props.isPsykerControlled ? this.parent.pawn.Faction : Find.FactionManager.FirstFactionOfDef(Corruption.Core.FactionsDefOf.IoM_NPC));
                GenSpawn.Spawn(pawn, target.Cell, this.parent.pawn.Map);
                if (this.Props.spawningMentalState != null)
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(this.Props.spawningMentalState);
                }
                foreach (var hediffDef in this.Props.spawningHediffs)
                {
                    Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
                    HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
                    if (hediffComp_Disappears != null)
                    {
                        hediffComp_Disappears.ticksToDisappear = GetDurationSeconds(pawn).SecondsToTicks();
                    }
                    pawn.health.AddHediff(hediff);

                }

            }
        }
    }

    public class CompProperties_SpawnPawn : CompProperties_AbilityEffect
    {
        public PawnKindDef kindDef;

        public int spawnCount = 1;

        public bool isPsykerControlled = true;

        public List<HediffDef> spawningHediffs = new List<HediffDef>();

        public MentalStateDef spawningMentalState;

        public CompProperties_SpawnPawn()
        {
            this.compClass = typeof(CompAbilityEffect_SpawnPawn);
        }
    }
}
