using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

        private sealed class PotentialMutation
        {
            public PotentialMutation(DamageDefAdditionalHediff def, BodyPartRecord bodyPart)
            {
                Def = def;
                Part = bodyPart;
            }

            public DamageDefAdditionalHediff Def { get; set; }
            public BodyPartRecord Part { get; set; }
        }

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            var damageResult = base.Apply(dinfo, victim);
            Pawn pawn = victim as Pawn;
            if (pawn != null)
            {
                MutationUtility.ApplyMutation(pawn, this.Def.mutationHediffs.Select(x => x.hediff).ToList(), this.Def.mutationHediffs.Average(x => x.severityPerDamageDealt) * dinfo.Amount);
            }
            return damageResult;
        }
    }
}
