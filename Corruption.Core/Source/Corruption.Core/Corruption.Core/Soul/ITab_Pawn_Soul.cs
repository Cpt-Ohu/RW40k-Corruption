using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;
using Corruption.Core.Soul;
using System.ComponentModel;
using Corruption.Core.Gods;

namespace Corruption.Core.Soul
{
    public class ITab_Pawn_Soul : ITab
    {
        private Pawn PawnToShowInfoAbout
        {
            get
            {
                Pawn pawn = null;
                if (base.SelPawn != null)
                {
                    pawn = base.SelPawn;
                }
                else
                {
                    Corpse corpse = base.SelThing as Corpse;
                    if (corpse != null)
                    {
                        pawn = corpse.InnerPawn;
                    }
                }
                if (pawn == null)
                {
                    Log.Error("Character tab found no selected pawn to display.", false);
                    return null;
                }
                return pawn;
            }
        }

        private CompSoul SoulToShow
        {
            get
            {
                return this.PawnToShowInfoAbout?.GetComp<CompSoul>();
            }
        }

        private IEnumerable<FavourProgress> PantheonProgress;

        public ITab_Pawn_Soul()
        {
            this.labelKey = "TabSoul";
            this.tutorTag = "Soul";
        }

        public override void OnOpen()
        {
            base.OnOpen();
            LessonAutoActivator.TeachOpportunity(CoreConceptDefOf.SoulKnowledge, OpportunityType.Important);
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();
            this.size = CharacterCardUtility.PawnCardSize(this.PawnToShowInfoAbout) + new Vector2(17f, 17f) * 2f;
        }

        protected override void FillTab()
        {
            if (this.SoulToShow == null)
            {
                this.CloseTab();
            }
            this.UpdateSize();
            this.PantheonProgress = this.SoulToShow.FavourTracker.FavorProgressForChosenPantheon();
            Rect innerRect = new Rect(0f, 32f, this.size.x, this.size.y).ContractedBy(4f);
            GUI.BeginGroup(innerRect);
            Rect soulMeterRect = new Rect(0f, 0f, this.size.x - 16f, 64f);
            SoulCardUtility.DrawSoulMeter(soulMeterRect, this.SoulToShow);

            Rect pantheonRect = new Rect(0f, soulMeterRect.yMax, soulMeterRect.width, 32f);
            Text.Font = GameFont.Medium;
            Widgets.Label(pantheonRect, "ChosenPantheon".Translate(this.SoulToShow.ChosenPantheon?.label));
            Text.Font = GameFont.Small;
            Rect descriptionRect = new Rect(0f, pantheonRect.yMax, pantheonRect.width, 100f);
            Widgets.TextArea(descriptionRect, this.SoulToShow.ChosenPantheon?.description, true);

            Rect godsTitleRect = new Rect(0f, descriptionRect.yMax + 4f, innerRect.width, 30f);
            Text.Font = GameFont.Medium;
            Widgets.Label(godsTitleRect, "GodsFavourOverview".Translate());
            Text.Font = GameFont.Small;

            Rect progressRect = new Rect(0f, godsTitleRect.yMax, godsTitleRect.width / 2, innerRect.height - godsTitleRect.yMax);
            DrawFavorProgress(progressRect);

            Rect settingsRect = new Rect(progressRect.xMax + 4f, godsTitleRect.yMax, godsTitleRect.width / 2f - 16f, progressRect.height);

            this.DrawSettings(settingsRect);

            GUI.EndGroup();
        }

        private void DrawSettings(Rect settingsRect)
        {
            GUI.BeginGroup(settingsRect.ContractedBy(4f));
            Rect doPrayerRect = new Rect(0f, 0f, settingsRect.width - 8f, Text.LineHeight);
            Widgets.CheckboxLabeled(doPrayerRect, "AllowPrayers", ref this.SoulToShow.PrayerTracker.AllowPraying);

            Rect showPrayerRect = new Rect(0f, doPrayerRect.yMax + 4f, settingsRect.width - 8f, Text.LineHeight);
            Widgets.CheckboxLabeled(showPrayerRect, "ShowPrayers".Translate(), ref this.SoulToShow.PrayerTracker.ShowPrayer, !this.SoulToShow.PrayerTracker.AllowPraying);

            if (this.SoulToShow.PrayerTracker.lastPrayer != null)
            {
                Rect lastPrayerRect = new Rect(0f, showPrayerRect.yMax + 6f, showPrayerRect.width, Text.LineHeight);
                Widgets.Label(lastPrayerRect, "LastPrayer".Translate());
                lastPrayerRect.y += Text.LineHeight + 4f;
                Widgets.FillableBar(lastPrayerRect, this.SoulToShow.PrayerTracker.CooldownPercentage,TexUI.GrayBg, null, false);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(lastPrayerRect, this.SoulToShow.PrayerTracker.lastPrayer.LabelCap);
                Text.Anchor = TextAnchor.UpperLeft;
                if (Mouse.IsOver(lastPrayerRect))
                {
                    GUI.DrawTexture(lastPrayerRect, TexUI.HighlightSelectedTex);
                }
                TooltipHandler.TipRegion(lastPrayerRect, new TipSignal(String.Join("\n", this.SoulToShow.PrayerTracker.lastPrayer.prayerLines)));

            }
            GUI.EndGroup();
        }

        private float DrawFavorProgress(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            float curY = 0f;
            foreach (var progress in this.SoulToShow.FavourTracker.AllFavoursSorted().Where(x => x.FavourLevel >= GodsFavourLevel.Noticed || this.SoulToShow.ChosenPantheon.GodsListForReading.Contains(x.God)))
            {
                Rect holdingRect = new Rect(0f, curY, inRect.width, 24f);
                if (Mouse.IsOver(holdingRect))
                {
                    GUI.DrawTexture(holdingRect, TexUI.HighlightTex);
                }
                GUI.BeginGroup(holdingRect);
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect labelRect = new Rect(0f, 0f, inRect.width / 2f, holdingRect.height);
                Widgets.Label(labelRect, progress.God.label.CapitalizeFirst());
                Rect position = new Rect(labelRect.xMax, 0f, inRect.width / 2, holdingRect.height);
                Widgets.FillableBar(position, progress.FavourPercentage, progress.God.WorshipBarFillTexture, null, false);
                Rect valRect = new Rect(position);
                valRect.yMin += 2f;
                GenUI.SetLabelAlign(TextAnchor.MiddleLeft);
                Widgets.Label(valRect, String.Concat("WorshipStanding", progress.FavourLevel.ToString()).Translate());

                GenUI.ResetLabelAlign();
                GUI.EndGroup();
                if (Mouse.IsOver(holdingRect))
                {
                    string text = GetProgressDescription(progress);
                    TooltipHandler.TipRegion(holdingRect, new TipSignal(text, progress.God.GetHashCode() * 397945));
                }
                curY += holdingRect.height + 4f;
            }
            Rect debugRect = new Rect(0f, curY, inRect.width, 26f);
            if (Prefs.DevMode)
            {
                if (Widgets.ButtonText(debugRect, "Debug: Add 1% favour"))
                {
                    foreach(var favour in this.SoulToShow.FavourTracker.AllFavoursSorted())
                    {
                        favour.TryAddProgress(FavourProgress.ProgressRange.max / 100f);
                    }
                }
                debugRect.y += debugRect.height + 4f;
                if (Widgets.ButtonText(debugRect, "Debug: Change Religion"))
                {
                    Find.WindowStack.Add(new Dialog_SetPawnPantheon(this.SoulToShow, this.SoulToShow.ChosenPantheon));
                }
            }

            GUI.EndGroup();
            return curY;
        }

        private string GetProgressDescription(FavourProgress progress)
        {
            var builder = new System.Text.StringBuilder();
            var nextLevel = progress.NextLevel;
            var nextLevelThreshold = 100f;
            FavourProgress.FavorLevelThresholds.TryGetValue(nextLevel, out nextLevelThreshold);
            nextLevelThreshold = nextLevelThreshold * 100;
            builder.AppendLine(string.Concat(new object[]
            {
                "Level".Translate() + " ",
                (progress.FavourPercentage * 100).ToString("N0") + "%",
            }));
            builder.AppendLine();
            if (progress.FavourLevel < GodsFavourLevel.Blessed)
            {
                builder.AppendLine(string.Concat(new string[]
                {
                "NextLevel".Translate() + ": ",
                String.Concat("WorshipStanding",nextLevel.ToString()).Translate() + string.Concat(" (",nextLevelThreshold.ToString("N0") + "%",")")
                }));
            }

            builder.AppendLine();
            builder.AppendLine("____________________");
            builder.AppendLine();

            builder.AppendLine("FavourSources".Translate());

            builder.AppendLine();

            foreach (var source in progress.God.favourSourceDescriptions)
            {
                builder.AppendLine(String.Concat(" - ", source));
            }

            return builder.ToString();
        }

        public override bool IsVisible
        {
            get
            {
                return SelPawn.RaceProps.Humanlike;
            }
        }
    }
}
