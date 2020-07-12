using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core
{
    public class MoteAttachedText : MoteText
    {
        public override void Tick()
        {
            base.Tick();
            this.UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (!this.link1.Equals(MoteAttachLink.Invalid))
            {
                this.link1.UpdateDrawPos();
                this.exactPosition = this.link1.LastDrawPos + new Vector3(0f, 0f, 1f) ;
            }
        }

        public override void DrawGUIOverlay()
        {
            base.DrawGUIOverlay();
        }

        public static void ThrowText(Thing thing, Vector3 loc, Map map, string text, Color color, float timeBeforeStartFadeout = -1f)
        {
            IntVec3 intVec = loc.ToIntVec3();
            if (intVec.InBounds(map))
            {
                MoteAttachedText moteText = (MoteAttachedText)ThingMaker.MakeThing(CoreThingDefOf.Mote_AttachedText);
                moteText.exactPosition = loc;
                moteText.Attach(thing);
                moteText.text = text;
                moteText.textColor = color;
                if (timeBeforeStartFadeout >= 0f)
                {
                    moteText.overrideTimeBeforeStartFadeout = timeBeforeStartFadeout;
                }
                GenSpawn.Spawn(moteText, intVec, map);
            }
        }

    }
}
