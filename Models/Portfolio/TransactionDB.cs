using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class TransactionDB
    {
        [Key]
        public int ID { get; set; }
        public int Portfolio { get; set; }
        public string TradeID { get; set; }
        public DateTime Date { get; set; }
        public string TranId { get; set; }
        public string Desc { get; set; }
        public decimal Qty { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public decimal Amount { get; set; }
        public decimal MyAdjustment { get; set; }
        public bool IsMyAdjustment { get; set; }
        public String Type { get; set; }

        public static List<TransactionDB> PortfolioTransactions(int PortID)
        {
            var context = new PortfolioContext();

            List<TransactionDB> pt = context.Transaction.Where(p => p.Portfolio == PortID).ToList();
            return pt;
        }
        public static List<TransactionDB> PortfolioTransactions(int PortID, DateTime endDate)
        {
            var context = new PortfolioContext();

            List<TransactionDB> pt = context.Transaction.Where(p => p.Portfolio == PortID && p.Date <=endDate).ToList();
            return pt;
        }

    }


}