using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Gods
{
    public class GodDef : Def
    {
        public List<Type> favourWorkerClasses = new List<Type>();

        public float favourCorruptionFactor = 1f;

        public string smallTexturePath = "UI/Emperor_bg";

        public string texturePath = "UI/Emperor_bg";

        public string worshipBarPath = "UI/Background/WorshipBarEmperor";

        public string prayerIconPath = "UI/Motes/MotePrayerEmperor";

        public string buttonPath = "UI/Buttons/ButtonEmperor";

        public Texture2D SmallTexture;

        public Texture2D Texture;

        public Texture2D WorshipBarTexture;

        public Texture2D WorshipBarFillTexture;

        public Texture2D PrayerMote;

        public Texture2D ButtonTex;

        public List<JobDef> pleasedByJobs = new List<JobDef>();

        public List<TraitDef> patronTraits = new List<TraitDef>();

        public Color mainColor =new Color(0.85f, 0.68f, 12f);

        public Color cultColorOne;

        public Color cultColorTwo;
                
        public List<AbilityDef> psykerPowers = new List<AbilityDef>();

        public List<ThoughtDef> pleasedByThought = new List<ThoughtDef>();

        public List<string> pleasedByThoughtTags = new List<string>();

        public bool pleasedByBattle = false;

        public float battleFavourFactor = 1f;

        public List<WorktagFavour> pleasedByWorkTags = new List<WorktagFavour>();

        public List<string> favourSourceDescriptions = new List<string>();

        public ThingDef effectMote;

        public GameConditionDef wonderOverlayDef;

        public bool acceptsPrayers = true;

        public List<BuildableDef> UnlockableBuildables = new List<BuildableDef>();

        public List<PantheonDef> approvesBattlingPantheon = new List<PantheonDef>();

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.SmallTexture = ContentFinder<Texture2D>.Get(this.smallTexturePath, true);
                this.Texture = ContentFinder<Texture2D>.Get(this.texturePath, true);
                this.WorshipBarTexture = ContentFinder<Texture2D>.Get(this.worshipBarPath, true);
                this.PrayerMote = ContentFinder<Texture2D>.Get(this.prayerIconPath, true);
                this.WorshipBarFillTexture = SolidColorMaterials.NewSolidColorTexture(this.mainColor);
                this.ButtonTex = ContentFinder<Texture2D>.Get(this.buttonPath, true);
            });
        }
    }
}
