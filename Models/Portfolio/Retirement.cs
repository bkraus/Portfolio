using Portfolio.Models.Preference;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Portfolio
{
    public class Retirement
    {
        public decimal Interest { get; set; }
        [Display(Name="Bank Interest")]
        public decimal BankInterest { get; set; }
        public decimal FutureRetire { get; set; }
        public decimal FutureSavings { get; set; }
        public List<RetireData> retireData { get; set; }
        [Display(Name = "Social Security")]
        public decimal SSABenefit { get; set; }
        public decimal Reed { get; set; }
        public decimal CurrentMonthlySalary { get; set; }
        public decimal FutureMonthlySalary { get; set; }
        public decimal MonthlySalary { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal SigFig { get; set; }
        public int RetiredMonths { get; set; }
        public int WorkingMonths { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RetireDate { get; set; }
        public decimal Bank1 { get; set; }
        public decimal Bank2 { get; set; }
        [Display(Name = "Raymond James")]
        public decimal OtherIncome2 { get; set; }
        public decimal OtherIncome { get; set; }

        public void LoadData(List<PortfolioFunds> portfolioFunds)
        {
            retireData = new List<RetireData>();
            DateTime myrday = new DateTime(1963, 4, 10).AddYears(62);
            Interest = Preferences.GetDecimal("Interest");
            BankInterest = Preferences.GetDecimal("BankInterest");
            FutureRetire = Preferences.GetDecimal("FutureRetire");
            FutureSavings = Preferences.GetDecimal("FutureSavings");
            SSABenefit = Preferences.GetDecimal("SSABenefit");
            Reed = Preferences.GetDecimal("Reed");
            EndDate = Preferences.GetDate("EndDate");
            RetireDate = Preferences.GetDate("RetireDate");
            Bank1 = Preferences.GetDecimal("Bank", "TD Checking");
            Bank2 = Preferences.GetDecimal("Bank", "TD Money Market");
            OtherIncome2 = Preferences.GetDecimal("Bank", "Raymond James");
            OtherIncome = Preferences.GetDecimal("Bank", "Other");
            double total = 0.0;
            double ftotal = 0.0;
            double fv = 0;
            SigFig = 0;
            RetiredMonths = ((EndDate.Year - RetireDate.Year) * 12) + EndDate.Month - RetireDate.Month;
            WorkingMonths = ((RetireDate.Year - DateTime.Today.Year) * 12) + RetireDate.Month - DateTime.Today.Month;
            int AllMonths = ((EndDate.Year - DateTime.Today.Year) * 12) + EndDate.Month - DateTime.Today.Month;

            Microsoft.Office.Interop.Excel.Application xl = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.WorksheetFunction wsf = xl.WorksheetFunction;
            TimeSpan ts = myrday.Subtract(DateTime.Today);
            int numberPayPeriods = (ts.Days + 13) / 14;
            foreach (PortfolioFunds ps in portfolioFunds)
            {
                if (ps.PortfolioType.Equals(1))
                {
                    SigFig += ps.Value;
                    RetireData row = new RetireData();
                    row.RowName = ps.Name;
                    row.PresentValue = string.Format("{0:C}", ps.Value);
                    fv = wsf.Fv((double)Interest / 26, numberPayPeriods, (double)0, -ps.Value, (double)1);
                    ftotal += fv;
                    total += (double)ps.Value;
                    row.FutureValue = string.Format("{0:C}", fv);
                    retireData.Add(row);
                }
            }
            RetireData b1row = new RetireData();
            b1row.RowName = "Checking";
            b1row.PresentValue = string.Format("{0:C}", Bank1);
            b1row.FutureValue = string.Format("{0:C}", Bank1);
            ftotal += (double)Bank1;
            total += (double)Bank1;
            retireData.Add(b1row);
            RetireData b2row = new RetireData();
            b2row.RowName = "Savings";
            b2row.PresentValue = string.Format("{0:C}", Bank2);
            fv = wsf.Fv((double)BankInterest / 26, numberPayPeriods, (double)0, -Bank2, (double)1);
            b2row.FutureValue = string.Format("{0:C}", fv);
            ftotal += (double)fv;
            total += (double)Bank2;
            retireData.Add(b2row);
            RetireData OIrow = new RetireData();
            OIrow.RowName = "Other";
            OIrow.PresentValue = string.Format("{0:C}", OtherIncome);
            OIrow.FutureValue = string.Format("{0:C}", OtherIncome);
            ftotal += (double)OtherIncome;
            total += (double)OtherIncome;
            retireData.Add(OIrow);
            RetireData OI2row = new RetireData();
            OI2row.RowName = "Raymond James";
            OI2row.PresentValue = string.Format("{0:C}", OtherIncome2);
            fv = wsf.Fv((double)Interest / 26, numberPayPeriods, (double)0, -OtherIncome2, (double)1);
            OI2row.FutureValue = string.Format("{0:C}", fv);
            ftotal += (double)fv;
            total += (double)OtherIncome2;
            retireData.Add(OI2row);

            RetireData FRrow = new RetireData();
            FRrow.RowName = "Fut Retire";
            FRrow.PresentValue = string.Format("{0:C}", FutureRetire);
            fv = wsf.Fv((double)Interest / 26, numberPayPeriods, (double)-FutureRetire, (double)0, (double)1);
            FRrow.FutureValue = string.Format("{0:C}", fv);
            ftotal += (double)fv;
            retireData.Add(FRrow);
            RetireData FSrow = new RetireData();
            FSrow.RowName = "Fut Sav";
            FSrow.PresentValue = string.Format("{0:C}", FutureSavings);
            fv = wsf.Fv((double)Interest / 26, numberPayPeriods, (double)-FutureSavings, (double)0, (double)1);
            FSrow.FutureValue = string.Format("{0:C}", fv);
            ftotal += (double)fv;
            retireData.Add(FSrow);
            RetireData Totrow = new RetireData();
            Totrow.RowName = "Total";
            Totrow.PresentValue = string.Format("{0:C}", total); ;
            Totrow.FutureValue = string.Format("{0:C}", ftotal); ;
            retireData.Add(Totrow);

            decimal benefits = SSABenefit + Reed;
            benefits = benefits * RetiredMonths;
            benefits = (benefits + (decimal)total) / AllMonths;
            CurrentMonthlySalary = benefits;

            double retmonth = wsf.Pmt((double)Interest / 12, (double)RetiredMonths, -ftotal);
            FutureMonthlySalary = (decimal)retmonth + SSABenefit;
            MonthlySalary = RealMonthlySalary((decimal)total);
            CurrentValue = (decimal)total;
            Preferences.Update("MonthlyEstimate", MonthlySalary);

        }

        private decimal RealMonthlySalary(decimal total)
        {
            decimal pmt = Preferences.GetDecimal("MonthlyEstimate"); ;
            decimal ii = findValue(total, pmt);
            if (ii < 0)
            {
                while (ii < 0)
                {
                    pmt -= .01M;
                    ii = findValue(total, pmt);
                }
            }
            else
            {
                while (ii > 0)
                {
                    pmt += .01M;
                    ii = findValue(total, pmt);
                }
                pmt -= .01M;
            }
            return pmt;
        }
        private decimal findValue(decimal total, decimal payment)
        {
            int Retiredmonths = ((EndDate.Year - RetireDate.Year) * 12) + EndDate.Month - RetireDate.Month;
            int workingmonths = ((RetireDate.Year - DateTime.Today.Year) * 12) + RetireDate.Month - DateTime.Today.Month;
            for (int ii = 0; ii < workingmonths; ii++)
            {
                decimal ipmt = ii == 0 ? 0 : total * (Interest / 12);
                total = total + ipmt - payment;
            }
            for (int ii = 0; ii < Retiredmonths; ii++)
            {
                decimal ipmt = total * (Interest / 12);
                total = total + ipmt - (payment -SSABenefit);
            }
            return total;
        }
    }
}