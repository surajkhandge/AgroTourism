using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using AgroClassLib.Visitor;
using Twilio;
using Twilio.Rest.Verify.V2.Service;


namespace GSTAgroTourism.Controllers
{
    public class VisitorController : Controller
    {
        BALVisitor objbalvisitor = new BALVisitor();
        #region Suraj
        /* show popular destiantnion and Top rated Farmhoueses*/
        [HttpGet]
        public async Task<ActionResult> Home()
        {
            var populardestination = await objbalvisitor.GetPopularDestinationSK();
            ViewBag.PopularDestination = populardestination;

            var topratedfarms = await objbalvisitor.GetTopRatedFarmsSK();
            ViewBag.TopRatedFarms = topratedfarms;

            var activities = await objbalvisitor.GetTopActivitiesSK();
            ViewBag.TopActivities = activities;

            var reviews = await objbalvisitor.GetTopReviewsSK();
            ViewBag.TopReviews = reviews;

            var stats = await objbalvisitor.GetStatisticsSK();
            ViewBag.TotalFarmhouses = stats.TotalFarmhouses;
            ViewBag.TotalVisitors = stats.TotalVisitors;
            ViewBag.Destinations = stats.TotalDestinations;
            ViewBag.AverageRating = stats.AvgRating;

            return View();
        }

        /* See All FarmhousesList */
        public async Task<ActionResult> FarmHousesListSK()
        {
            var farmhouselist = await objbalvisitor.GetAllFarmHousesSk(null, null);
            return View("FilterFarmhousesSK", farmhouselist);
        }

        /* Filter All FarmhouseList */
        [HttpGet]
        public async Task<ActionResult> FilterFarmhousesSK(decimal? maxPrice, decimal? minRating)
        {
            var farmhouselist = await objbalvisitor.GetAllFarmHousesSk(maxPrice, minRating);
            return PartialView("FarmHousesListSK", farmhouselist);
        }

        /* City Suggestion Dropdown in Searchbox*/
        [HttpGet]
        public async Task<JsonResult> CitySuggestionSK(string prefix)
        {
            var cities = await objbalvisitor.GetCitySuggestionSK(prefix);
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        /* To show searchResult */
        [HttpGet]
        public async Task<ActionResult> SearchResultSK(int cityId, string checkin = null, string checkout = null)
        {
            if (cityId == 0)
            {
                return RedirectToAction("Home");
            }
            var farms = await objbalvisitor.GetSearchResultSK(cityId, checkin, checkout);

            return View("FilterFarmhousesSK", farms);
        }

        /* Add Farmhouses To Wishlist */
        [HttpPost]
        public async Task<JsonResult> AddToWishlistSK(string farmhousecode)
        {
            string visitorcode = Session["VisitorCode"].ToString();
            await objbalvisitor.AddWishlistSK(visitorcode, farmhousecode);
            return Json("added"); ;
        }

        /* Remove Farmhouses From Wishlist */
        [HttpPost]
        public async Task<JsonResult> RemoveFromWishlistSK(string farmhousecode)
        {
            string visitorcode = Session["VisitorCode"].ToString();
            await objbalvisitor.RemoveWishlistSK(visitorcode, farmhousecode);
            return Json("removed");
        }

        /* View Farmhouses Wishlist */
        public async Task<ActionResult> WishlistSK()
        {
            string visitorcode = Session["VisitorCode"].ToString();
            var wishlist = await objbalvisitor.GetWishlistSK(visitorcode);
            return View(wishlist);

        }

        /* To View Complete Farmhouse Details */
        public async Task<ActionResult> FarmDetailsSK(string id)
        {
            VisitorSK farmdetail = await objbalvisitor.GetFarmFullDetailsSK(id);
            var reviews = await objbalvisitor.GetReviewsSKK(id);
            ViewBag.Reviews = reviews;
            return View(farmdetail);
        }

        /* To Check Farmhouse Booking Availibility */
        [HttpGet]
        public async Task<JsonResult> CheckAvailabilitySK(string farmCode, string checkIn, string checkOut)
        {
            var ds = await objbalvisitor.CheckFarmAvailabilitysk(farmCode, checkIn, checkOut);
            string status = "Select Dates";

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                status = ds.Tables[0].Rows[0]["Status"].ToString();
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

        /* To Check Farmhouse Booking Summary */
        public async Task<ActionResult> ConfirmBookingSK()
        {
            if (Session["FarmCode"] == null)
                return RedirectToAction("Index");

            string farmCode = Session["FarmCode"]?.ToString();
            string packageName = Session["PackageName"]?.ToString();
            string checkIn = Session["CheckIn"]?.ToString();
            int nights = Session["Nights"] != null ? Convert.ToInt32(Session["Nights"]) : 1;

            if (farmCode == null || packageName == null)
                return RedirectToAction("Index");
            /* Get package summary */
            ConfirmBookingVM model = new ConfirmBookingVM();
            /* Common View Model */
            var data = await objbalvisitor.GetConfirmBookingSummarySK(farmCode, packageName);
            model.FarmHouseCode = data.FarmHouseCode;
            model.FarmHouseName = data.FarmHouseName;
            model.Location = data.CityName;
            model.PackageName = data.PackageName;
            model.RoomTypeName = data.RoomTypeName;
            model.MealTypeName = data.MealTypeName;
            model.PackagePrice = data.PackagePrice;
            model.DiscountPercent = data.DiscountPercent;
            model.DiscountAmount = data.DiscountAmount;
            model.FinalPrice = data.TotalPrice;

            /* Dates */
            model.CheckIn = Convert.ToDateTime(checkIn);
            model.Nights = nights;
            model.CheckOut = model.CheckIn.AddDays(nights);

            /* Activities */
            var allActivities = await objbalvisitor.GetPackageActivitiesSK(farmCode);
            var codes = data.ActivityCode.Split(',');
            model.Activities = allActivities.FindAll(a => codes.Contains(a.ActivityCode));
            model.IsPackageBooking = true;
            return View("ConfirmBookingSK", model);
        }
        /* Booking Session  */
        [HttpPost]
        public ActionResult SetBookingSessionSK(string farmCode, string packageName, decimal price, int days, int nights, string checkIn, string checkOut)
        {
            Session["FarmCode"] = farmCode;
            Session["PackageName"] = packageName;
            Session["PackagePrice"] = price;
            Session["Days"] = days;
            Session["Nights"] = nights;
            Session["CheckIn"] = checkIn;
            Session["CheckOut"] = checkOut;
            return Json("ok");
        }
        #endregion Suraj

        #region Vaibhav

        // ----------------------------------For Visitor Registration-----------------------------
        [HttpGet]
        public async Task<ActionResult> VisitorRegistrationVH()
        {
            ViewBag.CountryId = new SelectList(await objbalvisitor.FetchAllCountryVH(), "CountryId", "CountryName");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> VisitorRegistrationVH(RegistrationVH VR)
        {
            ViewBag.CountryId = new SelectList(await objbalvisitor.FetchAllCountryVH(), "CountryId", "CountryName");

            await objbalvisitor.VisitorInsertVH(VR);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> SendOTPVH(RegistrationVH VR)
        {
            try
            {
                string accountSid = ConfigurationManager.AppSettings["TwilioAccountSID"];
                string authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
                string verifySid = ConfigurationManager.AppSettings["TwilioVerifySID"];

                if (string.IsNullOrEmpty(accountSid))
                {
                    return Json(new { success = false, message = "Account SID missing from Web.config" });
                }

                TwilioClient.Init(accountSid, authToken);

                await VerificationResource.CreateAsync(
                    to: "+91" + VR.MobileNumber,
                    channel: "sms",
                    pathServiceSid: verifySid
                );

                Session["TempVisitor"] = VR;

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        [HttpPost]
        public async Task<JsonResult> VerifyOTPVH(string otp)
        {
            string accountSid = ConfigurationManager.AppSettings["TwilioAccountSID"];
            string authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
            string verifySid = ConfigurationManager.AppSettings["TwilioVerifySID"];

            if (string.IsNullOrEmpty(accountSid))
            {
                return Json(new { success = false, message = "Account SID missing from Web.config" });
            }

            TwilioClient.Init(accountSid, authToken);

            var VR = Session["TempVisitor"] as RegistrationVH;

            if (VR == null)
            {
                return Json(new { success = false, message = "Session expired." });
            }

            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: "+91" + VR.MobileNumber,
                code: otp,
                pathServiceSid: verifySid
            );

            if (verificationCheck.Status == "approved")
            {
                await objbalvisitor.VisitorInsertVH(VR);
                Session.Remove("TempVisitor");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }



        // ---------------------------------AJAX Calls for fetching states.------------------------------- 
        [HttpGet]
        public async Task<JsonResult> GetStatesAjaxVH(int CountryId)
        {
            var states = await objbalvisitor.FetchAllStateVH(CountryId);
            var stateList = states.Select(s => new { StateId = s.StateId, StateName = s.StateName }).ToList();
            return Json(stateList, JsonRequestBehavior.AllowGet);
        }


        // --------------------------------AJAX Calls for fetching cities.-------------------------------
        [HttpGet]
        public async Task<JsonResult> GetCitiesAjaxVH(int StateId)
        {
            var cities = await objbalvisitor.FetchAllCitiesVH(StateId);
            var cityList = cities.Select(s => new { CityId = s.CityId, CityName = s.CityName }).ToList();
            return Json(cityList, JsonRequestBehavior.AllowGet);
        }

        // ---------------------------------For Visitor Profile--------------------------------

        public async Task<ActionResult> VisitorProfileAA(bool isEdit = false)
        {
            string visitorCode = Session["VisitorCode"].ToString();

            DataSet ds = await objbalvisitor.FetchVisitorProfileAA(visitorCode);

            RegistrationVH vs = new RegistrationVH();

            if (ds.Tables[0].Rows.Count > 0)
            {
                vs.VisitorCode = ds.Tables[0].Rows[0]["VisitorCode"].ToString();
                vs.FullName = ds.Tables[0].Rows[0]["VFullName"].ToString();
                vs.EmailAddress = ds.Tables[0].Rows[0]["VEmail"].ToString();
                vs.MobileNumber = ds.Tables[0].Rows[0]["VMobileNo"].ToString();
                vs.Gender = ds.Tables[0].Rows[0]["GenderName"].ToString();
                vs.AdharCardNo = ds.Tables[0].Rows[0]["VAdharCardNo"].ToString();
                vs.StateName = ds.Tables[0].Rows[0]["StateName"].ToString();
                vs.CityName = ds.Tables[0].Rows[0]["CityName"].ToString();
                vs.Address = ds.Tables[0].Rows[0]["VAddress"].ToString();
                vs.Pincode = ds.Tables[0].Rows[0]["VPincode"].ToString();
            }

            ViewBag.IsEdit = isEdit;

            return View(vs);
        }

        // ---------------------------------For Visitor Profile Post--------------------------------

        [HttpPost]
        public async Task<ActionResult> VisitorProfileAA(RegistrationVH vs)
        {
            try
            {
                await objbalvisitor.EditVisitorProfileAA(vs.VisitorCode, vs.MobileNumber);

                TempData["SuccessMessage"] = "Profile updated successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("VisitorProfileAA");
        }

        // ==================== ROOMS =====================

        // ---------------------------------For Room Selection--------------------------------
        [HttpGet]
        public async Task<ActionResult> SelectRoomsVH(string farmCode)
        {
            Session.Remove("PackageName");
            Session.Remove("PackagePrice");
            Session["FarmHouseCode"] = farmCode;
            RoomBookingVH model = new RoomBookingVH();
            model.Rooms = new List<RoomSelectionVH>();

            DataSet ds = await objbalvisitor.GetRoomsVH();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                model.Rooms.Add(new RoomSelectionVH
                {
                    RoomTypeCode = row["RoomTypeCode"].ToString(),
                    RoomTypeName = row["RoomTypeName"].ToString(),
                    NumberOfGuests = Convert.ToInt32(row["NumberOfGuests"]),
                    PricePerNight = Convert.ToDecimal(row["PricePerNight"]),
                    TotalRooms = Convert.ToInt32(row["TotalRooms"]),
                    Quantity = 0
                });
            }

            model.CheckIn = DateTime.Today;
            model.CheckOut = DateTime.Today.AddDays(1);
            return View(model);
        }

        //  ================================ For Room Selection Post================================
        [HttpPost]
        public ActionResult SaveBookingVH(RoomBookingVH model)
        {
            var selectedRooms = model.Rooms.Where(r => r.Quantity > 0).ToList();

            if (!selectedRooms.Any())
            {
                return RedirectToAction("SelectRoomsVH");
            }
            Session.Remove("PackageName");
            Session.Remove("PackagePrice");
            Session.Remove("Days");
            Session.Remove("Nights");
            Session.Remove("FarmCode");
            Session["SelectedRooms"] = selectedRooms;
            Session["CheckIn"] = model.CheckIn;
            Session["CheckOut"] = model.CheckOut;

            return RedirectToAction("SelectActivitiesVH");
        }


        // ==================== ACTIVITIES =====================


        // ---------------------------------For Activity Selection--------------------------------

        [HttpGet]
        public async Task<ActionResult> SelectActivitiesVH()
        {
            if (Session["CheckIn"] == null)
                return RedirectToAction("SelectRoomsVH");

            ActivityPageVH model = new ActivityPageVH();
            model.Activities = new List<ActivitiesVH>();
            model.CheckIn = (DateTime)Session["CheckIn"];
            model.CheckOut = (DateTime)Session["CheckOut"];
            string farmCode = Session["FarmHouseCode"].ToString();

            var ds = await objbalvisitor.GetActivitiesVH(farmCode);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                model.Activities.Add(new ActivitiesVH
                {
                    ActivityServiceCode = row["ActivityCode"].ToString(),
                    ActivityServiceName = row["ActivityName"].ToString(),
                    Description = row["Description"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    ImagePath = row["ImagePath"].ToString(),
                    //IsSelected = false
                });
            }
            return View(model);
        }


        //  ================================ For Save Activity ================================
        [HttpPost]
        public ActionResult SaveActivitiesVH(ActivityPageVH model)
        {
            var selectedActivities = model.Activities.Where(a => a.IsSelected).ToList();
            Session["SelectedActivities"] = selectedActivities;
            return RedirectToAction("FoodServicesVH");
        }


        // ==================== FOOD SERVICES =====================

        // ---------------------------------For Food Services--------------------------------
        [HttpGet]
        public async Task<ActionResult> FoodServicesVH()
        {
            if (Session["CheckIn"] == null)
                return RedirectToAction("SelectRoomsVH");

            FoodPageVH model = new FoodPageVH();
            model.FoodItems = new List<FoodServiceVH>();
            model.CheckIn = (DateTime)Session["CheckIn"];
            model.CheckOut = (DateTime)Session["CheckOut"];
            string farmCode = Session["FarmHouseCode"].ToString();

            var ds = await objbalvisitor.GetFoodServicesVH();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                model.FoodItems.Add(new FoodServiceVH
                {
                    MealTypeCode = row["MealTypeCode"].ToString(),
                    StartTime = row["StartTime"].ToString(),
                    EndTime = row["EndTime"].ToString(),
                    ImagePath = row["ImagePath"].ToString(),
                    FoodStyleName = row["FoodStyleName"].ToString(),
                    MealName = row["MealName"].ToString()
                });
            }
            return View(model);
        }


        // ------------------------------- For Save Food Services -------------------------------
        [HttpPost]
        public ActionResult SaveFoodServicesVH(FoodPageVH model)
        {
            Session["SelectedFood"] = model.FoodItems;
            return RedirectToAction("GuestDetailsVH");
        }

        [HttpGet]
        public ActionResult GuestDetailsVH()
        {

            GuestPageVH model = new GuestPageVH();

            if (Session["PackageName"] != null)
            {

                model.IsPackageBooking = true;
                model.PackageName = Session["PackageName"].ToString();
                model.PackagePrice = Convert.ToDecimal(Session["PackagePrice"]);
                model.CheckIn = Session["CheckIn"].ToString();
                model.CheckOut = Session["CheckOut"].ToString();
                model.Nights = Convert.ToInt32(Session["Nights"]);
                model.TotalPrice =
                model.PackagePrice;
                model.RoomTotal =
                model.PackagePrice;
                model.ActivityTotal = 0;
                model.FoodTotal = 0;
                model.TotalGuests = 4; // or package capacity

            }
            else
            {
                if (Session["CheckIn"] == null)
                    return RedirectToAction("SelectRoomsVH");

                DateTime checkin = (DateTime)Session["CheckIn"];
                DateTime checkout = (DateTime)Session["CheckOut"];

                model.CheckIn = checkin.ToString("dd-MM-yyyy");
                model.CheckOut = checkout.ToString("dd-MM-yyyy");

                int totalDays = (checkout - checkin).Days;

                if (totalDays <= 0)
                    totalDays = 1;

                model.Nights = totalDays;

                decimal totalRoomPrice = 0;
                decimal totalActivityPrice = 0;
                decimal totalFoodPrice = 0;

                int totalGuests = 0;

                // ================= ROOM PRICE =================
                var rooms = Session["SelectedRooms"] as List<RoomSelectionVH>;

                if (rooms != null && rooms.Count > 0)
                {
                    foreach (var room in rooms)
                    {
                        totalRoomPrice += room.PricePerNight * room.Quantity * totalDays;
                        totalGuests += room.Quantity * room.NumberOfGuests;
                    }
                }

                model.RoomTotal = totalRoomPrice;
                model.TotalGuests = totalGuests;

                // ================= ACTIVITY PRICE =================
                var activities = Session["SelectedActivities"] as List<ActivitiesVH>;

                if (activities != null)
                {
                    foreach (var act in activities)
                    {
                        if (act.IsSelected)
                        {
                            totalActivityPrice += act.Price;
                        }
                    }
                }

                //  ================= Food PRICE =================

                //  ₹1000 per person per day
                decimal foodPerPersonPerDay = 1000;
                totalFoodPrice = foodPerPersonPerDay * totalGuests * totalDays;

                // ================== FINAL TOTAL PRICE =================

                model.RoomTotal = totalRoomPrice;
                model.ActivityTotal = totalActivityPrice;
                model.FoodTotal = totalFoodPrice;
                model.TotalPrice = model.RoomTotal + model.ActivityTotal + model.FoodTotal;
                model.Guests = new List<GuestVH>();
                model.Guests.Add(new GuestVH());
                // your custom booking code

            }

            model.Guests = new List<GuestVH>();

            model.Guests.Add(new GuestVH()
            );

            return View(model);

        }

        // ------------------------------- For Save Guest Details -------------------------------
        [HttpPost]
        public ActionResult GuestDetailsVH(GuestPageVH model)
        {

            if (model.Guests == null || !model.Guests.Any(g =>
            !string.IsNullOrWhiteSpace(g.FullName) ||
            !string.IsNullOrWhiteSpace(g.ContactNumber) ||
            !string.IsNullOrWhiteSpace(g.Email)))
            {
                ModelState.AddModelError("",
                "Please enter details for at least one guest.");

                return View(model);
            }

            // ⭐ IMPORTANT CONDITION

            if (Session["PackageName"] != null)
            {
                return RedirectToAction("ConfirmBookingSK");
            }
            else
            {
                return RedirectToAction("ConfirmBookingVH");
            }

        }


        // ------------------------------- For Booking Confirmation -------------------------------
        [HttpPost]
        public ActionResult ConfirmBookingVH(GuestPageVH model)
        {
            Session["GuestDetails"] = model.Guests;

            return RedirectToAction("BookingSuccessVH");
        }


        // ------------------------------- For Booking Success Page -------------------------------
        [HttpGet]
        public System.Web.Mvc.ActionResult BookingSuccessVH()
        {
            return RedirectToAction("ConfirmBookingVH");
        }


        // ------------------------------- For Booking Confirmation Page -------------------------------

        [HttpGet]
        public async Task<ActionResult> ConfirmBookingVH()
        {

            ConfirmBookingVM model = new ConfirmBookingVM();

            if (Session["PackageName"] != null)
            {

                model.IsPackageBooking = true;

                string farmCode = Session["FarmCode"].ToString();
                string packageName = Session["PackageName"].ToString();
                int nights = Convert.ToInt32(Session["Nights"]);
                string checkIn = Session["CheckIn"].ToString();
                var data = await objbalvisitor.GetConfirmBookingSummarySK(farmCode, packageName);
                model.FarmHouseCode = data.FarmHouseCode;
                model.FarmHouseName = data.FarmHouseName;
                model.Location = data.CityName;
                model.PackageName = data.PackageName;
                model.RoomTypeName = data.RoomTypeName;
                model.MealTypeName = data.MealTypeName;
                model.PackagePrice = data.PackagePrice;
                model.DiscountPercent = data.DiscountPercent;
                model.DiscountAmount = data.DiscountAmount;
                model.FinalPrice = data.TotalPrice;
                model.CheckIn = Convert.ToDateTime(checkIn);
                model.Nights = nights;
                model.CheckOut = model.CheckIn.AddDays(nights);
                var allActivities = await objbalvisitor.GetPackageActivitiesSK(farmCode);
                var codes = data.ActivityCode.Split(',');
                model.Activities = allActivities.FindAll(a => codes.Contains(a.ActivityCode));
                return View("ConfirmBookingSK", model);

            }
            else
            {
                if (Session["CheckIn"] == null || Session["CheckOut"] == null)
                {
                    return RedirectToAction("SelectRoomsVH");
                }



                model.CheckIn = Convert.ToDateTime(Session["CheckIn"]);
                model.CheckOut = Convert.ToDateTime(Session["CheckOut"]);
                model.Nights = (model.CheckOut - model.CheckIn).Days;
                if (model.Nights <= 0)
                    model.Nights = 1;
                model.Rooms = Session["SelectedRooms"] as List<RoomSelectionVH>;
                model.Activities = Session["SelectedActivities"] as List<ActivitiesVH>;

                // FARM DETAILS FIX ⭐
                string farmCode = Session["FarmHouseCode"]?.ToString();

                if (farmCode != null)
                {
                    var farm = await objbalvisitor.GetFarmFullDetailsSK(farmCode);
                    model.FarmHouseCode = farmCode;
                    model.FarmHouseName = farm.FarmHouseName;
                    model.Location = farm.CityName;
                }

                decimal roomTotal = 0;
                decimal activityTotal = 0;

                if (model.Rooms != null)
                {
                    foreach (var room in model.Rooms)
                    {
                        roomTotal += room.PricePerNight * room.Quantity * model.Nights;
                    }
                }

                if (model.Activities != null)
                {
                    foreach (var activity in model.Activities)
                    {
                        if (activity.IsSelected)
                        {
                            activityTotal += activity.Price;
                        }
                    }
                }

                int totalGuests = 0;

                if (model.Rooms != null)
                {
                    foreach (var room in model.Rooms)
                    {
                        totalGuests += room.NumberOfGuests * room.Quantity;
                    }
                }

                decimal foodtotal = 1000 * totalGuests * model.Nights;

                model.RoomTotal = roomTotal;
                model.ActivityTotal = activityTotal;
                model.FoodTotal = foodtotal;
                model.TotalCost = roomTotal + activityTotal + foodtotal;
                model.IsPackageBooking = false;

                return View("ConfirmBookingSK", model);

            }

        }
        #endregion

        #region Shruti 



        [HttpGet]

        public async Task<ActionResult> AddReviewSKK(string bookingCode)
        {
            string farmCode = await objbalvisitor.GetFarmHouseByBooking(bookingCode);

            AddReview model = new AddReview();
            model.FarmHouseCode = farmCode;

            return View(model);
        }

        //Insert AddReviews for perticular FarmHouse
        [HttpPost]
        public async Task<ActionResult> AddReviewSKK(AddReview model)
        {
            model.VisitorCode = Session["VisitorCode"]?.ToString();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(model.ReviewComment, @"^[A-Za-z\s.,]+$"))
            {
                ModelState.AddModelError("", "Only letters, comma and dot allowed.");
                return View(model);
            }

            var words = model.ReviewComment.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length < 3)
            {
                ModelState.AddModelError("", "Please enter at least 3 meaningful words.");
                return View(model);
            }

            foreach (var word in words)
            {
                if (word.Length < 2)
                {
                    ModelState.AddModelError("", "Review contains meaningless words.");
                    return View(model);
                }
            }

            await objbalvisitor.InsertReviewSKK(model);

            TempData["SuccessMessage"] = "Review Submitted Successfully!";

            return RedirectToAction("AddReviewSKK");
        }
        #endregion Shruti

        #region Tanuja

        //=============================================================MY BOOKINGS=====================================================\\
        [HttpGet]
        public async Task<ActionResult> MyBookingsTP()
        {
            if (Session["VisitorCode"] == null)
                return RedirectToAction("Login");

            string visitorCode = Session["VisitorCode"].ToString();

            DataSet ds = await objbalvisitor.FetchAllBookingTP(visitorCode);

            List<BookingTP> list = new List<BookingTP>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new BookingTP
                    {
                        BookingCode = row["BookingCode"].ToString(),
                        FarmHouseCode = row["FarmHouseCode"].ToString(),
                        FarmHouseName = row["FarmHouseName"].ToString(),
                        CheckinDate = Convert.ToDateTime(row["CheckinDate"]),
                        CheckoutDate = Convert.ToDateTime(row["CheckOutDate"]),
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                        StatusName = row["StatusName"].ToString()
                    });
                }
            }
            BookingTP visitor = new BookingTP();
            visitor.FarmHouseCode = Session["FarmHouseCode"]?.ToString();
            visitor.BookingCode = Session["BookingCode"]?.ToString();
            visitor.TotalAmount = Session["TotalAmount"] != null ? Convert.ToDecimal(Session["TotalAmount"]) : 0;
            return View(list);
        }

        //====================================================BOOKING DETAILS=====================================================\\
        [HttpGet]
        public async Task<ActionResult> BookingDetailsTP(string bookingCode)
        {
            if (Session["VisitorCode"] == null)
                return Content("Session Expired");

            string visitorCode = Session["VisitorCode"].ToString();

            var booking = await objbalvisitor.BookingDetailsTP(bookingCode, visitorCode, null);

            return PartialView("BookingDetailsTP", booking);
        }

        //=======================================================UPCOMING BOOKINGS TAB===============================================\\
        public async Task<ActionResult> UpcomingBookingTP()
        {
            if (Session["VisitorCode"] == null) return RedirectToAction("Login");
            string visitorCode = Session["VisitorCode"].ToString();
            ViewBag.ActiveTab = "UpcomingTripsTP";
            var bookingList = await objbalvisitor.FilterBookingsTP("UpcomingTripsTP", visitorCode);
            return View("MyBookingsTP", bookingList);
        }

        //==========================================BOOKING HISTORY TAB======================================================
        public async Task<ActionResult> BookingHistoryTP()
        {
            if (Session["VisitorCode"] == null)
                return RedirectToAction("Login");

            string visitorCode = Session["VisitorCode"].ToString();

            ViewBag.ActiveTab = "BookingHistoryTP";

            var bookingList =
                await objbalvisitor.FilterBookingsTP("BookingHistoryTP", visitorCode);

            return View("MyBookingsTP", bookingList);
        }

        //===================================================CANCELLED BOOKINGS TAB=================================================
        public async Task<ActionResult> CancelledBookingsTP()
        {
            if (Session["VisitorCode"] == null)
                return RedirectToAction("Login");

            string visitorCode = Session["VisitorCode"].ToString();

            ViewBag.ActiveTab = "CancelledBookingsTP";

            var bookingList =
                await objbalvisitor.FilterBookingsTP("CancelledBookingsTP", visitorCode);

            return View("MyBookingsTP", bookingList);
        }

        //==================================================CANCEL BOOKING============================================
        [HttpGet]
        public async Task<ActionResult> CancelBookingTP(string bookingCode)
        {
            if (Session["VisitorCode"] == null) return Content("Session Expired");
            string visitorCode = Session["VisitorCode"].ToString();

            BookingTP model = await objbalvisitor.GetRefundDetailsTP(bookingCode, visitorCode);

            return PartialView("CancelBookingTP", model);
        }

        //============================================= Invoice Details ===============================================
        [HttpGet]
        public async Task<ActionResult> GetInvoiceDetailsTP(string bookingCode)
        {
            if (Session["VisitorCode"] == null) return Content("Session Expired");
            string visitorCode = Session["VisitorCode"].ToString();
            BookingTP model = await objbalvisitor.FetchInvoiceDetailsTP(bookingCode, visitorCode);
            return PartialView("ViewInvoice", model);

        }

        //========================================================= Cancel Booking +=======================================

        [HttpPost]
        public async Task<ActionResult> ConfirmCancelBookingTP(string bookingCode, string cancelReason)
        {
            if (Session["VisitorCode"] == null) return Json(new { success = false });
            string visitorCode = Session["VisitorCode"].ToString();
            BookingTP model = await objbalvisitor.UpdateCancelStatusTP(bookingCode, visitorCode, cancelReason);
            return Json(new
            {
                success = true,
                refund = model.RefundedAmount,
                status = model.RefundStatus

            });
        }
        #endregion
    }
}




