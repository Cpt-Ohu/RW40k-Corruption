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
                    this.effectWorker.Def = this;
                }
            });
        }
    }

    public class PantheonAttributeTickWorker
    {
        public PantheonAttributeDef Def;

        public virtual void TickLong() { }

        public virtual void TickDay() { }
    }
    
    public class PantheonAttributeTickWorker_Trait : PantheonAttributeTickWorker
    {

        public override void TickDay()
        { 
            foreach (var map in Find.Maps)
            {
                Pawn pawn;
                var playerPawns = map.mapPawns.FreeColonists.Where(x => x.story != null && !x.story.traits.HasTrait(this.Def.trait));
                if (playerPawns.Count() > 0)
                {
                    if (playerPawns.TryRandomElement(out pawn))
                    {
                        pawn.story.traits.GainTrait(new Trait(this.Def.trait));
                        break;
                    }
                }
            }
        }
    }

    public class PantheonAttributeTickWorker_Cultist : PantheonAttributeTickWorker
    {
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
