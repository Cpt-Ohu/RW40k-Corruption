using Corruption.Core.Soul;
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
    public class JobDriver_PrayToShrine : JobDriver_Prayer
    {
        private CompShrine CompShrine
        {
            get
            {
                if (this.TargetB.HasThing)
                {
                    return this.TargetB.Thing.TryGetComp<CompShrine>();
                }
                return null;
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            CompSoul soul = this.GetActor().Soul();
            var toils = base.MakeNewToils().ToList();

            Toil lastToil = toils.Last();

            if (this.CompShrine != null)
            {
                lastToil.initAction = delegate
                {
                    float angle = (CompShrine.parent.Position - pawn.Position).ToVector3().AngleFlat();
                    faceDir = Pawn_RotationTracker.RotFromAngleBiased(angle);
                    soul?.PrayerTracker.StartRandomPrayer(this.job, true);
                };
            }

            lastToil.AddFinishAction(delegate { AddPrayerFinish(soul, lastToil); });


            return toils;
        }

        private void AddPrayerFinish(CompSoul soul, Toil lastToil)
        {
            if (soul != null)
            {
                float num = 100f + (CompShrine?.CompProps.worshipFactor * lastToil.defaultDuration / 3600f) ?? 0f;
                num *= targetedGod.favourCorruptionFactor;
                soul.GainCorruption(num, targetedGod);
            }
        }
    }
}
