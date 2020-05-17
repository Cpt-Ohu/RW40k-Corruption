using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Psykers
{
    [StaticConstructorOnStartup]
    public static class PsykerUtility
    {
        public static Color AbilityUnlockedBorder = TexUI.HighlightLineResearchColor;
        public static Color AbilityUnlockedBG = new ColorInt(26, 38, 46).ToColor;
        public static Color AbilityLockedBorder = TexUI.DefaultBorderResearchColor;
        public static Color AbilityLockedBG = new ColorInt(42, 42, 42).ToColor;
        public static Color AbilitySelectedBG = new ColorInt(72, 72, 72).ToColor;
        public static Color AbilityPerequisiteBorder = new Color(1f, 0f, 0f);

        public static readonly Texture2D PsykerIcon = ContentFinder<Texture2D>.Get("UI/Abilities/PsykerLearning", true);
        public static readonly Texture2D PowerLevelKappa = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelKappa", true);
        public static readonly Texture2D PowerLevelIota = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelIota", true);
        public static readonly Texture2D PowerLevelZeta = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelZeta", true);
        public static readonly Texture2D PowerLevelEpsilon = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelEpsilon", true);
        public static readonly Texture2D PowerLevelDelta = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelDelta", true);
        public static readonly Texture2D PowerLevelBeta = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelBeta", true);

        public static readonly Texture2D PowerCooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(9, 203, 4, 64));

        public static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG");
        public static readonly Texture2D BGTexHighlight = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBGHighlight");
        
        public static readonly Dictionary<int, int> PsykerDegreeXPCost = new Dictionary<int, int>{
            {1, 50 },
            {2, 150 },
            {3, 250 },
            {4, 400 },
            };

        public static Dictionary<int, int> PsykerAbilitieSlots = new Dictionary<int, int>()
        {
            {1, 2 },
            {2, 4 },
            {3, 3 },
            {4, 2 },
            {5, 1 }
        };

        public static List<AbilityDef> GetPowerDefsFor(int level = 0, GodDef god = null)
        {
            if (god == null)
            {
                return DefDatabase<AbilityDef>.AllDefsListForReading.Where(x => x.level == level).ToList();
            }
            return god.psykerPowers.FindAll(x => x.level == level);
        }

        public static Dictionary<int, string> PowerLevelLetters = new Dictionary<int, string>()
        {
            {10, "Kappa" },
            {20, "Iota" },
            {30, "Zeta" },
            {40, "Epsilon" },
            {50, "Delta" },
            {60, "Beta" }
        };
    }
}
