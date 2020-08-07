using Corruption.Core.Gods;
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

    public class HediffComp_NurglesRot : HediffComp_SeverityPerDay
    {

        private Pawn Victim;

        private CompSoul soul
        {
            get
            {
                CompSoul soulInt;
                if ((soulInt = Pawn.Soul()) != null)
                    return soulInt;
                else
                {
                    throw new Exception("Pawn with Nurgle's rot has no soul!");
                }
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.Victim = this.Pawn;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.Pawn.def.race.Humanlike)
            {
                soul.GainCorruption(5f, GodDefOf.Nurgle);
                if (this.parent.Severity > 0.8f)
                {
                    if (soul.Corrupted)
                    {
                        this.Pawn.health.AddHediff(HediffDefOf.MarkNurgle);
                        soul.GainCorruption(10000f, GodDefOf.Nurgle);
                        this.parent.Heal(1f);
                    }
                }
            }
        }

        public override void Notify_PawnDied()
        {
            if (this.Pawn.Corpse.Spawned)
            {
                GenExplosion.DoExplosion(this.Pawn.Position, this.Pawn.Corpse.Map, 1, Corruption.Core.DamageDefOf.RottenBurst, null, 1,-1, null, null, null, null, ThingDefOf.Filth_Vomit, 1);
            }
        }
    }

    public class HediffComp_NurglesMark : HediffComp
    {

        public override void Notify_PawnDied()
        {
            if (this.Pawn.Corpse.Spawned)
            {
                GenExplosion.DoExplosion(this.Pawn.Position, this.Pawn.Map, 5, Corruption.Core.DamageDefOf.RottenBurst, null, 0,0, null, null, null,null, ThingDefOf.Filth_Vomit, 1);
                Pawn.Corpse.Destroy(DestroyMode.Vanish);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.Pawn.IsHashIntervalTick(1200))
            {
                FilthMaker.TryMakeFilth(this.Pawn.DrawPos.ToIntVec3(), this.Pawn.Map, ThingDefOf.Filth_Vomit, 1);
            }
        }
    }
}
