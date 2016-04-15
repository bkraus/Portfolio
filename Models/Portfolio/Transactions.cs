using Portfolio.Models.Portfolio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Portfolio.Models
{
    public class Transactions
    {
        public string TransName = "";
        public List<TransRec> Records = null;
        public string TransQry = string.Empty;
        private bool HasTransactions = true;
        public Transactions(string name)
        {
            TransName = name;
            TransQry = "SELECT * FROM [" + name + "$]";
            if (TransName.Equals("Sold"))
                HasTransactions = false;
        }
        public Transactions(string name, string Watches)
        {
            TransName = name;
            HasTransactions = false;
            List<string> symbols = Watches.Split(',').ToList<string>();
            foreach (string x in symbols)
            {
                StocksOwned stock = new StocksOwned();
                stock.Symbol = x;
                stock.Qty = 0;
                stock.Cost = 0;
                stock.Commission = 0;
                stock.Price = 0;
                stock.TranId = 0;
//                ps.Owned.Add(stock);
            }
        }
        public void LoadData()
        {
            if (!HasTransactions)
                return;
            LoadTransactions();
        }

        public void LoadTransactions()
        {
            Records = new List<TransRec>();
            DataTable data = Util.GetExcelData(TransQry);
            if (data != null)
            {
                foreach (DataRow dr in data.Rows)
                {
                    if (dr.Field<DateTime?>("Date") != null)
                    {
                        TransRec rec = new TransRec();
                        rec.Date = dr.Field<DateTime>("Date");
                        rec.TranId = dr.Field<double?>("TranID") == null ? 0 : dr.Field<double>("TranID");
                        rec.Desc = dr.Field<string>("Description");
                        rec.Type = dr.Field<string>("TranType");
                        rec.Qty = dr.Field<double?>("Quantity") == null ? 0 : Util.getdecimal(dr.Field<double>("Quantity"));
                        rec.Symbol = dr.Field<string>("Symbol");
                        rec.Price = dr.Field<double?>("Price") == null ? 0 : Util.getdecimal(dr.Field<double>("Price"));
                        rec.Commission = dr.Field<double?>("Commission") == null ? 0 : Util.getdecimal(dr.Field<double>("Commission"));
                        rec.amount = Util.getdecimal(dr.Field<Double>("AMOUNT"));
                        rec.MyAdjustment = dr.Field<double?>("MyAdj") == null ? 0 : Util.getdecimal(dr.Field<double>("MyAdj"));
                        rec.IsMyAjustment = dr.Field<string>("MyTran") == "Y";
                        string dbl = string.IsNullOrEmpty(dr.Field<string>("trade")) ? "0" : dr.Field<string>("trade");
                        rec.TradeID = int.Parse(dbl);
                        Records.Add(rec);
                    }
                }
            }
        }


    }


    public class TransRec
    {
        public DateTime Date { get; set; }
        public double TranId { get; set; }
        public string Desc { get; set; }
        public decimal Qty { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public decimal amount { get; set; }
        public decimal MyAdjustment { get; set; }
        public bool IsMyAjustment { get; set; }
        public String Type { get; set; }
        public int TradeID { get; set; }
    }





}