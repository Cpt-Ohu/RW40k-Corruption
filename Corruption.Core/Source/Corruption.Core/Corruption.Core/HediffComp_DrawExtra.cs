using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_DrawPawnExtra : HediffComp
    {
        public HediffCompProperties_DrawPawnExtra Props => this.props as HediffCompProperties_DrawPawnExtra;

        public virtual Color ColorOne => Color.white;
        public virtual Color ColorTwo => Color.white;
        private string graphicPath;

        private Apparel _fakeApparel;
        public Apparel FakeApparel
        {
            get
            {
                if (this._fakeApparel == null)
                {
                    this._fakeApparel = (Apparel)ThingMaker.MakeThing(this.Props.templateApparelDef);
                }
                return _fakeApparel;
            }
        }

        private Graphic _graphic;
        public Graphic Graphic
        {
            get
            {
                if (this._graphic == null)
                {

                    var graphicData = this.Props.graphicData ?? this.FakeApparel.def.graphicData;
                    var path = this.GetTexPath();
                    this._graphic = GraphicDatabase.Get(graphicData.graphicClass, path, graphicData.shaderType.Shader, graphicData.drawSize, this.ColorOne, this.ColorTwo);
                }
                return _graphic;
            }
        }

        private string GetTexPath()
        {
            if (this.graphicPath == null)
            {
                string path = this.Props.texPath;

                if (this.Props.graphicData != null)
                {
                    if (this.Props.isRandomMultiGraphic)
                    {
                        List<Texture2D> list = (from x in ContentFinder<Texture2D>.GetAllInFolder(this.Props.texPath)
                                                where !x.name.EndsWith(Graphic_Single.MaskSuffix)
                                                orderby x.name
                                                select x).ToList();
                        if (list.Count > 0)
                        {
                            var randomName = list.RandomElement().name;
                            path = path + "/" + randomName + "/" + randomName;
                        }
                    }
                    this._graphic = GraphicDatabase.Get(this.Props.graphicData.graphicClass, path, this.Props.graphicData.shaderType.Shader, this.Props.graphicData.drawSize, this.ColorOne, this.ColorTwo);
                    //path = this._graphic.path;
                }
                if (this.Props.useBodyTypes)
                {
                    path = string.Join("_", path, this.Pawn.story.bodyType.defName);
                }
                else if (this.Props.useCrownTypes)
                {
                    path = string.Join("_", path, this.Pawn.story.crownType.ToString());
                }
                this.graphicPath = path;
                return path;
            }
            return this.graphicPath;
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (this.Pawn.RaceProps.Humanlike)
            {
                this.Pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<string>(ref this.graphicPath, "graphicPath");
        }
    }

    public class HediffCompProperties_DrawPawnExtra : HediffCompProperties
    {
        public string texPath;

        public GraphicData graphicData;

        public bool useBodyTypes = false;

        public bool useCrownTypes = false;

        public bool keepHair = true;

        public bool isRandomMultiGraphic;

        public ThingDef templateApparelDef;

        public float minSeverity = -1f;

        public HediffCompProperties_DrawPawnExtra()
        {
            this.compClass = typeof(HediffComp_DrawPawnExtra);
        }
    }
}
