using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class DamageDefMutation : DamageDef
    {
        public List<DamageDefAdditionalHediff> mutationHediffs = new List<DamageDefAdditionalHediff>();
    }

    public class DamageWorker_Mutate : DamageWorker
    {
        public DamageDefMutation Def => this.def as DamageDefMutation;

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            var damageResult = base.Apply(dinfo, victim);

            Pawn pawn = victim as Pawn;

            if (pawn != null)
            {

                var existingHediffs = pawn.health.hediffSet.hediffs.FindAll(x => this.def.additionalHediffs.Exists(y => y.hediff == x.def));

                var selectedHediff = existingHediffs.FindAll(x => x.Severity < x.def.maxSeverity).RandomElement();
                var selectedAdditionalHediff = this.Def.mutationHediffs.FirstOrDefault(x => x.hediff == selectedHediff.def);

                float severity = 0f;
                if (selectedHediff != null && Rand.Value > 0.2f)
                {
                    severity = MutationSeverity(dinfo, selectedAdditionalHediff);
                    selectedHediff.Severity += (selectedAdditionalHediff.severityFixed <= 0f) ? (dinfo.Amount * selectedAdditionalHediff.severityPerDamageDealt) : selectedAdditionalHediff.severityFixed;
                }
                else
                {
                    var randomDef = this.Def.mutationHediffs.RandomElement();
                    severity = MutationSeverity(dinfo, randomDef);
                    Hediff hediff = HediffMaker.MakeHediff(randomDef.hediff, pawn);
                    pawn.health.AddHediff(hediff);
                    hediff.Severity += severity;
                }

            }

            return damageResult;
        }

        private static float MutationSeverity(DamageInfo dinfo, DamageDefAdditionalHediff selectedAdditionalHediff)
        {
            float num = (selectedAdditionalHediff.severityFixed <= 0f) ? (dinfo.Amount * selectedAdditionalHediff.severityPerDamageDealt) : selectedAdditionalHediff.severityFixed;
            return num;
        }
    }
}
