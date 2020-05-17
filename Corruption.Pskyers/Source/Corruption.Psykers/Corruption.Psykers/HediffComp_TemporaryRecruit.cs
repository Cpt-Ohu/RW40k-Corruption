using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class HediffComp_TemporaryRecruit : HediffComp_Disappears
    {
        public Faction PawnFactionOri;

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.PawnFactionOri = this.Pawn.Faction;
            Faction setFaction = this.PawnFactionOri == Faction.OfPlayer ? Find.FactionManager.FirstFactionOfDef(Corruption.Core.FactionsDefOf.IoM_NPC) : Faction.OfPlayer; 
            this.Pawn.jobs.EndCurrentJob(Verse.AI.JobCondition.InterruptForced);

            this.Pawn.SetFactionDirect(setFaction);

            Find.ColonistBar.MarkColonistsDirty();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            MoteMaker.MakeStaticMote(this.Pawn.Position, this.Pawn.Map, ThingDefOf.Mote_MicroSparks);
        }

        public override bool CompShouldRemove
        {
            get
            {
                if (base.CompShouldRemove)
                {
                    this.Pawn.SetFactionDirect(PawnFactionOri);

                    Find.ColonistBar.MarkColonistsDirty();
                    return true;
                }
                return false;

            }
        }
    }
}
