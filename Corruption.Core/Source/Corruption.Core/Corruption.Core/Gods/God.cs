using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Verse;

namespace Corruption.Core.Gods
{
    public class God
    {
        public GodDef Def;

        public List<GodFavourWorker> FavourWorkers = new List<GodFavourWorker>();

        public God() { }

        public God(GodDef def)
        {
            Def = def;
            foreach (var workClass in def.favourWorkerClasses)
            {
                this.FavourWorkers.Add((GodFavourWorker)Activator.CreateInstance(workClass));
            }
        }
    }
}
