using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class Thought_MemoryBloodlust : Thought_Memory
    {
        private static SimpleCurve overkillFactor = new SimpleCurve(new List<CurvePoint>() {
            new CurvePoint(1f,1f),
            new CurvePoint(2f,1f),
            new CurvePoint(3f,1f),
            new CurvePoint(4f,2f),
            new CurvePoint(5f,2.5f),
            new CurvePoint(6f,3f),

        });

        private const int firstPositiveStage = 4;

        private int lastKillTick;

        private int curStageKillMemory;

        private int lastCheckTick;

        public override bool ShouldDiscard => (base.ShouldDiscard && this.CurStageIndex == 0);

        public Thought_MemoryBloodlust()
        {
            this.lastCheckTick = Find.TickManager.TicksGame;
        }

        public override void ThoughtInterval()
        {
            base.ThoughtInterval();
            if (this.pawn.story.traits.HasTrait(CorruptionTraitDefOf.Khorne_Fervor))
            {
                if (this.CurStageIndex == 0)
                {
                    this.SetForcedStage(firstPositiveStage - 1);
                }


                if (Find.TickManager.TicksGame > lastCheckTick + this.def.DurationTicks / this.StageCooldownFactor)
                {
                    this.curStageKillMemory = 0;
                    this.lastCheckTick = Find.TickManager.TicksGame;
                    this.SetForcedStage(Math.Max(1, this.CurStageIndex - 1));
                }
            }
        }

        public float StageCooldownFactor
        {
            get
            {
                return Math.Max(1, this.CurStageIndex - firstPositiveStage);
            }
        }

        public override bool TryMergeWithExistingMemory(out bool showBubble)
        {
            if (this.CurStageIndex == 0 && !this.pawn.story.traits.HasTrait(CorruptionTraitDefOf.Khorne_Fervor))
            {
                return base.TryMergeWithExistingMemory(out showBubble);
            }
            else
            {
                this.lastKillTick = Find.TickManager.TicksGame;
                ThoughtHandler thoughts = pawn.needs.mood.thoughts;
                Thought_MemoryBloodlust existingThought = thoughts.memories.OldestMemoryOfDef(this.def) as Thought_MemoryBloodlust;
                if (existingThought != null)
                {
                    existingThought.UpdateMemory();
                    showBubble = true;
                    return true;
                }
                else
                {
                    this.UpdateMemory();
                    showBubble = true;
                    return false;
                }
            }
        }

        private void UpdateMemory()
        {
            this.curStageKillMemory++;
            float overkill = (this.curStageKillMemory - killTickLevels[this.CurStageIndex]);
            this.moodPowerFactor = Math.Abs(1f + (this.curStageKillMemory - killTickLevels[this.CurStageIndex]) * overkillFactor.Evaluate(this.CurStageIndex));
            this.lastCheckTick = Find.TickManager.TicksGame;
            if (this.ShouldIncreaseStage || this.CurStageIndex < firstPositiveStage)
            {
                this.SetForcedStage(Math.Max(this.CurStageIndex +1, firstPositiveStage));
            }
        }

        public bool ShouldIncreaseStage
        {
            get
            {
                return this.curStageKillMemory >= killTickLevels[this.CurStageIndex];
            }
        }

        private static Dictionary<int, int> killTickLevels = new Dictionary<int, int>()
        {
            {0, 0 },
            {1, 0 },
            {2, 0 },
            {3, 0 },
            {4, 1 },
            {5, 5 },
            {6, 10 }
        };

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref lastKillTick, "lastKillTick");
            Scribe_Values.Look<int>(ref lastCheckTick, "lastCheckTick");
            Scribe_Values.Look<int>(ref curStageKillMemory, "curStageKillMemory");
        }
    }
}
