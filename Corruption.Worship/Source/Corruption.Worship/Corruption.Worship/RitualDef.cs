using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Worship
{
    public class RitualDef : Def
    {
        public Type ritualClass;

        public RitualWorker Worker;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.Worker = (RitualWorker)Activator.CreateInstance(this.ritualClass);
            });
        }
    }

    public abstract class RitualWorker
    {
        public abstract void FinishShrineRitual(CompShrine compShrine);
    }
}
