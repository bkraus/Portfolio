using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class PortfolioData
    {
        public decimal MarketValue = 0;
        public decimal DaysChange = 0;
        public decimal untrackedValue = 0;
        public int Digits = 2;
        public List<PortfolioStocks> Stocks = null;
        public string LowestStock { get; set; }
        public decimal LowestPct { get; set; }
        public int stocksabove = 0;
        public int stocksbelow = 0;
        private DateTime eDate;
        public decimal DayPct
        {
            get
            {
                if (MarketValue - DaysChange == 0)
                    return 0;
                return Util.GetPct((DaysChange / (MarketValue - DaysChange)));
            }
        }

        public List<PortfolioStocks> load(List<Quotes> prices, PortfolioInvestments tr, bool isOwned, DateTime spDate)
        {
            eDate = DateTime.Today;
            decimal lowerpct = 9999;
            List<PortfolioStocks> portfolio = new List<PortfolioStocks>();
            List<StocksOwned> stocks = isOwned ? tr.Owned : tr.Sold;
            foreach (StocksOwned so in stocks)
            {
                Quotes p = prices.FirstOrDefault(m => m.Symbol == so.Symbol);
                if (p == null)
                {
                    p = new Quotes();
                    p.Last = 0;
                    p.Open = 0;
                }
                PortfolioStocks f = new PortfolioStocks();
                f.Symbol = so.Symbol;
                f.CurrPrice = Util.RoundDecimal(p.Last, 4);
                f.PriceChange = Util.RoundDecimal(p.Last - p.Open, 4);
                f.shares = so.Qty;
                f.Dividends = so.dividends;
                f.Cost = Util.RoundDecimal(so.Cost, Digits);
                f.MarketValue = Util.RoundDecimal(p.Last * so.Qty, Digits);
                MarketValue += p.Last * so.Qty;
                f.DaysChange = Util.RoundDecimal(f.PriceChange * so.Qty, Digits);
                DateTime trandate = DateTime.Today.AddDays(-1);
                DateTime.TryParse(p.LastTranDate, out trandate);
                if (trandate >= DateTime.Today)
                {
                    DaysChange += f.DaysChange;
                    if (f.DaysChange < 0)
                        stocksbelow++;
                    else
                        stocksabove++;
                }
                else
                    untrackedValue += f.MarketValue;
                f.DaysChangePct = p.Open == 0 ? 0 : Util.GetPct((p.Last - p.Open) / p.Open);
                if (f.DaysChangePct < lowerpct && trandate >= DateTime.Today)
                    lowerpct = f.DaysChangePct;
                f.PricePaid = so.Price;
                f.Gain = Util.RoundDecimal(f.MarketValue - f.Cost, Digits);
                f.GainPct = f.Cost == 0 ? 0 : Util.GetPct((f.MarketValue - f.Cost) / f.Cost);
                f.BuyDate = so.BuyDate;
                if (f.BuyDate < eDate)
                    eDate = f.BuyDate;
                portfolio.Add(f);
            }
            portfolio.Sort();

            Quotes pr = prices.FirstOrDefault(m => m.Symbol == "^GSPC");
            foreach (PortfolioStocks p in portfolio)
            {
                p.AgainstSP = GetAgainstSP(p, pr.Last);
                p.SPYPct = getSPYPct(p, spDate, pr.Last);

                if (p.DaysChangePct == lowerpct)
                {
                    LowestPct = p.DaysChangePct;
                    LowestStock = p.Symbol;
                }
                p.PortfolioPct = MarketValue == 0 ? 0 : Util.GetPct(p.MarketValue / MarketValue);
            }
            Stocks = portfolio;
            return portfolio;
        }
        private decimal getSPYPct(PortfolioStocks p, DateTime spDate, decimal splast)
        {
            HistoricalData h = HistoricalData.RetrieveOne("^GSPC",spDate);
            HistoricalData s = HistoricalData.RetrieveOne(p.Symbol, spDate);
            if (h == null || s == null || h.Open == 0 || s.Open == 0)
                return 0;
            decimal spy = (splast - (decimal)h.Close) / (decimal)h.Close;
            decimal stk = (p.CurrPrice - (decimal)s.AdjClose) / (decimal)s.AdjClose;
            return Util.GetPct(stk - spy);


        }

        public static decimal GetAgainstSP(PortfolioStocks p, decimal spLast)
        {
            if (p.Cost == 0)
                return 0;

            HistoricalData h = HistoricalData.RetrieveOne("^GSPC", p.BuyDate);
            if (h == null || h.Open == 0 || p.Cost == 0)
                return 0;
            decimal spy = (spLast - (decimal)h.Open) / (decimal)h.Open;
            decimal stk = (p.MarketValue + p.Dividends - p.Cost) / p.Cost;
            return Util.GetPct(stk - spy);
        }
    }
}
