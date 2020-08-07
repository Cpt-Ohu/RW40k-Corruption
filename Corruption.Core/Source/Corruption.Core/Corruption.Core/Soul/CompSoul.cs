using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Soul
{
    public class CompSoul : ThingComp
    {
        private static FloatRange CorruptionRange = new FloatRange(0f, 25000f);
        private float _corruption;

        /// <summary>
        /// Whether the player is allowed to see information about this soul
        /// </summary>
        public bool KnownToPlayer;

        public bool IsOnPilgrimage;
        private bool soulInitialized;

        public Soul_FavourTracker FavourTracker;

        public Pawn_PrayerTracker PrayerTracker;

        private PantheonDef _chosenPantheon;

        public PantheonDef ChosenPantheon
        {
            get { return _chosenPantheon; }
            set
            {
                _chosenPantheon = value;
                this.FavourTracker.TryAddGods(_chosenPantheon.GodsListForReading);
            }
        }

        public FloatRange LoyalistRange => new FloatRange(1f, CorruptionRange.max * SoulAfflictionDefOf.Tainted.Threshold - 2000f);

        public Pawn Pawn => this.parent as Pawn;

        public bool IsBlank
        {
            get
            {
                var sensitivityTrait = Pawn.story.traits.GetTrait(TraitDefOf.PsychicSensitivity);
                return sensitivityTrait != null && sensitivityTrait.Degree == -2;
            }
        }

        private float _cachedResistanceFactor = 1f;

        public void CacheCorruptionResistance()
        {
            float pawnKindFactor = 1f;
            if (this.Pawn.kindDef is CorruptionPawnKindDef cDef)
            {
                pawnKindFactor = cDef.affliction?.resolveFactor ?? 1f;
            }

            var sensitivityFactor = SoulUtility.PsychicSensitivityFactor(Pawn);
            _cachedResistanceFactor = pawnKindFactor * sensitivityFactor;

        }

        public void GainCorruption(float change, GodDef favouredGod = null, bool addFavour = true)
        {
            var adjustedChange = change * _cachedResistanceFactor;
            this._corruption = CorruptionRange.ClampToRange(this._corruption += adjustedChange);
            if (favouredGod != null &&  Find.TickManager.TicksGame > 0)
            {
                this.TryAddFavorProgress(favouredGod, Math.Abs(change * 0.076f));
            }
            if (this.Corrupted)
            {
                this.FallToChaos();
            }
            else if (adjustedChange < 0f)
            {
                var chaosFavour = this.FavourTracker.AllFavoursSorted().FirstOrDefault(x => PantheonDefOf.Chaos.GodsListForReading.Contains(x.God));
                if (chaosFavour != null)
                {
                    this.FavourTracker.TryAddProgressFor(chaosFavour.God, change * 0.1f);
                }
            }
            LessonAutoActivator.TeachOpportunity(CoreConceptDefOf.CorruptionKnowledge, OpportunityType.GoodToKnow);


        }

        private void FallToChaos()
        {
            if (this.ChosenPantheon != PantheonDefOf.Chaos)
            {
                this.ChosenPantheon = PantheonDefOf.Chaos;
            }
        }

        public float CorruptionLevel => this._corruption / CorruptionRange.max;

        public CompProperties_Soul Props => this.props as CompProperties_Soul;

        public CompSoul()
        {
            this.FavourTracker = new Soul_FavourTracker(this);
            this.PrayerTracker = new Pawn_PrayerTracker(this);
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.ChosenPantheon = this.Props?.defaultPantheon ?? PantheonDefOf.ImperialCult;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!this.soulInitialized && Pawn.RaceProps.Humanlike)
            {
                CorruptionPawnKindDef corruptionKind = this.Pawn.kindDef as CorruptionPawnKindDef;
                if (corruptionKind != null && corruptionKind.affliction != null)
                {
                    foreach (var favour in corruptionKind.affliction.favorProgressTemplates)
                    {
                        this.TryAddFavorProgress(favour.god, favour.initialProgressRange.RandomInRange);
                    }

                    this.GainCorruption(corruptionKind.affliction.afflictionRange.RandomInRange);
                }
            }
        }


        public int DevotionDegree
        {
            get
            {
                return this.Pawn.story.traits.DegreeOfTrait(SoulTraitDefOf.Devotion);
            }
        }

        public TraitDegreeData DevotionDegreeData
        {
            get
            {
                var devotion = this.Pawn.story.traits.GetTrait(SoulTraitDefOf.Devotion);
                if (devotion == null)
                {
                    return null;
                }
                return devotion.CurrentData;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (this.Pawn.RaceProps.Humanlike && this.Pawn.IsHashIntervalTick(60))
            {
                this.CheckEmotions();
            }
            this.PrayerTracker?.PrayerTick();
        }

        private void CheckEmotions()
        {
            float moodLevel = this.Pawn.needs.mood.CurLevel;
            float moodOffset = this.Pawn.needs.mood.thoughts.TotalMoodOffset();
            if (moodOffset > 20f && moodLevel > 0.9f)
            {
                this.GainCorruption(0.2f, GodDefOf.Slaanesh);
            }
            else if (moodOffset < -20f && moodLevel < 0.5f)
            {
                this.GainCorruption(0.2f, GodDefOf.Nurgle);
            }
            if (this.Pawn.health.hediffSet.AnyHediffMakesSickThought)
            {
                this.GainCorruption(0.5f, GodDefOf.Nurgle);
            }

            if (this.Pawn.CurJobDef == JobDefOf.Meditate)
            {
                GodDef god = this.ChosenPantheon.GodsListForReading.RandomElementByWeight(x => 1f + this.FavourTracker.FavourValueFor(x) * 10f);
                this.GainCorruption(god.favourCorruptionFactor, god);
            }
            if (this.Pawn.CurJobDef == JobDefOf.Research)
            {
                this.GainCorruption(0.2f, GodDefOf.Tzeentch);
            }
            if (this.Pawn.CurJobDef == JobDefOf.Lovin)
            {
                this.GainCorruption(0.5f, GodDefOf.Slaanesh);
            }
            if (this.Pawn.IsFighting())
            {
                this.GainCorruption(1f, GodDefOf.Khorne);
            }
        }

        public bool Corrupted => this.CorruptionLevel >= SoulAfflictionDefOf.Corrupted.Threshold;

        public void InitializeForPawn()
        {
            CorruptionPawnKindDef cdef = this.Pawn.kindDef as CorruptionPawnKindDef;
            if (cdef != null && cdef.affliction != null)
            {
                this.ChosenPantheon = cdef.affliction.forcedPantheon;
                foreach (var progressTemplate in cdef.affliction.favorProgressTemplates)
                {
                    this.TryAddFavorProgress(progressTemplate.god, progressTemplate.initialProgressRange.RandomInRange);
                }

                this.GainCorruption(cdef.affliction.afflictionRange.RandomInRange);

            }
            else
            {
                this.InitializeDefaultPawn();
            }
            this.CacheCorruptionResistance();
        }

        internal void InitializeDefaultPawn()
        {
            if (this.Props == null || this.Props.defaultPantheon == null)
            {
                if (Rand.Value > 0.01f)
                {
                    this.ChosenPantheon = PantheonDefOf.ImperialCult;
                    this.GainCorruption(this.LoyalistRange.RandomInRange);
                }
                else
                {
                    this.ChosenPantheon = PantheonDefOf.Chaos;
                    var god = PantheonDefOf.Chaos.GodsListForReading.RandomElement();
                    this.GainCorruption(FavourProgress.ProgressRange.RandomInRange, god);
                }
            }
        }

        public void TryAddFavorProgress(GodDef god, float change)
        {
            if (this.Pawn.RaceProps.Humanlike)
            {
                if (this.FavourTracker == null) this.FavourTracker = new Soul_FavourTracker(this);
                this.FavourTracker.TryAddProgressFor(god, change);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<Soul_FavourTracker>(ref this.FavourTracker, "WorshipTracker", this);
            Scribe_Deep.Look<Pawn_PrayerTracker>(ref this.PrayerTracker, "PrayerTracker", this);
            Scribe_Values.Look<float>(ref this._cachedResistanceFactor, "resistanceFactor", 1f);
            Scribe_Defs.Look<PantheonDef>(ref this._chosenPantheon, "chosenPantheon");
            Scribe_Values.Look<bool>(ref soulInitialized, "soulInitialized");
            Scribe_Values.Look<float>(ref _corruption, "corruption");

        }

        public static void TryDiscoverAlignment(Pawn investigator, Pawn target, float modifier)
        {
            CompSoul soul = target.Soul();
            if (soul != null && !soul.KnownToPlayer)
            {
                int socialSkillDifference = CorruptionStoryTrackerUtilities.SocialSkillDifference(investigator, target);
                float skillFactor = socialSkillDifference / 20f;
                if (Rand.Value < 0.2f + skillFactor + modifier)
                {
                    soul.DiscoverAlignment();
                }
                else
                {
                    soul.AlignmentDiscoveryFailure(investigator, target, socialSkillDifference);
                }
            }
        }

        private void AlignmentDiscoveryFailure(Pawn investigator, Pawn target, int socialSkillDifference)
        {
            this.KnownToPlayer = false;
        }

        private void DiscoverAlignment()
        {
            this.KnownToPlayer = true;
        }
    }

    public class CompProperties_Soul : CompProperties
    {
        public PantheonDef defaultPantheon;

        public CompProperties_Soul()
        {
            this.compClass = typeof(CompSoul);
        }
    }
}
