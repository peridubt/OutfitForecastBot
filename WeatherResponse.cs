using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutfitForecastBot
{
    public class WeatherResponse
    {
        public TemperatureInfo Main { get; set; }

        public string Name { get; set; }
    }
}
