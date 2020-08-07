using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Corruption.Worship
{
	public class JobDriver_AttendSermon : JobDriver_Spectate
	{
		private BuildingAltar altar;

		private CompShrine compShrine => this.altar.TryGetComp<CompShrine>();

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			BuildingAltar firstThing = this.TargetB.Cell.GetFirstThing<BuildingAltar>(base.Map);
			this.altar = firstThing;
			return base.TryMakePreToilReservations(errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil lastToil = null;
			foreach (var toil in base.MakeNewToils())
			{
				lastToil = toil;
				yield return toil;
			}

			lastToil.AddPreTickAction(delegate
			{
				if (this.compShrine != null)
				{
					this.pawn.Soul()?.GainCorruption(this.compShrine.CompProps.worshipFactor, this.compShrine.God);
				}
			});
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<BuildingAltar>(ref this.altar, "altar");
		}

	}
}
