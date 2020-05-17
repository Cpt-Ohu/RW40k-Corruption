using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using System.Reflection;
using Corruption.Core.Soul;

namespace Corruption.Core
{
    [StaticConstructorOnStartup]
    public class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Log.Message("Generating Corruption Core Patches");
            Harmony harmony = new Harmony("rimworld.ohu.corruption.core");

            harmony.Patch(AccessTools.Method(typeof(Verse.PawnGenerator), "GenerateTraits", null), null, new HarmonyMethod(typeof(HarmonyPatches), "GenerateTraitSoulPostfix", null));
            //harmony.Patch(AccessTools.Method(typeof(Verse.PawnGraphicSet), "ResolveApparelGraphics", null), new HarmonyMethod(typeof(HarmonyPatches), "ResolveApparelGraphicsOriginal"), null);
            //harmony.Patch(AccessTools.Method(typeof(Verse.PawnRenderer), "DrawEquipmentAiming"), new HarmonyMethod(typeof(HarmonyPatches), "DrawEquipmentAimingModded"), null);
            //harmony.Patch(AccessTools.Method(typeof(RimWorld.Page_ConfigureStartingPawns), "CanDoNext"), null, new HarmonyMethod(typeof(HarmonyPatches), "WorldGeneratePostfix"), null);

        }

        private static void GenerateTraitSoulPostfix(Pawn pawn, PawnGenerationRequest request)
        {
            CompSoul soul = pawn.TryGetComp<CompSoul>();
            if (soul != null)
            {
                soul.InitializeForPawn();
            }
        }
    }
}
