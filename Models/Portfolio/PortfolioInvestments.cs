using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class PortfolioInvestments
    {
        public int PortfolioID { get; set; }
        public decimal SeedCash {get;set;}
        public decimal Cash {get;set;}
        public List<CashDeposits> Deposits {get;set;}
        public List<TransactionDB> CashRec {get;set;}
        public List<StocksOwned> Owned {get;set;}
        public List<StocksOwned> Sold {get;set;}
        public DateTime StartDate {get;set;}
        public List<string> OwnedList
        {
            get
            {
                return (from p in Owned select p.Symbol).Distinct().ToList();
            }
        }
        public List<string> SoldList
        {
            get
            {
                return (from p in Sold select p.Symbol).Distinct().ToList();
            }
        }

        public PortfolioInvestments()
        {
            Deposits = new List<CashDeposits>();
            CashRec = new List<TransactionDB>();
            Owned = new List<StocksOwned>();
            Sold = new List<StocksOwned>();
            StartDate = new DateTime();
            StartDate = DateTime.Now;
        }
        public decimal GetSeedCashOnDate(DateTime dte)
        {
            decimal total = 0;
            if (Deposits == null || Deposits.Count == 0)
                return 0;
            foreach (CashDeposits dep in Deposits)
            {
                if (dep.date <= dte)
                    total += dep.Cash;
            }
            return total;
        }
        public PortfolioInvestments Retrieve(int PortID)
        {
            PortfolioID = PortID;
            List<TransactionDB> pt = TransactionDB.PortfolioTransactions(PortID);
            foreach (TransactionDB rec in pt)
                AddTransaction(rec);
            CleanupOwned();
            return this;
        }
        public void CleanupOwned()
        {
            this.Owned = this.Owned.OrderBy(p => p.Symbol).ThenBy(p => p.BuyDate).ToList();
            for (int ii = 1; ii < this.Owned.Count; ii++)
            {
                if (this.Owned[ii].Symbol == this.Owned[ii - 1].Symbol)
                {
                    this.Owned[ii].Qty += this.Owned[ii - 1].Qty;
                    this.Owned[ii].Cost += this.Owned[ii - 1].Cost;
                    this.Owned[ii].Commission += this.Owned[ii - 1].Commission;
                    if (this.Owned[ii].Qty > 0)
                        this.Owned[ii].dividends += this.Owned[ii - 1].dividends;
                    else
                    {
                        this.Owned[ii].dividends = 0;
                        this.Owned[ii].Cost = 0;
                    }
                    if (this.Owned[ii - 1].Qty > 0)
                    {
                        this.Owned[ii].BuyDate = this.Owned[ii - 1].BuyDate;
                        if (this.Owned[ii].Qty == 0)
                            this.Owned[ii].Price = 0;
                        else
                            this.Owned[ii].Price = Util.RoundDecimal((this.Owned[ii].Cost - this.Owned[ii].Commission) / this.Owned[ii].Qty, 4);
                    }
                    this.Owned[ii - 1].Qty = 0;
                }
            }

            this.Owned = (from p in this.Owned where p.Qty > 0 select p).ToList();
        }

        private void AddDividend(TransactionDB Rec, List<StocksOwned> own)
        {
            for (int ii = this.Owned.Count - 1; ii >= 0; ii--)
            {
                StocksOwned o = this.Owned[ii];
                if (Rec.Symbol == o.Symbol)
                {
                    o.Trans.Add(Rec);
                    o.dividends += Rec.Amount;
                    return;
                }
            }
            return;
        }

        private void AddCash(TransactionDB Rec)
        {
            if (Rec.Type.Equals("CASH") || Rec.Type.Equals("CASHIN"))
            {
                if (Rec.MyAdjustment > 0)
                {
                    CashDeposits dep = new CashDeposits();
                    dep.Cash = Rec.MyAdjustment;
                    dep.date = Rec.Date;
                    dep.Type = Rec.Type;
                    this.Deposits.Add(dep);
                    if (Rec.Type.Equals("CASHIN"))
                        this.SeedCash += Rec.MyAdjustment;
                }
                else
                {
                    CashDeposits dep = new CashDeposits();
                    dep.Cash = Rec.Amount;
                    dep.date = Rec.Date;
                    dep.Type = Rec.Type;
                    this.Deposits.Add(dep);
                    if (Rec.Type.Equals("CASHIN"))
                        this.SeedCash += Rec.Amount;
                }
            }
            else if (Rec.Type.Equals("SHARESIN"))
            {
                CashDeposits dep = new CashDeposits();
                dep.Cash = -Rec.Amount;
                dep.date = Rec.Date;
                dep.Type = Rec.Type;
                this.Deposits.Add(dep);
                this.SeedCash += -Rec.Amount;
            }
            if (Rec.MyAdjustment > 0)
                this.Cash += Rec.MyAdjustment;
            else
            {
                if (Rec.Type.Equals("SHARESIN") || Rec.Type.Equals("REINVEST"))
                    this.Cash += -Rec.Amount;
                else
                    this.Cash += Rec.Amount;
            }
            this.CashRec.Add(Rec);
        }

        public void AddTransaction(TransactionDB Rec)
        {
            if (StartDate > Rec.Date)
                StartDate = Rec.Date;
            int cnt = 0;
            if (Rec.Symbol.Equals("UA"))
                cnt++;
            if (Rec.Type.Equals("CASH") || Rec.Type.Equals("CASHIN") || Rec.Symbol.Equals("REFUND") || Rec.Type.Equals("INTEREST"))
            {
                AddCash(Rec);
            }
            else
            {
                if (Rec.Type.Equals("SHARESIN") || Rec.Type.Equals("REINVEST"))
                    AddCash(Rec);
                if (Rec.Type.Equals("SPLIT"))
                {
                    for (int ii = this.Owned.Count - 1; ii >= 0; ii--)
                    {
                        StocksOwned o = this.Owned[ii];
                        if (Rec.Symbol == o.Symbol)
                        {
                            o.Qty = o.Qty * Rec.MyAdjustment;
                            o.Price = o.Price / Rec.MyAdjustment;
                            o.Trans.Add(Rec);
                        }
                    }

                }
                else if (Rec.Amount > 0 && Rec.Qty == 0)
                {
                    AddDividend(Rec, this.Owned);
                    if (Rec.MyAdjustment > 0)
                        this.Cash += Rec.MyAdjustment;
                    else
                        this.Cash += Rec.Amount;

                }
                else if (Rec.Amount > 0)
                {
                    SoldStock(Rec);
                    decimal qty = Rec.Qty;
                    for (int ii = this.Owned.Count - 1; ii >= 0; ii--)
                    {
                        StocksOwned o = this.Owned[ii];
                        if (Rec.Symbol == o.Symbol)
                        {
                            if (o.Qty <= qty)
                            {
                                qty -= o.Qty;
                                o.Qty = 0;
                                o.Cost = 0;
                            }
                            else
                            {
                                o.Qty -= qty;
                                o.Cost -= qty * o.Price;
                                qty = 0;
                            }
                            o.Trans.Add(Rec);
                        }
                    }
                    this.Cash += Rec.Amount;
                }
                else
                {
                    if (Rec.MyAdjustment > 0)
                    {
                        Rec.Type = "SHARESIN";
                        Rec.Price = Rec.MyAdjustment;
                        Rec.Amount = -(Rec.Price * Rec.Qty) - Rec.Commission;
                    }
                    StocksOwned stock = new StocksOwned();
                    stock.Symbol = Rec.Symbol;
                    stock.Qty = Rec.Qty;
                    stock.Cost = -Rec.Amount;
                    stock.Commission = Rec.Commission;
                    stock.Price = Rec.Price;
                    stock.TranId = Convert.ToDouble(Rec.TranId);
                    stock.BuyDate = Rec.Date;
                    stock.dividends = 0;
                    stock.Type = Rec.Type;
                    stock.Trans = new List<TransactionDB>();
                    stock.Trans.Add(Rec);
                    this.Owned.Add(stock);
                    this.Cash += Rec.Amount;
                }
            }
        }


        private void SoldStock(TransactionDB Rec)
        {
            StocksOwned stock = new StocksOwned();
            stock.Symbol = Rec.Symbol;
            stock.Qty = Rec.Qty;
            stock.Cost = Rec.Amount;
            stock.Commission = Rec.Commission;
            stock.Price = Rec.Price;
            stock.TranId = Convert.ToDouble(Rec.TranId);
            stock.BuyDate = Rec.Date;
            stock.dividends = 0;
            this.Sold.Add(stock);

        }
    }

    public struct CashDeposits
    {
        public DateTime date { get; set; }
        public decimal Cash { get; set; }
        public string Type { get; set; }
    }
}
