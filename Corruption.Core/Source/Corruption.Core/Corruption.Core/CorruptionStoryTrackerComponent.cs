using Corruption.Core.Soul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Corruption.Core
{
    public abstract class CorruptionStoryTrackerComponent : IExposable
    {
        public virtual void ExposeData()
        {
        }

        public virtual void Initialize() { }

        public virtual void Notify_PawnCorrupted(CompSoul soul) { }

        public virtual void Notify_PawnPantheonChanged(CompSoul soul) { }
    }
}
