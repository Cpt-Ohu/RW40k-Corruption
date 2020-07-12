using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Worship.Wonders
{
    public class WonderWorker_ChangeRelation : WonderWorker_Targetable
    {
        public override bool TryExecuteWonder(GodDef god, int worshipPoints)
        {
            Pawn pawn = this.target.Thing as Pawn;
            if (pawn != null)
            {
                if (pawn.Faction != Faction.OfPlayer)
                {

                    float change = this.Def.ResolveWonderPoints(worshipPoints);
                    return pawn.Faction.TryAffectGoodwillWith(Faction.OfPlayer, (int)change);
                }
            }
            Messages.Message("MessageInvalidWonderTargetDesc".Translate(), MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }
    }
}
