using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using RapModel.ViewModel;
using Repository;

namespace RAPSys.Controllers
{
    public class CommunityController : Controller
    {
        readonly CommunityRepository Community = new CommunityRepository();
        // GET: Community

        #region VIEWS
        public ActionResult Index() => View();

        public ActionResult Audit() => View("~/Views/Error/PageUnderConstruction.cshtml");

        public ActionResult Paps() => View();

        public ActionResult Properties() => View();

        public ActionResult PapLAC() => View();
        #endregion

        #region LOAD DATA
        public ActionResult PapDetails(int? id) => View(Community.GetPAPDetails(id.Value));

        public ActionResult CollectLand(int? id) => View(Community.GetPAPDetails(id.Value));

        public ActionResult LoadPapDetails(int PapID) => Json(Community.GetPAPDetails(PapID), JsonRequestBehavior.AllowGet);

        public ActionResult LoadPapDetailsLac(int PersonID, int LacID) => Json(Community.GetPAPDetails(PersonID, LacID), JsonRequestBehavior.AllowGet);

        public ActionResult LoadPAPLAC(int LacID) => Json(Community.LoadPAP(LacID), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult LoadPapProperties(int papID)
        {
            var papLac = new PAPViewModel() {PAPId=papID };
            return Json(Community.LoadProperties(papLac), JsonRequestBehavior.AllowGet); 
        }

        [HttpGet]
        public ActionResult LoadPapLacProperties(int papID, int lacID) => Json(Community.LoadProperties(new PAPViewModel() { PAPId = papID }, lacID), JsonRequestBehavior.AllowGet);

        public ActionResult LoadPAP() => Json(Community.LoadPAP(), JsonRequestBehavior.AllowGet);

        public ActionResult LoadLACPAP(int PapID) => Json(Community.LoadPAPLAC(PapID), JsonRequestBehavior.AllowGet);

        public ActionResult LoadLACPAPList() => Json(Community.LoadPAPLAC(), JsonRequestBehavior.AllowGet);

        public ActionResult LoadProperties() => Json(Community.LoadProperties(), JsonRequestBehavior.AllowGet);

        public ActionResult PropertyDetail(int? id) => View(Community.LoadProperties(id.Value));

        public ActionResult LoadPropertyDetails(int PropertyID) => Json(Community.LoadProperties(PropertyID), JsonRequestBehavior.AllowGet);

        #endregion

        #region CRUD

        #region LAND MANAGMENT
        public ActionResult AddPAPLAC()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV"))
            {
                HttpFileCollectionBase files = Request.Files;
                var JsonPap = Request["PAPLAC"];
                var Properties = Request["Properties"];
                var Pap = JsonConvert.DeserializeObject<PAPViewModel>(JsonPap);
                var PapProperties = JsonConvert.DeserializeObject<List<PropertiesViewModel>>(Properties);
                Pap.Properties = PapProperties.ToArray();

                List<HttpPostedFileBase> upload = new List<HttpPostedFileBase>();

                if (files != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        upload.Add(file);
                    }
                }
                return Json(Community.AddPAPLAC(Pap, upload.ToArray()), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult LinkPAPLAC(PAPViewModel pap) => Json(Community.AddPAPLAC(pap), JsonRequestBehavior.AllowGet);

        public ActionResult AddPAP()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO"))
            {
                HttpFileCollectionBase files = Request.Files;
                var addPapToLac = Convert.ToBoolean(Request["addPapToLac"]);
                var isUpdate = Convert.ToBoolean(Request["isUpdate"]);
                var JsonPap = Request["PAPLAC"];
                var Pap = JsonConvert.DeserializeObject<PAPViewModel>(JsonPap);

                List<HttpPostedFileBase> upload = new List<HttpPostedFileBase>();

                if (files != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        upload.Add(file);
                    }
                    Pap.PAPFile = upload.ToArray();
                }
                return Json(Community.AddPAP(Pap, addPapToLac, isUpdate), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");

        }

        public ActionResult AddPAPProperty()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO"))
            {
                HttpFileCollectionBase files = Request.Files;
                var JsonPap = Request["PAPLAC"];
                var Properties = Request["Properties"];
                var Pap = JsonConvert.DeserializeObject<PAPViewModel>(JsonPap);
                var PapProperties = JsonConvert.DeserializeObject<List<PropertiesViewModel>>(Properties);
                Pap.Properties = PapProperties.ToArray();

                List<HttpPostedFileBase> upload = new List<HttpPostedFileBase>();

                if (files != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        upload.Add(file);
                    }
                }
                return Json(Community.AddPAPProperty(Pap, upload.ToArray()), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult UnlinkPAPLAC(PAPViewModel pap) => Json(Community.UnlinkPAPLAC(pap), JsonRequestBehavior.AllowGet);

        public ActionResult UnlinkPAPProperty(PAPViewModel pap) => Json(Community.UnlinkPAPProperty(pap), JsonRequestBehavior.AllowGet);

        public ActionResult PAPLACSurvey()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV"))
            {
                HttpFileCollectionBase files = Request.Files;
                var jsonPap = Request["PAPLAC"];
                var jsonHHMember = Request["Members"];
                var jsonHHRevenue = Request["Revenues"];
                var jsonHHResidences = Request["Residences"];
                var jsonHHCultures = Request["Culture"];
                var jsonHHGoods = Request["Goods"];
                var jsonHHProperty = Request["Properties"];
                var jsonHHCompensation = Request["Compensations"];
                var jsonHH = Request["Household"];
                var Pap = JsonConvert.DeserializeObject<PAPViewModel>(jsonPap);
                var hh = JsonConvert.DeserializeObject<HouseholdViewModel>(jsonHH);
                var hhMembers = JsonConvert.DeserializeObject<List<HouseholdMembersViewModel>>(jsonHHMember);
                var hhRevenues = JsonConvert.DeserializeObject<List<RevenueViewModel>>(jsonHHRevenue);
                var hhResidences = JsonConvert.DeserializeObject<List<ResidenceViewModel>>(jsonHHResidences);
                var hhCultures = JsonConvert.DeserializeObject<List<HouseholdCultureViewModel>>(jsonHHCultures);
                var hhGoods = JsonConvert.DeserializeObject<List<HouseHoldGoodViewModel>>(jsonHHGoods);
                var hhProperties = JsonConvert.DeserializeObject<List<HouseHoldPropertyViewModel>>(jsonHHProperty);
                var hhCompensations = JsonConvert.DeserializeObject<List<HouseHoldPreviousCompensationViewModel>>(jsonHHCompensation);

                hh.PAPs = Pap;
                hh.HouseholdMembers = hhMembers;
                hh.HouseholdRevenues = hhRevenues;
                hh.HouseholdResidences = hhResidences;
                hh.HouseholdCultures = hhCultures;
                hh.HouseholdGoods = hhGoods;
                hh.HouseholdProperties = hhProperties;
                hh.HouseholdCompensations = hhCompensations;

                List<HttpPostedFileBase> upload = new List<HttpPostedFileBase>();

                if (files != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        upload.Add(file);
                    }
                }
                return Json(Community.PAPLACSurvey(hh, upload.ToArray()), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult CollectLandData()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO"))
            {
                HttpFileCollectionBase files = Request.Files;
                var JsonPap = Request["PAPLAC"];
                var Properties = Request["Properties"];
                var Pap = JsonConvert.DeserializeObject<PAPViewModel>(JsonPap);
                var PapProperties = JsonConvert.DeserializeObject<List<PropertiesViewModel>>(Properties);
                Pap.Properties = PapProperties.ToArray();

                List<HttpPostedFileBase> upload = new List<HttpPostedFileBase>();

                if (files != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        upload.Add(file);
                    }
                }
                return Json(Community.CollectLandData(Pap, upload.ToArray()), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }
        #endregion
        #endregion

    }
}