using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly Texture2D SoulNodePure = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterNodePure", true);
        private static readonly Texture2D SoulNodeIntrigued = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterNodeIntrigued", true);
        private static readonly Texture2D SoulNodeWarptouched = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterNodeWarptouched", true);
        private static readonly Texture2D SoulNodeTainted = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterNodeTainted", true);
        private static readonly Texture2D SoulNodeCorrupted = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterNodeCorrupted", true);
        private static readonly Texture2D SoulNodeLost = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterNodeLost", true);
        private static readonly Texture2D BorderWorship = ContentFinder<Texture2D>.Get("UI/Background/BorderWorship", true);
        private static readonly Texture2D BorderPsyker = ContentFinder<Texture2D>.Get("UI/Background/BorderPsyker", true);


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
            GUI.EndGroup();
            return progressRect.yMax;
        }

        private static void DrawSoulmeterNodes(CompSoul soul, float width)
        {
            DrawNode(soul, SoulAffliction.Pure, 0f);
            DrawNode(soul, SoulAffliction.Intrigued, width * 0.22f);
            DrawNode(soul, SoulAffliction.Warptouched, width * 0.416f);
            DrawNode(soul, SoulAffliction.Tainted, width * 0.646f);

            DrawNode(soul, SoulAffliction.Corrupted, width * 0.846f);
            DrawNode(soul, SoulAffliction.Lost, width);
        }

        private static void DrawNode(CompSoul soul, SoulAffliction affliction, float curX)
        {
            Rect nodeRect = new Rect(curX, 0f, 48f, 48f);
            if (soul.Affliction >= affliction)
            {
                GUI.DrawTexture(nodeRect.ContractedBy(4f), SoulCardUtility.SoulmeterProgressTex);
            }
            else
            {
                GUI.DrawTexture(nodeRect, SoulCardUtility.SoulNodeBG);
            }
            GUI.DrawTexture(nodeRect, GetAfflictionTexture(affliction));
            TipSignal tip4 = new TipSignal(() => string.Concat("Affliction", affliction.ToString().Translate()), (int)curX * 37);
            TooltipHandler.TipRegion(nodeRect, tip4);
        }


        private static Texture2D GetAfflictionTexture(SoulAffliction type)
        {
            switch (type)
            {
                case SoulAffliction.Pure:
                    {
                        return SoulCardUtility.SoulNodePure;
                    }
                case SoulAffliction.Intrigued:
                    {
                        return SoulCardUtility.SoulNodeIntrigued;
                    }
                case SoulAffliction.Warptouched:
                    {
                        return SoulCardUtility.SoulNodeWarptouched;
                    }
                case SoulAffliction.Tainted:
                    {
                        return SoulCardUtility.SoulNodeTainted;
                    }
                case SoulAffliction.Corrupted:
                    {
                        return SoulCardUtility.SoulNodeCorrupted;
                    }
                case SoulAffliction.Lost:
                    {
                        return SoulCardUtility.SoulNodeLost;
                    }
                default:
                    {
                        return SoulCardUtility.SoulNodeWarptouched;
                    }
            }



        }
    }
}
