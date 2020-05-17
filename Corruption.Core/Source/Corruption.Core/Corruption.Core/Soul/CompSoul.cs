using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Soul_FavourTracker FavourTracker;

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
                pawnKindFactor = cDef.affliction.resolveFactor;
            }

            var vanillaPsychicTrait = this.Pawn.story.traits.allTraits.FirstOrDefault(x => x.def == TraitDefOf.PsychicSensitivity);
            var sensitivityFactor = SoulUtility.PsychicSensitivityFactor(Pawn);
            _cachedResistanceFactor = pawnKindFactor * sensitivityFactor;

        }

        public void GainCorruption(float change)
        {
            var adjustedChange = change * _cachedResistanceFactor;
            this._corruption = CorruptionRange.ClampToRange(this._corruption += adjustedChange);
        }

        public float CorruptionLevel => this._corruption / CorruptionRange.max;

        public CompSoul()
        {
            this.FavourTracker = new Soul_FavourTracker(this);
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
        }

        public SoulAffliction Affliction
        {
            get
            {
                if (this.CorruptionLevel == 1.0f)
                {
                    return SoulAffliction.Lost;
                }
                if (this.CorruptionLevel > 0.9f)
                {
                    return SoulAffliction.Corrupted;
                }
                if (this.CorruptionLevel > 0.7f)
                {
                    return SoulAffliction.Tainted;
                }
                if (this.CorruptionLevel > 0.3f)
                {
                    return SoulAffliction.Warptouched;
                }
                if (this.CorruptionLevel > 0.1f)
                {
                    return SoulAffliction.Intrigued;
                }
                return SoulAffliction.Pure;
            }
        }

        public bool Corrupted => this.Affliction >= SoulAffliction.Corrupted;

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

        public bool IsOnPilgrimage;

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

                float afup = cdef.affliction.upperAfflictionLimit;
                float afdown = cdef.affliction.lowerAfflictionLimit;
                this._corruption = (Rand.Range(afup, afdown));

            }
            else
            {
                this.InitializeDefaultPawn();
            }
            this.CacheCorruptionResistance();
        }

        internal void InitializeDefaultPawn()
        {
            float pNum = Rand.GaussianAsymmetric(2.5f, 0.45f, 2);
            if (pNum < 0)
            {
                pNum = 0;
            }
            else if (pNum > 7)
            {
                pNum = 7;
            }

            if (Rand.Value > 0.1f)
            {
                this.ChosenPantheon = PantheonDefOf.ImperialCult;
                this.FavourTracker.TryAddProgressFor(GodDefOf.Emperor, Rand.Range(0.02f, 0.6f));
            }
        }

        public void TryAddFavorProgress(GodDef god, float change)
        {
            this.FavourTracker.TryAddProgressFor(god, change);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<Soul_FavourTracker>(ref this.FavourTracker, "WorshipTracker", new object[] { this });
            Scribe_Values.Look<float>(ref this._cachedResistanceFactor, "resistanceFactor", 1f);
            Scribe_Defs.Look<PantheonDef>(ref this._chosenPantheon, "chosenPantheon");
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
            throw new NotImplementedException();
        }

        private void DiscoverAlignment()
        {
            throw new NotImplementedException();
        }
    }
}
