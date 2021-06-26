using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Abilities
{
    public class CompAbilityEffect_Recruit : CompAbilityEffect
    {
        public new CompProperties_AbilityRecruit Props => base.props as CompProperties_AbilityRecruit;

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var p = target.Pawn;
            return p != null && p.Faction != Faction.OfPlayer && p.RaceProps.Humanlike;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var p = target.Pawn;
            if(p != null)
            {
                if (p.Faction != Faction.OfPlayer && p.RaceProps.Humanlike)
                {
                    InteractionWorker_RecruitAttempt.DoRecruit(this.parent.pawn, p, 1f);

                    if (this.Props.mote != null)
                    {
                        MoteMaker.MakeAttachedOverlay(this.parent.pawn, this.Props.mote, Vector3.zero);
                        MoteMaker.MakeAttachedOverlay(p, this.Props.mote, Vector3.zero);
                    }
                }
            }
        }
    }

    public class CompProperties_AbilityRecruit : CompProperties_AbilityEffect
    {
        public CompProperties_AbilityRecruit()
        {
            this.compClass = typeof(CompAbilityEffect_Recruit);
        }

        public ThingDef mote;

    }
}
