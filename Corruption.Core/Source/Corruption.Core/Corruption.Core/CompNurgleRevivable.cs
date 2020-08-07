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
    public class CompNurgleRevivable : HediffComp
    {
        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            float revChance = this.Pawn.Soul()?.FavourTracker.FavourValueFor(GodDefOf.Nurgle) / 11000f ?? 0f;
            if (Rand.Value > revChance)
            {
                if (this.Pawn != null)
                {
                    ResurrectionUtility.Resurrect(this.Pawn);
                }
            }
        }
    }
}
