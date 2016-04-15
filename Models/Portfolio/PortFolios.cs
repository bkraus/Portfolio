using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class PortFolios
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int PortfolioID { get; set; }
        public bool MyStocks { get; set; }
        public bool IsEditable { get; set; }
        public int Type { get; set; }
        public int SortPosition { get; set; }
        public static List<PortFolios> Retrieve()
        {
            var context = new PortfolioContext();
            return context.PortFolios.ToList();
        }
    }
}