using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core
{
    public class HediffComp_AffectSkin : HediffComp
    {
        public HediffCompProperties_AffectSkin Props => this.props as HediffCompProperties_AffectSkin;
    }

    public class HediffCompProperties_AffectSkin : HediffCompProperties
    {
        public bool useSkinColor;

        public Color skinColor;

        public string headPath;

        public string bodyPath;

        public float minSeverity = -1f;

        public HediffCompProperties_AffectSkin()
        {
            this.compClass = typeof(HediffComp_AffectSkin);
        }
    }
}
