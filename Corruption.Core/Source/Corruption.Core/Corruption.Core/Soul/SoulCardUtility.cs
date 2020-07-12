using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Soul
{
    [StaticConstructorOnStartup]
    public class SoulCardUtility
    {
        private static readonly Texture2D BackgroundTile = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterTile", true);
        private static readonly Texture2D SoulmeterProgressTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0.0f, 0.0f));
        private static readonly Texture2D TransparentBackground = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0f, 0f, 0f));
        private static readonly Texture2D SoulNode = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterNode", true);
        private static readonly Texture2D SoulNodeBG = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterNodeBG", true);

        public static Texture2D SkillBarFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));

        public static float DrawSoulMeter(Rect inRect, CompSoul soul)
        {
            GUI.BeginGroup(inRect);
            Rect bgRect = new Rect(0f, 8f, inRect.width, 32f);

            GUI.DrawTexture(bgRect, SoulCardUtility.BackgroundTile);
            Rect progressRect = new Rect(bgRect);
            progressRect.height = 15f;
            progressRect.y += 9f;
            Widgets.FillableBar(progressRect, soul.CorruptionLevel, SoulCardUtility.SoulmeterProgressTex, SoulCardUtility.TransparentBackground, false);
            DrawSoulmeterNodes(soul, inRect.width - 48f);
            if (Mouse.IsOver(progressRect))
            {
                TooltipHandler.TipRegion(progressRect, new TipSignal("", soul.Pawn.GetHashCode() * 397945));
            }
            GUI.EndGroup();
            return progressRect.yMax;
        }

        private static void DrawSoulmeterNodes(CompSoul soul, float width)
        {
            DrawNode(soul, SoulAfflictionDefOf.Pure, 0f);
            DrawNode(soul, SoulAfflictionDefOf.Intrigued, width * SoulAfflictionDefOf.Intrigued.Threshold);
            DrawNode(soul, SoulAfflictionDefOf.Warptouched, width * SoulAfflictionDefOf.Warptouched.Threshold);
            DrawNode(soul, SoulAfflictionDefOf.Tainted, width * SoulAfflictionDefOf.Tainted.Threshold);

            DrawNode(soul, SoulAfflictionDefOf.Corrupted, width * SoulAfflictionDefOf.Corrupted.Threshold);
            DrawNode(soul, SoulAfflictionDefOf.Lost, width);
        }

        private static void DrawNode(CompSoul soul, SoulAfflictionDef affliction, float curX)
        {
            Rect nodeRect = new Rect(curX, 0f, 48f, 48f);
            if (soul.CorruptionLevel >= affliction.Threshold)
            {
                GUI.DrawTexture(nodeRect.ContractedBy(4f), SoulCardUtility.SoulmeterProgressTex);
            }
            else
            {
                GUI.DrawTexture(nodeRect, SoulCardUtility.SoulNodeBG);
            }
            GUI.DrawTexture(nodeRect, affliction.uiIcon);
            TipSignal tip4 = new TipSignal(() => string.Concat("Affliction", affliction.defName).Translate(), (int)curX * 37);
            TooltipHandler.TipRegion(nodeRect, tip4);
        }
    }
}
