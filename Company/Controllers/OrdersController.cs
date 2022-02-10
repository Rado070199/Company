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
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using Company.ViewModels;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

using MailKit.Search;
using MailKit.Net.Imap;
using System.IO;
using MimeKit.Text;


namespace Company.Controllers
{
    public class OrdersController : Controller
    {
        private CompanyContext db = new CompanyContext();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.device).Include(o => o.profile);
            if (User.IsInRole("Admin"))
            {
                return View(orders.ToList());
            }
            else
            {
                return View(db.Orders.Where(o => o.profile.UserName == User.Identity.Name).ToList());
            }
        }


        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.DeviceID = new SelectList(db.Devices, "ID", "NameElectronicDevice");
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName");
            return View();
        }

        // POST: Orders/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "OrdersVM", Include = "ID,ProfileID,DeviceID,NumberOfPieces,Price,status,TransactionDate")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                orders.status = Orders.Status.nieopłacone;
                Profile user = db.Profiles.Single(o => o.UserName.Equals(User.Identity.Name));
                orders.ProfileID = user.ID;
                orders.profile = db.Profiles.Single(o => o.ID.Equals(orders.ProfileID));
                orders.TransactionDate = DateTime.Now.ToString("dd/MM/yyyy");
                orders.device = db.Devices.Single(o => o.ID.Equals(orders.DeviceID));
                orders.Price = orders.NumberOfPieces * orders.device.PricePerPieces;
                orders.device.quantity = orders.device.quantity - orders.NumberOfPieces;
                db.Orders.Add(orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeviceID = new SelectList(db.Devices, "ID", "NameElectronicDevice", orders.DeviceID);
            ViewBag.ProfileID = 1;
            return View(orders);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceID = new SelectList(db.Devices, "ID", "NameElectronicDevice", orders.DeviceID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", orders.ProfileID);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProfileID,DeviceID,NumberOfPieces,Price,status,TransactionDate")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeviceID = new SelectList(db.Devices, "ID", "NameElectronicDevice", orders.DeviceID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", orders.ProfileID);
            return View(orders);
        }
       /* [HttpPost]
        public ActionResult SendConfirmMessage(Orders orders)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("xena2021@int.pl");
                    Profile profile = db.Profiles.Single(p => p.ID.Equals(orders.ProfileID));
                    var receiverEmail = new MailAddress(orders.profile.UserName, "Receiver");
                    var password = "Inzynier2020!";
                    var subject = "Potwierdzenie Zakupu!";
                    var body = $"Szanowny Kliencie! " + "\n" +
                        $"Przesyłam potwierdzenie zakupu  {orders.device.deviceCategory} {orders.device.NameElectronicDevice}  " +
                        $"w liczbie sztuk : {orders.NumberOfPieces} , towar dotrze w przeciagu tygodnia dzieki darmowej przesyłce zawartej w cenie naszych usług!." + "\n" +
                        $"Koszt który Państwo uiścili to  {orders.Price} PLN." + $"W razie wszelkich pytań porosimy o kontakt podany na stronie sklepu." + "\n" + "\n" +
                        $"Dziękujemy za zaufanie i wybranie sklepu smartshop shaya!" + "\n" + "\n" +
                        $"____________________________FAKTURA___________________________________" + "\n" +
                        $"Pan/ Pani :  {profile.Name}  {profile.Surname}" + "\n" +
                        $"zamieszkały/ła w {profile.Address}" + "\n" +
                        $"Zamówienie na urzadzenie : {orders.device.deviceCategory} " + "\n" +
                        $"Nazwa urzadzenia:  {orders.device.NameElectronicDevice}" + "\n" +
                        $"W terminie dnia {orders.TransactionDate} roku" + "\n" +
                        $"w liczbie sztuk : {orders.NumberOfPieces}" + "\n" +
                        $"Koszt : {orders.Price} PLN";

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
        }*/

        [HttpPost]
        public ActionResult SendConfirmMessage(Orders orders)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    String username = "xena3827@gmail.com";
                    String pass = "Tii71hir!";
                    Profile profile = db.Profiles.Single(p => p.ID.Equals(orders.ProfileID));
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(username));
                    var receiverEmail = new MailAddress(orders.profile.UserName, "Receiver");
                    email.To.Add(MailboxAddress.Parse(receiverEmail.ToString()));
                    var subject = "Potwierdzenie Zakupu!";
                    var body = $"Szanowny Kliencie! " + "\n" +
                        $"Przesyłam potwierdzenie zakupu  {orders.device.deviceCategory} {orders.device.NameElectronicDevice}  " +
                        $"w liczbie sztuk : {orders.NumberOfPieces} , towar dotrze w przeciagu tygodnia dzieki darmowej przesyłce zawartej w cenie naszych usług!." + "\n" +
                        $"Koszt który Państwo uiścili to  {orders.Price} PLN." + $"W razie wszelkich pytań porosimy o kontakt podany na stronie sklepu." + "\n" + "\n" +
                        $"Dziękujemy za zaufanie i wybranie sklepu smartshop shaya!" + "\n" + "\n" +
                        $"____________________________FAKTURA___________________________________" + "\n" +
                        $"Pan/ Pani :  {profile.Name}  {profile.Surname}" + "\n" +
                        $"zamieszkały/ła w {profile.Address}" + "\n" +
                        $"Zamówienie na urzadzenie : {orders.device.deviceCategory} " + "\n" +
                        $"Nazwa urzadzenia:  {orders.device.NameElectronicDevice}" + "\n" +
                        $"W terminie dnia {orders.TransactionDate} roku" + "\n" +
                        $"w liczbie sztuk : {orders.NumberOfPieces}" + "\n" +
                        $"Koszt : {orders.Price} PLN";
                    email.Subject = subject.ToString();
                    email.Body = new TextPart(TextFormat.Plain) { Text = body };
                    using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                    {

                        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                        smtp.Authenticate(username, pass);
                        smtp.Send(email);
                        smtp.Disconnect(true);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Nie udało się wysłać email!";
            }
            return View();
        }















        public ActionResult Pay([Bind(Include = "ID")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                orders = db.Orders.Single(o => o.ID.Equals(orders.ID));
                db.Entry(orders).State = EntityState.Modified;
                orders.status = Orders.Status.opłacone;
                db.SaveChanges();
                SendConfirmMessage(orders);
                return RedirectToAction("Index");
            }
            return View(orders);
        }
        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders orders = db.Orders.Find(id);
            if(orders.status== Orders.Status.nieopłacone)
            {
                orders.device.quantity = orders.device.quantity + orders.NumberOfPieces;
            }
            db.Orders.Remove(orders);
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
