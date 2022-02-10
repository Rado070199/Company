using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Company.DAL;
using Company.Models;

namespace Company.Controllers
{
    public class TopicalitiesController : Controller
    {
        private CompanyContext db = new CompanyContext();

        // GET: Topicalities
        public ActionResult Index()
        {
            return View(db.Topicalities.ToList());
        }

        // GET: Topicalities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topicality topicality = db.Topicalities.Find(id);
            if (topicality == null)
            {
                return HttpNotFound();
            }
            return View(topicality);
        }

        // GET: Topicalities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Topicalities/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TripDescription")] Topicality topicality)
        {
            if (ModelState.IsValid)
            {
                db.Topicalities.Add(topicality);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(topicality);
        }

        // GET: Topicalities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topicality topicality = db.Topicalities.Find(id);
            if (topicality == null)
            {
                return HttpNotFound();
            }
            return View(topicality);
        }

        // POST: Topicalities/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TripDescription")] Topicality topicality)
        {
            if (ModelState.IsValid)
            {
                db.Entry(topicality).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(topicality);
        }

        // GET: Topicalities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topicality topicality = db.Topicalities.Find(id);
            if (topicality == null)
            {
                return HttpNotFound();
            }
            return View(topicality);
        }

        // POST: Topicalities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Topicality topicality = db.Topicalities.Find(id);
            db.Topicalities.Remove(topicality);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
