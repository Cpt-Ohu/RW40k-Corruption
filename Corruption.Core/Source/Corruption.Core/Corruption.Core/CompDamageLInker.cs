using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class CompDamageLinker : ThingComp
    {
        public Thing linkedTo;

        public override void CompTick()
        {
            base.CompTick();
            if (this.linkedTo == null || !this.linkedTo.Spawned)
            {
                this.parent.Destroy();
            }
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            this.linkedTo.TakeDamage(dinfo);
            base.PostPreApplyDamage(dinfo, out absorbed);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look<Thing>(ref this.linkedTo, "thing");
        }
    }
}
