using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class ThoughtWorker_NurgleGifts : ThoughtWorker
    {
        private static List<HediffDef> sickHediffs = DefDatabase<HediffDef>.AllDefsListForReading.FindAll(x => x.makesSickThought);

        private List<HediffDef> HediffDefs
        {
            get
            {
                List<HediffDef> tmplist = new List<HediffDef>();

                return tmplist;
            }
        }

        private int SumOfGifts;

        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            foreach (HediffDef hdef in sickHediffs)
            {
                if (p.health.hediffSet.hediffs.Any(x => x.def == hdef))
                {
                    SumOfGifts += 1;
                    if (hdef == HediffDefOf.NurglesRot)
                    {
                        SumOfGifts += 2;
                    }
                }
            }

            if (SumOfGifts > 3)
            {
                return ThoughtState.ActiveAtStage(2);
            }
            if (SumOfGifts > 1)
            {
                return ThoughtState.ActiveAtStage(1);
            }
            if (SumOfGifts > 0)
            {
                return ThoughtState.ActiveAtStage(0);
            }
            return ThoughtState.Inactive;

        }
    }
}
