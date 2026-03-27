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
        public ActionResult Index()
        {
            if (Session["VisitorCode"] != null)
            {
                return RedirectToAction("Home", "Visitor");
            }

            return View();
        }

        //[HttpPost] //login 

        //public async Task<ActionResult> Login(LoginRS model, string returnUrl)
        //{

        //    BALFarmOwner obj = new BALFarmOwner();
        //    DataSet ds = await obj.Login(model);

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        // OWNER
        //        Session["UserId"] = ds.Tables[0].Rows[0]["UserId"];
        //        Session["Email"] = ds.Tables[0].Rows[0]["Email"];
        //        Session["OwnerCode"] = ds.Tables[0].Rows[0]["FarmOwnerCode"];

        //        return RedirectToAction("Dashboard", "FarmOwner");
        //    }
        //    else if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
        //    {
        //        // VISITOR
        //        Session["UserId"] = ds.Tables[1].Rows[0]["UserId"];
        //        Session["Email"] = ds.Tables[1].Rows[0]["Email"];
        //        Session["VisitorCode"] = ds.Tables[1].Rows[0]["VisitorCode"];
        //        Session["VisitorName"] = ds.Tables[1].Rows[0]["FullName"];
        //        TempData["Login"] = "Success";

        //        //        // 🔥 IMPORTANT PART
        //        if (!string.IsNullOrEmpty(returnUrl))
        //            return Redirect(returnUrl);

        //        return RedirectToAction("Home", "Visitor");
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Invalid Email or Password";
        //        return View("Index");
        //    }
        //}

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

                    TempData["Login"] = "Success";

                    return RedirectToAction("Home", "Visitor");
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

    }
}