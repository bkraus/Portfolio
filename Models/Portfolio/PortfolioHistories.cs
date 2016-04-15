using Portfolio.Models.Preference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class PortfolioHistories
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public decimal SPClose { get; set; }
        public int PortID { get; set; }
        public decimal Cash { get; set; }
        public decimal Value { get; set; }
        public decimal SeedCash { get; set; }
        public decimal SPValue { get; set; }    
        private List<PortfolioHistories> phList= null;
        List<PortFolios> portfolios = new List<PortFolios>();
        List<HistoricalData> historicalData = new List<HistoricalData>();
        public static PortfolioHistories Retrieve(int PortId, DateTime dte)
        {
            var Context = new PortfolioContext();
            return  Context.History.Where(p => p.PortID ==PortId && p.Date == dte).FirstOrDefault();

        }

        public List<PortfolioHistories> UpdateHistory()
        {
            var Context = new PortfolioContext();
            phList = Context.History.OrderBy(p=>p.ID).ToList();
            portfolios = Context.PortFolios.ToList();
            int rec = phList.Count - 1;
            Calulatehistory();
            return phList;
        }
        private void Calulatehistory()
        {
            DateTime currDate = Preferences.GetDate("StartDate");
            if (phList.Count >0)
                currDate = phList[phList.Count-1].Date;
            if (currDate.Date >= DateTime.Now.Date.AddDays(-1))
                return;
            var Context = new PortfolioContext();
            DateTime LastDate = currDate.AddDays(200) < DateTime.Now ? currDate.AddDays(200) : DateTime.Now;
            foreach (PortFolios pf in portfolios)
            {
                int sptran = 0;
                DateTime phDate = currDate;
                List<TransactionDB> Trans = TransactionDB.PortfolioTransactions(pf.PortfolioID, DateTime.Now);
                PortfolioInvestments pi = new PortfolioInvestments();
                while (phDate < LastDate)
                {
                    HistoricalData qq = GetHistoryData("^GSPC", phDate);
                    if (qq != null)
                    {
                        if (Trans.Count > 0)
                        {
                            while (sptran < Trans.Count() && Trans[sptran].Date <= phDate)
                            {
                                pi.AddTransaction(Trans[sptran]);
                                sptran++;
                            }
                            pi.CleanupOwned();
                        }
                        decimal mv = pi.Cash;
                        foreach (StocksOwned o in pi.Owned)
                        {
                            string stock = o.Symbol;
                            HistoricalData q = GetHistoryData(stock, phDate);
                            if (q != null)
                                mv += (decimal)q.Close * o.Qty;
                        }
                        PortfolioHistories ps = new PortfolioHistories();
                        ps.SeedCash = pi.SeedCash;
                        ps.Cash = pi.Cash;
                        ps.SPValue = (decimal)qq.Close;
                        ps.Value = mv;
                        ps.PortID = pf.PortfolioID;
                        ps.SPClose = qq.Close;
                        ps.Date = phDate;
                        phList.Add(ps);
                        Context.History.Add(ps);
                    }
                    phDate = phDate.AddDays(1);
                }
            }
            Context.SaveChanges();
        }
        private HistoricalData GetHistoryData(string ticker, DateTime Date)
        {
            HistoricalData h = historicalData.Where(p => p.Symbol == ticker && p.Date.Date == Date.Date).FirstOrDefault();
            if (h != null)
                return h;
            historicalData.AddRange( HistoricalData.RetrieveAll(ticker));
            h = historicalData.Where(p => p.Symbol == ticker && p.Date.Date == Date.Date).FirstOrDefault();
            return h;
        }

    }

}