using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_DamageEquipment : HediffComp
    {
        public HediffCompProperties_DamageEquipment Props => this.props as HediffCompProperties_DamageEquipment;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.Pawn.RaceProps.intelligence >= Intelligence.ToolUser)
            {
                var equipment = this.Pawn.equipment.AllEquipmentListForReading.Concat(this.Pawn.apparel.WornApparel);
                var targetableGear = equipment.Where(x => this.Props.categoriesToDamage.Any(y => x.def.IsWithinCategory(y))).ToList();
                if (targetableGear.Count > 0)
                {
                    var targetedGear = targetableGear.RandomElement();
                    var dinfo = new DamageInfo(this.Props.damageDef, this.Props.damagePerSecond / 60);
                    targetedGear.TakeDamage(dinfo);
                }

            }
        }
    }


    public class HediffCompProperties_DamageEquipment : HediffCompProperties
    {
        public float damagePerSecond;
        public List<ThingCategoryDef> categoriesToDamage = new List<ThingCategoryDef>();
        public DamageDef damageDef;
        public ThingDef mote;

        public HediffCompProperties_DamageEquipment()
        {
            this.compClass = typeof(HediffComp_DamageEquipment);
        }
    }
}
