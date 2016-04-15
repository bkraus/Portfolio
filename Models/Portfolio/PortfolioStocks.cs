using Portfolio.Models.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models
{



    public class PortfolioStocks : IComparable<PortfolioStocks>
    {
        public string Symbol { get; set; }
        public decimal CurrPrice { get; set; }
        public decimal PriceChange { get; set; }
        public decimal shares { get; set; }
        public decimal Cost { get; set; }
        public decimal MarketValue { get; set; }
        public decimal DaysChange { get; set; }
        public decimal DaysChangePct { get; set; }
        public decimal PricePaid { get; set; }
        public decimal Gain { get; set; }
        public decimal GainPct { get; set; }
        public decimal PortfolioPct { get; set; }
        public DateTime BuyDate { get; set; }
        public decimal Dividends { get; set; }
        public decimal AgainstSP { get; set; }
        public decimal SPYPct { get; set; }
        public int CompareTo(PortfolioStocks port)
        {
            if (this.MarketValue < port.MarketValue)
                return 1;
            else if (this.MarketValue > port.MarketValue)
                return -1;
            else
                return 0;

        }

    }


}