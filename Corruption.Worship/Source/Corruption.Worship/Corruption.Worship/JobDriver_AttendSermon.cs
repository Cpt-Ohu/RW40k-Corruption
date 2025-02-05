﻿using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace Corruption.Worship
{
    public class JobDriver_AttendSermon : JobDriver_Spectate
    {
        private BuildingAltar altar => this.TargetB.Cell.GetFirstThing<BuildingAltar>(base.Map);

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
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

            Lord lord = pawn.GetLord();
            LordJob_Sermon lordJob = lord.LordJob as LordJob_Sermon;
            lastToil.FailOn(x => this.altar == null || altar.CurrentActiveSermon == null);
            lastToil.AddPreTickAction(delegate
            {
                if (this.altar != null && altar.CurrentActiveSermon != null)
                {
                    this.pawn.Soul()?.GainCorruption(this.altar.CompShrine.Props.worshipFactor * altar.CurrentActiveSermon.DedicatedTo.favourCorruptionFactor, altar.CurrentActiveSermon.DedicatedTo);
                }
            });
        }

    }
}
