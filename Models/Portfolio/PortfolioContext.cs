using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Portfolio.Models.Preference;

namespace Portfolio.Models.Portfolio
{
    public class PortfolioContext : DbContext
    {
        public DbSet<TransactionDB> Transaction { get; set; }
        public DbSet<Preferences> Options { get; set; }
        public DbSet<PortFolios> PortFolios { get; set; }
        public DbSet<Quotes> Quotes { get; set; }
        public DbSet<PortfolioHistories> History { get; set; }
        public DbSet<HistoricalData> HistoricalData { get; set; }

        public void EmptyAllTables()
        {
            this.Database.ExecuteSqlCommand("delete from transactionDBs");
            this.Database.ExecuteSqlCommand("ALTER TABLE transactionDBs ALTER COLUMN [Id] IDENTITY (1, 1) ");
            this.Database.ExecuteSqlCommand("delete from Portfolios");
            this.Database.ExecuteSqlCommand("ALTER TABLE Portfolios ALTER COLUMN [Id] IDENTITY (1, 1) ");
            this.Database.ExecuteSqlCommand("delete from Preferences");
            this.Database.ExecuteSqlCommand("ALTER TABLE Preferences ALTER COLUMN [Id] IDENTITY (1, 1) ");
            this.SaveChanges();
        }
        public void EmptyQuotes()
        {
            try
            {
                this.Database.ExecuteSqlCommand("delete from Quotes");
                this.SaveChanges();
//                this.Database.ExecuteSqlCommand("ALTER TABLE Quotes ALTER COLUMN [Id] IDENTITY (1, 1) ");
//                this.SaveChanges();
            }
            catch { };
        }
        public void EmptyHistoricalData()
        {
            try
            {
                this.Database.ExecuteSqlCommand("delete from HistoricalDatas");
                this.SaveChanges();
                this.Database.ExecuteSqlCommand("ALTER TABLE HistoricalDatas ALTER COLUMN [Id] IDENTITY (1, 1) ");
                this.SaveChanges();
            }
            catch { };
        }
    }

}