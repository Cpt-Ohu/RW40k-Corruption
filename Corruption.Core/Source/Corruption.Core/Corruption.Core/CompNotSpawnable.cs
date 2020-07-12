using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class CompNotSpawnable : ThingComp
    {
        public override void CompTick()
        {
            base.CompTick();
            this.parent.Destroy();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            Log.Warning($"Tried to spawn {this.parent.def.defName}. This shouldn't happen");
            this.parent.Destroy();
        }
    }
}
