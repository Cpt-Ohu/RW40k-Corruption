using Corruption.Core.Soul;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.IO;
using System.Linq.Expressions;

namespace Corruption.Psykers
{
    public class Window_PsykerDiscipline : Window
    {
        protected PsykerDisciplineDef selectedDef;

        protected List<PsykerDisciplineDef> psykerDisciplines = new List<PsykerDisciplineDef>();

        protected CompPsyker comp;

        private Window_Psyker parentWindow;

        protected virtual string Title => "MainDiscipline".Translate();

        public override Vector2 InitialSize => new Vector2(800f, 700f);

        public Window_PsykerDiscipline(CompPsyker comp, Window_Psyker windowPsyker)
        {
            this.closeOnClickedOutside = true;

            this.comp = comp;
            if (comp.parent == null) Log.Message("No PArent");
            this.parentWindow = windowPsyker;
            var disciplines = DefDatabase<Corruption.Psykers.PsykerDisciplineDef>.AllDefsListForReading.FindAll(x => x.associatedPantheons.NullOrEmpty() == false && x != PsykerDisciplineDefOf.Initiate);

            var pawnPantheon = comp.Pawn.Soul().ChosenPantheon;

            this.psykerDisciplines = disciplines.FindAll(x => x.associatedPantheons.Any(p => pawnPantheon == p));
            this.selectedDef = psykerDisciplines.Contains(comp.MainDiscipline) ? comp.MainDiscipline : psykerDisciplines.FirstOrDefault();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect disciplinesRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(inRect);
            disciplinesRect.height = inRect.height - 128f;

            GUI.BeginGroup(disciplinesRect);

            var vector = new Vector2(disciplinesRect.width / 2f, disciplinesRect.height / 2f);

            if (psykerDisciplines.Count > 1)
            {
                Vector2 initialVector = new Vector2(0f, disciplinesRect.height / 2f - 64f);

                float radialOffset = 360f / psykerDisciplines.Count;

                for (int i = 0; i < psykerDisciplines.Count; i++)
                {
                    var outerPosition = initialVector.RotatedBy(radialOffset * i) + vector;
                    DrawDiscipline(psykerDisciplines[i], outerPosition);
                }
            }


            Rect selectedRect = new Rect(disciplinesRect.width / 2f - 64f, disciplinesRect.height / 2f - 64f, 128f, 128f);
            DrawSelectedDiscipline(selectedRect);

            GUI.EndGroup();


            Rect confirmRect = new Rect(inRect.width / 2f - 132f, disciplinesRect.yMax + 32f, 128f, 56f);
            if (Widgets.ButtonText(confirmRect, "ChooseDiscipline".Translate(), true, true, this.comp.MainDiscipline != this.selectedDef))
            {
                this.ChoosePower();
            }
            Rect cancelRect = new Rect(confirmRect.xMax + 8f, disciplinesRect.yMax + 32f, 128f, 56f);
            if (Widgets.ButtonText(cancelRect, "Cancel".Translate()))
            {
                this.Close();
            }
            GUI.EndGroup();


            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperCenter;
            Rect titleRect = new Rect(0f, 8f, inRect.width, Text.LineHeight);
            Widgets.Label(titleRect, this.Title);
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        private void DrawSelectedDiscipline(Rect inRect)
        {
            Rect iconRect = new Rect(inRect.x, inRect.y, 128f, 128f);
            GUI.DrawTexture(iconRect, PsykerUtility.BGTex);
            GUI.DrawTexture(iconRect, this.selectedDef.Icon);
            Rect descriptionRect = new Rect(iconRect.x - 38f, iconRect.yMax + 8f, iconRect.width + 76f, Text.LineHeight * 4f);
            Widgets.DrawBox(descriptionRect);
            descriptionRect = descriptionRect.ContractedBy(2f);
            Widgets.TextArea(descriptionRect, selectedDef.description, true);
        }

        private void DrawDiscipline(PsykerDisciplineDef def, Vector2 vector)
        {
            Rect rect = new Rect(vector.x - 32f, vector.y - 32f, 64f, 64f);
            Color bgColor = this.selectedDef == def ? PsykerUtility.AbilitySelectedBG : PsykerUtility.AbilityLockedBG;
            Color fColor = this.selectedDef == def ? PsykerUtility.AbilityUnlockedBorder : PsykerUtility.AbilityLockedBorder;
            if (Widgets.CustomButtonText(ref rect, "", bgColor, Color.white, fColor))
            {
                this.selectedDef = def;
            }
            GUI.DrawTexture(rect, def.Icon);
            Widgets.DrawHighlightIfMouseover(rect);
            TooltipHandler.TipRegion(rect, new TipSignal(def.label));

            Rect titleRect = new Rect(rect.x - 16f, rect.yMax + 4f, rect.width + 32f, Text.LineHeight);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(titleRect, def.LabelCap);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        protected virtual void ChoosePower()
        {
            comp.MainDiscipline = this.selectedDef;
            this.parentWindow.UpdatePowers();
            Log.Message(this.comp.parent.Label);
            this.Close();
        }
    }

    public class Window_PsykerDisciplineMinor : Window_PsykerDiscipline
    {

        protected override string Title => "MinorDiscipline".Translate();

        public Window_PsykerDisciplineMinor(CompPsyker comp, Window_Psyker windowPsyker) : base(comp, windowPsyker)
        {
        }

        protected override void ChoosePower()
        {
            this.comp.minorDiscipline = this.selectedDef;
            this.Close();
        }
    }
}
