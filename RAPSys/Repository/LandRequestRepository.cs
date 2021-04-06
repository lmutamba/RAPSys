using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;
using RapModel.ViewModel;
using RAPSys.Models.Model;
using System.Transactions;
using System.Web.Security;

namespace Repository
{
    public class LandRequestRepository
    {
        readonly HelpersRepository Helpers = new HelpersRepository();
        readonly RAPSystemEntities db = new RAPSystemEntities();
        private const string folderRoot = HelpersRepository.folderRoot;
        readonly PersonViewModel _person;
        private readonly string loggedUser;

        public LandRequestRepository()
        {
            loggedUser = Helpers.loggedUser;
            _person = Helpers.GetLogInPerson();
        }

        #region CRUD LAND REQUEST
        public List<string> CreateLandRequest(LandRequestViewModel LandRequest, HttpPostedFileBase[] upload)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-REQ") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                if (LandRequest != null)
                {
                    //Validate fields and values
                    if (string.IsNullOrWhiteSpace(LandRequest.ProjectName))
                        error.Add("Project Name can not be empty");
                    if (string.IsNullOrWhiteSpace(LandRequest.WorkDescription))
                        error.Add("Project Description can not be empty");
                    if (string.IsNullOrWhiteSpace(LandRequest.ContactPerson))
                        error.Add("Contact Person can't be empty or null");
                    if (LandRequest.AccessScheduledDate < DateTime.Today)
                        error.Add("Project Access date is less than today");
                    if (string.IsNullOrWhiteSpace(LandRequest.ProjectCostCode))
                        error.Add("Please provide the Project Cost Code");

                    if (error.Count > 1)
                        return error;
                    else
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            try
                            {
                                //Get contact person detail
                                string contactEmployeeID = LandRequest.ContactPerson.Split('(')[1];
                                contactEmployeeID = contactEmployeeID.Substring(0, contactEmployeeID.Length - 1);
                                LandRequest.ContactPersonId = db.T_Employee.FirstOrDefault(e => e.TFMID == contactEmployeeID).EmployeeId;
                                LandRequest.RegionName = db.T_Region.FirstOrDefault(l => l.RegionId == LandRequest.RegionId).RegionName;

                                //Insert into T_Request
                                var request = new T_Request()
                                {
                                    RequestType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId,
                                    RequestStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Request Submitted").ListId,
                                    RequestedDate = DateTime.Now,
                                    Requestor = Helpers.GetLogInPerson().EmployeeId,
                                    CurrentStep = 0,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_Request.Add(request);
                                db.SaveChanges();
                                int requestID = request.RequestId;

                                //Insert into T_LACRequest
                                var lacRequest = new T_LACRequest()
                                {
                                    Requestid = requestID,
                                    ProjectName = LandRequest.ProjectName,
                                    ProjectCostCode = LandRequest.ProjectCostCode,
                                    WorkDescription = LandRequest.WorkDescription,
                                    AccessScheduledDate = LandRequest.AccessScheduledDate,
                                    IsUrgent = LandRequest.IsUrgent,
                                    ContactPerson = LandRequest.ContactPersonId,
                                    RegionId = LandRequest.RegionId,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_LACRequest.Add(lacRequest);
                                db.SaveChanges();
                                int category = db.T_List.FirstOrDefault(l => l.ListName == "Land Category" && l.ListValue == "Unknown").ListId;
                                int location = db.T_Location.FirstOrDefault(l => l.RegionId == LandRequest.RegionId)?.LocationId ?? -1;
                                int status = db.T_List.FirstOrDefault(l => l.ListName == "Land Status" && l.ListValue == "Active").ListId;
                                int uom = db.T_UOM.FirstOrDefault(u => u.UOM == "Ha").UOMId;

                                var land = new T_Land()
                                {
                                    LandCategory = category,
                                    LACRequestId = lacRequest.LACRequestId,
                                    LocationId = location,
                                    LandStatus = status,
                                    LandName = lacRequest.ProjectName,
                                    Surface = 0,
                                    SurfaceUOM = uom,
                                    DepartementId = _person.DepartmentId,
                                    GPS_Date = DateTime.Now,
                                    Easting = 0,
                                    Northing = 0,
                                    Comment = "",
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser,
                                    Updated = DateTime.Now,
                                    UpdatedBy = loggedUser
                                };
                                db.T_Land.Add(land);
                                db.SaveChanges();

                                //Insert into T_Attachement
                                List<int> attachementId = new List<int>();
                                List<PointViewModel> Points = new List<PointViewModel>();
                                if (upload.Length > 0)
                                {
                                    foreach (HttpPostedFileBase item in upload)
                                    {
                                        //Reading CSV file to get GPS point and save them in a List of Point
                                        string result = Helpers.SaveAttachment(item, "Land Request", requestID);
                                        if (result.StartsWith("Error"))
                                            throw new Exception();
                                        string fileExtension = Path.GetExtension(item.FileName);
                                        if (fileExtension == ".csv" || fileExtension == ".xls" || fileExtension == ".xlsx")
                                        {
                                            Points = Helpers.ReadPointFiles(item);
                                        }
                                    }

                                    if (Points.Count > 0)
                                    {
                                        foreach (var point in Points)
                                        {
                                            db.T_Point.Add(new T_Point()
                                            {
                                                PointName = point.PointName,
                                                Latitude = point.Latitude,
                                                Longitude = point.Longitude,
                                                Elevation = point.Elevation,
                                                LandId = land.LandId,
                                                Created = DateTime.Now,
                                                Updated = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                UpdatedBy = loggedUser
                                            });
                                        }
                                        db.SaveChanges();
                                    }
                                }

                                db.T_RequestLog.Add(new T_RequestLog()
                                {
                                    RequestID = request.RequestId,
                                    RequestType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId,
                                    ActionType = db.T_List.FirstOrDefault(l => l.ListName == "Action Type" && l.ListValue == "Submit Request").ListId,
                                    Comment = LandRequest.WorkDescription,
                                    Created = DateTime.Now,
                                    CreatedBy = loggedUser
                                });

                                db.SaveChanges();

                                //Send Emails
                                LandRequestNotification("Requestor", LandRequest);
                                //LandRequestNotification("Approver", LandRequest);
                                LandRequestNotification("Land", LandRequest);
                                LandRequestNotification("RAP", LandRequest);

                                success.Add("New Land request created");
                                scope.Complete();
                                return success;
                            }
                            catch (Exception e)
                            {
                                foreach (var file in upload)
                                {
                                    string serverPath = folderRoot+@"LR\";
                                    if (file != null && File.Exists(Path.Combine(serverPath, file.FileName)))
                                        File.Delete(Path.Combine(serverPath, file.FileName));
                                }
                                List<string> exception = new List<string> { "Exception", e.Message };
                                return exception;
                            }
                        }
                    }
                }
                else
                {
                    error.Add("An error occures when trying to create a land request. Try again later. If problem perists, please raise an MIS incident");
                    return error;
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> UpdateLandRequest(LandRequestViewModel LandRequest, HttpPostedFileBase[] upload)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-REQ"))
            {
                if (LandRequest != null)
                {
                    //Validate fields and values
                    if (string.IsNullOrWhiteSpace(LandRequest.ProjectName))
                        error.Add("Project Name can not be empty");
                    if (string.IsNullOrWhiteSpace(LandRequest.WorkDescription))
                        error.Add("Project Description can not be empty");
                    if (string.IsNullOrWhiteSpace(LandRequest.ContactPerson))
                        error.Add("Contact Person can't be empty or null");
                    if (LandRequest.AccessScheduledDate < DateTime.Today)
                        error.Add("Project Access date is less than today");
                    if (LandRequest.Attachments.Length < 1 || LandRequest.Attachments == null)
                        error.Add("Please provide attachement");
                    if (string.IsNullOrWhiteSpace(LandRequest.ProjectCostCode))
                        error.Add("Please provide the Project Cost Code");

                    if (error.Count > 1)
                        return error;
                    else
                    {
                        //Get contact person detail
                        string contactEmployeeID = LandRequest.ContactPerson.Split('(')[1];
                        contactEmployeeID = contactEmployeeID.Substring(0, contactEmployeeID.Length - 1);
                        LandRequest.ContactPersonId = db.T_Employee.FirstOrDefault(e => e.TFMID == contactEmployeeID).EmployeeId;
                        LandRequest.RegionName = db.T_Region.FirstOrDefault(l => l.RegionId == LandRequest.RegionId).RegionName;

                        int deptId = Helpers.GetLogInPerson().DepartmentId;
                        int approverId = db.T_Department.Find(deptId).Manager;
                        var dept = db.T_Department.Find(deptId);
                        string approverName = dept.T_Employee.T_Person.FirstName + " " + dept.T_Employee.T_Person.LastName;

                        using (TransactionScope scope = new TransactionScope())
                        {
                            List<string> fileToDelete = new List<string>();

                            try
                            {
                                //UPDATE T_LACREQUEST
                                var LacRequest = db.T_LACRequest.Find(LandRequest.LACRequestId);
                                LandRequest.RequestId = LacRequest.Requestid;
                                LacRequest.ProjectName = LandRequest.ProjectName;
                                LacRequest.ProjectCostCode = LandRequest.ProjectCostCode;
                                LacRequest.WorkDescription = LandRequest.WorkDescription;
                                LacRequest.AccessScheduledDate = LandRequest.AccessScheduledDate;
                                LacRequest.IsUrgent = LandRequest.IsUrgent;
                                LacRequest.ContactPerson = LandRequest.ContactPersonId;
                                LacRequest.Updated = DateTime.Now;
                                LacRequest.UpdatedBy = loggedUser;
                                db.Entry(LacRequest).State = EntityState.Modified;

                                //UPDATE T_REQUEST
                                var Request = db.T_Request.Find(LacRequest.Requestid);
                                Request.RequestStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Request Resubmitted").ListId;
                                Request.Updated = DateTime.Now;
                                Request.UpdatedBy = loggedUser;
                                db.Entry(Request).State = EntityState.Modified;

                                //UPDATE LAND
                                var landId = db.T_Land.FirstOrDefault(l => l.LACRequestId == LacRequest.LACRequestId).LandId;
                                var land = db.T_Land.Find(landId);
                                land.LandCategory = db.T_List.FirstOrDefault(l => l.ListName == "Land Category" && l.ListValue == "Unknown").ListId;
                                land.LACRequestId = LacRequest.LACRequestId;
                                land.LocationId = db.T_Location.FirstOrDefault(l => l.RegionId == LandRequest.RegionId).LocationId;
                                land.Updated = DateTime.Now;
                                land.UpdatedBy = loggedUser;
                                db.Entry(land).State = EntityState.Modified;

                                //GET AND DELETE ATTACHMENTS LIST
                                var requestAttachments = db.T_RequestAttachment.Where(r => r.RequestId == LandRequest.RequestId);

                                foreach (var i in requestAttachments)
                                {
                                    var att = db.T_Attachment.Find(i.AttachmentId);
                                    fileToDelete.Add(att.RequestAttachementPath);
                                    db.Entry(att).State = EntityState.Deleted;
                                }
                                db.T_RequestAttachment.RemoveRange(requestAttachments);

                                //Insert into T_Attachement
                                List<PointViewModel> Points = new List<PointViewModel>();
                                List<int> attachementId = new List<int>();
                                foreach (HttpPostedFileBase item in upload)
                                {
                                    //Reading CSV file to get GPS point and save them in a List of Point
                                    string fileExtension = Path.GetExtension(item.FileName);
                                    if (fileExtension == ".csv" || fileExtension == ".xls" || fileExtension == ".xlsx")
                                        Points = Helpers.ReadPointFiles(item);
                                    string result = Helpers.SaveAttachment(item, "Land Request", LandRequest.RequestId);
                                    if (result.StartsWith("Error"))
                                        throw new Exception();
                                }

                                if (Points.Count > 0)
                                {
                                    foreach (var point in Points)
                                    {
                                        db.T_Point.Add(new T_Point()
                                        {
                                            PointName = point.PointName,
                                            Latitude = point.Latitude,
                                            Longitude = point.Longitude,
                                            Elevation = point.Elevation,
                                            LandId = land.LandId,
                                            Created = DateTime.Now,
                                            Updated = DateTime.Now,
                                            CreatedBy = loggedUser,
                                            UpdatedBy = loggedUser
                                        });
                                    }
                                    db.SaveChanges();
                                }

                                db.SaveChanges();

                                //DELETE OLD FILES BEFORE SAVING NEW FILES
                                foreach (var file in fileToDelete)
                                {
                                    if (File.Exists(file))
                                        File.Delete(file);
                                }
                                success.Add("The Land Request has been updated");
                                scope.Complete();
                                return success;
                            }
                            catch (Exception e)
                            {
                                string serverPath = folderRoot + @"LR\" + LandRequest.RequestId.ToString() + @"\";
                                foreach (var item in upload)
                                {
                                    string fileName = item.FileName;
                                    serverPath = Path.Combine(serverPath, fileName);
                                    if (File.Exists(serverPath))
                                        File.Delete(serverPath);
                                }
                                List<string> exception = new List<string> { "Exception", e.Message };
                                return exception;
                            }
                        }
                    }
                }
                else
                {
                    error.Add("An error occures when trying to update a land request. Try again later. If problem perists, please raise an MIS incident");
                    return error;
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }  
        }

        public List<string> CancelLandRequest(int LACRequestId)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-REQ"))
            {
                //Validate values
                if (LACRequestId == -1 || LACRequestId < 1)
                {
                    error.Add("Land Request ID is empty. Please refresh the page and try again.");
                    return error;
                }
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //CHANGE REQUEST STATUS TO CANCELLED
                            var landRequest = db.T_LACRequest.Find(LACRequestId);
                            var request = db.T_Request.FirstOrDefault(r => r.RequestId == landRequest.Requestid);
                            request.RequestStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Cancelled").ListId;
                            request.Updated = DateTime.Now;
                            request.UpdatedBy = loggedUser;
                            db.Entry(request).State = EntityState.Modified;

                            db.T_RequestLog.Add(new T_RequestLog()
                            {
                                RequestID = request.RequestId,
                                RequestType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId,
                                ActionType = db.T_List.FirstOrDefault(l => l.ListName == "Action Type" && l.ListValue == "Cancel Request").ListId,
                                Comment = "Cancel Request",
                                Created = DateTime.Now,
                                CreatedBy = loggedUser
                            });

                            db.SaveChanges();

                            dbTransaction.Commit();
                            success.Add("The Land Request has been Cancelled");
                            return success;
                        }
                        catch (Exception e)
                        {
                            dbTransaction.Rollback();
                            List<string> exception = new List<string> { "Exception", e.Message };
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> ApproveRejectRequest(string type, int RequestID, string Comment)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-APP") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                //Validate values
                if (RequestID == -1 || RequestID < 1)
                    error.Add("Land Request ID is empty. Please refresh the page and try again.");
                else if (string.IsNullOrWhiteSpace(type))
                    error.Add("Please indicate wheter you want to approve or to reject");
                else if (type == "Reject" && string.IsNullOrWhiteSpace(Comment))
                    error.Add("Rejection comment is mandatory");

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Request = db.T_Request.FirstOrDefault(r => r.RequestId == RequestID);
                            int _status = (type == "Approve" ? db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Approved by HOD").ListId : db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Rejected").ListId);

                            //UPDATE REQUEST WITH NEW REQUEST STATUS
                            Request.RequestStatus = _status;
                            Request.Updated = DateTime.Now;
                            Request.UpdatedBy = loggedUser;
                            db.Entry(Request).State = EntityState.Modified;

                            Comment = (type == "Approve" ? "Approved" : Comment);
                            //INSERT RECORD IN APPROVAL
                            db.T_Approval.Add(new T_Approval()
                            {
                                RequestId = RequestID,
                                Approver = _person.EmployeeId,
                                ActionDate = DateTime.Now,
                                ApprovalStatus = _status,
                                IsApproved = true,
                                ApprovalComments = Comment,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });

                            int actionType = (type == "Approve" ? db.T_List.FirstOrDefault(l => l.ListName == "Action Type" && l.ListValue == "Approve Request").ListId : db.T_List.FirstOrDefault(l => l.ListName == "Action Type" && l.ListValue == "Reject Request").ListId);

                            db.T_RequestLog.Add(new T_RequestLog()
                            {
                                RequestID = RequestID,
                                RequestType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId,
                                ActionType = actionType,
                                Comment = Comment,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser
                            });
                            db.SaveChanges();

                            //SEND RELEVANT MAILS
                            string approverName = _person.FirstName + " " + _person.LastName;
                            string approverEmail = Request.T_Employee.T_Department1.T_Employee.T_Person.Email;
                            string filePathRequestedFor = "";
                            string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Land/MyRequest";

                            //Get Requestor details
                            EmailNotificationViewModel notificationRequestedFor = new EmailNotificationViewModel();
                            string requestorEmail = Request.T_Employee.T_Person.Email;

                            if (approverEmail != null)
                                notificationRequestedFor.Cc.Add(approverEmail);

                            if (requestorEmail != null)
                                notificationRequestedFor.To.Add(requestorEmail);

                            if (type == "Approve")
                            {
                                filePathRequestedFor = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestApprovedToRequestor.html");
                                notificationRequestedFor.Subject = "Your Land Request has been Approved";
                            }
                            else
                            {
                                filePathRequestedFor = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestRejectedToRequestor.html");
                                notificationRequestedFor.Subject = "Your Land Request has been Rejected";
                            }

                            StreamReader strRequestedFor = new StreamReader(filePathRequestedFor);
                            string mailTextRequestedFor = strRequestedFor.ReadToEnd();
                            strRequestedFor.Close();

                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestorName]", Request.T_Employee.T_Person.FirstName + " " + Request.T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestType]", "Land Request");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDate]", Request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approverComment]", Comment);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approver]", approverName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[projectName]", Request.T_LACRequest.FirstOrDefault().ProjectName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[locationName]", Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[scheduleDate]", Request.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[contactPerson]", Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDetails]", Request.T_LACRequest.FirstOrDefault().WorkDescription);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestLink]", link);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationRequestedFor.Body = mailTextRequestedFor;
                            Helpers.SendEmail(notificationRequestedFor);

                            dbTransaction.Commit();
                            success.Add("This land request has been " + (type == "Approve" ? "Approved" : "Rejected") + " successfully");
                            return success;
                        }
                        catch (Exception e)
                        {
                            dbTransaction.Rollback();
                            List<string> exception = new List<string> { "Exception", e.Message };
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }


        }

        public List<string> UpdatePoint(PointViewModel point)
        {
            List<string> success = new List<string>() { "Success", "Point updated successfully!" };
            List<string> error = new List<string>() { "Error" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-REQ"))
            {
                if (point.LandId < 1)
                    error.Add("Please refresh the page!");
                if (point.PointId < 1)
                    error.Add("Please select the point");

                if (error.Count > 1)
                    return error;
                else
                {
                    var existingPoint = db.T_Point.FirstOrDefault(p => p.Latitude == point.Latitude && p.Longitude == point.Longitude && p.LandId == point.LandId && p.PointName == point.PointName && p.Elevation == point.Elevation);
                    if (existingPoint != null)
                    {
                        error.Add("There is already another point on this land with these coordinates");
                        return error;
                    }
                    else
                    {
                        try
                        {
                            var landPoint = db.T_Point.Find(point.PointId);
                            if (landPoint != null)
                            {
                                landPoint.PointName = point.PointName;
                                landPoint.Latitude = point.Latitude;
                                landPoint.Longitude = point.Longitude;
                                landPoint.Elevation = point.Elevation;
                                landPoint.Updated = DateTime.Now;
                                landPoint.UpdatedBy = loggedUser;
                                db.Entry(landPoint).State = EntityState.Modified;

                                db.SaveChanges();
                                return success;
                            }
                            else
                            {
                                error.Add("This point already doesn't exists on this land");
                                return error;
                            }
                        }
                        catch (Exception e)
                        {
                            error.Clear();
                            error.Add("Exception");
                            error.Add(e.Message);
                            return error;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> DeletePoint(PointViewModel point)
        {
            List<string> success = new List<string>() { "Success", "Point removed successfully!" };
            List<string> error = new List<string>() { "Error" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-REQ"))
            {
                if (point.LandId < 1)
                    error.Add("Please refresh the page!");
                if (point.PointId < 1)
                    error.Add("Please select the point");

                if (error.Count > 1)
                    return error;
                else
                {
                    var existingPoint = db.T_Point.FirstOrDefault(p => p.PointId == point.PointId && p.LandId == point.LandId);
                    if (existingPoint != null)
                    {
                        try
                        {
                            db.T_Point.Remove(existingPoint);
                            db.Entry(existingPoint).State = EntityState.Deleted;

                            db.SaveChanges();
                            return success;
                        }
                        catch (Exception e)
                        {
                            error.Clear();
                            error.Add("Exception");
                            error.Add(e.Message);
                            return error;
                        }
                    }
                    else
                    {
                        error.Add("This point doesn't exists on this land");
                        return error;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> InsertPoint(List<PointViewModel> points)
        {
            List<string> success = new List<string>() { "Success", "Point removed successfully!" };
            List<string> error = new List<string>() { "Error" };
            List<PointViewModel> errors = new List<PointViewModel>();
            List<List<string>> errorsList = new List<List<string>>();

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-REQ"))
            {
                if (points.Count > 0)
                {
                    //Start by delete all previous point for this land
                    int landId = points.FirstOrDefault().LandId;
                    var landPoint = db.T_Point.Where(p => p.LandId == landId);

                    if (landPoint != null)
                    {
                        db.T_Point.RemoveRange(landPoint);
                        db.SaveChanges();
                    }

                    foreach (var point in points)
                    {
                        if (point.LandId < 1)
                            error.Add("Please refresh the page!");
                        if (point.Latitude < 1)
                            error.Add("Latitude can't be empty");
                        if (point.Longitude < 1)
                            error.Add("Longitude can't be empty or null");

                        if (error.Count > 1)
                        {
                            errors.Add(point);
                            errorsList.Add(error);
                            error.RemoveRange(1, error.Count - 1);
                        }
                        else
                        {
                            db.T_Point.Add(new T_Point()
                            {
                                LandId = point.LandId,
                                PointName = point.PointName,
                                Latitude = point.Latitude,
                                Longitude = point.Longitude,
                                Elevation = point.Elevation,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });
                        }
                    }
                    db.SaveChanges();
                    return success;
                }
                else
                {
                    error.Add("Can't insert an empty list!");
                    return error;
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> CreateLAC (LACViewModel LacViewModel)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "LAC created successfuly!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-REQ"))
            {

            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }

            //if (LacViewModel.AreaRequestedUOM == -1 || LacViewModel.AreaRequestedUOM < 1)
            //    error.Add("Requested Area UOM is mandatory");
            if (string.IsNullOrWhiteSpace(LacViewModel.LACName))
                error.Add("Please specify the name for this LAC");
            if (LacViewModel.LACRequestId == -1 || LacViewModel.LACRequestId <1 )
                error.Add("Please specify valid LAC ID. Please refresh the page and try again");
            //if (LacViewModel.AreaRequested == -1 || LacViewModel.AreaRequested <1)
            //    error.Add("Please specify Requested Area Size");
            //if (LacViewModel.CostEstimate == -1 || LacViewModel.CostEstimate < 1)
            //    error.Add("Please specify the Estimated Cost");

            if (error.Count > 1)
                return error;
            else
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var lacRequest = db.T_LACRequest.FirstOrDefault(l=>l.LACRequestId == LacViewModel.LACRequestId);
                        var existingLac = db.T_LAC.FirstOrDefault(l => l.LACRequestId == LacViewModel.LACRequestId);

                        if (existingLac != null)
                        {
                            error.Add("There is already a LAC for this request. " + existingLac.LACName);
                            return error;
                        } 
                        else
                        {
                            var lac = new T_LAC()
                            {
                                LAC_ID = "",
                                LACName = LacViewModel.LACName,
                                LACRequestId = LacViewModel.LACRequestId,
                                LACStatus = db.T_List.FirstOrDefault(l => l.ListName == "LAC Status" && l.ListValue == "LAC Created").ListId,
                                Realcosts = 0,
                                PAPs = 0,
                                AreaDescription = lacRequest.ProjectName,
                                AreaRequested = LacViewModel.AreaRequested,
                                AreaRequestedUOM = (LacViewModel.AreaRequestedUOM == -1? default : LacViewModel.AreaRequestedUOM),
                                CostEstimate = LacViewModel.CostEstimate,
                                Comment = LacViewModel.Comment,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            };
                            db.T_LAC.Add(lac);
                            db.SaveChanges();
                            int lacId = lac.LACId;

                            lac.LAC_ID = "LAC" + lac.LACId;
                            db.Entry(lac).State = EntityState.Modified;

                            //UPDATE LACREQUEST
                            lacRequest.Locked = true;
                            lacRequest.Updated = DateTime.Now;
                            lacRequest.UpdatedBy = loggedUser;
                            db.Entry(lacRequest).State = EntityState.Modified;

                            //UPDATE REQUEST
                            var request = lacRequest.T_Request;
                            request.RequestStatus = db.T_List.FirstOrDefault(l=>l.ListName == "Request Status" && l.ListValue == "In progress").ListId;
                            request.Locked = true;
                            request.Updated = DateTime.Now;
                            request.UpdatedBy = loggedUser;
                            db.Entry(request).State = EntityState.Modified;

                            //INSERT INTO T_REQUEST LOG
                            db.T_RequestLog.Add(new T_RequestLog()
                            {
                                RequestID = lacRequest.LACRequestId,
                                RequestType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId,
                                ActionType = db.T_List.FirstOrDefault(l => l.ListName == "Action Type" && l.ListValue == "Create LAC").ListId,
                                Comment = LacViewModel.Comment,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser
                            });

                            db.SaveChanges();

                            //MOVE LAC REQUEST ATTACHMENTS AND PUT THEM IN LAC FOLDER
                            var attachment = request.T_RequestAttachment;

                            dbTransaction.Commit();

                            //SEND EMAIL TO REQUESTOR
                            string approverName = _person.FirstName + " " + _person.LastName;
                            //string approverEmail = request.T_Employee.T_Department1.T_Employee.T_Person.Email;
                            string filePathRequestedFor = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestStatusToRequestor.html");
                            string filePathTopograph = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestToTopograph.html");
                            string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Land/MyRequest";

                            //Get Requestor details
                            EmailNotificationViewModel notificationRequestedFor = new EmailNotificationViewModel();
                            EmailNotificationViewModel notificationTopograph = new EmailNotificationViewModel();

                            string requestorEmail = request.T_Employee.T_Person.Email;

                            //if (approverEmail != null)
                            //    notificationRequestedFor.Cc.Add(approverEmail);

                            if (requestorEmail != null)
                                notificationRequestedFor.To.Add(requestorEmail);

                            StreamReader strRequestedFor = new StreamReader(filePathRequestedFor);
                            string mailTextRequestedFor = strRequestedFor.ReadToEnd();
                            strRequestedFor.Close();

                            StreamReader strRequested = new StreamReader(filePathTopograph);
                            string mailTextTopograph = strRequested.ReadToEnd();
                            strRequested.Close();

                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestTitle]", "A NEW LAC "+lac.LACName + " ("+lac.LAC_ID+") HAS BEEN CREATED");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestorName]", request.T_Employee.T_Person.FirstName + " " + request.T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestType]", request.T_List1.ListValue);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestStatus]", request.T_List.ListValue);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDate]", request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approverComment]", LacViewModel.Comment);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approver]", approverName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[projectName]", lacRequest.ProjectName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[locationName]", lacRequest.T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[scheduleDate]", lacRequest.AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[contactPerson]", lacRequest.T_Employee.T_Person.FirstName + " " + lacRequest.T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDetails]", lacRequest.WorkDescription);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestLink]", link);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationRequestedFor.Body = mailTextRequestedFor;
                            notificationRequestedFor.Subject = "Your Land request is converted to LAC";
                            Helpers.SendEmail(notificationRequestedFor);

                            //Mail to Topograph group
                            List<string> to = new List<string> { "SG-FGM-RAP-POWER@tfm.cmoc.com", "SG-FGM-RAP-LO@tfm.cmoc.com", "SG-FGM-RAP-INV@tfm.cmoc.com" };
                            notificationTopograph.Cc.Add("SG-FGM-RAP-MGT@tfm.cmoc.com");
                            notificationTopograph.To = to;
                            //notificationTopograph.To.Add("lmutamba@tfm.cmoc.com");

                            mailTextTopograph = mailTextTopograph.Replace("[requestTitle]", "A NEW LAC " + lac.LACName + " ("+lac.LAC_ID+") HAS BEEN CREATED");
                            mailTextTopograph = mailTextTopograph.Replace("[requestorName]", request.T_Employee.T_Person.FirstName + " " + request.T_Employee.T_Person.LastName);
                            mailTextTopograph = mailTextTopograph.Replace("[requestType]", "Land Request");//lacName
                            mailTextTopograph = mailTextTopograph.Replace("[requestStatus]", request.T_List.ListValue);
                            mailTextTopograph = mailTextTopograph.Replace("[requestDate]", request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextTopograph = mailTextTopograph.Replace("[approverComment]", LacViewModel.Comment);
                            mailTextTopograph = mailTextTopograph.Replace("[approver]", approverName);
                            mailTextTopograph = mailTextTopograph.Replace("[projectName]", lacRequest.ProjectName);
                            mailTextTopograph = mailTextTopograph.Replace("[lacName]", lac.LACName + " (" + lac.LAC_ID + ")");
                            mailTextTopograph = mailTextTopograph.Replace("[locationName]", lacRequest.T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextTopograph = mailTextTopograph.Replace("[scheduleDate]", lacRequest.AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextTopograph = mailTextTopograph.Replace("[contactPerson]", lacRequest.T_Employee.T_Person.FirstName + " " + lacRequest.T_Employee.T_Person.LastName);
                            mailTextTopograph = mailTextTopograph.Replace("[requestDetails]", lacRequest.WorkDescription);
                            mailTextTopograph = mailTextTopograph.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationTopograph.Body = mailTextTopograph;
                            notificationTopograph.Subject = "A new LAC has been created!";
                            Helpers.SendEmail(notificationTopograph);

                            return success;
                        } 
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Rollback();
                        List<string> exception = new List<string> { "Exception", e.Message };
                        return exception;
                    }
                }
            }
        }

        public List<string> ForwardImpactMitigation(int RequestID, string Comment, List<PointViewModel> points) 
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {

                if (RequestID == -1 || RequestID < 1)
                    error.Add("Land Request ID is empty. Please refresh the page and try again.");
                else if (string.IsNullOrWhiteSpace(Comment))
                    error.Add("Forward comment is mandatory");

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //UPDATE REQUEST TO BE SEEN BY TOPOGRAPH
                            var Request = db.T_Request.Find(RequestID);
                            Request.RequestStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Topograph Input").ListId;
                            Request.Updated = DateTime.Now;
                            Request.UpdatedBy = loggedUser;
                            db.Entry(Request).State = EntityState.Modified;
                            db.SaveChanges();

                            //CHANGE LAND TYPE TO RESTRICTED
                            var land = Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault();
                            land.LandCategory = db.T_List.FirstOrDefault(l => l.ListName == "Land Category" && l.ListValue == "Restricted Zone").ListId;
                            db.Entry(land).State = EntityState.Modified;
                            db.SaveChanges();

                            //INSERT NEW COORDINATE IN LAND
                            if (points.Count > 0)
                            {
                                //Delete existing point for this land
                                var Point = db.T_Point.Where(p => p.LandId == land.LandId);
                                if (Point != null)
                                    db.T_Point.RemoveRange(Point);

                                foreach (var point in points)
                                {
                                    db.T_Point.Add(new T_Point()
                                    {
                                        PointName = point.PointName,
                                        Latitude = point.Latitude,
                                        Longitude = point.Longitude,
                                        Elevation = point.Elevation,
                                        LandId = land.LandId,
                                        Created = DateTime.Now,
                                        Updated = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        UpdatedBy = loggedUser
                                    });
                                }
                                db.SaveChanges();
                            }

                            db.T_RequestLog.Add(new T_RequestLog()
                            {
                                RequestID = RequestID,
                                RequestType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId,
                                ActionType = db.T_List.FirstOrDefault(l => l.ListName == "Action Type" && l.ListValue == "Impact and Mitigation").ListId,
                                Comment = Comment,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser
                            });
                            db.SaveChanges();

                            dbTransaction.Commit();

                            //SEND EMAIL TO REQUESTOR
                            string approverName = _person.FirstName + " " + _person.LastName;
                            string approverEmail = Request.T_Employee.T_Department1.T_Employee.T_Person.Email;
                            string filePathRequestedFor = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestStatusToRequestor.html");
                            string filePathTopograph = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestToTopograph.html");
                            string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Land/MyRequest";

                            //Get Requestor details
                            EmailNotificationViewModel notificationRequestedFor = new EmailNotificationViewModel();
                            EmailNotificationViewModel notificationTopograph = new EmailNotificationViewModel();

                            string requestorEmail = Request.T_Employee.T_Person.Email;

                            if (approverEmail != null)
                                notificationRequestedFor.Cc.Add(approverEmail);

                            if (requestorEmail != null)
                                notificationRequestedFor.To.Add(requestorEmail);

                            StreamReader strRequestedFor = new StreamReader(filePathRequestedFor);
                            string mailTextRequestedFor = strRequestedFor.ReadToEnd();
                            strRequestedFor.Close();

                            StreamReader strRequested = new StreamReader(filePathTopograph);
                            string mailTextTopograph = strRequested.ReadToEnd();
                            strRequested.Close();

                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestTitle]", "Status change on your Land Request");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestorName]", Request.T_Employee.T_Person.FirstName + " " + Request.T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestType]", "Land Request");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestStatus]", Request.T_List.ListValue);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDate]", Request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approverComment]", Comment);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approver]", approverName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[projectName]", Request.T_LACRequest.FirstOrDefault().ProjectName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[locationName]", Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[scheduleDate]", Request.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[contactPerson]", Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDetails]", Request.T_LACRequest.FirstOrDefault().WorkDescription);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestLink]", link);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationRequestedFor.Body = mailTextRequestedFor;
                            notificationRequestedFor.Subject = "Your Land request is sent for Impact and Mitigation";
                            Helpers.SendEmail(notificationRequestedFor);

                            //Mail to Topograph group
                            notificationTopograph.To.Add("SG-FGM-RAP-LO@tfm.cmoc.com");
                            mailTextTopograph = mailTextTopograph.Replace("[requestTitle]", "Status change on your Land Request");
                            mailTextTopograph = mailTextTopograph.Replace("[requestorName]", Request.T_Employee.T_Person.FirstName + " " + Request.T_Employee.T_Person.LastName);
                            mailTextTopograph = mailTextTopograph.Replace("[requestType]", "Land Request");
                            mailTextTopograph = mailTextTopograph.Replace("[requestStatus]", Request.T_List.ListValue);
                            mailTextTopograph = mailTextTopograph.Replace("[requestDate]", Request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextTopograph = mailTextTopograph.Replace("[approverComment]", Comment);
                            mailTextTopograph = mailTextTopograph.Replace("[approver]", approverName);
                            mailTextTopograph = mailTextTopograph.Replace("[projectName]", Request.T_LACRequest.FirstOrDefault().ProjectName);
                            mailTextTopograph = mailTextTopograph.Replace("[locationName]", Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextTopograph = mailTextTopograph.Replace("[scheduleDate]", Request.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextTopograph = mailTextTopograph.Replace("[contactPerson]", Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName);
                            mailTextTopograph = mailTextTopograph.Replace("[requestDetails]", Request.T_LACRequest.FirstOrDefault().WorkDescription);
                            mailTextTopograph = mailTextTopograph.Replace("[requestLink]", link);
                            mailTextTopograph = mailTextTopograph.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationTopograph.Body = mailTextTopograph;
                            notificationTopograph.Subject = "A new land request needs your attention for impact and mitigation!";
                            Helpers.SendEmail(notificationTopograph);

                            success.Add("Request forwarded successfuly!");
                            return success;
                        }
                        catch (Exception e)
                        {
                            dbTransaction.Rollback();
                            List<string> exception = new List<string> { "Exception", e.Message };
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> SendRequestor(int RequestID, string Comment, HttpPostedFileBase[] upload)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {

                if (RequestID == -1 || RequestID < 1)
                    error.Add("Land Request ID is empty. Please refresh the page and try again.");
                else if (string.IsNullOrWhiteSpace(Comment))
                    error.Add("Forward comment is mandatory");

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        string serverPath = folderRoot + @"LR\";
                        List<string> fileToDelete = new List<string>();
                        try
                        {
                            //UPDATE REQUEST TO BE SENT TO REQUESTOR
                            var Request = db.T_Request.Find(RequestID);
                            Request.RequestStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Send to Requestor").ListId;
                            Request.Updated = DateTime.Now;
                            Request.UpdatedBy = loggedUser;
                            db.Entry(Request).State = EntityState.Modified;
                            db.SaveChanges();

                            //CHANGE LAND TYPE TO RESTRICTED
                            var land = Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault();

                            //INSERT ATTACHMENT
                            List<PointViewModel> Points = new List<PointViewModel>();
                            List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                            foreach (var item in upload)
                            {
                                if (item != null)
                                {
                                    string fileName = Path.GetFileName(item.FileName);
                                    serverPath = folderRoot + @"LR\" + RequestID.ToString() + @"\";
                                    serverPath = Path.Combine(serverPath, fileName);
                                    var attachs = new T_Attachment()
                                    {
                                        RequestAttachementType = db.T_List.FirstOrDefault(l => l.ListName == "Attachment Type" && l.ListValue == "Land Request").ListId,
                                        RequestAttachementPath = serverPath,
                                        RequestAttachementContentType = item.ContentType,
                                        RequestAttachementFile = item.FileName,
                                        Created = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        Updated = DateTime.Now,
                                        UpdatedBy = loggedUser
                                    };
                                    db.T_Attachment.Add(attachs);
                                    db.SaveChanges();
                                    item.SaveAs(serverPath);
                                    attachments.Add(new System.Net.Mail.Attachment(item.InputStream, item.FileName));
                                    int attachmentId = attachs.AttachmentId;

                                    //Reading CSV file to get GPS point and save them in a List of Point

                                    string fileExtension = Path.GetExtension(item.FileName);
                                    if (fileExtension == ".csv" || fileExtension == ".xls" || fileExtension == ".xlsx")
                                        Points = Helpers.ReadPointFiles(item, -1);

                                    db.T_RequestAttachment.Add(new T_RequestAttachment()
                                    {
                                        RequestId = RequestID,
                                        AttachmentId = attachmentId,
                                        Created = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        Updated = DateTime.Now,
                                        UpdatedBy = loggedUser
                                    });
                                    db.SaveChanges();

                                    if (Points.Count > 0)
                                    {
                                        foreach (var point in Points)
                                        {
                                            db.T_Point.Add(new T_Point()
                                            {
                                                Latitude = point.Latitude,
                                                Longitude = point.Longitude,
                                                Elevation = point.Elevation,
                                                LandId = land.LandId,
                                                Created = DateTime.Now,
                                                Updated = DateTime.Now,
                                                CreatedBy = loggedUser,
                                                UpdatedBy = loggedUser
                                            });
                                        }
                                        Points.Clear();
                                        db.SaveChanges();
                                    }
                                }
                            }

                            db.T_RequestLog.Add(new T_RequestLog()
                            {
                                RequestID = RequestID,
                                RequestType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId,
                                ActionType = db.T_List.FirstOrDefault(l => l.ListName == "Action Type" && l.ListValue == "Submit Request").ListId,
                                Comment = Comment,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser
                            });
                            db.SaveChanges();

                            dbTransaction.Commit();

                            //SEND EMAIL TO REQUESTOR
                            string approverName = _person.FirstName + " " + _person.LastName;
                            string approverEmail = Request.T_Employee.T_Department1.T_Employee.T_Person.Email;
                            string filePathRequestedFor = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestStatusToRequestor.html");
                            string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Land/MyRequest";

                            //Get Requestor details
                            EmailNotificationViewModel notificationRequestedFor = new EmailNotificationViewModel();

                            string requestorEmail = Request.T_Employee.T_Person.Email;

                            if (approverEmail != null)
                                notificationRequestedFor.Cc.Add(approverEmail);

                            if (requestorEmail != null)
                                notificationRequestedFor.To.Add(requestorEmail);

                            StreamReader strRequestedFor = new StreamReader(filePathRequestedFor);
                            string mailTextRequestedFor = strRequestedFor.ReadToEnd();
                            strRequestedFor.Close();

                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestTitle]", "Status change on your Land Request");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestorName]", Request.T_Employee.T_Person.FirstName + " " + Request.T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestType]", "Land Request");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestStatus]", Request.T_List.ListValue);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDate]", Request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approverComment]", Comment);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approver]", approverName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[projectName]", Request.T_LACRequest.FirstOrDefault().ProjectName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[locationName]", Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[scheduleDate]", Request.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[contactPerson]", Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDetails]", Request.T_LACRequest.FirstOrDefault().WorkDescription);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestLink]", link);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationRequestedFor.Body = mailTextRequestedFor;
                            notificationRequestedFor.Attachment = attachments.ToArray();
                            notificationRequestedFor.Subject = "Your Land request is sent back to you. Please update your Request";
                            Helpers.SendEmail(notificationRequestedFor);

                            success.Add("Request returned successfuly!");
                            return success;
                        }
                        catch (Exception e)
                        {
                            dbTransaction.Rollback();
                            foreach (var item in upload)
                            {
                                serverPath = folderRoot + @"LR\" + RequestID.ToString() + @"\";
                                if (File.Exists(Path.Combine(serverPath, item.FileName)))
                                    File.Delete(Path.Combine(serverPath, item.FileName));
                            }
                            List<string> exception = new List<string> { "Exception", e.Message };
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> ImpactMitigation(int RequestID, string Comment)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success","Request sent to land Office successfuly!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {

                if (RequestID == -1 || RequestID < 1)
                    error.Add("Land Request ID is empty. Please refresh the page and try again.");
                else if (string.IsNullOrWhiteSpace(Comment))
                    error.Add("Return comment is mandatory");

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var Request = db.T_Request.Find(RequestID);
                            Request.RequestStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Topograph Return").ListId;
                            Request.Updated = DateTime.Now;
                            Request.UpdatedBy = loggedUser;
                            db.Entry(Request).State = EntityState.Modified;
                            db.SaveChanges();

                            //Insert info in T_RequestLog table
                            db.T_RequestLog.Add(new T_RequestLog()
                            {
                                RequestID = RequestID,
                                RequestType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId,
                                ActionType = db.T_List.FirstOrDefault(l => l.ListName == "Action Type" && l.ListValue == "Validate Request").ListId,
                                Comment = Comment,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser
                            });
                            db.SaveChanges();
                            dbTransaction.Commit();

                            //SEND EMAIL TO REQUESTOR
                            string topoName = _person.FirstName + " " + _person.LastName;
                            string topoEmail = Request.T_Employee.T_Department1.T_Employee.T_Person.Email;
                            string filePathRequestedFor = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestStatusToRequestor.html");
                            string filePathLO = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestToTopograph.html");
                            string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Land/MyRequest";

                            //Get Requestor details
                            EmailNotificationViewModel notificationRequestedFor = new EmailNotificationViewModel();
                            EmailNotificationViewModel notificationLO = new EmailNotificationViewModel();

                            string requestorEmail = Request.T_Employee.T_Person.Email;
                            string approverEmail = Request.T_Employee.T_Department1.T_Employee1.FirstOrDefault().T_Person.Email;
                            if (approverEmail != null)
                                notificationRequestedFor.Cc.Add(approverEmail);

                            if (requestorEmail != null)
                                notificationRequestedFor.To.Add(requestorEmail);

                            StreamReader strRequestedFor = new StreamReader(filePathRequestedFor);
                            string mailTextRequestedFor = strRequestedFor.ReadToEnd();
                            strRequestedFor.Close();

                            StreamReader strRequested = new StreamReader(filePathLO);
                            string mailTextTopograph = strRequested.ReadToEnd();
                            strRequested.Close();

                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestTitle]", "Status change on your Land Request");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestorName]", Request.T_Employee.T_Person.FirstName + " " + Request.T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestType]", "Land Request");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestStatus]", Request.T_List.ListValue);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDate]", Request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approverComment]", Comment);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approver]", topoName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[projectName]", Request.T_LACRequest.FirstOrDefault().ProjectName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[locationName]", Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[scheduleDate]", Request.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[contactPerson]", Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDetails]", Request.T_LACRequest.FirstOrDefault().WorkDescription);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestLink]", link);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationRequestedFor.Body = mailTextRequestedFor;
                            notificationRequestedFor.Subject = "Your Land request is sent back to Land Office";
                            Helpers.SendEmail(notificationRequestedFor);

                            //Mail to Topograph group
                            notificationLO.To.Add("SG-FGM-RAP-LO@tfm.cmoc.com");
                            mailTextTopograph = mailTextTopograph.Replace("[requestTitle]", "Status change on your Land Request");
                            mailTextTopograph = mailTextTopograph.Replace("[requestorName]", Request.T_Employee.T_Person.FirstName + " " + Request.T_Employee.T_Person.LastName);
                            mailTextTopograph = mailTextTopograph.Replace("[requestType]", "Land Request");
                            mailTextTopograph = mailTextTopograph.Replace("[requestStatus]", Request.T_List.ListValue);
                            mailTextTopograph = mailTextTopograph.Replace("[requestDate]", Request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextTopograph = mailTextTopograph.Replace("[approverComment]", Comment);
                            mailTextTopograph = mailTextTopograph.Replace("[approver]", topoName);
                            mailTextTopograph = mailTextTopograph.Replace("[projectName]", Request.T_LACRequest.FirstOrDefault().ProjectName);
                            mailTextTopograph = mailTextTopograph.Replace("[locationName]", Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextTopograph = mailTextTopograph.Replace("[scheduleDate]", Request.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextTopograph = mailTextTopograph.Replace("[contactPerson]", Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName);
                            mailTextTopograph = mailTextTopograph.Replace("[requestDetails]", Request.T_LACRequest.FirstOrDefault().WorkDescription);
                            mailTextTopograph = mailTextTopograph.Replace("[requestLink]", link);
                            mailTextTopograph = mailTextTopograph.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationLO.Body = mailTextTopograph;
                            notificationLO.Subject = "A new land request needs your attention for impact and mitigation!";
                            Helpers.SendEmail(notificationLO);

                            return success;
                        }
                        catch (Exception e)
                        {
                            dbTransaction.Rollback();
                            List<string> exception = new List<string> { "Exception", e.Message };
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> GrantGreenLight(int RequestID, string Comment, List<PointViewModel> points)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {

                if (RequestID == -1 || RequestID < 1)
                    error.Add("Land Request ID is empty. Please refresh the page and try again.");
                else if (string.IsNullOrWhiteSpace(Comment))
                    error.Add("Green Light comment is mandatory");

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //UPDATE REQUEST SO THAT IT WILL BE COMPLETED
                            var Request = db.T_Request.Find(RequestID);
                            Request.RequestStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Green Light").ListId;
                            //Request.CurrentStep = db.T_WorkFlow.Where(w=>w.RequestType == Request.RequestType).Max(w=>w.StepCount);
                            Request.Updated = DateTime.Now;
                            Request.UpdatedBy = loggedUser;
                            db.Entry(Request).State = EntityState.Modified;
                            db.SaveChanges();

                            //CHANGE LAND TYPE TO RESTRICTED
                            var land = Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault();
                            land.LandCategory = db.T_List.FirstOrDefault(l => l.ListName == "Land Category" && l.ListValue == "Forbidden Zone").ListId;
                            db.Entry(land).State = EntityState.Modified;
                            db.SaveChanges();

                            //INSERT NEW COORDINATE IN LAND
                            if (points.Count > 0)
                            {
                                //Delete existing point for this land
                                var Point = db.T_Point.Where(p => p.LandId == land.LandId);
                                if (Point != null)
                                    db.T_Point.RemoveRange(Point);

                                foreach (var point in points)
                                {
                                    db.T_Point.Add(new T_Point()
                                    {
                                        PointName = point.PointName,
                                        Latitude = point.Latitude,
                                        Longitude = point.Longitude,
                                        Elevation = point.Elevation,
                                        LandId = land.LandId,
                                        Created = DateTime.Now,
                                        Updated = DateTime.Now,
                                        CreatedBy = loggedUser,
                                        UpdatedBy = loggedUser
                                    });
                                }
                                db.SaveChanges();
                            }

                            //CREATE AN APPROVAL OBJECT
                            db.T_Approval.Add(new T_Approval()
                            {
                                Approver = Helpers.GetLogInPerson().EmployeeId,
                                RequestId = RequestID,
                                ActionDate = DateTime.Now,
                                IsApproved = true,
                                ApprovalComments = Comment,
                                ApprovalStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Green Light").ListId,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });

                            db.T_RequestLog.Add(new T_RequestLog()
                            {
                                RequestID = RequestID,
                                RequestType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId,
                                ActionType = db.T_List.FirstOrDefault(l => l.ListName == "Action Type" && l.ListValue == "Green Light").ListId,
                                Comment = Comment,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser
                            });
                            db.SaveChanges();

                            dbTransaction.Commit();

                            //SEND EMAIL TO REQUESTOR
                            string approverName = _person.FirstName + " " + _person.LastName;
                            string approverEmail = Request.T_Employee.T_Department1.T_Employee.T_Person.Email;
                            string filePathRequestedFor = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestStatusToRequestor.html");
                            string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Land/MyRequest";

                            //Get Requestor details
                            EmailNotificationViewModel notificationRequestedFor = new EmailNotificationViewModel();
                            EmailNotificationViewModel notificationRAPMgt = new EmailNotificationViewModel();

                            string requestorEmail = Request.T_Employee.T_Person.Email;

                            if (approverEmail != null)
                                notificationRequestedFor.Cc.Add(approverEmail);

                            if (requestorEmail != null)
                                notificationRequestedFor.To.Add(requestorEmail);

                            StreamReader strRequestedFor = new StreamReader(filePathRequestedFor);
                            string mailTextRequestedFor = strRequestedFor.ReadToEnd();
                            strRequestedFor.Close();

                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestTitle]", "You have a green Light for your project");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestorName]", Request.T_Employee.T_Person.FirstName + " " + Request.T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestType]", "Land Request");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestStatus]", Request.T_List.ListValue);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDate]", Request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approverComment]", Comment);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approver]", approverName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[projectName]", Request.T_LACRequest.FirstOrDefault().ProjectName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[locationName]", Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[scheduleDate]", Request.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[contactPerson]", Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDetails]", Request.T_LACRequest.FirstOrDefault().WorkDescription);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestLink]", link);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationRequestedFor.Body = mailTextRequestedFor;
                            notificationRequestedFor.Subject = "Green Light : Go ahead with your project!";
                            Helpers.SendEmail(notificationRequestedFor);

                            //SEND EMAIL TO RAP MANAGEMENT
                            string filePathRapMgmt = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestToRAPMgm.html");
                            StreamReader strRapMgt = new StreamReader(filePathRapMgmt);
                            string mailTextRAPMgt = strRapMgt.ReadToEnd();
                            strRequestedFor.Close();

                            mailTextRAPMgt = mailTextRAPMgt.Replace("[requestIntro]", "Authorization has been given to a Land Request with below information");
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[requestTitle]", "You have a green Light for your project");
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[requestorName]", Request.T_Employee.T_Person.FirstName + " " + Request.T_Employee.T_Person.LastName);
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[requestType]", "Land Request");
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[requestStatus]", Request.T_List.ListValue);
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[requestDate]", Request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[approverComment]", Comment);
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[approver]", approverName);
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[projectName]", Request.T_LACRequest.FirstOrDefault().ProjectName);
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[locationName]", Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[scheduleDate]", Request.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[contactPerson]", Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName);
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[requestDetails]", Request.T_LACRequest.FirstOrDefault().WorkDescription);
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[requestLink]", link);
                            mailTextRAPMgt = mailTextRAPMgt.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationRAPMgt.To.Add("SG-FGM-RAP-MGT@tfm.cmoc.com");
                            notificationRAPMgt.Body = mailTextRAPMgt;
                            notificationRAPMgt.Subject = "A green light has been given to a Lac request";
                            Helpers.SendEmail(notificationRAPMgt);

                            success.Add("Request Completed");
                            return success;
                        }
                        catch (Exception e)
                        {
                            List<string> exception = new List<string> { "Exception" };
                            try
                            {
                                dbTransaction.Rollback();
                            }
                            finally
                            {
                                exception.Add(e.Message);
                            }
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> MakeRestricted(LandRequestViewModel Land)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success","This Land has made restricted successfully" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {

                if (Land.LACRequestId == -1 || Land.LACRequestId < 1)
                    error.Add("Land Request ID is empty. Please refresh the page and try again.");
                else if (string.IsNullOrWhiteSpace(Land.LandComment))
                    error.Add("Green Light comment is mandatory");
                else if (Land.AreaSurface < 0)
                    error.Add("Area Surface can not be empty or null");
                else if (Land.AreaSurfaceUOM == -1 || Land.AreaSurfaceUOM < 1)
                    error.Add("You did not choose the UOM for this area");
                else if (Land.LandStatusId == -1 || Land.LandStatusId < 1)
                    error.Add("Please choose a valid Land Status");

                if (error.Count > 1)
                    return error;
                else
                {
                    using (var dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var land = db.T_Land.FirstOrDefault(l => l.LACRequestId == Land.LACRequestId);
                            var Request = db.T_LACRequest.FirstOrDefault(l => l.LACRequestId == Land.LACRequestId).T_Request;
                            int dept = Request.T_Employee.DepartmentId;
                            if (land != null)
                            {
                                land.LandCategory = db.T_List.FirstOrDefault(l => l.ListName == "Land Category" && l.ListValue == "Restricted Zone").ListId;
                                land.Surface = Land.AreaSurface;
                                land.GPS_Date = Land.AuthorizedDate;
                                land.LandStatus = Land.LandStatusId;
                                land.DepartementId = dept;
                                land.Easting = Land.LandEasting;
                                land.Northing = Land.LandNorthing;
                                land.Comment = Land.LandComment;
                                land.Updated = DateTime.Now;
                                land.UpdatedBy = loggedUser;
                                db.Entry(land).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            dbTransaction.Commit();

                            //SEND EMAIL TO REQUESTOR
                            string approverName = _person.FirstName + " " + _person.LastName;
                            string approverEmail = Request.T_Employee.T_Department1.T_Employee.T_Person.Email;
                            string filePathRequestedFor = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestStatusToRequestor.html");
                            string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Land/MyRequest";

                            //Get Requestor details
                            EmailNotificationViewModel notificationRequestedFor = new EmailNotificationViewModel();

                            string requestorEmail = Request.T_Employee.T_Person.Email;

                            if (approverEmail != null)
                                notificationRequestedFor.Cc.Add(approverEmail);

                            if (requestorEmail != null)
                                notificationRequestedFor.To.Add(requestorEmail);

                            StreamReader strRequestedFor = new StreamReader(filePathRequestedFor);
                            string mailTextRequestedFor = strRequestedFor.ReadToEnd();
                            strRequestedFor.Close();

                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestTitle]", "The Land is now a Restricted Zone");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestorName]", Request.T_Employee.T_Person.FirstName + " " + Request.T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestType]", "Land Request");
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestStatus]", Request.T_List.ListValue);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDate]", Request.RequestedDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approverComment]", Land.LandComment);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[approver]", approverName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[projectName]", Request.T_LACRequest.FirstOrDefault().ProjectName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[locationName]", Request.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[scheduleDate]", Request.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[contactPerson]", Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + Request.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestDetails]", Request.T_LACRequest.FirstOrDefault().WorkDescription);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[requestLink]", link);
                            mailTextRequestedFor = mailTextRequestedFor.Replace("[year]", DateTime.Now.Year.ToString());

                            notificationRequestedFor.Body = mailTextRequestedFor;
                            notificationRequestedFor.Subject = "Your Land has been updated to be Restricted Zone";
                            Helpers.SendEmail(notificationRequestedFor);

                            return success;
                        }
                        catch (Exception e)
                        {
                            List<string> exception = new List<string> { "Exception" };
                            try
                            {
                                dbTransaction.Rollback();
                            }
                            finally
                            {
                                exception.Add(e.Message);
                            }
                            return exception;
                        }
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> AddLandImpactedVillage(VillageViewModel village)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "Village associated successfully!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV"))
            {

                if (village.LandId == -1 || village.LandId < 1)
                    error.Add("Invalid Land Id. Please refresh the page!");
                if (village.VillageId == -1 || village.VillageId < 1)
                    error.Add("Invalid Village. Please refresh the page and select the village again");

                if (error.Count > 1)
                    return error;
                else
                {
                    try
                    {
                        var landVillage = db.T_LandVillage.FirstOrDefault(l => l.LandId == village.LandId && l.VillageId == village.VillageId);
                        if (landVillage == null)
                        {
                            db.T_LandVillage.Add(new T_LandVillage()
                            {
                                VillageId = village.VillageId,
                                LandId = village.LandId,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });
                            db.SaveChanges();
                            return success;
                        }
                        else
                        {
                            error.Add("Village already associated");
                            return error;
                        }
                    }
                    catch (Exception e)
                    {
                        List<string> exception = new List<string> { "Exception", "An error occured. " + e.Message };
                        return exception;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }

        }

        public List<string> RemoveLandImpactedVillage(VillageViewModel village)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "Village deleted successfully!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV"))
            {

                if (village.LandId == -1 || village.LandId < 1)
                    error.Add("Invalid Land Id. Please refresh the page!");
                if (village.VillageId == -1 || village.VillageId < 1)
                    error.Add("Invalid Village. Please refresh the page and select the village again");

                if (error.Count > 1)
                    return error;
                else
                {
                    try
                    {
                        var landVillage = db.T_LandVillage.FirstOrDefault(l => l.LandId == village.LandId && l.VillageId == village.VillageId);
                        db.T_LandVillage.Remove(landVillage);
                        db.SaveChanges();
                        return success;
                    }
                    catch (Exception e)
                    {
                        List<string> exception = new List<string> { "Exception", "An error occured. " + e.Message };
                        return exception;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> CreateNewVillage(VillageViewModel village)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success","Village inserted successfuly!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV"))
            {

                if (village.RegionId == -1 || village.RegionId < 1)
                    error.Add("Invalid Region Id. Please refresh the page!");
                if (string.IsNullOrWhiteSpace(village.VillageName))
                    error.Add("Village Name can not be empty");
                if (village.VillageStatus == -1 || village.VillageStatus < 1)
                    error.Add("Village Status can not be empty");

                if (error.Count > 1)
                    return error;
                else
                {
                    try
                    {
                        var vil = db.T_Village.FirstOrDefault(v => v.VillageName == village.VillageName && v.RegionId == village.RegionId);
                        if (vil != null)
                        {
                            error.Add("Village already exist in the system!");
                            return error;
                        }
                        else
                        {
                            db.T_Village.Add(new T_Village()
                            {
                                VillageName = village.VillageName,
                                RegionId = village.RegionId,
                                VillageStatus = village.VillageStatus,
                                OldName = village.VillageOldName,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });
                            db.SaveChanges();
                            return success;
                        }
                    }
                    catch (Exception e)
                    {
                        List<string> exception = new List<string> { "Exception", "Error occured when saving the village. " + e.Message };
                        return exception;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        #region CRUD ATTACHMENT
        public List<string> AddAttachement(AttachmentViewModel Attachs, HttpPostedFileBase[] upload)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "Attachment added successfuly!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-REQ") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-REQ"))
            {
                int lacRequestId = 0;
                if (Attachs.LacId == 0 && Attachs.LandId > 0)
                    lacRequestId = db.T_Land.FirstOrDefault(l => l.LandId == Attachs.LandId).LACRequestId;
                if (Attachs.LandId == 0 && Attachs.LacId > 0)
                    lacRequestId = db.T_LAC.FirstOrDefault(l => l.LACId == Attachs.LacId).LACRequestId;
                int requestId = db.T_LACRequest.FirstOrDefault(l => l.LACRequestId == lacRequestId).Requestid;

                foreach (var item in upload)
                {
                    string result = Helpers.SaveAttachment(item, "Land Request", requestId);
                    if (result != "success")
                    {
                        error.Add(result);
                        return error;
                    }
                }
                return success;
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> RemoveAttachement(AttachmentViewModel Attachs)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "Attachment deleted successfuly!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-TOPO") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-INV"))
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var att = db.T_Attachment.Find(Attachs.AttachmentId);
                        var requestAtt = att.T_RequestAttachment;
                        db.T_RequestAttachment.RemoveRange(requestAtt);

                        if (File.Exists(att.RequestAttachementPath))
                            File.Delete(att.RequestAttachementPath);

                        db.T_Attachment.Remove(att);
                        db.SaveChanges();

                        dbTransaction.Commit();
                        return success;
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Rollback();
                        error.Add("An error occurred while removing attachement! " + e.Message);
                        return error;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }
        #endregion

        #region CRUD RESTRICTED AREA
        public List<string> AddNewRestrictedArea(RestrictedAreaViewModel restrictedArea)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "Restricted zone inserted successfuly!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                if (restrictedArea == null)
                    error.Add("This object can not be empty");
                if (string.IsNullOrEmpty(restrictedArea.LandName))
                    error.Add("Restricted Area Name is mandatory");
                if (restrictedArea.LandStatusId < 1)
                    error.Add("Restricted Area Status is mandatory");
                if (restrictedArea.AreaSurface < 0)
                    error.Add("Restricted Area Surface is mandatory");
                if (restrictedArea.AreaSurfaceUOM < 1)
                    error.Add("Area Surface UOM is mandatory");
                //if (restrictedArea.DepartmentId < 1)
                //    error.Add("Choose a valid requestor department");
                if (string.IsNullOrWhiteSpace(restrictedArea.LandComment))
                    error.Add("Please add a valid comment");

                if (error.Count > 1)
                    return error;
                else
                {
                    try
                    {
                        db.T_Land.Add(new T_Land()
                        {
                            LandName = restrictedArea.LandName,
                            LandCategory = db.T_List.FirstOrDefault(l => l.ListName == "Land Category" && l.ListValue == "Restricted Zone").ListId,
                            LACRequestId = -1,
                            LocationId = restrictedArea.LocationId,
                            LandStatus = restrictedArea.LandStatusId,
                            Surface = restrictedArea.AreaSurface,
                            SurfaceUOM = restrictedArea.AreaSurfaceUOM,
                            DepartementId = restrictedArea.DepartmentId,
                            GPS_Date = restrictedArea.GPSDate,
                            Easting = restrictedArea.LandEasting,
                            Northing = restrictedArea.LandNorthing,
                            Comment = restrictedArea.LandComment,
                            Created = DateTime.Now,
                            CreatedBy = loggedUser,
                            Updated = DateTime.Now,
                            UpdatedBy = loggedUser
                        });

                        db.SaveChanges();
                        return success;
                    }
                    catch (Exception e)
                    {
                        List<string> exception = new List<string> { "Exception", "Error occured when saving Restricted Area. " + e.Message };
                        return exception;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> ChangeRAStatus(int LandId)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "Restricted zone status updated!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                if (LandId < 1)
                {
                    error.Add("Please refresh the page and try again!");
                    return error;
                }
                else
                {
                    try
                    {
                        var land = db.T_Land.Find(LandId);
                        int _status = land.LandStatus;
                        int _active = db.T_List.Where(l => l.ListName == "Land Status" && l.ListValue == "Active").Select(l => l.ListId).FirstOrDefault();
                        int _inactive = db.T_List.FirstOrDefault(l => l.ListName == "Land Status" && l.ListValue == "Inactive").ListId;
                        _status = (_status == _active ? _inactive : _active);

                        land.LandStatus = _status;
                        land.Updated = DateTime.Now;
                        land.UpdatedBy = loggedUser;

                        db.Entry(land).State = EntityState.Modified;
                        db.SaveChanges();
                        return success;
                    }
                    catch (Exception e)
                    {
                        List<string> exception = new List<string> { "Exception", "Error occured when saving Restricted Area. " + e.Message };
                        return exception;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> AddImpactedLAC(LACViewModel Lac)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "Restricted zone linked to LAC succesfully!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                if (Lac.LandID < 1 || Lac.LandID == null)
                    error.Add("Invalid Land ID. Please refresh the page and try again");
                if (Lac.LACId < 1)
                    error.Add("Please select a valid LAC");

                if (error.Count > 1)
                    return error;
                else
                {
                    try
                    {
                        var lac = db.T_LAC.Find(Lac.LACId);
                        var land = db.T_Land.Find(Lac.LandID.Value);
                        if (lac != null && land != null)
                        {
                            db.T_LandLAC.Add(new T_LandLAC()
                            {
                                LandID = Lac.LandID.Value,
                                LACID = Lac.LACId,
                                Created = DateTime.Now,
                                CreatedBy = loggedUser,
                                Updated = DateTime.Now,
                                UpdatedBy = loggedUser
                            });
                            db.SaveChanges();
                            return success;
                        }
                        else
                        {
                            error.Add("The specified LAC or Restricted Area does not exists");
                            return error;
                        }
                    }
                    catch (Exception e)
                    {
                        List<string> exception = new List<string> { "Exception", "Error occured when trying to link Restricted Area with a LAC. " + e.Message };
                        return exception;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        public List<string> RemoveImpactedLAC(LACViewModel Lac)
        {
            List<string> error = new List<string> { "Error" };
            List<string> success = new List<string> { "Success", "Unlink LAC and Restricted Area done!" };

            if (Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-ADM") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-POWER") || Roles.IsUserInRole(loggedUser, "CMOCTFM\\SG-FGM-RAP-LO"))
            {
                if (Lac.LandID < 1)
                    error.Add("Invalid Land ID. Please refresh the page and try again");
                if (Lac.LACId < 1)
                    error.Add("Please select a valid LAC");
                if (Lac.LandLacID < 1)
                    error.Add("Please refresh the page");

                if (error.Count > 1)
                    return error;
                else
                {
                    try
                    {
                        var lacLand = db.T_LandLAC.FirstOrDefault(l => l.LACID == Lac.LACId && l.LandID == Lac.LandID && l.LandLACID == Lac.LandLacID);
                        if (lacLand != null)
                        {
                            db.T_LandLAC.Remove(lacLand);
                            db.Entry(lacLand).State = EntityState.Deleted;
                            db.SaveChanges();
                            return success;
                        }
                        else
                        {
                            error.Add("This LAC is not associated to this LAND.");
                            return error;
                        }
                    }
                    catch (Exception e)
                    {
                        List<string> exception = new List<string> { "Exception", "Error occured when trying to unlink LAC to Restricted Area. " + e.Message };
                        return exception;
                    }
                }
            }
            else
            {
                error.Add("You don't have sufficient permissions to perform this task.");
                return error;
            }
        }

        #endregion

        #endregion

        #region LOAD DATA
        public List<AttachmentViewModel> LoadLandAttachement(int ID, string Source)
        {
            int lacRequest = 0;
            if (Source == "Lac")
                lacRequest = db.T_LAC.FirstOrDefault(l => l.LACId == ID).LACRequestId;
            else if (Source == "Land")
                lacRequest = db.T_Land.FirstOrDefault(l => l.LandId == ID).LACRequestId;

            var request = db.T_LACRequest.FirstOrDefault(l => l.LACRequestId == lacRequest).Requestid;
            var requestAttachment = db.T_RequestAttachment.Where(r => r.RequestId == request).Select(r => r.AttachmentId).ToList();
            var attachment = db.T_Attachment.Where(a => requestAttachment.Contains(a.AttachmentId)).Select(a => new AttachmentViewModel()
            {
                AttachmentId = a.AttachmentId,
                RequestAttachementType = a.RequestAttachementType,
                AttachmentType = a.T_List.ListValue,
                RequestAttachementPath = a.RequestAttachementPath,
                RequestAttachementContentType = a.RequestAttachementContentType,
                RequestAttachementFile = a.RequestAttachementFile,
                RequestId = request,
                LandId = ID
            }).ToList();
            return attachment;
        }

        public IEnumerable<LandRequestViewModel> LoadLandRequests()
        {
            int employeeId = Helpers.GetLogInPerson().EmployeeId;
            int reqType = db.T_List.FirstOrDefault(l=>l.ListName == "Request Type" && l.ListValue == "Land Request").ListId;
            int reqStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Cancelled").ListId;
            var requests = db.T_Request.Where(r => r.Requestor == employeeId && r.RequestType == reqType && !r.Locked && r.RequestStatus != reqStatus).ToList().Select(l => new LandRequestViewModel() { 
                LACRequestId = l.T_LACRequest.FirstOrDefault().LACRequestId,
                RequestId = l.RequestId,
                Requestor = l.Requestor,
                RequestorName = l.T_Employee.T_Person.FirstName +" "+ l.T_Employee.T_Person.LastName,
                RegionId = l.T_LACRequest.FirstOrDefault().RegionId,
                RegionName = l.T_LACRequest.FirstOrDefault().T_Region.RegionName,
                ProjectCostCode = l.T_LACRequest.FirstOrDefault().ProjectCostCode,
                //LocationId = l.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationId,
                //LocationName = l.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName,
                RequestedDate = l.RequestedDate,
                RequestStatus = l.T_List.ListValue,
                WorkDescription = l.T_LACRequest.FirstOrDefault().WorkDescription,
                ProjectName = l.T_LACRequest.FirstOrDefault().ProjectName,
                AccessScheduledDate = l.T_LACRequest.FirstOrDefault().AccessScheduledDate,
                AccessDateString = l.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMM/yyyy"),
                IsUrgent = l.T_LACRequest.FirstOrDefault().IsUrgent,
                ContactPersonId = l.T_LACRequest.FirstOrDefault().ContactPerson,
                ContactPerson = l.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName +" " + l.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName
            });

            return requests;
        }

        public LandRequestViewModel GetLandRequest(int LandRequestID)
        {
            try
            {
                var requestId = db.T_LACRequest.FirstOrDefault(l => l.LACRequestId == LandRequestID).Requestid;
                var a = db.T_RequestAttachment.Where(r => r.RequestId == requestId);

                var landId = db.T_Land.FirstOrDefault(l => l.LACRequestId == LandRequestID).LandId;
                List<AttachmentViewModel> attachments = LoadLandAttachement(landId, "Land");

                var point = db.T_Point.Where(p => p.LandId == landId).Select(p => new PointViewModel()
                {
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Elevation = p.Elevation.Value
                }).ToList().ToArray();

                var lacRequest = db.T_LACRequest.Where(l => l.LACRequestId == LandRequestID).Select(r => new LandRequestViewModel()
                {
                    LACRequestId = r.LACRequestId,
                    ProjectName = r.ProjectName,
                    LandID = r.T_Land.FirstOrDefault().LandId,
                    ProjectCostCode = r.ProjectCostCode,
                    RegionId = r.RegionId,
                    RegionName = r.T_Region.RegionName,
                    LocationId = r.T_Land.FirstOrDefault().LocationId,
                    LocationName = r.T_Land.FirstOrDefault().T_Location.LocationName,
                    WorkDescription = r.WorkDescription,
                    AccessScheduledDate = r.AccessScheduledDate,
                    IsUrgent = r.IsUrgent,
                    ContactPersonId = r.ContactPerson,
                    ContactPerson = r.T_Employee.T_Person.FirstName + " " + r.T_Employee.T_Person.LastName + "(" + r.T_Employee.TFMID + ")",
                    RequestId = r.Requestid,
                    Requestor = r.T_Request.Requestor,
                    RequestorName = r.T_Request.T_Employee.T_Person.FirstName + " " + r.T_Request.T_Employee.T_Person.LastName,
                    RequestedDate = r.T_Request.RequestedDate,
                    RequestorDepartment = r.T_Request.T_Employee.T_Department1.DepartmentName,
                    RequestType = r.T_Request.RequestType,
                    RequestTypeName = r.T_Request.T_List1.ListValue,
                    RequestStatus = r.T_Request.T_List.ListValue,
                    RequestStatusID = r.T_Request.RequestStatus
                }).FirstOrDefault();
                lacRequest.AccessDateString = lacRequest.AccessScheduledDate.ToString("yyyy-MM-dd");
                lacRequest.LandRequestPoints = point;
                lacRequest.AttachmentsDocuments = attachments.ToArray();

                return lacRequest;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<LandRequestViewModel> LoadMyApproval()
        {
            var logInPerson = Helpers.GetLogInPerson();
            var _person = db.T_Employee.Where(e => e.DepartmentId == logInPerson.DepartmentId).ToList().Select(e=>e.EmployeeId);
            int reqType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId;
            int reqStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Awaiting Approval").ListId;
            var requests = db.T_Request.Where(r =>_person.Contains(r.Requestor) && r.RequestStatus == reqStatus).ToList().Select(l => new LandRequestViewModel()
            {
                LACRequestId = l.T_LACRequest.FirstOrDefault().LACRequestId,
                RequestId = l.RequestId,
                Requestor = l.Requestor,
                RequestorName = l.T_Employee.T_Person.FirstName + " " + l.T_Employee.T_Person.LastName,
                RegionId = l.T_LACRequest.FirstOrDefault().RegionId,
                RegionName = l.T_LACRequest.FirstOrDefault().T_Region.RegionName,
                //LocationId = l.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationId,
                //LocationName = l.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName,
                RequestedDate = l.RequestedDate,
                RequestStatus = l.T_List.ListValue,
                WorkDescription = l.T_LACRequest.FirstOrDefault().WorkDescription,
                ProjectCostCode = l.T_LACRequest.FirstOrDefault().ProjectCostCode,
                ProjectName = l.T_LACRequest.FirstOrDefault().ProjectName,
                AccessScheduledDate = l.T_LACRequest.FirstOrDefault().AccessScheduledDate,
                AccessDateString = l.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMM/yyyy"),
                IsUrgent = l.T_LACRequest.FirstOrDefault().IsUrgent,
                ContactPersonId = l.T_LACRequest.FirstOrDefault().ContactPerson,
                ContactPerson = l.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + l.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName
            });

            return requests;
        }

        public IEnumerable<LandRequestViewModel> LoadApprovedLandRequest()
        {
            int reqType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId;
            List<int> reqStatus = db.T_List.
                Where(l => l.ListName == "Request Status" && (l.ListValue == "Request Submitted" || l.ListValue == "Topograph Return" || l.ListValue== "Request Resubmitted")).
                Select(lt=>lt.ListId).ToList();

            var requests = db.T_Request.Where(r => reqStatus.Contains(r.RequestStatus) && !r.Locked).ToList().Select(l => new LandRequestViewModel()
            {
                LACRequestId = l.T_LACRequest.FirstOrDefault().LACRequestId,
                RequestId = l.RequestId,
                Requestor = l.Requestor,
                RequestorName = l.T_Employee.T_Person.FirstName + " " + l.T_Employee.T_Person.LastName,
                RequestorDepartment = l.T_Employee.T_Department1.DepartmentName,
                RegionId = l.T_LACRequest.FirstOrDefault().RegionId,
                RegionName = l.T_LACRequest.FirstOrDefault().T_Region.RegionName,
                ProjectCostCode = l.T_LACRequest.FirstOrDefault().ProjectCostCode,
                RequestedDate = l.RequestedDate,
                RequestStatus = l.T_List.ListValue,
                WorkDescription = l.T_LACRequest.FirstOrDefault().WorkDescription,
                ProjectName = l.T_LACRequest.FirstOrDefault().ProjectName,
                AccessScheduledDate = l.T_LACRequest.FirstOrDefault().AccessScheduledDate,
                AccessDateString = l.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMM/yyyy"),
                IsUrgent = l.T_LACRequest.FirstOrDefault().IsUrgent,
                ContactPersonId = l.T_LACRequest.FirstOrDefault().ContactPerson,
                ContactPerson = l.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + l.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName
            });

            return requests;
        }

        public IEnumerable<RestrictedAreaViewModel> LoadRestrictedArea()
        {
            int reqType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId;
            int landCat = db.T_List.FirstOrDefault(l => l.ListName == "Land Category" && l.ListValue == "Restricted Zone").ListId;

            var requests = db.T_Land.Where(l => l.LandCategory == landCat && l.LACRequestId > 0).ToList().Select(l => new RestrictedAreaViewModel() {
                LandId = l.LandId,
                LACRequestId = l.LACRequestId,
                Requestor = l.T_LACRequest.T_Request.Requestor,
                RequestorName = l.T_LACRequest.T_Request.T_Employee.T_Person.FirstName + " " + l.T_LACRequest.T_Request.T_Employee.T_Person.LastName,
                LandName = (string.IsNullOrEmpty(l.LandName) ? l.T_LACRequest.ProjectName: l.LandName),
                RequestedDate = l.T_LACRequest.T_Request.RequestedDate,
                RequestedDateString = l.T_LACRequest.T_Request.RequestedDate.ToString("dd/MMM/yyyy"),
                ContactPersonId = l.T_LACRequest.ContactPerson,
                ContactPerson = l.T_LACRequest.T_Employee.T_Person.FirstName+ " "+ l.T_LACRequest.T_Employee.T_Person.LastName,
                DepartmentId = (int)l.DepartementId,
                DepartmentName = l.T_Department.DepartmentName,
                RegionId = l.T_Location.RegionId,
                RegionName = l.T_Location.T_Region.RegionName,
                LandComment = l.Comment,
                LandStatusId = l.LandStatus,
                LandStatusString = l.T_List.ListValue,
                AreaSurface = l.Surface,
                AreaSurfaceUOM = l.SurfaceUOM.Value,
                AreaSurfaceUOMString = l.T_UOM.UOM,
                AreaSurfaceString = l.Surface + " " + l.T_UOM.UOM,
                GPSDate = l.GPS_Date,
                GPSDateString = l.GPS_Date.ToString("dd/MMM/yyyy"),
                LandEasting = l.Easting,
                LandNorthing = l.Northing
            });

            //Manually created Restricted Zones
            var restricted = db.T_Land.Where(l => l.LandCategory == landCat && l.LACRequestId == -1).ToList().Select(l => new RestrictedAreaViewModel() { 
                LandId = l.LandId,
                LandName = l.LandName,
                DepartmentId = (int)l.DepartementId,
                DepartmentName = l.T_Department.DepartmentName,
                RegionId = l.T_Location.RegionId,
                RegionName = l.T_Location.T_Region.RegionName,
                LandComment = l.Comment,
                LandStatusId = l.LandStatus,
                LandStatusString = l.T_List.ListValue,
                AreaSurface = l.Surface,
                AreaSurfaceUOM = l.SurfaceUOM.Value,
                AreaSurfaceUOMString = l.T_UOM.UOM,
                AreaSurfaceString = l.Surface +" "+ l.T_UOM.UOM,
                GPSDate= l.GPS_Date,
                GPSDateString = l.GPS_Date.ToString("dd/MMM/yyyy"),
                LandEasting = l.Easting,
                LandNorthing = l.Northing, 
                ContactPerson= ""
            });

            return requests.Union(restricted);
        }

        public IEnumerable<LandRequestViewModel> LoadLandForImpactMitigation()
        {
            int reqType = db.T_List.FirstOrDefault(l => l.ListName == "Request Type" && l.ListValue == "Land Request").ListId;
            int reqStatus = db.T_List.FirstOrDefault(l => l.ListName == "Request Status" && l.ListValue == "Topograph Input").ListId;
            var requests = db.T_Request.Where(r => r.RequestStatus == reqStatus).ToList().Select(l => new LandRequestViewModel()
            {
                LACRequestId = l.T_LACRequest.FirstOrDefault().LACRequestId,
                RequestId = l.RequestId,
                Requestor = l.Requestor,
                RequestorName = l.T_Employee.T_Person.FirstName + " " + l.T_Employee.T_Person.LastName,
                RequestorDepartment = l.T_Employee.T_Department1.DepartmentName,
                RegionId = l.T_LACRequest.FirstOrDefault().RegionId,
                RegionName = l.T_LACRequest.FirstOrDefault().T_Region.RegionName,
                ProjectCostCode = l.T_LACRequest.FirstOrDefault().ProjectCostCode,
                //LocationId = l.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationId,
                //LocationName = l.T_LACRequest.FirstOrDefault().T_Land.FirstOrDefault().T_Location.LocationName,
                RequestedDate = l.RequestedDate,
                RequestStatus = l.T_List.ListValue,
                WorkDescription = l.T_LACRequest.FirstOrDefault().WorkDescription,
                ProjectName = l.T_LACRequest.FirstOrDefault().ProjectName,
                AccessScheduledDate = l.T_LACRequest.FirstOrDefault().AccessScheduledDate,
                AccessDateString = l.T_LACRequest.FirstOrDefault().AccessScheduledDate.ToString("dd/MMM/yyyy"),
                IsUrgent = l.T_LACRequest.FirstOrDefault().IsUrgent,
                ContactPersonId = l.T_LACRequest.FirstOrDefault().ContactPerson,
                ContactPerson = l.T_LACRequest.FirstOrDefault().T_Employee.T_Person.FirstName + " " + l.T_LACRequest.FirstOrDefault().T_Employee.T_Person.LastName
            });

            return requests;
        }

        public IEnumerable<LACViewModel> LoadLACList()
        {
            List<int> lacstatus = db.T_List.Where(l => l.ListName == "LAC Status" && (l.ListValue == "LAC Created" || l.ListValue == "Survey Completed" || l.ListValue == "On Hold")).Select(l => l.ListId).ToList() ;
            var lac = db.T_LAC.Where(l => !l.Locked && lacstatus.Contains(l.LACStatus)).ToList().Select(l=> new LACViewModel() 
            {
                LACId= l.LACId,
                LAC_ID = l.LAC_ID,
                LACName = l.LACName,
                LACRequestId = l.LACRequestId,
                LACStatus = l.LACStatus,
                LacStatusString = l.T_List.ListValue,
                Realcosts = l.Realcosts,
                PAPs = l.PAPs,
                AuthorizedAreaSize = l.AuthorizedAreaSize ?? 0,
                AuthorizedAreaSizeUOM = l.AuthorizedAreaSizeUOM ?? 0,
                AuthorizedAreaSizeString = l.AuthorizedAreaSize.HasValue ? (l.AuthorizedAreaSize.Value +" "+ l.T_UOM2?.UOM) : string.Empty,
                AuthorizedAreaSizeUOMDescription = (!l.AuthorizedAreaSizeUOM.HasValue ? "" : l.T_UOM2.UOM),
                AuthorizedDate = l.AuthorizedDate ?? DateTime.Now ,
                AuthorizedDateString = !l.AuthorizedDate.HasValue ? string.Empty: l.AuthorizedDate.Value.ToString("dd/MMM/yyyy"),
                AreaDescription = l.AreaDescription,
                AreaRequested = l.AreaRequested.HasValue ? l.AreaRequested:default,
                AreaRequestedUOM = l.AreaRequestedUOM.HasValue ? l.AreaRequestedUOM : default,
                AreaRequestedUOMString = l.AreaRequestedUOM.HasValue ? l.T_UOM1?.UOM : string.Empty,
                AreaRequestedSize = l.AreaRequested.HasValue ? (l.AreaRequested+" "+l.T_UOM1.UOM): string.Empty,
                AreaCompensed = l.AreaCompensed ?? 0,
                AreaCompensedUOM = l.AreaCompensedUOM ?? 0,
                AreaCompensedUOMString = l.AreaCompensedUOM.HasValue ? l.T_UOM.UOM : string.Empty,
                AreaCompensedString = (!l.AreaCompensed.HasValue ? string.Empty : l.AreaCompensed.Value.ToString()) + " "+ (!l.AreaCompensedUOM.HasValue ? "" : l.T_UOM.UOM),
                CostEstimate = l.CostEstimate,
                Comment = l.Comment,
                RequestDate = l.T_LACRequest.Created,
                RequestDateString = l.T_LACRequest.Created.ToString("dd/MMM/yyyy"),
                RegionName = l.T_LACRequest.T_Region.RegionName,
                Department = l.T_LACRequest.T_Request.T_Employee.T_Department1.DepartmentName
            });
            return lac;
        }

        public IEnumerable<PointViewModel> LoadLandPoint(int landId)
        {
            var point = db.T_Point.Where(p => p.LandId == landId).Select(p => new PointViewModel()
            {
                PointId = p.PointId,
                PointName = p.PointName,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Elevation = p.Elevation.Value,
                LandId = p.LandId
            });
            return point;
        }

        public IEnumerable<VillageViewModel> LoadLandVillage(int landId)
        {
            var village = db.T_LandVillage.Where(v => v.LandId == landId).Select(v => new VillageViewModel()
            {
                VillageId = v.VillageId,
                VillageName = v.T_Village.VillageName,
                RegionId = v.T_Village.RegionId,
                RegionName = v.T_Village.T_Region.RegionName,
                LandId = v.LandId
            });
            return village;
        }

        public RestrictedAreaViewModel RestrictedAreaDetails(int LandID)
        {
            List<AttachmentViewModel> attachments = LoadLandAttachement(LandID,"Land");

            var landVillage = db.T_LandVillage.Where(v => v.LandId == LandID).ToList().Select(v => new VillageViewModel()
            {
                VillageId = v.VillageId,
                VillageName = v.T_Village.VillageName,
                VillageOldName = v.T_Village.OldName,
                VillageStatus = v.T_Village.VillageStatus,
                VillageStatusString = v.T_Village.T_List1.ListValue,
                RegionId = v.T_Village.RegionId,
                RegionName = v.T_Village.T_Region.RegionName
            }).OrderBy(v => v.VillageName).ToArray();

            var landPoints = db.T_Point.Where(p => p.LandId == LandID).ToList().Select(p => new PointViewModel()
            {
                PointId = p.PointId,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Elevation = p.Elevation.Value
            }).ToArray();

            var land = db.T_Land.Where(l=>l.LandId==LandID).ToList().Select(l => new RestrictedAreaViewModel()
            {
                LandId = l.LandId,
                LandName = (string.IsNullOrEmpty(l.LandName)? l.T_LACRequest.ProjectName : l.LandName),
                LandCategory = l.LandCategory,
                LandCategoryString = l.T_List1.ListValue,
                LACRequestId = l.LACRequestId,
                LandStatusId = l.LandStatus,
                LandStatusString = l.T_List.ListValue,
                LocationId = l.LocationId,
                LocationName = l.T_Location.LocationName,
                AreaSurface = l.Surface,
                AreaSurfaceUOM = l.SurfaceUOM.Value,
                AreaSurfaceUOMString = l.T_UOM.UOM,
                AreaSurfaceString = l.Surface + " " + l.T_UOM.UOM,
                DepartmentId = (int)l.DepartementId,
                DepartmentName = l.T_Department.DepartmentName,
                GPSDate = l.GPS_Date,
                GPSDateString = l.GPS_Date.ToString("dd/MMM/yyyy"),
                LandEasting = l.Easting,
                LandNorthing = l.Northing,
                LandComment = l.Comment,
                LandVillage = landVillage,
                LandRequestPoints = landPoints,
                AttachmentsDocuments = attachments.ToArray()
            }).FirstOrDefault();

            return land;
        }

        public IEnumerable<LACViewModel> LoadImpactedLAC (int landId)
        {
            var lac = db.T_LandLAC.Where(l => l.LandID == landId).ToList().Select(l => new LACViewModel()
            {
                LACId = l.LACID,
                LAC_ID = l.T_LAC.LAC_ID,
                LACName = l.T_LAC.LACName,
                LACRequestId = l.T_LAC.LACRequestId,
                LACStatus = l.T_LAC.LACStatus,
                LacStatusString = l.T_LAC.T_List.ListValue,
                Realcosts = l.T_LAC.Realcosts,
                PAPs = l.T_LAC.T_PAPLAC.Count(),
                AuthorizedAreaSize = l.T_LAC.AuthorizedAreaSize == null ? 0 : l.T_LAC.AuthorizedAreaSize.Value,
                AuthorizedAreaSizeUOM = l.T_LAC.AuthorizedAreaSizeUOM == null ? 0 : l.T_LAC.AuthorizedAreaSizeUOM.Value,
                AuthorizedAreaSizeString = (l.T_LAC.AuthorizedAreaSize == null ? 0 : l.T_LAC.AuthorizedAreaSize.Value) + " " + (l.T_LAC.AuthorizedAreaSizeUOM == null ? "" : l.T_LAC.T_UOM2.UOM),
                AuthorizedAreaSizeUOMDescription = (l.T_LAC.AuthorizedAreaSizeUOM == null ? "" : l.T_LAC.T_UOM2.UOM),
                AuthorizedDate = l.T_LAC.AuthorizedDate == null ? DateTime.Now : l.T_LAC.AuthorizedDate.Value,
                AuthorizedDateString = l.T_LAC.AuthorizedDate == null ? "" : l.T_LAC.AuthorizedDate.Value.ToString("dd/MMM/yyyy"),
                AreaDescription = l.T_LAC.AreaDescription,
                AreaRequested = l.T_LAC.AreaRequested,
                AreaRequestedUOM = l.T_LAC.AreaRequestedUOM,
                AreaRequestedUOMString = l.T_LAC.T_UOM1.UOM,
                AreaRequestedSize = l.T_LAC.AreaRequested + " " + l.T_LAC.T_UOM1.UOM,
                AreaCompensed = l.T_LAC.AreaCompensed == null ? 0 : l.T_LAC.AreaCompensed.Value,
                AreaCompensedUOM = l.T_LAC.AreaCompensedUOM == null ? 0 : l.T_LAC.AreaCompensedUOM.Value,
                AreaCompensedUOMString = l.T_LAC.AreaCompensedUOM == null ? "" : l.T_LAC.T_UOM.UOM,
                AreaCompensedString = (l.T_LAC.AreaCompensed == null ? 0 : l.T_LAC.AreaCompensed.Value) + " " + (l.T_LAC.AreaCompensedUOM == null ? "" : l.T_LAC.T_UOM.UOM),
                CostEstimate = l.T_LAC.CostEstimate,
                LandID = l.LandID,
                LandLacID = l.LandLACID,
                Locked = l.T_LAC.Locked,
                Comment = l.T_LAC.Comment,
                RequestDate = l.T_LAC.T_LACRequest.Created,
                RequestDateString = l.T_LAC.T_LACRequest.Created.ToString("dd/MMM/yyyy"),
                RegionName = l.T_LAC.T_LACRequest.T_Region.RegionName,
                Department = l.T_LAC.T_LACRequest.T_Request.T_Employee.T_Department1.DepartmentName
            });

            return lac;
        }

        public IEnumerable<RestrictedAreaViewModel> LoadIncludingRestrictedArea(int lacID)
        {
            var ra = db.T_LandLAC.Where(l => l.LACID == lacID).ToList().Select(l => new RestrictedAreaViewModel() {
                LandLacID = l.LandLACID,
                LacID = lacID,
                LandId = l.LandID,
                LandName = l.T_Land.LandName,
                DepartmentId = (int)l.T_Land.DepartementId,
                DepartmentName = l.T_Land.T_Department.DepartmentName,
                RegionId = l.T_Land.T_Location.RegionId,
                RegionName = l.T_Land.T_Location.T_Region.RegionName,
                LandComment = l.T_Land.Comment,
                LandStatusId = l.T_Land.LandStatus,
                LandStatusString = l.T_Land.T_List.ListValue,
                AreaSurface = l.T_Land.Surface,
                AreaSurfaceUOM = l.T_Land.SurfaceUOM.Value,
                AreaSurfaceUOMString = l.T_Land.T_UOM.UOM,
                AreaSurfaceString = l.T_Land.Surface + " " + l.T_Land.T_UOM.UOM,
                GPSDate = l.T_Land.GPS_Date,
                GPSDateString = l.T_Land.GPS_Date.ToString("dd/MMM/yyyy"),
                LandEasting = l.T_Land.Easting,
                LandNorthing = l.T_Land.Northing,
            });
            return ra;
        }

        public LACViewModel LoadLacDetails(int LacID)
        {
            int lacRequestId = db.T_LAC.FirstOrDefault(l=>l.LACId == LacID).LACRequestId;
            var LandRequest = db.T_LACRequest.Where(l => l.LACRequestId == lacRequestId).ToList().Select(l => new LandRequestViewModel() 
            {
                ProjectName = l.ProjectName,
                ProjectCostCode = l.ProjectCostCode,
                WorkDescription = l.WorkDescription,
                ContactPerson = l.T_Employee.T_Person.FirstName +" "+ l.T_Employee.T_Person.LastName,
                RequestorName = l.T_Request.T_Employee.T_Person.FirstName + " "+ l.T_Request.T_Employee.T_Person.LastName,
                RequestorDepartment = l.T_Request.T_Employee.T_Department1.DepartmentName
            }).FirstOrDefault();

            var pap = db.T_PAPLAC.Where(p => p.LACId == LacID).ToList().Select(p => new PAPViewModel(){});

            var lac = db.T_LAC.Where(l => l.LACId == LacID).ToList().Select(l => new LACViewModel()
            {
                LACId = l.LACId,
                LAC_ID = l.LAC_ID,
                LACName = l.LACName,
                LandID = l.T_LACRequest.T_Land.FirstOrDefault().LandId,
                LACRequestId = l.LACRequestId,
                LACStatus = l.LACStatus,
                LacStatusString = l.T_List.ListValue,
                Realcosts = l.Realcosts,
                PAPs = pap.Count(),
                AuthorizedAreaSize = l.AuthorizedAreaSize == null ? 0 : l.AuthorizedAreaSize.Value,
                AuthorizedAreaSizeUOM = l.AuthorizedAreaSizeUOM == null ? 0 : l.AuthorizedAreaSizeUOM.Value,
                AuthorizedAreaSizeString = l.AreaRequested.HasValue ? (l.AreaRequested + " " + l.T_UOM1.UOM) : string.Empty,
                AuthorizedAreaSizeUOMDescription = (l.AuthorizedAreaSizeUOM == null ? "" : l.T_UOM2.UOM),
                AuthorizedDate = l.AuthorizedDate == null ? DateTime.Now : l.AuthorizedDate.Value,
                AuthorizedDateString = l.AuthorizedDate == null ? "" : l.AuthorizedDate.Value.ToString("dd/MMM/yyyy"),
                AreaDescription = l.AreaDescription,
                AreaRequested = l.AreaRequested.HasValue ? l.AreaRequested : default,
                AreaRequestedUOM = l.AreaRequestedUOM.HasValue ? l.AreaRequestedUOM : default,
                AreaRequestedUOMString = l.AreaRequestedUOM.HasValue ? l.T_UOM1?.UOM : string.Empty,
                AreaRequestedSize = l.AreaRequested.HasValue ? (l.AreaRequested + " " + l.T_UOM1.UOM) : string.Empty,
                AreaCompensed = l.AreaCompensed == null? 0: l.AreaCompensed.Value,
                AreaCompensedUOM = l.AreaCompensed == null? 0:l.AreaCompensedUOM.Value,
                AreaCompensedUOMString = l.AreaCompensedUOM == null? "":l.T_UOM.UOM,
                AreaCompensedString = (l.AreaCompensed == null ? 0 : l.AreaCompensed.Value) + " " + (l.AreaCompensedUOM == null ? "" : l.T_UOM.UOM),
                CostEstimate = l.CostEstimate,
                Comment = l.Comment,
                RequestDate = l.T_LACRequest.Created,
                RequestDateString = l.T_LACRequest.Created.ToString("dd/MMM/yyyy"),
                RegionName = l.T_LACRequest.T_Region.RegionName,
                Department = l.T_LACRequest.T_Request.T_Employee.T_Department1.DepartmentName,
                LandRequest = LandRequest,
                PapLac = pap.ToArray(),
                AttachmentsDocuments = LoadLandAttachement(LacID, "Lac").ToArray()
            }).FirstOrDefault();
            return lac;
        }

        #endregion

        #region HELPERS
        public void LandRequestNotification(string _user, LandRequestViewModel LandRequest)
        {
            try
            {
                int deptId = Helpers.GetLogInPerson().DepartmentId;
                var dept = db.T_Department.Find(deptId);
                string approverName = dept.T_Employee?.T_Person?.FirstName + " " + dept.T_Employee?.T_Person?.LastName;
                string filePathRequest = "";
                string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                EmailNotificationViewModel notificationRequest = new EmailNotificationViewModel();

                if (_user == "Requestor")
                {
                    filePathRequest = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestToRequestor.html");
                    link += "/Land/MyRequest";

                    if (Helpers.GetLogInPerson().Email != null)
                    {
                        notificationRequest.To.Add(Helpers.GetLogInPerson().Email);
                        notificationRequest.Subject = "Your Land Request has been created";
                    }
                }
                else if (_user == "Approver" && dept.T_Employee?.T_Person?.Email != null)
                {
                    filePathRequest = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestToApprover.html");
                    link += "/Land/MyApproval";

                    if (dept.T_Employee?.T_Person?.Email != null)
                    {
                        notificationRequest.To.Add(dept.T_Employee?.T_Person?.Email);
                        notificationRequest.Subject = "A new Land Request has been created";
                    }
                }
                else if (_user == "Land")
                {
                    filePathRequest = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestToLand.html");
                    notificationRequest.To.Add("SG-FGM-RAP-LO@tfm.cmoc.com");
                    notificationRequest.Subject = "A new Land Request has been created";
                    link += "/Land/LandStatus";
                }
                else if (_user == "RAP")
                {
                    filePathRequest = HttpContext.Current.Server.MapPath("~/EmailTemplates/LandRequestToRAPMgm.html");
                    notificationRequest.To.Add("SG-FGM-RAP-MGT@tfm.cmoc.com");
                    notificationRequest.To.Add("SG-FGM-RAP-POWER@tfm.cmoc.com");
                    notificationRequest.Subject = "A new Land Request has been created";
                }

                StreamReader strRequest = new StreamReader(filePathRequest);
                string mailTextRequest = strRequest.ReadToEnd();
                strRequest.Close();

                mailTextRequest = mailTextRequest.Replace("[requestorName]", Helpers.GetLogInPerson().FirstName + " " + Helpers.GetLogInPerson().LastName);
                mailTextRequest = mailTextRequest.Replace("[requestType]", "Land Request");
                mailTextRequest = mailTextRequest.Replace("[requestDate]", DateTime.Now.ToString("dd/MMMM/yyyy"));
                mailTextRequest = mailTextRequest.Replace("[requestStatus]", "Saved");
                mailTextRequest = mailTextRequest.Replace("[approver]", approverName);
                mailTextRequest = mailTextRequest.Replace("[projectName]", LandRequest.ProjectName);
                mailTextRequest = mailTextRequest.Replace("[locationName]", LandRequest.LocationName);
                mailTextRequest = mailTextRequest.Replace("[scheduleDate]", LandRequest.AccessScheduledDate.ToString("dd/MMMM/yyyy"));
                mailTextRequest = mailTextRequest.Replace("[contactPerson]", LandRequest.ContactPerson);
                mailTextRequest = mailTextRequest.Replace("[requestDetails]", LandRequest.WorkDescription);
                mailTextRequest = mailTextRequest.Replace("[requestLink]", link);
                mailTextRequest = mailTextRequest.Replace("[year]", DateTime.Now.Year.ToString());

                notificationRequest.Body = mailTextRequest;
                Helpers.SendEmail(notificationRequest);
            }
            catch (Exception)
            {

            }
        }

        #endregion
    }
}