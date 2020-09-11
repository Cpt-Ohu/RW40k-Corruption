using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Items
{
    public class Projectile_Trail : Bullet
    {
        private bool impacted = false;
        private Comp_ProjectileTrail TrailComp => this.TryGetComp<Comp_ProjectileTrail>();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.equipmentDef = this.launcher?.def ?? ThingDefOf.Human;
        }

        public override void Draw()
        {
            if (this.TrailComp != null)
            {
                GenDrawExtension.DrawLineBetween(origin, ExactPosition, this.TrailComp.Trail.MeshAt(this.Rotation), FadedMaterialPool.FadedVersionOf(this.TrailComp.Trail.MatSingle, this.Opacity), 1.3f);
            }
            if (!this.impacted)
            {
                base.Draw();
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (this.impacted)
            {
                this.Opacity -= 1f / this.TrailComp.Props.postImpactLifetime;
            }
            if (this.Opacity <= 0f || this.launcher.Destroyed)
            {
                Destroy();
            }
        }

        public float Opacity = 1f;

        protected override void Impact(Thing hitThing)
        {
            if (!this.impacted)
            {
                this.impacted = true;
                if (this.def.projectile.damageDef.isExplosive)
                {
                    Map map = base.Map;
                    Destroy();
                    if (base.def.projectile.explosionEffect != null)
                    {
                        Effecter effecter = base.def.projectile.explosionEffect.Spawn();
                        effecter.Trigger(new TargetInfo(base.Position, map), new TargetInfo(base.Position, map));
                        effecter.Cleanup();
                    }
                    GenExplosion.DoExplosion(base.Position, map, base.def.projectile.explosionRadius, base.def.projectile.damageDef, base.launcher, base.DamageAmount, base.ArmorPenetration, base.def.projectile.soundExplode, base.equipmentDef, base.def, intendedTarget.Thing, base.def.projectile.postExplosionSpawnThingDef, base.def.projectile.postExplosionSpawnChance, base.def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: base.def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: base.def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: base.def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: base.def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: base.def.projectile.explosionChanceToStartFire, damageFalloff: base.def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));
                }
                else
                {
                    base.Impact(hitThing);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.impacted, "impacted");
        }
    }

    public class Comp_ProjectileTrail : ThingComp
    {
        public CompProperties_ProjectileTrail Props => this.props as CompProperties_ProjectileTrail;

        public Projectile Projectile => this.parent as Projectile;

        private Graphic _trail;
        public Graphic Trail
        {
            get
            {
                if (this._trail == null)
                {
                    this._trail = GraphicDatabase.Get<Graphic_Single>(Props.trailTexPath, ShaderDatabase.MoteGlow);
                }
                return _trail;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
        }
    }

    public class CompProperties_ProjectileTrail : CompProperties
    {
        public string trailTexPath;
        public int postImpactLifetime = 10;

        public CompProperties_ProjectileTrail()
        {
            this.compClass = typeof(Comp_ProjectileTrail);
        }
    }

}
