using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core
{
    public class MainTabWindow_CorruptionCore : MainTabWindow
    {
        private CorruptionMainTabDef selectedDef;
        private static List<TabRecord> tabsList = new List<TabRecord>();

        public MainTabWindow_CorruptionCore()
        {

        }

        public override void PreOpen()
        {
            base.PreOpen();
            MainTabWindow_CorruptionCore.tabsList.Clear();
            foreach (var def in DefDatabase<CorruptionMainTabDef>.AllDefsListForReading)
            {
                if (this.selectedDef == null) this.selectedDef = def;
                MainTabWindow_CorruptionCore.tabsList.Add(new TabRecord(def.label, delegate
                {
                    this.selectedDef = def;

                }, this.selectedDef == def
                ));
            }
        }

        public override void WindowOnGUI()
        {
            base.WindowOnGUI();
            Rect rect = new Rect(0f, 0f, this.InitialSize.x, this.InitialSize.y);
            Widgets.DrawMenuSection(rect);
            TabDrawer.DrawTabs(rect, tabsList);
            Rect inRect = rect.ContractedBy(17f);
            if (this.selectedDef != null)
            {
                this.selectedDef.Window.WindowOnGUI();
            }
            if (Widgets.CloseButtonFor(inRect.AtZero()))
            {
                Close();
            }
        }
    }

    public class CorruptionMainTabDef : Def
    {
        public Type MainTabWindowType;

        public MainTabWindow Window;

        public override void ResolveReferences()
        {
            this.Window = (MainTabWindow)Activator.CreateInstance(this.MainTabWindowType);
        }
    }
}

