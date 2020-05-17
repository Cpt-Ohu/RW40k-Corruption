using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Corruption.Psykers
{
    [StaticConstructorOnStartup]
    public class Window_Psyker : Window
    {
        private const float MINOR_DISCIPLINE_COST = 1000f;

        private PsykerDisciplineDef selectedDiscipline = PsykerDisciplineDefOf.Initiate;
        private PsykerLearnablePower selectedPower;

        private CompPsyker comp;
        private int psykerPowerLevel;

        public Window_Psyker(CompPsyker comp)
        {
            this.comp = comp;
            UpdatePowers();
            this.closeOnClickedOutside = true;
            this.forcePause = true;
        }

        public void UpdatePowers()
        {
            this.psykerPowerLevel = comp.PsykerPowerTrait.Degree;
            this.selectedDiscipline = comp.MainDiscipline;
        }

        public override Vector2 InitialSize => new Vector2(800f, 700f);

        public override void DoWindowContents(Rect inRect)
        {
            inRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(inRect);
            Rect disciplineRect = new Rect(0f, 0f, 196f, inRect.height);
            Widgets.DrawMenuSection(disciplineRect);
            disciplineRect = disciplineRect.ContractedBy(17f);
            DrawDisciplineRect(disciplineRect);

            Rect infoRect = new Rect(202f, 0f, 520f, 148f);
            Widgets.DrawMenuSection(infoRect);
            infoRect = infoRect.ContractedBy(17f);
            DrawInfoRect(infoRect);

            Rect powersRect = new Rect(202f, 154f, 520f, disciplineRect.yMax - infoRect.yMax - 6f);

            Widgets.DrawBox(powersRect);
            powersRect = powersRect.ContractedBy(17f);
            DrawPowersRect(powersRect);

            GUI.EndGroup();
            if (Widgets.CloseButtonFor(inRect.ExpandedBy(17f).AtZero()))
            {
                this.Close();
            }
        }

        private void DrawPowersRect(Rect powersRect)
        {
            GUI.BeginGroup(powersRect);

            DrawPowerLevels(powersRect);

            foreach (var learnablePower in this.selectedDiscipline.abilities)
            {
                float x = 64f + 64 * (learnablePower.position.x - 1);
                float y = powerLevelYLookup[learnablePower.ability.level] - 16f;
                Rect abilityRect = new Rect(x, y, 32f, 32f);
                Color borderColor = PsykerUtility.AbilityLockedBorder;
                Color bgColor = PsykerUtility.AbilityLockedBG;
                int borderSize = 1;
                bool hasLearnedAbility = comp.HasLearnedAbility(learnablePower.ability);
                if (this.selectedPower == learnablePower)
                {
                    bgColor = PsykerUtility.AbilitySelectedBG;
                    borderColor = PsykerUtility.AbilityUnlockedBorder;
                    borderSize = 2;
                }
                else if (hasLearnedAbility)
                {
                    borderColor = PsykerUtility.AbilityUnlockedBorder;
                    bgColor = PsykerUtility.AbilityUnlockedBG;
                    borderSize = 2;
                }
                else if (!hasLearnedAbility && learnablePower.ability == this.selectedPower?.perequesiteAbility)
                {
                    borderColor = PsykerUtility.AbilityPerequisiteBorder;
                }
                if (Widgets.CustomButtonText(ref abilityRect, "", bgColor, Color.white, borderColor, false, borderSize))
                {
                    this.selectedPower = learnablePower;
                }
                Material material = learnablePower.ability.level > psykerPowerLevel ? TexUI.GrayscaleGUI : null;
                GenUI.DrawTextureWithMaterial(abilityRect, learnablePower.ability.uiIcon, material);
                Widgets.DrawHighlightIfMouseover(abilityRect);

                TooltipHandler.TipRegion(abilityRect, new TipSignal(learnablePower.ability.LabelCap, learnablePower.ability.GetHashCode()));

                if (learnablePower.perequesiteAbility != null)
                {
                    var perequisite = this.selectedDiscipline.abilities.FirstOrDefault(a => a.ability == learnablePower.perequesiteAbility);
                    if (perequisite != null)
                    {
                        float xPer = 64f + 64 * (perequisite.position.x - 1);
                        float yPer = powerLevelYLookup[perequisite.ability.level];
                        var start = new Vector2(xPer + 16f, yPer - 16f);

                        var end = new Vector2(x + 16f, y + 32f);
                        Widgets.DrawLine(start, end, Color.white, 1f);
                    }
                }
            }

            if (this.selectedPower != null)
            {
                Rect selectedTitleRect = new Rect(0f, 316f, powersRect.width, Text.LineHeight);
                Rect descrRect = new Rect(0f, selectedTitleRect.yMax + 4f, powersRect.width, Text.LineHeight * 2);
                Rect costRect = new Rect(0f, descrRect.yMax + 4f, powersRect.width, Text.LineHeight);
                Widgets.Label(selectedTitleRect, this.selectedPower.ability.LabelCap);
                Widgets.Label(descrRect, this.selectedPower.ability.description);
                Widgets.Label(costRect, "LearnPowerCost".Translate(new NamedArgument((int)(this.selectedPower.cost), "COST")));
                Rect buttonRect = new Rect(0f, costRect.yMax + 2f, 196f, 32f);


                if (this.selectedPower.ability.level > this.comp.PsykerPowerTrait.Degree)
                {
                    Widgets.CustomButtonText(ref buttonRect, "LearnPower".Translate(), TexUI.LockedResearchColor, TexUI.DefaultLineResearchColor, TexUI.DefaultLineResearchColor);
                    TooltipHandler.TipRegion(buttonRect, new TipSignal("PsykerLearnAboveLevel".Translate()));
                }
                else if (this.selectedPower.cost > this.comp.PsykerXP)
                {
                    Widgets.CustomButtonText(ref buttonRect, "LearnPower".Translate(), TexUI.LockedResearchColor, TexUI.DefaultLineResearchColor, TexUI.DefaultLineResearchColor);
                    TooltipHandler.TipRegion(buttonRect, new TipSignal("PsykerLearnXPShortage".Translate()));
                }
                else if (this.comp.HasLearnedAbility(selectedPower.ability))
                {
                    Widgets.CustomButtonText(ref buttonRect, "LearntPower".Translate(), TexUI.LockedResearchColor, TexUI.DefaultLineResearchColor, TexUI.HighlightBorderResearchColor);
                }
                else
                {
                    if (Widgets.ButtonText(buttonRect, "LearnPower".Translate()))
                    {
                        this.comp.TryLearnPower(this.selectedPower);
                    }
                }

                Rect debugRect = new Rect(buttonRect);
                debugRect.x = buttonRect.xMax + 8f;
                if (Prefs.DevMode)
                {
                    if (Widgets.ButtonText(debugRect, "Debug: +100XP"))
                    {
                        this.comp.AddXP(100f);
                    }
                }

            }
            GUI.EndGroup();
        }

        private void DrawPowerLevels(Rect inRect)
        {
            var levelRect = new Rect(0f, 9f, 32f, 32f);
            Widgets.DrawLineHorizontal(4f, 25f, inRect.width - 8f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelBeta);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Beta"));

            levelRect.y += 50f;
            Widgets.DrawLineHorizontal(4f, 75f, inRect.width - 8f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelDelta);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Delta"));

            levelRect.y += 50f;
            Widgets.DrawLineHorizontal(4f, 125f, inRect.width - 8f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelEpsilon);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Epsilon"));

            levelRect.y += 50f;
            Widgets.DrawLineHorizontal(4f, 175f, inRect.width - 8f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelZeta);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Zeta"));

            levelRect.y += 50f;
            Widgets.DrawLineHorizontal(4f, 225f, inRect.width - 8f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelIota);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Iota"));

            levelRect.y += 50f;
            Widgets.DrawLineHorizontal(4f, 275f, inRect.width - 8f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelKappa);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Kappa"));
        }

        private Dictionary<int, float> powerLevelYLookup = new Dictionary<int, float>()
        {
            {60, 25f },
            {50, 75f },
            {40, 125f },
            {30, 175f },
            {20, 225f },
            {10, 275f }
        };

        private void DrawInfoRect(Rect infoRect)
        {
            GUI.BeginGroup(infoRect);

            GUI.color = comp.MainDiscipline.color;
            Text.Font = GameFont.Medium;
            Rect disciplineRect = new Rect(0f, 0f, infoRect.width, Text.LineHeight);
            Widgets.Label(disciplineRect, comp.MainDiscipline.practitionerTitle.CapitalizeFirst());
            GUI.color = Color.white;
            Text.Font = GameFont.Small;

            if (comp.minorDiscipline != null && comp.minorDiscipline != PsykerDisciplineDefOf.Initiate)
            {
                Rect minorTitleRect = new Rect(0f, disciplineRect.yMax + 4f, infoRect.width, Text.LineHeight);
                Widgets.Label(minorTitleRect, "MinorDisciplineInfo".Translate(comp.minorDiscipline.practitionerTitle.CapitalizeFirst()));
            }

            Rect psykerLevelRect = new Rect(0f, disciplineRect.yMax + 24f, infoRect.width, Text.LineHeight);

            string powerLevelLetter = PsykerUtility.PowerLevelLetters[comp.PsykerPowerTrait.Degree];
            Widgets.Label(psykerLevelRect, "PsykerLevelInfo".Translate(powerLevelLetter));

            Rect psykerXPRect = new Rect(psykerLevelRect);
            psykerXPRect.y = psykerXPRect.yMax + 4f;

            Widgets.Label(psykerXPRect, "PsykerXPInfo".Translate(new NamedArgument((int)this.comp.PsykerXP, "XP")));

            GUI.EndGroup();
        }

        private void DrawDisciplineRect(Rect disciplineRect)
        {
            GUI.BeginGroup(disciplineRect);
            Rect disciplineIconRect = new Rect(disciplineRect.width / 2f - 64f, 0f, 128f, 128f);
            GUI.DrawTexture(disciplineIconRect, PsykerUtility.BGTex);
            GUI.DrawTexture(disciplineIconRect, comp.MainDiscipline.Icon);

            Widgets.DrawHighlightIfMouseover(disciplineIconRect);


            if (comp.MainDiscipline == PsykerDisciplineDefOf.Initiate)
            {
                if (Widgets.ButtonInvisible(disciplineIconRect))
                {
                    Find.WindowStack.Add(new Window_PsykerDiscipline(this.comp, this));
                }
            }
            else
            {
                if (Widgets.ButtonInvisible(disciplineIconRect))
                {
                    this.selectedDiscipline = comp.MainDiscipline;
                }
                TooltipHandler.TipRegion(disciplineIconRect, new TipSignal(comp.MainDiscipline.description));
            }

            Rect rect2 = new Rect(0f, disciplineIconRect.yMax + 8f, disciplineRect.width, Text.LineHeight);
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rect2, "MinorDisciplines".Translate());
            Text.Anchor = TextAnchor.UpperLeft;


            Rect minorPowerRect = new Rect(39f, rect2.yMax + 4f, 64f, 64f);
            if (comp.minorDiscipline == null)
            {
                TooltipHandler.TipRegion(minorPowerRect, new TipSignal("UnlockMinorDisciplineSlot".Translate(new NamedArgument(MINOR_DISCIPLINE_COST, "COST"))));
                if (Widgets.ButtonText(minorPowerRect, "+"))
                {
                    if (comp.PsykerXP < MINOR_DISCIPLINE_COST)
                    {
                        Messages.Message("PsykerLearnXPShortage".Translate(), null, MessageTypeDefOf.RejectInput);
                    }
                    else
                    {
                        this.comp.PsykerXP -= MINOR_DISCIPLINE_COST;
                        this.comp.minorDiscipline = PsykerDisciplineDefOf.Initiate;
                    }
                }
            }
            else
            {

                GUI.DrawTexture(minorPowerRect, PsykerUtility.BGTex);
                GUI.DrawTexture(minorPowerRect, comp.minorDiscipline.Icon);
                if (this.selectedDiscipline == comp.minorDiscipline)
                {
                    Widgets.DrawHighlight(minorPowerRect);
                }
                else
                {
                    Widgets.DrawHighlightIfMouseover(minorPowerRect);
                }

                if (comp.minorDiscipline == PsykerDisciplineDefOf.Initiate && Widgets.ButtonInvisible(minorPowerRect))
                {
                    if (comp.MainDiscipline == PsykerDisciplineDefOf.Initiate)
                    {
                        Messages.Message("MinorDisciplineWithoutMajorDiscipline".Translate(), null, MessageTypeDefOf.RejectInput);
                    }
                    else if (comp.PsykerXP < 1000f)
                    {
                        Messages.Message("PsykerLearnXPShortage".Translate(), null, MessageTypeDefOf.RejectInput);
                    }
                    else if (comp.minorDiscipline == PsykerDisciplineDefOf.Initiate)
                    {
                        Find.WindowStack.Add(new Window_PsykerDisciplineMinor(this.comp, this));
                    }
                    else
                    {
                        this.selectedDiscipline = comp.minorDiscipline;
                        TooltipHandler.TipRegion(disciplineIconRect, new TipSignal(comp.minorDiscipline.description));
                    }
                }
            }


            //Rect eqipmentTitleRect = new Rect(0f, minorPowerRect.yMax + 16f, disciplineRect.width, Text.LineHeight);
            //Widgets.Label(eqipmentTitleRect, "PsykerEquipment".Translate());

            GUI.EndGroup();
        }

        public override void PostClose()
        {
            Log.Message(this.comp.MainDiscipline.defName);
            base.PostClose();
        }
    }
}
