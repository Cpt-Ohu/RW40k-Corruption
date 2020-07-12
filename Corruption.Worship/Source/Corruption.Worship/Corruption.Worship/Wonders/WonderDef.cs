using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Corruption.Worship.Wonders
{
    public class WonderDef : Def
    {
        public Type workerClass;

        public List<GodDef> associatedGods = new List<GodDef>();

        public List<ThingDefCountClass> thingsToSpawn = new List<ThingDefCountClass>();

        public List<HediffDef> hediffsToAdd = new List<HediffDef>();

        private int cooldownTicks;

        public int CooldownTicks => cooldownTicks;

        public float minHediffSeverity = 0.1f;

        public TraitDef traitToGive;

        public List<AbilityDef> unlockedPowers = new List<AbilityDef>();

        public IncidentDef incident;

        public MentalStateDef mentalStateToStart;

        public int incidentPoints;

        public string wonderIconPath;

        public Texture2D wonderIcon;

        public bool pointsScalable;

        public SimpleCurve pointsCurve;

        public int favourCost;
        
        public override void ResolveReferences()
        {
            base.ResolveReferences();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.wonderIcon = ContentFinder<Texture2D>.Get(this.wonderIconPath, true);
                this.workerInt = (WonderWorker)Activator.CreateInstance(this.workerClass);
                this.workerInt.Def = this;
            });
        }

        private WonderWorker workerInt;
        public WonderWorker Worker
        {
            get
            {
                return this.workerInt;
            }
        }

        public int ResolveWonderPoints(int worshipPoints)
        {
            if (this.pointsScalable)
            {

                int result = (int)this.pointsCurve.Evaluate(worshipPoints);
                return result;
            }
            else
            {
                return this.incidentPoints;
            }
        }

        public string ToolTip
        {
            get
            {
                var builder = new StringBuilder();
                builder.AppendLine(this.description);
                builder.AppendLine();
                builder.AppendLine("FavourCost".Translate(new NamedArgument(this.favourCost, "COST")));
                return builder.ToString();
            }
        }

        public override IEnumerable<string> ConfigErrors()
        {
            if (this.mentalStateToStart == null && this.workerClass == typeof(WonderWorker_StartMentalState))
            {
                yield return this.defName + " has type " + typeof(WonderWorker_StartMentalState).Name + " but mentalStateToStart is null";
            }
            if (this.traitToGive == null && this.workerClass == typeof(WonderWorker_AddSpecialTrait))
            {
                yield return this.defName + " has type " + typeof(WonderWorker_AddSpecialTrait).Name + " but traitToGive is null";
            }
            if (this.hediffsToAdd.NullOrEmpty() && this.workerClass == typeof(WonderWorker_AddHediff))
            {
                yield return this.defName + " has type " + typeof(WonderWorker_AddHediff).Name + " but HediffsToAdd are null or empty";
            }
        }
    }
}
