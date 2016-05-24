using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class PortfolioSummary
    {
        public List<PortfolioFunds> Funds { get; set; }
        public decimal YTD { get; set; }
        public decimal CashValue { get; set; }
        public decimal SPCashValue { get; set; }
        public decimal DiffCashValue { get; set; }
        public decimal MainFundChange { get; set; }
        public decimal MainFundPct { get; set; }
        public decimal IndicesFundChange { get; set; }
        public decimal IndicesFundPct { get; set; }
        public decimal FundDifference { get; set; }
        public decimal TodayDifference { get; set; }
        public string LowestStock { get; set; }
        public decimal LowestPct { get; set; }
        public decimal StocksAbove { get; set; }
        public decimal StocksBelow { get; set; }
        public DateTime LastUpdate { get; set; }
        public decimal? SPFutures { get; set; }
        public decimal? NQFutures { get; set; }
    }
}