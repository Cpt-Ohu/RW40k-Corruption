using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class HediffComp_TemporalEntity : HediffComp_Disappears
    {
        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            this.Pawn.Destroy();
        }
    }

    public class HediffCompProperties_TemporalEntity : HediffCompProperties
    {
        public HediffCompProperties_TemporalEntity()
        {
            this.compClass = typeof(HediffComp_TemporalEntity);
        }
    }
}
