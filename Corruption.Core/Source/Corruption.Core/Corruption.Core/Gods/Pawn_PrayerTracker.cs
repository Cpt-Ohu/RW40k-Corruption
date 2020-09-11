using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Corruption.Core.Gods
{
    public class Pawn_PrayerTracker : IExposable
    {
        public const int COOLDOWN_TICKS = 2500;

        private PrayerDef currentPrayer;
        private bool isPraying;
        private int currentLineIndex;
        private CompSoul compSoul;
        private Job startedWithJob;
        private int cooldownTicks;
        public bool ShowPrayer = true;
        public bool AllowPraying = true;

        public GodDef PreferredGod;

        public PrayerDef lastPrayer;
        public int Cooldown => cooldownTicks;
        public float CooldownPercentage => 1f - ((float)this.Cooldown / (float)COOLDOWN_TICKS);


        private float AccumulatedEffect
        {
            get
            {
                if (this.currentPrayer == null) return 0f;
                return this.currentPrayer.ticksPerLine * 0.1f * this.currentPrayer.dedicatedTo.favourCorruptionFactor * (this.currentLineIndex + 1f);
            }
        }

        public Pawn_PrayerTracker(CompSoul compSoul)
        {
            this.compSoul = compSoul;
        }

        private Vector3 MoteCenter => compSoul.Pawn.TrueCenter() + new Vector3(0f, 0f, 0.7f);

        public void PrayerTick()
        {
            if (this.cooldownTicks > 0)
            {
                this.cooldownTicks--;
            }

            if (isPraying && this.compSoul.Pawn.IsHashIntervalTick(this.currentPrayer.ticksPerLine))
            {
                if (this.startedWithJob != null && this.compSoul.Pawn.CurJob != this.startedWithJob)
                {
                    this.FinishPrayer(0.66f);
                }
                this.AdvancePrayer();
            }
        }

        private void AdvancePrayer()
        {
            if (this.currentLineIndex == this.currentPrayer.prayerLines.Count)
            {
                this.FinishPrayer();
            }
            else if (this.ShowPrayer)
            {
                MoteAttachedText.ThrowText(this.compSoul.Pawn, this.MoteCenter, this.compSoul.Pawn.Map, this.currentPrayer.prayerLines[currentLineIndex], this.currentPrayer.dedicatedTo.mainColor);
            }
            this.currentLineIndex++;
        }

        private void FinishPrayer(float gainFactor = 1f)
        {
            this.isPraying = false;
            compSoul.Pawn.Soul()?.GainCorruption(AccumulatedEffect * gainFactor, this.currentPrayer.dedicatedTo);
            this.currentLineIndex = 0;
            this.lastPrayer = this.currentPrayer;
            this.currentPrayer = null;
            this.cooldownTicks = COOLDOWN_TICKS;
        }

        public void StartPrayer(PrayerDef prayerDef, bool forced = false)
        {
            LessonAutoActivator.TeachOpportunity(CoreConceptDefOf.Prayers, OpportunityType.GoodToKnow);
            if (forced || (this.AllowPraying && !this.isPraying && this.cooldownTicks == 0))
            {
                this.currentLineIndex = 0;
                this.currentPrayer = prayerDef;
                this.isPraying = true;
            }
        }

        public void StartRandomPrayer(Job initializedJob = null, bool forced = false, WorkTags? forWorkTag = null)
        {
            if (!this.AllowPraying || this.isPraying || this.cooldownTicks > 0)
            {
                return;
            }
            GodDef god = this.PreferredGod ?? this.compSoul.ChosenPantheon.GodsListForReading.RandomElementByWeight(x => 1f + this.compSoul.FavourTracker.FavourValueFor(x));
            if (god != null)
            {
                var potentialPrayers = DefDatabase<PrayerDef>.AllDefsListForReading.Where(x => x.dedicatedTo == god && ValidatePrayer(initializedJob, forWorkTag, x));
                if (potentialPrayers.Count() > 0)
                {
                    PrayerDef prayerDef = potentialPrayers.RandomElement();
                    if (prayerDef != null)
                    {
                        this.StartPrayer(prayerDef, forced);
                    }
                }
            }
        }


        private bool ValidatePrayer(Job initializedJob, WorkTags? forWorkTag, PrayerDef x)
        {
            if (forWorkTag != null)
            {
                return forWorkTag.Value.HasFlag(x.preferredWorktags);
            }
            if (initializedJob != null && initializedJob.workGiverDef != null)
            {
                return initializedJob.workGiverDef.workTags.HasFlag(x.preferredWorktags) || initializedJob.workGiverDef.workType.workTags.HasFlag(x.preferredWorktags);
            }
            return false;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look<PrayerDef>(ref this.currentPrayer, "currentPrayer");
            Scribe_Defs.Look<PrayerDef>(ref this.lastPrayer, "lastPrayer");
            Scribe_References.Look<Job>(ref this.startedWithJob, "job");
            Scribe_Values.Look<int>(ref this.currentLineIndex, "currentLineIndex");
            Scribe_Values.Look<bool>(ref this.isPraying, "isPraying");
            Scribe_Values.Look<bool>(ref this.ShowPrayer, "ShowPrayer");
            Scribe_Values.Look<bool>(ref this.AllowPraying, "AllowPraying");
            Scribe_Values.Look<int>(ref this.cooldownTicks, "countDownToPray");
        }
    }
}
