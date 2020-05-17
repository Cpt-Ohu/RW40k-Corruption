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
    }
}
