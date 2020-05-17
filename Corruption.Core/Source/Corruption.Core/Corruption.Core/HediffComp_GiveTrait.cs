using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_GiveTrait : HediffComp
    {
        private bool shouldRemoveAfterwards = true;

        public HediffCompProperties_GiveTrait Props
        {
            get
            {
                return this.props as HediffCompProperties_GiveTrait;
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (this.Pawn != null)
            {
                if (this.Pawn.story.traits.GetTrait(this.Props.trait) == null)
                {
                    Trait trait = new Trait(this.Props.trait, 0, true);
                    this.Pawn.story.traits.GainTrait(trait);
                }
                else
                {
                    this.shouldRemoveAfterwards = false;
                }
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            Trait givenTrait = this.Pawn.story.traits.allTraits.FirstOrDefault(x => x.def == this.Props.trait);
            if (givenTrait != null)
            {
                this.Pawn.story.traits.allTraits.Remove(givenTrait);
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<bool>(ref this.shouldRemoveAfterwards, "shouldRemoveAfterwards");
        }
    }

    public class HediffCompProperties_GiveTrait : HediffCompProperties
    {
        public TraitDef trait;

        public HediffCompProperties_GiveTrait()
        {
            this.compClass = typeof(HediffComp_GiveTrait);
        }
    }
}
