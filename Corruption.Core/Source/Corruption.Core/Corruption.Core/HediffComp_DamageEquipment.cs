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
            foreach (var gear in this.Pawn.equipment.AllEquipmentListForReading.Concat(this.Pawn.apparel.WornApparel))
            {
                if (gear.def.thingCategories.Any(x => this.Props.categoriesToDamage.Contains(x)))
                {
                    var dinfo = new DamageInfo(this.Props.damageDef, this.Props.damagePerSecond);
                    gear.TakeDamage(dinfo);
                }
            }
        }
    }

    public class HediffCompProperties_DamageEquipment : HediffCompProperties
    {
        public float damagePerSecond;
        public List<ThingCategoryDef> categoriesToDamage = new List<ThingCategoryDef>();
        public DamageDef damageDef;

        public HediffCompProperties_DamageEquipment()
        {
            this.compClass = typeof(HediffComp_DamageEquipment);
        }
    }
}
