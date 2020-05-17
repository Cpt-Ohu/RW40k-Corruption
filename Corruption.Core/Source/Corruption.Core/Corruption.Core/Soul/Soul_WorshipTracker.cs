﻿using Corruption.Core.Gods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Core.Soul
{
    public class Soul_FavourTracker : IExposable
    {
        private List<FavourProgress> Favours;
        private CompSoul Soul;

        public FavourProgress HighestFavour => this.Favours.MaxBy(x => x.Favour);

        public Soul_FavourTracker(CompSoul soul)
        {
            this.Soul = soul;
            this.Favours = new List<FavourProgress>();
        }

        public void TryAddProgressFor(GodDef god, float change)
        {
            var worship = this.Favours.FirstOrDefault(x => x.God == god);
            worship.TryAddProgress(change);
        }

        public IEnumerable<FavourProgress> FavorProgressForChosenPantheon()
        {
            return this.Favours.Where(x => this.Soul.ChosenPantheon.IsMember(x.God));
        }

        public GodsFavourLevel FavourLevelFor(GodDef god)
        {
            var favour = this.Favours.FirstOrDefault(x => x.God == god);
            if (favour != null)
            {
                return favour.FavourLevel;
            }
            else
            {
                return GodsFavourLevel.Unknown;
            }
        }

        internal void TryAddGods(List<GodDef> gods)
        {
            this.TryAddGods(gods, FloatRange.Zero);
        }

        internal void TryAddGods(List<GodDef> gods, FloatRange startingRange)
        {
            var newGods = gods.Where(x => !this.Favours.Any(w => w.God == x));
            foreach(var god in newGods)
            {
                this.Favours.Add(new FavourProgress(god, startingRange.RandomInRange));
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look<FavourProgress>(ref this.Favours, "favours", LookMode.Deep);
        }

    }
}