using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class TraitMentalStateGiver_Slaaneshi : TraitMentalStateGiver
    {
        public override bool CheckGive(Pawn pawn, int checkInterval)
        {
			if (traitDegreeData.theOnlyAllowedMentalBreaks.Count == 0)
			{
				return false;
			}
			float curMood = pawn.mindState.mentalBreaker.CurMood;
			if (Rand.MTBEventOccurs(traitDegreeData.randomMentalStateMtbDaysMoodCurve.Evaluate(curMood), 60000f, checkInterval) && traitDegreeData.randomMentalState.Worker.StateCanOccur(pawn))
			{
				return pawn.mindState.mentalStateHandler.TryStartMentalState(traitDegreeData.theOnlyAllowedMentalBreaks.RandomElement().mentalState, "MentalStateReason_Trait".Translate(traitDegreeData.label));
			}
			return false;
		}
    }
}
