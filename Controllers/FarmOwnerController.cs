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
using AgroClassLib.FarmOwner;

namespace GSTAgroTourism.Controllers
{
    public class FarmOwnerController : Controller
    {
        // GET: FarmOwner

        BALFarmOwner objbalfarmowner = new BALFarmOwner();

        public ActionResult Index()
        {
            return View();
        }
    }
}