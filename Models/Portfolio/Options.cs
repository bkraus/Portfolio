using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Portfolio.Models
{
    public class Options
    {
        public PPreferences pref = new PPreferences();
        List<opts> optrecs = new List<opts>();
        
        public Options()
        {
        }
        public Options(PPreferences p)
        {
            pref = p;
        }
        public PPreferences Load()
        {
            pref.SSA = new List<SSA>();
            pref.Analyst = new List<Analyst>();
            pref.portfolios = new List<PrefOpts>();
            pref.Earnings = new List<string>();
            pref.yrValue = new List<decimal>();
            pref.CapsPlayer = new List<CapsPlayer>();
            pref.Watches = "";
            GetXmlData();
            return pref;
        }
        public bool Save()
        {
            SaveOptionsXML();
            return true;
        }
        private void GetXmlData()
        {
            XDocument history = XDocument.Load("c:\\MusicBackup\\FolioOptions.xml");
            List<XElement> Session = history.Root.Elements("Portfolio").ToList();
            foreach (XElement xSession in Session)
            {
                PrefOpts Port = new PrefOpts();
                Port.Name = xSession.Element("Name").Value.ToString();
                Port.Id = Decimal.Parse(xSession.Element("ID").Value);
                Port.IsEditable = xSession.Element("Editable").Value.ToString().Equals("Y");
                Port.MyStocks = xSession.Element("MyStocks").Value.ToString().Equals("Y");
                pref.portfolios.Add(Port);
            }

            List<XElement> Earnings = history.Root.Element("Earnings").Elements("Company").ToList();
            foreach (XElement Comp in Earnings)
            {
                pref.Earnings.Add(Comp.Value.ToString());
            }
            List<XElement> Benefits = history.Root.Element("SSA").Elements("Benefits").ToList();
            foreach (XElement Comp in Benefits)
            {
                SSA ss = new SSA();
                ss.Year = Comp.Attribute("Year").Value;
                ss.benefit = decimal.Parse(Comp.Value);
                pref.SSA.Add(ss);
            }
            List<XElement> Analyst = history.Root.Element("Analyst").Elements("Ticker").ToList();
            foreach (XElement Comp in Analyst)
            {
                Analyst ss = new Analyst();
                ss.Symbol = Comp.Attribute("Symbol").Value;
                ss.PrevScore = decimal.Parse(Comp.Attribute("PrevScore").Value);
                ss.Score = decimal.Parse(Comp.Value);
                pref.Analyst.Add(ss);
            }

            List<XElement> track = history.Root.Element("Track").Elements("Player").ToList();
            foreach (XElement play in track)
            {
                CapsPlayer cp = new CapsPlayer();
                cp.Name = play.Value;
                cp.DisplayName = play.Attribute("Display").Value;
                pref.CapsPlayer.Add(cp);
            }

            pref.Reed = decimal.Parse(history.Root.Element("Reed").Value);

            pref.StartDate = DateTime.Parse(history.Root.Element("StartDate").Value);
            pref.SPYDate = DateTime.Parse(history.Root.Element("SPYDate").Value);
            pref.Watches = history.Root.Element("Watches").Value;
            pref.MaxHeight = int.Parse(history.Root.Element("MHeight").Value);
            pref.MaxWidth = int.Parse(history.Root.Element("MWidth").Value);
            pref.Bank1 = decimal.Parse(history.Root.Element("Bank1").Value);
            pref.Bank2 = decimal.Parse(history.Root.Element("Bank2").Value);
            pref.OtherIncome = decimal.Parse(history.Root.Element("OtherIncome").Value);
            pref.OtherIncome2 = decimal.Parse(history.Root.Element("OtherIncome2").Value);
            pref.RetireDate = DateTime.Parse(history.Root.Element("RetireDate").Value);
            pref.EndDate = DateTime.Parse(history.Root.Element("EndDate").Value);
            pref.Interest = decimal.Parse(history.Root.Element("Interest").Value);
            pref.BankInterest = decimal.Parse(history.Root.Element("BankInterest").Value);
            pref.FutureSavings = decimal.Parse(history.Root.Element("FutureSavings").Value);
            pref.FutureRetire = decimal.Parse(history.Root.Element("FutureRetire").Value);
            pref.MonthlyEstimate = decimal.Parse(history.Root.Element("MonthlyEstimate").Value);
            pref.CapsAmeritrade = decimal.Parse(history.Root.Element("CapsAmeritrade").Value);
            pref.CapsWatch = decimal.Parse(history.Root.Element("CapsWatch").Value);
            pref.CapsVanguard = decimal.Parse(history.Root.Element("CapsVanguard").Value);
            pref.CapsUpdate = history.Root.Element("CapsUpdate").Value;
            pref.AnalystUpdate = history.Root.Element("AnalystUpdate").Value;
        }
        private void SaveOptionsXML()
        {
            XElement root = new XElement("FolioOptions",
                new XElement("StartDate", pref.StartDate.ToShortDateString()),
                new XElement("SPYDate", pref.SPYDate.Date.ToShortDateString()),
                new XElement("Watches", pref.Watches),
                new XElement("MHeight", pref.MaxHeight),
                new XElement("MWidth", pref.MaxWidth),
                new XElement("Bank1", pref.Bank1),
                new XElement("Bank2", pref.Bank2),
                new XElement("OtherIncome", pref.OtherIncome),
                new XElement("OtherIncome2", pref.OtherIncome2),
                new XElement("RetireDate", pref.RetireDate.ToShortDateString()),
                new XElement("EndDate", pref.EndDate.ToShortDateString()),
                new XElement("Interest", pref.Interest),
                new XElement("BankInterest", pref.BankInterest),
                new XElement("FutureSavings", pref.FutureSavings),
                new XElement("FutureRetire", pref.FutureRetire),
                new XElement("MonthlyEstimate", pref.MonthlyEstimate),
                new XElement("CapsAmeritrade", pref.CapsAmeritrade),
                new XElement("CapsWatch", pref.CapsWatch),
                new XElement("CapsVanguard", pref.CapsVanguard),
                new XElement("CapsUpdate", pref.CapsUpdate),
                new XElement("AnalystUpdate", pref.AnalystUpdate),
                new XElement("Reed", pref.Reed),
                new XElement("SSA",
                        pref.SSA.Select(c => new XElement("Benefits", c.benefit,
                        new XAttribute("Year", c.Year)))),
                new XElement("Analyst",
                        pref.Analyst.Select(c => new XElement("Ticker", c.Score,
                        new XAttribute("Symbol", c.Symbol),
                        new XAttribute("PrevScore", c.PrevScore)))),
                new XElement("Track",
                        pref.CapsPlayer.Select(c => new XElement("Player", c.Name,
                        new XAttribute("Display", c.DisplayName)))),
                pref.Earnings.Select(c => new XElement("Earnings",
                 new XElement("Company", c))),
                pref.portfolios.Select(b => new XElement("Portfolio",
                 new XElement("Name", b.Name),
                 new XElement("ID", b.Id),
                 new XElement("Editable", b.IsEditable?"Y":"N"),
                 new XElement("MyStocks", b.MyStocks ? "Y" : "N")
                 )));
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                                          new XDocumentType("cXML", null, "http://xml.cXML.org/schemas/cXML/1.2.024/cXML.dtd", null),
                                          root);
            xDoc.Save("c:\\MusicBackup\\FolioOptions.xml");
        }

    }
    class opts
    {
        public int idx;
        public string item;
        public string value;
    }
    public class PrefOpts
    {
        public decimal Id;
        public string Name;
        public bool IsEditable;
        public bool MyStocks;
    }
    public class SSA
    {
        public string Year;
        public decimal benefit;
    }
    public class Analyst
    {
        public string Symbol;
        public decimal Score;
        public decimal PrevScore;
    }

    public class CapsPlayer
    {
        public string Name;
        public string DisplayName;
    }

    public class PPreferences
    {
        public List<PrefOpts> portfolios;
//        public List<decimal> PortId;
//        public DateTime yrDate;
        public List<decimal> yrValue;
        public List<string> Earnings;
        public string Watches;
        public int MaxHeight;
        public int MaxWidth;
        public DateTime SPYDate;
        public DateTime StartDate;
        public DateTime RetireDate;
        public DateTime EndDate;
        public decimal Bank1;
        public decimal Bank2;
        public decimal OtherIncome;
        public decimal OtherIncome2;
        public List<SSA> SSA;
        public List<Analyst> Analyst;
        public decimal Interest;
        public decimal BankInterest;
        public decimal FutureSavings;
        public decimal FutureRetire;
        public decimal MonthlyEstimate;
        public decimal CapsAmeritrade;
        public decimal CapsWatch;
        public decimal CapsVanguard;
        public string CapsUpdate;
        public string AnalystUpdate;
        public List<CapsPlayer> CapsPlayer;
        public decimal Reed;
    }
}
