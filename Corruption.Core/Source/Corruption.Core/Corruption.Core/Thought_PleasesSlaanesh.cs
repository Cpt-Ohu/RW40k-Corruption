using Corruption.Core.Gods;
using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corruption.Core
{
    public class Thought_PleasesSlaanesh : Thought_Memory
    {
        public override bool TryMergeWithExistingMemory(out bool showBubble)
        {
            CompSoul soul = this.pawn.Soul();
            if (soul != null)
            {
                soul.GainCorruption(((100f + this.CurStage.baseMoodEffect) * 2f), GodDefOf.Slaanesh);
            }
            return base.TryMergeWithExistingMemory(out showBubble);
        }
    }

    public class Thought_PleasesNurgle : Thought_Memory
    {
        public override bool TryMergeWithExistingMemory(out bool showBubble)
        {
            CompSoul soul = this.pawn.Soul();
            if (soul != null)
            {
                soul.GainCorruption(((100f + this.CurStage.baseMoodEffect) * 2f), GodDefOf.Nurgle);
            }
            return base.TryMergeWithExistingMemory(out showBubble);
        }
    }

    public class Thought_PleasesTzeentch : Thought_Memory
    {
        public override bool TryMergeWithExistingMemory(out bool showBubble)
        {
            CompSoul soul = this.pawn.Soul();
            if (soul != null)
            {
                soul.GainCorruption(((100f + this.CurStage.baseMoodEffect) * 2f), GodDefOf.Nurgle);
            }
            return base.TryMergeWithExistingMemory(out showBubble);
        }
    }

    public class Thought_PleasesKhorne : Thought_Memory
    {
        public override bool TryMergeWithExistingMemory(out bool showBubble)
        {
            CompSoul soul = this.pawn.Soul();
            if (soul != null)
            {
                soul.GainCorruption(((100f + this.CurStage.baseMoodEffect) * 2f), GodDefOf.Nurgle);
            }
            return base.TryMergeWithExistingMemory(out showBubble);
        }
    }
}
