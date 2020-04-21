using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using OBM.Models.Contact;
using reCAPTCHA.MVC;


namespace OBM.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaValidator]
        public async Task<ActionResult> ContactForm(Contact contact)
        {
            if (ModelState.IsValid)
            {
                var senderEmail = System.Web.Configuration.WebConfigurationManager.AppSettings["senderEmail"];
                var senderPassword = System.Web.Configuration.WebConfigurationManager.AppSettings["senderPass"];
                var supportEmail = System.Web.Configuration.WebConfigurationManager.AppSettings["supportEmail"];

                var body = 
                "<div style=\"border-style: dotted;\">"+
                    "<div>"+
                            "<b> User Inquiry</ b>"+
                   "</ div>"+
                    "< div >"+
                       "< p >< b > Sender's Name: </b>"+contact.SenderName+"</p>"+
                    "</ div>"+
                    "< div >"+
                        "<p><b>Sender's Contact: </b>"+contact.SenderEmail+"</p>"+
                    "</ div>"+
                    "<div>"+
                        "<p><b>Inquiry: </ b> "+contact.EmailSubject+" </ p>"+
                        "<hr />"+
                    "</ div>"+
                    "<div>"+
                        "<p><b>Inquiry Content: </ b></ p>"+
                        "<p>"+contact.EmailBody+"</ p>"+
                    "</ div>"+
                "</ div>";

                var email = new MailMessage();
                email.To.Add(new MailAddress(supportEmail));
                email.From = new MailAddress(senderEmail);
                email.Subject = $"{contact.EmailSubject} {DateTime.Now}";
                email.Body = string.Format(body, contact.SenderName, contact.SenderEmail, contact.EmailBody);
                email.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential 
                    {
                        UserName = senderEmail,
                        Password = senderPassword
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await smtp.SendMailAsync(email);
                    return RedirectToAction("EmailConfirmation");
                }
            }
            return View(contact);
        }

        [HttpGet]
        public ActionResult EmailConfirmation()
        {
            return View();
        }
    }
}