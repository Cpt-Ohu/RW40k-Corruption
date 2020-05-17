using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class HediffComp_ReplacePart : HediffComp
    {
        public HediffCompProperties_ReplacePart Props => this.props as HediffCompProperties_ReplacePart;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            BodyPartRecord part = this.GetPart();
            Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(RimWorld.HediffDefOf.MissingBodyPart, this.Pawn);
            hediff_MissingPart.IsFresh = false;
            hediff_MissingPart.lastInjury = RimWorld.HediffDefOf.Carcinoma;
            hediff_MissingPart.Part = part;
            this.Pawn.health.hediffSet.AddDirect(hediff_MissingPart);
        }

        private BodyPartRecord GetPart()
        {

            var parts = this.Pawn.def.race.body.AllParts.Where(x => x.def == this.Props.partToReplace);

            if (parts.Count() == 0)
            {
                return null;
            }

            var fittingParts = parts.Where(x => this.Pawn.health.hediffSet.PartIsMissing(x) == false);
            if (fittingParts.Count() == 0)
            {
                return null;
            }

            var selectedPart = fittingParts.RandomElement();

            return selectedPart;
        }
    }

    public class HediffCompProperties_ReplacePart : HediffCompProperties
    {
        public BodyPartDef partToReplace;

        public float severityOnExisting;
    }
}
