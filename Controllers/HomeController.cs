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
        public ActionResult Index()
        {
            if (Session["VisitorCode"] != null)
            {
                return RedirectToAction("Home", "Visitor");
            }

            return View();
        }

        [HttpPost] //login 

        public async Task<ActionResult> Login(LoginRS model, string returnUrl)
        {

            BALFarmOwner obj = new BALFarmOwner();
            DataSet ds = await obj.Login(model);

            if (ds.Tables[0].Rows.Count > 0)
            {
                // OWNER
                Session["UserId"] = ds.Tables[0].Rows[0]["UserId"];
                Session["Email"] = ds.Tables[0].Rows[0]["Email"];
                Session["OwnerCode"] = ds.Tables[0].Rows[0]["FarmOwnerCode"];

                return RedirectToAction("Dashboard", "FarmOwner");
            }
            else if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                // VISITOR
                Session["UserId"] = ds.Tables[1].Rows[0]["UserId"];
                Session["Email"] = ds.Tables[1].Rows[0]["Email"];
                Session["VisitorCode"] = ds.Tables[1].Rows[0]["VisitorCode"];
                Session["VisitorName"] = ds.Tables[1].Rows[0]["FullName"];
                TempData["Login"] = "Success";

                //        // 🔥 IMPORTANT PART
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Home", "Visitor");
            }
            else
            {
                ViewBag.Message = "Invalid Email or Password";
                return View("Index");
            }
        }

        public ActionResult Logout()
        {
            TempData["Logout"] = true;

            Session.RemoveAll();

            return RedirectToAction("Home", "Visitor");
        }


        public ActionResult AgroTrip()
        {
            return View();
        }
        public ActionResult Header()
        {
            return View();
        }

    }
}
