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
    public class PantheonDef : Def
    {
        public Texture2D FillTex;

        public List<GodDef> GodsListForReading => this.members.Select(x => x.god).ToList();

        public bool IsMember(GodDef god) => this.GodsListForReading.Contains(god);

        public List<PantheonMember> members = new List<PantheonMember>();

        public List<PantheonAttributeDef> pantheonAttributes = new List<PantheonAttributeDef>();

        public List<PantheonDef> approvingPantheons = new List<PantheonDef>();
        public List<PantheonDef> rejectingPantheons = new List<PantheonDef>();
        public List<FactionDef> approvingFactions = new List<FactionDef>();
        public List<FactionDef> rejectingFactions = new List<FactionDef>();

        public SimpleCurve takeoverThresholds = new SimpleCurve(new List<CurvePoint>() { new CurvePoint(0.8f,1f) });

        public bool violentTakeover;

        public int approvingGoodwill;

        public int rejectingGoodwill;

        public Color mainColor = Color.white;

        public string iconPath = "UI/Pantheons/ChaosIcon";

        public string prayerTitle;

        public Texture2D Icon;

        public string requiresMod = null;

        public bool immuneToChaos = false;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.FillTex = SolidColorMaterials.NewSolidColorTexture(this.mainColor);
                this.Icon = ContentFinder<Texture2D>.Get(this.iconPath);
            });
        }
    }

    public class PantheonMember
    {
        public GodDef god;
        public float pantheonWeight = 1f;
    }
}
