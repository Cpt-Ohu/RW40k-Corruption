using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core.Abilities
{
    public class CompAbilityEffect_AffectSettlementRelation : CompAbilityEffect
    {
        public new CompProperties_AffectSettlementRelation Props => base.props as CompProperties_AffectSettlementRelation;

        public override bool CanApplyOn(GlobalTargetInfo target)
        {
            return target.HasWorldObject && target.WorldObject.Faction != Faction.OfPlayer;
        }

        public override void Apply(GlobalTargetInfo target)
        {
            var wo = target.WorldObject;

            var faction = wo.Faction;
            if (Props.goodwillImpact != 0)
            {
                faction.TryAffectGoodwillWith(this.parent.pawn.Faction, this.Props.goodwillImpact, false, true, "LeaderExperiencedVision".Translate(faction.leader.Name), target);
            }
        }
    }

    public class CompProperties_AffectSettlementRelation : CompProperties_AbilityEffect
    {
        public CompProperties_AffectSettlementRelation()
        {
            this.compClass = typeof(CompAbilityEffect_AffectSettlementRelation);
        }
    }
}
