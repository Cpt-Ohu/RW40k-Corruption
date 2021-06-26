using Corruption.Core.Soul;
using RimWorld;
using RimWorld.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace Corruption.Core.Abilities
{
    [StaticConstructorOnStartup]
    public abstract class Window_TraitAbilities : Window
    {
        private static Texture2D LockedTex = SolidColorMaterials.NewSolidColorTexture(TexUI.LockedResearchColor);
        protected CompSoul Soul;
        protected Trait Trait;
        protected SoulTraitDef TraitDef;
        protected static readonly Texture2D BackgroundTile = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterTile", true);
        private static readonly Texture2D TransparentBackground = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0f, 0f, 0f));
        protected static readonly Texture2D NodeBG = ContentFinder<Texture2D>.Get("UI/Background/AbilityNodeBG", true);
        private static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG");
        private static readonly Texture2D BGTexHighlight = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBGHighlight");
        private static readonly Texture2D Plus = ContentFinder<Texture2D>.Get("UI/Buttons/Plus");

        private static Vector2 scrollPos;

        protected Texture2D ProgressTex;
        protected int minDegree = 1;
        protected int maxDegree = 1;
        protected SoulTraitDegreeOptions selectedDegree;
        protected LearnableAbility selectedAbility;

        private static Vector2 descScrollPos;
        public override Vector2 InitialSize => new Vector2(1100f, 700f);

        public Window_TraitAbilities(CompSoul soul, Trait soulTrait)
        {

            this.Soul = soul;
            this.Trait = soulTrait;
            this.TraitDef = Trait.def as SoulTraitDef;
            if (this.TraitDef == null)
            {
                Log.Error($"Tried to open Window_Chosen with null SoulTraitDef from {Trait.def.defName}");
                this.Close();
            }
            if (TraitDef.associatedGod != null)
            {
                this.ProgressTex = SolidColorMaterials.NewSolidColorTexture(TraitDef.associatedGod.mainColor);
            }
            else
            {
                this.ProgressTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0.0f, 0.0f)); ;
            }
            this.maxDegree = Math.Max(1, TraitDef.degreeDatas.Max(x => x.degree));
            this.minDegree = Math.Max(1, TraitDef.degreeDatas.Min(x => x.degree));
        }

        protected virtual TaggedString LearnActionText => "LearnPower".Translate();

        protected virtual float DrawTitle(Rect inRect)
        {
            var titleRect = new Rect(0f, 0f, inRect.width, 32f);
            Verse.Text.Font = GameFont.Medium;
            Widgets.Label(titleRect, "TraitAbilityWindowTitle".Translate(Soul.Pawn.NameShortColored, this.Trait.LabelCap));
            Verse.Text.Font = GameFont.Small;
            return titleRect.yMax;
        }

        protected virtual float DrawDescription(Rect inRect, float curY)
        {
            var titleRect = new Rect(0f, curY, inRect.width, 32f);
            Widgets.Label(titleRect, this.Trait.CurrentData.description);
            return titleRect.yMax;
        }

        public override void DoWindowContents(Rect inRect)
        {
            inRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(inRect);
            float curY = this.DrawTitle(inRect);

            curY = this.DrawDescription(inRect, curY);
            Rect topRect = GetNodeParentRect(ref inRect, curY);

            GUI.BeginGroup(topRect);
            topRect = DrawNodesRect(topRect);
            GUI.EndGroup();

            Rect bottomRect = new Rect(0f, topRect.yMax + 4f, inRect.width - 250f, inRect.height - topRect.height - 128f);
            // GUI.BeginGroup(bottomRect);

            Widgets.DrawBox(bottomRect);
            if (this.selectedDegree != null)
            {
                Rect viewRect = new Rect(0f, 0f, bottomRect.width - 36f, selectedDegree.learnableAbilities.Count * 70f);
                Widgets.BeginScrollView(bottomRect.ContractedBy(4f), ref scrollPos, viewRect);
                curY = 0f;
                foreach (var learnable in selectedDegree.learnableAbilities)
                {
                    var totalRect = new Rect(0f, curY, bottomRect.width, 64f);
                    if (this.selectedAbility == learnable || Mouse.IsOver(totalRect))
                    {
                        Widgets.DrawHighlight(totalRect);
                    }

                    Rect iconRect = new Rect(0f, curY, 64f, 64f);
                    GUI.DrawTexture(iconRect, BGTex);
                    GUI.DrawTexture(iconRect, learnable.ability.uiIcon);
                    Verse.Text.Font = GameFont.Medium;
                    Rect labelRect = new Rect(70f, curY, bottomRect.width - 70f, Verse.Text.LineHeight);
                    Widgets.Label(labelRect, learnable.ability.LabelCap);
                    Verse.Text.Font = GameFont.Small;
                    if (Widgets.ButtonInvisible(totalRect))
                    {
                        this.selectedAbility = learnable;
                    }
                    curY = totalRect.yMax + 2f;
                }
                Widgets.EndScrollView();
            }

            Rect selectionRect = new Rect(bottomRect.xMax + 4f, 0f, 220f, inRect.height);

            Widgets.DrawBox(selectionRect);
            Widgets.DrawMenuSection(selectionRect);
            AbilityUI.DrawSelection(selectionRect.ContractedBy(4f), this.selectedAbility);

            Rect actionRect = new Rect(bottomRect.x, bottomRect.yMax + 4f, bottomRect.width, inRect.height - bottomRect.yMax - 4f);

            Widgets.DrawBox(actionRect);
            Widgets.DrawMenuSection(actionRect);

            GUI.BeginGroup(actionRect.ContractedBy(4f));
            Rect buttonRect = new Rect(64f, 0f + 4f, 196f, 32f);


            var learntAbility = this.AbilityAtDegree(selectedDegree);

            if (learntAbility != null)
            {
                Widgets.CustomButtonText(ref buttonRect, "LearntPower".Translate(), TexUI.LockedResearchColor, TexUI.HighlightBorderResearchColor, TexUI.HighlightBorderResearchColor);
            }
            else if (this.selectedDegree?.degree > this.Trait.Degree)
            {
                Widgets.CustomButtonText(ref buttonRect, "PowerRequirementNotMet".Translate(), TexUI.LockedResearchColor, TexUI.HighlightBorderResearchColor, TexUI.HighlightBorderResearchColor);
            }
            else
            {
                if (this.selectedAbility != null)
                {
                    if (Widgets.ButtonText(buttonRect, LearnActionText))
                    {
                        this.Soul.Pawn.abilities.GainAbility(selectedAbility.ability);
                        this.Soul.LearnedAbilities.Add(selectedAbility.ability);
                    }
                }
            }

            GUI.EndGroup();

            GUI.EndGroup();
            if (Widgets.CloseButtonFor(inRect.ExpandedBy(17f).AtZero()))
            {
                this.Close();
            }

        }

        protected virtual Rect DrawNodesRect(Rect containerRect)
        {
            float maxWidth = containerRect.width - 64f;
            foreach (var degree in this.TraitDef.degreeDatas.Where(x => x is SoulTraitDegreeOptions).Cast<SoulTraitDegreeOptions>().OrderBy(x => x.degree))
            {
                DrawNode(maxWidth, degree, 0f);
            }

            return containerRect;
        }

        protected virtual Rect GetNodeParentRect(ref Rect inRect, float curY)
        {
            return new Rect(0f, curY, inRect.width - 250f, 70f);
        }

        protected Rect GetNodeRect(float maxWidth, SoulTraitDegreeOptions degree, float curY)
        {
            float relativePos = (degree.degree - minDegree) / (float)(maxDegree - minDegree);
            var nodeRect = new Rect(maxWidth * relativePos - relativePos * 64f, curY, 64f, 64f);
            return nodeRect;
        }

        protected virtual Rect DrawNode(float maxWidth, SoulTraitDegreeOptions degree, float curY)
        {
            var nodeRect = GetNodeRect(maxWidth, degree, curY);
            if (degree == this.selectedDegree)
            {
                GUI.DrawTexture(nodeRect, BGTexHighlight);
            }
            else
            {
                GUI.DrawTexture(nodeRect, BGTex);
            }
            var learnt = degree.learnableAbilities.FirstOrDefault(x => Soul.LearnedAbilities.Contains(x.ability));
            if (learnt != null)
            {
                GUI.DrawTexture(nodeRect, learnt.ability.uiIcon);
            }
            else if (Trait.Degree >= degree.degree)
            {
                GUI.DrawTexture(nodeRect, Plus);
            }
            if (Mouse.IsOver(nodeRect))
            {
                Widgets.DrawHighlight(nodeRect);
            }
            if (Widgets.ButtonInvisible(nodeRect))
            {
                this.selectedDegree = degree;
            }
            return nodeRect;
        }

        protected LearnableAbility AbilityAtDegree(int degree)
        {
            var degreeOptions = this.TraitDef.degreeDatas.Where(x => x is SoulTraitDegreeOptions).Cast<SoulTraitDegreeOptions>().FirstOrDefault(x => x.degree == degree);
            return AbilityAtDegree(degreeOptions);
        }

        protected LearnableAbility AbilityAtDegree(SoulTraitDegreeOptions degreeOptions)
        {
            var learnt = degreeOptions?.learnableAbilities.FirstOrDefault(x => Soul.LearnedAbilities.Contains(x.ability));
            return learnt;
        }
    }
}
