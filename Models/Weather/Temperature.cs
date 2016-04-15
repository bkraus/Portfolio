using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Weather
{
    public class Temperature
    {
        public decimal day { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Night { get; set; }
        public decimal Eve { get; set; }
        public decimal Morn { get; set; }
    }
}