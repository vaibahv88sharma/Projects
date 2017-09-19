using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Data;
using System.Data.SqlClient;

namespace SharePointProj666
{
    class CustCode : SPJobDefinition
    {
        public CustCode() : base() { }

        public CustCode(string jobName, SPService service) :
            base(jobName, service, null, SPJobLockType.None)
        {
            this.Title = jobName;
            //this.Title = "Task Complete Timer1";
        }

        public CustCode(string jobName, SPWebApplication webapp) :
            base(jobName, webapp, null, SPJobLockType.Job)
        {
            this.Title = jobName;
        }

        public override void Execute(Guid targetInstanceId)
        {
            ////SPWebApplication webApp = this.Parent as SPWebApplication;
            ////string var1 = webApp.Sites[0].RootWeb.Url.ToString();
            ////string var2 = webApp.Sites[0].Url.ToString();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPWebApplication webApp = this.Parent as SPWebApplication;
                SPList logList = webApp.Sites[0].RootWeb.Lists["TimerLogList"];
                SPListItem logListItems = logList.Items.Add();
                logListItems["Title"] = var1 + "......." + var2;
                logListItems.Update();
            });


            GetSiteNames1();
            LogAction_SQLDataInsert("Before starting foreach of SQL function");
            CopySQLTableData();
            LogAction_SQLDataInsert("After starting foreach of SQL function");
            /*
             TimerLogList_SQLDataInsert
             */

            #region commented region
            //            SPSecurity.RunWithElevatedPrivileges(delegate()
            //                {                  
            //                    using (SPSite site = new SPSite("http://win-njfp7te48bn/sites/HVEDev"))
            //                    {
            //                        using (SPWeb web = site.OpenWeb())
            //                        {
            //                            SPList list = web.Lists["TopViewedDocs"];
            //                            SPQuery myQuery = new SPQuery();
            //                            myQuery.ViewXml = @"<View>
            //                                                    <Query>
            //														<OrderBy>
            //															<FieldRef Name='DocViewCount' Ascending='False' />
            //														</OrderBy>
            //														<FieldRef Name='DocName' />
            //														<FieldRef Name='DocViewCount' />														
            //                                                    </Query>
            //                                               </View>";
            //                            SPListItemCollection myItems = list.GetItems(myQuery);
            //                            foreach (SPListItem item in myItems)
            //                            {
            //                                //DocViewCounts(item["SiteName"].ToString(), item["DocLibName"].ToString());
            //                                //htmlStr.Append(li["DocName"].ToString() + "--" + li["DocViewCount"].ToString() + "<br>");
            //                            }
            //                            //Literal1.Text = htmlStr1.ToString();
            //                        }
            //                    }
            //                });

            //TopViewedDocs

            ////SPWebApplication webapp = this.Parent as SPWebApplication;           
            ////SPList tasklist = webapp.Sites[0].RootWeb.Lists["Tasks1"];

            // SPSite site1 = new SPSite("http://win-njfp7te48bn/sites/HVEDev");
            // SPWeb web1 = site1.OpenWeb();
            // SPList tasklist = web1.Lists["Tasks1"];
            // SPListItem li = tasklist.Items.Add();
            // li["Title"] = "New Task :- " + DateTime.Now.ToString();
            // li.Update();
            #endregion
        }

        #region TimerCode 11 Sep 2015   0313PM

        #region TimerJob


        #region Global Variables

        public StringBuilder htmlStr = new StringBuilder("This is string builder for Most Viewed Docs <br><br>");
        public StringBuilder htmlStr1 = new StringBuilder("This is string builder <br><br>");
        string var1, var2, var3, var4;
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        DataTable dtAudit = new DataTable();
        //////////DataTable dtAudit = null;

        string sqlDocName, sqlDocLocation;

        #endregion

        public void LogAction(string message)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                try
                {
                    SPWebApplication webApp = this.Parent as SPWebApplication;
                    SPList logList = webApp.Sites[0].RootWeb.Lists["TimerLogList"];
                    SPListItem logListItems = logList.Items.Add();
                    logListItems["Title"] = string.Concat(message + " **** " + DateTime.Now.ToString());
                    logListItems.Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

        }

        protected void GetSiteNames1()
        {
            #region Fetching Site Names and DocLibrary Name
            LogAction("Start of GetSiteNames1");
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPWebApplication webApp = this.Parent as SPWebApplication;
                SPList docLibNameList;

                try
                {
                    LogAction("Try Block of Fetching Site Names and DocLibrary Name ");
                    docLibNameList = webApp.Sites[0].RootWeb.Lists["DocLibLocations"];
                    SPQuery myQuery = new SPQuery();
                    myQuery.ViewXml = @"<View>
                                                    <Query>
                                                        <FieldRef Name='SiteName' />
                                                        <FieldRef Name='DocLibName' />
                                                    </Query>
                                               </View>";
                    SPListItemCollection myItems = docLibNameList.GetItems(myQuery);
                    dtAudit.Columns.Add("DocName");
                    dtAudit.Columns.Add("DocLocation");
                    dtAudit.Columns.Add("DownloadCount");
                    foreach (SPListItem item in myItems)
                    {
                        LogAction("Step 1 :- Fetching Site Names and DocLibrary Name for:- " + item["SiteName"].ToString() + " and " + item["DocLibName"].ToString());
                        DocViewCounts1(item["SiteName"].ToString(), item["DocLibName"].ToString());
                    }
                }
                catch (Exception ee)
                {
                    LogAction(ee.Message);
                }
                finally
                {
                    dtAudit = null;
                }


            });
            #endregion
        }

        protected void DocViewCounts1(string siteObj, string libName)
        {
            #region t1
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                try
                { 
                    using (SPSite site = new SPSite(siteObj))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists[libName];
                            SPListItemCollection coll = list.GetItems();
                            LogAction("Before starting foreach of SPAudit for web:" + web.Url);
                            LogAction("operating for list:" + libName);
                            foreach (SPListItem item in coll)
                            {
                                SPAuditQuery spQuery = new SPAuditQuery(site);
                                spQuery.RestrictToListItem(item);
                                SPAuditEntryCollection auditCol = site.Audit.GetEntries(spQuery);

                                string docName = "";
                                int counter = 0;
                                LogAction("Starting Foreach for SPAudit123");
                                LogAction(auditCol.Count.ToString());
                                foreach (SPAuditEntry entry in auditCol)
                                {
                                    LogAction("Event:- " + entry.Event.ToString() + "Document type:- " + entry.ItemType.ToString());
                                    if (entry.ItemType == SPAuditItemType.Document && entry.Event == SPAuditEventType.View)
                                    {
                                        try
                                        {
                                            var1 = entry.DocLocation.Substring(entry.DocLocation.LastIndexOf("/"));
                                            var2 = var1.Substring(var1.LastIndexOf("/"));
                                            var3 = var2.Substring(1);
                                            var4 = var3.Substring(var3.LastIndexOf('.') + 1);
                                            LogAction(entry.Event.ToString() + " --- " + var4);
                                            if (var4 != "aspx")
                                            {
                                                if (entry.EventSource == SPAuditEventSource.SharePoint)
                                                {
                                                    if (docName != var3)
                                                    {
                                                        docName = var3;
                                                        counter = 1;

                                                        DataRow drRow = dtAudit.NewRow();
                                                        drRow["DocName"] = var3;
                                                        drRow["DocLocation"] = entry.DocLocation;
                                                        drRow["DownloadCount"] = 1;
                                                        dtAudit.Rows.Add(drRow);
                                                    }
                                                    else
                                                    {
                                                        DataRow[] drExists = dtAudit.Select("DocName = '" + var3 + "' AND DocLocation = '" + entry.DocLocation + "'");
                                                        if (drExists != null && drExists.Length > 0)
                                                        {
                                                            drExists[0]["DownloadCount"] = Convert.ToInt32(drExists[0]["DownloadCount"]) + 1;
                                                        }
                                                        counter = counter + 1;
                                                    }
                                                }
                                            }
                                           // LogAction("Ending Foreach for SPAudit");
                                        }
                                        catch (Exception ee)
                                        {
                                            LogAction(ee.Message);
                                        }

                                    }
                                }
                            }
                        }
                    }


                    try
                    {
                        HVEFiles.dbConnection conn = new HVEFiles.dbConnection();
                        LogAction("Before starting foreach of SQL");
                        foreach (DataRow rr in dtAudit.Rows)
                        {
                            #region SQL Command for select from [AAES Home].[dbo].[TopViewedDocsTable11]

                            //HVEFiles.dbConnection conn = new HVEFiles.dbConnection();
                            sqlDocName = rr["DocName"].ToString();
                            sqlDocLocation = rr["DocLocation"].ToString();

                            //Fetch form SQL
                            LogAction("Before Select for: sqlDocName - " + sqlDocName + " and Doclaocation: "+sqlDocLocation);
                            string sclearsql = string.Concat("SELECT * FROM [AAES Home].[dbo].[TopViewedDocsTable11] " +
                                                                     "WHERE DocName = @DocName AND DocLocation = @DocLocation");
                            SqlParameter[] parameter = {                                
                                                new SqlParameter("@DocName", SqlDbType.VarChar) { Value =sqlDocName },
                                                new SqlParameter("@DocLocation", SqlDbType.VarChar) { Value = sqlDocLocation }                                        
                                                 };
                            DataTable tempTable = null;

                            tempTable = conn.executeSelectQuery(sclearsql, parameter);

                            if ((tempTable == null) || (tempTable.Rows.Count == 0))
                            {
                                //insert
                                string sclearsqlIns = string.Concat("INSERT INTO [AAES Home].[dbo].[TopViewedDocsTable11] " +
                                                                    "(DocName, DocLocation, DownloadCount) " +
                                                                    "VALUES(@DocName, @DocLocation, @DownloadCount)");
                                SqlParameter[] parameterUpd = {                                
                                                    new SqlParameter("@DocName", SqlDbType.VarChar) { Value = rr["DocName"].ToString() },
                                                    new SqlParameter("@DocLocation", SqlDbType.VarChar) { Value = rr["DocLocation"].ToString() },
                                                    new SqlParameter("@DownloadCount", SqlDbType.Int) { Value = Convert.ToInt32(rr["DownloadCount"]) }
                                                         };                                
                                bool isInsert = conn.executeInsertQuery(sclearsqlIns, parameterUpd);
                                LogAction("After Insert for: sqlDocName - " + sqlDocName + " and Doclaocation: " + sqlDocLocation);
                            }
                            else
                            {
                                //update
                                string sclearsqlUpd = string.Concat("UPDATE [AAES Home].[dbo].[TopViewedDocsTable11] " +
                                                                    "SET  DownloadCount = @DownloadCount " +
                                                                    "WHERE DocName = @DocName AND DocLocation = @DocLocation");
                                SqlParameter[] parameterUpd = {                                
                                                    new SqlParameter("@DocName", SqlDbType.VarChar) { Value = rr["DocName"].ToString() },
                                                    new SqlParameter("@DocLocation", SqlDbType.VarChar) { Value = rr["DocLocation"].ToString() },
                                                    new SqlParameter("@DownloadCount", SqlDbType.Int) { Value = Convert.ToInt32(rr["DownloadCount"]) }
                                                     };
                                bool isInsert = conn.executeUpdateQuery(sclearsqlUpd, parameterUpd);
                                LogAction("After Update for: sqlDocName - " + sqlDocName + " and Doclaocation: " + sqlDocLocation);
                            }
                            #endregion
                        }
                    }
                    catch (Exception Exception1)
                    {
                        LogAction(Exception1.Message);
                    }
                }


                catch (Exception eee)
                {
                    LogAction(eee.Message);
                }
            });

            #endregion
        }

        #endregion

        #endregion

        #region Copy SQL Table

        public void LogAction_SQLDataInsert(string message)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                try
                {
                    SPWebApplication webApp = this.Parent as SPWebApplication;
                    SPList logList = webApp.Sites[0].RootWeb.Lists["TimerLogList_SQLDataInsert"];
                    SPListItem logListItems = logList.Items.Add();
                    logListItems["Title"] = string.Concat(message + " **** " + DateTime.Now.ToString());
                    logListItems.Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

        }

        protected void CopySQLTableData()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {                    
                    try
                    {
                        HVEFiles.dbConnection conn = new HVEFiles.dbConnection();
                        LogAction_SQLDataInsert("Before starting foreach of SQL");
                        string sclearsql = string.Concat(@"[dbo].[CopySearchData]");
                            //DataTable tempTable = null;
                            //tempTable = conn.executeSelectNoParameter(sclearsql);
                        conn.executeSelectNoParameter(sclearsql);                                                
                    }
                    catch (Exception Exception)
                    {
                        LogAction_SQLDataInsert(Exception.Message);
                    }                
            });
        }

        #endregion

        #region Old Timer

        ////////////        #region Global Variables

        ////////////        public StringBuilder htmlStr = new StringBuilder("This is string builder for Most Viewed Docs <br><br>");
        ////////////        public StringBuilder htmlStr1 = new StringBuilder("This is string builder <br><br>");
        ////////////        string var1, var2, var3, var4;
        ////////////        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        ////////////        DataTable dtAudit = new DataTable();
        ////////////        string sqlDocName, sqlDocLocation;

        ////////////        #endregion

        ////////////        protected void GetSiteNames()
        ////////////        {
        ////////////            #region Fetching Site Names and DocLibrary Name
        ////////////            SPSecurity.RunWithElevatedPrivileges(delegate()
        ////////////            {
        ////////////                using (SPSite site1 = new SPSite("http://win-njfp7te48bn/sites/HVEDev"))
        ////////////                {
        ////////////                    using (SPWeb web1 = site1.OpenWeb())
        ////////////                    {
        ////////////                        SPSecurity.RunWithElevatedPrivileges(delegate()
        ////////////                        {
        ////////////                            using (SPSite siteLogList = new SPSite("http://win-njfp7te48bn/sites/HVEDev"))
        ////////////                            {
        ////////////                                using (SPWeb webLogList = siteLogList.OpenWeb())
        ////////////                                {
        ////////////                                    SPList logList = webLogList.Lists["TimerLogList"]; //Documents 
        ////////////                                    try
        ////////////                                    {
        ////////////                                        SPList list = web1.Lists["DocLibLocations"];
        ////////////                                        SPQuery myQuery = new SPQuery();
        ////////////                                        myQuery.ViewXml = @"<View>
        ////////////                                                    <Query>
        ////////////                                                        <FieldRef Name='SiteName' />
        ////////////                                                        <FieldRef Name='DocLibName' />
        ////////////                                                    </Query>
        ////////////                                               </View>";
        ////////////                                        SPListItemCollection myItems = list.GetItems(myQuery);
        ////////////                                        dtAudit.Columns.Add("DocName");
        ////////////                                        dtAudit.Columns.Add("DocLocation");
        ////////////                                        dtAudit.Columns.Add("DownloadCount");
        ////////////                                        SPListItem logListItems = logList.Items.Add();
        ////////////                                        logListItems["Title"] = "Step 1 :- Fetching Site Names and DocLibrary Name";
        ////////////                                        logListItems.Update();
        ////////////                                        foreach (SPListItem item in myItems)
        ////////////                                        {
        ////////////                                            DocViewCounts(item["SiteName"].ToString(), item["DocLibName"].ToString());
        ////////////                                        }
        ////////////                                        //Literal1.Text = htmlStr1.ToString();
        ////////////                                    }
        ////////////                                    catch (Exception ee)
        ////////////                                    {
        ////////////                                        SPListItem logListItems = logList.Items.Add();
        ////////////                                        logListItems["Title"] = "1:- GetSiteNames :- " + ee.Message;
        ////////////                                        logListItems.Update();
        ////////////                                    }
        ////////////                                }

        ////////////                            }
        ////////////                        });
        ////////////                    }
        ////////////                }
        ////////////            });
        ////////////            #endregion
        ////////////        }

        ////////////        protected void DocViewCounts(string siteObj, string libName)
        ////////////        {
        ////////////            #region t1
        ////////////            SPSecurity.RunWithElevatedPrivileges(delegate()
        ////////////               {
        ////////////                   using (SPSite siteLogList = new SPSite("http://win-njfp7te48bn/sites/HVEDev"))
        ////////////                   {
        ////////////                       using (SPWeb webLogList = siteLogList.OpenWeb())
        ////////////                       {
        ////////////                           SPList logList = webLogList.Lists["TimerLogList"];
        ////////////                           SPListItem logListItems = null;
        ////////////                           try
        ////////////                           {

        ////////////                               using (SPSite site = new SPSite(siteObj))
        ////////////                               {
        ////////////                                   using (SPWeb web = site.OpenWeb())
        ////////////                                   {
        ////////////                                       //lbltest.Text += "<br/>" + web.Title.ToString();
        ////////////                                       SPList list = web.Lists[libName]; //Documents           
        ////////////                                       SPListItemCollection coll = list.GetItems();
        ////////////                                       //Dictionary<string, int> dictionary = new Dictionary<string, int>();

        ////////////                                       //dtAudit.Columns.Add("DocName");
        ////////////                                       //dtAudit.Columns.Add("DocLocation");
        ////////////                                       //dtAudit.Columns.Add("DownloadCount");

        ////////////                                       foreach (SPListItem item in coll)
        ////////////                                       {
        ////////////                                           SPAuditQuery spQuery = new SPAuditQuery(site);
        ////////////                                           spQuery.RestrictToListItem(item);
        ////////////                                           SPAuditEntryCollection auditCol = site.Audit.GetEntries(spQuery);

        ////////////                                           string docName = "";
        ////////////                                           int counter = 0;
        ////////////                                           foreach (SPAuditEntry entry in auditCol)
        ////////////                                           {
        ////////////                                               if (entry.ItemType == SPAuditItemType.Document && entry.Event == SPAuditEventType.View)
        ////////////                                               {
        ////////////                                                   try
        ////////////                                                   {
        ////////////                                                       var1 = entry.DocLocation.Substring(entry.DocLocation.LastIndexOf("/"));
        ////////////                                                       var2 = var1.Substring(var1.LastIndexOf("/"));
        ////////////                                                       var3 = var2.Substring(1);
        ////////////                                                       var4 = var3.Substring(var3.LastIndexOf('.') + 1);
        ////////////                                                       if (var4 != "aspx")
        ////////////                                                       {
        ////////////                                                           if (entry.EventSource == SPAuditEventSource.SharePoint)
        ////////////                                                           {
        ////////////                                                               if (docName != var3)
        ////////////                                                               {
        ////////////                                                                   docName = var3;
        ////////////                                                                   counter = 1;

        ////////////                                                                   DataRow drRow = dtAudit.NewRow();
        ////////////                                                                   drRow["DocName"] = var3;
        ////////////                                                                   drRow["DocLocation"] = entry.DocLocation;
        ////////////                                                                   drRow["DownloadCount"] = 1;
        ////////////                                                                   //dictionary.Add(var3, 1);
        ////////////                                                                   dtAudit.Rows.Add(drRow);
        ////////////                                                               }
        ////////////                                                               else
        ////////////                                                               {
        ////////////                                                                   DataRow[] drExists = dtAudit.Select("DocName = '" + var3 + "' AND DocLocation = '" + entry.DocLocation + "'");
        ////////////                                                                   if (drExists != null && drExists.Length > 0)
        ////////////                                                                   {
        ////////////                                                                       //int cont = drExists[0]["DownloadCount"];
        ////////////                                                                       drExists[0]["DownloadCount"] = Convert.ToInt32(drExists[0]["DownloadCount"]) + 1;
        ////////////                                                                       //drExists[0]["DownloadCount"];
        ////////////                                                                   }
        ////////////                                                                   //if (dictionary.TryGetValue(var3, out counter))
        ////////////                                                                   //{
        ////////////                                                                   //    dictionary[var3] = counter + 1;
        ////////////                                                                   //}
        ////////////                                                                   counter = counter + 1;
        ////////////                                                               }
        ////////////                                                           }
        ////////////                                                       }
        ////////////                                                   }
        ////////////                                                   catch (Exception ee)
        ////////////                                                   {
        ////////////                                                       // SPSecurity.RunWithElevatedPrivileges(delegate()
        ////////////                                                       // {

        ////////////                                                       //lbltest.Text += "<br/>" + web.Title.ToString();
        ////////////                                                       //SPList logList = webLogList.Lists["TimerLogList"]; //Documents           
        ////////////                                                       //SPListItem logListItems = 
        ////////////                                                           logList.Items.Add();
        ////////////                                                       logListItems["Title"] = "2 :- SPAuditEntry :- " + ee.Message;
        ////////////                                                       logListItems.Update();
        ////////////                                                       //  }
        ////////////                                                       // }
        ////////////                                                       //});
        ////////////                                                   }

        ////////////                                               }
        ////////////                                           }
        ////////////                                       }
        ////////////                                   }
        ////////////                               }
        ////////////                               using (SPSite docSite = new SPSite("http://win-njfp7te48bn/sites/HVEDev"))
        ////////////                               {
        ////////////                                   using (SPWeb docWeb = docSite.OpenWeb())
        ////////////                                   {
        ////////////                                       SPList docList = docWeb.Lists["TopViewedDocs"];
        ////////////                                       //foreach (KeyValuePair<string, int> pair in dictionary)
        ////////////                                       foreach (DataRow rr in dtAudit.Rows)
        ////////////                                       {
        ////////////                                           #region List Data Insert
        ////////////                                           ////////htmlStr.Append(("Document Name: " + pair.Key.ToString() + "  -  " + "Views Count: " + pair.Value.ToString()) + "<br>");
        ////////////                                           //////SPListItem li = docList.Items.Add();                             
        ////////////                                           //////li["DocName"] = rr["DocName"].ToString();// pair.Value.ToString();
        ////////////                                           //////li["DocLocation"] = rr["DocLocation"].ToString();// pair.Value.ToString();
        ////////////                                           //////li["DocViewCount"] = Convert.ToInt32(rr["DownloadCount"]);// pair.Value.ToString();
        ////////////                                           //////li["Title"] = "Data Entered at :- " + DateTime.Now.ToString();

        ////////////                                           ////////li["DocViewCount"] = pair.Value.ToString();
        ////////////                                           ////////li["Title"] = "Data Entered at :- " + DateTime.Now.ToString();
        ////////////                                           //////li.Update();
        ////////////                                           #endregion
        ////////////                                           try
        ////////////                                           {
        ////////////                                               #region SQL Command for select from [AAES Home].[dbo].[TopViewedDocsTable11]

        ////////////                                               HVEFiles.dbConnection conn = new HVEFiles.dbConnection();
        ////////////                                               //CustCode conn = new CustCode();
        ////////////                                               sqlDocName = rr["DocName"].ToString();
        ////////////                                               sqlDocLocation = rr["DocLocation"].ToString();
        ////////////                                               string sclearsql = string.Concat("SELECT * FROM [AAES Home].[dbo].[TopViewedDocsTable11] " +
        ////////////                                                                                        "WHERE DocName = @DocName AND DocLocation = @DocLocation");
        ////////////                                               SqlParameter[] parameter = {                                
        ////////////                                                new SqlParameter("@DocName", SqlDbType.VarChar) { Value =sqlDocName },
        ////////////                                                new SqlParameter("@DocLocation", SqlDbType.VarChar) { Value = sqlDocLocation }                                        
        ////////////                                                 };
        ////////////                                               DataTable tempTable = null;
        ////////////                                               //SPListItem logListItems = logList.Items.Add();
        ////////////                                               logList.Items.Add();
        ////////////                                               logListItems["Title"] = "3 :- SQL FUnctions :- " + "before executeSelectQuery";
        ////////////                                               logListItems.Update();
        ////////////                                               tempTable = conn.executeSelectQuery(sclearsql, parameter);
        ////////////                                               //SPListItem logListItems = logList.Items.Add();
        ////////////                                               logList.Items.Add();
        ////////////                                               logListItems["Title"] = "3 :- SQL FUnctions :- " + "After executeSelectQuery";
        ////////////                                               logListItems.Update();
        ////////////                                               if ((tempTable == null) || (tempTable.Rows.Count == 0))
        ////////////                                               {
        ////////////                                                   //insert
        ////////////                                                   string sclearsqlIns = string.Concat("INSERT INTO [AAES Home].[dbo].[TopViewedDocsTable11] " +
        ////////////                                                                                       "(DocName, DocLocation, DownloadCount) " +
        ////////////                                                                                       "VALUES(@DocName, @DocLocation, @DownloadCount)");
        ////////////                                                   SqlParameter[] parameterUpd = {                                
        ////////////                                                    new SqlParameter("@DocName", SqlDbType.VarChar) { Value = rr["DocName"].ToString() },
        ////////////                                                    new SqlParameter("@DocLocation", SqlDbType.VarChar) { Value = rr["DocLocation"].ToString() },
        ////////////                                                    new SqlParameter("@DownloadCount", SqlDbType.Int) { Value = Convert.ToInt32(rr["DownloadCount"]) }
        ////////////                                                         };
        ////////////                                                   bool isInsert = conn.executeInsertQuery(sclearsqlIns, parameterUpd);
        ////////////                                               }
        ////////////                                               else
        ////////////                                               {
        ////////////                                                   //update
        ////////////                                                   string sclearsqlUpd = string.Concat("UPDATE [AAES Home].[dbo].[TopViewedDocsTable11] " +
        ////////////                                                                                       "SET  DownloadCount = @DownloadCount " +
        ////////////                                                                                       "WHERE DocName = @DocName AND DocLocation = @DocLocation");
        ////////////                                                   SqlParameter[] parameterUpd = {                                
        ////////////                                                    new SqlParameter("@DocName", SqlDbType.VarChar) { Value = rr["DocName"].ToString() },
        ////////////                                                    new SqlParameter("@DocLocation", SqlDbType.VarChar) { Value = rr["DocLocation"].ToString() },
        ////////////                                                    new SqlParameter("@DownloadCount", SqlDbType.Int) { Value = Convert.ToInt32(rr["DownloadCount"]) }
        ////////////                                                     };
        ////////////                                                   bool isInsert = conn.executeUpdateQuery(sclearsqlUpd, parameterUpd);
        ////////////                                               }


        ////////////                                               #endregion
        ////////////                                           }
        ////////////                                           catch (Exception Exception1)
        ////////////                                           {
        ////////////                                               //SPSecurity.RunWithElevatedPrivileges(delegate()
        ////////////                                               //{
        ////////////                                               //    using (SPSite siteLogList = new SPSite("http://win-njfp7te48bn/sites/HVEDev"))
        ////////////                                               //    {
        ////////////                                               //        using (SPWeb webLogList = siteLogList.OpenWeb())
        ////////////                                               //        {
        ////////////                                               //lbltest.Text += "<br/>" + web.Title.ToString();
        ////////////                                               // SPList logList = webLogList.Lists["TimerLogList"]; //Documents           
        ////////////                                              // SPListItem logListItems = logList.Items.Add();
        ////////////                                               logList.Items.Add();
        ////////////                                               logListItems["Title"] = "3 :- SQL FUnctions :- " + Exception1.Message;
        ////////////                                               logListItems.Update();
        ////////////                                               //        }
        ////////////                                               //    }
        ////////////                                               //});
        ////////////                                               // Exception1Label.Text = Exception1.Message; //Label1
        ////////////                                           }
        ////////////                                           #region SQL Command for insert into SP
        ////////////                                           //    string sclearsql = "sp_TopViewedDocs"; /*"INSERT INTO TopViewedDocsTable (TraineeName, LoginID,LearnerDSId, TraineeOnboardingDate,SupervisorLogin,FunctionalArea1,TraineeOrganization,RoleFamily1,Role1,FunctionalSpeciality1,ManagerRightsNeeded,ArchiveRole1,ArchiveRole2,ArchiveRole3,TraineeLocation,RoleFamily2,Role2,FunctionalArea2,FunctionalSpeciality2,RoleFamily3,Role3,FunctionalArea3,FunctionalSpeciality3) " +
        ////////////                                           //"VALUES (@TraineeName, @LoginID,@LearnerDSId, @TraineeOnboardingDate,@SupervisorLogin, @FunctionalArea1, @TraineeOrganization,@RoleFamily1,@Role1,@FunctionalSpeciality1,@ManagerRightsNeeded,@ArchiveRole1,@ArchiveRole2,@ArchiveRole3,@TraineeLocation,@RoleFamily2, @Role2, @FunctionalArea2,@FunctionalSpeciality2,@RoleFamily3,@Role3,@FunctionalArea3,@FunctionalSpeciality3)";*/

        ////////////                                           //    SqlParameter[] parameter = {                                
        ////////////                                           //    new SqlParameter("@DocName", SqlDbType.VarChar) { Value = rr["DocName"].ToString() },
        ////////////                                           //    new SqlParameter("@DocLocation", SqlDbType.VarChar) { Value = rr["DocLocation"].ToString() },
        ////////////                                           //    new SqlParameter("@DownloadCount", SqlDbType.Int) { Value = Convert.ToInt32(rr["DownloadCount"]) }
        ////////////                                           //    };
        ////////////                                           //    HVE.Files.dbConnection conn = new HVE.Files.dbConnection();
        ////////////                                           //    bool isInsert = conn.executeInsertQuery(sclearsql, parameter);

        ////////////                                           #endregion
        ////////////                                       }
        ////////////                                   }
        ////////////                               }

        ////////////                           }
        ////////////                           catch (Exception eee)
        ////////////                           {
        ////////////                               //Console.WriteLine(eee.Message);
        ////////////                               //SPSecurity.RunWithElevatedPrivileges(delegate()
        ////////////                               //{
        ////////////                               //    using (SPSite siteLogList1 = new SPSite("http://win-njfp7te48bn/sites/HVEDev"))
        ////////////                               //    {
        ////////////                               //        using (SPWeb webLogList1 = siteLogList1.OpenWeb())
        ////////////                               //        {
        ////////////                               //lbltest.Text += "<br/>" + web.Title.ToString();
        ////////////                               //SPList logList1 = webLogList1.Lists["TimerLogList"]; //Documents           
        ////////////                               //SPListItem logListItems = logList.Items.Add();
        ////////////                               logList.Items.Add();
        ////////////                               logListItems["Title"] = eee.Message;
        ////////////                               logListItems.Update();
        ////////////                               //        }
        ////////////                               //    }
        ////////////                               //});
        ////////////                           }
        ////////////                           //LiteralText.Text = htmlStr.ToString();
        ////////////                       }
        ////////////                   }
        ////////////               });

        ////////////            #endregion
        ////////////        }

        #endregion



    }
}



