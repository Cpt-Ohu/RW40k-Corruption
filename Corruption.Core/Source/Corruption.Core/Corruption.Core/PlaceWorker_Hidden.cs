using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class PlaceWorker_Hidden : PlaceWorker
    {
        public override bool IsBuildDesignatorVisible(BuildableDef def)
        {
            return CorruptionStoryTracker.Current.UnlockedHiddenDefs.Contains(def);
        }
    }
}
