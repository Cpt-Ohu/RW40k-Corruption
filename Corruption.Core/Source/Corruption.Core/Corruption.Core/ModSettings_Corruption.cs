using Corruption.Core.Gods;
using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Corruption.Core.ModSettings_Corruption;

namespace Corruption.Core
{
    public class ModSettings_Corruption : ModSettings
    {
        public sealed class SoulRaceEntry : IExposable
        {
            public string Race;
            public string DefaultPantheon;
            public float StartingCorruption;

            public void ExposeData()
            {
                Scribe_Values.Look<string>(ref this.Race, "Race");
                Scribe_Values.Look<string>(ref this.DefaultPantheon, "DefaultPantheon");
                Scribe_Values.Look<float>(ref this.StartingCorruption, "StartingCorruption");
            }
        }

        public List<SoulRaceEntry> SoulRaceCombinations = new List<SoulRaceEntry>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<SoulRaceEntry>(ref this.SoulRaceCombinations, "soulRaceCombinations", LookMode.Deep);
        }


    }

    public class CorruptionMod : Mod
    {
        ModSettings_Corruption settings;

        public override string SettingsCategory()
        {
            return "CorruptionModName".Translate();
        }

        private List<ThingDef> AvailableRaces;
        private List<PantheonDef> AvailablePantheons;
        private static Vector2 ScrollPos;

        public CorruptionMod(ModContentPack content) : base(content)
        {
            settings = ((Mod)this).GetSettings<ModSettings_Corruption>();

        }

        public override void WriteSettings()
        {
            this.settings.SoulRaceCombinations.RemoveAll(x => x.Race == null);
            base.WriteSettings();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (this.AvailablePantheons == null)
            {
                this.AvailablePantheons = DefDatabase<PantheonDef>.AllDefsListForReading;
            }
            if (this.AvailableRaces == null)
            {
                this.AvailableRaces = DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && x.race.Humanlike && x.defName != ThingDefOf.Human.defName).ToList();
            }
            GUI.BeginGroup(inRect);
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(0f, 0f, inRect.width, Text.LineHeight);

            Widgets.Label(titleRect, "SoulSettings".Translate());
            Widgets.DrawLineHorizontal(4f, titleRect.yMax + 2f, inRect.width - 8f);
            Text.Font = GameFont.Small;

            Rect descrRect = new Rect(0f, titleRect.yMax + 6f, inRect.width, Text.LineHeight * 3f);
            Widgets.Label(descrRect, "SoulSettingsDesc".Translate());



            Listing_Standard listingStandard = new Listing_Standard();
            Rect allRect = new Rect(0f, descrRect.yMax + 8f, inRect.width - 24f, 256f);

            Widgets.DrawBox(allRect);
            Widgets.DrawWindowBackground(allRect);

            allRect = allRect.ContractedBy(4f);

            Rect viewRect = new Rect(0f, 0f, allRect.width - 36f, this.AvailableRaces.Count * Text.LineHeight * 1.02f);
            listingStandard.BeginScrollView(allRect, ref ScrollPos, ref viewRect);
            foreach (var race in AvailableRaces)
            {
                Rect rect = listingStandard.GetRect(Text.LineHeight + 4f);
                Widgets.Label(rect, race.LabelCap);

                Rect btnRect = new Rect(220f, rect.y, 200f, Text.LineHeight + 4f);

                var entry = this.settings.SoulRaceCombinations.FirstOrDefault(x => x.Race == race.defName);

                if (entry != null)
                {
                    if (Widgets.ButtonText(btnRect, entry.DefaultPantheon ?? "NoneLower".Translate()))
                    {
                        this.OpenPantheonSelectMenu(entry);
                    }
                }
                else
                {
                    if (Widgets.ButtonText(btnRect, "NoneLower".Translate()))
                    {
                        entry = new SoulRaceEntry() { Race = race.defName, DefaultPantheon = null };
                        this.settings.SoulRaceCombinations?.Add(entry);
                        this.OpenPantheonSelectMenu(entry);
                    }
                }

                if (entry.DefaultPantheon != null)
                {
                    Rect sliderRect = new Rect(btnRect);
                    sliderRect.x += btnRect.width + 8f;
                    sliderRect.width = allRect.width - btnRect.xMax;
                    if (entry.DefaultPantheon.Equals(PantheonDefOf.Chaos.defName))
                    {
                        entry.StartingCorruption = Widgets.HorizontalSlider(sliderRect, entry.StartingCorruption, SoulAfflictionDefOf.Corrupted.Threshold, SoulAfflictionDefOf.Lost.Threshold, true, "StartingCorruption".Translate(), SoulAfflictionDefOf.Corrupted.label, SoulAfflictionDefOf.Lost.label);
                    }
                    else
                    {
                        entry.StartingCorruption = Widgets.HorizontalSlider(sliderRect, entry.StartingCorruption, SoulAfflictionDefOf.Pure.Threshold, SoulAfflictionDefOf.Corrupted.Threshold - 0.05f, true, "StartingCorruption".Translate(), SoulAfflictionDefOf.Pure.label, SoulAfflictionDefOf.Tainted.label);
                    }
               }

            }
            listingStandard.EndScrollView(ref viewRect);
            GUI.EndGroup();
            base.DoSettingsWindowContents(inRect);
        }

        private void OpenPantheonSelectMenu(SoulRaceEntry raceEntry)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption("NoneLower".Translate(), delegate
            {
                raceEntry.DefaultPantheon = null;
            }, MenuOptionPriority.Default, null, null, 0f, null));

            foreach (var pantheon in this.AvailablePantheons)
            {
                list.Add(new FloatMenuOption(pantheon.LabelCap, delegate
                {
                    raceEntry.DefaultPantheon = pantheon.defName;
                }, MenuOptionPriority.Default, null, null, 0f, null));
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}
