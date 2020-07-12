using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Corruption.Worship
{
    public class JoyGiver_Prayer : JoyGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            IntVec3 result = pawn.Position;
            if (pawn.ownership == null)
            {
                return null;
            }
            Room ownedRoom = pawn.ownership.OwnedRoom;
            LocalTargetInfo shrineTarget = LocalTargetInfo.Invalid;
            Thing ownedShrine = ownedRoom?.ContainedAndAdjacentThings.FirstOrDefault(x => x.TryGetComp<CompAltar>() != null);

            Predicate<Thing> shrineCheck = delegate (Thing t)
            {
                return t.TryGetComp<CompAltar>() != null;
            };
            Thing nearbyShrine = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors), 10f, shrineCheck);
            if (ownedShrine != null)
            {
                result = ownedShrine.InteractionCell;
                shrineTarget = ownedShrine;
            }
            else if (nearbyShrine != null)
            {
                result = nearbyShrine.InteractionCell;
                shrineTarget = nearbyShrine;
            }
            else if (ownedRoom != null && ownedRoom.Cells.Where((IntVec3 c) => c.Standable(pawn.Map) && !c.IsForbidden(pawn) && pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.None)).TryRandomElement(out IntVec3 cell))
            {
                result = cell;
            }
            return JobMaker.MakeJob(def.jobDef, result, shrineTarget);
        }

        public override Job TryGiveJobWhileInBed(Pawn pawn)
        {
            return JobMaker.MakeJob(def.jobDef, pawn.CurrentBed());
        }
    }
}
