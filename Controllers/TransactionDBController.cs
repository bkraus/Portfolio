using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portfolio.Models.Portfolio;

namespace Portfolio.Controllers
{
    public class TransactionDBController : Controller
    {
        private PortfolioContext db = new PortfolioContext();

        //
        // GET: /TransactionDB/

        public ActionResult Index()
        {
            return View(db.Transaction.ToList());
        }

        //
        // GET: /TransactionDB/Details/5

        public ActionResult Details(int id = 0)
        {
            TransactionDB transactiondb = db.Transaction.Find(id);
            if (transactiondb == null)
            {
                return HttpNotFound();
            }
            return View(transactiondb);
        }

        //
        // GET: /TransactionDB/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TransactionDB/Create

        [HttpPost]
        public ActionResult Create(TransactionDB transactiondb)
        {
            if (ModelState.IsValid)
            {
                db.Transaction.Add(transactiondb);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(transactiondb);
        }

        //
        // GET: /TransactionDB/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TransactionDB transactiondb = db.Transaction.Find(id);
            if (transactiondb == null)
            {
                return HttpNotFound();
            }
            return View(transactiondb);
        }

        //
        // POST: /TransactionDB/Edit/5

        [HttpPost]
        public ActionResult Edit(TransactionDB transactiondb)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transactiondb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(transactiondb);
        }

        //
        // GET: /TransactionDB/Delete/5

        public ActionResult Delete(int id = 0)
        {
            TransactionDB transactiondb = db.Transaction.Find(id);
            if (transactiondb == null)
            {
                return HttpNotFound();
            }
            return View(transactiondb);
        }

        //
        // POST: /TransactionDB/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            TransactionDB transactiondb = db.Transaction.Find(id);
            db.Transaction.Remove(transactiondb);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}