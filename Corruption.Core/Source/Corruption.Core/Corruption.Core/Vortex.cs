using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Corruption.Core
{
    public class Vortex : ThingWithComps, ISizeReporter
    {
        private int spawnTick;
        public Pawn Instigator;

        private Effecter effecter;
        private Sustainer sustainer;

        public Vector2 DrawSize { get; set; }

        private float _radius;

        public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                this.DrawSize = new Vector2(value, value);
            }
        }


        public float AgeSecs
        {
            get
            {
                return (float)(Find.TickManager.TicksGame - spawnTick) / 60f;
            }
        }

        public int LifeTimeSecs;

        private int ticksToEffect;

        private CompVortex _vortexComp;

        public CompVortex VortexComp
        {
            get
            {
                if (_vortexComp == null)
                {
                    _vortexComp = this.GetComp<CompVortex>();
                }
                return _vortexComp;
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            this.Radius = this.VortexComp.Props.effectRadius;
            this.LifeTimeSecs = this.VortexComp.Props.avgLifetime.RandomInRange;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.spawnTick = GenTicks.TicksGame;
            MoteMaker.ThrowLightningGlow(this.DrawPos, this.Map, this.Radius);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (sustainer != null)
            {
                if (sustainer.externalParams.sizeAggregator == null)
                {
                    sustainer.externalParams.sizeAggregator = new SoundSizeAggregator();
                }
                sustainer.externalParams.sizeAggregator.RemoveReporter(this);
            }
            if (this.effecter != null)
            {
                effecter.Cleanup();
                effecter = null;
            }
            base.DeSpawn(mode);
        }

        public float exactRotation = 0f;

        public override void Draw()
        {
            this.Graphic.DrawWorker(this.DrawPos, Rot4.North, this.def, this, this.exactRotation);
            //Graphics.DrawMesh(MeshPool.GridPlane(this.DrawSize), DrawPos, Quaternion.AngleAxis(exactRotation, Vector3.up), this.Graphic.MatSingle, 0);
            Comps_PostDraw();
        }

        public override void Tick()
        {
            base.Tick();
            this.TrySpawnEffecter();
            ticksToEffect--;
            this.exactRotation += this.VortexComp.Props.rotationPerTick;
            if (this.exactRotation > 360) this.exactRotation = 0;
            if (ticksToEffect <= 0)
            {
                ticksToEffect = this.VortexComp.Props.ticksPerEffectCycle;
                var targets = GetAffectedTargets();
                ApplyEffects(targets);
            }

            if (this.AgeSecs > this.LifeTimeSecs + this.def.mote.fadeOutTime)
            {
                this.Destroy();
            }
            if (this.VortexComp.Props.sustainerSound == null)
            {
                return;
            }
            else if (sustainer != null)
            {
                sustainer.Maintain();
            }
            else if (!base.Position.Fogged(base.Map))
            {
                SoundInfo info = SoundInfo.InMap(new TargetInfo(base.Position, base.Map), MaintenanceType.PerTick);
                sustainer = SustainerAggregatorUtility.AggregateOrSpawnSustainerFor(this, this.VortexComp.Props.sustainerSound, info);
            }
        }

        private void TrySpawnEffecter()
        {
            if (this.VortexComp.Props.effecterDef != null && this.effecter == null)
            {
                this.effecter = this.VortexComp.Props.effecterDef.Spawn();
            }
            if (this.effecter != null)
            {
                this.effecter.EffectTick(this, new TargetInfo(this.Position, this.Map));
            }
        }

        public void ApplyEffects(IEnumerable<LocalTargetInfo> targets)
        {
            foreach (var target in targets)
            {
                Thing hitThing = target.Thing;
                if (hitThing != null)
                {
                    if (this.VortexComp.Props.drawToCenter == true)
                    {
                        hitThing.Position = this.Position;
                    }
                    if (this.VortexComp.Props.damageDef != null)
                    {
                        DamageInfo dinfo = new DamageInfo(this.VortexComp.Props.damageDef, this.VortexComp.Props.damageAmount, this.VortexComp.Props.armorPenetration, -1f, this.Instigator, null, null, DamageInfo.SourceCategory.ThingOrUnknown, hitThing);
                        hitThing.TakeDamage(dinfo);
                    }
                    if (this.VortexComp.Props.hediffToGive != null)
                    {
                        TryGiveHediff(hitThing);
                    }
                    if (this.VortexComp.Props.moteOnEffect != null)
                    {
                        MoteMaker.ThrowMetaIcon(hitThing.Position, hitThing.Map, this.VortexComp.Props.moteOnEffect);
                    }
                }
            }
        }

        private void TryGiveHediff(Thing hitThing)
        {
            var pawn = hitThing as Pawn;
            if (pawn != null)
            {
                if (this.VortexComp.Props.hediffSeverityToAdd == 0)
                {
                    pawn.health.AddHediff(this.VortexComp.Props.hediffToGive, null, null);
                }
                else
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.VortexComp.Props.hediffToGive);
                    if (hediff == null)
                    {
                        hediff = pawn.health.AddHediff(this.VortexComp.Props.hediffToGive);
                    }
                    hediff.Severity += this.VortexComp.Props.hediffSeverityToAdd;
                }
            }
        }

        public IEnumerable<LocalTargetInfo> GetAffectedTargets()
        {
            foreach (LocalTargetInfo item in from t in GenRadial.RadialDistinctThingsAround(this.Position, this.Map, this.VortexComp.Props.effectRadius, useCenter: true)
                                             where this.VortexComp.Props.verb.targetParams.CanTarget(t)
                                             select new LocalTargetInfo(t))
            {
                yield return item;
            }
        }

        public virtual float Alpha
        {
            get
            {
                float ageSecs = AgeSecs;
                if (ageSecs <= def.mote.fadeInTime)
                {
                    if (def.mote.fadeInTime > 0f)
                    {
                        return ageSecs / def.mote.fadeInTime;
                    }
                    return 1f;
                }
                if (ageSecs <= def.mote.fadeInTime + LifeTimeSecs)
                {
                    return 1f;
                }
                if (def.mote.fadeOutTime > 0f)
                {
                    return 1f - Mathf.InverseLerp(def.mote.fadeInTime + LifeTimeSecs, def.mote.fadeInTime + LifeTimeSecs + def.mote.fadeOutTime, ageSecs);
                }
                return 1f;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this._radius, "radius");
        }

        public float CurrentSize()
        {
            return this.Radius;
        }
    }

    public class CompVortex : ThingComp
    {
        public CompProperties_Vortex Props => this.props as CompProperties_Vortex;

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }

    public class CompProperties_Vortex : CompProperties
    {
        public float effectRadius;
        public int ticksPerEffectCycle = 60;
        public DamageDef damageDef;
        public int damageAmount;
        public float armorPenetration;
        public HediffDef hediffToGive;
        public float hediffSeverityToAdd = -1;
        public VerbProperties verb = new VerbProperties();
        public IntRange avgLifetime = IntRange.one;
        public ThingDef moteOnEffect;
        public float drawToCenterSpeed = 0f;
        public bool drawToCenter;
        public float rotationPerTick = 1f;
        public SoundDef sustainerSound;

        public EffecterDef effecterDef;

        public CompProperties_Vortex()
        {
            this.compClass = typeof(CompVortex);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            if (this.moteOnEffect == null) this.moteOnEffect = ThingDefOf.Mote_ExplosionFlash;
            return base.ConfigErrors(parentDef);
        }

    }
}
