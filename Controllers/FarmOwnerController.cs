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

        BALFarmOwner objbalfarm = new BALFarmOwner();

        #region Rutuja
        public ActionResult Index()
        {
            if (Session["VisitorCode"] != null)
            {
                return RedirectToAction("Home", "Visitor");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoginRS(LoginRS model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                DataSet ds = await objbalfarm.Login(model);

                // OWNER LOGIN
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Session["UserId"] = ds.Tables[0].Rows[0]["FarmOwnerId"].ToString();
                    Session["Email"] = ds.Tables[0].Rows[0]["Email"].ToString();
                    Session["OwnerCode"] = ds.Tables[0].Rows[0]["FarmOwnerCode"].ToString();

                    // Subscription check
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        return RedirectToAction("BankAccountAA", "FarmOwner");
                    }
                    else
                    {
                        return RedirectToAction("SubscriptionRS", "FarmOwner");
                    }
                }

                // VISITOR LOGIN  🔥 IMPORTANT CHANGE
                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    Session["UserId"] = ds.Tables[2].Rows[0]["UserId"].ToString();
                    Session["Email"] = ds.Tables[2].Rows[0]["Email"].ToString();
                    Session["VisitorCode"] = ds.Tables[2].Rows[0]["VisitorCode"].ToString();
                    Session["VisitorName"] = ds.Tables[2].Rows[0]["FullName"].ToString();

                    TempData["Login"] = "success";

                    TempData.Keep("Login"); 

                    return RedirectToAction("Home");
                }

                // INVALID LOGIN
                ModelState.AddModelError("", "Invalid email or password");
                return View("Index", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", model);
            }
        }
        public ActionResult Logout()
        {
            TempData["Logout"] = true;

            Session.RemoveAll();

            return RedirectToAction("Home", "Visitor");
        }

        #endregion Rutuja
    }
}