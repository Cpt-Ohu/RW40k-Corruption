using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core.Soul
{
    public static class SoulExtensions
    {
        public static CompSoul Soul(this Pawn pawn)
        {
            return pawn.GetComp<CompSoul>();
        }
    }
}
