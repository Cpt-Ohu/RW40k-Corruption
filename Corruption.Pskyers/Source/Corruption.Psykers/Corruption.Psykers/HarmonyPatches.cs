using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Corruption.Core.Soul;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Corruption.Psykers
{
    [StaticConstructorOnStartup]
    public class HarmonyPatches
    {

        static HarmonyPatches()
        {
            Log.Message("Generating Corruption Core Patches");
            Harmony harmony = new Harmony("rimworld.ohu.corruption.core");
            harmony.Patch(AccessTools.Method(typeof(Verse.Pawn_EquipmentTracker), "Notify_EquipmentAdded", null), null, new HarmonyMethod(typeof(HarmonyPatches), "Notify_EquipmentAddedPostfix", null));
            harmony.Patch(AccessTools.Method(typeof(RimWorld.Pawn_ApparelTracker), "Wear", null), null, new HarmonyMethod(typeof(HarmonyPatches), "WearPostfix", null));
            harmony.Patch(AccessTools.Method(typeof(Verse.Pawn), "PreApplyDamage", null), new HarmonyMethod(typeof(HarmonyPatches), "PreApplyDamagePrefix", null));

            harmony.Patch(AccessTools.Method(typeof(Verse.ArmorUtility), "GetPostArmorDamage", null), new HarmonyMethod(typeof(HarmonyPatches), "GetPostArmorDamagePrefix"), null, null);

        }

        private static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            Pawn pawn = __instance.pawn;
            CompImbuedAbility compImbued = eq.GetComp<CompImbuedAbility>();
            if (compImbued != null)
            {
                foreach (var ability in compImbued.ImbuedAbilities)
                {
                    pawn.abilities.GainAbility(ability);
                }
            }
        }

        private static void WearPostfix(Apparel newApparel, bool dropReplacedApparel = true, bool locked = false)
        {
            Pawn pawn = newApparel.Wearer;
            if (pawn != null)
            {
                CompImbuedAbility compImbued = newApparel.GetComp<CompImbuedAbility>();
                if (compImbued != null)
                {
                    foreach (var ability in compImbued.ImbuedAbilities)
                    {
                        pawn.abilities.GainAbility(ability);
                    }
                }
            }
        }

        private static bool GetPostArmorDamagePrefix(Pawn pawn, float amount, float armorPenetration, BodyPartRecord part, ref DamageDef damageDef, out bool deflectedByMetalArmor, out bool diminishedByMetalArmor, float __result)
        {
            Hediff misfortuneHediff = pawn.health.hediffSet.GetFirstHediffOfDef(Corruption.Psykers.HediffDefOf.DiviMisfortune);

            deflectedByMetalArmor = false;
            diminishedByMetalArmor = false;

            if (misfortuneHediff != null)
            {
                armorPenetration = armorPenetration -= misfortuneHediff.CurStage.statOffsets.GetStatOffsetFromList(StatDefOf.ArmorRating_Sharp);
                __result = ArmorUtility.GetPostArmorDamage(pawn, amount, armorPenetration, part, ref damageDef, out deflectedByMetalArmor, out diminishedByMetalArmor);
                return false;
            }
            return true;
        }

        private static bool PreApplyDamagePrefix(ref bool absorbed, ref DamageInfo dinfo, Pawn __instance)
        {
            var shieldHediffs = __instance.health.hediffSet.GetAllComps().Where(x => x is HediffComp_Shield);
            foreach (var hediff in shieldHediffs)
            {
                ((HediffComp_Shield)hediff).TryAbsorbDamage(ref dinfo, out absorbed);
                if (absorbed)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
