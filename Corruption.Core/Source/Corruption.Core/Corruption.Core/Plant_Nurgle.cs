using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class Plant_Nurgle : Plant
    {
        private CompAffectTerrain CompAffectTerrain;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            if (this.factionInt == null)
            {
                this.factionInt = Find.FactionManager.FirstFactionOfDef(Corruption.Core.FactionsDefOf.Chaos_NPC);
            }
            base.SpawnSetup(map, respawningAfterLoad);
            this.CompAffectTerrain = this.GetComp<CompAffectTerrain>();
        }


        public override void TickLong()
        {
            base.TickLong();
            this.CompAffectTerrain?.TryAffectTerrain();
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
