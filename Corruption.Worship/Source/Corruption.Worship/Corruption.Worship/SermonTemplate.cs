using Corruption.Core.Gods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Worship
{
    public class SermonTemplate : IExposable
    {
        public IntRange AvailableRange;

        public bool Active;

        private int preferredStartTime;

        public WorshipActType WorshipAct;

        public int PreferredStartTime => preferredStartTime;

        public float Length;

        public GodDef DedicatedTo;

        public SermonTemplate()
        {
        }

        public void SetStartTime(int value)
        {
            this.preferredStartTime = Mathf.Clamp(value, AvailableRange.min, AvailableRange.max);
        }

        public SermonTemplate(IntRange availableRange, bool active, int startTime, float length, WorshipActType worshipAct)
        {
            AvailableRange = availableRange;
            Active = active;
            preferredStartTime = startTime;
            Length = length;
            this.WorshipAct = worshipAct;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<WorshipActType>(ref WorshipAct, "WorshipAct");
            Scribe_Values.Look<int>(ref preferredStartTime, "preferredStartTime");
            Scribe_Values.Look<float>(ref Length, "Length");
            Scribe_Values.Look<bool>(ref Active, "Active");
            Scribe_Values.Look<bool>(ref Active, "Active");
            Scribe_Values.Look<IntRange>(ref AvailableRange, "AvailableRange");
            Scribe_Defs.Look<GodDef>(ref this.DedicatedTo, "DedicatedTo");
        }
    }
}
