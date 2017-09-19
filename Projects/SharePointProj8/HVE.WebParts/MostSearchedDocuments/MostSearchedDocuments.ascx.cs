using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using System.Web;

namespace SharePointProj8.HVE.WebParts.MostSearchedDocuments
{
    [ToolboxItemAttribute(false)]
    public partial class MostSearchedDocuments : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MostSearchedDocuments()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        #region Global Variable
        string var1, var2, var3, var4;
        DataTable tempTabale = null;
        public StringBuilder htmlString = new StringBuilder("");
        string hrefLink;
        string webApp;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                { 
                    HVE.Files.dbConnection conn = new HVE.Files.dbConnection();
                    string sclearsql = string.Concat("SELECT top 5 DocName, DocLocation, DownloadCount FROM [AAES Home].[dbo].[TopViewedDocsTable11] order by DownloadCount desc");
                    tempTabale = conn.executeSelectNoParameter(sclearsql);

                    webApp = SPContext.Current.Site.WebApplication.GetResponseUri(Microsoft.SharePoint.Administration.SPUrlZone.Default).ToString();
                    for (int i = 0; i < tempTabale.Rows.Count; i++)
                    {                    
                        hrefLink = "/"+tempTabale.Rows[i][1].ToString();
                        htmlString.Append("<br>");
                        htmlString.Append(@"<span>" + 
                                                "<a href='" + hrefLink + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a>" +
                                                " (" + tempTabale.Rows[i][2] + ")" +
                                            "</span>");                        
                        htmlString.Append("&nbsp");
                    }
                    Literal1.Text = htmlString.ToString();                    
                }
            }
            catch (System.Exception excep)
            {
                throw excep;
            }
        }

    }
}
