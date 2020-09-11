using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_DemonicAttention : HediffComp
    {
        private bool demonGained;

        public override bool CompShouldRemove => demonGained;

        public HediffCompProperties_DemonicAttention Props => this.props as HediffCompProperties_DemonicAttention;

        public override void Notify_EntropyGained(float baseAmount, float finalAmount, Thing source = null)
        {
            base.Notify_EntropyGained(baseAmount, finalAmount, source);
            this.parent.Severity += finalAmount / 200f * ModSettings_Corruption.PossessionGainFactor; 
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.parent.Severity >= this.parent.def.maxSeverity)
            {
                var possessionHediff = this.Props.possessionHediffs.RandomElement();
                if (possessionHediff != null)
                    this.Pawn.health.AddHediff(possessionHediff);
                this.demonGained = true;
            }
        }
    }

    public class HediffCompProperties_DemonicAttention : HediffCompProperties
    {
        public List<HediffDef> possessionHediffs = new List<HediffDef>();

        public HediffCompProperties_DemonicAttention()
        {
            this.compClass = typeof(HediffComp_DemonicAttention);
        }
    }
}
