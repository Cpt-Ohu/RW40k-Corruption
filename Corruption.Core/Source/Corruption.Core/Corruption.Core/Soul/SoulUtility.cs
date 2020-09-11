using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Soul
{
    public static class SoulUtility
    {
        public static float PsychicSensitivityFactor(Trait psychicTrait)
        {
            if (psychicTrait == null) return 1f;
            var sensitivityOffset = psychicTrait.OffsetOfStat(StatDefOf.PsychicSensitivity);
            return Mathf.Clamp(1f + sensitivityOffset, 0.01f, 2f);
        }

        internal static float PsychicSensitivityFactor(Pawn pawn)
        {
            return pawn.GetStatValue(StatDefOf.PsychicSensitivity);
        }


        internal static void ThrowPrayerMote(Pawn pawn, GodDef god)
        {
            MoteBubble moteBubble2 = (MoteBubble)ThingMaker.MakeThing(ThingDefOf.Mote_Speech, null);
            moteBubble2.SetupMoteBubble(god.PrayerMote, pawn);
            moteBubble2.Attach(pawn);
            GenSpawn.Spawn(moteBubble2, pawn.Position, pawn.Map);
        }
    }
}
