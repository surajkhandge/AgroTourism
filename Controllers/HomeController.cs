using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AgroClassLib.FarmOwner;


namespace GSTAgroTourism.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Temp()
        {
            return View();
        }

    }
}
