using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Psykers
{
    public class HediffComp_Heal : HediffComp
    {
        private HediffCompProperties_Heal HealProperties
        {
            get
            {
                return this.props as HediffCompProperties_Heal;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            float healingAmount = this.HealProperties.healingPower * this.Pawn.HealthScale * 0.1f;
            foreach (var hediff in this.Pawn.health.hediffSet.hediffs.Where(x => x is Hediff_Injury || x.def.makesSickThought == true))
            {
                hediff.Heal(healingAmount);
            }
        }
    }

    public class HediffCompProperties_Heal : HediffCompProperties
    {
        public float healingPower = 8f;

    }
}
