using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace Corruption.Core
{
    public class MotePulse : Mote
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (this.def.mote.landSound != null)
            {
                this.def.mote.landSound.PlayOneShot(new TargetInfo(this.Position, this.Map));
            }
        }
    }
}
