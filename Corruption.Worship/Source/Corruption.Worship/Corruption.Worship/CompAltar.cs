﻿using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Worship
{
    public class CompAltar : ThingComp, IThingHolder, IOpenable
    {


        public CompProperties_Altar CompProps
        {
            get
            {
                return this.props as CompProperties_Altar;
            }
        }

        public Thing InstalledEffigy
        {
            get
            {
                return (this.innerContainer.Count != 0) ? this.innerContainer[0] : null;
            }
        }

        public bool HasEffigy
        {
            get
            {
                return this.InstalledEffigy != null && this.CompProps.requiresEffigy == true;
            }
        }

        public Thing thingToInstall;

        protected ThingOwner innerContainer;

        public CompAltar()
        {
            this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
        }

        public GodDef God = GodDefOf.Emperor;

        public bool CanOpen
        {
            get
            {
                return this.InstalledEffigy != null && this.CompProps.requiresEffigy == true;
            }
        }

        public void Open()
        {
            this.DropEffigy();
        }

        public void DropEffigy()
        {
            IntVec3 c = this.parent.Position;
            if (this.parent.def.hasInteractionCell)
            {
                c = this.parent.InteractionCell;
            }
            this.innerContainer.TryDropAll(c, this.parent.Map, ThingPlaceMode.Near);
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (this.HasEffigy)
            {
                Mesh mesh = MeshPool.plane10;
                Vector3 vector = this.parent.DrawPos;
                vector.y += 1f;
                vector.z += 0.11f;
                Vector3 s = new Vector3(1.0f, 1f, 1.0f);
                Matrix4x4 matrix = default(Matrix4x4);
                Mesh drawMesh = MeshPool.plane10;
                if (this.parent.Rotation == Rot4.West)
                {
                    drawMesh = MeshPool.plane10Flip;
                }
                matrix.SetTRS(vector, Quaternion.AngleAxis(0f, Vector3.up), s);
                Graphics.DrawMesh(drawMesh, matrix, this.InstalledEffigy.Graphic.MatAt(this.parent.Rotation, null), 0);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
            {
             this
            });
        }
    }

    public class CompProperties_Altar : CompProperties
    {
        public Vector3 EffigyDrawOffset;

        public float WorshipRatePerTick;

        public bool requiresEffigy = true;

        public CompProperties_Altar()
        {
            this.compClass = typeof(CompAltar);
        }

    }
}