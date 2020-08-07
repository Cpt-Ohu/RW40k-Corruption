using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corruption.Core
{
    class Plant_NormalTicker : Plant
    {
        public override void TickLong()
        {
            base.TickLong();
            foreach (var comp in this.AllComps)
            {
                comp.CompTick();
            }
        }
    }
}
