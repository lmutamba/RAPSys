var formData = new FormData();
$(document).ready(function () {
    
    //#region Initialize fields and select box and modals
    $("#newLandRequest").on("click", function () {
        //getLocationForSelect("newLandRequestModal #projectLocation", '-1');
        getRegionForSelect("newLandRequestModal #projectRegion", '-1');
        getContactPerson("#newLandRequestModal #projectContactPerson");
        
        //Reset all fields
        $("#newLandRequestModal #projectUploadFileName").empty();
        $("#newLandRequestModal :input").val('');
        $("#newLandRequestModal :input").prop("disabled", false);
        $("#newLandRequestModal #projectDate").val(getCurrentDateDay(7));
        $("#newLandRequestModal #projectCreateRequest").show();
        $("#newLandRequestModal #projectGPS, #newLandRequestModal #projectMAP, #newLandRequestModal #projectAFE").fileinput('clear');
        $("#newLandRequestModal #projectRequestError, #newLandRequestModal #projectUpdateRequest, #newLandRequestModal #fileAttachments, #newLandRequestModal #divProjectAFE").hide();
        $("#newLandRequestModal").appendTo("body").modal("show");
    });

    $("#newLandRequestModal #projectAttachment").on("change", function () {
        $("#newLandRequestModal #projectUploadFileName").empty();
        var fp = $("#newLandRequestModal #projectAttachment");
        var lg = fp[0].files.length; // get length
        var items = fp[0].files;
        var fragment = "<ul>";
        if (lg > 0) {
            for (var i = 0; i < lg; i++) {
                var fileName = items[i].name;
                fragment += "<li>" + fileName + "</li>";
            }
            fragment += "</ul>"
            $("#newLandRequestModal #projectUploadFileName").append(fragment);
        }
    });

    $("#newLandRequestModal :input").on("focus", function () { $("#newLandRequestModal #projectRequestError").hide(); });
    $("#newLandRequestModal :input").on("change", function () { $("#newLandRequestModal #projectRequestError").hide(); });
    //#endregion

    //#region Request Details Buttons
    $("#greenLight").on("click", function () {
        let RequestId = $("#lacRequestIDField").val();
        $("#actionModal #actionModalErrors, #actionModal #actionForward").hide();
        $("#actionModal .modal-title").html("Authorize Land Request");
        $("#actionModal #actionGreenLight").show();
        $("#actionModal #requestId").val(RequestId);
        $("#actionModal").modal("show");
        return false;
    });

    $("#rejectRequest").on("click", function () {
        RejectLandRequest($("#lacRequestIDField").val());
    });

    $("#forwardRequest").on("click", function () {
        //let row = $(this).closest("tr");
        let RequestId = $("#lacRequestIDField").val();

        $("#actionModal #actionModalErrors, #actionModal #actionGreenLight").hide();
        $("#actionModal .modal-title").html("Send for Impact and Mitigation");
        $("#actionModal #actionForward").show();
        $("#actionModal #requestId").val(RequestId);
        $("#actionModal").modal("show");
        return false;
        //ForwardToTopograph($("#lacRequestIDField").val())
    });

    $("#createLACDetails").on("click", function () {
        getSurfaceUOM("createLACModal #AuthorizedAreaSizeUOM", '-1');
        getSurfaceUOM("createLACModal #AreaRequestedUOM", '-1');
        $("#createLACModal #createLACModalErrors").hide();
        $("#createLACModal #AuthorizedDate").val(getCurrentDate());
        $("#createLACModal #LandRequestId").val($("#lacRequestIDField").val());
        $("#createLACModal").modal("show");
    });

    $("#returnLO").on("click", function () {
        returnRequestLO($("#lacRequestIDField").val());
    });

    $("#makeRestricted").on("click", function () {
        getSurfaceUOM("makeRestrictedModal #AreaRequestedUOM", '-1');
        getZIStatusForSelect("makeRestrictedModal #zoneStatus", '-1');
        $("#makeRestrictedModal #makeRestrictedModalErrors").hide();
        $("#makeRestrictedModal #AuthorizedDate").val(getCurrentDate());
        $("#makeRestrictedModal #LandRequestId").val($("#lacRequestIDField").val());
        $("#makeRestrictedModal").modal("show");
        return false;
    });

    $("#sendToRequestor").on("click", function () {
        let RequestId = $("#lacRequestIDField").val();
        let url = $(location).attr('pathname');

        $.confirm({
            type: 'dark',
            title: 'Send to Requestor',
            content: '' +
                '<form action="" class="formComments">' +
                '<div class="form-group">' +
                '<textarea rows="3" class="comments autogrow form-control transition-height" placeholder="Add a comment here" style="overflow: hidden; overflow-wrap: break-word;resize: none;"></textarea>' +
                '<br /><label for="lacMap"> Add here the correct MAP(pdf) </label>'+
                '<input id="lacMap" name="lacMap" type="file" data-show-preview="false" data-theme="fas" data-show-upload="false" data-show-cancel="false">' +
                '</div>' +
                '<br /><label for="lacAFE"> Add AFE </label>' +
                '<input id="lacAFE" name="lacAFE" type="file" data-show-preview="false" data-theme="fas" data-show-upload="false" data-show-cancel="false">' +
                '</div>' +
                '</form>',
            buttons: {
                formSubmit: {
                    text: 'Return',
                    btnClass: 'btn-blue',
                    action: function () {
                        var comments = this.$content.find('.comments').val();
                        var afe = $("#lacAFE")[0];
                        var map = $("#lacMap")[0];
                        var afeFile = afe.files;
                        var mapFile = map.files;

                        if (!comments) {
                            $.alert('Provide a valid comment');
                            return false;
                        }
                        if (afe.files.length === 0 && map.files.length === 0) {
                            $.alert('You must attach the AFE file');
                            return false;
                        }

                        var formData = new FormData();

                        if (afe.files.length > 0) 
                            formData.append(afeFile[0].name, afeFile[0]);

                        if (map.files.length > 0)
                            formData.append(mapFile[0].name, mapFile[0]);

                        formData.append("RequestID", RequestId);
                        formData.append("Comments", comments);

                        $.ajax({
                            type: "POST",
                            url: "/Land/SendRequestor/",
                            data: formData,
                            contentType: false,
                            processData: false,
                            success: function (response) {
                                if (response[0] == "Success") {
                                    if (url != "/LandManagement/LandStatus") {
                                        window.location.href = "/LandManagement/LandStatus";
                                    }
                                    LoadApprovedLandRequest();
                                    $.notify(response[1], "success");
                                } else if (response[0] == "Error") {
                                    $.notify(response[1], "error");
                                } else {
                                    $.notify(response[1], "error");
                                }
                                console.log(response);
                            }
                        });
                    }
                },
                cancel: function () {
                    //close
                },
            }
        });
    });

    //#endregion

    //#region Restricted Zone Details
    $("#addImpactedLAC").on("click", function () {
        let landId = $("#landIdField").val();
        $("#addImpactedLACModal #LandID").val(landId);
        $("#addImpactedLACModal #addImpactedLACModalErrors").hide();
        getLACForSelect("addImpactedLACModal #LacID", '-1');
        console.log("Land ID " + landId);
        $("#addImpactedLACModal").modal("show");
    });
    //#endregion

    //#region Lac Details
    $("#addToRestrictedArea").on("click", function () {
        let lacID = $("#lacIdField").val();
        
        $("#addImpactedLACModal #LacID").val(lacID);
        $("#addImpactedLACModal #addImpactedLACModalErrors").hide();
        getRestrictedAreaForSelect("addImpactedLACModal #LandID", '-1');
        $("#addImpactedLACModal").modal("show");
    });

    $("#addPapToLac").on("click", function () {
        let lacId = $("#lacIdField").val();
        let lacName = "LAC" + lacId;

        launchModalWizard(lacId, lacName);
    });
    //#endregion

    //#region Impacted Village
    $("#addNewVillage").on("click", function () {
        getRegionForSelect("newVillageModal #RegionId", '-1');
        getVillageStatus("newVillageModal #villageStatus", '-1');
        $("#newVillageModal #villageOldName").val('');
        $("#newVillageModal #villageName").val('');
        $("#newVillageModal #newVillageModalErrors").hide();
        $("#newVillageModal").modal("show");
    });

    $("#newVillageModal #addVillage").on("click", function () {
        let villageId = $("#newVillageModal #villageId").val();
        let villageName = $("#newVillageModal #villageName").val();
        let regionId = $("#newVillageModal #RegionId").val();
        let villageStatus = $("#newVillageModal #villageStatus").val();
        let villageOldName = $("#newVillageModal #villageOldName").val();

        var village = {
            VillageId: villageId,
            VillageName: villageName,
            RegionId: regionId,
            VillageStatus: villageStatus,
            VillageOldName: villageOldName
        }

        $.ajax({
            type: "POST",
            url: "/Land/CreateNewVillage",
            data: JSON.stringify(village),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response[0] == "Error") {
                    $.notify(response[1], "error");
                } else if (response[0] == "Exception") {
                    $.notify(response[1], "error");
                    console.log(response[2]);
                }
                else {
                    $.notify(response[1], "success");
                    $("#newVillageModal #villageOldName").val('');
                    $("#newVillageModal #villageStatus").val('-1');
                    $("#newVillageModal #villageName").val('');
                    $("#newVillageModal #RegionId").val('-1');
                }
            },
            error: function (error) {
                console.log(error);
                $.notify("Something went wrong while updating the information", "error");
            }
        });
    });

    $("#tableVillage").on("click", "#deleteVillage", function () {
        let row = $(this).closest("tr");
        let villageId = row.find(".villageId").val();
        let LacRequestId = $("#lacRequestIDField").val();
        let landId = row.find(".landId").val();

        var village = {
            LacRequestId: LacRequestId,
            VillageId: villageId,
            LandId: landId
        }

        $.confirm({
            title: 'Confirm!',
            content: 'Do you really want to delete this village from the requested land?',
            buttons: {
                confirm: {
                    btnClass: 'btn-success',
                    action: function () {
                        $.ajax({
                            type: "POST",
                            url: "/Land/RemoveLandImpactedVillage/",
                            data: JSON.stringify(village),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                if (response[0] == "Error") {
                                    $.notify(response[1], "error");
                                } else if (response[0] == "Exception") {
                                    $.notify(response[1], "error");
                                    console.log(response[2]);
                                }
                                else {
                                    $.notify(response[1], "success");
                                    loadVillageTable(landId);
                                }
                            },
                            error: function (error) {
                                console.log(error);
                                $.notify("Something went wrong while updating the information", "error");
                            }
                        });
                    }
                },
                cancel: {
                    btnClass: 'btn-danger',
                    action: function () {
                        $.notify("Request was not cancelled!", "info");
                    }
                }
            }
        });
    });

    $("#addImpactedVillage").on("click", function () {
        let lacRequestID = $("#lacRequestIDField").val();
        let landID = $("#landIdField").val();
        GetVillageForSelect("addImpactedVillageModal #villageId", '-1');
        $("#addImpactedVillageModal #addImpactedVillageModalErrors").hide();
        $("#addImpactedVillageModal #LandRequestId").val(lacRequestID);
        $("#addImpactedVillageModal #LandId").val(landID);
        $("#addImpactedVillageModal").modal("show");
    });

    $("#addImpactedVillageModal #addVillage").on("click", function () {
        let villageId = $("#addImpactedVillageModal #villageId").val();
        let lacRequestId = $("#addImpactedVillageModal #LandRequestId").val();
        let landId = $("#addImpactedVillageModal #LandId").val();

        var village = {
            LacRequestId: lacRequestId,
            VillageId: villageId,
            LandId: landId
        }

        $.ajax({
            type: "POST",
            url: "/Land/AddLandImpactedVillage",
            data: JSON.stringify(village),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response[0] == "Error") {
                    $.notify(response[1], "error");
                } else if (response[0] == "Exception") {
                    $.notify(response[1], "error");
                    console.log(response[2]);
                }
                else {
                    $.notify(response[1], "success");
                    loadVillageTable(landId);
                    $("#addImpactedVillageModal #villageId").val('-1');
                }
            },
            error: function (error) {
                console.log(error);
                $.notify("Something went wrong while updating the information", "error");
            }
        });
    });
    //#endregion

    //#region Land Point
    $("#tablePoint").on("dblclick", ".editValue", function () {
        var cell = $(this);
        var row = $(this).closest("tr");
        var span = cell.find(".editValueSpan");

        var input;

        if (cell.find(".editInTable").length) {
            input = cell.find(".editInTable");
            var currentValue = span.html().trim();
            span.hide();
            input.val(currentValue).show().focus();
        }
        return false;
    });

    $("#tablePoint").on("click", ".deletePoint", function () {
        var row = $(this).closest("tr");

        var pointId = row.find(".PointId").val();
        var landId = row.find(".landId").val();

        $.confirm({
            title: 'Confirm!',
            content: 'Do you really want to cancel this request? This is irreversible',
            buttons: {
                confirm: {
                    btnClass: 'btn-success',
                    action: function () {
                        if (pointId > 0 && pointId != "" && landId > 0 && landId != "") {
                            var points = {
                                PointId: pointId,
                                LandId: landId
                            }
                            console.log(points);
                            $.ajax({
                                type: "POST",
                                url: "/Land/DeletePoint",
                                data: JSON.stringify(points),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (response) {
                                    if (response[0] == "Error") {
                                        $.notify(response[1], "error");
                                    } else if (response[0] == "Exception") {
                                        $.notify(response[1], "error");
                                        console.log(response[2]);
                                    }
                                    else {
                                        loadPointTable(landId);
                                    }
                                },
                                error: function (error) {
                                    console.log(error);
                                    $.notify("Something went wrong while updating the information", "error");
                                }
                            });
                        }
                    }
                },
                cancel: {
                    btnClass: 'btn-danger',
                    action: function () {
                        $.notify("Point not deleted", "info");
                    }
                }
            }
        });
    });

    $("#tablePoint").on("blur", ".editInTable", function () {
        var cell = $(this).closest("td");
        var row = $(this).closest("tr");

        var span = cell.find(".editValueSpan");
        var input = cell.find(".editInTable");

        var currentValue = span.html().trim();

        var pointId = row.find(".PointId").val();
        var landId = row.find(".landId").val();

        if (input.val().trim() != currentValue && input.val().trim() != "") {
            var points = {
                PointId: pointId,
                PointName: row.find('td:eq(1)').find("input").val(),
                Latitude: row.find('td:eq(2)').find("input").val(),
                Longitude: row.find('td:eq(3)').find("input").val(),
                Elevation: row.find('td:eq(4)').find("input").val(),
                LandId: landId
            }

            console.log(points);

            $.ajax({
                type: "POST",
                url: "/Land/UpdatePoint",
                data: JSON.stringify(points),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response[0] == "Error") {
                        $.notify(response[1], "error");
                    } else if (response[0] == "Exception") {
                        $.notify(response[1], "error");
                        console.log(response[2]);
                    }
                    else {
                        span.html(input.val().trim());
                        loadPointTable(landId);
                    }
                },
                error: function (error) {
                    console.log(error);
                    $.notify("Something went wrong while updating the information", "error");
                }
            });
        }
        input.hide();
        span.show();

        return false;
    });
    //#endregion

    //#region Attachments
    $("#newLandRequestModal #tableAttachment #downloadFiles").on("click", function () {
        let row = $(this).closest("tr");
        let attachmentID = row.find(".attachmentID").val();

        $.dialog(attachmentID);
        return false;
        //$.ajax(
        //    {
        //        url: "/Helper/DownloadAttachment/",
        //        contentType: 'application/json; charset=utf-8',
        //        datatype: 'json',
        //        data: { attachmentId: attachmentID },
        //        type: "GET",
        //        success: function (response) {
        //            if (response == "Not found")
        //                $.notify("The requested file doesn't exists. Please make sure that it can be find at that location!", "error");
        //            else
        //                window.location = "/Helper/DownloadAttachment?attachmentId=" + attachmentID;
        //        }
        //    });
    });

    $("#tableAttachment #downloadFile").on("click", function () {
        let row = $(this).closest("tr");
        let attachmentID = row.find(".attachmentID").val();

        $.ajax(
            {
                url: "/Helper/DownloadAttachment/",
                contentType: 'application/json; charset=utf-8',
                datatype: 'json',
                data: { attachmentId: attachmentID },
                type: "GET",
                success: function (response) {
                    if (response == "Not found")
                        $.notify("The requested file doesn't exists. Please make sure that it can be find at that location!", "error");
                    else
                        window.location = "/Helper/DownloadAttachment?attachmentId=" + attachmentID;
                }
            });
    });

    $("#tableAttachment #removeFile").on("click", function () {
        let row = $(this).closest("tr");
        let attachmentID = row.find(".attachmentID").val();
        let LandID = row.find(".landID").val();
        let lacID = row.find(".lacID").val();
        let attachment = {
            AttachmentId: attachmentID,
            LandId: LandID,
            LacId: lacID
        }

        $.confirm({
            type: 'red',
            title: 'Remove Attachment',
            content: 'Do you really want to delete this attachment? This is irreversible',
            buttons: {
                formSubmit: {
                    text: 'Remove',
                    btnClass: 'btn-danger',
                    action: function () {
                        $.ajax({
                            type: "POST",
                            url: "/Land/RemoveAttachement/",
                            data: JSON.stringify(attachment),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                if (response[0] == "Success") {
                                    location.reload();
                                    $.notify(response[1], "success");
                                } else if (response[0] == "Error") {
                                    $.notify(response[1], "error");
                                } else {
                                    $.notify(response[1], "error");
                                }
                                console.log(response);
                            }
                        });
                    }
                },
                cancel: function () {
                    //close
                },
            }
        });
    });

    $("#addAttachment").on("click", function () {
        let lacId = $("#lacIdField").val();
        let landId = $("#landIdField").val();
        $("#addAttachmentModal #LandId").val(landId);
        $("#addAttachmentModal #LacId").val(lacId);
        $("#addAttachmentModal #addAttachmentModalErrors").hide();

        $("#addAttachmentModal").modal("show");
    });

    $("#addAttachmentModal #addAttachments").on("click", function () {
        let LacId = $("#addAttachmentModal #LacId").val();
        let LandId = $("#addAttachmentModal #LandId").val();

        var att = $("#addAttachmentModal #fileNameList")[0];
        var formData = new FormData();

        for (var i = 0; i < att.files.length; i++) {
            formData.append(att.files[i].name, att.files[i]);
        }
        if (LacId == null || LacId == "")
            LacId = 0;
        if (LandId == null || LandId == "")
            LandId = 0;
        formData.append("LandId", LandId);
        formData.append("LacId", LacId);

        console.log("Land Id " + LandId + " Lac ID: " + LacId + " Files Count " + att.files.length);
        //return false;

        $.ajax({
            type: "POST",
            url: "/Land/AddAttachment",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                console.log(response);
                if (response[0] == "Error") {
                    var errorMessage = "<ul>";

                    $.each(response.slice(1), function (index, value) {
                        errorMessage += "<li>" + value + "</li>";
                    });
                    errorMessage += "</ul>";
                    $("#addAttachmentModal #addAttachmentModalErrors").html(errorMessage).show(250);

                } else if (response[0] == "Exception") {
                    $.notify(response[1], "error");
                }
                else {
                    $.notify(response[1], "success");
                    $("#addAttachmentModal").modal("hide");
                    location.reload();
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    });

    //#endregion

    //#region PAP Details
    $("#tablePAPProperties").on("click", "#deleteProperty", function () {
        let row = $(this).closest("tr");
        let papID = $("#papIdField").val();
        
        let ownerID = row.find(".ownerId").val();
        let userID = row.find(".userId").val();
        let lacID = row.find(".lacId").val();
        let propertyID = row.find(".propertyId").val();

        let property = {
            PropertyId: propertyID,
            Owner: ownerID,
            User: userID
        };

        let pap = {
            Properties: property,
            PAPId: papID,
            LACId: lacID
        };
        console.log(pap);

        $.confirm({
            title: 'Confirm!',
            content: 'Do you really want to delete this property for this ' + papID + '?',
            buttons: {
                confirm: {
                    btnClass: 'btn-success',
                    action: function () {
                        $.ajax({
                            type: "POST",
                            url: "/Community/UnlinkPAPProperty/",
                            data: JSON.stringify(pap),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                if (response[0] == "Error") {
                                    $.notify(response[1], "error");
                                } else if (response[0] == "Exception") {
                                    $.notify(response[1], "error");
                                    console.log(response[2]);
                                }
                                else {
                                    $.notify(response[1], "success");
                                    LoadPapLacProperties(papID);
                                    LoadPapLacDetails(papID);
                                }
                            },
                            error: function (error) {
                                console.log(error);
                                $.notify("Something went wrong while updating the information", "error");
                            }
                        });
                    }
                },
                cancel: {
                    btnClass: 'btn-danger',
                    action: function () {
                        $.notify("Request was not cancelled!", "info");
                    }
                }
            }
        });
    });

    $("#tablePAPProperties").on("click", "#editProperty", function () {
        //editPropertiesModal
        $.alert("Edit Property to Add Topograph details");
        let papID = $("#papIdField").val();
        let personID = $("#personIdField").val();

        //$.alert(" PAP ID " + papID + " Person ID " + personID);
        resetModal('editPropertiesModal');
        $("#editPropertiesModal #propertyPapID").val(papID);
        $("#editPropertiesModal #propertyPersonID").val(personID);

        getLACForSelect("editPropertiesModal #propertyLacID", '-1');
        getLocations("editPropertiesModal #ownerPrimaryResidence");
        getListDetails("editPropertiesModal #murCode", "-1", "Wall");
        getListDetails("editPropertiesModal #toitCode", "-1", "Roof");
        getListDetails("editPropertiesModal #solCode", "-1", "Floor");
        getTreeMaturityForSelect("editPropertiesModal #treeMaturity", "-1");
        getFieldTypeForSelect("editPropertiesModal #fieldType", "-1");
        getStructureUsageForSelect("editPropertiesModal #structureUsage", "-1");
        getStructureForSelect("editPropertiesModal #structureCode", "-1");
        getAssetTypeForSelect("editPropertiesModal #goodType", "-1");
        getCultureType("#editPropertiesModal #cultureName");
        getEmployeeByRole('#editPropertiesModal #topographCode', 'Topograph');
        getPAPOwner("#editPropertiesModal #ownerFirstName");
        $("#editPropertiesModal #topoDate").val(getCurrentDate());
        $("#editPropertiesModal #tableGoodsError, #editPropertiesModal #papDetailsError, #editPropertiesModal #papSurveyorError").hide();
        formData = new FormData();

        $("#editPropertiesModal").modal("show");
    });

    $("#addProperty").on("click", function () {
        let papID = $("#papIdField").val();
        let personID = $("#personIdField").val();

        //$.alert(" PAP ID " + papID + " Person ID " + personID);
        resetModal('addPAPPropertiesModal');
        $("#addPAPPropertiesModal #propertyPapID").val(papID);
        $("#addPAPPropertiesModal #propertyPersonID").val(personID);

        getLACForSelect("addPAPPropertiesModal #propertyLacID", '-1');
        getLocations("addPAPPropertiesModal #ownerPrimaryResidence");
        getListDetails("addPAPPropertiesModal #murCode", "-1","Wall");
        getListDetails("addPAPPropertiesModal #toitCode", "-1","Roof");
        getListDetails("addPAPPropertiesModal #solCode", "-1","Floor");
        getTreeMaturityForSelect("addPAPPropertiesModal #treeMaturity", "-1");
        getFieldTypeForSelect("addPAPPropertiesModal #fieldType", "-1");
        getStructureUsageForSelect("addPAPPropertiesModal #structureUsage", "-1");
        getStructureForSelect("addPAPPropertiesModal #structureCode", "-1");
        getAssetTypeForSelect("addPAPPropertiesModal #goodType", "-1");
        getCultureType("#addPAPPropertiesModal #cultureName");
        getEmployeeByRole("#addPAPPropertiesModal #propertyPresurveyorCode", 'Surveyor');
        getPAPOwner("#addPAPPropertiesModal #ownerFirstName");
        $("#addPAPPropertiesModal #presurveyorDate").val(getCurrentDate());
        $("#addPAPPropertiesModal #tableGoodsError, #addPAPPropertiesModal #papDetailsError, #addPAPPropertiesModal #papSurveyorError").hide();
        formData = new FormData();

        $("#addPAPPropertiesModal").modal("show");
    });

    $("#editPAPDetails").on("click", function () {
        let papID = $("#papIdField").val();
        let personID = $("#personIdField").val();

        $("#addPAPModal #papDetailsError, #addPAPModal #divLinkPAPToLAC, #addPAPModal #saveNewPap").hide();
        $("#addPAPModal #addPAPModalTitle").html("Edit PAP " + papID);
        $("#addPAPModal #updatePap, #addPAPModal #papPicture").show();
        $("#addPAPModal .reset").val('');
        $('#addPAPModal input[name=gender]').prop('checked', false);
        $("#addPAPModal #personID").val(personID);
        $("#addPAPModal #papID").val(papID);
        LoadPAPDetails(papID, "addPAPModal");
        $("#addPAPModal").modal("show");
    });

    $("#linkPapLAC").on("click", function () {
        let papID = $("#papIdField").val();

        $("#linkPAPLACModal :input, #linkPAPLACModal #papLacComment").val("");
        getLACForSelect("linkPAPLACModal #lacID", '-1');
        getEmployeeByRole("#linkPAPLACModal #linkpresurveyorCode", 'Surveyor');
        getEmployeeByRole("#linkPAPLACModal #linksurveyorCode", 'Surveyor');
        $("#linkPAPLACModal #surveyDate, #linkPAPLACModal #presurveyDate, #linkPAPLACModal #submissionDate").val(getCurrentDate());
        getPaymentPreferencesForSelect("linkPAPLACModal #paymentPreference", '-1');
        $("#linkPAPLACModal #linkPAPLACModalError").hide();
        $("#linkPAPLACModal #linkPAPID").val(papID);
        $("#linkPAPLACModal").modal("show");
    });

    $("#tablePAPLacList").on("click", "#unlinkPAPLAC", function () {
        let row = $(this).closest("tr");
        let papLacId = row.find(".papLacId").val();
        let papId = row.find(".papId").val();
        let lacId = row.find(".lacId").val();

        let pap = {
            PAPId: papId,
            LACId: lacId,
            PAPLACId: papLacId
        };

        $.confirm({
            title: 'Confirm!',
            content: 'Do you really want to unlink this PAP ' + papId + ' from the LAC LAC' + lacId + '?',
            buttons: {
                confirm: {
                    btnClass: 'btn-success',
                    action: function () {
                        $.ajax({
                            type: "POST",
                            url: "/Community/UnlinkPAPLAC/",
                            data: JSON.stringify(pap),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                if (response[0] == "Error") {
                                    $.notify(response[1], "error");
                                } else if (response[0] == "Exception") {
                                    $.notify(response[1], "error");
                                    console.log(response[2]);
                                }
                                else {
                                    $.notify(response[1], "success");
                                    LoadPapLacDetails(papId);
                                    LoadPapLacProperties(papId);
                                }
                            },
                            error: function (error) {
                                console.log(error);
                                $.notify("Something went wrong while updating the information", "error");
                            }
                        });
                    }
                },
                cancel: {
                    btnClass: 'btn-danger',
                    action: function () {
                        $.notify("Request was not cancelled!", "info");
                    }
                }
            }
        });
    });
    //#endregion

    //#region PAP LIST
    $("#addPAP").on("click", function () {
        $("#addPAPModal #papLacDetails").hide();
        $("#addPAPModal .reset").val('');
        $('#addPAPModal input[name=gender]').prop('checked', false);
        getEmployeeByRole("#addPAPModal #presurveyorCode", 'Surveyor');
        getEmployeeByRole("#addPAPModal #surveyorCode", 'Surveyor');
        getLocations("#addPAPModal #primaryResidence");
        getVulnerabilityForSelect("addPAPModal #vulnerabilityType", "-1");
        getPAP("#addPAPModal #papSpouse");
        getPAP("#addPAPModal #papMother");
        getPAP("#addPAPModal #papfather");
        getLACForSelect("addPAPModal #lacID", '-1');
        getPaymentPreferencesForSelect("addPAPModal #paymentPreference", '-1');
        $("#addPAPModal #surveyDate, #addPAPModal #presurveyDate, #addPAPModal #submissionDate").val(getCurrentDate());
        $("#addPAPModal #papDetailsError, #addPAPModal #updatePap, #addPAPModal #papPicture").hide();
        $("#addPAPModal #saveNewPap").show();
        $("#addPAPModal #addPAPModalTitle").html("Add new PAP");

        $("#addPAPModal").modal("show");
    });

    $("#datatablePapList, #datatableLacPapList").on("click", "#linkPapLAC", function () {
        let row = $(this).closest("tr");
        let papID = row.find(".papID").val();

        $("#linkPAPLACModal :input, #linkPAPLACModal #papLacComment").val("");
        getLACForSelect("linkPAPLACModal #lacID", '-1');
        getEmployeeByRole("#linkPAPLACModal #linkpresurveyorCode", 'Surveyor');
        getEmployeeByRole("#linkPAPLACModal #linksurveyorCode", 'Surveyor');
        $("#linkPAPLACModal #surveyDate, #linkPAPLACModal #presurveyDate, #linkPAPLACModal #submissionDate").val(getCurrentDate());
        getPaymentPreferencesForSelect("linkPAPLACModal #paymentPreference", '-1');
        $("#linkPAPLACModal #linkPAPLACModalError").hide();
        $("#linkPAPLACModal #linkPAPID").val(papID);
        $("#linkPAPLACModal").modal("show");
    });

    $("#datatablePapList, #datatableLacPapList").on("click", "#editPAP", function () {
        let row = $(this).closest("tr");
        let papID = row.find(".papID").val();
        let personID = row.find(".personID").val();

        $("#addPAPModal #papDetailsError, #addPAPModal #divLinkPAPToLAC, #addPAPModal #saveNewPap").hide();
        $("#addPAPModal #addPAPModalTitle").html("Edit PAP " + papID);
        $("#addPAPModal #updatePap, #addPAPModal #papPicture").show();
        $("#addPAPModal .reset").val('');
        $('#addPAPModal input[name=gender]').prop('checked', false);
        $("#addPAPModal #personID").val(personID);
        $("#addPAPModal #papID").val(papID);
        LoadPAPDetails(papID, "addPAPModal");
        $("#addPAPModal").modal("show");
    });

    $("#datatableLacPapList").on("click", "#surveyPAP", function () {
        let row = $(this).closest("tr");
        let papID = row.find(".papID").val();
        let paplacID = row.find(".paplacID").val();
        let lacID = row.find(".lacID").val();
        let fileID = row.find(".fileID").val();
        let householdID = row.find(".householdID").val();
        //console.log(householdID);

        if (householdID >= 1) {
            $.dialog({
                icon: 'fa fa-spinner fa-spin',
                type: 'blue',
                theme: 'dark',
                title: 'Information',
                content: 'This PAP has been already surveyed! The household number is ' + householdID,
                backgroundDismiss: false,
                backgroundDismissAnimation: 'glow',
            });
        }
        else {
            formData = new FormData();

            $("#surveyPAPModal input[type='text']").val('');
            $("#surveyPAPModal input[type='radio'], #surveyPAPModal input[type='checkbox']").prop('checked', false);
            $("#surveyPAPModal select").val('-1');
            $("#surveyPAPModal #hhRevenue tbody, #surveyPAPModal #hhTable tbody, #surveyPAPModal #hhResidence tbody, #surveyPAPModal #hhRevenue tbody, #surveyPAPModal #hhTFMCompensed tbody, #surveyPAPModal #hhEquipment tbody, #surveyPAPModal #hhAnimals tbody, #surveyPAPModal #hhCultures tbody, #surveyPAPModal #hhCultureSold tbody").children().remove();
            $("#surveyPAPModal #papId").val(papID);
            $("#surveyPAPModal #paplacId").val(paplacID);
            $("#surveyPAPModal #lacId").val(lacID);
            $("#surveyPAPModal #fileID").val(fileID);
            $("#surveyPAPModal #surveyPAPModalTitle").html("Socio-Economic Survey for PAP " + papID);
            $("#surveyPAPModal #surveyDate").val(getCurrentDate());
            getLACForSelect("surveyPAPModal #surveylacID", '-1');
            getEmployeeByRole('#surveyPAPModal #surveyCode','Surveyor');
            $("#surveyPAPModal #cultureSale").hide();
            $("#surveyPAPModal #hhActivity, #surveyPAPModal #hhSkill, #surveyPAPModal #hhCultureWorker, #surveyPAPModal #hhCultureTool").val(null).trigger("change");
            $('#surveyPAPModal input[name="medecineLocation"], #surveyPAPModal input[name="hhMosquitoUsage"]').prop("disabled", true);
            $('#surveyPAPModal #hhPhoto, #surveyPAPModal #hhFile').fileinput('clear');
            getRegionForSelect('surveyPAPModal #surveyRegion', '-1');
            GetVillageForSelect('surveyPAPModal #surveyVillage','-1');
            getListDetails('surveyPAPModal #hhRelation', '-1', 'Relationship');
            getListDetails('surveyPAPModal #hhFrenchLevel', '-1', 'French Level');
            getListDetails('surveyPAPModal #hhVulnerability', '-1', 'Handicap');
            getListDetails('surveyPAPModal #hhSchoolLevel', '-1', 'School Level');
            getListDetails('surveyPAPModal #hhActivity', '-1', 'Activity');
            getListDetails('surveyPAPModal #hhSkill', '-1', 'Skill');
            getListDetails('surveyPAPModal #hhReason', '-1', 'Location Reason');
            getListDetails('surveyPAPModal #hhLatrine', '-1', 'Latrine');
            getListDetails('surveyPAPModal #hhRevenueSource', '-1', 'Revenue Source');
            getGoodTypeForSelect('surveyPAPModal #hhEquipmentName', '-1', 'Equipment');
            getGoodTypeForSelect('surveyPAPModal #hhAnimalName', '-1', 'Animal');
            getListDetails('surveyPAPModal #hhCultureWorker', '-1', 'Worker');
            getCultureToolList('surveyPAPModal #hhCultureTool', '-1');
            getCultureNameForSelect('surveyPAPModal #hhCultureName', '-1');
            getLocations("#surveyPAPModal #hhResidence");
            getListDetails("surveyPAPModal #hhWall", "-1","Wall");
            getListDetails("surveyPAPModal #hhRoof", "-1","Roof");
            getListDetails("surveyPAPModal #hhFloor", "-1", "Floor");
            
            $("#surveyPAPModal").modal("show");
            $("#surveyPAPModal").wizard('begin');
            $("#surveyPAPModal #surveyRegion").focus();
        }
    }); 

    $("#datatableLacPapList").on("click", "#collectLand", function () {
        let row = $(this).closest("tr");
        let papID = row.find(".papID").val();
        let personID = row.find(".personID").val();
        let lacID = row.find(".lacID").val();
        let fileID = row.find(".fileID").val();
        let householdID = row.find(".householdID").val();
        //console.log(householdID);
        formData = new FormData();

        //if (householdID == null || householdID < 1) {
        //    $.alert({
        //        icon: 'fa fa-spinner fa-spin',
        //        type: 'blue',
        //        theme: 'dark',
        //        title: 'Information',
        //        content: 'This PAP has never been surveyed. You can not collect Land Data for a non-surveyed PAP ',
        //        backgroundDismiss: false,
        //        backgroundDismissAnimation: 'glow',
        //    });
        //}
        //else {
            
        //}

        $("#collectLandModal input[type='text']").val('');
        $("#collectLandModal input[type='radio'], #collectLandModal input[type='checkbox']").prop('checked', false);
        $("#collectLandModal select").val('-1');
        $("#collectLandModal #divContent *").prop("disabled", true);

        getLocations("collectLandModal #ownerPrimaryResidence");
        getListDetails("collectLandModal #murCode", "-1", "Wall");
        getListDetails("collectLandModal #toitCode", "-1", "Roof");
        getListDetails("collectLandModal #solCode", "-1", "Floor");
        getTreeMaturityForSelect("collectLandModal #treeMaturity", "-1");
        getFieldTypeForSelect("collectLandModal #fieldType", "-1");
        getStructureUsageForSelect("collectLandModal #structureUsage", "-1");
        getStructureForSelect("collectLandModal #structureCode", "-1");
        getCultureType("#collectLandModal #cultureName");
        getListDetails('collectLandModal #collectVulnerabilityType', '-1', 'Handicap');
        getEmployeeByRole('#collectLandModal #topographCode', 'Topograph');
        getSurfaceUOM("collectLandModal #cultureSurfaceUOM", '-1');
        LoadPAPDetails(papID, "collectLandModal");
        LoadPAPLACProperties2(papID, lacID);

        $("#collectLandModal #collectPapID").val(papID);
        $("#collectLandModal #collectHouseholdID").val(householdID);
        $("#collectLandModal #collectLacID").val(lacID);
        $("#collectLandModal #collectFileID").val(fileID);
        $("#collectLandModal #collectPersonID").val(personID);
        $("#collectLandModal #topoDate").val(getCurrentDate());
        getAssetTypeForSelect("collectLandModal #goodType", "-1");
        $("#collectLandModal #collectLandModalTitle").html("Collect Land Details on LAC" + lacID + " for PAP " + papID);
        $("#collectLandModal #cultureDetails, #collectLandModal #structureDetails, #collectLandModal #treeDetails").hide();
        $("#collectLandModal").modal('show');
        $("#collectLandModal").wizard('begin');
    });
    //#endregion
});

//#region HELPERS METHODS

function getLocationForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetLocationForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Please Select Location</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].LocationId == selectedId) {
                    select += '<option value="' + data[i].LocationId + '" selected>' + data[i].LocationName + '</option>';
                }
                select += '<option value="' + data[i].LocationId + '">' + data[i].LocationName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getRegionForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetRegionForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Please Select Region</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].RegionId == selectedId) {
                    select += '<option value="' + data[i].RegionId + '" selected>' + data[i].RegionName + '</option>';
                }
                select += '<option value="' + data[i].RegionId + '">' + data[i].RegionName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function GetVillageForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetVillageForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Please Select village</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].VillageId == selectedId) {
                    select += '<option value="' + data[i].VillageId + '" selected>' + data[i].VillageName + '</option>';
                }
                select += '<option value="' + data[i].VillageId + '">' + data[i].VillageName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function GetRegionVillageForSelect(id, regionId, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetRegionVillageForSelect",
        data: { RegionId: regionId},
        success: function (data) {
            var select = '<option value="-1">Please Select village</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].VillageId == selectedId) {
                    select += '<option value="' + data[i].VillageId + '" selected>' + data[i].VillageName + '</option>';
                }
                select += '<option value="' + data[i].VillageId + '">' + data[i].VillageName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getVillageStatus(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetVillageStatusForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Please Select Village Status</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].VillageStatus == selectedId) {
                    select += '<option value="' + data[i].VillageStatus + '" selected>' + data[i].VillageStatusString + '</option>';
                }
                select += '<option value="' + data[i].VillageStatus + '">' + data[i].VillageStatusString + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getSurfaceUOM(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetSurfaceUOMForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">UOM...</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].UOMId == selectedId) {
                    select += '<option value="' + data[i].UOMId + '" selected>' + data[i].UOM + '</option>';
                }
                select += '<option value="' + data[i].UOMId + '">' + data[i].UOM + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getZIStatusForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetZIStatus",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Select Status</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].VillageStatus == selectedId) {
                    select += '<option value="' + data[i].VillageStatus + '" selected>' + data[i].VillageStatusString + '</option>';
                }
                select += '<option value="' + data[i].VillageStatus + '">' + data[i].VillageStatusString + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getDepartmentForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetDepartmentForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Select Department</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].DepartmentId == selectedId) {
                    select += '<option value="' + data[i].DepartmentId + '" selected>' + data[i].DepartmentName + '</option>';
                }
                select += '<option value="' + data[i].DepartmentId + '">' + data[i].DepartmentName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getLACForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetLACForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Select LAC</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].LACId == selectedId) {
                    select += '<option value="' + data[i].LACId + '" selected>' + data[i].LAC_ID + '</option>';
                }
                select += '<option value="' + data[i].LACId + '">' + data[i].LAC_ID + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getRestrictedAreaForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetRestrictedAreaForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Select Restricted Area</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].LandId == selectedId) {
                    select += '<option value="' + data[i].LandId + '" selected>' + data[i].LandName + '</option>';
                }
                select += '<option value="' + data[i].LandId + '">' + data[i].LandName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getTreeMaturityForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetTreeMaturityForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Maturity</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].TreeMaturity == selectedId) {
                    select += '<option value="' + data[i].TreeMaturity + '" selected>' + data[i].TreeMaturityName + '</option>';
                }
                select += '<option value="' + data[i].TreeMaturity + '">' + data[i].TreeMaturityName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getFieldTypeForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetFieldTypeForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Type</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].CultureId == selectedId) {
                    select += '<option value="' + data[i].CultureId + '" selected>' + data[i].CultureType + '</option>';
                }
                select += '<option value="' + data[i].CultureId + '">' + data[i].CultureType + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getStructureForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetStructureForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Structure</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].StructureID == selectedId) {
                    select += '<option value="' + data[i].StructureID + '" selected>' + data[i].StructureCode  + '</option>';
                }
                select += '<option value="' + data[i].StructureID + '">' + data[i].StructureCode  + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getMaterialsForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetMaterialsForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Material</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].MaterialsID == selectedId) {
                    select += '<option value="' + data[i].MaterialsID + '" selected>' + data[i].MaterialsCode  + '</option>';
                }
                select += '<option value="' + data[i].MaterialsID + '">' + data[i].MaterialsCode  + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getPaymentPreferencesForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetPaymentPreferencesForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Payment Preference</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].PreferenceId == selectedId) {
                    select += '<option value="' + data[i].PreferenceId + '" selected>' + data[i].PreferenceName + '</option>';
                }
                select += '<option value="' + data[i].PreferenceId + '">' + data[i].PreferenceName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getVulnerabilityForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetVulnerabilityForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Vulnerability</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].VulnerabilityId == selectedId) {
                    select += '<option value="' + data[i].VulnerabilityId + '" selected>' + data[i].VulnerabilityName + '</option>';
                }
                select += '<option value="' + data[i].VulnerabilityId + '">' + data[i].VulnerabilityName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getStructureUsageForSelect(id, selectedId){
    $.ajax({
        type: "GET",
        url: "/Helper/GetStructureUsageForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Usage</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].StructureID == selectedId) {
                    select += '<option value="' + data[i].StructureID + '" selected>' + data[i].StructureUsageCode  + '</option>';
                }
                select += '<option value="' + data[i].StructureID + '">' + data[i].StructureUsageCode  + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getStructureOwnerTypeForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetStructureOwnerTypeForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Ownership</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].StructureID == selectedId) {
                    select += '<option value="' + data[i].StructureID + '" selected>' + data[i].StructureOwnerCode + '</option>';
                }
                select += '<option value="' + data[i].StructureID + '">' + data[i].StructureOwnerCode+ '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getAssetTypeForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetAssetTypeForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Goods Type</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].AssetTypeId == selectedId) {
                    select += '<option value="' + data[i].AssetTypeId + '" selected>' + data[i].AssetName + '</option>';
                }
                select += '<option value="' + data[i].AssetTypeId + '">' + data[i].AssetName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getListDetails(id, selectedId, listName) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetListDetails",
        data: { listName: listName},
        success: function (data) {
            var select = '<option value="-1">' + listName + '</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].ListId == selectedId) {
                    select += '<option value="' + data[i].ListId + '" selected>' + data[i].ListValue + '</option>';
                }
                select += '<option value="' + data[i].ListId + '">' + data[i].ListValue + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getGoodTypeForSelect(id, selectedId, typeName) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetGoodTypeForSelect",
        data: { typeName: typeName },
        success: function (data) {
            var select = '<option value="-1">' + typeName + '</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].ListId == selectedId) {
                    select += '<option value="' + data[i].ListId + '" selected>' + data[i].ListValue + '</option>';
                }
                select += '<option value="' + data[i].ListId + '">' + data[i].ListValue + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getCultureToolList(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetCultureTool",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Culture Tools</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].CultureToolId == selectedId) {
                    select += '<option value="' + data[i].CultureToolId + '" selected>' + data[i].CultureToolType + '</option>';
                }
                select += '<option value="' + data[i].CultureToolId + '">' + data[i].CultureToolType + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getCultureNameForSelect(id, selectedId) {
    $.ajax({
        type: "GET",
        url: "/Helper/GetCultureDiversitiesForSelect",
        data: "{}",
        success: function (data) {
            var select = '<option value="-1">Culture Diversity</option>';
            for (var i = 0; i < data.length; i++) {
                if (data[i].CultureDiversityId == selectedId) {
                    select += '<option value="' + data[i].CultureDiversityId + '" selected>' + data[i].CultureDiversityName + '</option>';
                }
                select += '<option value="' + data[i].CultureDiversityId + '">' + data[i].CultureDiversityName + '</option>';
            }
            $("#" + id).html(select);
        }
    });
}

function getContactPerson(id) {
    $(id).autocomplete({
        source: function (request, response) {
            console.log(request.term);
            $.ajax({
                type: "POST",
                url: "/Helper/GetDepartmentContactPerson/",
                data: { personName: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.FirstName + " " + item.LastName + "(" + item.TFMID + ")", value: item.FirstName + " " + item.LastName + "(" + item.TFMID + ")" };
                    }));
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    });
}

function getLocations(id) {
    $(id).autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "/Helper/GetLocations/",
                data: { locationName: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.LocationName, value: item.LocationName };
                    }));
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    });
}

function getPAP(id) {
    $(id).autocomplete({
        source: function (request, response) {
            //console.log(request.term);
            $.ajax({
                type: "POST",
                url: "/Helper/GetPAP/",
                data: { personName: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        $("#addPAPLACModal #middleName").val(item.MiddleName);
                        $("#addPAPLACModal #lastName").val(item.LastName);
                        return { label: item.FirstName + " " + item.MiddleName + " " + item.LastName + "(" + item.PersonId + ")", value: item.FirstName + " " + item.LastName + "(" + item.PersonId + ")" };
                    }));
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    });
}

function getPAPOwner(id) {
    $(id).autocomplete({
        source: function (request, response) {
            console.log(request.term);
            $.ajax({
                type: "POST",
                url: "/Helper/GetPAP/",
                data: { personName: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        $("#addPAPLACModal #ownerMiddleName").val(item.MiddleName);
                        $("#addPAPLACModal #ownerLastName").val(item.LastName);
                        return { label: item.FirstName + " " + item.MiddleName + " " + item.LastName + "(" + item.PersonId + ")", value: item.FirstName + "(" + item.PersonId + ")" };
                    }));
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    });
}

function getCultureType(id) {
    $(id).autocomplete({
        source: function (request, response) {
            console.log(request.term);
            $.ajax({
                type: "POST",
                url: "/Helper/GetCultureType/",
                data: { cultureName: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.CultureName, value: item.CultureName + "(" + item.CultureId + ")" };
                    }));
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    });
}

function getEmployeeByRole(id, personRole) {
    $(id).autocomplete({
        source: function (request, response) {
            //console.log(request.term);
            $.ajax({
                type: "POST",
                url: "/Helper/GetEmployeeByRole/",
                data: { personName: request.term, personRole: personRole },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.FirstName + " " + item.LastName + " " + item.EmployeeCode, value: item.EmployeeCode };
                    }));
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    });
}

function getAttachementsName(itemId) {
    if (itemId.text().length >0) {
        var fp = $(itemId);
        var lg = fp[0].files.length;
        var items = fp[0].files;
        var fragment = "<ul>";
        var fileNames = [];
        if (lg > 0) {
            for (var i = 0; i < lg; i++) {
                var fileName = items[i].name;
                fragment += "<li>" + fileName + "</li>";
                fileNames.push(fileName);
            }
            fragment += "</ul>"
            //$("#newLandRequestModal #projectUploadFileName").append(fragment);
            return fileNames;
        }
    }
}

function convertToJavaScriptDate(value) {
    if (value != null) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));

        var year = dt.getFullYear();
        var month = (dt.getMonth() + 1);
        var day = dt.getDate();

        if (day < 10) {
            day = "0" + day;
        }
        if (month < 10) {
            month = "0" + month;
        }
        return year + "-" + month + "-" + day;
    } else {
        return "";
    }
}

function convertDateToString(value) {
    if (value != null) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));

        var year = dt.getFullYear();
        var month = (dt.getMonth() + 1);
        var day = dt.getDate();

        if (day < 10) {
            day = "0" + day;
        }
        if (month < 10) {
            month = "0" + month;
        }
        return day + "/" + month + "/" + year;
    } else {
        return "";
    }
}

function getCurrentDate() {
    var _date = new Date();
    var month = _date.getMonth() + 1;
    var day = _date.getDate();
    var newDate = _date.getFullYear() + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;
    //console.log(newDate);

    return newDate;
}

function getCurrentDateDay(days) {
    var _date = new Date();
    _date.setDate(_date.getDate() + days);
    var month = _date.getMonth() + 1;
    var day = _date.getDate();
    var newDate = _date.getFullYear() + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;
    //console.log(newDate);

    return newDate;
}

//Return date between two date in days
function dateDiffDays(currentDate, actionDate) {
    var dt1 = new Date(currentDate);
    var dt2 = new Date(actionDate);
    return Math.floor((Date.UTC(dt2.getFullYear(), dt2.getMonth(), dt2.getDate()) - Date.UTC(dt1.getFullYear(), dt1.getMonth(), dt1.getDate())) / (1000 * 60 * 60 * 24));
}

//#endregion

//#region LAND MANAGEMENT
$("#newLandRequestModal #projectCreateRequest").on("click", function () {
    let projectName = $("#newLandRequestModal #projectName").val();
    let projectDescription = $("#newLandRequestModal #projectDescription").val();
    let projectCostCode = $("#newLandRequestModal #projectCostCode").val();
    let projectDate = $("#newLandRequestModal #projectDate").val();
    let urgent = $("#newLandRequestModal #projectUrgent").prop('checked');
    let contactPerson = $("#newLandRequestModal #projectContactPerson").val();
    let regionId = $("#newLandRequestModal #projectRegion").val();
    let regionName = $("#newLandRequestModal #projectRegion option:selected").text();
    let fileAFE = $("#newLandRequestModal #projectAFE")[0];
    let fileMAP = $("#newLandRequestModal #projectMAP")[0];
    let fileGPS = $("#newLandRequestModal #projectGPS")[0];

    let currentDate = new Date();
    
    var errors = [];

    if (projectName == "" || projectName == null)
        errors.push("Project Name can not be empty");

    if (projectDescription == "" || projectDescription == null)
        errors.push("Project Description can not be empty");

    if (contactPerson == "" || contactPerson == null)
        errors.push("Please specify the Contact Person");

    if (projectCostCode == "" || projectCostCode == null)
        errors.push("Define a correct Cost Code for this project");

    if (!Date.parse(projectDate))
        errors.push("You did not provide a valid start Date");

    var projectDate2 = new Date(projectDate);
    if (currentDate > projectDate2)
        errors.push("Project Date can not be less than today");

    var diffDays = dateDiffDays(currentDate, projectDate2);
    if (diffDays < 7) 
        errors.push("You can not submit a request less than 7 days prior the project schedule date!");
    

    //if (fileMAP == null || fileMAP == "")
    //    errors.push("Please upload the project file for this project");

    //if (fileMAP.files.length === 0)
    //    errors.push("You need to upload either a MAP file or a GPS file");

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#newLandRequestModal #projectRequestError").html(errorMessage).show(250);
    }
    else {
        var formData = new FormData();

        //var fileNameAFE;
        //if (fileAFE.files.length > 0) {
        //    fileNameAFE = fileAFE.files[0].name;
        //    formData.append(fileNameAFE, fileAFE.files[0]);
        //}

        var fileNameMAP;
        if (fileMAP.files.length > 0) {
            fileNameMAP = fileMAP.files[0].name;
            formData.append(fileNameMAP, fileMAP.files[0]);
        }
        //alert(fileNameMAP);

        var fileNameGPS;
        if (fileGPS.files.length > 0) {
            fileNameGPS = "GPSFile";
            formData.append(fileNameGPS, fileGPS.files[0]);
        }

        formData.append('ProjectName', projectName);
        formData.append('ProjectCostCode', projectCostCode);
        formData.append('WorkDescription', projectDescription);
        formData.append('AccessScheduledDate', projectDate);
        formData.append('IsUrgent', urgent);
        formData.append('ContactPerson', contactPerson);
        formData.append('RegionId', regionId);
        formData.append('RegionName', regionName);

        console.log(formData);
        //return false;

        $.ajax({
            type: "POST",
            url: "/LandManagement/CreateLandRequest",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                console.log(response);
                if (response[0] == "Error") {
                    var errorMessage = "<ul>";

                    $.each(response.slice(1), function (index, value) {
                        errorMessage += "<li>" + value + "</li>";
                    });
                    errorMessage += "</ul>";
                    $("#newLandRequestModal #projectRequestError").html(errorMessage).show(250);

                } else if (response[0] == "Exception") {
                    $.notify(response[1], "error");
                }
                else {
                    $.notify(response[1], "success");
                    $("#newLandRequestModal").modal("hide");
                    LoadLandRequest();
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
});

$("#datatable-table").on("click", "#editRequest", function () {
    let row = $(this).closest("tr");
    let LacRequestId = row.find(".lacRequestId").val();
    $("#newLandRequestModal .modal-title").html("Edit Land Request");
    $("#newLandRequestModal #projectUpdateRequest").show();
    $("#newLandRequestModal :input").val('');
    $("#newLandRequestModal :input").prop("disabled", false);
    $("#newLandRequestModal #projectUploadFileName").empty();
    $("#newLandRequestModal #projectRequestError, #newLandRequestModal #projectCreateRequest").hide();
    $("#newLandRequestModal #divProjectAFE").show();
    $("#newLandRequestModal #projectUpdateRequest").prop("disabled", false);

    $("#newLandRequestModal #projectDate").val(getCurrentDate());
    LoadLandRequestDetails(LacRequestId);
    $("#newLandRequestModal").modal("show");
});

$("#datatable-table").on("click", "#viewRequest", function () {
    let row = $(this).closest("tr");
    let LacRequestId = row.find(".lacRequestId").val();
    $("#newLandRequestModal .modal-title").html("View Land Request");
    $("#newLandRequestModal #projectRequestError, #newLandRequestModal #projectCreateRequest, #newLandRequestModal #projectUpdateRequest").hide();
    $("#newLandRequestModal :input").val('');
    $("#newLandRequestModal #projectUploadFileName").empty();
    $("#newLandRequestModal :input").prop("disabled", true);
    $("#newLandRequestModal .close-modal-btn").prop("disabled", false);
    $("#newLandRequestModal").modal("show");
    LoadLandRequestDetails(LacRequestId);
});

$("#datatable-approval").on("click", "#viewRequest", function () {
    let row = $(this).closest("tr");
    let LacRequestId = row.find(".approvalLacRequestId").val();
    console.log(LacRequestId);
    $("#newLandRequestModal .modal-title").html("View Land Request");
    $("#newLandRequestModal #projectRequestError, #newLandRequestModal #projectCreateRequest, #newLandRequestModal #projectUpdateRequest").hide();
    $("#newLandRequestModal :input").val('');
    $("#newLandRequestModal #projectUploadFileName").empty();
    $("#newLandRequestModal :input").prop("disabled", true);
    $("#newLandRequestModal .close-modal-btn").prop("disabled", false);
    $("#newLandRequestModal").modal("show");
    LoadLandRequestDetails(LacRequestId);
});

$("#datatable-table").on("click","#cancelRequest", function () {
    let row = $(this).closest("tr");
    let LacRequestId = row.find(".lacRequestId").val();

    $.confirm({
        title: 'Confirm!',
        content: 'Do you really want to cancel this request? This is irreversible',
        buttons: {
            confirm: {
                btnClass:'btn-success',
                action: function () {
                    $.ajax({
                        type: "POST",
                        url: "/Land/CancelRequest/",
                        data: { LacRequestId: LacRequestId },
                        success: function (response) {
                            if (response[0] == "Success") {
                                $.notify(response[1], "success");
                                LoadLandRequest();
                            } else if (response[0] == "Error") {
                                $.notify(response[1], "error");
                            } else {
                                $.notify(response[1], "error");
                            }
                            console.log(response);
                        }
                    });
                }
            },
            cancel: {
                btnClass:'btn-danger',
                action: function () {
                    $.notify("Request was not cancelled!", "info");
                }
            }
        }
    });
    return false;
});

$("#datatable-approval").on("click", "#approveRequest", function () {
    let row = $(this).closest("tr");
    let LacRequestId = row.find(".approvalLacRequestId").val();
    let RequestId = row.find(".requestId").val();

    $.ajax({
        type: "POST",
        url: "/Land/ApproveRejectRequest/",
        data: { type: "Approve", RequestID: RequestId, Comment:"" },
        success: function (response) {
            if (response[0] == "Success") {
                $.notify(response[1], "success");
                LoadMyApproval();
            } else if (response[0] == "Error") {
                $.notify(response[1], "error");
            } else {
                $.notify(response[1], "error");
            }
            console.log(response);
        }
    });
    return false;
});

$("#datatable-approval").on("click", "#rejectRequest", function () {
    let row = $(this).closest("tr");
    let RequestId = row.find(".requestId").val();

    $("#putRejectCommentModal #putRejectCommentModalErrors").hide();

    $("#putRejectCommentModal  #approvalId").val(RequestId);
    $("#putRejectCommentModal #rejectComment").val("");
    $('#putRejectCommentModal').modal("show");
    return false;
});

$("#newLandRequestModal #projectUpdateRequest").on("click", function () {
    let projectId = $("#newLandRequestModal #LandRequestId").val();
    let projectName = $("#newLandRequestModal #projectName").val();
    let projectDescription = $("#newLandRequestModal #projectDescription").val();
    let projectCostCode = $("#newLandRequestModal #projectCostCode").val();
    let projectDate = $("#newLandRequestModal #projectDate").val();
    let urgent = $("#newLandRequestModal #projectUrgent").prop('checked');
    let contactPerson = $("#newLandRequestModal #projectContactPerson").val();
    let regionId = $("#newLandRequestModal #projectRegion").val();
    let regionName = $("#newLandRequestModal #projectRegion option:selected").text();
    let fileAFE = $("#newLandRequestModal #projectAFE")[0];
    let fileMAP = $("#newLandRequestModal #projectMAP")[0];
    let fileGPS = $("#newLandRequestModal #projectGPS")[0];

    let attachment = getAttachementsName($("#newLandRequestModal #projectAttachment"));

    let currentDate = new Date();
    console.log("Date before " + projectDate);

    var errors = [];

    if (projectId == null || projectId == null || projectId == -1)
        errors.push("Request ID is empty. Please refresh the page and then try again");

    if (projectName == "" || projectName == null)
        errors.push("Project Name can not be empty");

    if (projectDescription == "" || projectDescription == null)
        errors.push("Project Description can not be empty");

    if (contactPerson == "" || contactPerson == null)
        errors.push("Please specify the Contact Person");

    //if (attachment == "" || attachment == null)
    //    errors.push("Please attach all documents - AFE, MAP, GPS");

    //if (location == "" || location == null || location == -1)
    //    errors.push("Define a correct location for this project");

    if (!Date.parse(projectDate))
        errors.push("You did not provide a valid start Date");

    var projectDate2 = new Date(projectDate);
    if (currentDate > projectDate2)
        errors.push("Project Date can not be less than today");

    if (fileAFE == null || fileAFE == "")
    errors.push("Please upload AFE for this project");

    if (fileMAP.files.length === 0 && fileGPS.files.length === 0)
        errors.push("You need to upload either a MAP file or a GPS file");


    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#newLandRequestModal #projectRequestError").html(errorMessage).show(250);
    }
    else {
        var formData = new FormData();

        var fileNameAFE;
        if (fileAFE.files.length > 0) {
            fileNameAFE = fileAFE.files[0].name;
            formData.append(fileNameAFE, fileAFE.files[0]);
        }

        var fileNameMAP;
        if (fileMAP.files.length > 0) {
            fileNameMAP = fileMAP.files[0].name;
            formData.append(fileNameMAP, fileMAP.files[0]);
        }

        var fileNameGPS;
        if (fileGPS.files.length > 0) {
            fileNameGPS = "GPSFile";
            formData.append(fileNameGPS, fileGPS.files[0]);
        }

        formData.append('LacRequestID', projectId);
        formData.append('ProjectName', projectName);
        formData.append('ProjectCostCode', projectCostCode);
        formData.append('WorkDescription', projectDescription);
        formData.append('AccessScheduledDate', projectDate);
        formData.append('IsUrgent', urgent);
        formData.append('ContactPerson', contactPerson);
        formData.append('RegionId', regionId);
        formData.append('RegionName', regionName);

        console.log(formData);

        $.ajax({
            type: "POST",
            url: "/LandManagement/UpdateLandRequest",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                console.log(response);
                if (response[0] == "Error") {
                    var errorMessage = "<ul>";

                    $.each(response.slice(1), function (index, value) {
                        errorMessage += "<li>" + value + "</li>";
                    });
                    errorMessage += "</ul>";
                    $("#newLandRequestModal #projectRequestError").html(errorMessage).show(250);

                } else if (response[0] == "Exception") {
                    $.notify(response[1], "error");
                }
                else {
                    $.notify(response[1], "success");
                    $("#newLandRequestModal").modal("hide");
                    LoadLandRequest();
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
});

$("#putRejectCommentModal").on("click", "#rejectApproval", function () {
    var comment = $("#putRejectCommentModal #rejectComment").val();
    var approvalId = $("#putRejectCommentModal #approvalId").val();

    if (comment == "") {
        $("#putRejectCommentModal #putRejectCommentModalErrors").html("The the rejection reason is mandatory").show(200);
    }
    else {
        console.log(comment);
        console.log(approvalId);
        
        $.ajax({
            type: "POST",
            url: "/Land/ApproveRejectRequest/",
            data: { type: "Reject", RequestID: approvalId, Comment: comment },
            success: function (result) {
                console.log(result);
                if (result[0] == "success") {
                    LoadMyApproval();
                    $('#putRejectCommentModal').modal("hide");
                } else if (result[0] == "error") {
                    $.notify(result[1], "error");
                    $('#putRejectCommentModal').modal("hide");
                }
                else {
                    console.log(result);
                    $('#putRejectCommentModal').modal("hide");
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
});

$("#putRejectCommentModal").on("focus", "#rejectComment", function () {
    $("#putRejectCommentModal #putRejectCommentModalErrors").hide(300);
});

$("#datatable-landApproval").on("click", "#rejectRequest", function () {
    let row = $(this).closest("tr");
    let RequestId = row.find(".requestId").val();
    RejectLandRequest(RequestId);
});

$("#datatable-landApproval").on("click", "#approveRequest", function () {
    let row = $(this).closest("tr");
    let RequestId = row.find(".requestId").val();
    $("#actionModal #actionModalErrors, #actionModal #actionForward").hide();
    $("#actionModal .modal-title").html("Authorize Land Request");
    $("#actionModal #actionGreenLight").show();
    $("#actionModal #requestId").val(RequestId);
    $("#actionModal").modal("show");
    return false;
});

$("#datatable-landApproval").on("click", "#sendTopograph", function () {
    let row = $(this).closest("tr");
    let RequestId = row.find(".requestId").val();

    $("#actionModal #actionModalErrors, #actionModal #actionGreenLight").hide();
    $("#actionModal .modal-title").html("Send for Impact and Mitigation");
    $("#actionModal #actionForward").show();
    $("#actionModal #requestId").val(RequestId);
    $("#actionModal").modal("show");
    return false;
});

$("#datatable-landApproval").on("click", "#createLAC", function () {
    let row = $(this).closest("tr");
    let RequestId = row.find(".requestId").val();

    getSurfaceUOM("createLACModal #AuthorizedAreaSizeUOM", '-1');
    getSurfaceUOM("createLACModal #AreaRequestedUOM", '-1');
    $("#createLACModal #createLACModalErrors").hide();
    $("#createLACModal #LandRequestId").val(RequestId);
    $("#createLACModal #AuthorizedDate").val(getCurrentDate());
    $("#createLACModal").modal("show");
});

$("#datatable-landApproval").on("click", "#makeRestricted", function () {
    let row = $(this).closest("tr");
    let RequestId = row.find(".requestId").val();

    getSurfaceUOM("makeRestrictedModal #AreaRequestedUOM", '-1');
    getZIStatusForSelect("makeRestrictedModal #zoneStatus", '-1');
    $("#makeRestrictedModal #makeRestrictedModalErrors").hide();
    $("#makeRestrictedModal #AuthorizedDate").val(getCurrentDate());
    $("#makeRestrictedModal #LandRequestId").val(RequestId);
    $("#makeRestrictedModal").modal("show");
    return false;
});

$("#datatable-landMitigation").on("click", "#returnLand", function () {
    let row = $(this).closest("tr");
    let RequestId = row.find(".requestId").val();
    returnRequestLO(RequestId);
});

//#region RESTRICTED ZONE
$("#addRA").on("click", function () {
    getZIStatusForSelect("addRestrictedAreaModal #landStatus", '-1');
    getSurfaceUOM("addRestrictedAreaModal #raSurfaceUOM", '-1');
    getLocationForSelect("addRestrictedAreaModal #raLocation", '-1');
    getDepartmentForSelect("addRestrictedAreaModal #raDepartment", '-1');
    $("#addRestrictedAreaModal #addRestrictedAreaModalErrors").hide();
    $("#addRestrictedAreaModal #raGpsDate").val(getCurrentDate());
    $("#addRestrictedAreaModal").modal("show");
});

$("#addRestrictedAreaModal :input").on("focus", function () { $("#addRestrictedAreaModal #addRestrictedAreaModalErrors").hide(); });
$("#addRestrictedAreaModal :input").on("change", function () { $("#addRestrictedAreaModal #addRestrictedAreaModalErrors").hide(); });

$("#addRestrictedAreaModal #createRA").on("click", function () {
    let landName = $("#addRestrictedAreaModal #landName").val();
    let landStatusId = $("#addRestrictedAreaModal #landStatus").val();
    let locationId = $("#addRestrictedAreaModal #raLocation").val();
    let surface = $("#addRestrictedAreaModal #raSurface").val();
    let surfaceUOM = $("#addRestrictedAreaModal #raSurfaceUOM").val();
    let departmentId = $("#addRestrictedAreaModal #raDepartment").val();
    let gpsDate = $("#addRestrictedAreaModal #raGpsDate").val();
    let easting = $("#addRestrictedAreaModal #raEasting").val();
    let northing = $("#addRestrictedAreaModal #raNorthing").val();
    let raComment = $("#addRestrictedAreaModal #raComment").val();

    var errors = [];

    if (landName == "" || landName == null)
        errors.push("Restricted area Name is mandatory");
    if (raComment == "" || raComment == null)
        errors.push("Comment is mandatory");
    if (surfaceUOM == "" || surfaceUOM == null || surfaceUOM == -1)
        errors.push("Surface UOM is mandatory");
    //if (departmentId == "" || departmentId == null || departmentId == -1)
    //    errors.push("Department Requestor is mandatory");
    if (!Date.parse(gpsDate))
        errors.push("You did not provide a valid Date");
    if (surface <= 1 || $.isNumeric(surface) == false)
        errors.push("Please specify valid Surface value");
    if (landStatusId == "" || landStatusId == null || landStatusId == -1)
        errors.push("Restricted Area Status is mandatory");

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addRestrictedAreaModal #addRestrictedAreaModalErrors").html(errorMessage).show(250);
    }
    else {
        var RA = {
            LandName: landName,
            LandStatusId: landStatusId,
            LocationId: locationId,
            AreaSurface: surface,
            AreaSurfaceUOM: surfaceUOM,
            DepartmentId: departmentId,
            GPSDate: gpsDate,
            LandEasting: easting,
            LandNorthing: northing,
            LandComment: raComment
        }

        console.log(RA);

        $.ajax({
            type: "POST",
            url: "/Land/AddNewRestrictedArea",
            data: JSON.stringify(RA),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response[0] == "Error") {
                    $.notify(response[1], "error");
                } else if (response[0] == "Exception") {
                    $.notify(response[1], "error");
                    console.log(response[2]);
                }
                else {
                    let url = $(location).attr('pathname');
                    if (url != "/LandManagement/RestrictedZones") {
                        window.location.href = "/LandManagement/RestrictedZones";
                    }
                    LoadRestricedZone();
                    $.notify(response[1], "success");
                    $("#addRestrictedAreaModal").modal("hide");
                }
            },
            error: function (error) {
                console.log(error);
                $.notify("Something went wrong while updating the information", "error");
            }
        });
    }
});

$("#datatable-restrictedZone").on("click", "#editZone", function () {
    let row = $(this).closest("tr");
    let landId = row.find(".LandId").val();

    $.confirm({
        title: 'Confirm!',
        content: 'Do you really want to change the current status?',
        buttons: {
            confirm: {
                btnClass: 'btn-success',
                action: function () {
                    $.ajax({
                        type: "POST",
                        url: "/Land/ChangeRAStatus/",
                        data: { LandId: landId },
                        success: function (response) {
                            if (response[0] == "Success") {
                                $.notify(response[1], "success");
                                LoadRestricedZone();
                            } else if (response[0] == "Error") {
                                $.notify(response[1], "error");
                            } else {
                                $.notify(response[1], "error");
                            }
                            console.log(response);
                        }
                    });
                }
            },
            cancel: {
                btnClass: 'btn-danger',
                action: function () {
                    $.notify("Status not changed", "info");
                }
            }
        }
    });
});

$("#datatable-restrictedZone").on("click", "#impactedLAC", function () {
    let row = $(this).closest("tr");
    let landId = row.find(".LandId").val();

    $("#addImpactedLACModal #LandID").val(landId);
    $("#addImpactedLACModal #addImpactedLACModalErrors").hide();
    getLACForSelect("addImpactedLACModal #LacID", '-1');
    console.log("Land ID " + landId);
    $("#addImpactedLACModal").modal("show");
});

$("#addImpactedLACModal").on("click", "#addLAC", function () {
    let landId = $("#addImpactedLACModal #LandID").val();
    let lacId = $("#addImpactedLACModal #LacID").val();

    console.log(landId);
    console.log(lacId);

    var errors = [];
    if (landId == "" || landId == null || landId == -1)
        errors.push("Land ID is mandatory. Please reflesh the page and try again!");
    if (lacId == "" || lacId == null || lacId == -1)
        errors.push("Please choose a LAC. This field is mandatory");

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addImpactedLACModal #addImpactedLACModalErrors").html(errorMessage).show(250);
    }
    else {
        var Lac = {
            LACId: lacId,
            LandID: landId
        }
        console.log(Lac);

        $.ajax({
            type: "POST",
            url: "/Land/AddImpactedLAC",
            data: JSON.stringify(Lac),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response[0] == "Error") {
                    $("#addImpactedLACModal #addImpactedLACModalErrors").html(response[1]).show(250);
                } else if (response[0] == "Exception") {
                    $("#addImpactedLACModal #addImpactedLACModalErrors").html(response[1]).show(250);
                    console.log(response[2]);
                }
                else {
                    let url = $(location).attr('pathname');
                    if (url.match("^/Land/LoadLacDetails/"))///Land/LoadLacDetails/2
                        LoadIncludingRestrictedArea(lacId);
                    else if (url.match("^/Land/RestrictedZoneDetails/"))///Land/RestrictedZoneDetails/6
                        loadImpactedLACTable(landId);
                    $.notify(response[1], "success");
                    $("#addImpactedLACModal").modal("hide");
                }
            },
            error: function (error) {
                console.log(error);
                $.notify("Something went wrong while updating the information", "error");
            }
        });

    }
});

$("#tableLAC").on("click", "#deleteLAC", function () {
    let row = $(this).closest("tr");
    let lacId = row.find(".lacId").val();
    let landId = row.find(".landId").val();
    let landLacId = row.find(".landLacId").val();

    console.log("Lac ID " + lacId + " Land Id " + landId + " Land Lac Id " + landLacId);
    unlinkLacRestrictedArea(lacId, landId, landLacId);
});

$("#tableLacRestrictedArea").on("click", "#unlinkRestricted", function () {
    let row = $(this).closest("tr");
    let lacId = row.find(".lacId").val();
    let landId = row.find(".landId").val();
    let landLacId = row.find(".landLacId").val();

    console.log("Lac ID " + lacId + " Land Id " + landId + " Land Lac Id " + landLacId);
    unlinkLacRestrictedArea(lacId, landId, landLacId);
});

//#endregion

//#region ACTION MODAL
$("#actionModal #actionGreenLight").on("click", function () {
    let url = $(location).attr('pathname');
    let fileGPS = $("#actionModal #projectGPS")[0];
    var comments = $("#actionModal #actionComment").val();
    let RequestId = $("#actionModal #requestId").val();

    if (!comments) {
        $.alert('Provide a valid comment');
        return false;
    }
    else {
        var formData = new FormData();

        var fileNameGPS;
        if (fileGPS.files.length > 0) {
            fileNameGPS = "GPSFile";
            formData.append(fileNameGPS, fileGPS.files[0]);
        }

        formData.append('Comments', comments);
        formData.append('RequestID', RequestId);

        $.ajax({
            type: "POST",
            url: "/Land/GrantGreenLight/",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response[0] == "Success") {
                    if (url != "/LandManagement/LandStatus") {
                        window.location.href = "/LandManagement/LandStatus";
                    }
                    LoadApprovedLandRequest();
                    $("#actionModal").modal("hide");
                    $.notify(response[1], "success");
                } else if (response[0] == "Error") {
                    $.notify(response[1], "error");
                } else {
                    $.notify(response[1], "error");
                }
                console.log(response);
            }
        });
    }
});

$("#actionModal #actionForward").on("click", function () {
    let url = $(location).attr('pathname');
    let fileGPS = $("#actionModal #projectGPS")[0];
    var comments = $("#actionModal #actionComment").val();
    let RequestId = $("#actionModal #requestId").val();

    if (!comments) {
        $.alert('Provide a valid comment');
        return false;
    }
    else {
        var formData = new FormData();

        var fileNameGPS;
        if (fileGPS.files.length > 0) {
            fileNameGPS = "GPSFile";
            formData.append(fileNameGPS, fileGPS.files[0]);
        }

        formData.append('Comments', comments);
        formData.append('RequestID', RequestId);

        $.ajax({
            type: "POST",
            url: "/Land/ForwardImpactMitigation/",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response[0] == "Success") {
                    if (url != "/LandManagement/LandStatus") {
                        window.location.href = "/LandManagement/LandStatus";
                    }
                    $("#actionModal").modal("hide");
                    LoadApprovedLandRequest();
                    $.notify(response[1], "success");
                } else if (response[0] == "Error") {
                    $.notify(response[1], "error");
                } else {
                    $.notify(response[1], "error");
                }
                console.log(response);
            }
        });
    }
});
//#endregion

//#region CREATE LAC MODAL
$("#createLACModal :input").on("focus", function () { $("#createLACModal #createLACModalErrors").hide(); });
$("#createLACModal :input").on("change", function () { $("#createLACModal #createLACModalErrors").hide(); });

$("#createLACModal #createLAC").on("click", function () {
    let areaRequested = $("#createLACModal #AreaRequested").val();
    let areaRequestedUOM = $("#createLACModal #AreaRequestedUOM").val();
    let estimatedCost = $("#createLACModal #CostEstimate").val();
    let lacComment = $("#createLACModal #lacComment").val();
    let LacRequestId = $("#createLACModal #LandRequestId").val();
    let lacName = $("#createLACModal #lacName").val();

    var errors = [];

    if (LacRequestId == "" || LacRequestId == null)
        errors.push("Please specify valid LAC ID. Please refresh the page and try again");

    if (lacName == "" || lacName == null)
        errors.push("Please specify LAC Name");

    //if (estimatedCost == "" || estimatedCost == null)
    //    errors.push("Please specify the Estimated Cost");

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#createLACModal #createLACModalErrors").html(errorMessage).show(250);
    }
    else {
        let LacView = {
            LACRequestId: LacRequestId,
            LACName: lacName,
            AreaRequested:areaRequested,
            AreaRequestedUOM:areaRequestedUOM,
            CostEstimate:estimatedCost,
            Comment:lacComment
        }
        console.log(LacView);
        //return false;

        $.ajax({
            type: "POST",
            url: "/Land/CreateLAC",
            data: JSON.stringify(LacView),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response[0] == "Error") {
                    $.notify(response[1], "error");
                } else if (response[0] == "Exception") {
                    $.notify(response[1], "error");
                    console.log(response[2]);
                }
                else {
                    let url = $(location).attr('pathname');
                    if (url != "/LandManagement/LandStatus") {
                        window.location.href = "/LandManagement/LandStatus";
                    }
                    $.notify(response[1], "success");
                    LoadApprovedLandRequest();
                    $("#createLACModal").modal("hide");
                }
            },
            error: function (error) {
                console.log(error);
                $.notify("Something went wrong while updating the information", "error");
            }
        });
    }
});
//#endregion

//#region MAKE RESTRICTED
$("#makeRestrictedModal :input").on("focus", function () { $("#makeRestrictedModal #makeRestrictedModalErrors").hide(); });
$("#makeRestrictedModal :input").on("change", function () { $("#makeRestrictedModal #makeRestrictedModalErrors").hide(); });

$("#makeRestrictedModal #makeRestrictedModal").on("click", function () {
    let AuthorizedDate = $("#makeRestrictedModal #AuthorizedDate").val();
    let areaRequested = $("#makeRestrictedModal #AreaRequested").val();
    let areaRequestedUOM = $("#makeRestrictedModal #AreaRequestedUOM").val();
    let lacComment = $("#makeRestrictedModal #landComment").val();
    let LacRequestId = $("#makeRestrictedModal #LandRequestId").val();
    let landStatusId = $("#makeRestrictedModal #zoneStatus").val();
    let LandEasting = $("#makeRestrictedModal #LandEasting").val();
    let LandNorthing = $("#makeRestrictedModal #LandNorthing").val();
    
    var errors = [];

    if (lacComment == "" || lacComment == null)
        errors.push("LAC Comment is mandatory");

    if (LacRequestId == "" || LacRequestId == null)
        errors.push("Please specify valid LAC ID. Please refresh the page and try again");

    if (areaRequestedUOM == "" || areaRequestedUOM == null || areaRequestedUOM == -1)
        errors.push("Area Surface UOM is mandatory");

    if (!Date.parse(AuthorizedDate))
        errors.push("You did not provide a valid Date");

    if (areaRequested == "" || areaRequested == null)
        errors.push("Please specify Area Surface");

    if (landStatusId == "" || landStatusId == null || landStatusId == -1)
        errors.push("You did not select the Status of this Area");

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#makeRestrictedModal #makeRestrictedModalErrors").html(errorMessage).show(250);
    }
    else {
        let LacView = {
            LACRequestId: LacRequestId,
            AuthorizedDate: AuthorizedDate,
            AreaSurface: areaRequested,
            AreaSurfaceUOM: areaRequestedUOM,
            LandComment: lacComment,
            LandEasting: LandEasting,
            LandNorthing: LandNorthing,
            LandStatusId: landStatusId
        }
        console.log(LacView);

        $.ajax({
            type: "POST",
            url: "/Land/MakeRestricted",
            data: JSON.stringify(LacView),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response[0] == "Error") {
                    $.notify(response[1], "error");
                } else if (response[0] == "Exception") {
                    $.notify(response[1], "error");
                    console.log(response[2]);
                }
                else {
                    let url = $(location).attr('pathname');
                    if (url != "/LandManagement/LandStatus") {
                        window.location.href = "/LandManagement/LandStatus";
                    }
                    $.notify(response[1], "success");
                    LoadApprovedLandRequest();
                    $("#makeRestrictedModal").modal("hide");
                }
            },
            error: function (error) {
                console.log(error);
                $.notify("Something went wrong while updating the information", "error");
            }
        });
    }
});
//#endregion

//#region ACTION FUNCTIONS

function ForwardToTopograph(RequestId) {
    let url = $(location).attr('pathname');

    $.confirm({
        type: 'blue',
        title: 'Forward to Topograph',
        content: '' +
            '<form action="" class="formComments">' +
            '<div class="form-group">' +
            '<textarea rows="3" class="comments autogrow form-control transition-height" placeholder="Add a comment here" style="overflow: hidden; overflow-wrap: break-word;resize: none;"></textarea>' +
            '</div>' +
            '</form>',
        buttons: {
            formSubmit: {
                text: 'Forward',
                btnClass: 'btn-blue',
                action: function () {
                    var comments = this.$content.find('.comments').val();
                    if (!comments) {
                        $.alert('Provide a valid comment');
                        return false;
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Land/ForwardImpactMitigation/",
                        data: { RequestID: RequestId, Comment: comments },
                        success: function (response) {
                            if (response[0] == "Success") {
                                if (url != "/LandManagement/LandStatus") {
                                    window.location.href = "/LandManagement/LandStatus";
                                }
                                LoadApprovedLandRequest();
                                $.notify(response[1], "success");
                            } else if (response[0] == "Error") {
                                $.notify(response[1], "error");
                            } else {
                                $.notify(response[1], "error");
                            }
                            console.log(response);
                        }
                    });
                }
            },
            cancel: function () {
                //close
            },
        }
    });
}

function CreateLAC(RequestId) {
    getSurfaceUOM("createLACModal #AuthorizedAreaSizeUOM", '-1');
    getSurfaceUOM("createLACModal #AreaRequestedUOM", '-1');
    $("#createLACModal #createLACModalErrors").hide();
    $("#createLACModal").modal("show");
    console.log(RequestId);
    return false;
}

function ApproveLandRequest(RequestId) {
    let url = $(location).attr('pathname');
    $.confirm({
        type: 'blue',
        title: 'Give Green Light',
        content: '' +
            '<form action="" class="formComments">' +
            '<div class="form-group">' +
            '<label>Enter a comment</label>' +
            '<textarea rows="3" class="comments autogrow form-control transition-height" placeholder="Add a comment here" style="overflow: hidden; overflow-wrap: break-word;resize: none;"></textarea>' +
            '</div>' +
            '</form>',
        buttons: {
            formSubmit: {
                text: 'Approve',
                btnClass: 'btn-blue',
                action: function () {
                    var comments = this.$content.find('.comments').val();
                    if (!comments) {
                        $.alert('Provide a valid comment');
                        return false;
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Land/GrantGreenLight/",
                        data: { RequestID: RequestId, Comment: comments },
                        success: function (response) {
                            if (response[0] == "Success") {
                                if (url != "/LandManagement/LandStatus") {
                                    window.location.href = "/LandManagement/LandStatus";
                                }
                                LoadApprovedLandRequest();
                                $.notify(response[1], "success");
                            } else if (response[0] == "Error") {
                                $.notify(response[1], "error");
                            } else {
                                $.notify(response[1], "error");
                            }
                            console.log(response);
                        }
                    });
                }
            },
            cancel: function () {
                //close
            },
        }
    });
}

function RejectLandRequest(RequestId) {
    let url = $(location).attr('pathname');
    $.confirm({
        type: 'red',
        title: 'Reject Request',
        content: '' +
            '<form action="" class="formComments">' +
            '<div class="form-group">' +
            '<textarea rows="3" class="comments autogrow form-control transition-height" placeholder="Add a comment here" style="overflow: hidden; overflow-wrap: break-word;resize: none;"></textarea>' +
            '</div>' +
            '</form>',
        buttons: {
            formSubmit: {
                text: 'Reject',
                btnClass: 'btn-danger',
                action: function () {
                    var comments = this.$content.find('.comments').val();
                    if (!comments) {
                        $.alert('Provide a valid comment');
                        return false;
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Land/ApproveRejectRequest/",
                        data: { type: "Reject", RequestID: RequestId, Comment: comments },
                        success: function (response) {
                            if (response[0] == "Success") {
                                if (url != "/LandManagement/LandStatus") {
                                    window.location.href = "/LandManagement/LandStatus";
                                }
                                $.notify(response[1], "success");
                                LoadApprovedLandRequest();
                            } else if (response[0] == "Error") {
                                $.notify(response[1], "error");
                            } else {
                                $.notify(response[1], "error");
                            }
                            console.log(response);
                        }
                    });
                }
            },
            cancel: function () {
                //close
            },
        }
    });
}

function returnRequestLO(RequestId) {
    let url = $(location).attr('pathname');
    $.confirm({
        type: 'blue',
        title: 'Return Land Request',
        content: '' +
            '<form action="" class="formComments">' +
            '<div class="form-group">' +
            '<textarea rows="3" class="comments autogrow form-control transition-height" placeholder="Add a comment here" style="overflow: hidden; overflow-wrap: break-word;resize: none;"></textarea>' +
            '</div>' +
            '</form>',
        buttons: {
            formSubmit: {
                text: 'Return',
                btnClass: 'btn-blue',
                action: function () {
                    var comments = this.$content.find('.comments').val();
                    if (!comments) {
                        $.alert('Provide a valid comment');
                        return false;
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Land/ImpactMitigation/",
                        data: { RequestID: RequestId, Comment: comments },
                        success: function (response) {
                            if (response[0] == "Success") {
                                if (url != "/LandManagement/LandImpactMitigation") {
                                    window.location.href = "/LandManagement/LandImpactMitigation";
                                }
                                LoadLandRequestImpactMitigation();
                                $.notify(response[1], "success");
                            } else if (response[0] == "Error") {
                                $.notify(response[1], "error");
                            } else {
                                $.notify(response[1], "error");
                            }
                            console.log(response);
                        }
                    });
                }
            },
            cancel: function () {
                //close
            },
        }
    });
}

function unlinkLacRestrictedArea(lacId, landId, landLacId) {
    var lac = {
        LACId: lacId,
        LandID: landId,
        LandLacID: landLacId
    }

    console.log(lac);

    $.confirm({
        title: 'Confirm!',
        content: 'Do you really want to remove this LAC from the land?',
        buttons: {
            confirm: {
                btnClass: 'btn-success',
                action: function () {
                    $.ajax({
                        type: "POST",
                        url: "/Land/RemoveImpactedLAC/",
                        data: JSON.stringify(lac),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response[0] == "Error") {
                                $.notify(response[1], "error");
                            } else if (response[0] == "Exception") {
                                $.notify(response[1], "error");
                                console.log(response[2]);
                            }
                            else {
                                let url = $(location).attr('pathname');
                                if (url.match("^/Land/LoadLacDetails/"))
                                    LoadIncludingRestrictedArea(lacId);
                                else if (url.match("^/Land/RestrictedZoneDetails/"))
                                    loadImpactedLACTable(landId);

                                $.notify(response[1], "success");
                            }
                        },
                        error: function (error) {
                            console.log(error);
                            $.notify("Something went wrong while updating the information", "error");
                        }
                    });
                }
            },
            cancel: {
                btnClass: 'btn-danger',
                action: function () {

                }
            }
        }
    });
}

//#endregion

//#endregion

//#region LOAD FUNCTIONS
function LoadLandRequest() {
    if ($.fn.DataTable.isDataTable('#datatable-table')) {
        $('#datatable-table').DataTable().destroy();
    }
    $.ajax({
        type: "POST",
        url: "/LandManagement/LoadLandRequest",
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td>' +
                    '<a href="#" class="btn btn-dark btn-rounded-f" data-dismiss="modal" id="editRequest" title="Edit Request"><i class="fas fa-edit"></i></a> ' +
                    '<a href="#" class="btn btn-info btn-rounded-f" data-dismiss="modal" id="viewRequest" title="View Request"><i class="fas fa-eye"></i></a> ' +
                    '<a href="#" class="btn btn-danger btn-rounded-f" id="cancelRequest" title="Cancel Request"><i class="fas fa-window-close"></i></a>' +
                    '<input type="hidden" class="lacRequestId" value="' + item.LACRequestId + '"/>' +
                    '<input type="hidden" class="requestId" value="' + item.RequestId + '"/>' +
                    '</td>' +
                    '<td>' + item.ProjectName + '</td>' +
                    '<td>' + item.ProjectCostCode + '</td>' +
                    '<td>' + item.RegionName + '</td>' +
                    '<td>' + item.AccessDateString + '</td>' +
                    '<td>' + item.ContactPerson + '</td>' +
                    '<td>' + convertDateToString(item.RequestedDate) + '</td>' +
                    '<td>' + item.RequestStatus + '</td>' +
                    '</tr>';
            });
            $("#datatable-table tbody").html(tr);
            $('#datatable-table').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'My Requests'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadMyApproval() {
    if ($.fn.DataTable.isDataTable('#datatable-approval')) {
        $('#datatable-approval').DataTable().destroy();
    }
    $.ajax({
        type: "POST",
        url: "/LandManagement/LoadMyApproval",
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td>' +
                    '<a href="#" class="btn btn-dark btn-rounded-f" data-dismiss="modal" id="approveRequest" title="Approve Request"><i class="fas fa-check"></i></a> ' +
                    '<a href="#" class="btn btn-danger btn-rounded-f" id="rejectRequest" title="Reject Request"><i class="fas fa-window-close"></i></a> ' +
                    '<a href="#" class="btn btn-info btn-rounded-f" data-dismiss="modal" id="viewRequest" title="View Request"><i class="fas fa-eye"></i></a>' +
                    '<input type="hidden" class="approvalLacRequestId" value="' + item.LACRequestId + '"/>' +
                    '<input type="hidden" class="requestId" value="' + item.RequestId + '"/>' +
                    '</td>' +
                    '<td>' + item.ProjectName + '</td>' +
                    '<td>' + item.ProjectCostCode + '</td>' +
                    '<td>' + item.RegionName + '</td>' +
                    '<td>' + item.AccessDateString + '</td>' +
                    '<td>' + item.ContactPerson + '</td>' +
                    '<td>' + convertDateToString(item.RequestedDate) + '</td>' +
                    '<td>' + item.RequestorName + '</td>' +
                    '</tr>';
            });
            $("#datatable-approval tbody").html(tr);
            $('#datatable-approval').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'My Approval'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadApprovedLandRequest() {
    if ($.fn.DataTable.isDataTable('#datatable-landApproval')) {
        $('#datatable-landApproval').DataTable().destroy();
    }
    $.ajax({
        type: "POST",
        url: "/LandManagement/LoadApprovedLandRequest",
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td>' +
                    '<a href="#" class="btn btn-success btn-rounded-f" data-dismiss="modal" id="approveRequest" title="Green Light"><i class="fas fa-lightbulb"></i></a> ' +
                    '<a href="#" class="btn btn-danger btn-rounded-f" id="rejectRequest" title="Reject Request"><i class="fas fa-window-close"></i></a> ' +
                    '<a href="#" class="btn btn-info btn-rounded-f" data-dismiss="modal" id="sendTopograph" title="Send for Impact and Mitigation"><i class="fas fa-step-forward"></i></a> ' +
                    '<a href="#" class="btn btn-gray btn-rounded-f" data-dismiss="modal" id="makeRestricted" title="Make Restricted"><i class="fas fa-lock"></i></a>' +
                    '<input type="hidden" class="approvalLacRequestId" value="' + item.LACRequestId + '"/>' +
                    '<input type="hidden" class="requestId" value="' + item.RequestId + '"/>' +
                    '</td>' +
                    '<td> <a href="/LandManagement/RequestDetails/' + item.LACRequestId + '">' + item.ProjectName + '</a></td>' +
                    '<td>' + item.RegionName + '</td>' +
                    '<td>' + convertDateToString(item.RequestedDate) + '</td>' +
                    '<td>' + item.RequestorName + '</td>' +
                    '<td>' + item.ContactPerson + '</td>' +
                    '<td>' + item.RequestorDepartment + '</td>' +
                    '<td>' + item.ProjectCostCode + '</td>' +
                    '<td>' + item.AccessDateString + '</td>' +
                    '</tr>';
            });
            $("#datatable-landApproval tbody").html(tr);
            $('#datatable-landApproval').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'Check Land Status'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadLandRequestImpactMitigation() {
    if ($.fn.DataTable.isDataTable('#datatable-landMitigation')) {
        $('#datatable-landMitigation').DataTable().destroy();
    }
    $.ajax({
        type: "POST",
        url: "/LandManagement/LoadLandForImpactMitigation",
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td>' +
                    '<a href="#" class="btn btn-info btn-rounded-f" data-dismiss="modal" id="returnLand" title="Return Land"><i class="fas fa-undo"></i></a> ' +
                    '<input type="hidden" class="approvalLacRequestId" value="' + item.LACRequestId + '"/>' +
                    '<input type="hidden" class="requestId" value="' + item.RequestId + '"/>' +
                    '</td>' +
                    '<td> <a href="/LandManagement/RequestDetails/' + item.LACRequestId + '">' + item.ProjectName + '</a></td>' +
                    '<td>' + item.RegionName + '</td>' +
                    '<td>' + item.AccessDateString + '</td>' +
                    '<td>' + item.ContactPerson + '</td>' +
                    '<td>' + convertDateToString(item.RequestedDate) + '</td>' +
                    '<td>' + item.RequestorName + '</td>' +
                    '<td>' + item.RequestorDepartment + '</td>' +
                    '</tr>';
            });
            $("#datatable-landMitigation tbody").html(tr);
            $('#datatable-landMitigation').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'Land for Impact and Mitigation'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadLACList() {
    if ($.fn.DataTable.isDataTable('#datatable-lacList')) {
        $('#datatable-lacList').DataTable().destroy();
    }
    $.ajax({
        type: "POST",
        url: "/Land/LoadLACList",
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td>' +
                    '<a href="#" class="btn btn-dark btn-rounded-f" data-dismiss="modal" id="addPAP" title="Add PAP"><i class="fas fa-user-plus"></i></a> ' +
                    '<a href="#" class="btn btn-info btn-rounded-f" data-dismiss="modal" id="linkLandLac" title="Add to Restricted Area"><i class="fas fa-link"></i></a> ' +
                    '<input type="hidden" class="LacRequestId" value="' + item.LACRequestId + '"/>' +
                    '<input type="hidden" class="lacID" value="' + item.LACId + '"/>' +
                    '<input type="hidden" class="lacName" value="' + item.LAC_ID + '"/>' +
                    '</td>' +
                    '<td> <a href="/Land/LoadLacDetails/' + item.LACId + '">' + item.LAC_ID + '</a></td>' +
                    '<td> <a href="/Land/LoadLacDetails/' + item.LACId + '">' + item.LACName + '</a></td>' +
                    '<td>' + item.LacStatusString + '</td>' +
                    '<td>' + item.AreaDescription + '</td>' +
                    '<td>' + item.RequestDateString + '</td>' +
                    '<td>' + item.Department + '</td>' +
                    '<td>' + item.AreaRequestedSize + '</td>' +
                    '<td>' + item.AreaCompensedString + '</td>' +
                    '<td>' + item.AuthorizedAreaSizeString + '</td>' +
                    '<td>' + item.CostEstimate + '</td>' +
                    '<td>' + item.Realcosts + '</td>' +
                    '<td>' + item.Comment + '</td>' +
                    '</tr>';
            });
            $("#datatable-lacList tbody").html(tr);
            $('#datatable-lacList').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of LAC'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadRestricedZone() {
    if ($.fn.DataTable.isDataTable('#datatable-restrictedZone')) {
        $('#datatable-restrictedZone').DataTable().destroy();
    }
    $.ajax({
        type: "POST",
        url: "/Land/LoadRestrictedArea",
        success: function (response) {
            console.log(response);
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td>' +
                    '<a href="#" class="btn btn-dark btn-rounded-f" data-dismiss="modal" id="editZone" title="Edit Zone"><i class="far fa-edit"></i></a> ' +
                    '<a href="#" class="btn btn-info btn-rounded-f" data-dismiss="modal" id="impactedLAC" title="Associate LAC"><i class="fas fa-paperclip"></i></a> ' +
                    '<input type="hidden" class="LandId" value="' + item.LandId + '"/>' +
                    '</td>' +
                    '<td> <a href="/Land/RestrictedZoneDetails/' + item.LandId + '">' + item.LandName + '</a></td>' +
                    '<td>' + item.AreaSurfaceString + '</td>' +
                    '<td>' + item.RegionName + '</td>' +
                    '<td>' + item.GPSDateString + '</td>' +
                    '<td>' + item.ContactPerson + '</td>' +
                    '<td>' + item.DepartmentName + '</td>' +
                    '<td>' + item.LandStatusString + '</td>' +
                    '</tr>';
            });
            $("#datatable-restrictedZone tbody").html(tr);
            $('#datatable-restrictedZone').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of Restricted Zone'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadLandRequestDetails(landRequestId) {
    $.ajax({
        type: "GET",
        url: "/Land/GetLandRequest/",
        data: { landRequestID: landRequestId },
        success: function (data) {
            console.log(data);
            getRegionForSelect("newLandRequestModal #projectRegion", data.RegionId);
            getContactPerson("#newLandRequestModal #projectContactPerson");

            $("#newLandRequestModal #LandRequestId").val(data.LACRequestId);
            $("#newLandRequestModal #projectName").val(data.ProjectName);
            $("#newLandRequestModal #projectDescription").val(data.WorkDescription);
            $("#newLandRequestModal #projectDate").val(data.AccessDateString);
            $("#newLandRequestModal #projectContactPerson").val(data.ContactPerson);
            $("#newLandRequestModal #fileAttachments").show();
            $("#newLandRequestModal #projectUploadFileName").append(data.AttachmentsList);
            $("#newLandRequestModal #projectCostCode").val(data.ProjectCostCode);

            var att = '<table id="tableAttachment"><tbody>';
            $.each(data.AttachmentsDocuments, function (idx, obj) {
                att += '<tr>' +
                    '<td><a href="#" id="downloadFile"><i class="fas fa-download"></i></a>' +
                    '<input type="hidden" value="' + obj.AttachmentId + '" class="attachmentID" />' +
                    '<input type = "hidden" value = "' + obj.LandId + '" class="landID" /></td>' +
                    '<td>' + obj.RequestAttachementFile + '</td>' +
                    '</tr>';
            });
            att += '</tbody></table>';
            $("#newLandRequestModal #projectUploadFileName").append(att);

            if (data.IsUrgent) {
                $('#newLandRequestModal #projectUrgent').attr("checked", true);
            }
            else {
                $('#newLandRequestModal #projectUrgent').attr("checked", false);
            }
        },
        error: function (error) {
            console.log(error);
            $("#newLandRequestModal").modal("hide");
            $.alert({
                icon: 'fas fa-exclamation-triangle',
                title: "Alert",
                content: "Something went wrong. If this persists, please contact the administrator",
                type: "red"
            });
        }
    });
}

function loadPointTable(lacRequestId) {
    if ($.fn.DataTable.isDataTable('#tablePoint')) {
        $('#tablePoint').DataTable().destroy();
    }
    $.ajax({
        type: "GET",
        url: "/Land/LoadLandPoint",
        data: { LacRequestId: lacRequestId },
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td style="text-align:center;">' +
                    '<a href="#" class="btn btn-danger btn-sm deletePoint"><i class="fas fa-trash-alt"></i></a>' +
                    '<input type="hidden" class="PointId" value="' + item.PointId + '"/>' +
                    '<input type="hidden" class="landId" value="' + item.LandId + '"/>' +
                    '</td>' +
                    '<td class="editValue">' +
                    '<span class="editValueSpan" style="display: block;">' + item.PointName + '</span>' +
                    '<input type="text" class="form-control editInTable" value="' + item.PointName + '"/>' +
                    '</td>' + '<td class="editValue">' +
                    '<span class="editValueSpan" style="display: block;">' + item.Latitude + '</span>' +
                    '<input type="text" class="form-control editInTable" value="' + item.Latitude + '"/>' +
                    '</td>' + '<td class="editValue">' +
                    '<span class="editValueSpan" style="display: block;">' + item.Longitude + '</span>' +
                    '<input type="text" class="form-control editInTable" value="' + item.Longitude + '"/>' +
                    '</td>' + '<td class="editValue">' +
                    '<span class="editValueSpan" style="display: block;">' + item.Elevation + '</span>' +
                    '<input type="text" class="form-control editInTable" value="' + item.Elevation + '"/>' +
                    '</td>'
                '</tr>';
            });

            $("#tablePoint tbody").html(tr);

            $("#tablePoint .editInTable").hide();

            $("#tablePoint").DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of Land Village'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function loadVillageTable(lacRequestId) {
    if ($.fn.DataTable.isDataTable('#tableVillage')) {
        $('#tableVillage').DataTable().destroy();
    }
    $.ajax({
        type: "GET",
        url: "/Land/LoadLandVillage",
        data: { LacRequestId: lacRequestId },
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td style="text-align:center;">' +
                    '<a href="#" class="btn btn-danger btn-sm" id="deleteVillage"><i class="fas fa-trash-alt"></i></a>' +
                    '<input type="hidden" class="villageId" value="' + item.VillageId + '"/>' +
                    '<input type="hidden" class="regionId" value="' + item.RegionId + '"/>' +
                    '<input type="hidden" class="landId" value="' + item.LandId + '"/>' +
                    '</td>' +
                    '<td>' + item.VillageName + '</td>' +
                    '<td>' + item.RegionName + '</td>' +
                    '</tr>';
            });

            $("#tableVillage tbody").html(tr);

            $("#tableVillage").DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of Land Village'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function loadImpactedLACTable(landId) {
    if ($.fn.DataTable.isDataTable('#tableLAC')) {
        $('#tableLAC').DataTable().destroy();
    }
    $.ajax({
        type: "GET",
        url: "/Land/LoadImpactedLAC",
        data: { LandId: landId },
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td style="text-align:center;">' +
                    '<a href="#" class="btn btn-danger btn-sm" id="deleteLAC"><i class="fas fa-trash-alt"></i></a>' +
                    '<input type="hidden" class="lacId" value="' + item.LACId + '"/>' +
                    '<input type="hidden" class="landId" value="' + item.LandID + '"/>' +
                    '<input type="hidden" class="landLacId" value="' + item.LandLacID + '"/>' +
                    '</td>' +
                    '<td> <a href="/Land/LoadLacDetails/' + item.LACId + '">' + item.LACName + '</a></td>' +
                    '<td >' + item.LacStatusString + '</td>' +
                    '<td >' + item.PAPs + '</td>' +
                    '<td >' + item.AuthorizedAreaSizeString + '</td>' +
                    '<td >' + item.RequestDateString + '</td>' +
                    '<td >' + item.Department + '</td>' +
                    '</tr>';
            });

            $("#tableLAC tbody").html(tr);

            $("#tableLAC").DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of Impacted LAC'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadIncludingRestrictedArea(lacID) {
    if ($.fn.DataTable.isDataTable('#tableLacRestrictedArea')) {
        $('#tableLacRestrictedArea').DataTable().destroy();
    }
    $.ajax({
        type: "GET",
        url: "/Land/LoadIncludingRestrictedArea",
        data: { LacID: lacID },
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td style="text-align:center;">' +
                    '<a href="#" class="btn btn-danger btn-sm" id="unlinkRestricted"><i class="fas fa-unlink"></i></a>' +
                    '<input type="hidden" class="lacId" value="' + item.LacID + '"/>' +
                    '<input type="hidden" class="landId" value="' + item.LandId + '"/>' +
                    '<input type="hidden" class="landLacId" value="' + item.LandLacID + '"/>' +
                    '</td>' +
                    '<td> <a href="/Land/RestrictedZoneDetails/' + item.LandId + '">' + item.LandName + '</a></td>' +
                    '<td>' + item.DepartmentName + '</td>' +
                    '<td>' + item.RegionName + '</td>' +
                    '<td>' + item.AreaSurfaceString + '</td>' +
                    '<td>' + item.LandStatusString + '</td>' +
                    '</tr>';
            });

            $("#tableLacRestrictedArea tbody").html(tr);

            $("#tableLacRestrictedArea").DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of Restricted Area '
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadPapLac(lacId) {
    if ($.fn.DataTable.isDataTable('#tablePapLac')) {
        $('#tablePapLac').DataTable().destroy();
    }

    $.ajax({
        type: "GET",
        url: "/Community/LoadPAPLAC",
        data: { LacID: lacId },
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td style="text-align:center;">' +
                    '<a href="#" class="btn btn-danger" id="deletePAPLAC"><i class="fas fa-unlink"></i></a> ' +
                    '<a href="#" class="btn btn-info" data-dismiss="modal" id="surveyPAP" title="Socio-economic Survey"><i class="fas fa-users"></i></a> ' +
                    '<input type="hidden" class="lacId" value="' + item.LACId + '"/>' +
                    '<input type="hidden" class="papId" value="' + item.PAPId + '"/>' +
                    '<input type="hidden" class="papLacId" value="' + item.PAPLACId + '"/>' +
                    '<input type="hidden" class="fileID" value="' + item.FileNumber + '"/>' +
                    '</td>' +
                    '<td> <a href="/Community/PapDetails/' + item.PAPId + '">' + item.PAPId + '</a></td>' +
                    '<td>' + item.FirstName + '</td>' +
                    '<td>' + item.LastName + '</td>' +
                    '<td>' + item.ResidenceName + '</td>' +
                    '<td><img src="' + item.Picture + '" width="50" height="50" style="border-radius:5px;"></td>' +
                    '<td>' + item.FileNumber + '</td>' +
                    '</tr>';
            });

            $("#tablePapLac tbody").html(tr);

            $("#tablePapLac").DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of PAP '
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadPapLacProperties(papID) {
    if ($.fn.DataTable.isDataTable('#tablePAPProperties')) {
        $('#tablePAPProperties').DataTable().destroy();
    }
    var groupColumn = 4;
    var collapsedGroups = {};

    $.ajax({
        type: "GET",
        url: "/Community/LoadPapProperties",
        data: { papID: papID},
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td style="text-align:center;">' +
                    '<a href="#" class="btn btn-danger" id="deleteProperty"><i class="fas fa-unlink"></i></a>' +
                    '<input type="hidden" class="lacId" value="' + item.LACId + '"/>' +
                    '<input type="hidden" class="ownerId" value="' + item.Owner + '"/>' +
                    '<input type="hidden" class="userId" value="' + item.User + '"/>' +
                    '<input type="hidden" class="propertyId" value="' + item.PropertyId + '"/>' +
                    '</td>' +
                    '<td> <a href="/Community/PropertyDetail/' + item.PropertyId + '">' + item.IPointName + '</a></td>' +
                    '<td>' + item.PropertyTypeName + '</td>' +
                    '<td>' + item.PropertyName + '</td>' +
                    '<td>' + item.LACName + '</td>'+
                    '<td>' + item.UserType + '</td>' +
                    '<td> <a href="/Community/PapDetails/' + item.Owner + '">' + item.Owner +'</a></td>' +
                    '<td><img src="' + item.PictureName + '" width="50" height="50" style="border-radius:5px;"></td>' +
                    '</tr>';
            });

            $("#tablePAPProperties tbody").html(tr);

            var table = $("#tablePAPProperties").DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of PAP Properties'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                order: [[groupColumn, 'asc']],
                rowGroup: {
                    dataSrc: groupColumn,
                    startRender: function (rows, group) {
                        var collapsed = !!collapsedGroups[group];
                        rows.nodes().each(function (r) {
                            r.style.display = collapsed ? 'none' : '';
                        });
                        return $('<tr/>')
                            .append('<td colspan="9">' + group + ' (' + rows.count() + ')</td>')
                            .attr('data-name', group)
                            .toggleClass('collapsed', collapsed);
                    }
                },
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
            $('#tablePAPProperties tbody tr.dtrg-start').each(function () {
                var name = $(this).data('name');
                collapsedGroups[name] = !collapsedGroups[name];
            });
            table.draw(false);
            $('#tablePAPProperties tbody').on('click', 'tr.dtrg-start', function () {
                var name = $(this).data('name');
                collapsedGroups[name] = !collapsedGroups[name];
                table.draw(false);
            });
        }
    });
}

function LoadPapLacDetails(papID) {
    if ($.fn.DataTable.isDataTable('#tablePAPLacList')) {
        $('#tablePAPLacList').DataTable().destroy();
    }

    $.ajax({
        type: "GET",
        url: "/Community/LoadLACPAP",
        data: { PapID: papID },
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td style="text-align:center;">' +
                    '<a href="#" class="btn btn-danger" id="unlinkPAPLAC"><i class="fas fa-unlink"></i></a> ' +
                    '<a href="#" class="btn btn-info" data-dismiss="modal" id="surveyPAP" title="Socio-economic Survey"><i class="fas fa-users"></i></a> ' +
                    '<input type="hidden" class="lacId" value="' + item.LACId + '"/>' +
                    '<input type="hidden" class="papId" value="' + item.PAPId + '"/>' +
                    '<input type="hidden" class="papLacId" value="' + item.PAPLACId + '"/>' +
                    '<input type="hidden" class="fileID" value="' + item.FileNumber + '"/>' +
                    '</td>' +
                    '<td> <a href="/Land/LoadLacDetails/' + item.LACId + '">' + item.LACName + '</a></td>' +
                    '<td>' + item.FileNumber + '</td>' +
                    '<td>' + item.PaymentPreferenceName + '</td>' +
                    '<td>' + item.FormSubmissionDateString + '</td>' +
                    '<td>' + item.PresurveyDateString + '</td>' +
                    '<td>' + item.SurveyDateString + '</td>' +
                    '<td>' + item.Comments + '</td>' +
                    '</tr>';
            });

            $("#tablePAPLacList tbody").html(tr);

            var table = $("#tablePAPLacList").DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of PAP Properties'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function LoadPAPList() {
    if ($.fn.DataTable.isDataTable('#datatablePapList')) {
        $('#datatablePapList').DataTable().destroy();
    }
    $.ajax({
        type: "POST",
        url: "/Community/LoadPAP",
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td>' +
                    '<a href="#" class="btn btn-primary btn-rounded-f" data-dismiss="modal" id="editPAP" title="Edit PAP"><i class="fas fa-edit"></i></a> ' +
                    '<a href="#" class="btn btn-info btn-rounded-f" data-dismiss="modal" id="linkPapLAC" title="Add to LAC"><i class="fas fa-link"></i></a> ' +
                    '<input type="hidden" class="papID" value="' + item.PAPId + '"/>' +
                    '<input type="hidden" class="personID" value="' + item.PersonId +'"/>' +
                    '</td>' +
                    '<td> <a href="/Community/PapDetails/' + item.PAPId + '">' + item.PAPId + '</a></td>' +
                    '<td>' + item.FirstName + '</td>' +
                    '<td>' + item.LastName + '</td>' +
                    '<td>' + item.IdCard + '</td>' +
                    '<td>' + item.Mobile + '</td>' +
                    '<td>' + item.ResidenceName + '</td>' +
                    '<td><img src="' + item.Picture + '" width="50" height="50" style="border-radius:5px;"></td>' +
                    '<td>' + item.DateOfBirthString + '</td>' +
                    '<td>' + item.PlaceOfBirth + '</td>' +
                    '</tr>';
            });
            $("#datatablePapList tbody").html(tr);

            var table = $('#datatablePapList').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of LAC'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
        }
    });
}

function PropertiesList() {
    if ($.fn.DataTable.isDataTable('#datatableProperties')) {
        $('#datatableProperties').DataTable().destroy();
    }
    var groupColumn = 4;
    var collapsedGroups = {};
    $.ajax({
        type: "POST",
        url: "/Community/LoadProperties",
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td>' +
                    '<a href="#" class="btn btn-dark btn-rounded-f" data-dismiss="modal" id="addPAP" title="Add PAP"><i class="fas fa-user-plus"></i></a> ' +
                    '<a href="#" class="btn btn-info btn-rounded-f" data-dismiss="modal" id="linkLandLac" title="Add to Restricted Area"><i class="fas fa-link"></i></a> ' +
                    '<input type="hidden" class="LacRequestId" value="' + item.PropertyId + '"/>' +
                    '<input type="hidden" class="lacID" value="' + item.LACId + '"/>' +
                    '<input type="hidden" class="lacName" value="' + item.LACName + '"/>' +
                    '</td>' +
                    '<td> <a href="/Community/PropertyDetail/' + item.IPointName + '">' + item.IPointName + '</a></td>' +
                    '<td>' + item.FirstName + '</td>' +
                    '<td>' + item.LastName + '</td>' +
                    '<td> <a href="/Land/LoadLacDetails/' + item.LACId + '">' + item.LACName + '</a></td>' +
                    '<td>' + item.IdCard + '</td>' +
                    '<td>' + item.FileNumber + '</td>' +
                    '<td>' + item.Mobile + '</td>' +
                    '<td>' + item.ResidenceName + '</td>' +
                    '<td><img src="' + item.Picture + '" width="50" height="50" style="border-radius:5px;"></td>' +
                    '<td>' + item.DateOfBirthString + '</td>' +
                    '<td>' + item.PlaceOfBirth + '</td>' +
                    '</tr>';
            });
            $("#datatableProperties tbody").html(tr);

            var table = $('#datatableProperties').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of LAC'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                order: [[groupColumn, 'asc']],
                rowGroup: {
                    dataSrc: groupColumn,
                    startRender: function (rows, group) {
                        var collapsed = !!collapsedGroups[group];
                        rows.nodes().each(function (r) {
                            r.style.display = collapsed ? 'none' : '';
                        });
                        return $('<tr/>')
                            .append('<td colspan="12">' + group + ' (' + rows.count() + ')</td>')
                            .attr('data-name', group)
                            .toggleClass('collapsed', collapsed);
                    }
                },
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });

            $('#datatableProperties tbody tr.dtrg-start').each(function () {
                var name = $(this).data('name');
                collapsedGroups[name] = !collapsedGroups[name];
            });
            table.draw(false);
            $('#datatableProperties tbody').on('click', 'tr.dtrg-start', function () {
                var name = $(this).data('name');
                collapsedGroups[name] = !collapsedGroups[name];
                table.draw(false);
            });
        }
    });
}

function LoadPAPDetails(papID, modalName) {
    $.ajax({
        type: "GET",
        url: "/Community/LoadPapDetails/",
        data: { PapID: papID },
        success: function (data) {
            console.log(data);
            if (modalName == "addPAPModal") {
                getLocations("#addPAPModal #primaryResidence");
                getVulnerabilityForSelect("addPAPModal #vulnerabilityType", data.VulnerabilityTypeId);
                getPAP("#addPAPModal #papSpouse");
                getPAP("#addPAPModal #papMother");
                getPAP("#addPAPModal #papfather");

                $('#addPAPModal #firstName').val(data.FirstName);
                $('#addPAPModal #lastName').val(data.LastName);
                $("#addPAPModal #primaryResidence").val(data.ResidenceName);
                $('#addPAPModal #pictureID').val(data.PhotoID);
                $('#addPAPModal #middleName').val(data.MiddleName);
                $('#addPAPModal #cellPhone').val(data.Mobile);
                $('#addPAPModal #email').val(data.Email);
                $('#addPAPModal #pob').val(data.PlaceOfBirth);
                $('#addPAPModal #papfather').val(data.FatherName);
                $('#addPAPModal #papMother').val(data.MotherName);
                $('#addPAPModal #vulnerabilityDetails').val(data.VulnerabilityDetail);
                $('#addPAPModal #papSpouse').val(data.SpouseName);
                $('#addPAPModal #cardNumber').val(data.IdCard);
                $('#addPAPModal #PAPphoto').val();
                $('#addPAPModal #dob').val(convertToJavaScriptDate(data.DateOfBirth));
                $('#addPAPModal #personID').val(data.PersonId);
                $('#addPAPModal #papID').val(data.PAPId);
                $("#addPAPModal input:radio[name='gender'][value=" + data.Gender + "]").prop('checked', true);
                $("#addPAPModal #picture").html('<img src="' + data.Picture + '" width="100" height="100" style="border-radius:5px;">').show();
            }
            else if (modalName == "collectLandModal") {
                getLocations("#collectLandModal #collectPrimaryResidence");
                getVulnerabilityForSelect("collectLandModal #collectVulnerabilityType", data.VulnerabilityTypeId);
                getPAP("#collectLandModal #collectPapSpouse");
                getPAP("#collectLandModal #collectPapMother");
                getPAP("#collectLandModal #collectPapfather");

                $('#collectLandModal #collectFirstName').val(data.FirstName);
                $('#collectLandModal #collectLastName').val(data.LastName);
                $("#collectLandModal #collectPrimaryResidence").val(data.ResidenceName);
                $('#collectLandModal #collectPictureID').val(data.PhotoID);
                $('#collectLandModal #collectMiddleName').val(data.MiddleName);
                $('#collectLandModal #collectCellPhone').val(data.Mobile);
                $('#collectLandModal #collectEmail').val(data.Email);
                $('#collectLandModal #collectPob').val(data.PlaceOfBirth);
                $('#collectLandModal #collectPapfather').val(data.FatherName);
                $('#collectLandModal #collectPapMother').val(data.MotherName);
                $('#collectLandModal #collectVulnerabilityDetails').val(data.VulnerabilityDetail);
                $('#collectLandModal #collectPapSpouse').val(data.SpouseName);
                $('#collectLandModal #collectCardNumber').val(data.IdCard);
                $('#collectLandModal #collectPAPphoto').val();
                $('#collectLandModal #collectDob').val(convertToJavaScriptDate(data.DateOfBirth));
                $('#collectLandModal #personID').val(data.PersonId);
                $("#collectLandModal input:radio[name='gender'][value=" + data.Gender + "]").prop('checked', true);
                $("#collectLandModal #collectPicture").html('<img src="' + data.Picture + '" width="100" height="100" style="border-radius:5px;">').show();
            }
        },
        error: function (error) {
            console.log(error);
            $("#addPAPModal").modal("hide");
            $.alert({
                icon: 'fas fa-exclamation-triangle',
                title: "Alert",
                content: "Something went wrong. If this persists, please contact the administrator",
                type: "red"
            });
        }
    });
}

function LoadOwnerLacDetails(papID, lacID, modal) {
    let pap = papID.split('(')[1];
    pap = pap.split(')')[0];

    console.log("PAP ID " + papID + " PAP " + pap + " Modal Name " + modal + " LAC ID " + lacID);
    
    if (pap != null || pap != "") {
        $.ajax({
            type: "GET",
            url: "/Community/LoadPapDetailsLac/",
            data: { PersonID: pap, LacID: lacID },
            success: function (data) {
                console.log(data);
                $('#' + modal + ' #ownerFirstName').val(data.FirstName);
                $('#' + modal + ' #ownerLastName').val(data.LastName);
                $('#' + modal + ' #ownerPrimaryResidence').val(data.ResidenceName).prop('disabled', true);
                $('#' + modal + ' #ownerPicture').fileinput('disable');
                $('#' + modal + ' #ownerPhotoID').val(data.PhotoID).prop('disabled', true);
                $('#' + modal + ' #ownerMiddleName').val(data.MiddleName).prop('disabled', true);
                if (data.FileNumber == "" || data.FileNumber == null)
                    $('#' + modal + ' #ownerFileID').val(data.FileNumber).prop('disabled', false);
                else
                    $('#' + modal + ' #ownerFileID').val(data.FileNumber).prop('disabled', true);
                $('#' + modal + ' #ownerPersonId').val(data.PersonId).prop('disabled', true);
            },
            error: function (error) {
                console.log(error);
                $("#" + modal).modal("hide");
                $.alert({
                    icon: 'fas fa-exclamation-triangle',
                    title: "Alert",
                    content: "Something went wrong. If this persists, please contact the administrator",
                    type: "red"
                });
            }
        });
    }
}

function LoadPAPLacDetails(papID, lacID, modal) {
    let pap = "";
    if ($.isNumeric(papID))
        pap = papID;
    else {
        if (papID.includes('(')) {
            pap = papID.split('(')[1];
            pap = pap.split(')')[0];
        }
    }

    //console.log("PAP ID " + papID + " PAP " + pap + " Modal Name " + modal + " LAC ID " + lacID);

    if (pap != "") {//pap != null || 
        $.ajax({
            type: "GET",
            url: "/Community/LoadPapDetailsLac/",
            data: { PersonID: pap, LacID: lacID },
            success: function (data) {
                console.log(data);
                if (modal == 'addPAPPropertiesModal') {
                    if (data.FileNumber == "" || data.FileNumber == null)
                        $('#addPAPPropertiesModal #fileID').val(data.FileNumber).prop('disabled', false);
                    else
                        $('#addPAPPropertiesModal #fileID').val(data.FileNumber).prop('disabled', true);
                }
                else if (modal == 'addPAPLACModal') {
                    $("#addPAPLACModal #lastName").val(data.LastName);
                    $('#addPAPLACModal #firstName').val(data.FirstName);
                    $("#addPAPLACModal #middleName").val(data.MiddleName).prop('disabled', true);
                    $('#addPAPLACModal #primaryResidence').val(data.ResidenceName).prop('disabled', true);
                    $('#addPAPLACModal #PAPphoto').fileinput('disable');
                    $('#addPAPLACModal #pictureID').val(data.PhotoID).prop('disabled', true);
                    if (data.FileNumber == "" || data.FileNumber == null)
                        $('#addPAPLACModal #fileID').val(data.FileNumber).prop('disabled', false);
                    else
                        $('#addPAPLACModal #fileID').val(data.FileNumber).prop('disabled', true);
                    $('#addPAPLACModal #PersonId').val(data.PersonId).prop('disabled', true);
                }
            },
            error: function (error) {
                console.log(error);
                $("#" + modal).modal("hide");
                $.alert({
                    icon: 'fas fa-exclamation-triangle',
                    title: "Alert",
                    content: "Something went wrong. If this persists, please contact the administrator",
                    type: "red"
                });
            }
        });
    }
}

function LoadLACPAPList() {
    if ($.fn.DataTable.isDataTable('#datatableLacPapList')) {
        $('#datatableLacPapList').DataTable().destroy();
    }
    var groupColumn = 4;
    var collapsedGroups = {};
    let powerUser = $('#poweruser').val();
    let surveyor = $('#surveyor').val();
    let topo = $('#topograph').val();
    $.ajax({
        type: "POST",
        url: "/Community/LoadLACPAPList",
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                if (item.LACId == '-1') {
                    tr += '<tr>' +
                        '<td>' +
                        '<a href="#" class="btn btn-primary btn-rounded-f ' + powerUser+'" data-dismiss="modal" id="editPAP" title="Edit PAP"><i class="fas fa-edit"></i></a> ' +
                        '<a href="#" class="btn btn-info btn-rounded-f ' + surveyor+'" data-dismiss="modal" id="surveyPAP" title="Socio-economic Survey"><i class="fas fa-users"></i></a> ' +
                        '<a href="#" class="btn btn-info btn-rounded-f" data-dismiss="modal" id="linkPapLAC" title="Add to LAC"><i class="fas fa-link"></i></a> ' +
                        '<input type="hidden" class="papID" value="' + item.PAPId + '"/>' +
                        '<input type="hidden" class="lacID" value="' + item.LACId + '"/>' +
                        '<input type="hidden" class="paplacID" value="' + item.PAPLACId + '"/>' +
                        '<input type="hidden" class="fileID" value="' + item.FileNumber + '"/>' +
                        '<input type="hidden" class="householdID" value="' + item.HouseHoldId + '"/>' +
                        '<input type="hidden" class="personID" value="' + item.PersonId + '"/>' +
                        '</td>' +
                        '<td> <a href="/Community/PapDetails/' + item.PAPId + '">' + item.PAPId + '</a></td>' +
                        '<td>' + item.FirstName + '</td>' +
                        '<td>' + item.LastName + '</td>' +
                        '<td>' + item.LACName + '</td>' +
                        '<td>' + item.FileNumber + '</td>' +
                        '<td>' + item.IdCard + '</td>' +
                        '<td>' + item.Mobile + '</td>' +
                        '<td>' + item.ResidenceName + '</td>' +
                        '<td><img src="' + item.Picture + '" width="50" height="50" style="border-radius:5px;"></td>' +
                        '</tr>';
                }
                else if (item.HouseHoldId == null || item.HouseHoldId < 1) {
                    tr += '<tr>' +
                        '<td>' +
                        '<a href="#" class="btn btn-primary btn-rounded-f ' + powerUser +'" data-dismiss="modal" id="editPAP" title="Edit PAP"><i class="fas fa-edit"></i></a> ' +
                        '<a href="#" class="btn btn-info btn-rounded-f ' + surveyor +'" data-dismiss="modal" id="surveyPAP" title="Socio-economic Survey"><i class="fas fa-users"></i></a> ' +
                        '<a href="#" class="btn btn-default btn-rounded-f ' + topo + ' disabled" data-dismiss="modal" id="collectLand" title="Collect Land Data"><i class="fas fa-ruler-combined"></i></a> ' +
                        '<input type="hidden" class="papID" value="' + item.PAPId + '"/>' +
                        '<input type="hidden" class="lacID" value="' + item.LACId + '"/>' +
                        '<input type="hidden" class="paplacID" value="' + item.PAPLACId + '"/>' +
                        '<input type="hidden" class="fileID" value="' + item.FileNumber + '"/>' +
                        '<input type="hidden" class="householdID" value="' + item.HouseHoldId + '"/>' +
                        '<input type="hidden" class="personID" value="' + item.PersonId + '"/>' +
                        '</td>' +
                        '<td> <a href="/Community/PapDetails/' + item.PAPId + '">' + item.PAPId + '</a></td>' +
                        '<td>' + item.FirstName + '</td>' +
                        '<td>' + item.LastName + '</td>' +
                        '<td>' + item.LACName + '</td>' +
                        '<td>' + item.FileNumber + '</td>' +
                        '<td>' + item.IdCard + '</td>' +
                        '<td>' + item.Mobile + '</td>' +
                        '<td>' + item.ResidenceName + '</td>' +
                        '<td><img src="' + item.Picture + '" width="50" height="50" style="border-radius:5px;"></td>' +
                        '</tr>';
                } else {
                    tr += '<tr>' +
                        '<td>' +
                        '<a href="#" class="btn btn-primary btn-rounded-f ' + powerUser + '" data-dismiss="modal" id="editPAP" title="Edit PAP"><i class="fas fa-edit"></i></a> ' +
                        '<a href="#" class="btn btn-info btn-rounded-f ' + surveyor + ' disabled " data-dismiss="modal" id="surveyPAP" title="Socio-economic Survey"><i class="fas fa-users"></i></a> ' +
                        '<a href="#" class="btn btn-default btn-rounded-f ' + topo + '" data-dismiss="modal" id="collectLand" title="Collect Land Data"><i class="fas fa-ruler-combined"></i></a> ' +
                        '<input type="hidden" class="papID" value="' + item.PAPId + '"/>' +
                        '<input type="hidden" class="lacID" value="' + item.LACId + '"/>' +
                        '<input type="hidden" class="paplacID" value="' + item.PAPLACId + '"/>' +
                        '<input type="hidden" class="fileID" value="' + item.FileNumber + '"/>' +
                        '<input type="hidden" class="householdID" value="' + item.HouseHoldId + '"/>' +
                        '<input type="hidden" class="personID" value="' + item.PersonId + '"/>' +
                        '</td>' +
                        '<td> <a href="/Community/PapDetails/' + item.PAPId + '">' + item.PAPId + '</a></td>' +
                        '<td>' + item.FirstName + '</td>' +
                        '<td>' + item.LastName + '</td>' +
                        '<td>' + item.LACName + '</td>' +
                        '<td>' + item.FileNumber + '</td>' +
                        '<td>' + item.IdCard + '</td>' +
                        '<td>' + item.Mobile + '</td>' +
                        '<td>' + item.ResidenceName + '</td>' +
                        '<td><img src="' + item.Picture + '" width="50" height="50" style="border-radius:5px;"></td>' +
                        '</tr>';
                }
            });
            $("#datatableLacPapList tbody").html(tr);

            var table = $('#datatableLacPapList').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    'copyHtml5',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of PAP'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                order: [[groupColumn, 'asc']],
                rowGroup: {
                    dataSrc: groupColumn,
                    startRender: function (rows, group) {
                        var collapsed = !!collapsedGroups[group];
                        rows.nodes().each(function (r) {
                            r.style.display = collapsed ? 'none' : '';
                        });
                        var toggleClass = collapsed ? 'fa-plus-square' : 'fa-minus-square';
                        return $('<tr/>')
                            .append('<td colspan="10"><i class="far fa-fw ' + toggleClass + ' toggler"></i>' + group + ' ( ' + rows.count() + ' )</td>')
                            .attr('data-name', group)
                            .toggleClass('collapsed', collapsed);
                    }
                },
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }, { "visible": false, "targets": groupColumn }]
            });
            $('#datatableLacPapList tbody tr.dtrg-start').each(function () {
                var name = $(this).data('name');
                collapsedGroups[name] = !collapsedGroups[name];
            });
            table.draw(false);
            $('#datatableLacPapList tbody').on('click', 'tr.dtrg-start', function () {
                var name = $(this).data('name');
                collapsedGroups[name] = !collapsedGroups[name];
                table.draw(false);
            });
        }
    });
}

function LoadPAPLACProperties(papID, lacID) {
    //LoadPapLacProperties
    if ($.fn.DataTable.isDataTable('#tablePAPProperties')) {
        $('#tablePAPProperties').DataTable().destroy();
    }
    var groupColumn = 4;
    var collapsedGroups = {};

    $.ajax({
        type: "GET",
        url: "/Community/LoadPapLacProperties",
        data: { papID: papID, lacID: lacID },
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td style="text-align:center;">' +
                    '<a href="#" class="btn btn-dark" id="editProperty"><i class="fas fa-edit"></i></a>' +
                    '<input type="hidden" class="lacId" value="' + item.LACId + '"/>' +
                    '<input type="hidden" class="ownerId" value="' + item.Owner + '"/>' +
                    '<input type="hidden" class="userId" value="' + item.User + '"/>' +
                    '<input type="hidden" class="propertyId" value="' + item.PropertyId + '"/>' +
                    '</td>' +
                    '<td> <a href="/Community/PropertyDetail/' + item.PropertyId + '">' + item.IPointName + '</a></td>' +
                    '<td>' + item.PropertyTypeName + '</td>' +
                    '<td>' + item.PropertyName + '</td>' +
                    '<td>' + item.LACName + '</td>' +
                    '<td>' + item.UserType + '</td>' +
                    '<td> <a href="/Community/PapDetails/' + item.Owner + '">' + item.Owner + '</a></td>' +
                    '<td><img src="' + item.PictureName + '" width="50" height="50" style="border-radius:5px;"></td>' +
                    '</tr>';
            });

            $("#tablePAPProperties tbody").html(tr);

            var table = $("#tablePAPProperties").DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'pageLength',
                    {
                        extend: 'excelHtml5',
                        autoFilter: true,
                        sheetName: 'List of PAP Properties'
                    },
                    'print'
                ],
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                order: [[groupColumn, 'asc']],
                rowGroup: {
                    dataSrc: groupColumn,
                    startRender: function (rows, group) {
                        var collapsed = !!collapsedGroups[group];
                        rows.nodes().each(function (r) {
                            r.style.display = collapsed ? 'none' : '';
                        });
                        return $('<tr/>')
                            .append('<td colspan="9">' + group + ' (' + rows.count() + ')</td>')
                            .attr('data-name', group)
                            .toggleClass('collapsed', collapsed);
                    }
                },
                columnDefs:
                    [{
                        targets: [0],
                        searchable: false,
                        orderable: false
                    }]
            });
            $('#tablePAPProperties tbody tr.dtrg-start').each(function () {
                var name = $(this).data('name');
                collapsedGroups[name] = !collapsedGroups[name];
            });
            table.draw(false);
            $('#tablePAPProperties tbody').on('click', 'tr.dtrg-start', function () {
                var name = $(this).data('name');
                collapsedGroups[name] = !collapsedGroups[name];
                table.draw(false);
            });
        }
    });
}

function LoadPAPLACProperties2(papID, lacID) {
    if ($.fn.DataTable.isDataTable('#tablePAPProperties')) {
        $('#tablePAPProperties').DataTable().destroy();
    }

    $.ajax({
        type: "GET",
        url: "/Community/LoadPapLacProperties",
        data: { papID: papID, lacID: lacID },
        success: function (response) {
            var tr = "";
            $.each(response, function (i, item) {
                tr += '<tr>' +
                    '<td style="display:none">' + item.PropertyId + '</td>' +
                    '<td>' + item.IPointName + '</td>' +
                    '<td> <a href="/Community/PropertyDetail/' + item.PropertyId + '">' + item.IPointName + '</a></td>' +
                    '<td>' + item.PropertyType + '</td>' +
                    '<td>' + item.PropertyTypeName + '</td>' +
                    '<td>' + item.PropertyName + '</td>' +
                    '<td>' + item.UserType + '</td>' +
                    '<td> <a href="/Community/PapDetails/' + item.Owner + '">' + item.Owner + '</a></td>' +
                    '</tr>';
            });

            $("#tablePAPProperties tbody").html(tr);

            $("#tablePAPProperties").DataTable({
                //dom: 'Bfrtip',
                select: true,
                destroy: true,
                ordering: false,
                autoWidth: false,
                responsive: true,
                columnDefs:
                    [
                        { targets: [0], visible: false },
                        { targets: [1], visible: false },
                        { targets: [3], visible: false }
                    ]
            });
        }
    });
}

function LoadPropertyDetails(propertyID, propertyType, papID) {
    //This function will load property details, and load it on the Modal Form
    $.ajax({
        type: "GET",
        url: "/Community/LoadPropertyDetails/",
        data: { PropertyID: propertyID },
        success: function (data) {

            getAssetTypeForSelect("collectLandModal #goodType", propertyType);
            let assetType = data.PropertyTypeName;
            
            let userType = (papID == data.OwnerID && papID != data.User) ? "P" : ((papID != data.OwnerID && papID == data.User)?"U":"PU");
            $("#collectLandModal input:radio[name='owner'][value=" + userType + "]").prop('checked', true);

            if ($("#collectLandModal input:radio[name='owner']").prop('checked'))
                $("#collectLandModal #ownerDetails").show();
            else
                $("#collectLandModal #ownerDetails").hide();

            $("#collectLandModal #gpsID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #goodPictureID, #collectLandModal #goodComments, #collectLandModal #goodPhoto, #collectLandModal #ownershipType").prop('disabled', false);

            if (userType == "PU")
                $("#collectLandModal #ownershipType").prop('disabled', true);
            else
                $("#collectLandModal #ownershipType").prop('disabled', false);

            $("#collectLandModal #cultureDetails, #collectLandModal #structureDetails, #collectLandModal #treeDetails").hide();

            $("#collectLandModal #propertyID").val(propertyID);
            $("#collectLandModal #goodComments").val(data.Comments);
            $("#collectLandModal #ownerPersonId").val(data.OwnerID);
            $("#collectLandModal #ownerFirstName").val(data.OwnerFirstName);
            $("#collectLandModal #ownerMiddleName").val(data.OwnerMiddleName);
            $("#collectLandModal #ownerLastName").val(data.OwnerLastName);

            getLocations("collectLandModal #ownerPrimaryResidence");
            getCultureType("#collectLandModal #cultureName");
            
            if (assetType == "Culture") {
                $("#collectLandModal #cultureDetails").show();
                $("#collectLandModal #cultureName").val(data.PropertyName);
                $("#collectLandModal #cultureSurface").val(data.Surface);
                getSurfaceUOM("collectLandModal #cultureSurfaceUOM", data.SurfaceUOM);
                $("#collectLandModal #cultureGpsID").val();
                $("#collectLandModal #cultureEasting").val();
                $("#collectLandModal #cultureNorthing").val();
                $("#collectLandModal #cultureSince").val(data.CultureStartDate);
                getFieldTypeForSelect("collectLandModal #fieldType", data.CultureOccupationType);
            }

            if (assetType == "Structure") {
                $("#collectLandModal #structureDetails").show();
                getStructureForSelect("collectLandModal #structureCode", data.StructureType);
                $("#collectLandModal #structureName").val(data.PropertyName);
                getListDetails("collectLandModal #murCode", data.WallType, "Wall");
                getListDetails("collectLandModal #toitCode", data.RoofType, "Roof");
                getListDetails("collectLandModal #solCode", data.SoilType, "Floor");
                $("#collectLandModal #structureLength").val(data.StructureLength);
                $("#collectLandModal #structureWidth").val(data.StructureWidth);
                $("#collectLandModal #structurePiece").val(data.RoomQty);
            }
                
            if (assetType == "Tree") {
                $("#collectLandModal #treeDetails").show();
                $("#collectLandModal #treeName").val(data.PropertyName);
                getTreeMaturityForSelect("collectLandModal #treeMaturity", data.TreeMaturity);
                $("#collectLandModal #treeQty").val(data.TreeQty);
            }
            $('#collectLandModal #gpsID').focus();
        },
        error: function (error) {
            console.log(error);
            $.alert({
                icon: 'fas fa-exclamation-triangle',
                title: "Alert",
                content: "Something went wrong. If this persists, please contact the administrator",
                type: "red"
            });
        }
    });
}

//#endregion

//#region COMMUNITY MANAGEMENT
//#region ADD PAP

$("#datatable-lacList").on("click", "#addPAP", function () {
    let row = $(this).closest("tr");
    let lacId = row.find(".lacID").val();
    let lacName = row.find(".lacName").val();
    formData = new FormData();

    launchModalWizard(lacId, lacName);
});

$("#datatable-lacList").on("click", "#linkLandLac", function () {
    let row = $(this).closest("tr");
    let lacID = row.find(".lacID").val();

    $("#addImpactedLACModal #LacID").val(lacID);
    $("#addImpactedLACModal #addImpactedLACModalErrors").hide();
    getRestrictedAreaForSelect("addImpactedLACModal #LandID", '-1');
    $("#addImpactedLACModal").modal("show");
});

$("#addPAPLACModal #presurveyorDate, #addPAPLACModal #presurveyorCode, #addPAPLACModal #presurveyorGPS").on("focus", function () { $("#addPAPLACModal #papSurveyorError").hide(); });
$("#addPAPLACModal #presurveyorDate, #addPAPLACModal #presurveyorCode, #addPAPLACModal #presurveyorGPS").on("change", function () { $("#addPAPLACModal #papSurveyorError").hide(); });

$("#addPAPLACModal #firstName, #addPAPLACModal #primaryResidence, #addPAPLACModal #pictureID, #addPAPLACModal #PAPphoto, #addPAPLACModal #fileID").on("focus", function () { $("#addPAPLACModal #papDetailsError").hide(); });
$("#addPAPLACModal #firstName, #addPAPLACModal #primaryResidence, #addPAPLACModal #pictureID, #addPAPLACModal #PAPphoto, #addPAPLACModal #fileID").on("change", function () { $("#addPAPLACModal #papDetailsError").hide(); });

$("#addPAPLACModal .reset").on("focus", function () { $("#addPAPLACModal #tableGoodsError").hide(); });
$("#addPAPLACModal .reset").on("change", function () { $("#addPAPLACModal #tableGoodsError").hide(); });

$("#addPAPLACModal #goodType").on("change", function () {
    let assetType = $("#addPAPLACModal #goodType option:selected").text();
    $("#addPAPLACModal #cultureDetails, #addPAPLACModal #structureDetails, #addPAPLACModal #treeDetails").hide();
    if (assetType == "Culture")
        $("#addPAPLACModal #cultureDetails").show();
    if (assetType == "Structure")
        $("#addPAPLACModal #structureDetails").show();
    if (assetType == "Tree")
        $("#addPAPLACModal #treeDetails").show();
});

$("#addPAPLACModal #ownershipType").on("change", function () {
    if ($(this).prop('checked')) 
        $("#addPAPLACModal #ownerDetails").show();
    else
        $("#addPAPLACModal #ownerDetails").hide();
});

$('#addPAPLACModal').on("click", "#addGoods", function (e) {
    e.preventDefault();
    var errors = [];
    let user = ($('#addPAPLACModal input[name="ownershipType"]:checked').val() == null ? "PU" : $('#addPAPLACModal input[name="ownershipType"]:checked').val());

    if ($('#addPAPLACModal #gpsID').val() == null || $('#addPAPLACModal #gpsID').val() == "")
        errors.push("Please specify the GPS point");
    if ($('#addPAPLACModal #pointEasting').val() == null || $('#addPAPLACModal #pointEasting').val() == "")
        errors.push("Please specify Point Easting");
    if ($('#addPAPLACModal #pointNorthing').val() == null || $('#addPAPLACModal #pointNorthing').val() == "")
        errors.push("Please specify Point Northing");
    if ($('#addPAPLACModal #goodPictureID').val() == null || $('#addPAPLACModal #goodPictureID').val() == "")
        errors.push("Please add the picture ID");
    if (isNaN($('#addPAPLACModal #goodType').val()) || $('#addPAPLACModal #goodType').val() == "")
        errors.push("Please specify a valid property type");
    if (user == 'U' && ($('#addPAPLACModal #ownerFirstName').val() == null || $('#addPAPLACModal #ownerFirstName').val() == ""))
        errors.push("Please specify Owner First Name");
    if (user == 'U' && ($('#addPAPLACModal #ownerLastName').val() == null || $('#addPAPLACModal #ownerLastName').val() == ""))
        errors.push("Please specify Owner Last Name");
    if (user == 'U' && ($('#addPAPLACModal #ownerFileID').val() == null || $('#addPAPLACModal #ownerFileID').val() == ""))
        errors.push("Please specify Owner file ID");

    if ($('#addPAPLACModal #fieldType').val() == null || $('#addPAPLACModal #fieldType').val() == "")
        errors.push("Please choose the Field Type");
    if (isNaN($('#addPAPLACModal #cultureSurface').val()) || $('#addPAPLACModal #cultureSurface').val() == "")
        errors.push("Please specify the surface");
    if ($('#addPAPLACModal #cultureSurfaceUOM').val() < 1 || $('#addPAPLACModal #cultureSurfaceUOM').val() == "")
        errors.push("Please specify Culture Surface UOM");
    if ($('#addPAPLACModal #goodPhoto').val() == null || $('#addPAPLACModal #goodPhoto').val() == "")
        errors.push("You did not put Picture ID");

    let ownerPhoto = $("#addPAPLACModal #ownerPicture")[0];
    let ownerPhotoFile = ownerPhoto.files;
    let goodPhoto = $("#addPAPLACModal #goodPhoto")[0];
    let goodPhotoFile = goodPhoto.files;

    if (goodPhoto.files.length > 0)
        formData.append(goodPhotoFile[0].name, goodPhotoFile[0]);

    if (ownerPhoto.files.length > 0)
        formData.append(ownerPhotoFile[0].name, ownerPhotoFile[0]);

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addPAPLACModal #tableGoodsError").html(errorMessage).show(250);
    }
    else {
        //var user = $('#addPAPLACModal #ownershipType').prop('checked') ? 'U' : 'PU';
        let user = ($('#addPAPLACModal input[name="ownershipType"]:checked').val() == null ? "PU" : $('#addPAPLACModal input[name="ownershipType"]:checked').val());
        var tr = "";
        tr += '<tr>' +
            '<td>' + $('#addPAPLACModal #gpsID').val() + '</td>' +
            '<td>' + $('#addPAPLACModal #goodPictureID').val() + '</td>' +
            '<td style="display:none"> ' + $('#addPAPLACModal #goodPhoto').val() + '</td>' +
            '<td>' + user + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #goodType').val() + '</td>' +
            '<td>' + $('#addPAPLACModal #goodType option:selected').text() + '</td>' +
            '<td>' + $('#addPAPLACModal #cultureName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #pointEasting').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #pointNorthing').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #pointElevation').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerFirstName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerMiddleName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerLastName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPrimaryResidence').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPrimaryResidence option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPhotoID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerFileID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPicture').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #fieldType').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #fieldType option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #cultureSurface').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#addPAPLACModal #goodComments').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPersonId').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #cultureSurfaceUOM').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #cultureSurfaceUOM option:selected').text() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeGoods" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';
        $("#propertiesTable tbody").append(tr);
        $('#addPAPLACModal #gpsID, #addPAPLACModal #goodPictureID, #addPAPLACModal #pointEasting, #addPAPLACModal #pointNorthing, #addPAPLACModal #pointElevation, #addPAPLACModal #goodPhoto, #addPAPLACModal #cultureName, #addPAPLACModal #cultureSurface, #addPAPLACModal #ownerFirstName, #addPAPLACModal #ownerMiddleName, #addPAPLACModal #ownerLastName, #addPAPLACModal #ownerPhotoID, #addPAPLACModal #ownerFileID, #addPAPLACModal #goodComments').val('');
        $("#addPAPLACModal #goodType, #addPAPLACModal #fieldType, #addPAPLACModal #ownerPrimaryResidence").val("-1");
        $('#addPAPLACModal #goodPhoto, #addPAPLACModal #ownerPicture').fileinput('clear');
        $('#addPAPLACModal #ownershipType').prop("checked", false);
        $("#addPAPLACModal .hiden").hide();
        $('#addPAPLACModal #gpsID').focus();
    }
    return false;
});

$('#addPAPLACModal').on("click", "#addStructure", function (e) {
    e.preventDefault();
    var errors = [];
    let user = ($('#addPAPLACModal input[name="ownershipType"]:checked').val() == null ? "PU" : $('#addPAPLACModal input[name="ownershipType"]:checked').val());

    if ($('#addPAPLACModal #gpsID').val() == null || $('#addPAPLACModal #gpsID').val() == "")
        errors.push("Please specify the GPS point");
    if ($('#addPAPLACModal #pointEasting').val() == null || $('#addPAPLACModal #pointEasting').val() == "")
        errors.push("Please specify Point Easting");
    if ($('#addPAPLACModal #pointNorthing').val() == null || $('#addPAPLACModal #pointNorthing').val() == "")
        errors.push("Please specify Point Northing");
    if ($('#addPAPLACModal #goodPictureID').val() == null || $('#addPAPLACModal #goodPictureID').val() == "")
        errors.push("Please add the picture ID");
    if (isNaN($('#addPAPLACModal #goodType').val()) || $('#addPAPLACModal #goodType').val() == "")
        errors.push("Please specify a valid property type");
    if (user == 'U' && ($('#addPAPLACModal #ownerFirstName').val() == null || $('#addPAPLACModal #ownerFirstName').val() == ""))
        errors.push("Please specify Owner First Name");
    if (user == 'U' && ($('#addPAPLACModal #ownerLastName').val() == null || $('#addPAPLACModal #ownerLastName').val() == ""))
        errors.push("Please specify Owner Last Name");
    if (user == 'U' && ($('#addPAPLACModal #ownerFileID').val() == null || $('#addPAPLACModal #ownerFileID').val() == ""))
        errors.push("Please specify Owner file ID");

    if ($("#addPAPLACModal #structureCode").val() == "" || $("#addPAPLACModal #structureCode").val() == null || isNaN($("#addPAPLACModal #structureCode").val()))
        errors.push("Structure Code can not be empty");
    if ($("#addPAPLACModal #toitCode").val() == "" || $("#addPAPLACModal #toitCode").val() == null || isNaN($("#addPAPLACModal #toitCode").val()))
        errors.push("Roof Code can not be empty");
    if ($("#addPAPLACModal #murCode").val() == "" || $("#addPAPLACModal #murCode").val() == null || isNaN($("#addPAPLACModal #murCode").val()))
        errors.push("Wall Code can not be empty");
    if ($("#addPAPLACModal #solCode").val() == "" || $("#addPAPLACModal #solCode").val() == null || isNaN($("#addPAPLACModal #solCode").val()))
        errors.push("Floor Code can not be empty");
    if ($("#addPAPLACModal #goodPhoto").val() == "" || $("#addPAPLACModal #goodPhoto").val() == null)
        errors.push("Please upload the picture of this structure");
    if ($("#addPAPLACModal #structureUsage").val() == "" || $("#addPAPLACModal #structureUsage").val() == null || isNaN($("#addPAPLACModal #structureUsage").val()))
        errors.push("Choose the structure Usage");
    if ($("#addPAPLACModal #structureLength").val() == "" || $("#addPAPLACModal #structureLength").val() == null)
        errors.push("Structure surface can't be empty");
    if ($("#addPAPLACModal #structureWidth").val() == "" || $("#addPAPLACModal #structureWidth").val() == null)
        errors.push("Structure surface can't be empty");
    if ($("#addPAPLACModal #structurePiece").val() == "" || $("#addPAPLACModal #structurePiece").val() == null || isNaN($("#addPAPLACModal #structurePiece").val()))
        errors.push("Specify the number of rooms");

    let ownerPhoto = $("#addPAPLACModal #ownerPicture")[0];
    let ownerPhotoFile = ownerPhoto.files;
    let goodPhoto = $("#addPAPLACModal #goodPhoto")[0];//#structurePhoto
    let goodPhotoFile = goodPhoto.files;

    if (goodPhoto.files.length > 0)
        formData.append(goodPhotoFile[0].name, goodPhotoFile[0]);

    if (ownerPhoto.files.length > 0)
        formData.append(ownerPhotoFile[0].name, ownerPhotoFile[0]);

    if (errors.length > 0) {
        var errorMessage = "<ul>";
        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addPAPLACModal #tableGoodsError").html(errorMessage).show(250);
    }
    else {
        var tr = "";
        tr += '<tr>' +
            '<td>' + $('#addPAPLACModal #gpsID').val() + '</td>' +
            '<td>' + $('#addPAPLACModal #goodPictureID').val() + '</td>' +
            '<td style="display:none"> ' + $('#addPAPLACModal #goodPhoto').val() + '</td>' +
            '<td>' + user + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #goodType').val() + '</td>' +
            '<td>' + $('#addPAPLACModal #goodType option:selected').text() + '</td>' +
            '<td>' + $('#addPAPLACModal #structureCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #pointEasting').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #pointNorthing').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #pointElevation').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerFirstName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerMiddleName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerLastName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPrimaryResidence').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPrimaryResidence option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPhotoID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerFileID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPicture').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#addPAPLACModal #structureCode').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #structureCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #toitCode').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #toitCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #murCode').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #murCode option:selected').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #solCode').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #solCode option:selected').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #structureUsage').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #structureUsage option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #structureLength').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #structureWidth').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #structurePiece').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#addPAPLACModal #structureComments').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPersonId').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeGoods" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';

        $("#propertiesTable tbody").append(tr);
        $('#addPAPLACModal #gpsID, #addPAPLACModal #goodPictureID, #addPAPLACModal #pointEasting, #addPAPLACModal #pointNorthing, #addPAPLACModal #pointElevation, #addPAPLACModal #goodPhoto, #addPAPLACModal #ownerFirstName, #addPAPLACModal #ownerMiddleName, #addPAPLACModal #ownerLastName, #addPAPLACModal #ownerPhotoID, #addPAPLACModal #ownerFileID, #addPAPLACModal #structureLength, #addPAPLACModal #structureWidth, #addPAPLACModal #structurePiece, #addPAPLACModal #structureComments').val('');
        $("#addPAPLACModal #goodType, #addPAPLACModal #ownerPrimaryResidence, #addPAPLACModal #structureCode, #addPAPLACModal #toitCode, #addPAPLACModal #murCode, #addPAPLACModal #solCode, #addPAPLACModal #structureUsage").val("-1");
        $('#addPAPLACModal #goodPhoto, #addPAPLACModal #ownerPicture').fileinput('clear');
        $('#addPAPLACModal #ownershipType').prop("checked", false);
        $("#addPAPLACModal .hiden").hide();
        $('#addPAPLACModal #gpsID').focus();
    }
    return false;
});

$('#addPAPLACModal').on("click", "#addTree", function (e) {
    e.preventDefault();
    var errors = [];
    let user = ($('#addPAPLACModal input[name="ownershipType"]:checked').val() == null ? "PU" : $('#addPAPLACModal input[name="ownershipType"]:checked').val());

    if ($('#addPAPLACModal #gpsID').val() == null || $('#addPAPLACModal #gpsID').val() == "")
        errors.push("Please specify the GPS point");
    if ($('#addPAPLACModal #pointEasting').val() == null || $('#addPAPLACModal #pointEasting').val() == "")
        errors.push("Please specify Point Easting");
    if ($('#addPAPLACModal #pointNorthing').val() == null || $('#addPAPLACModal #pointNorthing').val() == "")
        errors.push("Please specify Point Northing");
    if ($('#addPAPLACModal #goodPictureID').val() == null || $('#addPAPLACModal #goodPictureID').val() == "")
        errors.push("Please add the picture ID");
    if (isNaN($('#addPAPLACModal #goodType').val()) || $('#addPAPLACModal #goodType').val() == "")
        errors.push("Please specify a valid property type");
    if (user == 'U' && ($('#addPAPLACModal #ownerFirstName').val() == null || $('#addPAPLACModal #ownerFirstName').val() == ""))
        errors.push("Please specify Owner First Name");
    if (user == 'U' && ($('#addPAPLACModal #ownerLastName').val() == null || $('#addPAPLACModal #ownerLastName').val() == ""))
        errors.push("Please specify Owner Last Name");
    if (user == 'U' && ($('#addPAPLACModal #ownerFileID').val() == null || $('#addPAPLACModal #ownerFileID').val() == ""))
        errors.push("Please specify Owner file ID");

    if ($('#addPAPLACModal #treeName').val() == null || $('#addPAPLACModal #treeName').val() == "")
        errors.push("Please specify the tree name");
    if (isNaN($('#addPAPLACModal #treeQty').val()) || $('#addPAPLACModal #treeQty').val() == "")
        errors.push("Please specify a valid count number of trees");
    if ($('#addPAPLACModal #goodPhoto').val() == null || $('#addPAPLACModal #goodPhoto').val() == "")
        errors.push("You did not put Picture");

    let ownerPhoto = $("#addPAPLACModal #ownerPicture")[0];
    let ownerPhotoFile = ownerPhoto.files;
    let goodPhoto = $("#addPAPLACModal #goodPhoto")[0];//#treePhoto
    let goodPhotoFile = goodPhoto.files;

    if (goodPhoto.files.length > 0)
        formData.append(goodPhotoFile[0].name, goodPhotoFile[0]);

    if (ownerPhoto.files.length > 0)
        formData.append(ownerPhotoFile[0].name, ownerPhotoFile[0]);

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addPAPLACModal #tableGoodsError").html(errorMessage).show(250);
    }
    else {
        var tr = "";
        tr += '<tr>' +
            '<td>' + $('#addPAPLACModal #gpsID').val() + '</td>' +
            '<td>' + $('#addPAPLACModal #goodPictureID').val() + '</td>' +
            '<td style="display:none"> ' + $('#addPAPLACModal #goodPhoto').val() + '</td>' +
            '<td>' + user + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #goodType').val() + '</td>' +
            '<td>' + $('#addPAPLACModal #goodType option:selected').text() + '</td>' +
            '<td>' + $('#addPAPLACModal #treeName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #pointEasting').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #pointNorthing').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #pointElevation').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerFirstName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerMiddleName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerLastName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPrimaryResidence').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPrimaryResidence option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPhotoID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerFileID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPicture').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#addPAPLACModal #treeMaturity').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #treeMaturity option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #treeQty').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #treeComments').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPLACModal #ownerPersonId').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeGoods" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';

        $("#propertiesTable tbody").append(tr);

        $('#addPAPLACModal #gpsID, #addPAPLACModal #goodPictureID, #addPAPLACModal #pointEasting, #addPAPLACModal #pointNorthing, #addPAPLACModal #pointElevation, #addPAPLACModal #treeName, #addPAPLACModal #treeQty, #addPAPLACModal #ownerFirstName, #addPAPLACModal #ownerMiddleName, #addPAPLACModal #ownerLastName, #addPAPLACModal #ownerPhotoID, #addPAPLACModal #ownerFileID, #addPAPLACModal #treeComments').val('');
        $('#addPAPLACModal #goodType, #addPAPLACModal #treeMaturity, #addPAPLACModal #ownerPrimaryResidence').val('-1');
        $('#addPAPLACModal #goodPhoto, #addPAPLACModal #ownerPicture').fileinput('clear');
        $('#addPAPLACModal #ownershipType').prop("checked", false);
        $("#addPAPLACModal .hiden").hide();
        $('#addPAPLACModal #gpsID').focus();
    }
    return false;
});

$('#addPAPLACModal #ownerFirstName').on('blur', function () {
    let papID = $('#addPAPLACModal #ownerFirstName').val();
    let lacID = $('#addPAPLACModal #propertyLacID').val();
    LoadOwnerLacDetails(papID, lacID, "addPAPLACModal");
});

$("#addPAPLACModal #firstName").on("blur", function () {
    let papID = $('#addPAPLACModal #firstName').val();
    let lacID = $('#addPAPLACModal #lacID').val();
    LoadPAPLacDetails(papID, lacID, "addPAPLACModal")
});

function launchModalWizard(lacId, lacName) {
    resetModal('addPAPLACModal');
    getLocations("#addPAPLACModal #primaryResidence");
    getLocations("addPAPLACModal #ownerPrimaryResidence");
    getListDetails("addPAPLACModal #murCode", "-1","Wall");
    getListDetails("addPAPLACModal #toitCode", "-1", "Roof");
    getListDetails("addPAPLACModal #solCode", "-1", "Floor");
    getTreeMaturityForSelect("addPAPLACModal #treeMaturity", "-1");
    getFieldTypeForSelect("addPAPLACModal #fieldType", "-1");
    getStructureUsageForSelect("addPAPLACModal #structureUsage", "-1");
    getStructureForSelect("addPAPLACModal #structureCode", "-1");
    getAssetTypeForSelect("addPAPLACModal #goodType", "-1");
    getCultureType("#addPAPLACModal #cultureName");
    getEmployeeByRole("#addPAPLACModal #presurveyorCode", 'Surveyor');
    getPAP("#addPAPLACModal #firstName");
    getPAPOwner("#addPAPLACModal #ownerFirstName");
    getSurfaceUOM("addPAPLACModal #cultureSurfaceUOM", '-1');
    $("#addPAPLACModal #presurveyorDate").val(getCurrentDate());
    $("#addPAPLACModal #tableGoodsError, #addPAPLACModal #papDetailsError, #addPAPLACModal #papSurveyorError").hide();

    $("#addPAPLACModal #addPAPLACModalTitle").html("Pre-Survey on " + lacName);
    $("#addPAPLACModal #lacID").val(lacId);
    $("#addPAPLACModal").modal("show");
}

function removeRow(ctl) {
    $(ctl).parents("tr").remove();
}

function resetModal(modalName) {
    if (modalName == "addPAPLACModal") {
        let lacID = $("#addPAPLACModal #lacID").val();
        $("#addPAPLACModal #loader").hide();
        $('#addPAPLACModal').find("input,textarea").val('');
        $('#addPAPLACModal').find("select").val('-1');
        $('#addPAPLACModal #goodPhoto, #addPAPLACModal #PAPphoto, #addPAPLACModal #structurePhoto, #addPAPLACModal #treePhoto, #addPAPLACModal #ownerPicture').fileinput('clear');
        $('#addPAPLACModal #ownershipType').prop("checked", false);
        $("#addPAPLACModal #presurveyorDate").val(getCurrentDate());
        $("#addPAPLACModal .hiden").hide();
        $("#propertiesTable tbody").children().remove();
        $("#addPAPLACModal #lacID").val(lacID);
        $('#addPAPLACModal').wizard('begin');

        $('#addPAPLACModal #ownerPicture, #addPAPLACModal #PAPphoto').fileinput('enable');
        $('#addPAPLACModal #ownerPrimaryResidence, #addPAPLACModal #primaryResidence').prop('disabled', false);
        $('#addPAPLACModal #ownerPhotoID, #addPAPLACModal #pictureID').prop('disabled', false);
        $('#addPAPLACModal #ownerMiddleName, #addPAPLACModal #middleName').prop('disabled', false);
        $('#addPAPLACModal #ownerFileID, #addPAPLACModal #fileID').prop('disabled', false);
        $('#addPAPLACModal #ownerPersonId, #addPAPLACModal #PersonId').prop('disabled', false);
    }
    else if (modalName == "addPAPPropertiesModal") {
        let papID = $("#addPAPPropertiesModal #propertyPapID").val();
        let personID = $("#addPAPPropertiesModal #propertyPersonID").val();
        $("#addPAPPropertiesModal #loader").hide();
        $('#addPAPPropertiesModal').find("input,textarea").val('');
        $('#addPAPPropertiesModal').find("select").val('-1');
        $('#addPAPPropertiesModal #goodPhoto, #addPAPPropertiesModal #structurePhoto, #addPAPPropertiesModal #addPAPPropertiesModal, #addPAPPropertiesModal #ownerPicture').fileinput('clear');
        $('#addPAPPropertiesModal #ownershipType').prop("checked", false);
        $("#addPAPPropertiesModal #presurveyorDate").val(getCurrentDate());
        $("#addPAPPropertiesModal .hiden").hide();
        $("#addPAPPropertiesModal tbody").children().remove();
        $("#addPAPPropertiesModal #propertyPapID").val(papID);
        $("#addPAPPropertiesModal #propertyPersonID").val(personID);

        $('#addPAPPropertiesModal #ownerPrimaryResidence').prop('disabled', false);
        $('#addPAPPropertiesModal #ownerPicture').fileinput('enable');
        $('#addPAPPropertiesModal #ownerPhotoID').prop('disabled', false);
        $('#addPAPPropertiesModal #ownerMiddleName').prop('disabled', false);
        $('#addPAPPropertiesModal #ownerFileID, #addPAPPropertiesModal #fileID').prop('disabled', false);
        $('#addPAPPropertiesModal #ownerPersonId').prop('disabled', false);
    }
    else if (modalName == "surveyPAPModal") {
        let papID = $("#surveyPAPModal #papId").val();
        let paplacID = $("#surveyPAPModal #paplacId").val();
        let lacID = $("#surveyPAPModal #lacId").val();
        let fileID = $("#surveyPAPModal #fileID").val();

        $("#surveyPAPModal input[type='text']").val('');
        $("#surveyPAPModal input[type='radio'], #surveyPAPModal input[type='checkbox']").prop('checked', false);
        $("#surveyPAPModal select").val('-1');
        $("#surveyPAPModal #hhActivity, #surveyPAPModal #hhSkill, #surveyPAPModal #hhCultureWorker, #surveyPAPModal #hhCultureTool").val(null).trigger("change");
        $('#surveyPAPModal #hhPhoto, #surveyPAPModal #hhFile').fileinput('clear');

        $("#surveyPAPModal #hhTable tbody, #surveyPAPModal #hhAnimals tbody, #surveyPAPModal #hhEquipment tbody, #surveyPAPModal #hhTFMCompensed tbody, #surveyPAPModal #hhRevenue tbody, #surveyPAPModal #hhResidence tbody, #surveyPAPModal #hhCultures tbody, #surveyPAPModal #hhCultureSold tbody").children().remove();

        $("#surveyPAPModal #papId").val(papID);
        $("#surveyPAPModal #paplacId").val(paplacID);
        $("#surveyPAPModal #lacId").val(lacID);
        $("#surveyPAPModal #fileID").val(fileID);
        $("#surveyPAPModal #surveyPAPModalTitle").html("Socio-Economic Survey for PAP " + papID);
        $("#surveyPAPModal #surveyDate").val(getCurrentDate());

        $("#surveyPAPModal").wizard('begin');
        $("#surveyPAPModal #surveyRegion").focus();
    }
}

function convertTableToArray(tableName) {
    var dataToPost = [];
    
    $(tableName).find('tr').each(function (rowIndex, r) {
        var cols = [];
        $(this).find('td').each(function (colIndex, c) {
            cols.push(c.textContent.trim());
        });
        //console.log(cols);
        if (tableName == "#addPAPLACModal #propertiesTable") {
            if (cols.length > 0) {
                var obj = {
                    IPointName: null,
                    Picture: null,
                    PictureName: null,
                    UserType: null,
                    PropertyType: null,
                    PropertyTypeName: null,
                    PropertyName: null,
                    IPointLatitude: null,
                    IPointLongitude: null,
                    IPointElevation: null,
                    OwnerID: null,
                    OwnerFirstName: null,
                    OwnerMiddleName: null,
                    OwnerLastName: null,
                    OwnerPrimaryResidenceName: null,
                    OwnerPictureId: null,
                    OwnerFileNumber: null,
                    OwnerPictureName: null,
                    CultureOccupationType: null,
                    CultureOccupationName: null,
                    Surface: null,
                    StructureType: null,
                    StructureTypeName: null,
                    RoofType: null,
                    RoofTypeName: null,
                    WallType: null,
                    WallTypeName: null,
                    SoilType: null,
                    SoilTypeName: null,
                    StructureUsageID: null,
                    StructureUsageName: null,
                    StructureLength: null,
                    StructureWidth: null,
                    RoomQty: null,
                    TreeMaturity: null,
                    TreeMaturityName: null,
                    TreeQty: null,
                    Comments: null,
                    SurfaceUOM: null,
                    SurfaceUOMName: null
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.IPointName = cols[0];
                    obj.Picture = cols[1];
                    obj.PictureName = cols[2];
                    obj.UserType = cols[3];
                    obj.PropertyType = cols[4];
                    obj.PropertyTypeName = cols[5];
                    obj.PropertyName = cols[6];
                    obj.IPointLatitude = cols[7];
                    obj.IPointLongitude = cols[8];
                    obj.IPointElevation = cols[9];
                    obj.OwnerID = cols[38];
                    obj.OwnerFirstName = cols[10];
                    obj.OwnerMiddleName = cols[11];
                    obj.OwnerLastName = cols[12];
                    obj.OwnerPrimaryResidenceName = cols[14];
                    obj.OwnerPictureId = cols[15];
                    obj.OwnerFileNumber = cols[16];
                    obj.OwnerPictureName = cols[17];
                    obj.CultureOccupationType = cols[18];
                    obj.CultureOccupationName = cols[19];
                    obj.Surface = cols[20];
                    obj.StructureType = cols[21];
                    obj.StructureTypeName = cols[22];
                    obj.RoofType = cols[23];
                    obj.RoofTypeName = cols[24];
                    obj.WallType = cols[25];
                    obj.WallTypeName = cols[26];
                    obj.SoilType = cols[27];
                    obj.SoilTypeName = cols[28];
                    obj.StructureUsageID = cols[29];
                    obj.StructureUsageName = cols[30];
                    obj.StructureLength = cols[31];
                    obj.StructureWidth = cols[32];
                    obj.RoomQty = cols[33];
                    obj.TreeMaturity = cols[34];
                    obj.TreeMaturityName = cols[35];
                    obj.TreeQty = cols[36];
                    obj.Comments = cols[37];
                    obj.SurfaceUOM = cols[38];
                    obj.SurfaceUOMName = cols[39];
                    dataToPost.push(obj);
                }
            }
        }
        else if (tableName == "#collectLandModal #propertiesTable") {
            if (cols.length > 0) {
                var obj = {
                    IPointName: null,
                    IPointLatitude: null,
                    IPointLongitude: null,
                    Picture: null,
                    PictureName: null,
                    PropertyType: null,
                    PropertyTypeName: null,
                    PropertyName: null,
                    UserType: null,
                    OwnerID: null,
                    OwnerFirstName: null,
                    OwnerMiddleName: null,
                    OwnerLastName: null,
                    Surface: null,
                    SurfaceUOM: null,
                    SurfaceUOMName: null,
                    TPointName: null,
                    TPointLatitude: null,
                    TPointLongitude: null,
                    CultureStartDate: null,
                    CultureOccupationType: null,
                    CultureOccupationName: null,
                    CulturePoints: null,
                    StructureType: null,
                    StructureTypeName: null,
                    RoofType: null,
                    RoofTypeName: null,
                    WallType: null,
                    WallTypeName: null,
                    SoilType: null,
                    SoilTypeName: null,
                    StructureLength: null,
                    StructureWidth: null,
                    RoomQty: null,
                    TreeMaturity: null,
                    TreeMaturityName: null,
                    TreeQty: null,
                    Comments: null,
                    TopographerCode: null,
                    TopographDate: null,
                    TrimbleCode: null,
                    TrimbleFile: null,
                    PropertyId: null
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.PropertyId = cols[0];
                    obj.IPointName = cols[1];
                    obj.IPointLatitude = cols[2];
                    obj.IPointLongitude = cols[3];
                    obj.Picture = cols[4];
                    obj.PictureName = cols[5];
                    obj.PropertyType = cols[6];
                    obj.PropertyTypeName = cols[7];
                    obj.PropertyName = cols[8];
                    obj.UserType = cols[9];
                    obj.OwnerID = cols[10];
                    obj.OwnerFirstName = cols[11];
                    obj.OwnerMiddleName = cols[12];
                    obj.OwnerLastName = cols[13];
                    obj.Surface = cols[14];
                    obj.SurfaceUOM = cols[37];
                    obj.SurfaceUOMName = cols[38];
                    obj.TPointName = cols[15];
                    obj.TPointLatitude = cols[16];
                    obj.TPointLongitude = cols[17];
                    obj.CultureStartDate = cols[18];
                    obj.CultureOccupationType = cols[19];
                    obj.CultureOccupationName = cols[20];
                    obj.CulturePoints = cols[21];
                    obj.StructureType = cols[22];
                    obj.StructureTypeName = cols[23];
                    obj.RoofType = cols[24];
                    obj.RoofTypeName = cols[25];
                    obj.WallType = cols[26];
                    obj.WallTypeName = cols[27];
                    obj.SoilType = cols[28];
                    obj.SoilTypeName = cols[29];
                    obj.StructureLength = cols[30];
                    obj.StructureWidth = cols[31];
                    obj.RoomQty = cols[32];
                    obj.TreeMaturity = cols[33];
                    obj.TreeMaturityName = cols[34];
                    obj.TreeQty = cols[35];
                    obj.Comments = cols[36];
                    obj.TopographerCode = cols[39];
                    obj.TopographDate = cols[40];
                    obj.TrimbleCode = cols[41];
                    obj.TrimbleFile = cols[42];
                    dataToPost.push(obj);
                }
            }
        }
        else if (tableName == "#collectLandModal #tablePAPProperties") {
            if (cols.length > 0) {
                var obj = {
                    PropertyId: null,
                    IPointName: null,
                    PropertyType: null,
                    PropertyTypeName: null,
                    PropertyName: null,
                    UserType: null,
                    OwnerID: null
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.PropertyId = cols[0];
                    obj.IPointName = cols[1];
                    obj.PropertyType = cols[3];
                    obj.PropertyTypeName = cols[4];
                    obj.PropertyName = cols[5];
                    obj.UserType = cols[6];
                    obj.OwnerID = cols[7];
                    dataToPost.push(obj);
                }
            }
        }
    });
    return dataToPost;
}

function notInTables(searchString) {
    var found = false;
    $('#propertiesTable tr').each(function (row, tr) {
        var gpsID = $(tr).children().eq(0).text();
        var photoID = $(tr).children().eq(1).text();
        if (gpsID == searchString || photoID == searchString) {
            found = true;
            return false;
        }
    });
    return found;
}

function insertPAPLAC() {
    let url = $(location).attr('pathname');
    let lacID = $('#addPAPLACModal #lacID').val();
    let surveyor = $('#addPAPLACModal #presurveyorCode').val();
    let surveyorDate = $('#addPAPLACModal #presurveyorDate').val();
    let surveyGPS = $('#addPAPLACModal #presurveyorGPS').val();
    let surveyPhoto = $('#addPAPLACModal #presurveyorPhoto').val();
    let currentDate = new Date();
    var surveyorDate2 = new Date(surveyorDate);
    var propertiesArray = convertTableToArray("#addPAPLACModal #propertiesTable");

    var errors = [];

    //console.log("Lac ID " + lacID + " - Surveyor Code " + surveyor + " - Survey Date " + surveyorDate + " - GPS ID " + surveyGPS);

    if (isNaN(lacID) || lacID == null || lacID == "")
        errors.push("Please refresh the page, because this Lac ID doesn't exist!");
    if (!surveyor || surveyor === "" || surveyor.length === 0 || !surveyor.trim())
        errors.push("Surveyor Code can't be empty")
    if (!Date.parse(surveyorDate))
        errors.push("Survey Date is not valid.");
    if (surveyorDate2 > currentDate)
        errors.push("Survey Date can't be in the future");
    if (!surveyGPS || surveyGPS == "" || surveyGPS.length === 0 || !surveyGPS.trim() || surveyGPS == null)
        errors.push("Please identify the GPS equipment");
    if (!surveyPhoto || surveyPhoto == "" || surveyPhoto.length === 0 || !surveyPhoto.trim() || surveyPhoto == null)
        errors.push("Please identify the Photo equipment ID");
    if (propertiesArray.length < 1)
        errors.push("Can't save a PAP without properties!. Please add properties and then try again");

    if (errors.length >= 1) {
        var errorMessage = "<ul>";
        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addPAPLACModal #papSurveyorError").html(errorMessage).show(250);
        return false;
    }
    else { 
        $("#addPAPLACModal #loader").show();
        let pap = $('#addPAPLACModal #PersonId').val() == "" ? {
            FirstName : $('#addPAPLACModal #firstName').val(),
            LastName : $('#addPAPLACModal #lastName').val(),
            MiddleName : $('#addPAPLACModal #middleName').val(),
            PresurveyDate : surveyorDate,
            PresurveyorCode: surveyor,
            PresurveyorGPS: surveyGPS,
            PresurveyorCamera: surveyPhoto,
            FileNumber : $('#addPAPLACModal #fileID').val(),
            LACId:lacID, 
            ResidenceName: $('#addPAPLACModal #primaryResidence').val(),
            PhotoID: $('#addPAPLACModal #pictureID').val(),
            Picture: $('#addPAPLACModal #PAPphoto').val()
        } : {
                FirstName: $('#addPAPLACModal #firstName').val(),
                LastName: $('#addPAPLACModal #lastName').val(),
                MiddleName: $('#addPAPLACModal #middleName').val(),
                PresurveyDate: surveyorDate,
                PresurveyorCode: surveyor,
                PresurveyorGPS: surveyGPS,
                FileNumber: $('#addPAPLACModal #fileID').val(),
                LACId: lacID,
                ResidenceName: $('#addPAPLACModal #primaryResidence').val(),
                PhotoID: $('#addPAPLACModal #pictureID').val(),
                Picture: $('#addPAPLACModal #PAPphoto').val(),
                PersonId: $('#addPAPLACModal #PersonId').val()
            };

        let papPhoto = $("#addPAPLACModal #PAPphoto")[0];
        let papPhotoFile = papPhoto.files;

        if (papPhoto.files.length > 0)
            formData.append(papPhotoFile[0].name, papPhotoFile[0]);

        formData.append("PAPLAC", JSON.stringify(pap));
        formData.append("Properties", JSON.stringify(propertiesArray));

        $.ajax({
            type: "POST",
            url: "/Community/AddPAPLAC/",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response[0] == "Success") {
                    if (url != "/LandManagement/LacList") {
                        LoadPapLac(lacID);
                    }
                    resetModal("addPAPLACModal");
                    $.notify(response[1], "success");
                } else if (response[0] == "Error") {
                    $("#addPAPLACModal #loader").hide();
                    let errorMessage = "<ul>";

                    $.each(response.slice(1), function (index, value) {
                        errorMessage += "<li>" + value + "</li>";
                    });
                    errorMessage += "</ul>";
                    $("#addPAPLACModal #papSurveyorError").html(errorMessage).show(250);
                } else {
                    $("#addPAPLACModal #loader").hide();
                    $.notify("An error occured while inserting PAP with property", "error");
                    console.log(response[1]);
                }
                console.log(response);
            }
        });
        return true;
    }
}

//#endregion

//#region PAP AND PAP DETAILS
$("#linkPAPLACModal").on("click", "#savePapLac", function () {
    let papID = $("#linkPAPLACModal #linkPAPID").val();
    let lacID = $("#linkPAPLACModal #lacID").val();
    let lacName = $("#linkPAPLACModal #lacID option:selected").text();
    let paymentPreferenceID = $("#linkPAPLACModal #paymentPreference").val();
    let paymentPreferenceName = $("#linkPAPLACModal #paymentPreference").val();
    let submissionDate = $("#linkPAPLACModal #submissionDate").val();
    let presurveyDate = $("#linkPAPLACModal #presurveyDate").val();
    let presurveyor = $("#linkPAPLACModal #linkpresurveyorCode").val();
    let surveyDate = $("#linkPAPLACModal #surveyDate").val();
    let surveyor = $("#linkPAPLACModal #linksurveyorCode").val();
    let fileNumber = $("#linkPAPLACModal #papfileId").val();
    let comment = $("#linkPAPLACModal #papLacComment").val();
    let currentDate = new Date();
    let presurveyDate2 = new Date(presurveyDate);
    let surveyDate2 = new Date(surveyDate);

    var errors = [];

    if (!Date.parse(surveyDate))
        errors.push("Survey Date is not valid.");
    if (surveyDate2 > currentDate)
        errors.push("Survey Date can't be in the future");
    if (!Date.parse(presurveyDate))
        errors.push("Pre-Survey Date is not valid.");
    if (presurveyDate2 > currentDate)
        errors.push("Pre-Survey Date can't be in the future");
    if (Date.parse(surveyDate) && surveyDate2 < presurveyDate2)
        errors.push("Survey date can't be less thant pre-survey date");
    if (lacID == "-1" || isNaN(lacID)) 
        errors.push("Invalid LAC Number");
    if (fileNumber === "" || fileNumber === null || isNaN(fileNumber) || !fileNumber || fileNumber.length === 0)
        errors.push("Invalid File Number");

    if (errors.length >= 1) {
        var errorMessage = "<ul>";
        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#linkPAPLACModal #linkPAPLACModalError").html(errorMessage).show(250);
        return false;
    }
    else {
        $("#linkPAPLACModal #loader").show();
        let papLac = {
            LACId: lacID,
            LACName: lacName,
            PAPId: papID,
            Comments: comment,
            PaymentPreference: paymentPreferenceID,
            FormSubmissionDate: submissionDate,
            PresurveyDate: presurveyDate,
            PresurveyorCode: presurveyor,
            SurveyDate: surveyDate,
            SurveyorCode: surveyor,
            FileNumber: fileNumber
        }

        //console.log(papLac);

        $.ajax({
            type: "POST",
            url: "/Community/LinkPAPLAC",
            data: JSON.stringify(papLac),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response[0] == "Error") {
                    var errorMessage = "<ul>";
                    $.each(response, function (index, value) {
                        errorMessage += "<li>" + value + "</li>";
                    });
                    errorMessage += "</ul>";
                    $("#linkPAPLACModal #loader").hide();
                    $("#linkPAPLACModal #linkPAPLACModalError").html(errorMessage).show(250);
                } else if (response[0] == "Exception") {
                    $("#linkPAPLACModal #linkPAPLACModalError").html(response[1]).show(250);
                }
                else {
                    let url = $(location).attr('pathname');
                    if (url != "/Community/Paps")
                        LoadPapLacDetails(papID);
                    else if (url == "/Community/PapLAC")
                        LoadLACPAPList();
                    else
                        LoadPAPList();
                    $("#linkPAPLACModal #loader").hide();
                    $.notify(response[1], "success");
                    $("#linkPAPLACModal").modal("hide");
                }
            },
            error: function (error) {
                console.log(error);
                $.notify("Something went wrong while updating the information", "error");
            }
        });
        return false;
    }
});

$("#linkPAPLACModal :input, #linkPAPLACModal #papLacComment").on("focus", function () {$("#linkPAPLACModal #linkPAPLACModalError").hide();});
$("#linkPAPLACModal :input, #linkPAPLACModal #papLacComment").on("change", function () {$("#linkPAPLACModal #linkPAPLACModalError").hide();});

$("#addPAPModal :input, #addPAPModal #papLacComment").on("focus", function () { $("#addPAPModal #papDetailsError").hide(); });
$("#addPAPModal :input, #addPAPModal #papLacComment").on("change", function () { $("#addPAPModal #papDetailsError").hide(); });

$("#addPAPModal #addpaplacDetails").on("change", function () {
    $("#addPAPModal #presurveyorCode, #papLacComment, #fileId, #surveyorCode").val('');
    getLACForSelect("addPAPModal #lacID", '-1');
    getPaymentPreferencesForSelect("addPAPModal #paymentPreference", '-1');
    $("#addPAPModal #surveyDate, #addPAPModal #presurveyDate, #addPAPModal #submissionDate").val(getCurrentDate());
    if ($(this).prop('checked'))
        $("#addPAPModal #papLacDetails").show();
    else 
        $("#addPAPModal #papLacDetails").hide();
});

$("#addPAPModal").on("click", "#saveNewPap", function () {
    AddPAP(false);
});

$("#addPAPModal").on("click", "#updatePap", function () {
    AddPAP(true);
});

function AddPAP(isUpdate) {
    let lacID = $("#addPAPModal #lacID").val();
    let lacName = $("#addPAPModal #lacID option:selected").text();
    let paymentPreferenceID = $("#addPAPModal #paymentPreference").val();
    let paymentPreferenceName = $("#addPAPModal #paymentPreference").val();
    let submissionDate = $("#addPAPModal #submissionDate").val();
    let presurveyDate = $("#addPAPModal #presurveyDate").val();
    let presurveyor = $("#addPAPModal #presurveyorCode").val();
    let surveyDate = $("#addPAPModal #surveyDate").val();
    let surveyor = $("#addPAPModal #surveyorCode").val();
    let fileNumber = $("#addPAPModal #fileId").val();
    let comment = $("#addPAPModal #papLacComment").val();
    let currentDate = new Date();
    let presurveyDate2 = new Date(presurveyDate);
    let surveyDate2 = new Date(surveyDate);
    let firstName = $('#addPAPModal #firstName').val();
    let lastName = $('#addPAPModal #lastName').val();
    let pictureID = $('#addPAPModal #pictureID').val();
    let email = $('#addPAPModal #email').val();
    let primaryResidenceName = $('#addPAPModal #primaryResidence').val();
    let dob = $('#addPAPModal #dob').val();
    let dob2 = new Date(dob);
    let gender = $('#addPAPModal input[type=radio]:checked').val();

    var errors = [];
    var expr = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/;
    if (firstName == null || firstName == "")
        errors.push("Please add the First Name");
    if (lastName == null || lastName == "")
        errors.push("Please fill the Last Name of the PAP");
    if (isUpdate == false && ($('#addPAPModal #PAPphoto').val() == null || $('#addPAPModal #PAPphoto').val() == ""))
        errors.push("Please upload the PAP picture");
    if (primaryResidenceName == null || primaryResidenceName == "")
        errors.push("You did not specify the Primary Residence");
    if (pictureID == null || pictureID == "")
        errors.push("You did not put Picture ID");
    if (dob2 > currentDate)
        errors.push("Date of birth can't be in the future");
    if (gender == "" || gender == null)
        errors.push("Please specify PAP's gender");
    if (email != "" && email != null && !expr.test(email))
        errors.push("You entered an invalid email address");

    if ($("#addPAPModal #addpaplacDetails").prop('checked')) {
        if (!Date.parse(surveyDate))
            errors.push("Survey Date is not valid.");
        if (surveyDate2 > currentDate)
            errors.push("Survey Date can't be in the future");
        if (!Date.parse(presurveyDate))
            errors.push("Pre-Survey Date is not valid.");
        if (presurveyDate2 > currentDate)
            errors.push("Pre-Survey Date can't be in the future");
        if (Date.parse(surveyDate) && surveyDate2 < presurveyDate2)
            errors.push("Survey date can't be less thant pre-survey date");
        if (lacID == "-1" || isNaN(lacID))
            errors.push("Invalid LAC Number");
        if (fileNumber === "" || fileNumber === null || isNaN(fileNumber) || !fileNumber || fileNumber.length === 0)
            errors.push("Invalid or Empty File Number");
    }

    if (errors.length >= 1) {
        var errorMessage = "<ul>";
        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addPAPModal #papDetailsError").html(errorMessage).show(250);
        return false;
    }
    else {
        $("#addPAPModal #loader").show();
        let papID = $("#addPAPModal #papID").val();
        let pap = isUpdate == false ? {
            FirstName: firstName,
            LastName: lastName,
            MiddleName: $('#addPAPModal #middleName').val(),
            Mobile: $('#addPAPModal #cellPhone').val(),
            Email: email,
            Gender: gender,
            DateOfBirth: dob,
            PlaceOfBirth: $('#addPAPModal #pob').val(),
            FatherName: $('#addPAPModal #papfather').val(),
            MotherName: $('#addPAPModal #papMother').val(),
            VulnerabilityTypeId: $('#addPAPModal #vulnerabilityType').val(),
            VulnerabilityTypeName: $('#addPAPModal #vulnerabilityType option:selected').text(),
            VulnerabilityDetail: $('#addPAPModal #vulnerabilityDetails').val(),
            PhotoID: $('#addPAPModal #pictureID').val(),
            SpouseName: $('#addPAPModal #papSpouse').val(),
            IdCard: $('#addPAPModal #cardNumber').val(),
            LACId: lacID,
            LACName: lacName,
            ResidenceName: primaryResidenceName,
            Picture: $('#addPAPModal #PAPphoto').val(),
            Comments: comment,
            PaymentPreference: paymentPreferenceID,
            FormSubmissionDate: submissionDate,
            PresurveyDate: presurveyDate,
            PresurveyorCode: presurveyor,
            SurveyDate: surveyDate,
            SurveyorCode: surveyor,
            FileNumber: fileNumber
        } : {
                PersonId: $("#addPAPModal #personID").val(),
                PAPId: papID,
                FirstName: firstName,
                LastName: lastName,
                MiddleName: $('#addPAPModal #middleName').val(),
                Mobile: $('#addPAPModal #cellPhone').val(),
                Email: email,
                Gender: gender,
                DateOfBirth: dob,
                PlaceOfBirth: $('#addPAPModal #pob').val(),
                FatherName: $('#addPAPModal #papfather').val(),
                MotherName: $('#addPAPModal #papMother').val(),
                VulnerabilityTypeId: $('#addPAPModal #vulnerabilityType').val(),
                VulnerabilityTypeName: $('#addPAPModal #vulnerabilityType option:selected').text(),
                VulnerabilityDetail: $('#addPAPModal #vulnerabilityDetails').val(),
                PhotoID: $('#addPAPModal #pictureID').val(),
                SpouseName: $('#addPAPModal #papSpouse').val(),
                IdCard: $('#addPAPModal #cardNumber').val(),
                ResidenceName: primaryResidenceName,
                Picture: $('#addPAPModal #PAPphoto').val()
            };
        console.log(pap);

        formData = new FormData();

        let papPhoto = $("#addPAPModal #PAPphoto")[0];
        let papPhotoFile = papPhoto.files;

        if (papPhoto.files.length > 0)
            formData.append(papPhotoFile[0].name, papPhotoFile[0]);

        let PapLac = JSON.stringify(pap);
        formData.append("PAPLAC", PapLac);
        formData.append("addPapToLac", $("#addPAPModal #addpaplacDetails").prop('checked'));
        formData.append("isUpdate", isUpdate);

        $.ajax({
            type: "POST",
            url: "/Community/AddPAP",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response[0] == "Error") {
                    var errorMessage = "<ul>";
                    $.each(response, function (index, value) {
                        errorMessage += "<li>" + value + "</li>";
                    });
                    errorMessage += "</ul>";
                    $("#addPAPModal #loader").hide();
                    $("#addPAPModal #papDetailsError").html(errorMessage).show(250);
                    return false;
                } else if (response[0] == "Exception") {
                    $("#addPAPModal #loader").hide();
                    $.notify(response[1], "error");
                    console.log(response[2]);
                }
                else {
                    $("#addPAPModal #loader").hide();
                    let url = $(location).attr('pathname');
                    if (url != "/Community/Paps")
                        location.reload();
                    else
                        LoadPAPList();

                    $("#addPAPModal").modal("hide");
                    $.notify(response[1], "success");
                }
            },
            error: function (error) {
                console.log(error);
                $("#addPAPModal #loader").hide();
                $.notify("Something went wrong! Please try again later", "error");
            }
        });
        return false;
    }
}
//#endregion

//#region PAP PROPERTIES
$("#addPAPPropertiesModal #presurveyorDate, #addPAPPropertiesModal #propertyPresurveyorCode, #addPAPPropertiesModal #presurveyorGPS, #addPAPPropertiesModal #propertyLacID").on("focus", function () { $("#addPAPPropertiesModal #papSurveyorError").hide(); });
$("#addPAPPropertiesModal #presurveyorDate, #addPAPPropertiesModal #propertyPresurveyorCode, #addPAPPropertiesModal #presurveyorGPS, #addPAPPropertiesModal #propertyLacID").on("change", function () { $("#addPAPPropertiesModal #papSurveyorError").hide(); });

$("#addPAPPropertiesModal .reset").on("focus", function () { $("#addPAPPropertiesModal #tableGoodsError").hide(); });
$("#addPAPPropertiesModal .reset").on("change", function () { $("#addPAPPropertiesModal #tableGoodsError").hide(); });

$("#addPAPPropertiesModal #goodType").on("change", function () {
    let assetType = $("#addPAPPropertiesModal #goodType option:selected").text();
    $("#addPAPPropertiesModal #cultureDetails, #addPAPPropertiesModal #structureDetails, #addPAPPropertiesModal #treeDetails").hide();
    if (assetType == "Culture")
        $("#addPAPPropertiesModal #cultureDetails").show();
    if (assetType == "Structure")
        $("#addPAPPropertiesModal #structureDetails").show();
    if (assetType == "Tree")
        $("#addPAPPropertiesModal #treeDetails").show();
});

$("#addPAPPropertiesModal #ownershipType").on("change", function () {
    if ($(this).prop('checked'))
        $("#addPAPPropertiesModal #ownerDetails").show();
    else
        $("#addPAPPropertiesModal #ownerDetails").hide();
});

$('#addPAPPropertiesModal').on("click", "#addGoods", function (e) {
    e.preventDefault();
    var errors = [];
    var user = $('#addPAPPropertiesModal #ownershipType').prop('checked') ? 'U' : 'PU';

    if ($('#addPAPPropertiesModal #gpsID').val() == null || $('#addPAPPropertiesModal #gpsID').val() == "")
        errors.push("Please specify the GPS point");
    if ($('#addPAPPropertiesModal #pointEasting').val() == null || $('#addPAPPropertiesModal #pointEasting').val() == "")
        errors.push("Please specify Point Easting");
    if ($('#addPAPPropertiesModal #pointNorthing').val() == null || $('#addPAPPropertiesModal #pointNorthing').val() == "")
        errors.push("Please specify Point Northing");
    if ($('#addPAPPropertiesModal #goodPictureID').val() == null || $('#addPAPPropertiesModal #goodPictureID').val() == "")
        errors.push("Please add the picture ID");
    if (isNaN($('#addPAPPropertiesModal #goodType').val()) || $('#addPAPPropertiesModal #goodType').val() == "")
        errors.push("Please specify a valid property type");
    if (user == 'U' && ($('#addPAPPropertiesModal #ownerFirstName').val() == null || $('#addPAPPropertiesModal #ownerFirstName').val() == ""))
        errors.push("Please specify Owner First Name");
    if (user == 'U' && ($('#addPAPPropertiesModal #ownerLastName').val() == null || $('#addPAPPropertiesModal #ownerLastName').val() == ""))
        errors.push("Please specify Owner Last Name");
    if (user == 'U' && ($('#addPAPPropertiesModal #ownerFileID').val() == null || $('#addPAPPropertiesModal #ownerFileID').val() == ""))
        errors.push("Please specify Owner file ID");

    if ($('#addPAPPropertiesModal #fieldType').val() == null || $('#addPAPPropertiesModal #fieldType').val() == "")
        errors.push("Please choose the Field Type");
    if (isNaN($('#addPAPPropertiesModal #cultureSurface').val()) || $('#addPAPPropertiesModal #cultureSurface').val() == "")
        errors.push("Please specify the surface");
    if ($('#addPAPPropertiesModal #goodPhoto').val() == null || $('#addPAPPropertiesModal #goodPhoto').val() == "")
        errors.push("You did not put Picture ID");

    let ownerPhoto = $("#addPAPPropertiesModal #ownerPicture")[0];
    let ownerPhotoFile = ownerPhoto.files;
    let goodPhoto = $("#addPAPPropertiesModal #goodPhoto")[0];
    let goodPhotoFile = goodPhoto.files;

    if (goodPhoto.files.length > 0)
        formData.append(goodPhotoFile[0].name, goodPhotoFile[0]);

    if (ownerPhoto.files.length > 0)
        formData.append(ownerPhotoFile[0].name, ownerPhotoFile[0]);

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addPAPPropertiesModal #tableGoodsError").html(errorMessage).show(250);
    }
    else {
        var user = $('#addPAPPropertiesModal #ownershipType').prop('checked') ? 'U' : 'PU';
        var tr = "";
        tr += '<tr>' +
            '<td>' + $('#addPAPPropertiesModal #gpsID').val() + '</td>' +
            '<td>' + $('#addPAPPropertiesModal #goodPictureID').val() + '</td>' +
            '<td style="display:none"> ' + $('#addPAPPropertiesModal #goodPhoto').val() + '</td>' +
            '<td>' + user + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #goodType').val() + '</td>' +
            '<td>' + $('#addPAPPropertiesModal #goodType option:selected').text() + '</td>' +
            '<td>' + $('#addPAPPropertiesModal #cultureName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #pointEasting').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #pointNorthing').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #pointElevation').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerFirstName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerMiddleName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerLastName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPrimaryResidence').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPrimaryResidence').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPhotoID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerFileID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPicture').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #fieldType').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #fieldType option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #cultureSurface').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #goodComments').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPersonId').val() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeGoods" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';
        $("#propertiesTable tbody").append(tr);
        $('#addPAPPropertiesModal #gpsID, #addPAPPropertiesModal #goodPictureID, #addPAPPropertiesModal #pointEasting, #addPAPPropertiesModal #pointNorthing, #addPAPPropertiesModal #pointElevation, #addPAPPropertiesModal #goodPhoto, #addPAPPropertiesModal #cultureName, #addPAPPropertiesModal #cultureSurface, #addPAPPropertiesModal #ownerFirstName, #addPAPPropertiesModal #ownerMiddleName, #addPAPPropertiesModal #ownerLastName, #addPAPPropertiesModal #ownerPhotoID, #addPAPPropertiesModal #ownerFileID, #addPAPPropertiesModal #goodComments, #addPAPPropertiesModal #ownerPrimaryResidence, #addPAPPropertiesModal #ownerPersonId').val('');
        $("#addPAPPropertiesModal #goodType, #addPAPPropertiesModal #fieldType").val("-1");
        $('#addPAPPropertiesModal #goodPhoto, #addPAPPropertiesModal #ownerPicture').fileinput('clear');
        $('#addPAPPropertiesModal #ownershipType').prop("checked", false);
        $("#addPAPPropertiesModal .hiden").hide();
        $('#addPAPPropertiesModal #gpsID').focus();
    }
    return false;
});

$('#addPAPPropertiesModal').on("click", "#addStructure", function (e) {
    e.preventDefault();
    var errors = [];
    var user = $('#addPAPPropertiesModal #ownershipType').prop('checked') ? 'U' : 'PU';

    if ($('#addPAPPropertiesModal #gpsID').val() == null || $('#addPAPPropertiesModal #gpsID').val() == "")
        errors.push("Please specify the GPS point");
    if ($('#addPAPPropertiesModal #pointEasting').val() == null || $('#addPAPPropertiesModal #pointEasting').val() == "")
        errors.push("Please specify Point Easting");
    if ($('#addPAPPropertiesModal #pointNorthing').val() == null || $('#addPAPPropertiesModal #pointNorthing').val() == "")
        errors.push("Please specify Point Northing");
    if ($('#addPAPPropertiesModal #goodPictureID').val() == null || $('#addPAPPropertiesModal #goodPictureID').val() == "")
        errors.push("Please add the picture ID");
    if (isNaN($('#addPAPPropertiesModal #goodType').val()) || $('#addPAPPropertiesModal #goodType').val() == "")
        errors.push("Please specify a valid property type");
    if (user == 'U' && ($('#addPAPPropertiesModal #ownerFirstName').val() == null || $('#addPAPPropertiesModal #ownerFirstName').val() == ""))
        errors.push("Please specify Owner First Name");
    if (user == 'U' && ($('#addPAPPropertiesModal #ownerLastName').val() == null || $('#addPAPPropertiesModal #ownerLastName').val() == ""))
        errors.push("Please specify Owner Last Name");
    if (user == 'U' && ($('#addPAPPropertiesModal #ownerFileID').val() == null || $('#addPAPPropertiesModal #ownerFileID').val() == ""))
        errors.push("Please specify Owner file ID");

    if ($("#addPAPPropertiesModal #structureCode").val() == "" || $("#addPAPPropertiesModal #structureCode").val() == null || isNaN($("#addPAPPropertiesModal #structureCode").val()))
        errors.push("Structure Code can not be empty");
    if ($("#addPAPPropertiesModal #toitCode").val() == "" || $("#addPAPPropertiesModal #toitCode").val() == null || isNaN($("#addPAPPropertiesModal #toitCode").val()))
        errors.push("Roof Code can not be empty");
    if ($("#addPAPPropertiesModal #murCode").val() == "" || $("#addPAPPropertiesModal #murCode").val() == null || isNaN($("#addPAPPropertiesModal #murCode").val()))
        errors.push("Wall Code can not be empty");
    if ($("#addPAPPropertiesModal #solCode").val() == "" || $("#addPAPPropertiesModal #solCode").val() == null || isNaN($("#addPAPPropertiesModal #solCode").val()))
        errors.push("Floor Code can not be empty");
    if ($("#addPAPPropertiesModal #structurePhoto").val() == "" || $("#addPAPPropertiesModal #structurePhoto").val() == null)
        errors.push("Please upload the picture of this structure");
    if ($("#addPAPPropertiesModal #structureUsage").val() == "" || $("#addPAPPropertiesModal #structureUsage").val() == null || isNaN($("#addPAPPropertiesModal #structureUsage").val()))
        errors.push("Choose the structure Usage");
    if ($("#addPAPPropertiesModal #structureLength").val() == "" || $("#addPAPPropertiesModal #structureLength").val() == null)
        errors.push("Structure surface can't be empty");
    if ($("#addPAPPropertiesModal #structureWidth").val() == "" || $("#addPAPPropertiesModal #structureWidth").val() == null)
        errors.push("Structure surface can't be empty");
    if ($("#addPAPPropertiesModal #structurePiece").val() == "" || $("#addPAPPropertiesModal #structurePiece").val() == null || isNaN($("#addPAPPropertiesModal #structurePiece").val()))
        errors.push("Specify the number of rooms");

    let ownerPhoto = $("#addPAPPropertiesModal #ownerPicture")[0];
    let ownerPhotoFile = ownerPhoto.files;
    let goodPhoto = $("#addPAPPropertiesModal #structurePhoto")[0];
    let goodPhotoFile = goodPhoto.files;

    if (goodPhoto.files.length > 0)
        formData.append(goodPhotoFile[0].name, goodPhotoFile[0]);

    if (ownerPhoto.files.length > 0)
        formData.append(ownerPhotoFile[0].name, ownerPhotoFile[0]);

    if (errors.length > 0) {
        var errorMessage = "<ul>";
        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addPAPPropertiesModal #tableGoodsError").html(errorMessage).show(250);
    }
    else {
        var tr = "";
        tr += '<tr>' +
            '<td>' + $('#addPAPPropertiesModal #gpsID').val() + '</td>' +
            '<td>' + $('#addPAPPropertiesModal #goodPictureID').val() + '</td>' +
            '<td style="display:none"> ' + $('#addPAPPropertiesModal #structurePhoto').val() + '</td>' +
            '<td>' + user + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #goodType').val() + '</td>' +
            '<td>' + $('#addPAPPropertiesModal #goodType option:selected').text() + '</td>' +
            '<td>' + $('#addPAPPropertiesModal #structureCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #pointEasting').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #pointNorthing').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #pointElevation').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerFirstName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerMiddleName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerLastName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPrimaryResidence').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPrimaryResidence').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPhotoID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerFileID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPicture').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #structureCode').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #structureCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #toitCode').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #toitCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #murCode').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #murCode option:selected').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #solCode').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #solCode option:selected').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #structureUsage').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #structureUsage option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #structureLength').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #structureWidth').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #structurePiece').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #structureComments').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPersonId').val() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeGoods" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';

        $("#propertiesTable tbody").append(tr);
        $('#addPAPPropertiesModal #gpsID, #addPAPPropertiesModal #goodPictureID, #addPAPPropertiesModal #pointEasting, #addPAPPropertiesModal #pointNorthing, #addPAPPropertiesModal #pointElevation, #addPAPPropertiesModal #goodPhoto, #addPAPPropertiesModal #ownerFirstName, #addPAPPropertiesModal #ownerMiddleName, #addPAPPropertiesModal #ownerLastName, #addPAPPropertiesModal #ownerPhotoID, #addPAPPropertiesModal #ownerFileID, #addPAPPropertiesModal #structureLength, #addPAPPropertiesModal #structureWidth, #addPAPPropertiesModal #structurePiece, #addPAPPropertiesModal #structureComments, #addPAPPropertiesModal #ownerPrimaryResidence, #addPAPPropertiesModal #ownerPersonId').val('');
        $("#addPAPPropertiesModal #goodType, #addPAPPropertiesModal #structureCode, #addPAPPropertiesModal #toitCode, #addPAPPropertiesModal #murCode, #addPAPPropertiesModal #solCode, #addPAPPropertiesModal #structureUsage").val("-1");
        $('#addPAPPropertiesModal #structurePhoto, #addPAPPropertiesModal #ownerPicture').fileinput('clear');
        $('#addPAPPropertiesModal #ownershipType').prop("checked", false);
        $("#addPAPPropertiesModal .hiden").hide();
        $('#addPAPPropertiesModal #gpsID').focus();
    }
    return false;
});

$('#addPAPPropertiesModal').on("click", "#addTree", function (e) {
    e.preventDefault();
    var errors = [];
    var user = $('#addPAPPropertiesModal #ownershipType').prop('checked') ? 'U' : 'PU';

    if ($('#addPAPPropertiesModal #gpsID').val() == null || $('#addPAPPropertiesModal #gpsID').val() == "")
        errors.push("Please specify the GPS point");
    if ($('#addPAPPropertiesModal #pointEasting').val() == null || $('#addPAPPropertiesModal #pointEasting').val() == "")
        errors.push("Please specify Point Easting");
    if ($('#addPAPPropertiesModal #pointNorthing').val() == null || $('#addPAPPropertiesModal #pointNorthing').val() == "")
        errors.push("Please specify Point Northing");
    if ($('#addPAPPropertiesModal #goodPictureID').val() == null || $('#addPAPPropertiesModal #goodPictureID').val() == "")
        errors.push("Please add the picture ID");
    if (isNaN($('#addPAPPropertiesModal #goodType').val()) || $('#addPAPPropertiesModal #goodType').val() == "")
        errors.push("Please specify a valid property type");
    if (user == 'U' && ($('#addPAPPropertiesModal #ownerFirstName').val() == null || $('#addPAPPropertiesModal #ownerFirstName').val() == ""))
        errors.push("Please specify Owner First Name");
    if (user == 'U' && ($('#addPAPPropertiesModal #ownerLastName').val() == null || $('#addPAPPropertiesModal #ownerLastName').val() == ""))
        errors.push("Please specify Owner Last Name");
    if (user == 'U' && ($('#addPAPPropertiesModal #ownerFileID').val() == null || $('#addPAPPropertiesModal #ownerFileID').val() == ""))
        errors.push("Please specify Owner file ID");

    if ($('#addPAPPropertiesModal #treeName').val() == null || $('#addPAPPropertiesModal #treeName').val() == "")
        errors.push("Please specify the tree name");
    if (isNaN($('#addPAPPropertiesModal #treeQty').val()) || $('#addPAPPropertiesModal #treeQty').val() == "")
        errors.push("Please specify a valid count number of trees");
    if ($('#addPAPPropertiesModal #treePhoto').val() == null || $('#addPAPPropertiesModal #treePhoto').val() == "")
        errors.push("You did not put Picture");

    let ownerPhoto = $("#addPAPPropertiesModal #ownerPicture")[0];
    let ownerPhotoFile = ownerPhoto.files;
    let goodPhoto = $("#addPAPPropertiesModal #treePhoto")[0];
    let goodPhotoFile = goodPhoto.files;

    if (goodPhoto.files.length > 0)
        formData.append(goodPhotoFile[0].name, goodPhotoFile[0]);

    if (ownerPhoto.files.length > 0)
        formData.append(ownerPhotoFile[0].name, ownerPhotoFile[0]);

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addPAPPropertiesModal #tableGoodsError").html(errorMessage).show(250);
    }
    else {
        var tr = "";
        tr += '<tr>' +
            '<td>' + $('#addPAPPropertiesModal #gpsID').val() + '</td>' +
            '<td>' + $('#addPAPPropertiesModal #goodPictureID').val() + '</td>' +
            '<td style="display:none"> ' + $('#addPAPPropertiesModal #treePhoto').val() + '</td>' +
            '<td>' + user + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #goodType').val() + '</td>' +
            '<td>' + $('#addPAPPropertiesModal #goodType option:selected').text() + '</td>' +
            '<td>' + $('#addPAPPropertiesModal #treeName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #pointEasting').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #pointNorthing').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #pointElevation').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerFirstName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerMiddleName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerLastName').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPrimaryResidence').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPrimaryResidence').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPhotoID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerFileID').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPicture').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #treeMaturity').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #treeMaturity option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #treeQty').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #treeComments').val() + '</td>' +
            '<td style="display:none">' + $('#addPAPPropertiesModal #ownerPersonId').val() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeGoods" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';

        $("#propertiesTable tbody").append(tr);

        $('#addPAPPropertiesModal #gpsID, #addPAPPropertiesModal #goodPictureID, #addPAPPropertiesModal #pointEasting, #addPAPPropertiesModal #pointNorthing, #addPAPPropertiesModal #pointElevation, #addPAPPropertiesModal #treeName, #addPAPPropertiesModal #treeQty, #addPAPPropertiesModal #ownerFirstName, #addPAPPropertiesModal #ownerMiddleName, #addPAPPropertiesModal #ownerLastName, #addPAPPropertiesModal #ownerPhotoID, #addPAPPropertiesModal #ownerFileID, #addPAPPropertiesModal #treeComments, #addPAPPropertiesModal #ownerPrimaryResidence, #addPAPPropertiesModal #ownerPersonId').val('');
        $('#addPAPPropertiesModal #goodType, #addPAPPropertiesModal #treeMaturity').val('-1');
        $('#addPAPPropertiesModal #treePhoto, #addPAPPropertiesModal #ownerPicture').fileinput('clear');
        $('#addPAPPropertiesModal #ownershipType').prop("checked", false);
        $("#addPAPPropertiesModal .hiden").hide();
        $('#addPAPPropertiesModal #gpsID').focus();
    }
    return false;
});

$('#addPAPPropertiesModal #ownerFirstName').on('blur', function () {
    let papID = $('#addPAPPropertiesModal #ownerFirstName').val();
    let lacID = $('#addPAPPropertiesModal #propertyLacID').val();
    LoadOwnerLacDetails(papID, lacID, "addPAPPropertiesModal");
});

$('#addPAPPropertiesModal #propertyLacID').on('blur', function () {
    let papID = $('#addPAPPropertiesModal #propertyPersonID').val();
    let lacID = $('#addPAPPropertiesModal #propertyLacID').val();
    LoadPAPLacDetails(papID, lacID, "addPAPPropertiesModal");
});

$("#addPAPPropertiesModal").on("click", "#savePapProperties", function () {
    let url = $(location).attr('pathname');
    let papID = $("#addPAPPropertiesModal #propertyPapID").val();
    let lacID = $('#addPAPPropertiesModal #propertyLacID').val();
    let lacName = $('#addPAPPropertiesModal #propertyLacID option:selected').text();
    let surveyor = $('#addPAPPropertiesModal #propertyPresurveyorCode').val();
    let surveyorDate = $('#addPAPPropertiesModal #presurveyorDate').val();
    let surveyGPS = $('#addPAPPropertiesModal #presurveyorGPS').val();
    let currentDate = new Date();
    var surveyorDate2 = new Date(surveyorDate);
    var propertiesArray = convertTableToArray("#addPAPPropertiesModal #propertiesTable");

    var errors = [];

    console.log("Lac ID " + lacID + " - Surveyor Code " + surveyor + " - Survey Date " + surveyorDate + " -GPS ID " + surveyGPS);

    if (isNaN(lacID) || lacID == null || lacID == "")
        errors.push("Please refresh the page, because this Lac ID doesn't exist!");
    if (!surveyor || surveyor === "" || surveyor.length === 0 || !surveyor.trim())
        errors.push("Surveyor Code can't be empty")
    if (!Date.parse(surveyorDate))
        errors.push("Survey Date is not valid.");
    if (surveyorDate2 > currentDate)
        errors.push("Survey Date can't be in the future");
    if (!surveyGPS || surveyGPS == "" || surveyGPS.length === 0 || !surveyGPS.trim() || surveyGPS == null)
        errors.push("Please identify the GPS equipment");
    if (propertiesArray.length < 1)
        errors.push("Can't save a PAP without properties!. Please add properties and then try again");
    if ($('#addPAPPropertiesModal #fileID').val() == "" || $('#addPAPPropertiesModal #fileID').val() == null)
        errors.push("PAP File ID for this new LAC is mandatory!");

    if (errors.length >= 1) {
        var errorMessage = "<ul>";
        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#addPAPPropertiesModal #papSurveyorError").html(errorMessage).show(250);
        return false;
    }
    else {
        $("#addPAPPropertiesModal #loader").show();
        let pap = {
            PAPId: papID,
            PersonId: $("#addPAPPropertiesModal #propertyPersonID").val(),
            PresurveyDate: surveyorDate,
            PresurveyorCode: surveyor,
            PresurveyorGPS: surveyGPS,
            FileNumber: $('#addPAPPropertiesModal #fileID').val(),
            LACId: lacID,
            LACName: lacName
        };
        console.log(pap);
        console.log(propertiesArray);

        let PapLac = JSON.stringify(pap);
        formData.append("PAPLAC", PapLac);
        formData.append("Properties", JSON.stringify(propertiesArray));

        $.ajax({
            type: "POST",
            url: "/Community/AddPAPProperty/",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response[0] == "Success") {
                    LoadPapLacProperties(papID);
                    LoadPapLacDetails(papID);
                    resetModal("addPAPPropertiesModal");
                    $.notify(response[1], "success");
                } else if (response[0] == "Error") {
                    $("#addPAPPropertiesModal #loader").hide();
                    $.notify(response[1], "error");
                } else {
                    $("#addPAPPropertiesModal #loader").hide();
                    $.notify("An error occured while inserting property", "error");
                    console.log(response[1]);
                }
                console.log(response);
            }
        });
        return true;
    }
});

//#endregion

//#region PAP SURVEY
$("#surveyPAPModal #surveyCompleted").on("change", function () {
    if ($(this).prop('checked')) {
        $("#surveyPAPModal #surveyfileID").prop('disabled', false);
        $("#surveyPAPModal #surveylacID").prop('disabled', false);
    }
    else {
        $("#surveyPAPModal #surveyfileID").prop('disabled', true);
        $("#surveyPAPModal #surveylacID").prop('disabled', true);
    }
});

$("#surveyPAPModal #hhNoneRevenue").on("change", function () {
    if ($(this).prop('checked')) {
        $("#surveyPAPModal #hhRevenueSource, #surveyPAPModal #hhMonthlyRevenue, #surveyPAPModal #hhMonthCounts, #surveyPAPModal #addRevenue, #surveyPAPModal #hhRevenue").prop('disabled', true);
        $("#hhRevenue tbody").children().remove();
    }
    else {
        $("#surveyPAPModal #hhRevenueSource, #surveyPAPModal #hhMonthlyRevenue, #surveyPAPModal #hhMonthCounts, #surveyPAPModal #addRevenue, #surveyPAPModal #hhRevenue").prop('disabled', false);
    }
});

$("#surveyPAPModal #hhNeverCompensed").on("change", function () {
    if ($(this).prop('checked')) {
        $("#surveyPAPModal #hhYearCompensed, #surveyPAPModal #hhAmountCompensed, #surveyPAPModal #hhSurfaceCompensed, #surveyPAPModal #addCompensation, #surveyPAPModal #hhCompensedMoneyUtilisation, #surveyPAPModal #hhTFMCompensed").prop('disabled', true);
        $("#hhTFMCompensed tbody").children().remove();
    }
    else {
        $("#surveyPAPModal #hhYearCompensed, #surveyPAPModal #hhAmountCompensed, #surveyPAPModal #hhSurfaceCompensed, #surveyPAPModal #addCompensation, #surveyPAPModal #hhCompensedMoneyUtilisation, #surveyPAPModal #hhTFMCompensed").prop('disabled', false);
    }
});

$("#surveyPAPModal #hhNoEquipment").on("change", function () {
    if ($(this).prop('checked')) {
        $("#surveyPAPModal #hhEquipmentName, #surveyPAPModal #hhEquipmentDescription, #surveyPAPModal #hhEquipmentCount, #surveyPAPModal #addEquipment, #surveyPAPModal #hhEquipment").prop('disabled', true);
        $("#hhEquipment tbody").children().remove();
    }
    else {
        $("#surveyPAPModal #hhEquipmentName, #surveyPAPModal #hhEquipmentDescription, #surveyPAPModal #hhEquipmentCount, #surveyPAPModal #addEquipment, #surveyPAPModal #hhEquipment").prop('disabled', false);
    }
});

$("#surveyPAPModal #hhNoAnimal").on("change", function () {
    if ($(this).prop('checked')) {
        $("#surveyPAPModal #hhAnimalName, #surveyPAPModal #hhAnimalCount, #surveyPAPModal #hhAnimalDesc, #surveyPAPModal #addAnimal, #surveyPAPModal #hhAnimals").prop('disabled', true);
        $("#hhAnimals tbody").children().remove();
    }
    else {
        $("#surveyPAPModal #hhAnimalName, #surveyPAPModal #hhAnimalCount, #surveyPAPModal #hhAnimalDesc,  #surveyPAPModal #addAnimal, #surveyPAPModal #hhAnimals").prop('disabled', false);
    }
});

$("#surveyPAPModal #hhNoCulture").on("change", function () {
    if ($(this).prop('checked')) {
        $("#surveyPAPModal #hhCultureLocation, #surveyPAPModal #hhCultureDistance,  #surveyPAPModal #hhCultureWorker, #surveyPAPModal #hhCultureCultivated, #surveyPAPModal #hhCultureFallow, #surveyPAPModal #hhCultureRent, #surveyPAPModal #hhCultureAcquisitionYear, #surveyPAPModal #addCulture, #surveyPAPModal #hhCultures").prop('disabled', true);
        $("#hhAnimals hhCultures").children().remove();
        $("#surveyPAPModal input[name='hhCultureAcquired'], #surveyPAPModal input[name='hhCultureOwnership']").attr('disabled', true);
    }
    else {
        $("#surveyPAPModal #hhCultureLocation, #surveyPAPModal #hhCultureDistance,  #surveyPAPModal #hhCultureWorker, #surveyPAPModal #hhCultureCultivated, #surveyPAPModal #hhCultureFallow, #surveyPAPModal #hhCultureRent, #surveyPAPModal #hhCultureAcquisitionYear, #surveyPAPModal #addCulture, #surveyPAPModal #hhCultures").prop('disabled', false);
        $("#surveyPAPModal input[name=hhCultureAcquired], #surveyPAPModal input[name=hhCultureOwnership]").attr('disabled', false);
    }
});

$("#surveyPAPModal #hhCultureSales").on("change", function () {
    if ($(this).prop('checked')) {
        $("#surveyPAPModal #cultureSale").show();
    }
    else {
        $("#surveyPAPModal #cultureSale").hide();
    }
});

$("#surveyPAPModal input[name='hhMosquito']").on("change", function () {
    if ($(this).prop('checked')) {
        $("#surveyPAPModal input[name='hhMosquitoUsage']").prop("disabled", false);
    }
    else {
        $("#surveyPAPModal input[name='hhMosquitoUsage']").prop("disabled", true);
    }
});

$('#surveyPAPModal #hhDoctor').on("change", function () {
    if ($(this).prop('checked'))
        $('#surveyPAPModal input[name="medecineLocation"]').prop("disabled", false);
    else
        $('#surveyPAPModal input[name="medecineLocation"]').prop("disabled", true);
});

$("#surveyPAPModal #hhRelation, #surveyPAPModal #hhfirstName, #surveyPAPModal #hhlastName, #surveyPAPModal #hhMaritalStatus, #surveyPAPModal #hhdob, #surveyPAPModal #hhSchoolLevel, #surveyPAPModal #hhVulnerability, #surveyPAPModal #hhFrenchLevel, #surveyPAPModal #hhActivity, #surveyPAPModal #hhSkill, #surveyPAPModal #hhPhotoID, #surveyPAPModal #hhPhoto, #surveyPAPModal #interviewedName, #surveyPAPModal #surveylacID, #surveyPAPModal #surveyfileID, #surveyPAPModal #surveyCode, #surveyPAPModal #surveyDate, #surveyPAPModal input[name ='surveyCompleted'], #surveyPAPModal input[name='interviewedType'], #surveyPAPModal #surveyRegion, #surveyPAPModal #surveyVillage").on("change", function () { $("#surveyPAPModal #papSurveyError").hide() });

$("#surveyPAPModal #hhRelation, #surveyPAPModal #hhfirstName, #surveyPAPModal #hhlastName, #surveyPAPModal #hhMaritalStatus, #surveyPAPModal #hhdob, #surveyPAPModal #hhSchoolLevel, #surveyPAPModal #hhVulnerability, #surveyPAPModal #hhFrenchLevel, #surveyPAPModal #hhActivity, #surveyPAPModal #hhSkill, #surveyPAPModal #hhPhotoID, #surveyPAPModal #hhPhoto, #surveyPAPModal #interviewedName, #surveyPAPModal #surveylacID, #surveyPAPModal #surveyfileID, #surveyPAPModal #surveyCode, #surveyPAPModal #surveyDate, #surveyPAPModal input[name ='surveyCompleted'], #surveyPAPModal input[name='interviewedType'], #surveyPAPModal #surveyRegion, #surveyPAPModal #surveyVillage").on("focus", function () { $("#surveyPAPModal #papSurveyError").hide() });

$('#surveyPAPModal #hhDoctor, #surveyPAPModal input[name="medecineLocation"], #surveyPAPModal input[name="hhChildren"], #surveyPAPModal input[name="hhMosquito"], #surveyPAPModal input[name="hhMosquitoUsage"], #surveyPAPModal input[name="hhParticipation"]').on("change", function () { $("#surveyPAPModal #papHHDetailsError").hide() });

$("#surveyPAPModal #hhRoof, #surveyPAPModal #hhWall, #surveyPAPModal #hhFloor, #surveyPAPModal #hhReason, #surveyPAPModal #hhLatrine, #surveyPAPModal #hhResidenceLocation, #surveyPAPModal #hhRent, #surveyPAPModal #hhArrivalDate, #surveyPAPModal input[name='hhOwnership'], #surveyPAPModal input[name='hhStatus'], #surveyPAPModal #hhMonthlyRevenue, #surveyPAPModal #hhMonthCounts, #surveyPAPModal #hhRevenueSource").on("change", function () { $("#surveyPAPModal #papResidenceError").hide(); });

$("#surveyPAPModal #hhRoof, #surveyPAPModal #hhWall, #surveyPAPModal #hhFloor, #surveyPAPModal #hhReason, #surveyPAPModal #hhLatrine, #surveyPAPModal #hhResidenceLocation, #surveyPAPModal #hhRent, #surveyPAPModal #hhArrivalDate, #surveyPAPModal input[name='hhOwnership'], #surveyPAPModal input[name='hhStatus'], #surveyPAPModal #hhMonthlyRevenue, #surveyPAPModal #hhMonthCounts, #surveyPAPModal #hhRevenueSource").on("focus", function () { $("#surveyPAPModal #papResidenceError").hide(); });

$("#surveyPAPModal #hhEquipmentName, #surveyPAPModal #hhEquipmentDescription, #surveyPAPModal #hhEquipmentCount, #surveyPAPModal #hhYearCompensed, #surveyPAPModal #hhAmountCompensed, #surveyPAPModal #hhSurfaceCompensed, #surveyPAPModal #hhCompensedMoneyUtilisation, #surveyPAPModal #hhAnimalName, #surveyPAPModal #hhAnimalCount, #surveyPAPModal #hhAnimalDesc, #surveyPAPModal #hhLastWeekExpense").on("change", function () { $("#surveyPAPModal #papRevenueError").hide();});

$("#surveyPAPModal #hhEquipmentName, #surveyPAPModal #hhEquipmentDescription, #surveyPAPModal #hhEquipmentCount, #surveyPAPModal #hhYearCompensed, #surveyPAPModal #hhAmountCompensed, #surveyPAPModal #hhSurfaceCompensed, #surveyPAPModal #hhCompensedMoneyUtilisation, #surveyPAPModal #hhAnimalName, #surveyPAPModal #hhAnimalCount, #surveyPAPModal #hhAnimalDesc, #surveyPAPModal #hhLastWeekExpense").on("focus", function () { $("#surveyPAPModal #papRevenueError").hide(); });

$("#surveyPAPModal #hhCultureLocation, #surveyPAPModal #hhCultureDistance, #surveyPAPModal #hhCultureCultivated, #surveyPAPModal #hhCultureFallow, #surveyPAPModal #hhCultureRent, #surveyPAPModal #hhCultureAcquisitionYear, #surveyPAPModal input[name='hhCultureOwnership'], #surveyPAPModal input[name='hhCultureAcquired'],#surveyPAPModal #hhCultureDesc, #surveyPAPModal #hhCultureArea, #surveyPAPModal #hhCultureArea, #surveyPAPModal #hhCultureSold, #surveyPAPModal #hhCultureHarvest, #surveyPAPModal #hhCultureRevenue").on("change", function () { $("#surveyPAPModal #papCultureError").hide(); });

$("#surveyPAPModal #hhCultureLocation, #surveyPAPModal #hhCultureDistance, #surveyPAPModal #hhCultureCultivated, #surveyPAPModal #hhCultureFallow, #surveyPAPModal #hhCultureRent, #surveyPAPModal #hhCultureAcquisitionYear, #surveyPAPModal input[name='hhCultureOwnership'], #surveyPAPModal input[name='hhCultureAcquired'],#surveyPAPModal #hhCultureDesc, #surveyPAPModal #hhCultureArea, #surveyPAPModal #hhCultureArea, #surveyPAPModal #hhCultureSold, #surveyPAPModal #hhCultureHarvest, #surveyPAPModal #hhCultureRevenue").on("focus", function () { $("#surveyPAPModal #papCultureError").hide(); });

$("#surveyPAPModal #hhCulturePaid, #surveyPAPModal #hhCultureTool, #surveyPAPModal input[name='hhCulturePaidTime']").on("change", function () { $("#surveyPAPModal #papCultureSoldError").hide(); });

$("#surveyPAPModal #hhCulturePaid, #surveyPAPModal #hhCultureTool, #surveyPAPModal input[name='hhCulturePaidTime']").on("focus", function () { $("#surveyPAPModal #papCultureSoldError").hide(); });

$("#surveyPAPModal #surveylacID").on("change", function () {
    let errors = [];
    let surveyDate = $('#surveyPAPModal #surveyDate').val();
    let surveyDate2 = new Date(surveyDate);
    let surveyed = $('#surveyPAPModal #surveyCompleted').prop('checked');

    if ($('#surveyPAPModal #surveyCode').val() == null || $('#surveyPAPModal #surveyCode').val() == "")
        errors.push("Please add Surveyor Code");
    if ($('#surveyPAPModal #fileID').val() == null || $('#surveyPAPModal #fileID').val() == "")
        errors.push("Please add the file ID");
    if (surveyDate2 > new Date())
        errors.push("Survey Date can't be in the future");
    if (!new Date($('#surveyPAPModal #surveyDate').val()))
        errors.push("Survey Date is not valid");
    if (surveyed && ($('#surveyPAPModal #surveyfileID').val() == null || $('#surveyPAPModal #surveyfileID').val() == ""))
        errors.push("Please specify file ID for this surveyed PAP");
    if (surveyed && ($('#surveyPAPModal #surveylacID').val() < 1 || $('#surveyPAPModal #surveylacID').val() == ""))
        errors.push("Please specify the LAC where this PAP get surveyed");
    if (surveyed && $('#surveyPAPModal #surveylacID').val() == $('#surveyPAPModal #lacId').val())
        errors.push("The previous survey selected LAC is the same as the one this PAP belongs. Please choose another LAC");

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $('#surveyPAPModal #surveylacID').val('-1');
        $("#surveyPAPModal #papSurveyError").html(errorMessage).show(250);
        return false;
    }
    else {
        $.confirm({
            title: 'Confirm',
            content: 'Are you sure to link this Household to LAC ' + $("#surveyPAPModal #surveylacID option:selected").text() + ' (' + $('#surveyPAPModal #surveyfileID').val() + ')',
            type: 'red',
            typeAnimated: false,
            theme: 'dark',
            backgroundDismiss: false,
            backgroundDismissAnimation: 'glow',
            buttons: {
                cancel: {
                    btnClass: 'btn-red',
                    action: function () {
                        $.alert('Cancel');
                    }
                },
                continue: {
                    btnClass: 'btn-success',
                    action: function () {
                        savePAPSurvey();
                    }
                }
            }
        });
    }
});

$("#surveyPAPModal #hhAnimalName").on("change", function () {
    if ($("#surveyPAPModal #hhAnimalName option:selected").text() == "None") {
        $("#surveyPAPModal #hhNoAnimal").prop('checked', true);
        $("#surveyPAPModal #hhAnimalName").val('-1');
        $("#surveyPAPModal #hhAnimalName, #surveyPAPModal #hhAnimalCount,  #surveyPAPModal #addAnimal, #surveyPAPModal #hhAnimals, #surveyPAPModal #hhAnimalDesc").prop('disabled', true);
        $("#hhAnimals tbody").children().remove();
    }
    if ($("#surveyPAPModal #hhAnimalName option:selected").text() == "Other")
        $("#surveyPAPModal #hhAnimalDesc").prop('disabled', false);
    else {
        $("#surveyPAPModal #hhAnimalDesc").prop('disabled', true);
        $("#surveyPAPModal #hhAnimalCount").focus();
    }
});

$("#surveyPAPModal #hhEquipmentName").on("change", function () {
    if ($("#surveyPAPModal #hhEquipmentName option:selected").text() == "None") {
        $("#surveyPAPModal #hhNoEquipment").prop("checked", true);
        $("#surveyPAPModal #hhEquipmentName").val('-1');
        $("#surveyPAPModal #hhEquipmentName, #surveyPAPModal #hhEquipmentDescription, #surveyPAPModal #hhEquipmentCount, #surveyPAPModal #addEquipment, #surveyPAPModal #hhEquipment").prop('disabled', true);
        $("#hhEquipment tbody").children().remove();
    }
    if ($("#surveyPAPModal #hhEquipmentName option:selected").text() == "Other")
        $("#surveyPAPModal #hhEquipmentDescription").prop('disabled', false);
    else {
        $("#surveyPAPModal #hhEquipmentDescription").prop('disabled', true);
        $("#surveyPAPModal #hhEquipmentCount").focus();
    } 
});

$("#surveyPAPModal #hhRevenueSource").on("change", function () {
    if ($("#surveyPAPModal #hhRevenueSource option:selected").text() == "None") {
        $("#surveyPAPModal #hhNoneRevenue").prop("checked", true);
        $("#surveyPAPModal #hhRevenueSource").val('-1');
        $("#surveyPAPModal #hhRevenueSource, #surveyPAPModal #hhMonthlyRevenue, #surveyPAPModal #hhMonthCounts, #surveyPAPModal #addRevenue, #surveyPAPModal #hhRevenue").prop('disabled', true);
        $("#hhRevenue tbody").children().remove();
    }
    else
        $("#surveyPAPModal #hhMonthlyRevenue").focus;
});

$("#surveyPAPModal #hhCultureName").on("change", function () {
    if ($("#surveyPAPModal #hhCultureName option:selected").text() == "Other" || $("#surveyPAPModal #hhCultureName option:selected").text() == "Maize" || $("#surveyPAPModal #hhCultureName option:selected").text() == "Bean")
        $("#surveyPAPModal #hhCultureDesc").prop('disabled', false);
    else {
        $("#surveyPAPModal #hhCultureDesc").prop('disabled', true);
        $("#surveyPAPModal input[name='hhCultureCAV']").focus;
    }
        
});

$("#surveyPAPModal #surveyRegion").on("change", function () {
    let regionId = $("#surveyPAPModal #surveyRegion").val();
    
    if (regionId > 0)
        GetRegionVillageForSelect("surveyPAPModal #surveyVillage", regionId, '-1');
    else
        GetVillageForSelect('surveyPAPModal #surveyVillage', '-1');
});

$("#surveyPAPModal #surveyVillage").on("change", function () {
    let villageId = $("#surveyPAPModal #surveyVillage").val();

    if (villageId > 0) 
        $('#surveyPAPModal #hhResidence, #surveyPAPModal #hhResidenceLocation').val($("#surveyPAPModal #surveyVillage option:selected").text());
});

$("#surveyPAPModal").on("click", "#addHouseHold", function () {
    //Checking if all data are valid and then add them to the table
    let dob = $("#surveyPAPModal #hhdob").val();
    let gender = $('#surveyPAPModal input[name="gender"]:checked').val();
    let student = $("#surveyPAPModal #hhStudying").prop('checked');
    let dob2 = new Date(dob);
    let errors = [];
    if ($("#surveyPAPModal #hhRelation").val() < 1 || $("#surveyPAPModal #hhRelation").val() == null)
        errors.push("Please select relationship");
    if (notInHHTables($("#surveyPAPModal #hhRelation option:selected").text(), "#surveyPAPModal #hhTable"))
        errors.push("You can not have more than one " + $("#surveyPAPModal #hhRelation option:selected").text());
    if ($("#surveyPAPModal #hhfirstName").val() == "" || $("#surveyPAPModal #hhfirstName").val() == null)
        errors.push("First Name can't be empty");
    if ($("#surveyPAPModal #hhlastName").val() == "" || $("#surveyPAPModal #hhlastName").val() == null)
        errors.push("Last Name can't be empty");
    if (notInHHTables($("#surveyPAPModal #hhfirstName").val() + " " + $("#surveyPAPModal #hhlastName").val(), "#surveyPAPModal #hhTable"))
        errors.push("You can not have more than one Household Responsible or Spouse");
    if ($("#surveyPAPModal #hhMaritalStatus").val() < 1 || $("#surveyPAPModal #hhMaritalStatus").val() == null)
        errors.push("Please select the Marital Status");
    if ($("#surveyPAPModal #hhdob").val() == "" || $("#surveyPAPModal #hhdob").val() == null)
        errors.push("This DOB is not valid");
    if (dob2 > new Date())
        errors.push("DOB can't be in the future");
    if ($("#surveyPAPModal #hhSchoolLevel").val() < 1 || $("#surveyPAPModal #hhSchoolLevel").val() == null)
        errors.push("Please choose the School Level");
    if ($("#surveyPAPModal #hhVulnerability").val() < 1 || $("#surveyPAPModal #hhVulnerability").val() == null)
        errors.push("Please choose vunerability for this member");
    if ($("#surveyPAPModal #hhFrenchLevel").val() < 1 || $("#surveyPAPModal #hhFrenchLevel").val() == null)
        errors.push("Choose French Level");
    if ($("#surveyPAPModal #hhActivity").val() == "" || $("#surveyPAPModal #hhActivity").val() == null)
        errors.push("Please specify Activities");
    if ($("#surveyPAPModal #hhSkill").val() == "" || $("#surveyPAPModal #hhSkill").val() == null)
        errors.push("Please specify Skills");
    if ($('#surveyPAPModal #hhPhotoID').val() == null || $('#surveyPAPModal #hhPhotoID').val() == "" || parseInt($('#surveyPAPModal #hhPhotoID').val())<1)
        errors.push("Please add the picture ID");
    if (notInHHTables($('#surveyPAPModal #hhPhotoID').val(), "#surveyPAPModal #hhTable"))
        errors.push("This picture ID is already used by another Household member");
    if ($('#surveyPAPModal #hhPhoto').val() == null || $('#surveyPAPModal #hhPhoto').val() == "")
        errors.push("You did not put Picture");
    if (gender == null || gender == "")
        errors.push("Please choose a valid Gender");

    let selectedActivity = $('#surveyPAPModal #hhActivity').select2("data");
    let Activities = [];
    Activities = selectedActivity.length > 1 ? [selectedActivity[0].text, selectedActivity[1].text] : [selectedActivity[0].text];
    let selectedSkills = $('#surveyPAPModal #hhSkill').select2("data")
    let Skills = [];
    Skills = selectedSkills.length > 1 ? [selectedSkills[0].text, selectedSkills[1].text] : [selectedSkills[0].text];

    if (errors.length >= 1) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#surveyPAPModal #papSurveyError").html(errorMessage).show(250);
    }
    else {
        let hhPhoto = $("#surveyPAPModal #hhPhoto")[0];
        let hhPhotoFile = hhPhoto.files;

        if (hhPhoto.files.length > 0)
            formData.append(hhPhotoFile[0].name, hhPhotoFile[0]);

        let residence = (($('#surveyPAPModal #hhResidence').val() == null || $('#surveyPAPModal #hhResidence').val() == "" || $('#surveyPAPModal #hhResidence').val() == "SPC") && ($('#surveyPAPModal #surveyVillage').val() > 0)) ? $('#surveyPAPModal #surveyVillage').val() : $('#surveyPAPModal #hhResidence').val();

        var tr = ""; 
        tr += '<tr>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhRelation").val() + '</td>' +
            '<td>' + $("#surveyPAPModal #hhRelation option:selected").text() + '</td>' +
            '<td> ' + $.trim($("#surveyPAPModal #hhfirstName").val()) + '</td>' +
            '<td style="display:none">' + $.trim($("#surveyPAPModal #hhmiddleName").val()) + '</td>' +
            '<td>' + $.trim($('#surveyPAPModal #hhlastName').val()) + '</td>' +
            '<td style="display:none">' + gender + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhMaritalStatus").val() + '</td>' +
            '<td>' + dob + '</td>' +
            '<td style="display:none">' + $.trim($('#surveyPAPModal #hhCardID').val()) + '</td>' +
            '<td style="display:none">' + student + '</td>' +
            '<td style="display:none">' + $('#surveyPAPModal #hhSchoolLevel').val() + '</td>' +
            '<td style="display:none">' + $('#surveyPAPModal #hhSchoolLevel option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#surveyPAPModal #hhFrenchLevel').val() + '</td>' +
            '<td>' + $('#surveyPAPModal #hhFrenchLevel option:selected').text() + '</td>' +
            '<td>' + $.trim($('#surveyPAPModal #hhPhotoID').val()) + '</td>' +
            '<td style="display:none">' + $('#surveyPAPModal #hhPhoto').val() + '</td>' +
            '<td style="display:none">' + residence + '</td>' +
            '<td style="display:none">' + $('#surveyPAPModal #hhActivity').val() + '</td>' +
            '<td style="display:none">' + Activities + '</td>' +
            '<td style="display:none">' + $('#surveyPAPModal #hhSkill').val() + '</td>' +
            '<td style="display:none">' + Skills + '</td>' +
            '<td style="display:none">' + $('#surveyPAPModal #hhVulnerability').val() + '</td>' +
            '<td style="display:none">' + $('#surveyPAPModal #hhVulnerability option:selected').text() + '</td>' +
            '<td style="display:none">' + $.trim($('#surveyPAPModal #hhHandicapDetails').val()) + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeHousehold" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';

        $("#hhTable tbody").append(tr);

        $("#surveyPAPModal #hhActivity, #surveyPAPModal #hhSkill").val(null).trigger("change");
        $("#surveyPAPModal #hhRelation, #surveyPAPModal #hhVulnerability, #surveyPAPModal #hhFrenchLevel, #surveyPAPModal #hhSchoolLevel, #surveyPAPModal #hhMaritalStatus").val('-1');
        $("#surveyPAPModal #hhfirstName, #surveyPAPModal #hhmiddleName, #surveyPAPModal #hhlastName, #surveyPAPModal #hhCardID, #surveyPAPModal #hhResidence, #surveyPAPModal #hhPhotoID, #surveyPAPModal #hhHandicapDetails").val("");
        $('#surveyPAPModal #hhPhoto').fileinput('clear');
        $("#surveyPAPModal #hhStudying").prop("checked", false);
        $('#surveyPAPModal input[name="gender"]').prop("checked", false);
        $("#surveyPAPModal #surveyDate, #surveyPAPModal #hhdob").val(getCurrentDate());

        if ($("#surveyPAPModal #surveyVillage").val() > 0)
            $('#surveyPAPModal #hhResidence').val($("#surveyPAPModal #surveyVillage option:selected").text());

        $("#surveyPAPModal #hhRelation").focus();
    }
});

$("#surveyPAPModal").on("click", "#addResidence", function () {
    let ownership = $('#surveyPAPModal input[name="hhOwnership"]:checked').val();
    let usage = $('#surveyPAPModal input[name="hhStatus"]:checked').val();
    let arrivalDate = $('#surveyPAPModal #hhArrivalDate').val();
    let arrivalDate2 = new Date(arrivalDate, 0, 1);
    arrivalDate += "-01-01";

    let errors = [];
    if ($("#surveyPAPModal #hhResidenceLocation").val() == "" || $("#surveyPAPModal #hhResidenceLocation").val() == null)
        errors.push("Please add value for Residence Location");
    if ($("#surveyPAPModal #hhRoof").val() < 1 || $("#surveyPAPModal #hhRoof").val() == null)
        errors.push("Please select roof Type");
    if ($("#surveyPAPModal #hhWall").val() < 1 || $("#surveyPAPModal #hhWall").val() == null)
        errors.push("Please select wall Type");
    if ($("#surveyPAPModal #hhFloor").val() < 1 || $("#surveyPAPModal #hhFloor").val() == null)
        errors.push("Please select floor Type");
    if ($("#surveyPAPModal #hhReason").val() < 1 || $("#surveyPAPModal #hhReason").val() == null)
        errors.push("Please select location reason");
    if ($("#surveyPAPModal #hhLatrine").val() < 1 || $("#surveyPAPModal #hhLatrine").val() == null)
        errors.push("Please select Latrine Type");
    if ($("#surveyPAPModal #hhRoomQty").val() < 1 || $("#surveyPAPModal #hhRoomQty").val() == null)
        errors.push("Please add Room Qty");
    if (ownership == "" || ownership == null)
        errors.push("Please select choose ownership");
    if (usage == "" || usage == null)
        errors.push("Please select choose Usage");
    if (ownership =='U' && ($("#surveyPAPModal #hhRent").val() < 1 || $("#surveyPAPModal #hhRent").val() == null))
        errors.push("Please add the value for rent by month");
    if (arrivalDate2 > new Date())
        errors.push("Arrival Year can't be in the future");

    if (errors.length >= 1) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#surveyPAPModal #papResidenceError").html(errorMessage).show(250);
    }
    else {
        var tr = "";
        tr += '<tr>' +
            '<td>' + $.trim($("#surveyPAPModal #hhResidenceLocation").val()) + '</td>' +
            '<td style= "display:none">' + $("#surveyPAPModal #hhRoof").val() + '</td>' +
            '<td>' + $("#surveyPAPModal #hhRoof option:selected").text() + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhWall").val() + '</td>' +
            '<td>' + $("#surveyPAPModal #hhWall option:selected").text() + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhFloor").val() + '</td>' +
            '<td>' + $("#surveyPAPModal #hhFloor option:selected").text() + '</td>' +
            '<td>' + ownership + '</td>' +
            '<td>' + usage + '</td>' +
            '<td style="display:none">' + $.trim($("#surveyPAPModal #hhRent").val()) + '</td>' +
            '<td style="display:none">' + arrivalDate + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhReason").val() + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhReason option:selected").text() + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhLatrine").val() + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhLatrine option:selected").text() + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhRoomQty").val() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeResidence" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';

        $("#hhResidence tbody").append(tr);
        $("#surveyPAPModal #hhResidenceLocation, #surveyPAPModal #hhRent, #surveyPAPModal #hhRoomQty").val('');
        $("#surveyPAPModal #hhRoof, #surveyPAPModal #hhWall, #surveyPAPModal #hhFloor, #surveyPAPModal #hhReason, #surveyPAPModal #hhLatrine").val('-1');
        $('#surveyPAPModal #hhArrivalDate').val(getCurrentDate());
        $('#surveyPAPModal input[name="hhOwnership"], #surveyPAPModal input[name="hhStatus"]').prop('checked', false);
        $("#surveyPAPModal #hhResidenceLocation").focus();
    }
});

$("#surveyPAPModal").on ("click", "#addRevenue", function () {
    let errors = [];
    let revenueName = $("#surveyPAPModal #hhRevenueSource option:selected").text();

    if (revenueName == "None") {
        $("#surveyPAPModal #hhNoneRevenue").prop("checked", true);
        $("#surveyPAPModal #hhRevenueSource").val('-1');
        $("#surveyPAPModal #hhRevenueSource, #surveyPAPModal #hhMonthlyRevenue, #surveyPAPModal #hhMonthCounts, #surveyPAPModal #addRevenue, #surveyPAPModal #hhRevenue").prop('disabled', true);
        $("#hhRevenue tbody").children().remove();
    } else {
        if ($("#surveyPAPModal #hhRevenueSource").val() < 1 || $("#surveyPAPModal #hhRevenueSource").val() == null)
            errors.push("Please select Revenue Source");
        if (notInHHTables($("#surveyPAPModal #hhRevenueSource option:selected").text(), "#surveyPAPModal #hhRevenue"))
            errors.push("This revenue source is already used in the table. Please choose another Revenue Source");
        if (revenueName != "None" && ($("#surveyPAPModal #hhMonthlyRevenue").val() < 1 || $("#surveyPAPModal #hhMonthlyRevenue").val() == null))
            errors.push("Please add monthly revenue");
        if (revenueName != "None" && ($("#surveyPAPModal #hhMonthCounts").val() < 1 || $("#surveyPAPModal #hhMonthCounts").val() == null || $("#surveyPAPModal #hhMonthCounts").val() > 12))
            errors.push("Please add valid month count");

        if (errors.length >= 1) {
            var errorMessage = "<ul>";

            $.each(errors, function (index, value) {
                errorMessage += "<li>" + value + "</li>";
            });
            errorMessage += "</ul>";
            $("#surveyPAPModal #papResidenceError").html(errorMessage).show(250);
        }
        else {
            var tr = "";
            tr += '<tr>' +
                '<td style= "display:none">' + $("#surveyPAPModal #hhRevenueSource").val() + '</td>' +
                '<td>' + $("#surveyPAPModal #hhRevenueSource option:selected").text() + '</td>' +
                '<td>' + $("#surveyPAPModal #hhMonthlyRevenue").val() + '</td>' +
                '<td>' + $("#surveyPAPModal #hhMonthCounts").val() + '</td>' +
                '<td>' + '<a href="#" class="btn btn-danger" id="removeRevenue" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
                '</tr>';

            $("#hhRevenue tbody").append(tr);
            $("#surveyPAPModal #hhMonthlyRevenue, #surveyPAPModal #hhMonthCounts").val('');
            $("#surveyPAPModal #hhRevenueSource").val('-1');
            $("#surveyPAPModal #hhRevenueSource").focus();
        }
    }
});

$("#surveyPAPModal").on("click", "#addCompensation", function () {
    let errors = [];
    if ($("#surveyPAPModal #hhYearCompensed").val() < 1 || $("#surveyPAPModal #hhYearCompensed").val() == null)
        errors.push("Please specify the Year");
    if ($("#surveyPAPModal #hhAmountCompensed").val() < 1 || $("#surveyPAPModal #hhAmountCompensed").val() == null)
        errors.push("Please specify amount");
    if ($("#surveyPAPModal #hhSurfaceCompensed").val() < 1 || $("#surveyPAPModal #hhSurfaceCompensed").val() == null)
        errors.push("Please specify surface");

    if (errors.length >= 1) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#surveyPAPModal #papRevenueError").html(errorMessage).show(250);
    }
    else {
        let tr = '';
        tr += '<tr>' +
            '<td>' + $("#surveyPAPModal #hhYearCompensed").val() + '</td>' +
            '<td>' + $("#surveyPAPModal #hhAmountCompensed").val() + '</td>' +
            '<td>' + $("#surveyPAPModal #hhSurfaceCompensed").val() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeCompensation" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';

        $("#surveyPAPModal #hhTFMCompensed tbody").append(tr);
        $("#surveyPAPModal #hhAmountCompensed, #surveyPAPModal #hhSurfaceCompensed, #surveyPAPModal #hhYearCompensed").val('');
        $("#surveyPAPModal #hhYearCompensed").focus();
    }
});

$("#surveyPAPModal").on("click", "#addEquipment", function () {
    let errors = [];
    let equipment = $("#surveyPAPModal #hhEquipmentName option:selected").text();
    let equipmentDesc = $("#surveyPAPModal #hhEquipmentDescription").val();

    if (equipment == "None") {
        $("#surveyPAPModal #hhNoEquipment").prop("checked", true);
        $("#surveyPAPModal #hhEquipmentName").val('-1');
        $("#surveyPAPModal #hhEquipmentName, #surveyPAPModal #hhEquipmentDescription, #surveyPAPModal #hhEquipmentCount, #surveyPAPModal #addEquipment, #surveyPAPModal #hhEquipment").prop('disabled', true);
        $("#hhEquipment tbody").children().remove();
    }
    else {
        if ($("#surveyPAPModal #hhEquipmentName").val() < 1 || $("#surveyPAPModal #hhEquipmentName").val() == null)
            errors.push("Please specify equipment name");
        if (notInHHTables($("#surveyPAPModal #hhEquipmentName option:selected").text(), "#surveyPAPModal #hhEquipment"))
            errors.push("This Equipment is already used in the table. Please choose another Equipment");
        if (equipment != "None" && ($("#surveyPAPModal #hhEquipmentCount").val() < 1 || $("#surveyPAPModal #hhEquipmentCount").val() == null))
            errors.push("Please specify equipment quantity");
        if (equipment == "Other" && (equipmentDesc == "" || equipmentDesc == null))
            errors.push("Please specify Equipment Name");
        if (notInHHTables(equipmentDesc, "#surveyPAPModal #hhEquipment"))
            errors.push("This Equipment description is already used in the table. Please choose another Equipment description");

        if (errors.length >= 1) {
            var errorMessage = "<ul>";

            $.each(errors, function (index, value) {
                errorMessage += "<li>" + value + "</li>";
            });
            errorMessage += "</ul>";
            $("#surveyPAPModal #papRevenueError").html(errorMessage).show(250);
        }
        else {
            equipmentDesc = (equipmentDesc == "" || equipmentDesc == null) ? equipment : equipmentDesc;
            let tr = '';
            tr += '<tr>' +
                '<td style="display:none">' + $("#surveyPAPModal #hhEquipmentName").val() + '</td>' +
                '<td>' + equipment + '</td>' +
                '<td>' + $.trim(equipmentDesc) + '</td>' +
                '<td>' + $("#surveyPAPModal #hhEquipmentCount").val() + '</td>' +
                '<td>' + '<a href="#" class="btn btn-danger" id="removeEquipment" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
                '</tr>';

            $("#surveyPAPModal #hhEquipment tbody").append(tr);
            $("#surveyPAPModal #hhEquipmentDescription, #surveyPAPModal #hhEquipmentCount").val('');
            $("#surveyPAPModal #hhEquipmentName").val('-1');
            $("#surveyPAPModal #hhEquipmentName").focus();
        }
    }
});

$("#surveyPAPModal").on("click", "#addAnimal", function () {
    let errors = [];
    let animal = $("#surveyPAPModal #hhAnimalName option:selected").text();
    let animalDesc = $("#surveyPAPModal #hhAnimalDesc").val();

    if (animal == "None") {
        $("#surveyPAPModal #hhNoAnimal").prop('checked', true);
        $("#surveyPAPModal #hhAnimalName").val('-1');
        $("#surveyPAPModal #hhAnimalName, #surveyPAPModal #hhAnimalCount,  #surveyPAPModal #addAnimal, #surveyPAPModal #hhAnimals, #surveyPAPModal #hhAnimalDesc").prop('disabled', true);
        $("#hhAnimals tbody").children().remove();
    }
    else {
        if ($("#surveyPAPModal #hhAnimalName").val() < 1 || $("#surveyPAPModal #hhAnimalName").val() == null)
            errors.push("Please specify animal name");
        if (animal != "None" && ($("#surveyPAPModal #hhAnimalCount").val() < 1 || $("#surveyPAPModal #hhAnimalCount").val() == null))
            errors.push("Please specify animal quantity");
        if (animal == "Other" && (animalDesc == "" || animalDesc == null))
            errors.push("Animal Description can not be empty");
        if (notInHHTables($("#surveyPAPModal #hhAnimalName option:selected").text(), "#surveyPAPModal #hhAnimals"))
            errors.push("This Animal is already in the table. Please choose another Animal");
        if (notInHHTables(animalDesc, "#surveyPAPModal #hhAnimals"))
            errors.push("This Animal description is already used in the table. Please choose another Animal Description");

        if (errors.length >= 1) {
            var errorMessage = "<ul>";

            $.each(errors, function (index, value) {
                errorMessage += "<li>" + value + "</li>";
            });
            errorMessage += "</ul>";
            $("#surveyPAPModal #papRevenueError").html(errorMessage).show(250);
        }
        else {
            animalDesc = (animalDesc == "" || animalDesc == null) ? animal : animalDesc;
            let tr = '';
            tr += '<tr>' +
                '<td style="display:none">' + $("#surveyPAPModal #hhAnimalName").val() + '</td>' +
                '<td>' + animal + '</td>' +
                '<td>' + $.trim(animalDesc) + '</td>' +
                '<td>' + $("#surveyPAPModal #hhAnimalCount").val() + '</td>' +
                '<td>' + '<a href="#" class="btn btn-danger" id="removeAnimal" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
                '</tr>';

            $("#surveyPAPModal #hhAnimals tbody").append(tr);
            $("#surveyPAPModal #hhAnimalCount, #surveyPAPModal #hhAnimalDesc").val('');
            $("#surveyPAPModal #hhAnimalName").val('-1');
            $("#surveyPAPModal #hhAnimalName").focus();
        }
    }
});

$("#surveyPAPModal").on("click", "#addCulture", function () {
    let ownership = $('#surveyPAPModal input[name="hhCultureOwnership"]:checked').val();
    let acquired = $('#surveyPAPModal input[name="hhCultureAcquired"]:checked').val();
    let errors = [];
    if ($("#surveyPAPModal #hhCultureLocation").val() == "" || $("#surveyPAPModal #hhCultureLocation").val() == null)
        errors.push("Culture Location can't be empty");
    if ($("#surveyPAPModal #hhCultureDistance").val() < 1 || $("#surveyPAPModal #hhCultureDistance").val() == null)
        errors.push("Please add distance from Residence 1 to culture");
    if ($("#surveyPAPModal #hhCultureWorker").val() == "" || $("#surveyPAPModal #hhCultureWorker").val() == null)
        errors.push("Culture Worker can't be empty");
    if (($("#surveyPAPModal #hhCultureCultivated").val() < 0 || $("#surveyPAPModal #hhCultureCultivated").val() == null) && ($("#surveyPAPModal #hhCultureFallow").val() < 0 || $("#surveyPAPModal #hhCultureFallow").val() == null))
        errors.push("Cultivated area and Fallow area can't be both empty.");
    if (ownership == "" || ownership == null)
        errors.push("Please specify the culture ownership");
    if (acquired == "" || acquired == null)
        errors.push("You must add the acquired source");

    if (errors.length >= 1) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#surveyPAPModal #papCultureError").html(errorMessage).show(250);
    }
    else {

        let selectedWorker = $('#surveyPAPModal #hhCultureWorker').select2("data");
        let Workers = [];
        for (var i = 0; i <= selectedWorker.length - 1; i++) {
            Workers.push(selectedWorker[i].text);
        }
        

        let tr = '';
        tr += '<tr>' +
            '<td>' + $("#surveyPAPModal #hhCultureLocation").val() + '</td>' +
            '<td style="display:none">' + $.trim($("#surveyPAPModal #hhCultureDistance").val()) + '</td>' +
            '<td style="display:none">' + $.trim($("#surveyPAPModal #hhCultureWorker").val()) + '</td>' +
            '<td>' + Workers + '</td>' +
            '<td>' + $.trim($("#surveyPAPModal #hhCultureCultivated").val()) + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhCultureFallow").val() + '</td>' +
            '<td>' + ownership + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhCultureRent").val() + '</td>' +
            '<td>' + acquired + '</td>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhCultureAcquisitionYear").val() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeCultures" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';

        $("#surveyPAPModal #hhCultures tbody").append(tr);
        $("#surveyPAPModal #hhCultureLocation, #surveyPAPModal #hhCultureDistance, #surveyPAPModal #hhCultureCultivated, #surveyPAPModal #hhCultureFallow, #surveyPAPModal #hhCultureRent, #surveyPAPModal #hhCultureAcquisitionYear").val('');
        $('#surveyPAPModal input[name="hhCultureOwnership"], #surveyPAPModal input[name="hhCultureAcquired"]').prop('checked', false);
        $("#surveyPAPModal #hhCultureWorker").val(null).trigger("change");
        $("#surveyPAPModal #hhCultureLocation").focus();
    }
});

$("#surveyPAPModal").on("click", "#addCultureSold", function () {
    let errors = [];
    let culture = $("#surveyPAPModal #hhCultureName option:selected").text();
    let cultureDesc = $("#surveyPAPModal #hhCultureDesc").val();
    let cultureArea = $("#surveyPAPModal #hhCultureArea").val();
    let cultureHarvest = parseInt($("#surveyPAPModal #hhCultureHarvest").val());
    let cultureSold = parseInt($("#surveyPAPModal #hhCultureSold").val());
    let cultureRevenue = $("#surveyPAPModal #hhCultureRevenue").val();

    //console.log("Harvest " + cultureHarvest + " Sold " + cultureSold);

    var cultureCAV = [];
    $.each($("#surveyPAPModal input[name='hhCultureCAV']:checked"), function () {
        cultureCAV.push($(this).val());
    });

    if ($("#surveyPAPModal #hhCultureName").val() < 1 || $("#surveyPAPModal #hhCultureName").val() == null)
        errors.push("Please choose Culture");
    if ((culture == "Other" || culture == "Maize" || culture == "Bean") && (cultureDesc == "" || cultureDesc == null))
        errors.push("Please specify Culture Diversity Name");
    if (cultureCAV.length < 1)
        errors.push("Please specify if culture was Cultivated, Sold or Purchased");
    if ((cultureCAV.includes('C') && (culture == "Maize" || culture == "Bean")) && (cultureArea < 1 || cultureArea == null))
        errors.push("Please specify culture Area size");
    if ((cultureCAV.includes('C') && (culture == "Maize" || culture == "Bean")) && (cultureHarvest < 1 || cultureHarvest == null))
        errors.push("Please specify culture harvest quantity");
    if ((cultureCAV.includes('V') && (culture == "Maize" || culture == "Bean")) && (cultureSold < 1 || cultureSold == null))
        errors.push("Please specify culture sold quantity");
    if (cultureHarvest < cultureSold)
        errors.push("Culture Sold can't be higher than harvested quantity");
    if (cultureSold > 0 && (cultureRevenue < 1 || cultureRevenue == null))
        errors.push("You need to add Culture Revenue as you sold some of the harvested");
    if (notInHHTables($("#surveyPAPModal #hhCultureName option:selected").text(), "#surveyPAPModal #hhCultureSold"))
        errors.push("This Culture is already in the table. Please choose another Culture");
    if (notInHHTables(cultureDesc, "#surveyPAPModal #hhCultureSold"))
        errors.push("This Culture description is already used in the table. Please choose another Culture Description");
    

    if (errors.length >= 1) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#surveyPAPModal #papCultureError").html(errorMessage).show(250);
    }
    else {
        cultureDesc = (cultureDesc == "" || cultureDesc == null) ? culture : cultureDesc;
        let tr = '';
        tr += '<tr>' +
            '<td style="display:none">' + $("#surveyPAPModal #hhCultureName").val() + '</td>' +
            '<td>' + culture + '</td>' +
            '<td style="display:none">' + $.trim(cultureDesc) + '</td>' +
            '<td>' + cultureCAV + '</td>' +
            '<td>' + cultureArea + '</td>' +
            '<td>' + $("#surveyPAPModal #hhCultureHarvest").val() + '</td>' +
            '<td>' + $("#surveyPAPModal #hhCultureSold").val() + '</td>' +
            '<td>' + cultureRevenue + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeCulturesSold" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';

        $("#surveyPAPModal #hhCultureSold tbody").append(tr);
        $("#surveyPAPModal #hhCultureDesc, #surveyPAPModal #hhCultureArea, #surveyPAPModal #hhCultureArea, #surveyPAPModal #hhCultureSold, #surveyPAPModal #hhCultureHarvest, #surveyPAPModal #hhCultureRevenue").val('');
        $("#surveyPAPModal #hhCultureName").val('-1');
        $("#surveyPAPModal input[name='hhCultureCAV']").prop('checked', false);
        $("#surveyPAPModal #hhCultureName").focus();
    }
});

function convertHHTables(tableName) {
    var dataToPost = [];
    var tables = [];

    $(tableName +' tr').each(function () {
        let cols = [];
        let tableData = $(this).find('td');
        if (tableData.length > 0) {
            tableData.each(function () {
                cols.push($(this).text());
            });
            tables.push(cols);
        }
    });

    if (tableName == "#surveyPAPModal #hhTable") {
        if (tables.length > 0) {
            $.each(tables, function (index, cols) {
                let obj = {
                    FirstName: null,
                    LastName: null,
                    MiddleName: null,
                    Gender: null,
                    IdCard: null,
                    DateOfBirth: null,
                    FatherName: null,
                    MotherName: null,
                    VulnerabilityTypeId: null,
                    VulnerabilityTypeName: null,
                    VulnerabilityDetail: null,
                    MaritalStatus: null,
                    Relationship: null,
                    RelationshipName: null,
                    IsStudent: null,
                    EducationLevel: null,
                    EducationLevelName: null,
                    FrenchLevel: null,
                    FrenchLevelName: null,
                    PictureID: null,
                    Picture: null,
                    ResidenceName: null,
                    Activity1: null,
                    Activity1Name: null,
                    Activity2: null,
                    Activity2Name: null,
                    Competency1: null,
                    Competency1Name: null,
                    Competency2: null,
                    Competency2Name: null,
                    ParentPOB: null,
                    ParentPhone:null
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.FirstName = $.trim(cols[2]);
                    obj.LastName = $.trim(cols[4]);
                    obj.MiddleName = $.trim(cols[3]);
                    obj.Gender = cols[5];
                    obj.IdCard = $.trim(cols[8]);
                    obj.DateOfBirth = cols[7];
                    obj.VulnerabilityTypeId = cols[21];
                    obj.VulnerabilityTypeName = cols[22];
                    obj.VulnerabilityDetail = $.trim(cols[23]);
                    obj.MaritalStatus = cols[6];
                    obj.Relationship = cols[0];
                    obj.RelationshipName = cols[1];
                    obj.IsStudent = cols[9];
                    obj.EducationLevel = cols[10];
                    obj.EducationLevelName = cols[11];
                    obj.FrenchLevel = cols[12];
                    obj.FrenchLevelName = cols[13];
                    obj.PictureID = $.trim(cols[14]);
                    obj.Picture = cols[15];
                    obj.ResidenceName = cols[16];
                    obj.Activity1 = cols[17].includes(',') ? $.trim(cols[17].split(',')[0]) : $.trim(cols[17]);
                    obj.Activity1Name = cols[18].includes(',') ? $.trim(cols[18].split(',')[0]) : $.trim(cols[18]);
                    obj.Activity2 = cols[17].includes(',') ? $.trim(cols[17].split(',')[1]) : null;
                    obj.Activity2Name = cols[18].includes(',') ? $.trim(cols[18].split(',')[1]) : null;
                    obj.Competency1 = cols[19].includes(',') ? $.trim(cols[19].split(',')[0]) : $.trim(cols[19]);
                    obj.Competency1Name = cols[20].includes(',') ? $.trim(cols[20].split(',')[0]) : $.trim(cols[20]);
                    obj.Competency2 = cols[19].includes(',') ? $.trim(cols[19].split(',')[1]) : null;
                    obj.Competency2Name = cols[20].includes(',') ? $.trim(cols[20].split(',')[1]) : null;

                    if (cols[1] == 'HH Responsible') {
                        obj.FatherName = $('#surveyPAPModal #cdmFatherName').val();
                        obj.MotherName = $('#surveyPAPModal #cdmMotherName').val();
                        obj.ParentPOB = $('#surveyPAPModal #cdmParentPOB').val();
                        obj.ParentPhone = $('#surveyPAPModal #cdmParentPhone').val();
                    } else if (cols[1] == 'Spouse') {
                        obj.FatherName = $('#surveyPAPModal #spouseFatherName').val();
                        obj.MotherName = $('#surveyPAPModal #spouseMotherName').val();
                        obj.ParentPOB = $('#surveyPAPModal #spouseParentPOB').val();
                        obj.ParentPhone = $('#surveyPAPModal #spouseParentPhone').val();
                    }

                    dataToPost.push(obj);
                }
            })
        }
    }
    else if (tableName == "#surveyPAPModal #hhResidence") {
        if (tables.length > 0) {
            $.each(tables, function (index, cols) {
                let obj = {
                    ResidenceAddress: null,
                    RoofType: null,
                    RoofTypeString: null,
                    WallType: null,
                    WallTypeString: null,
                    FloorType: null,
                    FloorTypeString: null,
                    IsOwner: null,
                    IsPermanent: null,
                    Rent: null,
                    ArrivalDate: null,
                    ArrivalReason: null,
                    ArrivalReasonName: null,
                    ToiletType: null,
                    ToiletTypeName: null,
                    RoomQty: null
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.ResidenceAddress = $.trim(cols[0]);
                    obj.RoofType = cols[1];
                    obj.RoofTypeString = cols[2];
                    obj.WallType = cols[3];
                    obj.WallTypeString = cols[4];
                    obj.FloorType = cols[5];
                    obj.FloorTypeString = cols[6];
                    obj.IsOwner = (cols[7] == 'PU' || cols[7] == 'P');
                    obj.IsPermanent = (cols[8] == 'Permanent');
                    obj.Rent = cols[9];
                    obj.ArrivalDate = cols[10];
                    obj.ArrivalReason = cols[11];
                    obj.ArrivalReasonName = cols[12];
                    obj.ToiletType = cols[13];
                    obj.ToiletTypeName = cols[14];
                    obj.RoomQty = cols[15];

                    dataToPost.push(obj);
                }
            })
        }
    }
    else if (tableName == "#surveyPAPModal #hhRevenue") {
        if (tables.length > 0) {
            $.each(tables, function (index, cols) {
                var obj = {
                    RevenueSource: null,
                    RevenueSourceName: null,
                    RevenueAmount: null,
                    MonthsPerYear: null
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.RevenueSource = cols[0];
                    obj.RevenueSourceName = cols[1];
                    obj.RevenueAmount = cols[2];
                    obj.MonthsPerYear = cols[3];

                    dataToPost.push(obj);
                }
            }) 
        }
    }
    else if (tableName == "#surveyPAPModal #hhTFMCompensed") {
        if (tables.length > 0) {
            $.each(tables, function (index, cols) {
                var obj = {
                    Year: null,
                    Amount: null,
                    Surface: null
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.Year = cols[0];
                    obj.Amount = $.trim(cols[1]);
                    obj.Surface = $.trim(cols[2]);

                    dataToPost.push(obj);
                }
            })
        }
    }
    else if (tableName == "#surveyPAPModal #hhEquipment" || tableName == "#surveyPAPModal #hhAnimals") {
        if (tables.length > 0) {
            $.each(tables, function (index, cols) {
                var obj = {
                    Good: null,
                    GoodName: null,
                    GoodDescription: null,
                    Quantity: null,
                    GoodType: null,
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.Good = cols[0];
                    obj.GoodName = cols[1];
                    obj.GoodDescription = $.trim(cols[2]);
                    obj.Quantity = $.trim(cols[3]);
                    obj.GoodType = (tableName == "#surveyPAPModal #hhEquipment" ? "Equipment" : "Animal");

                    dataToPost.push(obj);
                }
            })
        }
    }
    else if (tableName == "#surveyPAPModal #hhCultures") {
        if (tables.length > 0) {
            $.each(tables, function (index, cols) {
                var obj = {
                    LandName: null,
                    DistanceFromResidence: null,
                    PropertyWorkerType: null,
                    PropertyWorkerTypeString: null,
                    CultivatedArea: null,
                    FallowArea: null,
                    IsOwner: null,
                    Rent: null,
                    PropertySourceName: null,
                    AcquisitionYear: null,
                    RegionName: null,
                    RegionId: null
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.LandName = $.trim(cols[0]);
                    obj.DistanceFromResidence = $.trim(cols[1]);
                    obj.PropertyWorkerType = cols[2].includes(',') ? cols[2].split(','): [cols[2]];
                    obj.PropertyWorkerTypeString = cols[3].includes(',') ? cols[3].split(',') : [cols[3]];
                    obj.CultivatedArea = cols[4];
                    obj.FallowArea = cols[5];
                    obj.IsOwner = (cols[6] == 'PU' || cols[6] == 'P');
                    obj.Rent = cols[7];
                    obj.PropertySourceName = cols[8];
                    obj.AcquisitionYear = cols[9];
                    obj.RegionId = $('#surveyPAPModal #surveyRegion').val();
                    obj.RegionName = $('#surveyPAPModal #surveyRegion option:selected').text();

                    dataToPost.push(obj);
                }
            })
        }
    }
    else if (tableName == "#surveyPAPModal #hhCultureSold") {
        if (tables.length > 0) {
            $.each(tables, function (index, cols) {
                var obj = {
                    CultureDiversityId: null,
                    CultureDiversity: null,
                    CultureDescription: null,
                    CultureAction: null,
                    Area: null,
                    HarvestLastSeason: null,
                    SoldLastSeason: null,
                    Revenue : null
                }

                if (cols[0] != "" && cols[0] != null) {
                    obj.CultureDiversityId = cols[0];
                    obj.CultureDiversity = cols[1];
                    obj.CultureDescription = $.trim(cols[2]);
                    obj.CultureAction = cols[3];
                    obj.Area = cols[4];
                    obj.HarvestLastSeason = cols[5];
                    obj.SoldLastSeason = cols[6];
                    obj.Revenue = cols[7];

                    dataToPost.push(obj);
                }
            })
        }
    }
    return dataToPost;
}

function notInHHTables(searchString, tableName) {
    var found = false;

    $(tableName + ' tr').each(function (row, tr) {
        let itemID = $(tr).children().eq(0).text();
        let itemName = $(tr).children().eq(1).text();
        let itemDesc = $(tr).children().eq(2).text();

        if (tableName == "#surveyPAPModal #hhTable") {
            let lastName = $(tr).children().eq(4).text();
            let photoID = $(tr).children().eq(14).text();
            let hhName = itemDesc + " " + lastName;
            if (((itemDesc == searchString && lastName == searchString) || photoID == searchString) || ((itemName == "HH Responsible" || itemName == "Spouse") && (itemName == searchString)) || hhName == searchString) {
                found = true;
                return false;
            }
        }
        else if (tableName == "#surveyPAPModal #hhRevenue") {
            if (itemID == searchString || itemName == searchString) {
                found = true;
                return false;
            }
        }
        else if (tableName == "#surveyPAPModal #hhEquipment" || tableName == "#surveyPAPModal #hhAnimals" || tableName == "#surveyPAPModal #hhCultureSold") {
            if (itemID == searchString || itemName == searchString || itemDesc == searchString) {
                found = true;
                return false;
            }
        } 
    });
    return found;
}

function savePAPSurvey() {
    $("#surveyPAPModal #loader").show();

    let surveyed = $('#surveyPAPModal #surveyCompleted').prop('checked');

    if (surveyed) {
        $.alert({
            icon: 'fa fa-spinner fa-spin',
            type: 'blue',
            theme: 'dark',
            title: 'Information',
            content: 'This PAP has been already surveyed on LAC ' + $("#surveyPAPModal #surveylacID option:selected").text(),
            backgroundDismiss: false,
            backgroundDismissAnimation: 'glow',
        });
    }
    else {
        //Loads tables
        let hhTable = convertHHTables('#surveyPAPModal #hhTable');
        let residences = convertHHTables('#surveyPAPModal #hhResidence');
        let revenues = convertHHTables('#surveyPAPModal #hhRevenue');
        let compensed = convertHHTables('#surveyPAPModal #hhTFMCompensed');
        let equipments = convertHHTables('#surveyPAPModal #hhEquipment');
        let animals = convertHHTables('#surveyPAPModal #hhAnimals');
        let goods = $.merge(equipments, animals);
        let fields = convertHHTables('#surveyPAPModal #hhCultures');
        let cultureSold = convertHHTables('#surveyPAPModal #hhCultureSold');
        let countPaid = $('#surveyPAPModal #hhCulturePaid').val();

        let socioElements = [];
        let tabletSource = [];
        
        if ($("#surveyPAPModal input[name='medecineLocation']:checked")) {
            $.each($("#surveyPAPModal input[name='medecineLocation']:checked"), function () {
                tabletSource.push($(this).val());
                let obj = {
                    SocioElementName: null,
                    SocioElementValue: null
                };
                obj.SocioElementName = "Tablet Source";
                obj.SocioElementValue = $(this).val();
                socioElements.push(obj);
            });
        }
        else {
            let obj = {
                SocioElementName: null,
                SocioElementValue: null
            };
            tabletSource.push("No Consult/Nothing Paid");
            obj.SocioElementName = "Tablet Source";
            obj.SocioElementValue = "No Consult/Nothing Paid";
            socioElements.push(obj);
        }   

        let lastWeekDesease = [];
        if ($("#surveyPAPModal input[name='hhChildren']:checked")) {
            $.each($("#surveyPAPModal input[name='hhChildren']:checked"), function () {
                let obj = {
                    SocioElementName: null,
                    SocioElementValue: null
                };
                lastWeekDesease.push($(this).val());
                obj.SocioElementName = "Last Week Desease";
                obj.SocioElementValue = $(this).val();
                socioElements.push(obj);
            });
        }
        else
        {
            let obj = {
                SocioElementName: null,
                SocioElementValue: null
            };
            lastWeekDesease.push("Nothing");
            obj.SocioElementName = "Last Week Desease";
            obj.SocioElementValue = "Nothing";
            socioElements.push(obj);
        }  

        let mosquitoSource = [];
        if ($("#surveyPAPModal input[name='hhMosquito']:checked")) {
            $.each($("#surveyPAPModal input[name='hhMosquito']:checked"), function () {
                let obj = {
                    SocioElementName: null,
                    SocioElementValue: null
                };
                mosquitoSource.push($(this).val());
                obj.SocioElementName = "Mosquito Source";
                obj.SocioElementValue = $(this).val();
                socioElements.push(obj);
            });
        }
        else
        {
            let obj = {
                SocioElementName: null,
                SocioElementValue: null
            };
            mosquitoSource.push("No");
            obj.SocioElementName = "Mosquito Source";
            obj.SocioElementValue = "No";
            socioElements.push(obj);
        }  

        let mosquitoUsers = [];
        if ($("#surveyPAPModal input[name='hhMosquitoUsage']:checked")) {
            $.each($("#surveyPAPModal input[name='hhMosquitoUsage']:checked"), function () {
                let obj = {
                    SocioElementName: null,
                    SocioElementValue: null
                };
                mosquitoUsers.push($(this).val());
                obj.SocioElementName = "Mosquito User";
                obj.SocioElementValue = $(this).val();
                socioElements.push(obj);
            });
        }
        else
        {
            let obj = {
                SocioElementName: null,
                SocioElementValue: null
            };
            mosquitoUsers.push("None");
            obj.SocioElementName = "Mosquito User";
            obj.SocioElementValue = "None";
            socioElements.push(obj);
        }

        let hhParticipations = [];
        if ($("#surveyPAPModal input[name='hhParticipation']:checked")) {
            $.each($("#surveyPAPModal input[name='hhParticipation']:checked"), function () {
                let obj = {
                    SocioElementName: null,
                    SocioElementValue: null
                };
                hhParticipations.push($(this).val());
                obj.SocioElementName = "Saving Type";
                obj.SocioElementValue = $(this).val();
                socioElements.push(obj);
            });
        }
        else
        {
            let obj = {
                SocioElementName: null,
                SocioElementValue: null
            };
            hhParticipations.push("None");
            obj.SocioElementName = "Saving Type";
            obj.SocioElementValue = "None";
            socioElements.push(obj);
        } 

        let selectedTools = $('#surveyPAPModal #hhCultureTool').select2("data");
        let Tools = [];

        for (var i = 0; i <= selectedTools.length - 1; i++) {
            Tools.push(selectedTools[i].text);
        }

        let pap = {
            PAPId: $("#surveyPAPModal #papId").val(),
            PAPLACId: $("#surveyPAPModal #paplacId").val(),
            SurveyDate: $('#surveyPAPModal #surveyDate').val(),
            SurveyorCode: $('#surveyPAPModal #surveyCode').val(),
            PresurveyorCamera: $('#surveyPAPModal #surveyCamera').val(),
            FileNumber: $("#surveyPAPModal #fileID").val(),
            LACId: $("#surveyPAPModal #lacId").val(),
            SurveyedBefore: surveyed,
            SurveyedFileID: $("#surveyPAPModal #surveyfileID").val(),
            SurveyedLacID: $("#surveyPAPModal #surveylacID").val()
        };

        let hh = {
            ResidenceAddress: $('#surveyPAPModal #surveyVillage option:selected').text(),
            ResidenceId: $('#surveyPAPModal #surveyVillage').val(),
            HouseHoldNumber: $("#surveyPAPModal #fileID").val(),
            PreviouslyCompensated: $('#surveyPAPModal #hhNeverCompensed').prop('checked'),
            CompensationUse: $('#surveyPAPModal #hhCompensedMoneyUtilisation').val(), 
            LastWeekExpense: $('#surveyPAPModal #hhLastWeekExpense').val(),
            FishOrMeat: $('#surveyPAPModal #hhFishMeat').prop('checked'),
            EnoughFood: $('#surveyPAPModal #hhEnoughFood').prop('checked'),
            SkinDesease: $('#surveyPAPModal #hhChildSkin').prop('checked'),
            SocioElements: socioElements,
            SavingTypeName: hhParticipations,
            MosquitoNetUserName: mosquitoUsers,
            MosquitoNetSourceName: mosquitoSource,
            LastWeekDeseaseName: lastWeekDesease,
            TabletsSourceName: tabletSource,
            PaidWorkersQty: (countPaid != "" ? countPaid : 0),
            PaidWorkersTime: (countPaid != "" ? $('#surveyPAPModal input[name="hhCulturePaidTime"]:checked').val() : ""),
            CultureTool: $('#surveyPAPModal #hhCultureTool').val(),
            CultureTools: Tools,
            Comments: $('#surveyPAPModal #hhComments').val(),
            NoRevenue: $('#surveyPAPModal #hhNoneRevenue').prop('checked'),
            NoEquipment: $('#surveyPAPModal #hhNoEquipment').prop('checked'),
            NoAnimal: $('#surveyPAPModal #hhNoAnimal').prop('checked'),
            InterviewedName: $('#surveyPAPModal #interviewedName').val(),
            InterviewedRelationship: $('#surveyPAPModal input[name="interviewedType"]:checked').val()
        };

        let hhFile = $("#surveyPAPModal #hhFile")[0];
        let hhScanFile = hhFile.files;

        if (hhFile.files.length > 0)
            formData.append(hhScanFile[0].name, hhScanFile[0]);

        formData.append("PAPLAC", JSON.stringify(pap));
        formData.append("Household", JSON.stringify(hh));
        formData.append("Members", JSON.stringify(hhTable));
        formData.append("Revenues", JSON.stringify(revenues));
        formData.append("Residences", JSON.stringify(residences));
        formData.append("Properties", JSON.stringify(fields));
        formData.append("Goods", JSON.stringify(goods));
        formData.append("Compensations", JSON.stringify(compensed));
        formData.append("Culture", JSON.stringify(cultureSold));

        //console.log(fields);

        $.ajax({
            type: "POST",
            url: "/Community/PAPLACSurvey/",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                $("#surveyPAPModal #loader").hide();
                if (response[0] == "Success") {
                    $("#surveyPAPModal").modal('hide');
                    $.notify(response[1], "success");
                    LoadLACPAPList();
                } else if (response[0] == "Error") {
                    var errorMessage = "<ul>";

                    $.each(response.slice(1), function (index, value) {
                        errorMessage += "<li>" + value + "</li>";
                    });
                    errorMessage += "</ul>";
                    $("#surveyPAPModal #papCultureSoldError").html(errorMessage).show(250);
                } else {
                    $.notify("An error occured while inserting Socio-Economic Survey result", "error");
                    console.log(response[1]);
                }
                console.log(response);
            }
        });
        return true;
    }
}
//#endregion

//#region COLLECT LAND DATA
$("#collectLandModal .reset").on("focus", function () { $("#collectLandModal #tableGoodsError").hide(); });
$("#collectLandModal .reset").on("change", function () { $("#collectLandModal #tableGoodsError").hide(); });

$("#collectLandModal .topo").on("focus", function () { $("#collectLandModal #papSurveyorError").hide(); });
$("#collectLandModal .topo").on("change", function () { $("#collectLandModal #papSurveyorError").hide(); });

$("#collectLandModal #goodType").on("change", function () {
    let assetType = $("#collectLandModal #goodType option:selected").text();
    $("#collectLandModal #cultureDetails, #collectLandModal #structureDetails, #collectLandModal #treeDetails").hide();
    if (assetType == "Culture")
        $("#collectLandModal #cultureDetails").show();
    if (assetType == "Structure")
        $("#collectLandModal #structureDetails").show();
    if (assetType == "Tree")
        $("#collectLandModal #treeDetails").show();
});

$("#collectLandModal #ownershipType").on("change", function () {
    if ($(this).prop('checked'))
        $("#collectLandModal #ownerDetails").show();
    else
        $("#collectLandModal #ownerDetails").hide();
});

$("#collectLandModal").on("click", "#addCulture", function () {
    var errors = [];
    var user = $('#collectLandModal #ownershipType').prop('checked') ? 'U' : 'PU';

    if ($('#collectLandModal #gpsID').val() == null || $('#collectLandModal #gpsID').val() == "")
        errors.push("Please specify the GPS point");
    if ($('#collectLandModal #pointEasting').val() == null || $('#collectLandModal #pointEasting').val() == "")
        errors.push("Please specify Point Easting");
    if ($('#collectLandModal #pointNorthing').val() == null || $('#collectLandModal #pointNorthing').val() == "")
        errors.push("Please specify Point Northing");
    if ($('#collectLandModal #goodPictureID').val() == null || $('#collectLandModal #goodPictureID').val() == "")
        errors.push("Please add the picture ID");
    if (isNaN($('#collectLandModal #goodType').val()) || $('#collectLandModal #goodType').val() == "")
        errors.push("Please specify a valid property type");
    if ($('#collectLandModal #goodPhoto').val() == null || $('#collectLandModal #goodPhoto').val() == "")
        errors.push("You did not put Picture");
    if (notInCollectTables($('#collectLandModal #propertyID').val(), "Property") == true)
        errors.push("This property is already edited");
    if (user == 'U' && ($('#collectLandModal #ownerFirstName').val() == null || $('#collectLandModal #ownerFirstName').val() == ""))
        errors.push("Please specify Owner First Name");
    if (user == 'U' && ($('#collectLandModal #ownerLastName').val() == null || $('#collectLandModal #ownerLastName').val() == ""))
        errors.push("Please specify Owner Last Name");

    if ($('#collectLandModal #fieldType').val() == null || $('#collectLandModal #fieldType').val() == "")
        errors.push("Please choose the Field Type");
    if ($('#collectLandModal #cultureName').val() == null || $('#collectLandModal #cultureName').val() == "")
        errors.push("Please enter culture name");
    if (isNaN($('#collectLandModal #cultureSurface').val()) || $('#collectLandModal #cultureSurface').val() == "")
        errors.push("Please specify the surface");
    if ($("#collectLandModal #cultureSurfaceUOM").val() < 1 || $("#collectLandModal #cultureSurfaceUOM").val() == "")
        errors.push("You did not define surface UOM");
    if ($('#collectLandModal #cultureGpsID').val() == null || $('#collectLandModal #cultureGpsID').val() == "")
        errors.push("Please specify the GPS point");
    //if ($('#collectLandModal #cultureEasting').val() == null || $('#collectLandModal #cultureEasting').val() == "")
    //    errors.push("Please specify Point Easting");
    //if ($('#collectLandModal #cultureNorthing').val() == null || $('#collectLandModal #cultureNorthing').val() == "")
    //    errors.push("Please specify Point Northing");
    if ($('#collectLandModal #cultureSince').val() == null || $('#collectLandModal #cultureSince').val() == "")
        errors.push("Please specify since when we have the culture");

    let goodPhoto = $("#collectLandModal #goodPhoto")[0];
    let goodPhotoFile = goodPhoto.files;

    if (goodPhoto.files.length > 0)
        formData.append(goodPhotoFile[0].name, goodPhotoFile[0]);

    let startYear = new Date($('#collectLandModal #cultureSince').val(), 0).toLocaleDateString();
    console.log(startYear);

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        if (notInCollectTables($('#collectLandModal #propertyID').val(), "Property") == true) {
            let table = $("#collectLandModal #tablePAPProperties").DataTable();
            table.$('tr.selected').removeClass('selected');

            $('#collectLandModal #gpsID, #collectLandModal #goodPictureID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #pointElevation, #collectLandModal #goodPhoto, #collectLandModal #cultureName, #collectLandModal #cultureSurface, #collectLandModal #ownerFirstName, #collectLandModal #ownerMiddleName, #collectLandModal #ownerLastName, #collectLandModal #goodComments').val('');
            $("#collectLandModal #goodType, #collectLandModal #fieldType").val("-1");
            $('#collectLandModal #goodPhoto, #collectLandModal #culturePointArray').fileinput('clear');
            $('#collectLandModal #ownershipType').prop("checked", false);
            $("#collectLandModal .hiden").hide();

            $("#collectLandModal #gpsID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #goodPictureID, #collectLandModal #goodType, #collectLandModal #goodComments, #collectLandModal #goodPhoto, #collectLandModal #ownershipType").prop('disabled', true);
        }
        $("#collectLandModal #tableGoodsError").html(errorMessage).show(250);
    }
    else {
        var tr = "";
        tr += '<tr>' +
            '<td style="display:none">' + $('#collectLandModal #propertyID').val() + '</td>' +
            '<td>' + $('#collectLandModal #gpsID').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #pointEasting').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #pointNorthing').val() + '</td>' +
            '<td>' + $('#collectLandModal #goodPictureID').val() + '</td>' +
            '<td style="display:none"> ' + $('#collectLandModal #goodPhoto').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #goodType').val() + '</td>' +
            '<td>' + $('#collectLandModal #goodType option:selected').text() + '</td>' +
            '<td>' + $('#collectLandModal #cultureName').val() + '</td>' +
            '<td>' + user + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerPersonId').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerFirstName').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerMiddleName').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerLastName').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #cultureSurface').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #cultureGpsID').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #cultureEasting').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #cultureNorthing').val() + '</td>' +
            '<td style="display:none">' + startYear + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #fieldType').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #fieldType option:selected').text() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#collectLandModal #goodComments').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #cultureSurfaceUOM').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #cultureSurfaceUOM option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #topographCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #topoDate').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #topoTrimbleCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #trimbleFile').val() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeGoods" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';
        $("#propertiesTable tbody").append(tr);
        $('#collectLandModal #gpsID, #collectLandModal #goodPictureID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #pointElevation, #collectLandModal #goodPhoto, #collectLandModal #cultureName, #collectLandModal #cultureSurface, #collectLandModal #ownerFirstName, #collectLandModal #ownerMiddleName, #collectLandModal #ownerLastName, #collectLandModal #goodComments').val('');
        $("#collectLandModal #goodType, #collectLandModal #fieldType").val("-1");
        $('#collectLandModal #goodPhoto').fileinput('clear');
        $('#collectLandModal #ownershipType').prop("checked", false);
        $("#collectLandModal .hiden").hide();

        $("#collectLandModal #gpsID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #goodPictureID, #collectLandModal #goodType, #collectLandModal #goodComments, #collectLandModal #goodPhoto, #collectLandModal #ownershipType").prop('disabled', true);
    }
    return false;
});

$("#collectLandModal").on("click", "#addStructure", function () {
    var errors = [];
    var user = $('#collectLandModal #ownershipType').prop('checked') ? 'U' : 'PU';

    if ($('#collectLandModal #gpsID').val() == null || $('#collectLandModal #gpsID').val() == "")
        errors.push("Please specify the GPS point");
    if ($('#collectLandModal #pointEasting').val() == null || $('#collectLandModal #pointEasting').val() == "")
        errors.push("Please specify Point Easting");
    if ($('#collectLandModal #pointNorthing').val() == null || $('#collectLandModal #pointNorthing').val() == "")
        errors.push("Please specify Point Northing");
    if ($('#collectLandModal #goodPictureID').val() == null || $('#collectLandModal #goodPictureID').val() == "")
        errors.push("Please add the picture ID");
    if (isNaN($('#collectLandModal #goodType').val()) || $('#collectLandModal #goodType').val() == "")
        errors.push("Please specify a valid property type");
    if ($('#collectLandModal #goodPhoto').val() == null || $('#collectLandModal #goodPhoto').val() == "")
        errors.push("You did not put Picture ID");
    if (notInCollectTables($('#collectLandModal #propertyID').val(), "Property") == true)
        errors.push("This property is already edited");
    if (user == 'U' && ($('#collectLandModal #ownerFirstName').val() == null || $('#collectLandModal #ownerFirstName').val() == ""))
        errors.push("Please specify Owner First Name");
    if (user == 'U' && ($('#collectLandModal #ownerLastName').val() == null || $('#collectLandModal #ownerLastName').val() == ""))
        errors.push("Please specify Owner Last Name");

    if ($("#collectLandModal #structureCode").val() == "" || $("#collectLandModal #structureCode").val() == null || isNaN($("#collectLandModal #structureCode").val()))
        errors.push("Structure Code can not be empty");
    if ($("#collectLandModal #toitCode").val() == "" || $("#collectLandModal #toitCode").val() == null || isNaN($("#collectLandModal #toitCode").val()))
        errors.push("Roof Code can not be empty");
    if ($("#collectLandModal #murCode").val() == "" || $("#collectLandModal #murCode").val() == null || isNaN($("#collectLandModal #murCode").val()))
        errors.push("Wall Code can not be empty");
    if ($("#collectLandModal #solCode").val() == "" || $("#collectLandModal #solCode").val() == null || isNaN($("#collectLandModal #solCode").val()))
        errors.push("Floor Code can not be empty");
    if ($("#collectLandModal #structureLength").val() == "" || $("#collectLandModal #structureLength").val() == null)
        errors.push("Structure surface can't be empty");
    if ($("#collectLandModal #structureWidth").val() == "" || $("#collectLandModal #structureWidth").val() == null)
        errors.push("Structure surface can't be empty");
    if ($("#collectLandModal #structurePiece").val() == "" || $("#collectLandModal #structurePiece").val() == null || isNaN($("#collectLandModal #structurePiece").val()))
        errors.push("Specify the number of rooms");

    let goodPhoto = $("#collectLandModal #goodPhoto")[0];
    let goodPhotoFile = goodPhoto.files;

    if (goodPhoto.files.length > 0)
        formData.append(goodPhotoFile[0].name, goodPhotoFile[0]);

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";

        if (notInCollectTables($('#collectLandModal #propertyID').val(), "Property") == true) {
            let table = $("#collectLandModal #tablePAPProperties").DataTable();
            table.$('tr.selected').removeClass('selected');

            $('#collectLandModal #gpsID, #collectLandModal #goodPictureID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing,  #collectLandModal #goodPhoto, #collectLandModal #ownerFirstName, #collectLandModal #ownerMiddleName, #collectLandModal #ownerLastName, #collectLandModal #goodComments, #collectLandModal #structureLength, #collectLandModal #structureWidth, #collectLandModal #structurePiece').val('');
            $("#collectLandModal #goodType, #collectLandModal #structureCode, #collectLandModal #toitCode, #collectLandModal #murCode, #collectLandModal #solCode").val("-1");
            $('#collectLandModal #goodPhoto').fileinput('clear');
            $('#collectLandModal #ownershipType').prop("checked", false);
            $("#collectLandModal .hiden").hide();

            $("#collectLandModal #gpsID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #goodPictureID, #collectLandModal #goodType, #collectLandModal #goodComments, #collectLandModal #goodPhoto, #collectLandModal #ownershipType").prop('disabled', true);
        }

        $("#collectLandModal #tableGoodsError").html(errorMessage).show(250);
    }
    else {
        var tr = "";
        tr += '<tr>' +
            '<td style="display:none">' + $('#collectLandModal #propertyID').val() + '</td>' +
            '<td>' + $('#collectLandModal #gpsID').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #pointEasting').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #pointNorthing').val() + '</td>' +
            '<td>' + $('#collectLandModal #goodPictureID').val() + '</td>' +
            '<td style="display:none"> ' + $('#collectLandModal #goodPhoto').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #goodType').val() + '</td>' +
            '<td>' + $('#collectLandModal #goodType option:selected').text() + '</td>' +
            '<td>' + $('#collectLandModal #structureName').val() + '</td>' +
            '<td>' + user + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerPersonId').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerFirstName').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerMiddleName').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerLastName').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#collectLandModal #structureCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #structureCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #toitCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #toitCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #murCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #murCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #solCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #solCode option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #structureLength').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #structureWidth').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #structurePiece').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#collectLandModal #goodComments').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#collectLandModal #topographCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #topoDate').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #topoTrimbleCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #trimbleFile').val() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeGoods" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';
        $("#propertiesTable tbody").append(tr);
        $('#collectLandModal #gpsID, #collectLandModal #goodPictureID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing,  #collectLandModal #goodPhoto, #collectLandModal #ownerFirstName, #collectLandModal #ownerMiddleName, #collectLandModal #ownerLastName, #collectLandModal #goodComments, #collectLandModal #structureLength, #collectLandModal #structureWidth, #collectLandModal #structurePiece').val('');
        $("#collectLandModal #goodType, #collectLandModal #structureCode, #collectLandModal #toitCode, #collectLandModal #murCode, #collectLandModal #solCode").val("-1");
        $('#collectLandModal #goodPhoto').fileinput('clear');
        $('#collectLandModal #ownershipType').prop("checked", false);
        $("#collectLandModal .hiden").hide();
        $('#collectLandModal #gpsID').focus();

        $("#collectLandModal #gpsID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #goodPictureID, #collectLandModal #goodType, #collectLandModal #goodComments, #collectLandModal #goodPhoto, #collectLandModal #ownershipType").prop('disabled', true);
    }
    
    return false;
});

$("#collectLandModal").on("click", "#addTree", function () {
    var errors = [];
    var user = $('#collectLandModal #ownershipType').prop('checked') ? 'U' : 'PU';

    if ($('#collectLandModal #gpsID').val() == null || $('#collectLandModal #gpsID').val() == "")
        errors.push("Please specify the GPS point");
    if ($('#collectLandModal #pointEasting').val() == null || $('#collectLandModal #pointEasting').val() == "")
        errors.push("Please specify Point Easting");
    if ($('#collectLandModal #pointNorthing').val() == null || $('#collectLandModal #pointNorthing').val() == "")
        errors.push("Please specify Point Northing");
    if ($('#collectLandModal #goodPictureID').val() == null || $('#collectLandModal #goodPictureID').val() == "")
        errors.push("Please add the picture ID");
    if (isNaN($('#collectLandModal #goodType').val()) || $('#collectLandModal #goodType').val() == "")
        errors.push("Please specify a valid property type");
    if ($('#collectLandModal #goodPhoto').val() == null || $('#collectLandModal #goodPhoto').val() == "")
        errors.push("You did not put Picture ID");
    if (notInCollectTables($('#collectLandModal #propertyID').val(), "Property") == true)
        errors.push("This property is already edited");
    if (user == 'U' && ($('#collectLandModal #ownerFirstName').val() == null || $('#collectLandModal #ownerFirstName').val() == ""))
        errors.push("Please specify Owner First Name");
    if (user == 'U' && ($('#collectLandModal #ownerLastName').val() == null || $('#collectLandModal #ownerLastName').val() == ""))
        errors.push("Please specify Owner Last Name");

    if ($('#collectLandModal #treeName').val() == null || $('#collectLandModal #treeName').val() == "")
        errors.push("Please specify the tree name");
    if (isNaN($('#collectLandModal #treeQty').val()) || $('#collectLandModal #treeQty').val() == "")
        errors.push("Please specify a valid count number of trees");

    let goodPhoto = $("#collectLandModal #goodPhoto")[0];
    let goodPhotoFile = goodPhoto.files;

    if (goodPhoto.files.length > 0)
        formData.append(goodPhotoFile[0].name, goodPhotoFile[0]);

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";

        if (notInCollectTables($('#collectLandModal #propertyID').val(), "Property") == true) {
            let table = $("#collectLandModal #tablePAPProperties").DataTable();
            table.$('tr.selected').removeClass('selected');

            $('#collectLandModal #gpsID, #collectLandModal #goodPictureID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing,  #collectLandModal #goodPhoto, #collectLandModal #ownerFirstName, #collectLandModal #ownerMiddleName, #collectLandModal #ownerLastName, #collectLandModal #goodComments, #collectLandModal #treeName').val('');
            $("#collectLandModal #goodType, #collectLandModal #treeMaturity").val("-1");
            $('#collectLandModal #goodPhoto').fileinput('clear');
            $('#collectLandModal #ownershipType').prop("checked", false);
            $("#collectLandModal .hiden").hide();

            $("#collectLandModal #gpsID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #goodPictureID, #collectLandModal #goodType, #collectLandModal #goodComments, #collectLandModal #goodPhoto, #collectLandModal #ownershipType").prop('disabled', true);
        }

        $("#collectLandModal #tableGoodsError").html(errorMessage).show(250);
        
    }
    else {
        var tr = "";
        tr += '<tr>' +
            '<td style="display:none">' + $('#collectLandModal #propertyID').val() + '</td>' +
            '<td>' + $('#collectLandModal #gpsID').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #pointEasting').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #pointNorthing').val() + '</td>' +
            '<td>' + $('#collectLandModal #goodPictureID').val() + '</td>' +
            '<td style="display:none"> ' + $('#collectLandModal #goodPhoto').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #goodType').val() + '</td>' +
            '<td>' + $('#collectLandModal #goodType option:selected').text() + '</td>' +
            '<td>' + $('#collectLandModal #treeName').val() + '</td>' +
            '<td>' + user + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerPersonId').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerFirstName').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerMiddleName').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #ownerLastName').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#collectLandModal #treeMaturity').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #treeMaturity option:selected').text() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #treeQty').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #goodComments').val() + '</td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none"></td>' +
            '<td style="display:none">' + $('#collectLandModal #topographCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #topoDate').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #topoTrimbleCode').val() + '</td>' +
            '<td style="display:none">' + $('#collectLandModal #trimbleFile').val() + '</td>' +
            '<td>' + '<a href="#" class="btn btn-danger" id="removeGoods" onclick="removeRow(this);" title="Remove"><i class="fas fa-trash-alt danger"></i></a> ' + '</td>' +
            '</tr>';
        $("#propertiesTable tbody").append(tr);
        $('#collectLandModal #gpsID, #collectLandModal #goodPictureID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing,  #collectLandModal #goodPhoto, #collectLandModal #ownerFirstName, #collectLandModal #ownerMiddleName, #collectLandModal #ownerLastName, #collectLandModal #goodComments, #collectLandModal #treeName').val('');
        $("#collectLandModal #goodType, #collectLandModal #treeMaturity").val("-1");
        $('#collectLandModal #goodPhoto').fileinput('clear');
        $('#collectLandModal #ownershipType').prop("checked", false);
        $("#collectLandModal .hiden").hide();
        $('#collectLandModal #gpsID').focus();

        $("#collectLandModal #gpsID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #goodPictureID, #collectLandModal #goodType, #collectLandModal #goodComments, #collectLandModal #goodPhoto, #collectLandModal #ownershipType").prop('disabled', true);
    }
    
    return false;
});

$("#collectLandModal #tablePAPProperties tbody").on("click", "tr", function () {
    let table = $("#collectLandModal #tablePAPProperties").DataTable();
    if ($(this).hasClass('selected')) {
        $(this).removeClass('selected');

        $("#collectLandModal #gpsID, #collectLandModal #pointEasting, #collectLandModal #pointNorthing, #collectLandModal #goodPictureID, #collectLandModal #goodType, #collectLandModal #goodComments, #collectLandModal #goodPhoto, #collectLandModal #ownershipType").prop('disabled', true);

        $("#collectLandModal #cultureDetails, #collectLandModal #structureDetails, #collectLandModal #treeDetails, #collectLandModal #ownerDetails").hide();
    }
    else {
        table.$('tr.selected').removeClass('selected');
        $(this).addClass('selected');

        let rowsIndex = table.row(this).index();
        let propertyID = table.cell(rowsIndex, 0).data();
        let propertyType = table.cell(rowsIndex, 3).data();
        let papID = $("#collectLandModal #collectPapID").val();
        $("#collectLandModal #goodType").prop('disabled', true);

        $("#collectLandModal input:radio[name='owner']").prop('checked', false);

        LoadPropertyDetails(propertyID, propertyType, papID);
    }
});

function notInCollectTables(searchString, column) {
    var found = false;
    $('#propertiesTable tr').each(function (row, tr) {
        var propertyID = $(tr).children().eq(0).text();
        var gpsID = $(tr).children().eq(1).text();
        var photoID = $(tr).children().eq(4).text();
        if (gpsID == searchString && column == "GPS") {
            found = true;
            return false;
        }
        if (photoID == searchString && column == "Picture") {
            found = true;
            return false;
        }
        if (propertyID == searchString && column == "Property") {
            found = true;
            return false;
        }
    });
    return found;
}

function insertLandData() {
    let lacID = $("#collectLandModal #collectLacID").val();
    let papID = $("#collectLandModal #collectPapID").val();
    let fileID = $("#collectLandModal #collectFileID").val();
    let houseHoldID = $("#collectLandModal #collectHouseholdID").val();

    let firstName = $('#collectLandModal #collectFirstName').val();
    let lastName = $('#collectLandModal #collectLastName').val();
    let pictureID = $('#collectLandModal #collectPictureID').val();
    let email = $('#collectLandModal #collectEmail').val();
    let primaryResidenceName = $('#collectLandModal #collectPrimaryResidence').val();
    let dob = $('#collectLandModal #collectDob').val();
    
    let gender = $('#collectLandModal input[name="gender"]:checked').val();

    var propertiesArray = convertTableToArray("#collectLandModal #propertiesTable");
    
    let errors = [];
    if (propertiesArray.length < 1)
        errors.push("You need to add at least one properties for this PAP");

    if (errors.length > 0) {
        var errorMessage = "<ul>";

        $.each(errors, function (index, value) {
            errorMessage += "<li>" + value + "</li>";
        });
        errorMessage += "</ul>";
        $("#collectLandModal #papSurveyorError").html(errorMessage).show(250);
        return false;
    }
    else {
        $("#collectLandModal #loader").show();
        let pap = {
            PersonId: $("#collectLandModal #collectPersonID").val(),
            PAPId: papID,
            FirstName: firstName,
            LastName: lastName,
            MiddleName: $('#collectLandModal #collectMiddleName').val(),
            Mobile: $('#collectLandModal #collectCellPhone').val(),
            Email: email,
            Gender: gender,
            DateOfBirth: dob,
            PlaceOfBirth: $('#collectLandModal #collectPob').val(),
            FatherName: $('#collectLandModal #collectPapfather').val(),
            MotherName: $('#collectLandModal #collectPapMother').val(),
            VulnerabilityTypeId: $('#collectLandModal #collectVulnerabilityType').val(),
            VulnerabilityTypeName: $('#collectLandModal #collectVulnerabilityType option:selected').text(),
            VulnerabilityDetail: $('#collectLandModal #collectVulnerabilityDetails').val(),
            PhotoID: pictureID,
            SpouseName: $('#collectLandModal #collectPapSpouse').val(),
            IdCard: $('#collectLandModal #collectCardNumber').val(),
            ResidenceName: primaryResidenceName,
            Picture: $('#collectLandModal #collectPAPphoto').val(),
            //PAPFile: $("#collectLandModal #collectTrimble").val(),
            LACId: lacID,
            HouseHoldId: houseHoldID,
            FileNumber: fileID
        };

        console.log(pap);
        console.log(propertiesArray);

        let papPhoto = $("#collectLandModal #collectPAPphoto")[0];
        let papPhotoFile = papPhoto.files;

        if (papPhoto.files.length > 0)
            formData.append(papPhotoFile[0].name, papPhotoFile[0]);

        let trimble = $("#collectLandModal #collectTrimble")[0];
        let trimbleFile = trimble.files;

        if (trimble.files.length > 0)
            formData.append(trimbleFile[0].name, trimbleFile[0]);

        let topoDoc = $("#collectLandModal #hhFile")[0];
        let topoFile = topoDoc.files;

        if (topoDoc.files.length > 0)
            formData.append(topoFile[0].name, topoFile[0]);

        formData.append("PAPLAC", JSON.stringify(pap));
        formData.append("Properties", JSON.stringify(propertiesArray));

        $.ajax({
            type: "POST",
            url: "/Community/CollectLandData/",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                $("#collectLandModal #loader").hide();
                if (response[0] == "Success") {
                    LoadLACPAPList();
                    $("#collectLandModal").modal('hide');
                    $.notify(response[1], "success");
                } else if (response[0] == "Error") {
                    let errorMessage = "<ul>";

                    $.each(response.slice(1), function (index, value) {
                        errorMessage += "<li>" + value + "</li>";
                    });
                    errorMessage += "</ul>";
                    $("#collectLandModal #papSurveyorError").html(errorMessage).show(250);
                } else {
                    $.notify("An error occured while Updating PAP with property", "error");
                    console.log(response[1]);
                }
                console.log(response);
            }
        });
        return true;
    }
}
//#endregion
//#endregion