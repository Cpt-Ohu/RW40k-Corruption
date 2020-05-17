using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Gods
{
    public class PantheonAttributeDef : Def
    {
        public string iconPath;

        public Texture2D Icon = BaseContent.BadTex;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.Icon = ContentFinder<Texture2D>.Get(this.iconPath);
            });
        }
    }
}
