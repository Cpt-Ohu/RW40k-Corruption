using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Corruption.Worship.Wonders
{
    public class WonderWorker_SpawnThing : WonderWorker_Targetable
    {
        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters()
            {
                canTargetLocations = true,
                canTargetPawns = true,
                validator = ((TargetInfo x) => x.Cell.Standable(x.Map))
            };
        }

        protected override void TryDoEffectOnTarget(int worshipPoints)
        {
            for (int i = 0; i < this.Def.thingsToSpawn.Count; i++)
            {
                ThingDefCountClass entry = this.Def.thingsToSpawn[i];
                int countToCreate = entry.count / entry.thingDef.stackLimit;
                for (int j = 0; j < countToCreate; j++)
                {
                    Thing thing = ThingMaker.MakeThing(entry.thingDef);
                    thing.stackCount = Math.Min(thing.def.stackLimit, countToCreate);
                    GenPlace.TryPlaceThing(thing, this.target.Cell, this.target.Map, ThingPlaceMode.Near);
                }
            }
        }
    }
}
