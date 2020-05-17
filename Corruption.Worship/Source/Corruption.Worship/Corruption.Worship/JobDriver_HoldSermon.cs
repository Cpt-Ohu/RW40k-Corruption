﻿using Corruption.Core.Gods;
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

    public class JobDriver_HoldSermon : JobDriver
    {
        private TargetIndex AltarIndex = TargetIndex.A;
        private TargetIndex AltarInteractionCell = TargetIndex.B;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<TargetIndex>(ref this.AltarIndex, "AltarIndex", TargetIndex.A);
            Scribe_Values.Look<TargetIndex>(ref this.AltarInteractionCell, "AltarInteractionCell", TargetIndex.B);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawn", false);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.EndOnDespawnedOrNull(AltarIndex, JobCondition.Incompletable);
            Toil gotoAltarToil;
            gotoAltarToil = Toils_Goto.GotoThing(AltarInteractionCell, PathEndMode.OnCell);

            yield return gotoAltarToil;

            BuildingAltar altar = this.TargetA.Thing as BuildingAltar;
            GodDef god = altar.DedicatedTo;
            var altarToil = new Toil();
            altarToil.defaultCompleteMode = ToilCompleteMode.Delay;
            altarToil.defaultDuration = this.job.def.joyDuration;
            altarToil.AddPreTickAction(() =>
            {
                this.pawn.rotationTracker.FaceCell(this.TargetA.Cell);
                this.pawn.GainComfortFromCellIfPossible();
                ThrowPreacherMote(this.pawn, god);
            });
            yield return altarToil;

            this.AddFinishAction(() =>
            {
                altar.CalledInFlock = false;
                SermonUtility.HoldSermonTickCheckEnd(this.pawn, god, altar);
            });
        }

        protected void ThrowPreacherMote(Pawn pawn, GodDef god)
        {
            MoteBubble moteBubble2 = (MoteBubble)ThingMaker.MakeThing(ThingDefOf.Mote_Speech, null);
            moteBubble2.SetupMoteBubble(god.prayerMote, pawn);
            moteBubble2.Attach(pawn);
            GenSpawn.Spawn(moteBubble2, pawn.Position, pawn.Map);
        }
    }
}