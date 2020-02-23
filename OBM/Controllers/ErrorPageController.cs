using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBM.Controllers
{
    public class ErrorPageController : Controller
    {
        // GET: ErrorPage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error(int statusCode, Exception exception)
        {
            if(statusCode > 399 && statusCode < 500)
            {
                ViewBag.Message = "Could not find this page.";
            }
            else if (statusCode > 499 && statusCode < 600)
            {
                ViewBag.Message = "Server made a mistake.";
            }
            else
            {
                ViewBag.Message = "This is an unexpeced error.";
            }

            Response.StatusCode = statusCode;
            ViewBag.Title = statusCode + " Error";
            ViewBag.StatusCode = statusCode + " Error";

            return View();
        }
    }
}