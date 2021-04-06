using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RapModel.ViewModel;
using Repository;

namespace RAPSys.Controllers
{
    public class LandManagementController : Controller
    {
        readonly LandRequestRepository RequestRepository = new LandRequestRepository();
        readonly HelpersRepository Helper = new HelpersRepository();

        // GET: LandManagement

        #region GET VIEWS
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyRequest()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(HttpContext.User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-REQ") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
                return View();
            else
                return View("~/Views/Error/Page401.cshtml");  
        }

        public ActionResult MyApproval()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-APP"))
                return View();
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult LandStatus()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
                return View();
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult LacList()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV"))
                return View();
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult LandImpactMitigation()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
                return View();
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult LandRequestMitigation()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
                return View();
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult RestrictedZones()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV"))
                return View();
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult Ressetlment() => View("~/Views/Error/PageUnderConstruction.cshtml");

        #endregion

        #region CRUD
        [HttpPost]
        public ActionResult CreateLandRequest() 
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-REQ") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO")) 
            {
                string ProjectName = Request["ProjectName"];
                string ProjectCostCode = Request["ProjectCostCode"];
                string WorkDescription = Request["WorkDescription"];
                int RegionId = int.Parse(Request["RegionId"]);
                string RegionName = Request["RegionName"];
                string _date = Request["AccessScheduledDate"];
                DateTime AccessScheduledDate = DateTime.Parse(_date);
                bool IsUrgent = bool.Parse(Request["IsUrgent"]);
                string ContactPerson = Request["ContactPerson"];

                LandRequestViewModel LandRequest = new LandRequestViewModel()
                {
                    ProjectName = ProjectName,
                    ProjectCostCode = ProjectCostCode,
                    WorkDescription = WorkDescription,
                    AccessScheduledDate = AccessScheduledDate,
                    IsUrgent = IsUrgent,
                    ContactPerson = ContactPerson,
                    RegionId = RegionId,
                    RegionName = RegionName
                };

                HttpFileCollectionBase files = Request.Files;
                if (files != null)
                {
                    List<HttpPostedFileBase> upload = new List<HttpPostedFileBase>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        upload.Add(file);
                    }
                    LandRequest.Attachments = upload.ToArray();
                    return Json(RequestRepository.CreateLandRequest(LandRequest, upload.ToArray()), JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        [HttpPost]
        public ActionResult UpdateLandRequest()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-REQ"))
            {
                int LacRequestID = int.Parse(Request["LacRequestID"]);
                string ProjectName = Request["ProjectName"];
                int RegionId = int.Parse(Request["RegionId"]);
                string RegionName = Request["RegionName"];
                string ProjectCostCode = Request["ProjectCostCode"];
                string WorkDescription = Request["WorkDescription"];
                string _date = Request["AccessScheduledDate"];
                DateTime AccessScheduledDate = DateTime.Parse(_date);
                bool IsUrgent = bool.Parse(Request["IsUrgent"]);
                string ContactPerson = Request["ContactPerson"];

                LandRequestViewModel LandRequest = new LandRequestViewModel()
                {
                    LACRequestId = LacRequestID,
                    ProjectName = ProjectName,
                    RegionId = RegionId,
                    ProjectCostCode = ProjectCostCode,
                    WorkDescription = WorkDescription,
                    AccessScheduledDate = AccessScheduledDate,
                    IsUrgent = IsUrgent,
                    ContactPerson = ContactPerson,
                    RegionName = RegionName
                };

                HttpFileCollectionBase files = Request.Files;
                if (files != null)
                {
                    List<HttpPostedFileBase> upload = new List<HttpPostedFileBase>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        upload.Add(file);
                    }

                    LandRequest.Attachments = upload.ToArray();
                    return Json(RequestRepository.UpdateLandRequest(LandRequest, upload.ToArray()), JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        [HttpPost]
        public ActionResult ApproveRejectRequest(string type, int RequestID, string comment) 
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-APP") || Roles.IsUserInRole(User.Identity.Name, "SG-FGM-RAP-LO"))
                return Json(RequestRepository.ApproveRejectRequest(type, RequestID, comment), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult CancelRequest(int LacRequestId) 
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-REQ"))
                return Json(RequestRepository.CancelLandRequest(LacRequestId), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult CreateLAC(LACViewModel LacViewModel) => Json(RequestRepository.CreateLAC(LacViewModel), JsonRequestBehavior.AllowGet);

        public ActionResult MakeRestricted(LandRequestViewModel Land)
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                return Json(RequestRepository.MakeRestricted(Land), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult ForwardImpactMitigation()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                HttpFileCollectionBase files = Request.Files;
                int RequestID = int.Parse(Request["RequestID"]);
                string Comment = Request["Comments"];

                List<PointViewModel> Points = new List<PointViewModel>();

                if (files != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (Path.GetExtension(file.FileName) == ".csv")
                        {
                            Points = Helper.ReadPointFiles(file);
                        }
                        else
                        {
                            List<string> error = new List<string> { "error", "File extension is not valid" };
                            return Json(error, JsonRequestBehavior.AllowGet);
                        }
                    } 
                }
                return Json(RequestRepository.ForwardImpactMitigation(RequestID, Comment, Points), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult SendRequestor()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                HttpFileCollectionBase files = Request.Files;
                int RequestID = int.Parse(Request["RequestID"]);
                string Comment = Request["Comments"];

                List<HttpPostedFileBase> upload = new List<HttpPostedFileBase>();
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    upload.Add(file);
                }

                return Json(RequestRepository.SendRequestor(RequestID, Comment, upload.ToArray()), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult ImpactMitigation(int RequestID, string Comment) => Json(RequestRepository.ImpactMitigation(RequestID, Comment), JsonRequestBehavior.AllowGet);

        public ActionResult GrantGreenLight() 
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                int RequestID = int.Parse(Request["RequestID"]);
                string Comment = Request["Comments"];
                HttpFileCollectionBase files = Request.Files;
                List<PointViewModel> Points = new List<PointViewModel>();
                if (files != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (Path.GetExtension(file.FileName) == ".csv")
                        {
                            Points = Helper.ReadPointFiles(file);
                        }
                        else
                        {
                            List<string> error = new List<string> { "error", "File extension is not valid" };
                            return Json(error, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(RequestRepository.GrantGreenLight(RequestID, Comment, Points), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult UploadGPSFile()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                HttpFileCollectionBase files = Request.Files;
                int LandId = int.Parse(Request["LandId"]);

                List<PointViewModel> Points = new List<PointViewModel>();
                HttpPostedFileBase file;

                if (files != null)
                {
                    file = files[0];
                    Points = Helper.ReadPointFiles(file, LandId);
                }

                return Json(RequestRepository.InsertPoint(Points), JsonRequestBehavior.AllowGet);
            }
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult UpdatePoint(PointViewModel points) 
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV"))
                return Json(RequestRepository.UpdatePoint(points), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult DeletePoint(PointViewModel points)
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO"))
                return Json(RequestRepository.DeletePoint(points), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult AddLandImpactedVillage(VillageViewModel village)
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV"))
                return Json(RequestRepository.AddLandImpactedVillage(village), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult RemoveLandImpactedVillage(VillageViewModel village)
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV"))
                return Json(RequestRepository.RemoveLandImpactedVillage(village), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult CreateNewVillage(VillageViewModel village)
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-INV"))
                return Json(RequestRepository.CreateNewVillage(village), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult AddAttachment()
        {
            int LandId = int.Parse(Request["LandId"]);
            int LacId = int.Parse(Request["LacId"]);

            AttachmentViewModel attachments = new AttachmentViewModel()
            {
                LandId = LandId,
                LacId = LacId
            };

            HttpFileCollectionBase files = Request.Files;
            if (files != null)
            {
                List<HttpPostedFileBase> upload = new List<HttpPostedFileBase>();
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    upload.Add(file);
                }
                return Json(RequestRepository.AddAttachement(attachments, upload.ToArray()), JsonRequestBehavior.AllowGet);
            }
            else
                return Json("Error", JsonRequestBehavior.AllowGet);
        }

        #region RESTRICTED AREA
        public ActionResult AddNewRestrictedArea(RestrictedAreaViewModel restrictedArea)
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") ||Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
                return Json(RequestRepository.AddNewRestrictedArea(restrictedArea), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult ChangeRAStatus(int LandId)
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
                return Json(RequestRepository.ChangeRAStatus(LandId), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult AddImpactedLAC (LACViewModel Lac)
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
                return Json(RequestRepository.AddImpactedLAC(Lac), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }

        public ActionResult RemoveImpactedLAC(LACViewModel Lac)
        {
            if (Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(User.Identity.Name, "CMOCTFM\\SG-FGM-RAP-LO"))
                return Json(RequestRepository.RemoveImpactedLAC(Lac), JsonRequestBehavior.AllowGet);
            else
                return View("~/Views/Error/Page401.cshtml");
        }
        #endregion

        #endregion

        #region LOAD DATA TO VIEWS
        public ActionResult LoadLandRequest() => Json(RequestRepository.LoadLandRequests(), JsonRequestBehavior.AllowGet);

        public ActionResult LoadMyApproval() => Json(RequestRepository.LoadMyApproval(), JsonRequestBehavior.AllowGet);

        public ActionResult LoadApprovedLandRequest() => Json(RequestRepository.LoadApprovedLandRequest(), JsonRequestBehavior.AllowGet);

        public ActionResult LoadLandForImpactMitigation() => Json(RequestRepository.LoadLandForImpactMitigation(), JsonRequestBehavior.AllowGet);

        public ActionResult LoadLACList() => Json(RequestRepository.LoadLACList(), JsonRequestBehavior.AllowGet);

        public ActionResult LoadRestrictedArea() => Json(RequestRepository.LoadRestrictedArea(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetLandRequest(int landRequestID) => Json(RequestRepository.GetLandRequest(landRequestID), JsonRequestBehavior.AllowGet);

        public ActionResult RequestDetails(int? id) => View(RequestRepository.GetLandRequest(id.Value));

        public ActionResult LoadLandPoint(int LacRequestId) => Json(RequestRepository.LoadLandPoint(LacRequestId), JsonRequestBehavior.AllowGet);

        public ActionResult LoadLandVillage(int LacRequestId) => Json(RequestRepository.LoadLandVillage(LacRequestId), JsonRequestBehavior.AllowGet);

        public ActionResult LoadLacDetails(int id) => View(RequestRepository.LoadLacDetails(id));

        public ActionResult RestrictedZoneDetails(int? id) => View(RequestRepository.RestrictedAreaDetails(id.Value));

        public ActionResult LoadImpactedLAC(int LandId) => Json(RequestRepository.LoadImpactedLAC(LandId), JsonRequestBehavior.AllowGet);

        public ActionResult LoadIncludingRestrictedArea(int LacID) => Json(RequestRepository.LoadIncludingRestrictedArea(LacID), JsonRequestBehavior.AllowGet);

        public ActionResult LoadLandAttachement(int LandID, string Source) => Json(RequestRepository.LoadLandAttachement(LandID, Source), JsonRequestBehavior.AllowGet);

        public ActionResult RemoveAttachement(AttachmentViewModel Attachs) => Json(RequestRepository.RemoveAttachement(Attachs), JsonRequestBehavior.AllowGet);

        #endregion
    }
}