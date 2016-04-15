using Portfolio.Models.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Portfolio.Models
{
    public class Quotes
    {
        public int ID { get; set; }
        public string Symbol { get; set; }
        public string LastTranTime { get; set; }
        public string LastTranDate { get; set; }
        public decimal Open { get; set; }
        public decimal PreviousClose { get; set; }
        public decimal Last { get; set; }
        public decimal Dividend { get; set; }
        public string ExDivDate { get; set; }
        public string DivPayDate { get; set; }
        public decimal PERatio { get; set; }
        public decimal YearLow { get; set; }
        public decimal YearHigh { get; set; }
        public decimal TargetPrice { get; set; }
        public DateTime LastUpdated { get; set; }
        public static List<Quotes> GetOldPrices()
        {
            var context = new PortfolioContext();
            return context.Quotes.ToList<Quotes>();
        }
        public static List<Quotes> GetPrices(string stocks)
        {
            string fields = "sl1pd1t1dt8qr1rjk";
            string csvData = "";

            using (WebClient web = new WebClient())
            {
                try
                {
                    csvData = web.DownloadString("http://finance.yahoo.com/d/quotes.csv?s=" + stocks + "&f=" + fields);
                }
                catch { }
            }
            if (csvData == null || csvData.Length == 0)
                return GetOldPrices();

            string[] rows = csvData.Replace("\r", "").Split('\n');

            return setPrices(rows);

        }

        private static List<Quotes> setPrices(string[] rows)
        {

            List<Quotes> prices = new List<Quotes>();
            foreach (string row in rows)
            {
                if (string.IsNullOrEmpty(row)) continue;

                string[] cols = row.Split(',');

                Quotes p = new Quotes();
                p.Symbol = Util.RemoveQuotes(cols[0]);
                p.Last = Util.getdecimal(cols[1]);
                p.Open = Util.getdecimal(cols[2]);
                p.LastTranDate = Util.RemoveQuotes(cols[3]);
                p.LastTranTime = Util.RemoveQuotes(cols[4]);
                p.Dividend = Util.getdecimal(cols[5]);
                p.TargetPrice = Util.getdecimal(cols[6]);
                DateTime dt = DateTime.MinValue;
                DateTime.TryParse(Util.RemoveQuotes(cols[7]), out dt);
                p.ExDivDate = dt.ToString();
                DateTime.TryParse(Util.RemoveQuotes(cols[8]), out dt);
                p.DivPayDate = dt.ToString();

                p.PERatio = Util.getdecimal(cols[9]);
                if (cols.Count() > 10)
                {
                    p.YearLow = Util.getdecimal(cols[10]);
                    p.YearHigh = Util.getdecimal(cols[11]);
                }
                prices.Add(p);
                
            }
            var context = new PortfolioContext();
            //            context.EmptyQuotes();
            foreach (Quotes p in prices)
            {
                var res = context.Quotes.Where(w => w.Symbol == p.Symbol).FirstOrDefault();
                if (res != null)
                {
                    if (res.LastUpdated < DateTime.Now.AddMinutes(-15))
                    {
                        res.Last = p.Last;
                        res.Dividend = p.Dividend;
                        res.DivPayDate = p.DivPayDate;
                        res.ExDivDate = p.ExDivDate;
                        res.LastTranDate = p.LastTranDate;
                        res.LastTranTime = p.LastTranTime;
                        res.LastUpdated = DateTime.Now;
                        res.Open = p.Open;
                        res.PERatio = p.PERatio;
                        res.PreviousClose = p.PreviousClose;
                        res.TargetPrice = p.TargetPrice;
                        res.YearHigh = p.YearHigh;
                        res.YearLow = p.YearLow;
                    }
                }
                else
                {
                    p.LastUpdated = DateTime.Now;
                    context.Quotes.Add(p);
                }
            }
            context.SaveChanges(); 
            return prices;
        }
    }
}