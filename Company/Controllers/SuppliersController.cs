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
using _Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using Spire.Xls;

namespace Company.Controllers
{
    public class SuppliersController : Controller
    {
        private CompanyContext db = new CompanyContext();

        // GET: Suppliers
        public ActionResult Index()
        {
            var suppliers = db.Suppliers.Include(s => s.device);
            return View(suppliers.ToList());
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Suppliers suppliers = db.Suppliers.Find(id);
            if (suppliers == null)
            {
                return HttpNotFound();
            }
            return View(suppliers);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            ViewBag.DeviceID = new SelectList(db.Devices, "ID", "NameElectronicDevice");
            return View();
        }

        // POST: Suppliers/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NameCompany,DeviceID,quantityDevices")] Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                suppliers.device = db.Devices.Single(o => o.ID.Equals(suppliers.DeviceID));
                suppliers.device.quantity = suppliers.device.quantity + suppliers.quantityDevices;
                db.Suppliers.Add(suppliers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeviceID = new SelectList(db.Devices, "ID", "NameElectronicDevice", suppliers.DeviceID);
            return View(suppliers);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Suppliers suppliers = db.Suppliers.Find(id);
            if (suppliers == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceID = new SelectList(db.Devices, "ID", "NameElectronicDevice", suppliers.DeviceID);
            return View(suppliers);
        }

        // POST: Suppliers/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NameCompany,DeviceID,quantityDevices")] Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(suppliers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeviceID = new SelectList(db.Devices, "ID", "NameElectronicDevice", suppliers.DeviceID);
            return View(suppliers);
        }
      

        [HttpGet]
        public ActionResult OpenFile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OpenFile(HttpPostedFileBase excelFile)
        {
            try
            {
                string filename = excelFile.FileName;
                //Path.GetExtension(excelFile.FileName);
                if (excelFile != null && (filename.EndsWith(".xls")))
                {
                    string path = Server.MapPath("~/Content/") + Guid.NewGuid() + filename;
                    excelFile.SaveAs(path);
                    Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
                    workbook.LoadFromFile($@"{path}");
                    Spire.Xls.Worksheet sheet = workbook.Worksheets[0];
                    string FinalPath = HttpContext.Server.MapPath("~/Files/");
                    sheet.SaveToFile($@"{FinalPath}" + $@"{filename.Replace(".xls", ".csv")}", ",", System.Text.Encoding.UTF8);

                    StreamReader reader = null;
                    int lncnt = 0;
                    List<string> line;
                    string csv = filename.Replace(".xls", ".csv");
                    string pathDownload = FinalPath + csv;
                    reader = new StreamReader(Path.Combine(pathDownload), System.Text.Encoding.UTF8);
                    string header = reader.ReadLine();
                    string hd = @"""device id"",""company name"",""quantity""";
                    List<Suppliers> suppliers = new List<Suppliers>();
                    if (header.Replace(" ", "") == hd.Replace(" ", ""))
                    {
                        while (!reader.EndOfStream)
                        {
                            lncnt++;
                            line = reader.ReadLine().Split(',').Select(t => t.Trim('"', '\'')).ToList();
                            Suppliers s = new Suppliers();
                            s.DeviceID = int.Parse(line[0]);
                            s.NameCompany = line[1];
                            s.quantityDevices = int.Parse(line[2]);
                            s.device = db.Devices.Single(o => o.ID.Equals(s.DeviceID));
                            s.device.quantity = s.device.quantity + s.quantityDevices;
                            suppliers.Add(s);

                        }
                        foreach (var item in suppliers)
                        {
                            db.Suppliers.Add(item);
                        }

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }


                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Nie znaleziono pliku" + e);
            }
            // var parti = from a in db.ShopTable select a;
            // return View(parti.ToList());
            return RedirectToAction("Index");
        }















        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Suppliers suppliers = db.Suppliers.Find(id);
            if (suppliers == null)
            {
                return HttpNotFound();
            }
            return View(suppliers);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Suppliers suppliers = db.Suppliers.Find(id);
            db.Suppliers.Remove(suppliers);
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
