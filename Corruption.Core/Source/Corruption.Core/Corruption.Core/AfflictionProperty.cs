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
        public FloatRange afflictionRange = new FloatRange(1f, 20000f);
        public int ImmuneDevotionDegree;
        public float resolveFactor = 1f;
        public bool canUseCalls = false;
        public bool useForcedPantheon = false;
        public PantheonDef forcedPantheon;
        public List<FavorProgressTemplate> favorProgressTemplates = new List<FavorProgressTemplate>();
        public List<AbilityDef> startingPowers = new List<AbilityDef>();
    }

    public class FavorProgressTemplate
    {
        public GodDef god;
        public FloatRange initialProgressRange;
    }

}
