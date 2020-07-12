using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Soul
{
    public class SoulAfflictionDef : Def
    {
        public float Threshold = -1f;

        public string iconPath;

        public Texture2D uiIcon;

        public override void PostLoad()
        {
            base.PostLoad();

            if (!string.IsNullOrEmpty(iconPath))
            {
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    uiIcon = ContentFinder<Texture2D>.Get(iconPath);
                });
            }
        }
    }

    [DefOf]
    public static class SoulAfflictionDefOf
    {
        public static SoulAfflictionDef Pure;
        public static SoulAfflictionDef Intrigued;
        public static SoulAfflictionDef Warptouched;
        public static SoulAfflictionDef Tainted;
        public static SoulAfflictionDef Corrupted;
        public static SoulAfflictionDef Lost;
    }
}
