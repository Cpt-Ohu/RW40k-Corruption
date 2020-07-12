using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Worship.Wonders
{
    public class Dialog_FormAndSendPilgrims : Dialog_FormCaravan
    {

        public Dialog_FormAndSendPilgrims(Map map) : base(map)
        {

        }
        public override void PostClose()
        {
            base.PostClose();
            foreach (Pawn p in TransferableUtility.GetPawnsFromTransferables(this.transferables))
            {
                CompSoul soul = p.Soul();

                if (soul != null)
                {
                    soul.IsOnPilgrimage = true;
                }
            }

        }
    }
}
