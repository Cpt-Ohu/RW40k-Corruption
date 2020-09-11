using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class CompAffectSurroundings : ThingComp
    {
        private int ticksSinceLastEffect;

        public CompProperties_AffectSurroundings Props => this.props as CompProperties_AffectSurroundings;

        public void TryAffectTerrain()
        {
            this.ticksSinceLastEffect++;
            if (this.ticksSinceLastEffect > this.Props.ticksToEffect)
            {
                IntVec3 position = parent.Position;
                int num = 0;
                IntVec3 intVec;
                while (true)
                {
                    if (num >= this.Props.maxRange)
                    {
                        return;
                    }
                    intVec = position + GenRadial.RadialPattern[num];
                    var existingTerrain = parent.Map.terrainGrid.TerrainAt(intVec);
                    if (intVec.InBounds(parent.Map) && existingTerrain != this.Props.terrainToSet &&  (this.Props.ignoreTerrain == null || existingTerrain != this.Props.ignoreTerrain))
                    {
                        break;
                    }
                    num++;
                }

                if (intVec != IntVec3.Zero)
                {
                    this.TryAffect(intVec);
                }
                this.ticksSinceLastEffect = 0;
            }
        }

        protected virtual void TryAffect(IntVec3 cell)
        {

        }
    }

    public class CompAffectTerrain : CompAffectSurroundings
    {
        protected override void TryAffect(IntVec3 cell)
        {
            this.parent.Map.terrainGrid.SetTerrain(cell, this.Props.terrainToSet);
        }
    }

    public class CompSpawnAround : CompAffectSurroundings
    {
        protected override void TryAffect(IntVec3 cell)
        {
            var things = cell.GetThingList(this.parent.Map);
            bool replace = true;
            var destroyableThings = new List<Thing>();
            foreach (var thing in things)
            {
                if (!this.Props.forceSpawn && thing.def.category != this.Props.canReplaceCategory)
                {
                    replace = false;
                }
                else
                {
                    destroyableThings.Add(thing);
                }
            }
            if (replace && ExtraPredicate(cell))
            {
                for (int i = destroyableThings.Count - 1; i >= 0; i--)
                {
                    destroyableThings[i].Destroy();
                }

                GenSpawn.Spawn(this.Props.thingToSpawn, cell, this.parent.Map);
            }
        }

        protected virtual bool ExtraPredicate(IntVec3 cell)
        {
            return true;
        }
    }

    public class CompSpawnAround_Plant : CompSpawnAround
    {
        protected override bool ExtraPredicate(IntVec3 cell)
        {
            return Props.thingToSpawn.CanEverPlantAt_NewTemp(cell, parent.Map, canWipePlantsExceptTree: true);
        }
    }

    public class CompProperties_AffectSurroundings : CompProperties
    {
        public TerrainDef terrainToSet;

        public TerrainDef ignoreTerrain;

        public int ticksToEffect = 2000;

        public int maxRange = 200;

        public ThingDef thingToSpawn;

        public ThingCategory canReplaceCategory;

        public bool forceSpawn;

        public CompProperties_AffectSurroundings()
        {
            this.compClass = typeof(CompAffectTerrain);
        }
    }
}
