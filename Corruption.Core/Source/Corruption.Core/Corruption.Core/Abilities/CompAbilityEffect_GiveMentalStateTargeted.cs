using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core.Abilities
{
    public class CompAbilityEffect_GiveMentalStateTargeted : CompAbilityEffect
	{
		public new CompProperties_AbilityGiveMentalState Props => (CompProperties_AbilityGiveMentalState)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Thing as Pawn;
			if (pawn != null && !pawn.InMentalState)
			{
				TryGiveMentalStateWithDuration(pawn.RaceProps.IsMechanoid ? (Props.stateDefForMechs ?? Props.stateDef) : Props.stateDef, pawn, parent.def, Props.durationMultiplier);
				RestUtility.WakeUp(pawn);
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn != null && !AbilityUtility.ValidateNoMentalState(pawn, throwMessages))
			{
				return false;
			}
			return true;
		}

		private void TryGiveMentalStateWithDuration(MentalStateDef def, Pawn p, AbilityDef ability, StatDef multiplierStat)
		{
			if (p.mindState.mentalStateHandler.TryStartMentalState(def, null, true, false, this.parent.pawn))
			{
				float num = ability.statBases.GetStatValueFromList(StatDefOf.Ability_Duration, 10f);
				if (multiplierStat != null)
				{
					num *= p.GetStatValue(multiplierStat);
				}
				p.mindState.mentalStateHandler.CurState.forceRecoverAfterTicks = num.SecondsToTicks();
			}
		}
	}
}
