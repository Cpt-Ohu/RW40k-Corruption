﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Corruption.Core.Items
{

    public class Projectile_Smoking : Projectile_Laser
    {
        private int ticksToDetonation;

        private bool istraveling = true;

        private bool exploded = false;

        private Vector3 SpawnPosition;

        private Vector3 ActualPosition
        {
            get
            {
                if (this.istraveling)
                {
                    return this.ExactPosition;
                }
                else
                {
                    return this.destination;
                }
            }
        }

        private Vector3 SmokeDrawingPos
        {
            get
            {
                return this.ActualPosition - (this.ActualPosition - this.origin) / 2f;
            }
        }

        private Mesh drawingMesh;

        private Graphic smokeMaterialInt;

        public Graphic SmokeMaterial
        {
            get
            {
                if (!this.additionalParameters.smokeGraphicPath.NullOrEmpty())
                {
                    if (this.smokeMaterialInt == null)
                    {
                        this.smokeMaterialInt = GraphicDatabase.Get<Graphic_Single>(this.additionalParameters.smokeGraphicPath, ShaderDatabase.MoteGlow);
                    }
                }
                else
                {
                    this.smokeMaterialInt = null;
                }
                return smokeMaterialInt;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.SpawnPosition = this.ExactPosition;
        }

        public override void Draw()
        {
            base.Comps_PostDraw();
            if (this.istraveling || !this.exploded)
            {
                Graphics.DrawMesh(MeshPool.plane10, this.ActualPosition, this.ExactRotation, this.def.DrawMatSingle, 2);
            }
            if (this.SmokeMaterial != null)
            {
                Graphics.DrawMesh(drawingMesh, this.SmokeDrawingPos, this.ExactRotation, FadedMaterialPool.FadedVersionOf(this.smokeMaterialInt.MatSingle, this.drawingIntensity), 2);
            }
        }

        public override void GetPreFiringDrawingParameters()
        {
            base.GetPreFiringDrawingParameters();

        }

        public override void GetPostFiringDrawingParameters()
        {
            base.GetPostFiringDrawingParameters();
            float length = ((this.destination - this.origin) * Math.Min(1f, (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact))).magnitude;
            this.SmokeMaterial.drawSize = new Vector2(1f, 1f * (length));
            drawingMesh = this.SmokeMaterial.MeshAt(Rot4.North);
        }

        public override void Tick()
        {
            try
            {
                //this.DoBaseTick();
                if (tickCounter == 0)
                {
                    GetParametersFromXml();
                    PerformPreFiringTreatment();
                }

                // Pre firing.
                if (tickCounter < preFiringDuration)
                {
                    tickCounter++;
                    GetPreFiringDrawingParameters();
                }
                // Firing.
                else if (tickCounter > this.preFiringDuration && this.istraveling)
                {
                    GetPostFiringDrawingParameters();
                }
                // Post firing.
                else
                {
                    GetPostFiringDrawingParameters();
                    this.ticksToDetonation--;
                    tickCounter++;
                }
                if ((tickCounter > (this.preFiringDuration + this.postFiringDuration)) && (!this.additionalParameters.causesExplosion || (this.additionalParameters.causesExplosion && this.ticksToDetonation > this.def.projectile.explosionDelay)) && !this.istraveling && !this.Destroyed)
                {
                    this.Destroy(DestroyMode.Vanish);
                }
                if (this.launcher != null)
                {
                    if (this.launcher is Pawn)
                    {
                        Pawn launcherPawn = this.launcher as Pawn;
                        if ((((launcherPawn.Dead) == true) && !this.Destroyed))
                        {
                            this.Destroy(DestroyMode.Vanish);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Destroy(DestroyMode.Vanish);
            }

        }

        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            if (this.additionalParameters.causesExplosion)
            {
                if (this.def.projectile.explosionDelay == 0)
                {
                    this.istraveling = false;
                    if (!this.exploded)
                    {
                        this.Explode();
                        this.exploded = true;
                    }
                    return;
                }
                this.landed = true;
                this.ticksToDetonation = this.def.projectile.explosionDelay;
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
            }
            else
            {
                if (hitThing != null)
                {
                    ThingDef equipmentDef = this.equipmentDef;
                    DamageInfo dinfo = new DamageInfo(this.def.projectile.damageDef, (float)base.DamageAmount, base.ArmorPenetration, this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                    hitThing.TakeDamage(dinfo);
                }
                else
                {
                    SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
                    MoteMaker.MakeStaticMote(this.ActualPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                    if (base.Position.GetTerrain(map).takeSplashes)
                    {
                        MoteMaker.MakeWaterSplash(this.ActualPosition, map, Mathf.Sqrt((float)this.def.projectile.explosionRadius) * 1f, 4f);
                    }
                }
                this.landed = true;
            }
            this.istraveling = false;
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {;
            base.DeSpawn(mode);
        }

        protected virtual void Explode()
        {
            Map map = base.Map;
            ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
            float explosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
            GenExplosion.DoExplosion(base.Position, map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, this.launcher, base.DamageAmount, base.ArmorPenetration, this.def.projectile.soundExplode, this.def, this.equipmentDef, this.intendedTarget.Thing, this.def.projectile.postExplosionSpawnThingDef, this.def.projectile.postExplosionSpawnChance, 1, false, preExplosionSpawnThingDef, explosionSpawnChance, 1);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector3>(ref this.SpawnPosition, "SpawnPosition", Vector3.one);
            Scribe_Values.Look(ref this.ticksToDetonation, "ticksToDetonation", 0);
            Scribe_Values.Look(ref this.istraveling, "istraveling", true);
            Scribe_Values.Look(ref this.exploded, "exploded", true);
        }
    }
}
