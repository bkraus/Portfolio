using Portfolio.Models.Portfolio;
using Portfolio.Models.Preference;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel; 

namespace Portfolio.Models
{
    public class Util
    {
        public const string HDLocation = "C:\\Users\\Robert\\Music\\iTunes\\iTunes Media\\Music";
        public const string USBLocation = "L:\\USB Music\\My Music";
        public const string MetadataLocation = "c:\\MusicBackup\\WMPMetadata.xml";
        public const string BattleDataLocation = "c:\\MusicBackup\\Battledata.xml";
        public static string RemoveQuotes(string col)
        {
            return col.Replace("\"", "");
        }
        public static decimal getdecimal(string col)
        {
            decimal res = 0;
            decimal.TryParse(col, out res);
            res = decimal.Round(res, 4);
            return res;

        }
        public static decimal getdecimal(Double col)
        {
            decimal res = (Decimal)col;
            res = decimal.Round(res, 6);
            return res;

        }
        public static decimal GetPct(Decimal col)
        {
            decimal res = col * 100;
            res = decimal.Round(res, 2);
            return res;

        }
        public static decimal RoundDecimal(decimal val, int pos)
        {
            return decimal.Round(val, pos);
        }

        public static DataTable GetExcelData(string qry)
        {
            if (qry.Contains("Watch") || qry.Contains("Sold"))
                return null;
            var fileName = string.Format("{0}\\Transactions.xlsx", "C:\\stocks\\stocks\\bin\\Debug");
//            var fileName = string.Format("{0}\\Transactions.xlsx", Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data"));
            var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; data source={0}; Extended Properties='Excel 12.0;HDR=yes'", fileName);

            var adapter = new OleDbDataAdapter(qry, connectionString);
            var ds = new DataSet();

            adapter.Fill(ds, "trans");

            DataTable data = ds.Tables["trans"];

            return data;
        }
        public static void AddStock(string port, StocksOwned stock)
        {
            Excel.Application oXL = new Excel.Application();
            var fileName = string.Format("{0}\\Transactions.xlsx", Directory.GetCurrentDirectory());
            Excel.Workbook oWB = oXL.Workbooks.Open(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            Excel.Worksheet oWS = oWB.Worksheets[port] as Excel.Worksheet;
            try
            {
                int ii = 1;
                while (!string.IsNullOrEmpty(oWS.Cells[ii, 5].Value2))
                {
                    ii++;
                }
                double tid = oWS.Cells[ii - 1, 2].Value2;

                decimal amt = 0;
                if (stock.Type.Equals("Buy") || stock.Type.Equals("Sold"))
                    amt = (stock.Price * stock.Qty) + stock.Commission;
                else
                    amt = stock.Price;
                oWS.Cells[ii, 1] = DateTime.Today;
                oWS.Cells[ii, 2] = tid + 10;
                oWS.Cells[ii, 3] = "App Added " + stock.Type.ToUpper();
                if (stock.Type.Equals("Buy") || stock.Type.Equals("Sold") || stock.Type.Equals("SharesIn") || stock.Type.Equals("SharesOut"))
                    oWS.Cells[ii, 4] = stock.Qty;
                else
                    oWS.Cells[ii, 4] = "";
                if (!stock.Type.Equals("Interest"))
                    oWS.Cells[ii, 5] = stock.Symbol;
                else
                    oWS.Cells[ii, 5] = "";
                if (stock.Type.Equals("Buy") || stock.Type.Equals("Sold"))
                    oWS.Cells[ii, 6] = stock.Price;
                else
                    oWS.Cells[ii, 6] = "";
                if (stock.Type.Equals("Buy") || stock.Type.Equals("Sold"))
                    oWS.Cells[ii, 7] = stock.Commission;
                else
                    oWS.Cells[ii, 7] = "";
                oWS.Cells[ii, 8] = stock.Type == "Buy" ? -amt : amt;
                oWS.Cells[ii, 9] = "";
                oWS.Cells[ii, 10] = "";
                oWS.Cells[ii, 11] = stock.Type == "Buy" ? -amt : amt;
                oWS.Cells[ii, 12] = "";
                oWS.Cells[ii, 13] = "Y";
                oWS.Cells[ii, 14] = stock.Type.ToUpper();
                oWS.Cells[ii, 15] = "";
                oWS.Cells[ii, 16] = "=p" + (ii - 1).ToString() + "+k" + ii.ToString();
                oXL.DisplayAlerts = false;
                oWB.Close(true);
                oXL.Quit();
            }
            catch { }
            finally
            {
                NAR(oWS);
                NAR(oWB);
                NAR(oXL);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        public static void WritetoExcel(PPreferences pref)
        {
            List<IEarn> earn = new List<IEarn>();
            foreach (string p in pref.Earnings)
            {
                List<string> v = p.Split(';').ToList();
                IEarn e = new IEarn();
                e.symbol = v[0];
                e.date = v[1];
                e.saved = false;
                earn.Add(e);
            }
            Excel.Application oXL = new Excel.Application();
            var fileName = string.Format("{0}\\Transactions.xlsx", Directory.GetCurrentDirectory());
            Excel.Workbook oWB = oXL.Workbooks.Open(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            Excel.Worksheet oWS = oWB.Worksheets["Main"] as Excel.Worksheet;
            try
            {
                int ii = 1;
                while (!string.IsNullOrEmpty(oWS.Cells[ii, 2].Value2))
                {
                    if (oWS.Cells[ii, 2].Value2 == "Watches")
                        oWS.Cells[ii, 3] = pref.Watches;
                    else if (oWS.Cells[ii, 2].Value2 == "Earnings")
                    {
                        string test = oWS.Cells[ii, 3].Value2;
                        List<string> pt = test.Split(';').ToList();
                        int gg = earn.FindIndex(x => x.symbol.Equals(pt[0]));
                        if (gg >= 0)
                        {
                            oWS.Cells[ii, 3] = earn[gg].symbol + ";" + earn[gg].date;
                            earn[gg].saved = true;

                        }
                    }
                    else if (oWS.Cells[ii, 2].Value2 == "SPYdate")
                        oWS.Cells[ii, 3] = pref.SPYDate;
                    else if (oWS.Cells[ii, 2].Value2 == "MWidth")
                        oWS.Cells[ii, 3] = pref.MaxWidth;
                    else if (oWS.Cells[ii, 2].Value2 == "MHeight")
                        oWS.Cells[ii, 3] = pref.MaxHeight;
                    ii++;
                }
                foreach (IEarn e in earn)
                {
                    if (!e.saved && !string.IsNullOrEmpty(e.date))
                    {
                        oWS.Cells[ii, 1] = ii;
                        oWS.Cells[ii, 2] = "Earnings";
                        oWS.Cells[ii, 3] = e.symbol + ";" + e.date;
                    }
                }
                oXL.DisplayAlerts = false;
                oWB.Close(true);
                oXL.Quit();
            }
            catch { }
            finally
            {
                NAR(oWS);
                NAR(oWB);
                NAR(oXL);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        private static void NAR(object o)
        {
            try
            {
                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(o) > 0) ;
            }
            catch { }
            finally
            {
                o = null;
            }
        }
        public static bool UpdateDBTransactions()
        {
            var context = new PortfolioContext();
            DateTime lastUpdate = Preferences.GetDate("LastUpdateDate");
            List<PortFolios> portfolios = context.PortFolios.ToList();
            foreach (PortFolios folio in portfolios)
            {
                Transactions transactions = new Transactions(folio.Name);
                transactions.LoadData();
                if (transactions.Records != null && transactions.Records.Count > 0)
                {
                    foreach (TransRec rec in transactions.Records)
                    {
                        if (rec.Date > lastUpdate)
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
                            tdb.Portfolio = folio.PortfolioID;
                            context.Transaction.Add(tdb);
                        }
                    }
                }
            }
            Preferences.Update("LastUpdateDate", DateTime.Now.Date.ToString());
            context.SaveChanges();
            return true;
        }
    }
    public class IEarn
    {
        public string symbol { get; set; }
        public string date { get; set; }
        public bool saved { get; set; }
    }
}