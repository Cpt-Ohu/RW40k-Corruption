using Corruption.Core.Gods;
using Corruption.Core.Soul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_GainCorruption : HediffComp
    {
        public HediffCompProperties_GainCorruption Props => this.props as HediffCompProperties_GainCorruption;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.Pawn.Soul()?.GainCorruption(this.Props.gainPerTick, this.Props.god);
        }
    }

    public class HediffCompProperties_GainCorruption : HediffCompProperties
    {
        public GodDef god;

        public float gainPerTick = 0.1f;

        public HediffCompProperties_GainCorruption()
        {
            this.compClass = typeof(HediffComp_GainCorruption);
        }
    }
}
