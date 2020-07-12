using Corruption.Core.Gods;
using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Corruption.Core.Soul
{

    public class JobDriver_Prayer : JobDriver_RelaxAlone
    {
        protected GodDef targetedGod;

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.SetGod();
            Toil lastToil = new Toil();
            IEnumerator<Toil> enumerator = base.MakeNewToils().GetEnumerator();
            while (enumerator.MoveNext())
            {
                lastToil = enumerator.Current;
                yield return enumerator.Current;
            }

            lastToil.AddPreInitAction(new Action(delegate
            {
                SoulUtility.ThrowPrayerMote(this.GetActor(), this.targetedGod);
            }));

            lastToil.tickAction = new Action(delegate
            {
                if (Find.TickManager.TicksGame % 120 == 0)
                    SoulUtility.ThrowPrayerMote(this.GetActor(), this.targetedGod);

            });

            lastToil.AddFinishAction(new Action(delegate
            {
                CompSoul soul = this.GetActor().Soul();
                if (soul != null)
                {

                    float num = 100f;
                    if (!soul.Corrupted)
                    {
                        num *= -1f * Rand.Range(0.5f, 1);
                    }
                    soul.GainCorruption(num, targetedGod);
                }
            }));

            yield break;
        }

        private void SetGod()
        {
            CompSoul soul = this.GetActor().Soul();
            if (soul != null)
            {
                this.targetedGod = soul.ChosenPantheon.GodsListForReading.RandomElementByWeight(x => 1 + soul.FavourTracker.FavourValueFor(x));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<GodDef>(ref this.targetedGod, "targetedGod");
        }
    }
}
