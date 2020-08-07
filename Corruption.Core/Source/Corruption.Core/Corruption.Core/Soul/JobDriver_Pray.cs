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
            CompSoul soul = this.GetActor().Soul();
            Toil lastToil = new Toil();
            IEnumerator<Toil> enumerator = base.MakeNewToils().GetEnumerator();
            while (enumerator.MoveNext())
            {
                lastToil = enumerator.Current;
                yield return enumerator.Current;
            }

            lastToil.AddPreInitAction(new Action(delegate
            {
                soul?.PrayerTracker.StartRandomPrayer(this.job, true);
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
