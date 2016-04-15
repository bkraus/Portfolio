using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Weather
{
    public class DayForecast
    {
        public DateTime Day { get; set; }
        public string Forecast { get; set; }
        public decimal precipitation { get; set; }
        public string WindDirection { get; set; }
        public string WindSpeed { get; set; }
        public Temperature Temperature { get; set; }
        public decimal Pressure { get; set; }
        public decimal Humidity { get; set; }
        public string Clouds { get; set; }
    }
}