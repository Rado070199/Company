using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Company.DAL;
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
    public class HomeController : Controller
    {
        private CompanyContext db = new CompanyContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            IQueryable<StatisticsViewModel> data = from order in db.Orders
                                                   group order by order.TransactionDate into dateGroup
                                                   select new StatisticsViewModel()
                                                   {
                                                       TransactionDate = dateGroup.Key,
                                                       OrdersCount = dateGroup.Count()
                                                   };
            return View(data.ToList());
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SendEmail()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult SendEmail(string receiver, string subject, string msg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    String username = "xena3827@gmail.com";
                    String pass = "Tii71hir!";
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(username));
                    email.To.Add(MailboxAddress.Parse(receiver));
                    email.Subject = subject;
                    email.Body = new TextPart(TextFormat.Plain) { Text = msg };
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
    }
}