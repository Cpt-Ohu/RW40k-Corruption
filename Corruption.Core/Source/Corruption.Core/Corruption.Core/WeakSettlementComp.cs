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
    public class WeakSettlementComp : WorldObjectComp
    {
        private MapParent mapParent => this.parent as MapParent;

        private const int maxDefeners = 10;

        public override void PostMapGenerate()
        {
            Faction ownerFaction = this.parent.Faction;
            if (ownerFaction != null)
            {
                List<Pawn> pawns = this.mapParent.Map.mapPawns.FreeHumanlikesSpawnedOfFaction(ownerFaction);
                foreach (var pawn in pawns)
                {
                    var hediff = HediffMaker.MakeHediff(RimWorld.HediffDefOf.Malnutrition, pawn);
                    hediff.Severity = Rand.Range(0.4f, 0.7f);
                    pawn.health.AddHediff(hediff);
                    pawn.inventory.DestroyAll();
                }
                int removablePawnCount = Math.Max(0, pawns.Count - maxDefeners);
                for (int i = 0; i < removablePawnCount; i++)
                {
                    Pawn pawn = pawns.RandomElement();
                    if (!pawn.Destroyed)
                    {
                        pawn.Destroy();
                    }
                }
            }
        }
    }
}
