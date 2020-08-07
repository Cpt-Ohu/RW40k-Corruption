using Corruption.Core.Gods;
using Corruption.Core.Soul;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class CorruptionStoryTracker : WorldComponent
    {
        public DefMap<GodDef, God> Gods;

        public List<BuildableDef> UnlockedHiddenDefs = new List<BuildableDef>();

        public static CorruptionStoryTracker Current => Find.World.GetComponent<CorruptionStoryTracker>();

        public CorruptionStoryTracker(World world) : base(world)
        {
            Gods = new DefMap<GodDef, God>();
            foreach (var def in DefDatabase<GodDef>.AllDefsListForReading)
            {
                this.Gods[def] = new God(def);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<BuildableDef>(ref this.UnlockedHiddenDefs, "hiddenDefs", LookMode.Def);
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            var settings = LoadedModManager.GetMod<CorruptionMod>().GetSettings<ModSettings_Corruption>();
            foreach (var def in DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && x.race.Humanlike))
            {
                var entry = settings.SoulRaceCombinations.FirstOrDefault(x => x.Race == def.defName);
                if (entry != null)
                {
                    var soulComp = new CompProperties_Soul();
                    soulComp.defaultPantheon = DefDatabase<PantheonDef>.AllDefs.FirstOrDefault(x => x.defName == entry.DefaultPantheon);
                    def.comps.Add(soulComp);
                    def.inspectorTabs.Add(typeof(ITab_Pawn_Soul));
                    def.ResolveReferences();
                }
            }
        }
    }
}
