using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherTarczykowski
{
    public class CurrentWeather
    {
        public string CityName { get; set; }
        public string Temperature { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string Humidity { get; set; }
        public string Pressure { get; set; }
        public string WindSpeedValue { get; set; }
        public string WindSpeedName { get; set; }
        public string WindDirectionCode { get; set; }
        public string WindDirectionName { get; set; }
        public string CloudsName { get; set; }
    }
}
