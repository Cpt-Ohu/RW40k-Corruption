using Corruption.Core.Soul;
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
        private CompAltar compAltar
        {
            get
            {
                if (this.TargetB.HasThing)
                {
                    return this.TargetB.Thing.TryGetComp<CompAltar>();
                }
                return null;
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil lastToil = new Toil();
            IEnumerator<Toil> enumerator = base.MakeNewToils().GetEnumerator();
            while (enumerator.MoveNext())
            {
                lastToil = enumerator.Current;
                yield return enumerator.Current;
            }
            
            if(this.compAltar != null)
            {
                lastToil.initAction = delegate
                {
                    float angle = (compAltar.parent.Position - pawn.Position).ToVector3().AngleFlat();
                    faceDir = Pawn_RotationTracker.RotFromAngleBiased(angle);
                };

                lastToil.finishActions[0] = delegate
                {
                    CompSoul soul = this.GetActor().Soul();
                    if (soul != null)
                    {
                        float num = 100f + (compAltar?.CompProps.WorshipRatePerTick * lastToil.defaultDuration * 0.008f) ?? 0f;
                        if (!soul.Corrupted)
                        {
                            num *= -1f * Rand.Range(0.5f, 1);
                        }
                        soul.GainCorruption(num, targetedGod);
                    }
                };
            }
        }


    }
}
