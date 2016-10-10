using System.Collections.Generic;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.BusinessLogic
{
    public interface IWindsAloftRetriever
    {
        List<WindsAloft> GetWindsAloft();
    }
}
