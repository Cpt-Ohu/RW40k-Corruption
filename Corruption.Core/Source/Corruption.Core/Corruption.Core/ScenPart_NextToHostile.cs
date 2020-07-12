using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class ScenPart_NextToHostile : ScenPart
    {
        private FactionDef factionToSpawnNextTo;

        private float threatPoints = 500f;

        public override void PreMapGenerate()
        {
            base.PreMapGenerate();
            if (this.factionToSpawnNextTo != null)
            {
                Faction faction = Find.FactionManager.FirstFactionOfDef(factionToSpawnNextTo);
                if (faction != null)
                {
                    //Site site = (Site)SiteMaker.MakeSite(SitePartDefOf.Outpost, 0, faction, true);
                    Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                    settlement.SetFaction(faction);
                    List<int> neighbours = new List<int>();
                    Find.WorldGrid.GetTileNeighbors(Find.GameInitData.startingTile, neighbours);
                    var tiles = neighbours.ConvertAll<Tile>(x => Find.WorldGrid[x]).Where(tile => !(!tile.biome.canBuildBase || !tile.biome.implemented || tile.hilliness == Hilliness.Impassable));

                    if (tiles.Count() > 0)
                    {
                        settlement.Tile = Find.WorldGrid.tiles.IndexOf(tiles.RandomElement());
                        var comp = new WeakSettlementComp();
                        comp.parent = settlement;
                        comp.props = new WorldObjectCompProperties();
                        settlement.AllComps.Add(comp);
                        //var outpostPart = settlement.parts.FirstOrDefault(x => x.def == SitePartDefOf.Outpost);
                        //outpostPart.parms.threatPoints = threatPoints;
                        Find.WorldObjects.Add(settlement);
                    }
                }
            }
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (var error in base.ConfigErrors())
            {
                yield return error;
            }
            if (this.factionToSpawnNextTo == null)
            {
                yield return "ScenPart_NextToHostile has null FactionDef";
            }
            if (this.factionToSpawnNextTo?.hidden == true)
            {
                yield return "ScenPart_NextToHostile has null FactionDef";
            }
        }
    }
}
