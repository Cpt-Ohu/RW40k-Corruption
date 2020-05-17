using Corruption.Core.Gods;
using Corruption.Core.Soul;
using JetBrains.Annotations;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Worship
{
    public class GlobalWorshipTracker : WorldComponent
    {
        public GlobalWorshipTracker(World world) : base(world)
        {
            this.PlayerPantheon = PantheonDefOf.ImperialCult;
        }

        private PantheonDef playerPantheon;

        public PantheonDef PlayerPantheon
        {
            get { return playerPantheon; }
            set
            {
                playerPantheon = value;
                foreach (var member in value.members)
                {
                    if (!this.Favours.Any(x => x.God == member.god))
                    {
                        this.Favours.Add(new FavourProgress(member.god, 0f));
                    }
                }
            }
        }

        public List<FavourProgress> Favours = new List<FavourProgress>();

        public List<FavourProgress> PantheonFavours
        {
            get
            {
                return this.Favours.FindAll(x => this.PlayerPantheon.IsMember(x.God));
            }
        }

        public float PantheonFavourPercentage
        {
            get
            {
                var pantheonMembers = PlayerPantheon.members;
                float totalWeight = pantheonMembers.Sum(x => x.pantheonWeight);
                float curProgress = 0.001f;
                foreach (var member in pantheonMembers)
                {
                    var progress = GetFavourProgressFor(member.god);
                    curProgress += (member.pantheonWeight / totalWeight) * progress.FavourPercentage;
                }
                return curProgress;
            }
        }

        public FavourProgress GetFavourProgressFor(GodDef god)
        {
            foreach (var progress in this.Favours)
            {
                if (progress.God.Equals(god))
                {
                    return progress;
                }
            }
            return null;
        }

        public void TryAddFavor(GodDef god, float value)
        {
            var favor = this.Favours.FirstOrDefault(x => x.God == god);
            if (favor == null)
            {
                this.Favours.Add(new FavourProgress(god, value));
            }
            else
            {
                favor.TryAddProgress(value);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<PantheonDef>(ref this.playerPantheon, "playerPantheon");
            Scribe_Collections.Look<FavourProgress>(ref this.Favours, "favors", LookMode.Deep);
        }

        public static GlobalWorshipTracker Current
        {
            get
            {
                return Find.World.GetComponent<GlobalWorshipTracker>();
            }
        }

        internal void ConsumeFavourFor(int value, GodDef god)
        {
            this.GetFavourProgressFor(god).Favour -= value;
        }
    }
}
