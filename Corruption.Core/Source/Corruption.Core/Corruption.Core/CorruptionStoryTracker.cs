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

        public List<CorruptionStoryTrackerComponent> Components = new List<CorruptionStoryTrackerComponent>();

        public static CorruptionStoryTracker Current => Find.World.GetComponent<CorruptionStoryTracker>();

        public CorruptionStoryTracker(World world) : base(world)
        {
            Gods = new DefMap<GodDef, God>();
            foreach (var def in DefDatabase<GodDef>.AllDefsListForReading)
            {
                this.Gods[def] = new God(def);
            }
            foreach (Type item2 in typeof(CorruptionStoryTrackerComponent).AllSubclassesNonAbstract())
            {
                if (!ComponentExists(item2))
                {
                    try
                    {
                        CorruptionStoryTrackerComponent item = (CorruptionStoryTrackerComponent)Activator.CreateInstance(item2);
                        Components.Add(item);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Could not instantiate a WorldComponent of type " + item2 + ": " + ex);
                    }
                }
            }
        }

        private bool ComponentExists(Type item2)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (item2.IsAssignableFrom(Components[i].GetType()))
                {
                    return true;
                }
            }
            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<BuildableDef>(ref this.UnlockedHiddenDefs, "hiddenDefs", LookMode.Def);
            Scribe_Collections.Look<CorruptionStoryTrackerComponent>(ref this.Components, "Components", LookMode.Deep);
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            var settings = LoadedModManager.GetMod<CorruptionMod>().GetSettings<ModSettings_Corruption>();
            foreach (var def in DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && x.race.Humanlike))
            {
                var entry = settings.SoulRaceCombinations.FirstOrDefault(x => x.Race == def.defName);
                if (entry != null && !def.comps.Any(x => x is CompProperties_Soul))
                {
                    var soulComp = new CompProperties_Soul();
                    soulComp.defaultPantheon = DefDatabase<PantheonDef>.AllDefs.Where(x =>x.requiresMod == null || (ModLister.GetModWithIdentifier(x.requiresMod)?.Active ?? false)).FirstOrDefault(x => x.defName == entry.DefaultPantheon);
                    soulComp.baseCorruptionResistanceFactor = entry.BaseCorruptionGainFactor;
                    def.comps.Add(soulComp);
                    def.inspectorTabs.Add(typeof(ITab_Pawn_Soul));
                    def.ResolveReferences();
                }
            }

            foreach (var component in this.Components)
            {
                component.Initialize();
            }
        }

        internal void Notify_PantheonChanged(CompSoul soul)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                this.Components[i].Notify_PawnPantheonChanged(soul);
            }
        }
    }
}
