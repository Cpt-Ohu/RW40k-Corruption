using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core.Soul
{
    class Pawn_ChosenTracker : IExposable
    {
        private Pawn pawn;

        public Pawn_ChosenTracker() { }

        public Pawn_ChosenTracker(Pawn pawn)
        {
            this.pawn = pawn;
        }

        public void ExposeData()
        {
            throw new NotImplementedException();
        }
    }
}
