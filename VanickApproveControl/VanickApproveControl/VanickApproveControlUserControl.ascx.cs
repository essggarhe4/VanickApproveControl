using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Collections.Generic;
using VanickApproveControl.Data;
using System.Web.Script.Serialization;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint.Utilities;

namespace VanickApproveControl.VanickApproveControl
{
    public partial class VanickApproveControlUserControl : UserControl
    {
        public String ApprovalList { get; set; }
        public String PageList { get; set; }
        public string ApprovalGroup { get; set; }

        List<DataPage> approveDataResult;

        protected void Page_Load(object sender, EventArgs e)
        {
            //this.ApprovalList = "Approve List";
            //this.PageList = "Pages";
            this.ApprovalGroup = "Approval group test";
            if (!string.IsNullOrEmpty(ApprovalList) && !string.IsNullOrEmpty(PageList))
            {
                PageDataInit();
            }
            else
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ErrorMessagec", string.Format("<script>alert('You need to configure the webpart');</script>"));
                //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ErrorMessagec", string.Format("<script>$(document).ready(function () { var statusId = SP.UI.Status.addStatus('You need to configure the webpart');SP.UI.Status.setStatusPriColor(statusId, 'red');});</script>"));
                //statusId = SP.UI.Status.addStatus("Status good!");
                //SP.UI.Status.setStatusPriColor(statusId, 'red');
                //Exception ex = new Exception("You need to configure the webpart");
                //SPUtility.TransferToErrorPage(ex.Message);
            }
            //GetPageData();
        }        

        private void PageDataInit()
        {
            GetPageData();
            //if (GroupExists(SPContext.Current.Web.SiteGroups,this.ApprovalGroup))
            //{
            //    GetPageData();
            //}
            //else
            //{
            //    //Not configurre
            //}
        }

        private void GetPageData()
        {
            if(SPContext.Current != null)
            {
                if (SPContext.Current.List != null && SPContext.Current.List.Title.ToString().Equals(this.PageList))
                {
                    if(SPContext.Current.Item != null)
                    {
                        ApproveData AD = new ApproveData(SPContext.Current.Site.ID, SPContext.Current.Web.ID, this.ApprovalList);
                        approveDataResult = AD.GetApprovalinformation(SPContext.Current.Item.ID.ToString());
                        //SPContext.Current.Item[SPBuiltInFieldId.Created_x0020_By]
                        SPFieldUserValue userValue = new SPFieldUserValue(SPContext.Current.Web, SPContext.Current.Item["Created By"].ToString());
                        SPGroup currentApprovalGroup = SPContext.Current.Web.SiteGroups[this.ApprovalGroup];


                        string Author = string.Empty;
                        
                        SPUser cuser = SPContext.Current.Web.CurrentUser;
                        if (userValue.User != null)
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetAuthoremailPageID", string.Format("<script>currentemailAuthor = '{0}';</script>", userValue.User.Email));
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetTitlePageID", string.Format("<script>currentPageName = '{0}';</script>", SPContext.Current.File.Title));
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetUserName", string.Format("<script>currentuserName = '{0}';</script>", cuser.Name));                            
                            //currentemailAuthor    currentuserName


                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetPageID", string.Format("<script>currentPageID = '{0}';</script>", SPContext.Current.Item.ID.ToString()));
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetPageURL", string.Format("<script>currentPageURL = '{0}';</script>", SPContext.Current.File.Url));
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetListPage", string.Format("<script>currentListName = '{0}';</script>", this.PageList));
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetListApproval", string.Format("<script>currentListApproval = '{0}';</script>", this.ApprovalList));
                            
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetPageVersion", string.Format("<script>currentPageVersion = '{0}';</script>", SPContext.Current.Item["Version"].ToString()));


                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetApprovePageData", string.Format("<script>ApprovePageData = {0};</script>", new JavaScriptSerializer().Serialize(approveDataResult)));

                            //if (((SPListItem)SPContext.Current.Item).ModerationInformation.Status == SPModerationStatusType.Pending)
                            //{
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetPageStatus", string.Format("<script>approvePageStatus = '{0}';</script>", ((SPListItem)SPContext.Current.Item).ModerationInformation.Status.ToString()));
                            //}
                            
                            //Author = userValue.User.Name;
                            if (cuser.LoginName == userValue.User.LoginName)
                            {
                                //Is the Author                 
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetPageAuthor", "<script type='text/javascript'>isPageAuthor = true;</script>");
                            }
                            //else if (isUserinGroup(currentApprovalGroup, userValue.User))
                            //{
                            //    //Is the approval
                            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetPageApprover", "<script>isPageApprover = true;</script>");
                            //}
                            else
                            {
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetPageAuthor", "<script type='text/javascript'>isPageAuthor = false;</script>");
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetPageApprover", "<script>isPageApprover = false;</script>");
                            }
                        }
                        //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ExeSetPageData", "SetPageData();", true);
                        
                        //LitaralPage.Text = string.Format("<label>Page Title: {0}, Page ID: {1}</label>",SPContext.Current.Item["Title"], SPContext.Current.Item.ID );
                    }
                }
            }
            
        }

        public static bool GroupExists(SPGroupCollection groups, string name)
        {

            if (string.IsNullOrEmpty(name) ||

                (name.Length > 255) ||

                (groups == null) ||

                (groups.Count == 0))

                return false;

            else

                return (groups.GetCollection(new String[] { name }).Count > 0);

        }

        private bool isUserinGroup(SPGroup isgroup, SPUser oUser)
        {
            Boolean bUserIsInGroup = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                if (oUser != null)
                {
                    foreach (SPUser item in isgroup.Users)
                    {
                        if (item.LoginName == oUser.LoginName)
                        {
                            bUserIsInGroup = true;
                            break;
                        }
                    }
                }
            });
            return bUserIsInGroup;
        }        
    }
}
