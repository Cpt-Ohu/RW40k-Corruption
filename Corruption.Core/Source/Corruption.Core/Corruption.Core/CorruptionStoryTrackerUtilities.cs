using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public static class CorruptionStoryTrackerUtilities
    {
        public static Faction IoM_NPC => Find.FactionManager.FirstFactionOfDef(FactionsDefOf.IoM_NPC);

        public static int SocialSkillDifference(Pawn first, Pawn second)
        {
            int firstSkill = first.skills.GetSkill(SkillDefOf.Social).levelInt;
            int secondSkill = second.skills.GetSkill(SkillDefOf.Social).levelInt;

            return firstSkill - secondSkill;
        }
    }
}
