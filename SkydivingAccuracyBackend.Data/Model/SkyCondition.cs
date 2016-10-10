using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkydivingAccuracyBackend.Data.Model
{
    public class SkyCondition
    {
        public SkyCondition()
        {
        }

        public SkyCondition(string altitude, string skyCoverData)
        {
            Altitude = altitude == null ? null : (int?)Int32.Parse(altitude);

            switch (skyCoverData)
            {
                case "OVC":
                    SkyCover = SkyCover.Overcast;
                    break;
                case "CLR":
                case "SKC":
                    SkyCover = SkyCover.Clear;
                    break;
                case "FEW":
                    SkyCover = SkyCover.Few;
                    break;
                case "BKN":
                    SkyCover = SkyCover.Broken;
                    break;
                case "SCT":
                    SkyCover = SkyCover.Scattered;
                    break;
                default:
                    SkyCover = SkyCover.Other;
                    break;
            }
        }

        public int? Altitude { get; set; }

        public SkyCover SkyCover { get; set; }
    }
}
