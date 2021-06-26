using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static System.Net.Mime.MediaTypeNames;

namespace Corruption.Core.Abilities
{
    public static class AbilityUI
    {
        public static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG");
        public static readonly Texture2D BGTexHighlight = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBGHighlight");
        public static readonly Texture2D UnknownIcon = ContentFinder<Texture2D>.Get("UI/Buttons/Unknown");

        private static Vector2 descrScrollPos;

        public static void DrawSelection(Rect selectionRect, LearnableAbility selectedPower)
        {
            GUI.BeginGroup(selectionRect);
            Rect labelRect = new Rect(0f, 0f, selectionRect.width, 32f);
            Verse.Text.Font = GameFont.Medium;
            Widgets.Label(labelRect, "SelectedPower".Translate());
            Verse.Text.Font = GameFont.Small;

            Rect iconRect = new Rect(selectionRect.width / 2f - 37f, labelRect.yMax + 4f, 74f, 74f);

            if (selectedPower == null)
            {
                GUI.DrawTexture(iconRect, AbilityUI.BGTex);
                GUI.DrawTexture(iconRect, UnknownIcon);
            }
            else
            {
                GUI.DrawTexture(iconRect, AbilityUI.BGTex);
                GUI.DrawTexture(iconRect, selectedPower.ability.uiIcon);

                Rect powerLabelRect = new Rect(0f, iconRect.yMax + 4f, selectionRect.width, 48f);
                Verse.Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(powerLabelRect, selectedPower.ability.LabelCap);
                Verse.Text.Anchor = TextAnchor.UpperLeft;

                string description = selectedPower.ability.description;
                Rect descriptionRect = new Rect(0f, powerLabelRect.yMax + 8f, selectionRect.width, Verse.Text.CalcHeight(description, selectionRect.width)); ;

                Widgets.Label(descriptionRect, description);
                float curY = descriptionRect.yMax + 4f;
                Widgets.ListSeparator(ref curY, selectionRect.width, "TabStats".Translate());
                Rect factRect = new Rect(0f, curY, selectionRect.width, selectionRect.height - curY);

                string toolTip = selectedPower.ability.StatSummary.ToLineList();

                if (selectedPower.ability.targetRequired)
                {
                    toolTip += "\n" + "Range".Translate() + ": " + selectedPower.ability.verbProperties.range.ToString("F0");
                }

                foreach (var directDamageComp in selectedPower.ability.comps.Where(x => x is CompProperties_DirectDamage).Cast<CompProperties_DirectDamage>())
                {
                    toolTip += "\n" + "DirectDamageDescr".Translate(new NamedArgument(directDamageComp.damage, "AMOUNT"), new NamedArgument(directDamageComp.damageDef.label, "DAMAGEDEF"));
                }

                foreach (var severityComp in selectedPower.ability.comps.Where(x => x is CompProperties_AbilityGiveHediffSeverity).Cast<CompProperties_AbilityGiveHediffSeverity>())
                {
                    var stage = severityComp.hediffDef.stages[0];
                    if (severityComp is CompProperties_AbilityGiveHediffSeverity compSeverity)
                    {
                        stage = compSeverity.hediffDef.stages.FirstOrDefault(x => x.minSeverity >= compSeverity.severity);
                    }
                    foreach (var stat in stage.SpecialDisplayStats())
                    {
                        toolTip += "\n" + stat.LabelCap + ": " + stat.ValueString;
                    }
                    foreach (var cap in stage.capMods)
                    {
                        toolTip += "\n" + cap.capacity.LabelCap + ":" + cap.offset;
                    }
                }

                foreach (var conflictingPower in selectedPower.conflictsWith)
                {
                    toolTip += "\n" + "LearntExcludingPower".Translate(new NamedArgument(conflictingPower.label, "ABILITY"));
                }

                if (selectedPower.ability.comps.Any(x => x.compClass == typeof(CompAbilityEffect_PsyProjectile)))
                {
                    var projectile = selectedPower.ability.verbProperties.defaultProjectile.projectile;
                    int maxDamage = projectile.GetDamageAmount(1f) * selectedPower.ability.verbProperties.burstShotCount;
                    toolTip += "\n" + "ProjectileDamageDescr".Translate(new NamedArgument(maxDamage, "AMOUNT"), new NamedArgument(projectile.damageDef.label, "DAMAGEDEF"));
                    if (projectile.explosionRadius > 0f) toolTip += "\n" + "ProjectileAoERadius".Translate(new NamedArgument(projectile.explosionRadius, "RADIUS"));
                }

                Widgets.TextAreaScrollable(factRect, toolTip, ref descrScrollPos, true);

                toolTip += "\n\n" + "LearnPowerCost".Translate(new NamedArgument((int)(selectedPower.cost), "COST"));
            }
            GUI.EndGroup();
        }
    }
}
