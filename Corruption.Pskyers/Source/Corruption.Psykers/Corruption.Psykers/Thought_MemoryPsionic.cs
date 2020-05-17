using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corruption.Psykers
{
    public class Thought_MemoryPsionic : Thought_Memory
    {
        public int expirationTicks;

        public Thought_MemoryPsionic()
        {
            this.expirationTicks = this.def.DurationTicks;
        }

        public Thought_MemoryPsionic(float durationTicks)
        {
            this.expirationTicks = (int)durationTicks;
        }

        public override bool ShouldDiscard => age > expirationTicks;
    }
}
