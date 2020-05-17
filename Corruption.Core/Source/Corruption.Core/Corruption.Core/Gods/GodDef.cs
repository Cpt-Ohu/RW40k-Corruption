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
        public string smallTexturePath = "UI/Emperor_bg";

        public string texturePath = "UI/Emperor_bg";

        public string worshipBarPath = "UI/Background/WorshipBarEmperor";

        public string prayerIconPath = "UI/Motes/MotePrayerEmperor";

        public string buttonPath = "UI/Buttons/ButtonEmperor";

        public Texture2D smallTexture;

        public Texture2D texture;

        public Texture2D worshipBarTexture;

        public Texture2D worshipBarFillTexture;

        public Texture2D prayerMote;

        public Texture2D buttonTex;

        public List<SoulTraitDef> patronTraits = new List<SoulTraitDef>();

        public Color mainColor =new Color(0.85f, 0.68f, 12f);

        public List<AbilityDef> psykerPowers = new List<AbilityDef>();

        public GameConditionDef wonderOverlayDef;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.smallTexture = ContentFinder<Texture2D>.Get(this.smallTexturePath, true);
                this.texture = ContentFinder<Texture2D>.Get(this.texturePath, true);
                this.worshipBarTexture = ContentFinder<Texture2D>.Get(this.worshipBarPath, true);
                this.prayerMote = ContentFinder<Texture2D>.Get(this.prayerIconPath, true);
                this.worshipBarFillTexture = SolidColorMaterials.NewSolidColorTexture(this.mainColor);
                this.buttonTex = ContentFinder<Texture2D>.Get(this.buttonPath, true);
            });
        }
    }
}
