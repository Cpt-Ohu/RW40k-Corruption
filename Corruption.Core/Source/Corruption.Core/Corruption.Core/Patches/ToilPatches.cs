using Corruption.Core.Gods;
using Corruption.Core.Soul;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Corruption.Core.Patches
{
    //[HarmonyPatch(typeof(Toils_Recipe))]
    //[HarmonyPatch("DoRecipeWork")]
    //public static class Toils_Recipe_DoWork
    //{
    //    private static void ToilPrayerPostfix(Toil __result)
    //    {
    //        CompSoul soul = __result.actor?.Soul();
    //        if (soul != null)
    //        {
    //            __result.AddPreTickAction(delegate
    //            {
    //                GodDef target = soul.ChosenPantheon.GodsListForReading.RandomElementByWeight(x => soul.FavourTracker.FavourValueFor(x) + 1f);
    //                SoulUtility.ThrowPrayerMote(__result.actor, target);
    //                soul.GainCorruption(target.favourCorruptionFactor, target);
    //            });
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Toils_Haul))]
    //[HarmonyPatch("CarryHauledThingToCell")]
    //public static class Toils_Haul_CarryToCell
    //{
    //    private static void ToilPrayerPostfix(Toil __result)
    //    {
    //        CompSoul soul = __result.actor?.Soul();
    //        if (soul != null)
    //        {
    //            __result.AddPreTickAction(delegate
    //            {
    //                GodDef target = soul.ChosenPantheon.GodsListForReading.RandomElementByWeight(x => soul.FavourTracker.FavourValueFor(x) + 1f);
    //                SoulUtility.ThrowPrayerMote(__result.actor, target);
    //                soul.GainCorruption(target.favourCorruptionFactor, target);
    //            });
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Toils_Haul))]
    //[HarmonyPatch("CarryHauledThingToCell")]
    //public static class Toils_Haul_CarryToCell
    //{
    //    private static void ToilPrayerPostfix(Toil __result)
    //    {
    //        CompSoul soul = __result.actor?.Soul();
    //        if (soul != null)
    //        {
    //            __result.AddPreTickAction(delegate
    //            {
    //                GodDef target = soul.ChosenPantheon.GodsListForReading.RandomElementByWeight(x => soul.FavourTracker.FavourValueFor(x) + 1f);
    //                SoulUtility.ThrowPrayerMote(__result.actor, target);
    //                soul.GainCorruption(target.favourCorruptionFactor, target);
    //            });
    //        }
    //    }
    //}
}
