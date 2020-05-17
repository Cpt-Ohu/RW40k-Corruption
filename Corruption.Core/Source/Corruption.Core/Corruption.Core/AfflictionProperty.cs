using Corruption.Core.Gods;
using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class AfflictionProperty
    {
        public float lowerAfflictionLimit = 1f;
        public float upperAfflictionLimit = 24000f;
        public int ImmuneDevotionDegree;
        public float resolveFactor = 1f;
        public bool canUseCalls = false;
        public bool useForcedPantheon = false;
        public PantheonDef forcedPantheon;
        public List<FavorProgressTemplate> favorProgressTemplates = new List<FavorProgressTemplate>();
        public List<AbilityDef> forcedPsykerPowers = new List<AbilityDef>();
        public IntRange psykerPowerLevelRange = IntRange.zero;
    }

    public class FavorProgressTemplate
    {
        public GodDef god;
        public FloatRange initialProgressRange;
    }
}
