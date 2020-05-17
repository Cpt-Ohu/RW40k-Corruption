using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Items
{
    public class Projectile_Expanding : Projectile
    {
        private Vector3 startingLocation;
        private float growthRate => this.def.projectile.SpeedTilesPerTick;
        private float travelledDistance;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.startingLocation = this.Position.ToVector3();
        }

        public override void Tick()
        {
            base.Tick();
            this.travelledDistance += growthRate;
        }

        public override void Draw()
        {
            Vector2 drawSize = new Vector2(travelledDistance, travelledDistance);
            Graphics.DrawMesh(MeshPool.GridPlane(drawSize), startingLocation, this.ExactRotation, def.DrawMatSingle, 0);
            Comps_PostDraw();
        }
    }
}
