using Portfolio.Models;
using Portfolio.Models.Portfolio;
using Portfolio.Models.Preference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Portfolio.Controllers
{
    public class PortfolioController : Controller
    {
        //
        // GET: /Portfolio/

        public ActionResult OverView()
        {
            PortfolioSummary Summary = LoadPortfolio();
            return View(Summary);
        }

        [OutputCache(NoStore = true, Location =
           OutputCacheLocation.Client, Duration = 30)]
        public ActionResult Refresh()
        {
            PortfolioSummary Summary = LoadPortfolio();

            return PartialView("Portfolios", Summary);
        }

        public ActionResult Convert()
        {
            ConvertData();
            PortfolioFunds portfolioSummary = new PortfolioFunds();
            List<PortfolioFunds> pov = portfolioSummary.Retrieve();
            return View("OverView", pov);
        }
        
        public ActionResult Update()
        {
            UpdateData();
            PortfolioFunds portfolioSummary = new PortfolioFunds();
            List<PortfolioFunds> pov = portfolioSummary.Retrieve();
            return View("OverView", pov);
        }

        public ActionResult Retirement()
        {
            PortfolioFunds portfolioSummary = new PortfolioFunds();
            List<PortfolioFunds> portfolioFunds = portfolioSummary.Retrieve();
            Retirement ret = new Retirement();
            ret.LoadData(portfolioFunds);
            return View(ret);
        }
        public ActionResult RetireUpdate(Retirement retirement)
        {
            Preferences.Update("Bank","TD Checking", retirement.Bank1);
            Preferences.Update("Bank","TD Money Market", retirement.Bank2);
            Preferences.Update("Reed", retirement.Reed);
            Preferences.Update("Bank", "Other", retirement.OtherIncome);
            Preferences.Update("Bank", "Raymond James", retirement.OtherIncome2);
            Preferences.Update("SSABenefit", retirement.SSABenefit);
            Preferences.Update("FutureRetire", retirement.FutureRetire);
            Preferences.Update("FutureSavings", retirement.FutureSavings);
            Preferences.Update("BankInterest", retirement.BankInterest);
            Preferences.Update("Interest", retirement.Interest);
            
            PortfolioFunds portfolioSummary = new PortfolioFunds();
            List<PortfolioFunds> portfolioFunds = portfolioSummary.Retrieve();
            Retirement ret = new Retirement();
            ret.LoadData(portfolioFunds);
            return View("Retirement",ret);
        }

        private PortfolioSummary LoadPortfolio()
        {
            PortfolioFunds portfolioFunds = new PortfolioFunds();
            List<PortfolioFunds> po = portfolioFunds.Retrieve();
            PortfolioSummary Summary = new PortfolioSummary();
            Summary.Funds = po;
            Summary.LastUpdate = DateTime.Now;
            Summary.CashValue = 0;
            decimal fundDif = 0;
            Summary.SPCashValue = 0;
            foreach (PortfolioFunds pf in po)
            {
                if (pf.PortfolioType == 1)
                {
                    Summary.SPCashValue += pf.SPValue;
                    Summary.YTD += pf.YearChange;
                }
                if (pf.Name == "TDAmeritrade")
                {
                    Summary.MainFundChange = pf.DayChange;
                    Summary.MainFundPct = pf.DayChangePct;
                    fundDif += pf.Value;
                    Summary.LowestPct = pf.portfolioData.LowestPct;
                    Summary.LowestStock = pf.portfolioData.LowestStock;
                    Summary.StocksAbove = pf.portfolioData.stocksabove;
                    Summary.StocksBelow = pf.portfolioData.stocksbelow;
                }
                if (pf.Name == "Indices")
                {
                    Summary.IndicesFundChange = pf.DayChange;
                    Summary.IndicesFundPct = pf.DayChangePct;
                    fundDif -= pf.Value;
                }
                if (pf.PortfolioType == 1)
                    Summary.CashValue += pf.Value;
            }
            Summary.DiffCashValue = Summary.CashValue - Summary.SPCashValue;
            Summary.TodayDifference = Summary.MainFundChange - Summary.IndicesFundChange;
            Summary.FundDifference = fundDif;
            if (DateTime.Now.TimeOfDay < new TimeSpan(9, 30, 0))
            {
                List<Quotes> SPQuotes = Quotes.GetPrices("ESM16.CME");
                List<Quotes> NQQuotes = Quotes.GetPrices("NQM16.CME");
                Summary.SPFutures = Util.RoundDecimal((SPQuotes[0].Last - SPQuotes[0].Open) * 100 / SPQuotes[0].Open,2);
                Summary.NQFutures = Util.RoundDecimal((NQQuotes[0].Last - NQQuotes[0].Open) * 100 / NQQuotes[0].Open,2);
            }
            return Summary;
        }
        private void UpdateData()
        {
            Util.UpdateDBTransactions();
        }

        private void ConvertData()
        {
            var context = new PortfolioContext();
            context.EmptyAllTables();
            Options opt = new Options();
            opt.Load();
            PPreferences pref = opt.pref;
            int trancount = pref.portfolios.Count;
            List<Transactions> tr = new List<Transactions>();

            foreach (PrefOpts po in pref.portfolios)
            {
                string xx = po.Name;
                if (xx.Equals("Watch"))
                    tr.Add(new Transactions(xx, pref.Watches));
                else
                    tr.Add(new Transactions(xx));
            }
            for (int ii = 0; ii < trancount; ii++)
            {
                tr[ii].LoadData();
                if (tr[ii].Records != null && tr[ii].Records.Count > 0)
                {
                    foreach (TransRec rec in tr[ii].Records)
                    {
                        TransactionDB tdb = new TransactionDB();
                        tdb.Amount = rec.amount;
                        tdb.Commission = rec.Commission;
                        tdb.Date = rec.Date;
                        tdb.Desc = rec.Desc;
                        tdb.IsMyAdjustment = rec.IsMyAjustment;
                        tdb.MyAdjustment = rec.MyAdjustment;
                        tdb.Price = rec.Price;
                        tdb.Qty = rec.Qty;
                        tdb.Symbol = rec.Symbol;
                        tdb.TradeID = rec.TradeID.ToString();
                        tdb.TranId = rec.TranId.ToString();
                        tdb.Type = rec.Type;
                        tdb.Portfolio = ii + 1;
                        context.Transaction.Add(tdb);
                    }
                }
            }
            int pID = 0;
            foreach (PrefOpts po in pref.portfolios)
            {
                pID++;
                PortFolios p = new PortFolios();
                p.IsEditable = po.IsEditable;
                p.MyStocks = po.MyStocks;
                p.Name = po.Name;
                p.PortfolioID = pID;
                p.SortPosition = (int)po.Id;
                if (po.Id == 1 || po.Id == 3 || po.Id == 4)
                    p.Type = 1;
                else
                    p.Type = 2;
                context.PortFolios.Add(p);
            }


            context.Options.Add(GetPref("Bank", pref.Bank1.ToString(), "TD Checking", "Portfolio"));
            context.Options.Add(GetPref("Bank", pref.Bank2.ToString(), "TD Money Market", "Portfolio"));
            context.Options.Add(GetPref("Bank", pref.OtherIncome.ToString(), "Raymond James", "Portfolio"));
            context.Options.Add(GetPref("Bank", pref.OtherIncome2.ToString(), "Other", "Portfolio"));
            context.Options.Add(GetPref("SPYDate", pref.SPYDate.ToString(), "SPYDate", "Portfolio"));
            context.Options.Add(GetPref("StartDate", pref.StartDate.ToString(), "StartDate", "Portfolio"));
            context.Options.Add(GetPref("Interest", pref.Interest.ToString(), "Interest", "Portfolio"));
            context.Options.Add(GetPref("BankInterest", pref.BankInterest.ToString(), "BankInterest", "Portfolio"));
            context.Options.Add(GetPref("FutureRetire", pref.FutureRetire.ToString(), "FutureRetire", "Portfolio"));
            context.Options.Add(GetPref("FutureSavings", pref.BankInterest.ToString(), "FutureSavings", "Portfolio"));
            context.Options.Add(GetPref("EndDate", pref.EndDate.ToString(), "EndDate", "Portfolio"));
            context.Options.Add(GetPref("RetireDate", pref.RetireDate.ToString(), "RetireDate", "Portfolio"));
            context.Options.Add(GetPref("SSABenefit", pref.SSA[0].benefit.ToString(), "SSABenefit", "Retirement"));
            context.Options.Add(GetPref("Reed", pref.Reed.ToString(), "Reed", "Retirement"));
            context.Options.Add(GetPref("LastUpdateDate", DateTime.Now.ToString(), "LastUpdateDate", "Portfolio"));
            context.Options.Add(GetPref("MonthlyEstimate", pref.MonthlyEstimate.ToString(), "MonthlyEstimate", "Retirement"));
            context.Options.Add(GetPref("MusicPath", "c:\\MusicBackup\\wmpMetadata.xml","MusicPath","Music"));



            context.SaveChanges();
        }

        private Preferences GetPref(string Pref, string Value, string Desc, string type)
        {
            Preferences option = new Preferences();
            option.Preference = Pref;
            option.Description = Desc;
            option.Type = type;
            option.Value = Value;
            return option;
        }



    }



}
