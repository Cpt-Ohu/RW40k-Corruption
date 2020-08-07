using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class CompSpawnTurret : ThingComp
    {
        private Building_Turret turret;

        public CompProperties_SpawnTurret Props => this.props as CompProperties_SpawnTurret;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Thing thing = ThingMaker.MakeThing(this.Props.turretToSpawn);
            thing.SetFactionDirect(this.parent.Faction);
            this.turret = GenSpawn.Spawn(thing, this.parent.Position, this.parent.Map, WipeMode.VanishOrMoveAside) as Building_Turret;
            var linkComp = this.turret.GetComp<CompDamageLinker>();
            if (linkComp != null)
            {
                linkComp.linkedTo = this.parent;
            }
        }



        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            this.turret?.Destroy();
        }

        public override void CompTick()
        {
            base.CompTick();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            return this.turret.GetGizmos();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look<Building_Turret>(ref this.turret, "turret");
        }
    }

    public class CompProperties_SpawnTurret : CompProperties
    {
        public ThingDef turretToSpawn;

        public CompProperties_SpawnTurret()
        {
            this.compClass = typeof(CompSpawnTurret);
        }

    }
}
