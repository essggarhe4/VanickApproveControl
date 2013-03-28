function checkInDocument(obj, pageUrl) {
    var success = true;
    $().SPServices({
        operation: "CheckInFile",
        async: false,
        pageUrl: pageUrl,
        comment: "Checked in during bulk upload",
        CheckinType: 1,
        completefunc: function (xData, Status) {
            $(xData.responseXML).find("errorstring").each(function () {
                alert("Please save all of your changes before attempting to check in the document.");
                success = false;
            });
        }
    });
}

var ListItem;

function Vanickpublish(){
        var ctx = SP.ClientContext.get_current();
        var page = ctx.get_web().getFileByServerRelativeUrl(window.location.pathname);
        page.checkIn();
        page.publish();
        ctx.load(page);
        ctx.executeQueryAsync(Function.createDelegate(this, publish_Success),
                                            Function.createDelegate(this, pubish_Fail));
}
function publish_Success(sender, args) {
        vanickupdateItemInit(currentPageID, currentListName);
        //vanickupdateItem(currentPageID, currentListName);
        //vanickupdateItemVersion(currentPageID, currentListName);
        //location.reload();
        //alert('i am good');
}
function pubish_Fail(sender, args){
        alert('something wrong happened…');
}


function GetListItemById(listName, listItemId) {
    var SPContext = new SP.ClientContext.get_current();
    var web = SPContext.get_web();

    var list = web.get_lists().getByTitle(listName);
    ListItem = list.getItemById(listItemId);

    SPContext.load(ListItem, "Id", "Title");
    SPContext.executeQueryAsync(GetListItemById_Success, GetListItemById_Fail);
}

function GetListItemById_Success(sender, args) {
    //item["_ModerationStatus"]
    var fileItem = ListItem.get_file();
    fileItem.publish();    
    var id = ListItem.get_id();
    var title = ListItem.get_item("Title");
    alert("Updated List Item: \n Id: " + id + " \n Title: " + title);
}

// Display an appropriate error message
function GetListItemById_Fail(sender, args) {
    alert("GetListItemById Failed. \n" + args.get_message() + "\n" + args.get_stackTrace());
}

function vanickupdateItemVersionInit(itemvarid, listname) {
    var context = new SP.ClientContext.get_current();
    var web = context.get_web();
    var list = web.get_lists().getByTitle(listname);
    var item = list.getItemById(itemvarid);

    context.load(item);

    context.executeQueryAsync(function () {
        vanickupdateItemVersion(item);
    });

}

function vanickupdateItemVersion(item) {
    var context = new SP.ClientContext.get_current();    
    item.set_item("_Version", parseInt(currentPageVersion, 10) + 1);
    item.update();
    context.executeQueryAsync(Function.createDelegate(this, vanickupdateItemVersionsuccess), Function.createDelegate(this, vanickupdateItemVersionfailed));

}


function vanickupdateItemVersionsuccess(sender, args) {
    Vanickpublish();
    //vanickupdateItem(currentPageID, currentListName);
    //alert('failed. Message:' + args.get_message());
    //location.reload();
}

function vanickupdateItemVersionfailed(sender, args) {
    alert('failed. Message:' + args.get_message());
}

function vanickupdateItemInit(itemvarid, listname) {
    var context = new SP.ClientContext.get_current();
    var web = context.get_web();
    var list = web.get_lists().getByTitle(listname);
    var itemvar = list.getItemById(itemvarid);
    context.load(itemvar);
    context.executeQueryAsync(function () {
        vanickupdateItem(itemvar);
    });

}

function vanickupdateItem(item) {
    var context = new SP.ClientContext.get_current();
    item.set_item("_ModerationStatus", "0");    
    item.update();
    context.executeQueryAsync(Function.createDelegate(this, vanickupdateItemsuccess), Function.createDelegate(this, vanickupdateItemfailed));

}

function vanickupdateItemsuccess(sender, args) {
    //alert('si');
    location.reload();
}

function vanickupdateItemfailed(sender, args) {
    alert('failed. Message:' + args.get_message());
}


//------------------------------Insert Data---------------------------

function createListItem(status) {    
    clientContext = new SP.ClientContext.get_current();
    oWeb = clientContext.get_web();
    oListColl = oWeb.get_lists();
    oList = oListColl.getByTitle(currentListApproval);
    listItemCreationInfo = new SP.ListItemCreationInformation();
    oListItem = oList.addItem(listItemCreationInfo);
    oListItem.set_item('Title', status);
    oListItem.set_item('Approve_x0020_Page_x0020_ID', currentPageID);
    oListItem.set_item('Approve_x0020_Comments', $("#vanick-approve-control-input-text-comment").val());
    oListItem.set_item('_Version', currentPageVersion);
    
    //Approval User
    oListItem.update();
    clientContext.load(oListItem);
    clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
}

function onQuerySucceeded() {
    //alert(oListItem.get_title() + ' item is created successfully.');
    //location.reload();
    if(!isAgreeResponse)
        SendNoAgreeEmail();
    else
        location.reload();
}

function onQueryFailed(sender, args) {
    alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
    location.reload();
}

//----------------------SEND EMAIL----------------------

function SendNoAgreeEmail() {
    var message = "The user " + currentuserName + " has rejected the page " + currentPageName;
    $.ajax({
        type: "POST",
        url: "/_layouts/VanickApproveControl/Service/Service.asmx/SendEmail",
        data: "{ 'Message': '" + message + "', 'email': '" + currentemailAuthor + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            //consolg.log(msg);
            location.reload();
        },
        error: function (msgerro) {
            //console.log(msgerri);
            location.reload();
        }
    });
}


//-----------------------------------------------------------------------------------------------
function CallMe() {
    //var ctrl = document.getElementById(src);
    // call server side method
    PageMethods.GetContactName("Si", CallSuccess, CallFailed, "dd");
}

// set the destination textbox value with the ContactName
function CallSuccess(res, destCtrl) {
    var dest = document.getElementById(destCtrl);
    dest.value = res;
}

// alert message on some failure
function CallFailed(res, destCtrl) {
    alert(res.get_message());
}