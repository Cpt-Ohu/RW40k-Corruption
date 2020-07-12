using Corruption.Core.Gods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Corruption.Core.Soul
{
    public class FavourProgress : IExposable
    {
        public static FloatRange ProgressRange = new FloatRange(0f, 10000f);

        public GodDef God;

        public FavourProgress()
        {
            this.God = GodDefOf.Emperor;
        }

        public FavourProgress(GodDef god)
        {
            this.God = god;
        }

        public FavourProgress(GodDef god, float value)
        {
            this.God = god;
            this.favourValue = value;
        }

        private float favourValue;

        public float Favour
        {
            get { return favourValue; }
            set { favourValue = value; }
        }

        public float FavourPercentage => this.favourValue / ProgressRange.max;

        public GodsFavourLevel FavourLevel
        {
            get
            {
                if (this.favourValue >= ProgressRange.max * 0.95f)
                {
                    return GodsFavourLevel.Blessed;
                }
                else if (this.favourValue >= ProgressRange.max * 0.8f)
                {
                    return GodsFavourLevel.Favoured;
                }
                else if (this.favourValue >= ProgressRange.max * 0.4f)
                {
                    return GodsFavourLevel.Acknowledged;
                }
                else if (this.favourValue >= ProgressRange.max * 0.05f)
                {
                    return GodsFavourLevel.Noticed;
                }
                else
                {
                    return GodsFavourLevel.Unknown;
                }
            }
        }

        public GodsFavourLevel NextLevel
        {
            get
            {
                return this.FavourLevel == GodsFavourLevel.Blessed ? GodsFavourLevel.Blessed : (GodsFavourLevel)(this.FavourLevel + 1);
            }
        }

        public void ExposeData()
        {
            Scribe_Defs.Look<GodDef>(ref this.God, "God");
            Scribe_Values.Look<float>(ref this.favourValue, "value", 0f);
        }

        public bool TryAddProgress(float change)
        {
            this.favourValue = ProgressRange.ClampToRange(favourValue + change);
            return true;
        }

        public static Dictionary<GodsFavourLevel, float> FavorLevelThresholds = new Dictionary<GodsFavourLevel, float>()
        {
            {GodsFavourLevel.Unknown, 0f },
            {GodsFavourLevel.Noticed, 0.05f },
            {GodsFavourLevel.Acknowledged, 0.4f },
            {GodsFavourLevel.Favoured, 0.8f },
            {GodsFavourLevel.Blessed, 0.95f }
        };
    }
}
