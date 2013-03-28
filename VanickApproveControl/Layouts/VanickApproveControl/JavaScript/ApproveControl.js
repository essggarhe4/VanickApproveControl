var ApprovePageData;
var ApprovePagelog;
var approvePageStatus;
var isPageAuthor;
var isPageApprover;
var currentPageID;
var currentPageURL;
var currentListName;
var currentListApproval;
var currentPageVersion;
var currentemailAuthor;
var currentPageName;
var currentuserName;
var isAgreeResponse = true;

var notifyId = '';
var currentmessageinitem = '';

$(document).ready(function () {
    SetPageData();
});

function SetPageData() {
    var tmpjson = JSON.stringify(ApprovePageData)
    ApprovePagelog = JSON.parse(tmpjson); ;
    createApproveControls();
}

function createApproveControls() {
    if (isPageAuthor) {
        if (approvePageStatus == "Draft") {
            $("#s4-workspace").prepend(createHTMLApproveControl("Creator-Draft"));            
            $("#vanicktabs").tabs({
                collapsible: true,
                selected: -2
            });
        }
        else if (approvePageStatus == "Approved") {
            /*$("#s4-workspace").prepend(createHTMLApproveControl("Creator-Approved"));
            $("#vanicktabs").tabs({
                collapsible: true,
                selected: -2
            });
            */
            //setMessageAppove("Status: Your page has been approved");
        }
        else if (approvePageStatus == "Pending") {
            $("#s4-workspace").prepend(createHTMLApproveControl("Creator-Pending"));
            $("#vanicktabs").tabs({
                collapsible: true,
                selected: -2
            });
            //setMessageAppove("Status: Waiting for approval");
        }
        
    }
    else {
        if (approvePageStatus == "Approved") {
            var currentIteminArray;
            var paintcolor = false;
            if (ApprovePagelog.length > 0) {
                var orderpage = ApprovePagelog.sort(function (a, b) {
                    if (a.Version < b.Version)
                        return -1;
                    if (a.Version > b.Version)
                        return 1;
                    return 0;
                });
                currentIteminArray = orderpage[orderpage.length - 1];
                var lastversion = currentIteminArray.Version;
                currentmessageinitem = currentIteminArray.Comments;
                if (currentPageVersion == lastversion) {
                    $("#s4-workspace").prepend(createHTMLApproveControl("User-Approved-Agree"));
                    paintcolor = true;                  
                }
                else {
                    $("#s4-workspace").prepend(createHTMLApproveControl("User-Approved-new-version"));
                }
                
            }
            else {
                $("#s4-workspace").prepend(createHTMLApproveControl("User-Approved"));
            }
            $("#vanicktabs").tabs({
                collapsible: true,
                selected: -2
            });
            if (paintcolor) {
                if (currentIteminArray != undefined && currentIteminArray.Status == "Agree") {
                    $("ul.ui-widget-header").css('background-color', 'green');
                }
                else {
                    $("ul.ui-widget-header").css('background-color', 'red');
                }
            }
        }        
    }
    SetVanickApproveControlEvents();
}
    
    function createHTMLApproveControl(controlMode) {
        var approvecontrol = "<div class='vanick-approve-control' id='vanick-approve-control'>";
        approvecontrol += createHistory(controlMode);       
        approvecontrol += "</div>";
        return approvecontrol;
    }

    function SetVanickApproveControlEvents() {
        var Button_Request_Appove = $("#vanick-approve-control-button-request-approve");
        var Button_Agree = $("#vanick-approve-control-button-agree");
        var Button_No_Agree = $("#vanick-approve-control-button-no-agree");

        Button_Request_Appove.unbind();
        Button_Agree.unbind();
        Button_No_Agree.unbind();

        Button_Request_Appove.click(function () {
            SP.UI.ModalDialog.showWaitScreenWithNoClose('Loading...', 'Please wait..', 60, 270);
            vanickupdateItemVersionInit(currentPageID, currentListName);
            //Vanickpublish();
        });

        Button_Agree.click(function () {
            var inputComment = $("#vanick-approve-control-input-text-comment");
            if (inputComment.val() != "" && inputComment.val() != undefined) {
                SP.UI.ModalDialog.showWaitScreenWithNoClose('Loading...', 'Please wait..', 60, 270);
                isAgreeResponse = true;
                createListItem("Agree");
            }
            else {
                alert("You need to insert a comment");
            }
            //alert("Agree");
            //Vanickpublish();
        });

        Button_No_Agree.click(function () {
            var inputComment = $("#vanick-approve-control-input-text-comment");
            if (inputComment.val() != "" && inputComment.val() != undefined) {
                SP.UI.ModalDialog.showWaitScreenWithNoClose('Loading...', 'Please wait..', 60, 270);
                isAgreeResponse = false;
                createListItem("No Agree");
            }
            else {
                alert("You need to insert a comment");
            }
            //alert("No Agree");
            //Vanickpublish();
        });
    }

    function createHistory(controlMode) {
        var Approvebuttons = "";
        switch (controlMode) {
            case "Creator-Draft": Approvebuttons += '<li><a id="vanick-approve-control-button-request-approve" class="ui-tabs-anchor">Request Approve</a></li>';
                break;
            case "Creator-Approved": Approvebuttons += "";
                break;
            case "Creator-Pending": Approvebuttons += "";
                break;
            case "User-Approved": Approvebuttons += '<li><a id="vanick-approve-control-button-agree" class="ui-tabs-anchor">I Agree</a></li>';
                Approvebuttons += '<li><a id="vanick-approve-control-button-no-agree" class="ui-tabs-anchor">Do not Agree</a></li>';
                Approvebuttons += '<li><input id="vanick-approve-control-input-text-comment" class="ui-tabs-anchor" type="text" name="textComment"/></li>';
                break;
            case "User-Approved-new-version": Approvebuttons += '<li><a class="ui-tabs-anchor">This page has been updated recently by author and requires your approval, version: ' + currentPageVersion + '</a></li>';
                Approvebuttons += '<li><a id="vanick-approve-control-button-agree" class="ui-tabs-anchor">I Agree</a></li>';
                Approvebuttons += '<li><a id="vanick-approve-control-button-no-agree" class="ui-tabs-anchor">Do not Agree</a></li>';
                Approvebuttons += '<li><input id="vanick-approve-control-input-text-comment" class="ui-tabs-anchor" type="text" name="textComment"/></li>';
                break;
            case "User-Approved-Agree": Approvebuttons += '<li><span class="ui-tabs-anchor">Comments: ' + currentmessageinitem + '</span></li>';
                //Approvebuttons += '<li><span class="ui-tabs-anchor">Message</span></li>';                
                break;
        }

        //ApprovePagelog

        var vanickTab = '<div id="vanicktabs">' +
                            '<ul>' +
                                //'<li><a href="#ItemsHitory">Approve History</a></li>' +
                                Approvebuttons +
                                //'<li class="vanick-approve-control-message" style="float:right;"><span id="vanick-approve-control-message-status">status</span></li>' +
                            '</ul>' +
                            //'<div id="ItemsHitory">' +
                            //    '<p>items</p>' +
                            //'</div>' +
                        '</div>';
        return vanickTab;
    }

    function setMessageAppove(message) {
        $("#vanick-approve-control-message-status").text(message);
    }
    