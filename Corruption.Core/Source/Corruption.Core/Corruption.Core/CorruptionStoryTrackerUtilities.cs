using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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


        //public static void AffectGoodwillWithSpacerFaction(Faction faction, Faction other, float goodwillChange)
        //{
        //    if (goodwillChange > 0f && ((faction.IsPlayer && SettlementUtility.IsPlayerAttackingAnySettlementOf(other)) || (other.IsPlayer && SettlementUtility.IsPlayerAttackingAnySettlementOf(faction))))
        //    {
        //        return;
        //    }
        //    float value = other.GoodwillWith(faction) + goodwillChange;
        //    FactionRelation factionRelation = other.RelationWith(faction, false);
        //    factionRelation.goodwill = (int)Mathf.Clamp(value, -100f, 100f);

        //    if (!faction.HostileTo(other) && faction.GoodwillWith(other) < -80f)
        //    {
        //        faction.SetHostileTo(other, true);
        //        if (Current.ProgramState == ProgramState.Playing && Find.TickManager.TicksGame > 100 && other == Faction.OfPlayer)
        //        {
        //            Find.LetterStack.ReceiveLetter("LetterLabelRelationsChangeBad".Translate(), "RelationsBrokenDown".Translate(new object[]
        //            {
        //        faction.Name
        //            }), LetterDefOf.NegativeEvent, null);
        //        }
        //    }
        //    if (faction.HostileTo(other) && faction.GoodwillWith(other) > 0f)
        //    {
        //        faction.SetHostileTo(other, false);
        //        if (Current.ProgramState == ProgramState.Playing && Find.TickManager.TicksGame > 100 && other == Faction.OfPlayer)
        //        {
        //            Find.LetterStack.ReceiveLetter("LetterLabelRelationsChangeGood".Translate(), "RelationsWarmed".Translate(new object[]
        //            {
        //        faction.Name
        //            }), LetterDefOf.NegativeEvent, null);
        //        }
        //    }
        //}


    }
}
