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
using System.IO;
using Company.ViewModels;
using System.Net.Mail;
using System.Threading.Tasks;
using PagedList;



namespace Company.Controllers
{
    public class DevicesController : Controller
    {
        private CompanyContext db = new CompanyContext();

        // GET: Devices
        public ActionResult Index(string sortOrder, string searchString, string price, int? page, string currentFilter)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DeviceSortParm = sortOrder == "Device" ? "Device_desc" : "Device";
            ViewBag.DoubleSortParm = sortOrder == "Double" ? "double_desc" : "Double";
            var devices = from a in db.Devices select a;
            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(price) )
            {
                double pricePieces = double.Parse(price);
                devices = db.Devices.Where(a => a.NameElectronicDevice.Contains(searchString) && a.PricePerPieces <= pricePieces);
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            switch (sortOrder)
            {
                case "Device_desc":
                   devices = devices.OrderByDescending(a => a.NameElectronicDevice);
                    break;
                case "Device":
                    devices = devices.OrderBy(a => a.NameElectronicDevice);
                    break;
                case "double_desc":
                    devices = devices.OrderByDescending(a => a.PricePerPieces);
                    break;
                case "Double":
                    devices = devices.OrderBy(a => a.PricePerPieces);
                    break;
                case "name_desc":
                    devices = devices.OrderByDescending(a => a.deviceCategory);
                    break;
                default:
                    devices = devices.OrderBy(a => a.deviceCategory);
                    break;


            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(devices.ToPagedList(pageNumber, pageSize));
        }
        /* public ActionResult Index()
       {
            return View(db.Devices.ToList());
        }*/

        // GET: Devices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            Orders orders = new Orders
            {
                DeviceID = device.ID
            };
            IEnumerable<Orders> ordersEn = db.Orders.Where(o => o.DeviceID.Equals(device.ID));

            DeviceOrdersViewModel viewModel = new DeviceOrdersViewModel
            {
                DeviceVM = device,
                OrdersVM = orders,
                OrdersVME = ordersEn

            };
            return View(viewModel);
        }

        // GET: Devices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Devices/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NameElectronicDevice,deviceCategory,PricePerPieces,quantity,DeviceDescription,Image")] Device device)
        {
            if (ModelState.IsValid)
            {
                UpdateModel(device);
                HttpPostedFileBase file = Request.Files["fileWithImage"];
                if (file != null && file.ContentLength > 0)
                {
                    device.Image = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/") + device.Image);
                }
                db.Devices.Add(device);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(device);
        }

        // GET: Devices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        // POST: Devices/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NameElectronicDevice,deviceCategory,PricePerPieces,quantity,DeviceDescription,Image")] Device device)
        {
            if (ModelState.IsValid)
            {
                UpdateModel(device);
                HttpPostedFileBase file = Request.Files["fileWithImage"];
                if (file != null && file.ContentLength > 0)
                {
                    device.Image = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/") + device.Image);
                }
                db.Entry(device).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(device);
        }

        // GET: Devices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Device device = db.Devices.Find(id);
            db.Devices.Remove(device);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SendEmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendEmail(string receiver, string subject, string message)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("smartshop2020@int.pl", "smartshop2020");
                    var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "Inzynier2020!";
                    var sub = subject;
                    var body = message;
                    var smtp = new SmtpClient
                    {
                        Host = "poczta.int.pl",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
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
