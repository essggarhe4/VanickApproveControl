using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace VanickApproveControl.VanickApproveControl
{
    [ToolboxItemAttribute(false)]
    public class VanickApproveControl : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/VanickApproveControl/VanickApproveControl/VanickApproveControlUserControl.ascx";

        [WebBrowsable(true), Category("Configuration"), Personalizable(PersonalizationScope.Shared), DefaultValue(""), WebDisplayName("Page List"), WebDescription("List name where the webpart will get the information of pages. Example: Pages")]
        public string PageList
        {
            get { return pagelist; }
            set { pagelist = value; }
        }
        public string pagelist = string.Empty;

        [WebBrowsable(true), Category("Configuration"), Personalizable(PersonalizationScope.Shared), DefaultValue(""), WebDisplayName("Approve List"), WebDescription("List name where the webpart will get the information of approve log. Example: Approval List")]
        public string ApproveList
        {
            get { return approvelist; }
            set { approvelist = value; }
        }
        public string approvelist = string.Empty;

        protected override void CreateChildControls()
        {           
            VanickApproveControlUserControl control = (VanickApproveControlUserControl)Page.LoadControl(_ascxPath);

            if (string.IsNullOrEmpty(this.PageList)) control.PageList = string.Empty;
            else control.PageList = this.PageList;

            if (string.IsNullOrEmpty(this.ApproveList)) control.ApprovalList = string.Empty;
            else control.ApprovalList = this.ApproveList;

            Controls.Add(control);

            base.CreateChildControls(); 
        }

        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // add the script manager to the site to use Ajax
            if (ScriptManager.GetCurrent(this.Page) == null)
            {
                this._sm = new ScriptManager();
                _sm.EnablePageMethods = true;
                this._sm.ID = "ScriptManager";
                if (this.Page.IsPostBack)
                {
                    this.Page.ClientScript.RegisterStartupScript(typeof(VanickApproveControlUserControl),
                        this.ID,
                        "_spOriginalFormAction = document.forms[0].action; _spSuppressFormOnSubmitWrapper=true;");

                        /*typeof("The Class Name of WebPart "),
                                "",
                                "_spOriginalFormAction = document.forms[0].action; _spSuppressFormOnSubmitWrapper=true;",
                                true);*/
                }
                if (this.Page.Form != null)
                {
                    string str = this.Page.Form.Attributes["onsubmit"];
                    if (!(string.IsNullOrEmpty(str) || !(str == "return _spFormOnSubmitWrapper();")))
                    {
                        this.Page.Form.Attributes["onsubmit"] = "_spFormOnSubmitWrapper();";
                    }
                    this.Page.Form.Controls.AddAt(0, this._sm);
                }

            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            CssRegistration.Register("/_layouts/VanickApproveControl/css/vanickapprovedata.css");

            Page.ClientScript.RegisterStartupScript(GetType(), "JQuery",
                   "<SCRIPT language='javascript' src='/_layouts/VanickApproveControl/JavaScript/jquery-1.9.1.min.js'></SCRIPT>", false);

            Page.ClientScript.RegisterStartupScript(GetType(), "JQueryUI",
                   "<SCRIPT language='javascript' src='/_layouts/VanickApproveControl/JavaScript/jquery-ui.js'></SCRIPT>", false);

            Page.ClientScript.RegisterStartupScript(GetType(), "VanickApproveControl",
                   "<SCRIPT language='javascript' src='/_layouts/VanickApproveControl/JavaScript/ApproveControl.js'></SCRIPT>", false);

            Page.ClientScript.RegisterStartupScript(GetType(), "VanickPageCreator",
                   "<SCRIPT language='javascript' src='/_layouts/VanickApproveControl/JavaScript/PageCreator.js'></SCRIPT>", false);

            //ScriptManager.RegisterClientScriptResource(this.Page, this.GetType(), string.Format(jsbase, "/_layouts/VanickApproveControl/JavaScript/ApproveControl.js"));
            //Page.ClientScript.RegisterClientScriptInclude("Jquery", "/_layouts/JavaScript/jquery-1.9.1.min.js");
            //Page.ClientScript.RegisterClientScriptInclude("ApproveControlJS", "/_layouts/JavaScript/ApproveControl.js");
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ExeSetPageData", "SetPageData();", true);
            base.Render(writer);
        }

        public ScriptManager _sm { get; set; }
    }
}
