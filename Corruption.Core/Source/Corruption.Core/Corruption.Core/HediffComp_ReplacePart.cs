using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_ReplacePart : HediffComp
    {
        public HediffCompProperties_ReplacePart Props => this.props as HediffCompProperties_ReplacePart;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            //BodyPartRecord part = this.GetPart();
            //Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(RimWorld.HediffDefOf.MissingBodyPart, this.Pawn);
            //hediff_MissingPart.IsFresh = false;
            //hediff_MissingPart.lastInjury = this.Def;
            //hediff_MissingPart.Part = this.parent.Part;
            //this.Pawn.health.hediffSet.AddDirect(hediff_MissingPart);
        }
    }

    public class HediffCompProperties_ReplacePart : HediffCompProperties
    {
        public BodyPartDef partToReplace;

        public float severityOnExisting;
    }
}
