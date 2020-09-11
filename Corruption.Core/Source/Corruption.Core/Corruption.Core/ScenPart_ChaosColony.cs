using Corruption.Core.Gods;
using Corruption.Core.Soul;
using FactionColors;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core
{
    public class ScenPart_ChaosColony : ScenPart
    {
        private static FloatRange corruptionRange = new FloatRange(20000f, 25000f);

        private GodDef initialPatron;

        public override void PreConfigure()
        {
            base.PreConfigure();
            if (this.initialPatron == null) this.initialPatron = PantheonDefOf.Chaos.GodsListForReading.RandomElement();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 3f + 31f);
            Rect titleRect = scenPartRect.TopPartPixels(ScenPart.RowHeight);
            Widgets.Label(titleRect, "InitialChaosPatron".Translate());
            Rect buttonRect = titleRect;
            buttonRect.y = titleRect.yMax + 8f;
            if (Widgets.ButtonText(buttonRect, initialPatron?.LabelCap ?? "None"))
            {
                FloatMenuUtility.MakeMenu(PossiblePatrons(), (GodDef god) => god.LabelCap, delegate (GodDef god)
                {
                    ScenPart_ChaosColony scenPart = this;
                    return delegate
                    {
                        scenPart.initialPatron = god;
                    };
                });
            }

        }

        private IEnumerable<GodDef> PossiblePatrons()
        {
            foreach (var pantheon in DefDatabase<GodDef>.AllDefs.Where(x => PantheonDefOf.Chaos.GodsListForReading.Contains(x)))
            {
                yield return pantheon;
            }
        }

        public override void PostWorldGenerate()
        {
            base.PostWorldGenerate();
            FactionColorsTracker factionColor = Find.World.GetComponent<FactionColorsTracker>() as FactionColorsTracker;

            if (factionColor != null)
            {
                factionColor.PlayerColorOne = initialPatron.cultColorOne;
                factionColor.PlayerColorTwo = initialPatron.cultColorTwo;
            }
        }

        public override void Notify_PawnGenerated(Pawn pawn, PawnGenerationContext context, bool redressed)
        {
            base.Notify_PawnGenerated(pawn, context, redressed);
            if (this.initialPatron != null && context == PawnGenerationContext.PlayerStarter)
            {
                foreach (var apparel in pawn.apparel.WornApparel)
                {
                    apparel.DrawColor = this.initialPatron.cultColorOne;
                    if (apparel is FactionColors.ApparelUniform uniform)
                    {
                        uniform.Col1 = this.initialPatron.cultColorOne;
                        uniform.Col2 = this.initialPatron.cultColorTwo;
                    }
                }
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
                    CompSoul soul = pawn.Soul();
                    if (soul != null && pawn.RaceProps.Humanlike)
                    {
                        float startingProgress = FavourProgress.ProgressRange.LerpThroughRange(Rand.Range(0.1f, 0.3f));
                        soul.TryAddFavorProgress(this.initialPatron, startingProgress);
                    }
                });
            }
        }

        public override bool AllowPlayerStartingPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            if (pawn.WorkTagIsDisabled(WorkTags.Violent))
            {
                return false;
            }

            CompSoul soul = pawn.Soul();
            if (soul != null && pawn.RaceProps.Humanlike)
            {
                soul.ChosenPantheon = PantheonDefOf.Chaos;
                soul.GainCorruption(corruptionRange.RandomInRange);
                pawn.story.traits.GainTrait(new Trait(this.initialPatron.patronTraits.FirstOrDefault()));
            }
            return true;
        }
    }
}
