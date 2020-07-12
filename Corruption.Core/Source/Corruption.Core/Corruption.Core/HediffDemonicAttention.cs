using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class HediffDemonicAttention : HediffWithComps
    {
    }

    public class HediffDemonicPossession : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if (this.pawn.health.hediffSet.HasHediff(HediffDefOf.Exorcised))
            {
                this.pawn.health.RemoveHediff(this);
            }
        }
    }
}
