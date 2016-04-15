using Portfolio.Models.Portfolio;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Preference
{
    public class Preferences
    {
        public int ID { get; set; }
        public string Preference { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public static string Retrieve(string Option)
        {
            var context = new PortfolioContext();
            Preferences opt = context.Options.Where(p => p.Preference == Option).First();
            return opt.Value;
        }
        public static DateTime GetDate(string Option)
        {
            var context = new PortfolioContext();
            Preferences opt = context.Options.Where(p => p.Preference == Option).First();
            return DateTime.Parse(opt.Value);
        }
        public static decimal GetDecimal(string Option)
        {
            var context = new PortfolioContext();
            Preferences opt = context.Options.Where(p => p.Preference == Option).First();
            return Decimal.Parse(opt.Value);
        }
        public static decimal GetDecimal(string Option, string desc)
        {
            var context = new PortfolioContext();
            Preferences opt = context.Options.Where(p => p.Preference == Option && p.Description == desc).First();
            return Decimal.Parse(opt.Value);
        }
        public static void Update(string Option, decimal Value)
        {
            Update(Option,"", Value.ToString());
        }
        public static void Update(string Option,string Desc, decimal Value)
        {
            Update(Option,Desc, Value.ToString());
        }
        public static void Update(string Option, string Value)
        {
            Update(Option,"", Value);
        }
        public static void Update(string Option,string desc, string Value)
        {
            var context = new PortfolioContext();
            Preferences opt = null;
            if (string.IsNullOrEmpty(desc))
                opt = context.Options.Where(p => p.Preference == Option ).FirstOrDefault();
            else
                opt = context.Options.Where(p => p.Preference == Option && p.Description==desc).FirstOrDefault();
            if (opt == null)
            {
                opt = new Preferences();
                opt.Preference = Option;
                opt.Value = Value;
                context.Options.Add(opt);
            }
            else
            {
                opt.Value = Value;
            }
            context.SaveChanges();
        }
    }

}