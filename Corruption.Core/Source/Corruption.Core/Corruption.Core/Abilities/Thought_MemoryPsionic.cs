using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corruption.Core.Abilities
{
    public class Thought_MemoryAbility : Thought_Memory
    {
        public int expirationTicks;

        public override bool ShouldDiscard => age > expirationTicks;
    }
}
