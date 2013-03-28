using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services;
using Microsoft.SharePoint;
using System.Xml;

using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.Script.Services;
using Microsoft.SharePoint;
using System.Collections.Specialized;
using Microsoft.SharePoint.Utilities;

namespace VanickApproveControl.Services
{
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), WebService(Namespace = "http://vanick.com/")]
    [System.Web.Script.Services.ScriptService]
    class CustomServices : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true, Description = "Send Email")]
        public string SendEmail(string Message, string email)
        {
            //string result = "edgar";         
  
            return SendEmialInternal(email, Message).ToString();
        }

        
        private bool SendEmialInternal(string to, string body)
        {
          try {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
              using (SPSite site = new SPSite(SPContext.Current.Site.ID,
              SPContext.Current.Site.Zone))
              {
                using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                {
                 SPUtility.SendEmail(web, true, true, to,
                 "No agree Page", body);
                }
              }
            });
            return true;
          }
          catch {
              return false;
          }
        }        
    }
}
