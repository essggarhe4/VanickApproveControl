using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace VanickApproveControl.Data
{
    public class ApproveData
    {
        private Guid siteID { set; get; }
        private Guid webID { set; get; }
        private string listName { set; get; }

        public string ApproveMessageError { get; set; }

        public ApproveData(Guid siteid, Guid webid, string listname)
        {
            this.ApproveMessageError = string.Empty;
            this.siteID = siteid;
            this.webID = webid;
            this.listName = listname;
        }

        public List<DataPage> GetApprovalinformation(string pageid)
        {
            List<DataPage> datapageList = new List<DataPage>();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(this.siteID))
                {
                    using (SPWeb web = site.OpenWeb(this.webID))
                    {
                        if (web.Lists.TryGetList(this.listName) != null)
                        {
                            SPList apporoveList = web.Lists[this.listName];
                            if (isValidList(apporoveList))
                            {
                                SPQuery listQuery = new SPQuery();
                                //listQuery.Query = string.Format("<Where><Eq><FieldRef Name='Approve_x0020_Page_x0020_ID' /><Value Type='Number'>{0}</Value></Eq></Where>", pageid);
                                listQuery.Query = string.Format("<Query><Where><And><Eq><FieldRef Name='Approve_x0020_Page_x0020_ID' /><Value Type='Number'>{0}</Value></Eq><Eq><FieldRef Name='Author' LookupId=’TRUE’/><Value Type=’Integer’><UserID/></Value></Eq></And></Where></Query>", pageid);
                                
                                SPListItemCollection cgenericCollection = apporoveList.GetItems(listQuery);
                                foreach (SPListItem approveItem in cgenericCollection)
                                {
                                    string ApprovalStatus = string.Empty;
                                    string ApproveComments = string.Empty;
                                    string ApprovePageID = string.Empty;
                                    int Version = 0;

                                    if (approveItem.Fields.ContainsField(constants.columns.ApprovalList.Title) && approveItem[constants.columns.ApprovalList.Title] != null)
                                    {
                                        ApprovalStatus = approveItem[constants.columns.ApprovalList.Title].ToString();
                                    }

                                    if (approveItem.Fields.ContainsField(constants.columns.ApprovalList.ApproveComments) && approveItem[constants.columns.ApprovalList.ApproveComments] != null)
                                    {
                                        ApproveComments = approveItem[constants.columns.ApprovalList.ApproveComments].ToString();
                                    }

                                    if (approveItem.Fields.ContainsField(constants.columns.ApprovalList.ApprovePageID) && approveItem[constants.columns.ApprovalList.ApprovePageID] != null)
                                    {
                                        ApprovePageID = approveItem[constants.columns.ApprovalList.ApprovePageID].ToString();
                                    }

                                    if (approveItem.Fields.ContainsField(constants.columns.ApprovalList.Version) && approveItem[constants.columns.ApprovalList.Version] != null)
                                    {
                                        Version = int.Parse(approveItem[constants.columns.ApprovalList.Version].ToString());
                                    }                                    

                                    datapageList.Add(new DataPage
                                    {
                                        Status = ApprovalStatus,
                                        Comments = ApproveComments,
                                        Version = Version
                                    });
                                }
                            }
                            else
                            {
                                //This list is not valid. It has not all teh necesasary columns
                                this.ApproveMessageError = "The list does not have the correct columns";
                            }
                        }
                        else
                        {
                            //This list doesn't exist
                            this.ApproveMessageError = "The list doesn't exist";
                        }
                    }
                }
            });
            return datapageList;
        }

        private bool isValidList(SPList navigationList)
        {
            if (navigationList.Fields.ContainsField(constants.columns.ApprovalList.ApproveComments)
                && navigationList.Fields.ContainsField(constants.columns.ApprovalList.ApprovePageID)
                && navigationList.Fields.ContainsField(constants.columns.ApprovalList.Version)               
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
