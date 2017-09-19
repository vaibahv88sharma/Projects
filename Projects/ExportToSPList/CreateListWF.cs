using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.SharePoint.Client.WorkflowServices;

namespace ExportToSPList
{
    class CreateListWF
    {
        public void CreateListWFs()
        {
            string tenant = "https://networkintegration.sharepoint.com/sites/365Build/Watersun";//"https://mytenant.sharepoint.com/sites/test";
            string userName = "andrew@365build.com.au";
            string passwordString = "187Ch@lleng3r";
            string listName = "Test1234567";
            //string wfAssoc = "05639090-c09B-478A-B1F8-611718539D7F";//"05639090-c09b-478a-b1f8-611718539d7f";

            GlobalLogic gl = new GlobalLogic();
            ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);



            try
            {
                if (gl.createList(ctx, listName))
                {
                    if (gl.createListColumns(ctx, listName))
                    {                        
                        if (gl.createListView(ctx, listName))
                        {
                            Guid guid = gl.getListGuid(ctx, listName);
                            if (guid != Guid.Empty)
                            //if (gl.getListGuid(ctx, listName) != Guid.Empty)
                            {
                                gl.addWorkflowSubscription(ctx, listName, guid);
                            }                                
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
