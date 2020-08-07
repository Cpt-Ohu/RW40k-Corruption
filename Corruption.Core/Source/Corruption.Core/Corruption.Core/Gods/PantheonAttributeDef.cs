using Corruption.Core.Soul;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Corruption.Core.Gods
{
    public class PantheonAttributeDef : Def
    {
        public string iconPath;

        public int effectTick = GenDate.TicksPerDay;

        public float effectChance;

        public Type workerClass = typeof(PantheonAttributeTickWorker);

        public PantheonAttributeTickWorker effectWorker;

        public TraitDef trait;

        public Texture2D Icon = BaseContent.BadTex;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.Icon = ContentFinder<Texture2D>.Get(this.iconPath);
                if (this.workerClass != null)
                {
                    this.effectWorker = (PantheonAttributeTickWorker)Activator.CreateInstance(this.workerClass);
                }
            });
        }
    }

    public class PantheonAttributeTickWorker
    {
        public virtual void TickLong() { }

        public virtual void TickDay() { }
    }
    
    public class PantheonAttributeTickWorker_Trait : PantheonAttributeTickWorker
    {
        private PantheonAttributeDef Def;

        public override void TickDay()
        { 
            foreach (var map in Find.Maps)
            {
                Pawn pawn;
                if(map.mapPawns.FreeColonists.Where(x => !x.story.traits.HasTrait(this.Def.trait)).TryRandomElement(out pawn))
                {
                    pawn.story.traits.GainTrait(new Trait(this.Def.trait));
                    break;
                }
            }
        }
    }

    public class PantheonAttributeTickWorker_Cultist : PantheonAttributeTickWorker
    {
        private PantheonAttributeDef Def;

        public override void TickDay()
        {
            foreach (var map in Find.Maps)
            {
                Pawn pawn;

                if (map.mapPawns.FreeColonists.Where(x => !x.story.traits.HasTrait(this.Def.trait)).TryRandomElement(out pawn))
                {
                    CompSoul soul = pawn.Soul();
                    if (soul != null)
                    {
                        var traits = PantheonDefOf.Chaos.GodsListForReading.Select(x => x.patronTraits);
                        var favouredPatron = PantheonDefOf.Chaos.GodsListForReading.RandomElementByWeight(x => soul.FavourTracker.FavourValueFor(x));
                        var patronTrait = favouredPatron.patronTraits.RandomElement();
                        if (patronTrait != null && !pawn.story.traits.allTraits.Any(x => x.def.ConflictsWith(patronTrait)))
                        {
                            pawn.story.traits.GainTrait(new Trait(patronTrait));
                            break;
                        }
                    }
                }
            }
        }
    }


}
