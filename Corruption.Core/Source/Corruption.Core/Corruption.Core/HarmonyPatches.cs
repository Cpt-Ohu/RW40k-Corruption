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
using Corruption.Core.Gods;
using Verse.AI;
using System.Reflection.Emit;

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
            harmony.Patch(AccessTools.Method(typeof(Verse.PawnGraphicSet), "ResolveApparelGraphics", null), null, new HarmonyMethod(typeof(HarmonyPatches), "ResolveAllGraphicsPostfix"), null);
            harmony.Patch(AccessTools.Method(typeof(RimWorld.MemoryThoughtHandler), "TryGainMemory", new Type[] { typeof(Thought_Memory), typeof(Pawn) }), null, new HarmonyMethod(typeof(HarmonyPatches), "TryGainMemoryPostfix", null));
            harmony.Patch(AccessTools.Method(typeof(RimWorld.JobGiver_Work), "TryIssueJobPackage", null), null, new HarmonyMethod(typeof(HarmonyPatches), "TryIssueJobPackagePostfix", null));

        }

        private static void GenerateTraitSoulPostfix(Pawn pawn, PawnGenerationRequest request)
        {
            CompSoul soul = pawn.TryGetComp<CompSoul>();
            if (soul != null)
            {
                soul.InitializeForPawn();
            }
        }

        private static void ResolveAllGraphicsPostfix(PawnGraphicSet __instance)
        {
            CompSoul soul = __instance.pawn?.Soul();
            if (soul != null && __instance.pawn.RaceProps.Humanlike)
            {
                foreach (var hediffComp in __instance.pawn.health.hediffSet.GetAllComps().Where(x => x is HediffComp_DrawPawnExtra))
                {
                    HediffComp_DrawPawnExtra extraDraw = hediffComp as HediffComp_DrawPawnExtra;
                    if (extraDraw != null && extraDraw.parent.Severity >= extraDraw.Props.minSeverity)
                    {
                        __instance.apparelGraphics.Insert(0, new ApparelGraphicRecord(extraDraw.Graphic, extraDraw.FakeApparel));
                        if (extraDraw.Props.keepHair && extraDraw.Props.templateApparelDef.apparel.LastLayer == ApparelLayerDefOf.Overhead && !__instance.apparelGraphics.Any(x => x.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead))
                        {
                            __instance.apparelGraphics.Insert(0, new ApparelGraphicRecord(__instance.pawn.Drawer.renderer.graphics.hairGraphic, extraDraw.FakeApparel));
                        }
                    }
                }
                foreach (var hediffComp in __instance.pawn.health.hediffSet.GetAllComps().Where(x => x is HediffComp_AffectSkin))
                {
                    HediffComp_AffectSkin skinComp = hediffComp as HediffComp_AffectSkin;

                    if (skinComp != null)
                    {
                        Color color = skinComp.Props.useSkinColor ? skinComp.Props.skinColor : __instance.pawn.story.SkinColor;

                        string bodyPath = __instance.pawn.story.bodyType.bodyNakedGraphicPath;
                        if (skinComp.Props.bodyPath != null) bodyPath = string.Join("_", skinComp.Props.bodyPath, __instance.pawn.story.bodyType.defName);

                        __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.CutoutSkin, Vector2.one, color);

                        string headPath = __instance.pawn.story.HeadGraphicPath;
                        if (skinComp.Props.headPath != null) headPath = string.Join("_", skinComp.Props.headPath, __instance.pawn.story.crownType.ToString());
                        __instance.headGraphic = GraphicDatabaseHeadRecords.GetHeadNamed(headPath, color);

                    }
                }
            }
        }

        private static void TryGainMemoryPostfix(MemoryThoughtHandler __instance, Thought_Memory newThought, Pawn otherPawn = null)
        {
            if (!ThoughtUtility.CanGetThought_NewTemp(__instance.pawn, newThought.def))
            {
                return;
            }
            CompSoul soul = __instance.pawn.Soul();
            if (soul != null)
            {
                foreach (var god in DefDatabase<GodDef>.AllDefsListForReading.Where(x => x.pleasedByThought.Contains(newThought.def) || x.pleasedByThoughtTags.Any(y => newThought.def.defName.Contains(y))))
                {
                    soul.GainCorruption(god.favourCorruptionFactor  * 20 * Math.Abs(newThought.CurStage.baseMoodEffect / 10f), god);
                }
            }
        }

        private static void TryIssueJobPackagePostfix(Pawn pawn, JobIssueParams jobParams, ThinkResult __result)
        {
            if (__result != ThinkResult.NoJob && __result.Job != null)
            {
                CompSoul soul = pawn.Soul();
                if(soul != null)
                {
                    soul.PrayerTracker?.StartRandomPrayer(__result.Job);
                }
            }
        }

    }

    //[HarmonyPatch(typeof(PawnGraphicSet), "RenderPawnInternal")]
    //public static class PawnGraphicSet_ResolveApparelGraphics_Patch
    //{
    //    [HarmonyTranspiler]
    //    static IEnumerable<CodeInstruction> ResolveApparelGraphics_Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        List<CodeInstruction> ILs = instructions.ToList();
    //        int injectIndex = ILs.FindIndex(ILs.FindIndex(x => x.opcode == OpCodes.Ldloca_S) + 1, x => x.opcode == OpCodes.Ldloca_S) - 2; // Second occurence
    //        ILs.RemoveRange(injectIndex, 2);
    //        MethodInfo childBodyCheck = typeof(Children_Drawing).GetMethod("ModifyChildBodyType");
    //        ILs.Insert(injectIndex, new CodeInstruction(OpCodes.Call, childBodyCheck));
    //        foreach (CodeInstruction IL in ILs)
    //        {
    //            yield return IL;
    //        }
    //    }
    //}

}
