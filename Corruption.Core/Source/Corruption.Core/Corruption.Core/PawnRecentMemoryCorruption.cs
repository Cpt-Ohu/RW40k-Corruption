using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class PawnRecentMemoryCorruption : PawnRecentMemory
    {
        public int LastKillTick = int.MaxValue;

        public PawnRecentMemoryCorruption(Pawn pawn) : base(pawn)
        {

        }        
    }
}
