using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Worship
{
    public class BuildingAltar : Building
    {
        public SermonTemplate CurrentActiveSermon;

        public List<SermonTemplate> Templates = new List<SermonTemplate>();

        public bool SermonActive => this.CurrentActiveSermon != null;

        public string RoomName;

        public Pawn preacher = null;

        public Altar_RecordsTracker records = new Altar_RecordsTracker();

        public override void PostMake()
        {
            base.PostMake();
            this.Templates.AddRange(SermonUtility.StandardTemplates().ToList());
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.preacher = Map.mapPawns.FreeColonistsSpawned.RandomElement<Pawn>();
            }
            RoomName = "Temple";
            TickManager f = Find.TickManager;

            f.RegisterAllTickabilityFor(this);
            LessonAutoActivator.TeachOpportunity(WorshipConceptDefOf.AltarKnowledge, OpportunityType.GoodToKnow);

        }

        public override void TickLong()
        {
            base.Tick();
            if (!this.Spawned)
            {
                return;
            }

            if (this.CurrentActiveSermon == null)
            {
                float curHour = GenLocalDate.HourFloat(this.Map);
                foreach (var template in this.Templates)
                {
                    if (template.Active && (int)curHour == template.PreferredStartTime)
                    {
                        if (SermonUtility.ForceSermon(this, template.WorshipAct))
                        {
                            this.CurrentActiveSermon = template;
                            break;
                        }
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look<Pawn>(ref this.preacher, "preacher", false);
            Scribe_Deep.Look<Altar_RecordsTracker>(ref this.records, "records");
            Scribe_Values.Look<string>(ref this.RoomName, "RoomName", "Temple", false);
            Scribe_Deep.Look<SermonTemplate>(ref this.CurrentActiveSermon, "CurrentActiveSermon");
            Scribe_Collections.Look<SermonTemplate>(ref this.Templates, "Templates", LookMode.Deep);
        }
    }
}
