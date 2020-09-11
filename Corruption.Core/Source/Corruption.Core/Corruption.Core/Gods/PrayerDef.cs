using Corruption.Core.Gods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core.Gods
{
    public class PrayerDef : Def
    {
        public List<string> prayerLines = new List<string>();

        public WorkTags preferredWorktags = WorkTags.AllWork;

        public int Repeats = 1;

        public int ticksPerLine = 180;

        public GodDef dedicatedTo;

        public bool allowIfDrafted = false;
    
    }
}
