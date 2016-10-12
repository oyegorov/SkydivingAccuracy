using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.BusinessLogic
{
    public class AggregatedWindsAloftRetriever : IWindsAloftRetriever
    {
        public List<WindsAloft> GetWindsAloft()
        {
            IWindsAloftRetriever[] windsAloftRetrievers = new IWindsAloftRetriever[] { new NavCanadaWindsAloftRetriever(), new NoaaWindsAloftRetriever() };

            List<WindsAloft> result = new List<WindsAloft>();
            foreach (var windsAloftRetriever in windsAloftRetrievers)
            {
                result.AddRange(windsAloftRetriever.GetWindsAloft());
            }

            return result;
        }
    }
}
