using Portfolio.Models.Preference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class HistoricalData : IComparable<HistoricalData>
    {
        public int ID { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public decimal AdjClose { get; set; }

        public int CompareTo(HistoricalData port)
        {
            if (this.Date < port.Date)
                return -1;
            else if (this.Date > port.Date)
                return 1;
            else
                return 0;

        }

        public static HistoricalData Find(string ticker, DateTime startDate)
        {
            if (startDate.Date >= DateTime.Now.Date || startDate.Date < Preferences.GetDate("StartDate").Date
                || ticker.Equals("NRTLQ"))
                return null;
            var Context = new PortfolioContext();
            var hd = Context.HistoricalData.Where(p => p.Symbol.Equals(ticker) && p.Date >= startDate).OrderBy(p=>p.Date).FirstOrDefault();

            return hd;
        }
        public static HistoricalData RetrieveOne(string ticker, DateTime startDate)
        {
            List<HistoricalData> h =  Retrieve(ticker, startDate, startDate.AddDays(1));
            if (h==null || h.Count==0)
                return null;
            return h[0];
        }
        public static List<HistoricalData> Retrieve(string ticker)
        {
            return Retrieve(ticker, Preferences.GetDate("StartDate"), DateTime.Now);
        }
        public static List<HistoricalData> Retrieve(string ticker, DateTime startDate, DateTime endDate)
        {
            if (startDate.Date >= DateTime.Now.Date || startDate.Date < Preferences.GetDate("StartDate").Date
                || ticker.Equals("NRTLQ"))
                return null;
            var Context = new PortfolioContext();
            var hd = Context.HistoricalData.Where(p => p.Symbol.Equals(ticker) && p.Date >= startDate && p.Date <= endDate).ToList();
            if (hd == null)
            {
                hd = GetData(ticker, startDate, endDate);
                if (hd != null)
                {
                    foreach (HistoricalData h in hd)
                        Context.HistoricalData.Add(h);
                    Context.SaveChanges();
                }
            }
            return hd;
        }
        public static List<HistoricalData> RetrieveAll(string ticker)
        {
            if (ticker.Equals("NRTLQ"))
                return new List<HistoricalData>();
            List<HistoricalData> hdlist = GetData(ticker, Preferences.GetDate("StartDate"),DateTime.Now);
            return hdlist;
        }
        public static List<HistoricalData> RetrieveByDate(DateTime StartDate)
        {
            var Context = new PortfolioContext();

            return  Context.HistoricalData.Where(p => p.Date == StartDate).ToList();

        }
        private static List<HistoricalData> GetData(string ticker, DateTime startDate, DateTime endDate)
        {
            List<HistoricalData> retval = new List<HistoricalData>();

            int endm = endDate.Month - 1;
            int stm = startDate.Month - 1;
            string dl = "http://ichart.finance.yahoo.com/table.csv?s=" + ticker + "&d=" + endm +
                "&e=" + endDate.Day + "&f=" + endDate.Year + "&g=d&a=" + stm + "&b=" + startDate.Day +
                    "&c=" + startDate.Year + "&ignore.csv";
            using (WebClient web = new WebClient())
            {
                string data = string.Empty;
                try
                {
                    data = web.DownloadString(dl);
                }
                catch
                { }
                if (data == null) return null;
                data = data.Replace("\r", "");

                string[] rows = data.Split('\n');

                //First row is headers so Ignore it
                for (int i = 1; i < rows.Length; i++)
                {
                    if (rows[i].Replace("\n", "").Trim() == "") continue;

                    string[] cols = rows[i].Split(',');

                    HistoricalData hs = new HistoricalData();
                    hs.Symbol = ticker;
                    hs.Date = Convert.ToDateTime(cols[0]);
                    hs.Open = Util.RoundDecimal(Convert.ToDecimal(cols[1]), 4);
                    hs.High = Util.RoundDecimal(Convert.ToDecimal(cols[2]), 4);
                    hs.Low = Util.RoundDecimal(Convert.ToDecimal(cols[3]), 4);
                    hs.Close = Util.RoundDecimal(Convert.ToDecimal(cols[4]), 4);
                    hs.Volume = 0;// Util.RoundDecimal(Convert.ToDecimal(cols[5]), 4);
                    hs.AdjClose = Util.RoundDecimal(Convert.ToDecimal(cols[6]), 4);
                    retval.Add(hs);
                }
                retval.Sort();
                return retval;
            }
        }
        public static DateTime? GetEarningsDate(string symbol)
        {
            string mfData = "";
            using (WebClient web = new WebClient())
            {
                try
                {
                    mfData = web.DownloadString("http://finance.yahoo.com/q?s=" + symbol);
                }
                catch { }
            }
            int d = mfData.IndexOf("Earnings Date");
            int a = mfData.IndexOf(">", d);
            a = mfData.IndexOf(">", a + 1);
            int b = mfData.IndexOf("<", a);
            string res = mfData.Substring(a + 1, b - a - 1);
            if (res.Equals("N/A"))
                return null;
            if (res.Contains("Est"))
            {
                int f = res.IndexOf("-");
                string res2 = res.Substring(0, f) + DateTime.Now.Year.ToString();
                DateTime dec2 = DateTime.ParseExact(res2, "MMM d yyyy", System.Globalization.CultureInfo.InvariantCulture);
                return dec2 < DateTime.Now ? dec2.AddYears(1) : dec2;
            }
            DateTime dec = new DateTime();
            try
            {
                dec = DateTime.ParseExact(res, "d-MMM-yy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { return null; };
            return dec;
        }
    }
}
