using Corruption.Core.Gods;
using Corruption.Core.Soul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core
{
    public abstract class Dialog_SetPantheon : Window
    {
        private List<PantheonDef> pantheons = new List<PantheonDef>();

        public PantheonDef SelectedDef { get; private set; }

        public Dialog_SetPantheon(PantheonDef currentDef)
        {
            forcePause = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnClickedOutside = true;
            this.SelectedDef = currentDef;
        }

        public override void PreOpen()
        {
            base.PreOpen();

            this.pantheons = DefDatabase<PantheonDef>.AllDefsListForReading.FindAll(x => x.requiresMod == null || (ModLister.GetModWithIdentifier(x.requiresMod)?.Active ?? false));
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            Rect listRect = new Rect(0f, 0f, inRect.width, inRect.height - 56f);
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(listRect);

            foreach (var def in this.pantheons)
            {
                if (listing_Standard.RadioButton_NewTemp(def.label, this.SelectedDef == def, 0, def.description, 1f))
                {
                    this.SelectedDef = def;
                }
            }

            listing_Standard.End();

            Rect confirmRect = new Rect(inRect.width / 2f - 64f, listRect.yMax + 8f, 128f, 32f);
            if (Widgets.ButtonText(confirmRect, "SelectReligion".Translate()))
            {
                SelectionChanged(this.SelectedDef);
                this.Close();
            }

            GUI.EndGroup();
        }

        protected abstract void SelectionChanged(PantheonDef selectedDef);
    }

    public class Dialog_SetPawnPantheon : Dialog_SetPantheon
    {
        private CompSoul soul;

        public Dialog_SetPawnPantheon(CompSoul soul, PantheonDef currentDef) : base(currentDef)
        {
            this.soul = soul;
        }

        protected override void SelectionChanged(PantheonDef selectedDef)
        {
            this.soul.ChosenPantheon = selectedDef ?? PantheonDefOf.ImperialCult;
        }
    }
}
