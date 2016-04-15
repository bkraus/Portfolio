using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class StocksOwned
    {
        public string Symbol;
        public decimal Price;
        public decimal Qty;
        public decimal Cost;
        public decimal Commission;
        public double TranId;
        public DateTime BuyDate;
        public decimal dividends;
        public string Type;
        public List<TransactionDB> Trans;
    }
}