using System.Web.Mvc;
using Repository;

namespace RAPSys.Controllers
{
    public class HelperController : Controller
    {
        readonly HelpersRepository HelpersRepository = new HelpersRepository();
        // GET: Helper
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetLocationForSelect() => Json(HelpersRepository.GetLocationForSelect(), JsonRequestBehavior.AllowGet);

        [HttpPost]
        public ActionResult GetLocations(string locationName) => Json(HelpersRepository.GetLocations(locationName), JsonRequestBehavior.AllowGet);

        [HttpPost]
        public ActionResult GetDepartmentContactPerson(string personName) => Json(HelpersRepository.GetDepartmentContactPerson(personName), JsonRequestBehavior.AllowGet);

        [HttpPost]
        public ActionResult GetCultureType(string cultureName) => Json(HelpersRepository.GetCultureType(cultureName), JsonRequestBehavior.AllowGet);

        [HttpPost]
        public ActionResult GetEmployeeByRole(string personName, string personRole) => Json(HelpersRepository.GetEmployeeByRole(personName, personRole), JsonRequestBehavior.AllowGet);

        [HttpPost]
        public ActionResult GetPAP(string personName) => Json(HelpersRepository.GetPAP(personName), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetRegionForSelect() => Json(HelpersRepository.GetRegionForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetAssetTypeForSelect() => Json(HelpersRepository.GetAssetTypeForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetVillageForSelect() => Json(HelpersRepository.GetVillageForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetRegionVillageForSelect(int RegionId) => Json(HelpersRepository.GetVillageForSelect(RegionId), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetVillageStatusForSelect() => Json(HelpersRepository.GetVillageStatusForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetSurfaceUOMForSelect() => Json(HelpersRepository.GetSurfaceUOMForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetZIStatus() => Json(HelpersRepository.GetZIStatus(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetDepartmentForSelect() => Json(HelpersRepository.GetDepartmentForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetLACForSelect() => Json(HelpersRepository.GetLACForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetRestrictedAreaForSelect() => Json(HelpersRepository.GetRestrictedAreaForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetStructureForSelect() => Json(HelpersRepository.GetStructureForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetTreeMaturityForSelect() => Json(HelpersRepository.GetTreeMaturityForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetFieldTypeForSelect() => Json(HelpersRepository.GetFieldTypeForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetMaterialsForSelect() => Json(HelpersRepository.GetMaterialsForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetPaymentPreferencesForSelect() => Json(HelpersRepository.GetPaymentPreferencesForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetVulnerabilityForSelect() => Json(HelpersRepository.GetVulnerabilityForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetStructureUsageForSelect() => Json(HelpersRepository.GetStructureUsageForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetStructureOwnerTypeForSelect() => Json(HelpersRepository.GetStructureOwnerTypeForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetListDetails(string listName) => Json(HelpersRepository.GetListDetails(listName), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetGoodTypeForSelect(string typeName) => Json(HelpersRepository.GetGoodsList(typeName), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetCultureTool() => Json(HelpersRepository.GetCultureTool(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult GetCultureDiversitiesForSelect() => Json(HelpersRepository.GetCultureDiversitiesForSelect(), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public ActionResult DownloadAttachment(int attachmentId)
        {
            var attach = HelpersRepository.AttachmentDetails(attachmentId);
            if (System.IO.File.Exists(attach.RequestAttachementPath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(attach.RequestAttachementPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, attach.RequestAttachementFile);
            }
            else
                return Json("Not found", JsonRequestBehavior.AllowGet);
        }
    }
}