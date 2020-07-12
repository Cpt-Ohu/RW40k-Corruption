using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class ThoughtWorker_PsychicBlank : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            int ownDegree = other.story?.traits?.GetTrait(TraitDefOf.PsychicSensitivity)?.Degree ?? -1;
            int otherDegree = other.story?.traits?.GetTrait(TraitDefOf.PsychicSensitivity)?.Degree ?? -1;

            if (ownDegree != -1)
            {
                return false;
            }

            if (otherDegree == -1)
            {
                return false;
            }

            if (otherDegree == 2)
            {
                return ThoughtState.ActiveAtStage(0);
            }
            else if (otherDegree == 3)
            {
                return ThoughtState.ActiveAtStage(1);
            }

            return false;
        }
    }
}
