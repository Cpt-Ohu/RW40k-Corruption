using Corruption.Core.Gods;
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
    public class ScenPart_StartingReligion : ScenPart
    {
        public override void PostWorldGenerate()
        {
            base.PostWorldGenerate();
            GlobalWorshipTracker.Current.PlayerPantheon = this.pantheon;
        }

        private PantheonDef pantheon;

        public override void DoEditInterface(Listing_ScenEdit listing)
		{
			Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 3f + 31f);
			if (Widgets.ButtonText(scenPartRect.TopPartPixels(ScenPart.RowHeight), pantheon.LabelCap))
			{
				FloatMenuUtility.MakeMenu(PossiblePantheons(), (PantheonDef pantheon) => pantheon.LabelCap, delegate (PantheonDef pantheon)
				{
					ScenPart_StartingReligion scenPart = this;
					return delegate
					{
						scenPart.pantheon = pantheon;
					};
				});
			}

		}

		private IEnumerable<PantheonDef> PossiblePantheons()
		{
			foreach (var pantheon in DefDatabase<PantheonDef>.AllDefs)
			{
				yield return pantheon;
			}
		}
	}
}
