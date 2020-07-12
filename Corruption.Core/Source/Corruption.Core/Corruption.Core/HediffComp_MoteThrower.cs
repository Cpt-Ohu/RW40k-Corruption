using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_MoteThrower : HediffComp
    {
        public HediffCompProperties_MoteThrower Props => this.props as HediffCompProperties_MoteThrower;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (this.Props.mote != null && this.Pawn.IsHashIntervalTick(100) && Pawn.Map != null && !Pawn.Position.Fogged(Pawn.Map))
            {
                MoteMaker.ThrowMetaIcon(this.Pawn.Position, Pawn.Map, this.Props.mote);
            }
        }
    }

    public class HediffCompProperties_MoteThrower : HediffCompProperties
    {
        public ThingDef mote;

        public HediffCompProperties_MoteThrower()
        {
            this.compClass = typeof(HediffComp_MoteThrower);
        }

        public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            foreach (var error in base.ConfigErrors(parentDef))
            {
                yield return error;
            }
            if (this.mote == null)
            {
                yield return $"{parentDef} contains HediffComp_MoteThrower with null mote";
            }
        }
    }
}
