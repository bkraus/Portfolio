using Portfolio.Models.Preference;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class PortfolioFunds : IComparable<PortfolioFunds>
    {
        public string Name { get; set; }
        public int PortID { get; set; }
        public int displayorder { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public decimal Cash { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public decimal Value { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePct { get; set; }
        public decimal GrowthPct { get; set; }
        public decimal YearChange { get; set; }
        public decimal YearPct { get; set; }
        public int PortfolioType { get; set; }
        public DateTime StartDate { get; set; }
        public PortfolioData portfolioData { get; set; }
        public decimal SPValue { get; set; }
        public decimal vsSP { get; set; }
        public int CompareTo(PortfolioFunds port)
        {
            if (this.displayorder < port.displayorder)
                return -1;
            else if (this.displayorder > port.displayorder)
                return 1;
            else
                return 0;

        }

        public List<PortfolioFunds> Retrieve()
        {
            List<PortFolios> portfolio = PortFolios.Retrieve();
            List<PortfolioInvestments> Investments = new List<PortfolioInvestments>();
            foreach (PortFolios folio in portfolio)
            {
                PortfolioInvestments invest = new PortfolioInvestments();
                Investments.Add(invest.Retrieve(folio.PortfolioID));

            }
            List<string> s = new List<string>();
            string stocks = "";
            for (int ii = 0; ii < Investments.Count; ii++)
            {
                s.AddRange(Investments[ii].OwnedList);
                s.AddRange(Investments[ii].SoldList);
            }
            s.Sort();
            s = s.Distinct().ToList();
            stocks = string.Join("+", s);

            List<Quotes> quotes = Quotes.GetPrices(stocks);

            List<PortfolioFunds> pslist = new List<PortfolioFunds>();
            DateTime SPYDate = Preferences.GetDate("SPYDate");
            foreach (PortFolios pf in portfolio)
            {
                PortfolioFunds ps = new PortfolioFunds();
               portfolioData = new PortfolioData();
                PortfolioInvestments piRec = Investments.First(p => p.PortfolioID == pf.PortfolioID);
                //    IfolioHistory h = cd.fhistory.FirstOrDefault(t => t.date == cd.pref.SPYDate);
                //    cd.pref.yrValue.Add(h.Port[ii].Value - tr[ii].ps.GetSeedCashOnDate(cd.pref.SPYDate) + tr[ii].ps.SeedCash);
                portfolioData.load(quotes,piRec,!pf.Name.Equals("Sold"),Preferences.GetDate("SPYDate"));
                ps.portfolioData = portfolioData;
                ps.StartDate = piRec.StartDate;
                ps.Name = pf.Name;
                ps.PortfolioType = pf.Type;
                ps.displayorder = pf.PortfolioID;
                ps.Value = Util.RoundDecimal(portfolioData.MarketValue + piRec.Cash, 2);
                ps.DayChange = Util.RoundDecimal(portfolioData.DaysChange, 2);
                ps.Cash = Util.RoundDecimal(piRec.Cash, 2);
                ps.DayChangePct = Util.RoundDecimal(portfolioData.DayPct, 2);
                ps.GrowthPct = piRec.SeedCash == 0 ? 0 : Util.GetPct((ps.Value - piRec.SeedCash) / piRec.SeedCash);
                // ps.GrowthPct = pref.StartValue[ii] == 0 ? 0 : Util.GetPct((ps.Value - pref.StartValue[ii]) / pref.StartValue[ii]);
                PortfolioHistories hist = PortfolioHistories.Retrieve(pf.PortfolioID, SPYDate);
                decimal yrValue = hist.Value - piRec.GetSeedCashOnDate(SPYDate) + piRec.SeedCash;
                ps.YearChange = Util.RoundDecimal(ps.Value - yrValue, 2);
                ps.YearPct = yrValue == 0 ? 0 : yrValue < 0 ? 0 : Util.GetPct((ps.Value - yrValue) / yrValue);
                ps.SPValue = GetTotalValue(quotes,piRec.Deposits);
                ps.vsSP = ps.Value - ps.SPValue;
                pslist.Add(ps);
            }
            pslist.Sort();

            return pslist;
        }
        private decimal GetTotalValue(List<Quotes> quotes, List<CashDeposits> deposits)
        {
            decimal sum = 0;
            List<HistoricalData> hList = HistoricalData.Retrieve("^GSPC");
            foreach (CashDeposits dep in deposits)
                {
                    HistoricalData h = hList.Where(w => w.Date == dep.date).FirstOrDefault();
                    if (dep.Type.Equals("SHARESIN") || dep.Type.Equals("CASHIN"))
                    {
                        if (h != null)
                            sum += dep.Cash / (decimal)h.Open;
                        else
                            sum += 1;
                    }
                }
            Quotes p = quotes.FirstOrDefault(m => m.Symbol == "^GSPC");
            if (p == null)
            {
                p = new Quotes();
                p.Last = 0;
                p.Open = 0;
            }
            sum = sum * p.Last;
            return sum;
        }
    }
}