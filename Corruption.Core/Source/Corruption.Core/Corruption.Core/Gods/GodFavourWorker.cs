using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core.Gods
{
    public class GodFavourWorker
    {
        public virtual void PostGainFavour(CompSoul soul, float change, GodDef god = null)
        { 

        }
    }

    public class GodFavourWorker_Khorne : GodFavourWorker
    {
        public override void PostGainFavour(CompSoul soul, float change, GodDef god = null)
        {
            base.PostGainFavour(soul, change);
            if (soul.Pawn.Spawned && soul.CorruptionLevel >= SoulAfflictionDefOf.Tainted.Threshold  && soul.Pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(ThoughtDefOf.KilledHumanlikeBloodlust) == null)
            {
                Thought_MemoryBloodlust thought = (Thought_MemoryBloodlust)ThoughtMaker.MakeThought(ThoughtDefOf.KilledHumanlikeBloodlust, 1);
                soul.Pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
            }
        }
    }
}
