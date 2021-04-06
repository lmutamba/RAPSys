using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using RapModel.ViewModel;
using RAPSys.Models.Model;
using System.Text.RegularExpressions;
using System.IO;
using IronXL;
using System.Data;
using System.Transactions;
using System.Drawing;

namespace Repository
{
    public class HelpersRepository
    {
        readonly RAPSystemEntities db = new RAPSystemEntities();
        public string loggedUser;
        public const string folderRoot = /*@"\\FGMDSSDBX01\Data\RAPSys\"; //*/@"\\fgmfp2\data\RAP\RAPSys\";

        public HelpersRepository()
        {
            loggedUser = HttpContext.Current.User.Identity.Name;
        }

        public PersonViewModel GetLogInPerson()
        {
            string login = HttpContext.Current.User.Identity.Name;
            login = login.Split('\\')[1] + "@tfm.cmoc.com";
            var person = db.T_Person.FirstOrDefault(p => p.Email == login);

            if (person !=null)
            {
                var PersonView = new PersonViewModel()
                {
                    PersonId = person.PersonId,
                    TFMID = person.T_Employee.FirstOrDefault().TFMID,
                    EmployeeId = person.T_Employee.FirstOrDefault().EmployeeId,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    MiddleName = person.MiddleName,
                    DepartmentId = person.T_Employee.FirstOrDefault().DepartmentId,
                    DepartmentName = person.T_Employee.FirstOrDefault().T_Department1.DepartmentName,
                    Email = person.Email
                };
                return PersonView;
            }
            else
            {
                return null;
            }
        }

        public PersonViewModel GetLogInPerson(string login)
        {
            login = login.Split('\\')[1] + "@tfm.cmoc.com";
            var person = db.T_Person.FirstOrDefault(p => p.Email == login);

            if (person != null)
            {
                var PersonView = new PersonViewModel()
                {
                    PersonId = person.PersonId,
                    TFMID = person.T_Employee.FirstOrDefault().TFMID,
                    EmployeeId = person.T_Employee.FirstOrDefault().EmployeeId,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    MiddleName = person.MiddleName,
                    DepartmentId = person.T_Employee.FirstOrDefault().DepartmentId,
                    DepartmentName = person.T_Employee.FirstOrDefault().T_Department1.DepartmentName,
                    Email = person.Email
                };
                return PersonView;
            }
            else
            {
                return null;
            }
        }

        public List<LocationViewModel> GetLocationForSelect()
        {
            var location = db.T_Location.Where(l=>l.LocationId>0).Select(l => new LocationViewModel(){
                                LocationId = l.LocationId,
                                LocationName = l.LocationName
                            }).OrderBy(l => l.LocationName).ToList();

            return location;
        }

        public List<RegionViewModel> GetRegionForSelect()
        {
            var region = db.T_Region.Where(r=>r.RegionId>0).Select(r=> new RegionViewModel() { 
                RegionId = r.RegionId,
                RegionName = r.RegionName
            }).OrderBy(r=>r.RegionName).ToList();

            return region;
        }

        public List<VillageViewModel> GetVillageForSelect()
        {
            var villages = db.T_Village.Select(v => new VillageViewModel() { 
                VillageId = v.VillageId,
                VillageName = v.VillageName,
                RegionId = v.RegionId,
                RegionName = v.T_Region.RegionName
            }).OrderBy(v => v.VillageName).ToList();

            return villages;
        }

        public List<VillageViewModel> GetVillageForSelect(int RegionId)
        {
            var villages = db.T_Village.Where(v=>v.RegionId == RegionId).Select(v => new VillageViewModel()
            {
                VillageId = v.VillageId,
                VillageName = v.VillageName,
                RegionId = v.RegionId,
                RegionName = v.T_Region.RegionName
            }).OrderBy(v => v.VillageName).ToList();

            return villages;
        }

        public List<VillageViewModel> GetVillageStatusForSelect()
        {
            var village = db.T_List.Where(l=>l.ListName == "Village Status").Select(v => new VillageViewModel()
            {
                VillageStatus = v.ListId,
                VillageStatusString = v.ListValue
            }).OrderBy(v=>v.VillageStatusString).ToList();
            return village;
        }

        public List<UOMViewModel> GetSurfaceUOMForSelect()
        {
            int uomType = db.T_List.FirstOrDefault(l=>l.ListName == "UOM Type" && l.ListValue== "Surface").ListId;
            var uom = db.T_UOM.Where(u => u.UOMType == uomType).Select( u=> new UOMViewModel() 
            {
                UOMId = u.UOMId,
                UOM = u.UOM,
                UOMConversion = u.UOMConversion,
                UOMReference = u.UOMReference,
                UOMType = u.UOMType,
                UOMTypeString = u.T_List.ListValue
            }).OrderBy(u=>u.UOM).ToList();
            return uom;
        }

        public List<VillageViewModel> GetZIStatus()
        {
            var status = db.T_List.Where(l => l.ListName == "Land Status").Select(v => new VillageViewModel()
            {
                VillageStatus = v.ListId,
                VillageStatusString = v.ListValue
            }).OrderBy(v => v.VillageStatusString).ToList();
            return status;
        }

        public List<DepartmentViewModel> GetDepartmentForSelect()
        {
            var depart = db.T_Department.Where(d=> d.DepartmentId > 0).Select(d => new DepartmentViewModel()
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName//,
                //ManagerId = d.Manager,
                //ManagerName = d.T_Employee.T_Person.FirstName + " " + d.T_Employee.T_Person.LastName
            }).OrderBy(d => d.DepartmentName).ToList();
            return depart;
        }

        public List<LACViewModel> GetLACForSelect()
        {
            var lac = db.T_LAC.Where(l => !l.Locked).Select(l => new LACViewModel()
            {
                LACId = l.LACId,
                LAC_ID = l.LAC_ID
            }).OrderBy(l=>l.LAC_ID).ToList();

            return lac;
        }

        public List<RestrictedAreaViewModel> GetRestrictedAreaForSelect() 
        {
            var landCategory = db.T_List.FirstOrDefault(l=>l.ListName == "Land Category" && l.ListValue == "Restricted Zone").ListId;
            return db.T_Land.Where(l => l.LandCategory == landCategory).Select(l => new RestrictedAreaViewModel()
               {
                   LandId = l.LandId,
                   LandName = l.LandName
               }).OrderBy(l => l.LandName).ToList();
        }
        
        public List<StructureViewModel> GetStructureForSelect()
        {
            var structure = db.T_List.Where(l => l.ListName == "Structure").Select(v => new StructureViewModel()
            {
                StructureID = v.ListId,
                StructureCode = v.ListValue,
                StructureLabel = v.ListLabel
            }).OrderBy(v => v.StructureCode).ToList();
            return structure;
        }

        public List<TreeViewModel> GetTreeMaturityForSelect()
        {
            var structure = db.T_List.Where(l => l.ListName == "Tree Maturity").Select(t => new TreeViewModel()
            {
                TreeMaturity = t.ListId,
                TreeMaturityName = t.ListValue,
                TreeMaturityLabel = t.ListLabel
            }).OrderBy(t => t.TreeMaturityName).ToList();
            return structure;
        }

        public List<CultureViewModel> GetFieldTypeForSelect()
        {
            var culture = db.T_List.Where(l => l.ListName == "Field Type").Select(c => new CultureViewModel() 
            {
                CultureId = c.ListId,
                CultureType = c.ListValue
            }).OrderBy(c=> c.CultureType).ToList();
            return culture;
        }

        public List<MaterialsViewModel> GetMaterialsForSelect()
        {
            var structure = db.T_List.Where(l => l.ListName == "Materiels").Select(v => new MaterialsViewModel()
            {
                MaterialsID = v.ListId,
                MaterialsCode = v.ListValue,
                MaterialsLabel = v.ListLabel
            }).OrderBy(v => v.MaterialsCode).ToList();
            return structure;
        }

        public List<ListViewModel> GetListDetails(string listName) => db.T_List.Where(l => l.ListName == listName).Select(v => new ListViewModel()
        {
            ListId = v.ListId,
            ListName = v.ListName,
            ListValue = v.ListValue,
            ListLabel = v.ListLabel
        }).OrderBy(v => v.ListValue).ToList();

        public List<CultureToolViewModel> GetCultureTool() => db.T_CultureTool.Select(c => new CultureToolViewModel() {
            CultureToolId = c.CultureToolId,
            CultureToolType = c.CultureToolType
        }).OrderBy(c => c.CultureToolType).ToList();

        public List<CultureDiversityViewModel> GetCultureDiversitiesForSelect() => db.T_CultureDiversity.Select(d => new CultureDiversityViewModel()
        {
            CultureDiversityId = d.CultureDiversityId,
            CultureDiversityName = d.Diversity
        }).OrderBy(d => d.CultureDiversityName).ToList();

        public List<ListViewModel> GetGoodsList(string type)
        {
            int goodType = db.T_List.FirstOrDefault(l=>l.ListName == "Good Type" && l.ListValue == type).ListId;
            return db.T_Good.Where(l => l.GoodType == goodType).Select(v => new ListViewModel()
            {
                ListId = v.GoodId,
                ListName = v.T_List.ListValue,
                ListValue = v.Good
            }).OrderBy(v => v.ListValue).ToList();

        }

        public List<PaymentPreferenceViewModel> GetPaymentPreferencesForSelect()
        {
            var preferences = db.T_List.Where(l=>l.ListName == "Payment Preference").Select(l => new PaymentPreferenceViewModel() 
            {
                PreferenceId = l.ListId,
                PreferenceLabel = l.ListLabel,
                PreferenceName = l.ListValue
            }).OrderBy(l => l.PreferenceName).ToList();
            return preferences;
        }

        public List<VulnerabilityViewModel> GetVulnerabilityForSelect()
        {
            var vulnerability = db.T_List.Where(l => l.ListName == "Vulnerability").Select(v => new VulnerabilityViewModel() { 
                VulnerabilityId = v.ListId,
                VulnerabilityLabel = v.ListLabel,
                VulnerabilityName = v.ListValue
            }).OrderBy(v => v.VulnerabilityName).ToList();
            return vulnerability;
        }

        public List<AssetTypeViewModel> GetAssetTypeForSelect()
        {
            var asset = db.T_AssetType.Select(a => new AssetTypeViewModel() { 
                AssetTypeId = a.AssetTypeId,
                AssetName = a.AssetName
            }).OrderBy(a => a.AssetName).ToList();
            return asset;
        }

        public List<StructureViewModel> GetStructureUsageForSelect()
        {
            var structure = db.T_List.Where(l => l.ListName == "Structure Usage").Select(v => new StructureViewModel()
            {
                StructureID = v.ListId,
                StructureUsageCode = v.ListValue,
                StructureUsageLabel = v.ListLabel
            }).OrderBy(v => v.StructureUsageCode).ToList();
            return structure;
        }

        public List<StructureViewModel> GetStructureOwnerTypeForSelect()
        {
            var structure = db.T_List.Where(l => l.ListName == "Owner Type").Select(v => new StructureViewModel()
            {
                StructureID = v.ListId,
                StructureOwnerCode = v.ListValue,
                StructureOwnerLabel = v.ListLabel
            }).OrderBy(v => v.StructureOwnerCode).ToList();
            return structure;
        }

        public IEnumerable<LocationViewModel> GetLocations(string locationName)
        {
            var location = db.T_Location.Where(l => l.LocationId > 0 && l.LocationName.Contains(locationName)).Select(l => new LocationViewModel()
            {
                LocationName = l.LocationName
            });
            int vStatus = db.T_List.FirstOrDefault(l=>l.ListName == "Village Status" && l.ListValue == "Existing").ListId;
            location = location.Union(db.T_Village.Where(v => v.VillageStatus == vStatus && v.VillageName.Contains(locationName)).Select(v => new LocationViewModel() { 
                LocationName = v.VillageName
            }));

            return location.Distinct().OrderBy(l => l.LocationName);
        }

        public IEnumerable<PersonViewModel> GetDepartmentContactPerson(string personName)
        {
            int dept = GetLogInPerson().DepartmentId;
            var employeeId = db.T_Employee.Where(e => e.DepartmentId == dept).Select(e => e.PersonId).ToList();
            var person = db.T_Person.Where(p => employeeId.Contains(p.PersonId) && p.PersonId > 0 && (p.FirstName.Contains(personName) || p.LastName.Contains(personName) || p.MiddleName.Contains(personName))).Select(pv => new PersonViewModel() {
                PersonId = pv.PersonId,
                TFMID = pv.T_Employee.FirstOrDefault().TFMID,
                EmployeeId = pv.T_Employee.FirstOrDefault().EmployeeId,
                FirstName = pv.FirstName,
                LastName = pv.LastName,
                MiddleName = pv.MiddleName,
                DepartmentId = pv.T_Employee.FirstOrDefault().DepartmentId,
                DepartmentName = pv.T_Employee.FirstOrDefault().T_Department1.DepartmentName
            });

            return person;
        }

        public IEnumerable<CultureViewModel> GetCultureType(string cultureName)
        {
            var culture = db.T_CultureDiversity.Where(c => c.Diversity.Contains(cultureName)).Select(c=> new CultureViewModel() { 
                CultureId = c.CultureDiversityId,
                CultureName = c.Diversity,
                CultureType = c.Diversity
            }).OrderBy(c=>c.CultureName);
            return culture;
        }

        public IEnumerable<PersonViewModel> GetEmployeeByRole(string personName, string personRole)
        {
            int _role = db.T_List.FirstOrDefault(l=> l.ListName == "Role" && l.ListValue == personRole).ListId;
            var surveyors = db.T_PersonRole.Where(r => r.Role == _role).Select(r => r.PersonId).ToList();
            var person = db.T_Employee.Where(p => surveyors.Contains(p.PersonId) && (p.T_Person.FirstName.Contains(personName) || p.T_Person.LastName.Contains(personName) || p.EmployeeCode.Contains(personName)) && !string.IsNullOrEmpty(p.EmployeeCode)).Select(p => new PersonViewModel()
            {
                PersonId = p.PersonId,
                EmployeeCode = p.EmployeeCode,
                TFMID = p.TFMID,
                EmployeeId = p.EmployeeId,
                FirstName = p.T_Person.FirstName,
                LastName = p.T_Person.LastName,
                MiddleName = p.T_Person.MiddleName,
                DepartmentId = p.DepartmentId,
                DepartmentName = p.T_Department1.DepartmentName
            });

            return person;
        }

        public IEnumerable<PersonViewModel> GetPAP(string firstName)
        {
            var person = db.T_Person.Where(p => (p.FirstName.Contains(firstName) || p.LastName.Contains(firstName) || p.MiddleName.Contains(firstName)) && (p.PersonType == "PAP" || p.PersonType == "Household")).Select(p => new PersonViewModel()
            {
                PersonId = p.PersonId,
                FirstName = p.FirstName,
                LastName = p.LastName,
                MiddleName = p.MiddleName
            });

            return person;
        }

        public AttachmentViewModel AttachmentDetails(int attachmentId)
        {
            var attachment = db.T_Attachment.Where(a => a.AttachmentId == attachmentId).Select(a => new AttachmentViewModel()
            {
                AttachmentId = a.AttachmentId,
                RequestAttachementType = a.RequestAttachementType,
                RequestAttachementContentType = a.RequestAttachementContentType,
                RequestAttachementFile = a.RequestAttachementFile,
                RequestAttachementPath = a.RequestAttachementPath,
                AttachmentType = a.T_List.ListValue,
                RequestId = a.T_RequestAttachment.FirstOrDefault().RequestId
            }).FirstOrDefault();
            return attachment;
        }

        public List<PointViewModel> ReadPointFiles(HttpPostedFileBase file, int LandId=-1)
        {
            List<PointViewModel> Points = new List<PointViewModel>();
            if (file != null)
            {
                string fileExtension = Path.GetExtension(file.FileName);
                try
                {
                    if (fileExtension == ".csv")
                    {
                        using (var reader = new StreamReader(file.InputStream))
                        {
                            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                            string[] headers = CSVParser.Split(reader.ReadLine());

                            while (!reader.EndOfStream)
                            {
                                string[] rows = CSVParser.Split(reader.ReadLine());
                                Points.Add(new PointViewModel()
                                {
                                    PointName = rows[0].ToString(),
                                    Latitude = decimal.Parse(rows[2].ToString()),
                                    Longitude = decimal.Parse(rows[3].ToString()),
                                    Elevation = decimal.Parse(rows[4].ToString()),
                                    LandId = LandId
                                });
                            }
                            reader.Close();
                        }
                    }
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string path = HttpContext.Current.Server.MapPath("~/") + file.FileName;
                        file.SaveAs(path);
                        WorkBook workBook = WorkBook.Load(path);
                        WorkSheet sheet = workBook.WorkSheets.First();
                        string cellValue = sheet["A1"].ToString();
                        if (cellValue == "Name")
                        {
                            for (int i = 1; i < sheet.RowCount; i++)
                            {
                                Points.Add(new PointViewModel()
                                {
                                    PointName = sheet.Rows[i].Columns[0].Value.ToString(),
                                    Latitude = decimal.Parse(sheet.Rows[i].Columns[1].Value.ToString()),
                                    Longitude = decimal.Parse(sheet.Rows[i].Columns[2].Value.ToString()),
                                    Elevation = decimal.Parse(sheet.Rows[i].Columns[3].Value.ToString())
                                });
                            }
                        }
                        if(File.Exists(path))
                            File.Delete(path);
                    }
                }
                catch (Exception)
                {
                    return Points;
                }
            }
            return Points;
        }

        public string SaveAttachment(HttpPostedFileBase file, string ObjectType, int ObjectID, int LacId=-1)
        {
            string listValue = "";
            string listPath = "";
            switch (ObjectType)
            {
                case "Land Request":
                    listValue = "Land Request";
                    listPath = "LR";
                    break;
                case "PAP":
                    listValue = "PAP";
                    listPath = "PAP";
                    break;
                case "PAPLAC":
                    listValue = "PAPLAC";
                    listPath = "PAPLAC";
                    break;
                case "HouseHold":
                    listValue = "Household";
                    listPath = @"HouseHold\LAC" +LacId.ToString();
                    break;
            }

            using(TransactionScope scope = new TransactionScope())
            {
                string serverPath = folderRoot + listPath + @"\" + ObjectID.ToString() + @"\";
                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);
                string fileName = Path.GetFileName(file.FileName);
                serverPath = Path.Combine(serverPath, fileName);
                try
                {
                    if (file != null)
                    {
                        var attachs = new T_Attachment()
                        {
                            RequestAttachementType = db.T_List.FirstOrDefault(l => l.ListName == "Attachment Type" && l.ListValue == listValue).ListId,
                            RequestAttachementPath = serverPath,
                            RequestAttachementContentType = file.ContentType,
                            RequestAttachementFile = file.FileName,
                            Created = DateTime.Now,
                            CreatedBy = loggedUser,
                            Updated = DateTime.Now,
                            UpdatedBy = loggedUser
                        };
                        db.T_Attachment.Add(attachs);
                        db.SaveChanges();
                        file.SaveAs(serverPath);
                        var attachementId = attachs.AttachmentId;

                        if (ObjectType == "PAP")
                        {
                            db.T_PAPAttachment.Add(new T_PAPAttachment()
                            {
                                PAPId = ObjectID,
                                AttachmentId = attachementId,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });
                        }
                        else if (ObjectType == "PAPLAC")
                        {
                            db.T_PAPLACAttachment.Add(new T_PAPLACAttachment()
                            {
                                PAPLACId = ObjectID,
                                AttachmentId = attachementId,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });
                        }
                        else if (ObjectType == "Land Request")
                        {
                            db.T_RequestAttachment.Add(new T_RequestAttachment()
                            {
                                RequestId = ObjectID,
                                AttachmentId = attachementId,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });
                        }
                        else if (ObjectType == "HouseHold")
                        {
                            db.T_HouseHoldAttachment.Add(new T_HouseHoldAttachment() {
                                HouseholdId = ObjectID,
                                AttachmentId = attachementId,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });
                        }
                        db.SaveChanges();
                    }
                    scope.Complete();
                    return "success";
                }
                catch (Exception e)
                {
                    if (File.Exists(serverPath))
                        File.Delete(serverPath);
                    return "Error saving " + fileName + " with " + e.Message;
                }
            }
        }

        public static string GetImageBase64(string PhotoPath)
        {
            if (!string.IsNullOrEmpty(PhotoPath) && File.Exists(PhotoPath))//
            {
                Image img = Image.FromFile(PhotoPath);
                bool IsJpeg = (PhotoPath.EndsWith(".jpg") || PhotoPath.EndsWith(".jpeg"));

                byte[] photo = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, IsJpeg ? System.Drawing.Imaging.ImageFormat.Jpeg : System.Drawing.Imaging.ImageFormat.Png);
                    photo = ms.ToArray();
                }

                string _return;
                if (IsJpeg)
                {
                    _return = string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(photo, 0, photo.Length));
                }
                else
                {
                    _return = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photo, 0, photo.Length));
                }
                return _return;
            }

            return string.Empty;
        }

        public void SendEmail(EmailNotificationViewModel email)
        {
            MailMessage msg = new MailMessage();

            email.FromEmail = "rapsys@tfm.cmoc.com";
            email.FromName = "RAP System";

            foreach (var to in email.To)
            {
                msg.To.Add(new MailAddress(to, ""));
            }

            foreach (var bcc in email.Bcc)
            {
                msg.Bcc.Add(new MailAddress(bcc, ""));
            }

            foreach (var cc in email.Cc)
            {
                msg.Bcc.Add(new MailAddress(cc, ""));
            }

            msg.From = new MailAddress(email.FromEmail, email.FromName);
            msg.Subject = email.Subject;
            msg.Body = email.Body;
            msg.IsBodyHtml = true;

            if (email.Attachment != null)
            {
                foreach (var att in email.Attachment)
                {
                    msg.Attachments.Add(att);
                }
            }

            SmtpClient client = new SmtpClient
            {
                UseDefaultCredentials = false,
                Port = 25,
                Host = "webmail.cmoctfm.com",//"FGMPMGMXX01.CMOCTFM.COM",
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                System.Security.Cryptography.X509Certificates.X509Chain chain,
                System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
                client.Send(msg);
            }
            catch (Exception e)
            {
                string ex = e.Message;
            }
        }
    }
}
