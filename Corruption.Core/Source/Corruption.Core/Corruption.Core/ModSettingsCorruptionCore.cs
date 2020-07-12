using Corruption.Core.Gods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class ModSettingsCorruptionCore : ModSettings
    {
        public ModSettingsCorruptionCore()
        {
            var gods = DefDatabase<GodDef>.AllDefsListForReading;

            foreach (var god in gods)
            {
                foreach (var thought in god.pleasedByThought)
                {
                    thought.description += "\n " + "PleasesGod".Translate(new NamedArgument(god.LabelCap, "GOD"));
                }
            }
        }
    }
}
