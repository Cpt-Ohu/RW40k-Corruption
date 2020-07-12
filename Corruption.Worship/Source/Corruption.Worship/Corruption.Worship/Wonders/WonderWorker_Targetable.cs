using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Corruption.Worship.Wonders
{
    public class WonderWorker_Targetable : WonderWorker
    {
        protected TargetInfo target;

        protected virtual TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetBuildings = false,                
                validator = ((TargetInfo x) => BaseTargetValidator(x.Thing))
            };
        }

        protected void StartTargeting(GodDef god, int worshipPoints)
        {
            if (this.target == null)
            {
                Log.Message("Returning Favour");
                GlobalWorshipTracker.Current.TryAddFavor(god, worshipPoints);
                return;
            }
            if (this.target != null && !this.GetTargetingParameters().CanTarget(this.target))
            {
                Log.Message("Returning Favour");
                GlobalWorshipTracker.Current.TryAddFavor(god, worshipPoints);
                return;
            }
            TryDoEffectOnTarget(worshipPoints);
        }

        protected virtual void TryDoEffectOnTarget(int worshipPoints)
        {

        }

        public bool BaseTargetValidator(Thing t)
        {
            return true;
        }

        public override bool TryExecuteWonder(GodDef god, int worshipPoints)
        {
            Find.Targeter.BeginTargeting(this.GetTargetingParameters(), delegate (LocalTargetInfo t)
            {
                this.target = t.ToTargetInfo(Find.CurrentMap);
                this.StartTargeting(god, worshipPoints);
            }, null, null, null);
            return true;
        }
    }
}
