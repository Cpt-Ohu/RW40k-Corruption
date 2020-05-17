using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core.Items
{
    public class CompEmpyreal : ThingComp
    {
        private Pawn Holder;

        public CompProperties_Empyreal Props
        {
            get
            {
                return this.props as CompProperties_Empyreal;
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            if(this.TryCacheHolder())
            {
                var soul = this.Holder.Soul();
                if (soul != null)
                {
                    soul.GainCorruption(this.Props.GainRate);
                }
            }
        }

        private bool TryCacheHolder()
        {
            this.Holder = null;
            CompEquippable compEquippable = this.parent.GetComp<CompEquippable>();
            if (compEquippable != null)
            {
                this.Holder = compEquippable.PrimaryVerb.CasterPawn;
            }

            Apparel apparel = this.parent as Apparel;
            if (apparel != null)
            {
                this.Holder = apparel.Wearer;
            }

            return this.Holder != null;
        }
    }

    public class CompProperties_Empyreal : CompProperties
    {
        public float GainRate = 0f;

        public EmpyrealItemNature Nature
        {
            get
            {
                if (this.GainRate > 0f) return EmpyrealItemNature.Corrupted;
                else if (this.GainRate < 0f) return EmpyrealItemNature.Blessed;
                else return EmpyrealItemNature.Unaffected;
            }
        }

        public List<AbilityDef> UnlockableAbilities = new List<AbilityDef>();

        public Gods.GodDef DedicatedGod;
    }

    public enum EmpyrealItemNature
    {
        Unaffected,
        Corrupted,
        Blessed
    }
}
