using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.BusinessLogic
{
    public static class FdUtils
    {
        public static WindsAloftRecord DecodeFdInfo(int altitude, string fd)
        {
            WindsAloftRecord windsAloftRecord = new WindsAloftRecord();

            windsAloftRecord.Altitude = altitude;

            if (String.IsNullOrWhiteSpace(fd))
                return windsAloftRecord;

            if (fd.Length > 4)
            {
                string temperatureInfo = fd.Substring(4);
                if (!String.IsNullOrWhiteSpace(temperatureInfo))
                    windsAloftRecord.Temperature = Int32.Parse(temperatureInfo);
            }

            fd = fd.Substring(0, 4);

            if (fd == "9900")
                return windsAloftRecord;

            int fdAngle = Int32.Parse(fd.Substring(0, 2));
            int fdKnots = Int32.Parse(fd.Substring(2, 2));

            if (fdAngle > 36)
            {
                windsAloftRecord.WindHeading = fdAngle - 50;
                windsAloftRecord.WindSpeed = 100 + fdKnots;
            }
            else
            {
                windsAloftRecord.WindHeading = fdAngle * 10;
                windsAloftRecord.WindSpeed = fdKnots;
            }

            return windsAloftRecord;
        }
    }
}
