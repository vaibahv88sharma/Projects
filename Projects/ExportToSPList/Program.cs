using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Client.WorkflowServices;
using Microsoft.SharePoint.Client.Workflow;
using System.Data.SqlTypes;
using Microsoft.SharePoint.Client.Utilities;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace ExportToSPList
{
    class Program
    {
        public static string queryId = string.Concat("SELECT [ID],[jobNumber],[jobId] ,[cstId] ,[SpId],[delayId] ,[dCreated] FROM [WatersunData].[dbo].[CounterDelay]" +
                                                             "WHERE jobNumber = @JobNum");
        /// <summary>
        /// Main Function, it calls functions to update jobs/delay data and ETS/Jobs/Supplier details
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            if (ConfigurationManager.AppSettings.Get("NonScheduling")=="1") 
            {
                //ReadAllSettings();            
            }
            if (ConfigurationManager.AppSettings.Get("OnlyScheduling") == "1")
            {
                CallForward();
            }                        
            
            #region Working Call Forward
//            string tenant = "https://networkintegration.sharepoint.com/sites/365Build/Watersun/WatersunJobs";
//            string userName = "andrew@365build.com.au";
//            string passwordString = "ch@lleng3r";

//            /*string sqlQuery = @"select JobNum,JobId,JobAddr, client,delay, week,overall, JobDelayLink,JobLink,Supervisor,ConstructionManager,JobsDelay,JobsDetail  from [WatersunData].[dbo].[vJobDelay]
//                                            where JobNum ='5921'";//*/
//            string sqlQuery = @"select JobNum,JobId,JobAddr, client,delay, week,overall, JobDelayLink,JobLink,Supervisor,ConstructionManager,JobsDelay,JobsDetail  from [WatersunData].[dbo].[vJobDelay]
//                                                    where JobNum in (SELECT  cast([JobNumber] as varchar) FROM [WatersunData].[dbo].[vFrameworkJobs] where left(JobNumber,1) in (4,5))";//*/

//            /*string sqlQuery = @"select JobNum,JobId,JobAddr, client,delay, week,overall, JobDelayLink,JobLink,Supervisor,ConstructionManager,JobsDelay,JobsDetail  
//                                 from [WatersunData].[dbo].[vJobDelay]
//                                 where JobNum in (SELECT  cast([JobNumber] as varchar) 
//                                                    FROM [WatersunData].[dbo].[vFrameworkJobs] 
//                                                    where JobNumber not in (select jobNum from [WatersunData].dbo.jobNmDemo) 
//                                                            and left(JobNumber,1) in (4,5))
//                                                        ";//*/

//            dbConnection conn = new dbConnection();
//            DataTable tempTable = conn.executeSelectNoParameter(sqlQuery);
//            SqlToSpCreateJobsMetadata(tenant, userName, passwordString, "Call Forwards", "Call Forwards", "JobNum", tempTable);
//            ///////////////////////////SqlToSpCreateJobsMetadata(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable);

//            #region Jobs Delay
//            //Jobs Delay
//            string queryJobsDelayItems = string.Concat("select Reason,Comments,DelayDate,ToDelayDate,DelayId,JobNumber,JobId,CstId,dateModified,timeModified From [WatersunData].[dbo].[vPerJobDelay]" +
//                                     "WHERE JobNumber = @JobNumber");
//            string queryJobsDelayItemsSpToSql = "SELECT [JobNumber] ,[Reason],[Comments],[JobId],[l_cst_id],[DelayId],[l_cst_dlyClass_id],[s_name],[DelayDate],[l_cst_dlyReas_id],[ToDelayDate],[Delay],[i_delAllowed_wD] FROM [WatersunData].[dbo].[v_JobDelaysNoCondition] where jobnumber = @JobNumber";

//            SqlToSpCreateJobsDelayData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsDelayItems);
//            //SpToSqlCreateJobsDelayData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsDelayItemsSpToSql);

//            #endregion

//            #region Jobs Data
//            //Jobs Data
//            string queryJobsItems = string.Concat("select ReqDays,s_costCentreCode,SupplierId,JobId, JobNum, CstCallId, Activity, called, calledfor, start, complete, Supervisor, ISNULL(Supplier,'[NULL]') as Supplier, Duration, calledBest,calledforBest, startBest,completeBest,b_complete,dateModified,timeModified " +
//                                                             "from [WatersunData].[dbo].[vJobsCalledForDates] " +
//                                                             "WHERE JobNum = @JobNum order by l_order");
//            //SqlToSpCreateJobsData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsItems);
//            //SpToSqlCreateJobsData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsItems);
//            #endregion


//            queryId = string.Concat("SELECT [ID],[jobNumber],[jobId] ,[cstId] ,[SpId],[delayId] ,[dCreated] FROM [WatersunData].[dbo].[CounterDelay]" +
            //                                                             "WHERE jobNumber = @JobNum");

            #endregion
        }

        /// <summary>
        /// Maintains and Updates :- Jobs Details / Supplier(ETS and Call Forwards) / ETS / Client
        /// </summary>
        static void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                ////////string tenant = "https://enterpriseuser.sharepoint.com/sites/worksite";
                ////////string userName = "officeuser@enterpriseuser.onmicrosoft.com";
                ////////string passwordString = "india@123";
                string tenant = appSettings.Get("URL");
                string userName = appSettings.Get("UserName2");
                string passwordString = appSettings.Get("Password2");

                if (appSettings.Count == 0)
                {
                    Console.WriteLine("AppSettings is empty.");
                }
                else
                {
                    string sqlQuery;
                    foreach (var key in appSettings.AllKeys)
                    {
                        sqlQuery = System.String.Empty;
                        switch (key)
                        {
                            case "JobsDataList":
                                Console.WriteLine(appSettings["JobsDataList"]);
                                sqlQuery = @"
                                                SELECT  [JobNumber]
                                                      ,[JobAddress]
                                                      ,[Supervisor]
                                                      ,[ConstructionManager]
                                                  FROM [WatersunData].[dbo].[vFrameworkJobs]
                                                where left(JobNumber,1) in (3,4,5)";

                                SqlSpConnect(tenant, userName, passwordString, key, appSettings[key], appSettings["JobsDataColumn"], sqlQuery);
                                break;
                            case "SuppliersList":
                                Console.WriteLine(appSettings["SuppliersList"]);
                                sqlQuery = @"
                                                SELECT [Supplier_Code]
                                                      ,[SupplierName]
                                                      ,[AccountEmail]
                                                      ,[GroupList]
                                                  FROM [WatersunData].[dbo].[vSupplierList] order by [GroupList], [Supplier_Code]
                                                ";
                                sqlQuery = @"
                                                SELECT [Supplier_Code]
                                                      ,[SupplierName]
                                                      ,[AccountEmail]
                                                      ,[GroupList]
                                                  FROM [WatersunData].[dbo].[vSupplierList] order by [GroupList], [SupplierName]
                                                ";

                                SqlSpConnect(tenant, userName, passwordString, key, appSettings[key], appSettings["SuppliersColumn"], sqlQuery);
                                break;
                            case "SuppliersList2":
                                Console.WriteLine(appSettings["SuppliersList2"]);
                                sqlQuery = @" SELECT distinct l_entity_id , s_name , s_name_ref FROM FworkSQLEcm.dbo.v_dlgSelEntity WHERE  l_entity_role_id <> 0 AND ( l_context_id = 0 OR l_context_id = 2 ) AND f_inactive = 0 ";

                                tenant = "https://networkintegration.sharepoint.com/sites/365Build/Watersun/WatersunJobs";
                                //SqlSpConnect(tenant, userName, passwordString, key, appSettings[key], appSettings["SuppliersColumn2"], sqlQuery);
                                SqlSpConnectEntity(tenant, userName, passwordString, key, appSettings[key], appSettings["SuppliersColumn2"], sqlQuery);
                                break;
                            case "ETSData":
                                Console.WriteLine(appSettings["ETSData"]);
                                #region query
                                sqlQuery = @"
                                                SELECT 
                                                    [Job], [ETS No], [ItemsDescription], [Selected Job], [Cost Centre], [Reason Code], [Supplier], [DeliveryDetails], [SupplierID], [DeliveryDate], [Price], [GST], [Created By], [Approved By], [Purchase Order], [RegeneratePO], [ID], [JobID], [ETSId], [CostCentreID], [Created], [Complete], [Recharge], [RechargeID], [RechargeAmount], [RechargeAMSupID], [RechargeNZSupID], [ReasonDescription], [RechargeSupId],[Status]
                                                FROM [WatersunData].[dbo].[ETSDataExportDemo]
                                                ";
                                #endregion

                                SqlSpConnect(tenant, userName, passwordString, key, appSettings[key], appSettings["ETSDataColumn"], sqlQuery);
                                break;
                            case "JobsSuppList":
                                Console.WriteLine(appSettings["JobsSuppList"]);
                                sqlQuery = @"
                                                SELECT  [JobNumber]
                                                      ,[JobAddress]
                                                      ,[Supervisor]
                                                      ,[ConstructionManager]
                                                      ,[filteredAddress]
                                                  FROM [WatersunData].[dbo].[vFrameworkJobs]
                                                where left(JobNumber,1) in (3,4,5)";
                                UpdateSqlSpConnect(tenant, userName, passwordString, key, appSettings[key], appSettings["JobsIDColumn"], appSettings["JobsSuppColumn"], sqlQuery);
                                break;
                            case "ClientData":
                                Console.WriteLine(appSettings["ClientData"]);
                                sqlQuery = @"
                                             select 
	                                            JobId, Client, Contact, Salutation, JobNumber, JobAddress, JobStatus--, Stage
                                             from [WatersunData].[dbo].[vClient]  
                                             order by jobid desc
                                           ";
                                SqlSpConnect(tenant, userName, passwordString, key, appSettings[key], appSettings["ClientDataColumn"], sqlQuery);
                                break;
                            case "UpdateEtsList":
                                Console.WriteLine(appSettings["UpdateEtsList"]);
                                sqlQuery = @"Select distinct ETSNo as ETS_x0020_No, ProcessedDB as Complete from [WatersunData].[dbo].[vwETSRecordsProcessed]";
                                UpdateSqlSpConnect(tenant, userName, passwordString, key, appSettings[key], appSettings["UpdateEtsIDColumn"], appSettings["UpdateEtsTargetColumn"], sqlQuery);
                                break;
                            default:
                                Console.WriteLine(key);
                                break;
                        }
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        /// <summary>
        /// Inserts and Updates the Call Forward and Delay delays
        /// </summary>
        static void CallForward() 
        {
            var appSettings = ConfigurationManager.AppSettings;
            string tenant = appSettings.Get("URLJobs");
            string userName = appSettings.Get("UserName2");
            string passwordString = appSettings.Get("Password2");

            //string tenant = "https://networkintegration.sharepoint.com/sites/365Build/Watersun/WatersunJobs";
            //string userName = "andrew@365build.com.au";
            //string passwordString = "ch@lleng3r";

            string sqlQuery = @"select JobNum,JobId,JobAddr, client,delay, week,overall, JobDelayLink,JobLink,Supervisor,ConstructionManager,JobsDelay,JobsDetail  from [WatersunData].[dbo].[vJobDelay]
                                            where JobNum ='5468'";//*/   /*5137*/
            /*string sqlQuery = @"select JobNum,JobId,JobAddr, client,delay, week,overall, JobDelayLink,JobLink,Supervisor,ConstructionManager,JobsDelay,JobsDetail  from [WatersunData].[dbo].[vJobDelay]
                                                    where JobNum in (SELECT  cast([JobNumber] as varchar) FROM [WatersunData].[dbo].[vFrameworkJobs] where left(JobNumber,1) in (4,5))";//*/
            
            //string sqlQuery = ConfigurationManager.AppSettings.Get("qryGetAllJobs");

            /*string sqlQuery = @"select JobNum,JobId,JobAddr, client,delay, week,overall, JobDelayLink,JobLink,Supervisor,ConstructionManager,JobsDelay,JobsDetail  
                                 from [WatersunData].[dbo].[vJobDelay]
                                 where JobNum in (SELECT  cast([JobNumber] as varchar) 
					                                FROM [WatersunData].[dbo].[vFrameworkJobs] 
					                                where JobNumber not in (select jobNum from [WatersunData].dbo.jobNmDemo) 
							                                and left(JobNumber,1) in (4,5))
						                                ";//*/

            dbConnection conn = new dbConnection();
            DataTable tempTable = conn.executeSelectNoParameter(sqlQuery);
            //SqlToSpCreateJobsMetadata(tenant, userName, passwordString, "Call Forwards", "Call Forwards", "JobNum", tempTable);
            ///////////////////////////SqlToSpCreateJobsMetadata(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable);

            #region Jobs Data
            //Jobs Data
            string queryJobsItems = string.Concat("select ReqDays,s_costCentreCode,SupplierId,JobId, JobNum, CstCallId, Activity, called, calledfor, start, complete, Supervisor, ISNULL(Supplier,'[NULL]') as Supplier, Duration, calledBest,calledforBest, startBest,completeBest,b_complete,dateModified,timeModified " +
                                                             "from [WatersunData].[dbo].[vJobsCalledForDates] " +
                                                             "WHERE JobNum = @JobNum order by l_order");
            SqlToSpCreateJobsData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsItems);
            SpToSqlCreateJobsData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsItems);
            #endregion

            #region Jobs Delay
            //Jobs Delay
            //string queryJobsDelayItems = string.Concat("select Reason,Comments,DelayDate,ToDelayDate,DelayId,JobNumber,JobId,CstId,dateModified,timeModified From [WatersunData].[dbo].[vPerJobDelay] WHERE JobNumber = @JobNumber");
            //string queryJobsDelayItemsSpToSql = "SELECT [JobNumber] ,[Reason],[Comments],[JobId],[l_cst_id],[DelayId],[l_cst_dlyClass_id],[s_name],[DelayDate],[l_cst_dlyReas_id],[ToDelayDate],[Delay],[i_delAllowed_wD],dateModified,timeModified FROM [WatersunData].[dbo].[v_JobDelaysNoCondition] where jobnumber = @JobNumber";

            string queryJobsDelayItems = ConfigurationManager.AppSettings.Get("PerJobDelayItems");
            string queryJobsDelayItemsSpToSql = ConfigurationManager.AppSettings.Get("JobDelaysNoCondition");

            ConfigurationManager.AppSettings.Get("qryGetAllJobs");

            SqlToSpCreateJobsDelayData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsDelayItems);
            SpToSqlCreateJobsDelayData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsDelayItemsSpToSql);

            #endregion

            #region Jobs Data
            ////Jobs Data
            //string queryJobsItems = string.Concat("select ReqDays,s_costCentreCode,SupplierId,JobId, JobNum, CstCallId, Activity, called, calledfor, start, complete, Supervisor, ISNULL(Supplier,'[NULL]') as Supplier, Duration, calledBest,calledforBest, startBest,completeBest,b_complete,dateModified,timeModified " +
            //                                                 "from [WatersunData].[dbo].[vJobsCalledForDates] " +
            //                                                 "WHERE JobNum = @JobNum order by l_order");
            //SqlToSpCreateJobsData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsItems);
            //SpToSqlCreateJobsData(tenant, userName, passwordString, "JobTasks", "JobTasks", "JobNum", tempTable, queryJobsItems);
            #endregion


            queryId = string.Concat("SELECT [ID],[jobNumber],[jobId] ,[cstId] ,[SpId],[delayId] ,[dCreated] FROM [WatersunData].[dbo].[CounterDelay]" +
                                                             "WHERE jobNumber = @JobNum");

        }

        /// <summary>
        /// Inserts Jobs / ETS / Client / ETS-Supplier
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="userName"></param>
        /// <param name="passwordString"></param>
        /// <param name="key"></param>
        /// <param name="appSettingsKey"></param>
        /// <param name="columnName"></param>
        /// <param name="sqlQuery"></param>
        private static void SqlSpConnect(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, string sqlQuery)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);
                //ListItemCollection getListItemsCol = gl.getListData(tenant, userName, passwordString, appSettingsKey);
                
                #region Get List Item Collection from SP

                    ListItemCollection getListItemsCol = null;
                    //ListItemCollection getListItemsCol = null;
                    List getList = ctx.Web.Lists.GetByTitle(appSettingsKey);
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<View><Query></Query></View>";
                    ListItemCollection getListItemsCollection = getList.GetItems(camlQuery);
                    ctx.Load(getListItemsCollection);
                    ctx.ExecuteQuery();

                    if (getListItemsCollection != null && getListItemsCollection.Count > 0)
                    {
                        getListItemsCol = getListItemsCollection;
                    }

                #endregion

                DataTable dt = new DataTable();

                if (key == "SuppliersList" || key == "JobsDataList" || key == "ClientData" || key == "SuppliersList2")
                {
                    dt.Columns.Add("JobNumber");
                    if (getListItemsCol != null)
                    {
                        foreach (ListItem listItemsCol in getListItemsCol)
                        {
                            DataRow dr = dt.NewRow();
                            dr["JobNumber"] = listItemsCol[columnName];
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["JobNumber"] = "";
                        dt.Rows.Add(dr);
                    }
                }
                else if (key == "ETSData")
                {
                    dt.Columns.Add("Job");
                    dt.Columns.Add("ETSId");
                    dt.Columns.Add("ItemsDescription");
                    dt.Columns.Add("Selected_x0020_Job");
                    dt.Columns.Add("Cost_x0020_Centre");
                    dt.Columns.Add("Reason_x0020_Code");
                    dt.Columns.Add("Supplier");
                    dt.Columns.Add("DeliveryDetails");
                    dt.Columns.Add("SupplierID");
                    dt.Columns.Add("DeliveryDate");
                    dt.Columns.Add("Price");
                    dt.Columns.Add("GST");
                    dt.Columns.Add("Author");
                    dt.Columns.Add("Approved_x0020_By");
                    dt.Columns.Add("RegeneratePO");
                    dt.Columns.Add("ID");
                    dt.Columns.Add("JobID");
                    dt.Columns.Add("CostCentreID");
                    dt.Columns.Add("Created");
                    dt.Columns.Add("Complete");
                    dt.Columns.Add("Recharge");
                    dt.Columns.Add("RechargeID");
                    dt.Columns.Add("Recharge_x0020_Amount");
                    dt.Columns.Add("RechargeAMSupID");
                    dt.Columns.Add("RechargeNZSupID");
                    dt.Columns.Add("ReasonDescription");
                    dt.Columns.Add("RechargeSupplierID");
                    dt.Columns.Add("CancelETS");
                    dt.Columns.Add("Status");

                    foreach (ListItem listItemsCol in getListItemsCol)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Job"] = listItemsCol["Title"];
                        dr["ETSId"] = listItemsCol["ETSId"];
                        dr["ItemsDescription"] = listItemsCol["ItemsDescription"];
                        dr["Selected_x0020_Job"] = listItemsCol["Selected_x0020_Job"];
                        dr["Cost_x0020_Centre"] = listItemsCol["Cost_x0020_Centre"];
                        dr["Reason_x0020_Code"] = listItemsCol["Reason_x0020_Code"];
                        dr["Supplier"] = listItemsCol["Supplier"];
                        dr["DeliveryDetails"] = listItemsCol["DeliveryDetails"];
                        dr["SupplierID"] = listItemsCol["ActualSupplierID"]; //SupplierID
                        dr["DeliveryDate"] = listItemsCol["DeliveryDate"];
                        dr["Price"] = listItemsCol["Price"];
                        dr["GST"] = listItemsCol["GST"];
                        dr["Status"] = listItemsCol["Status"];

                        if (listItemsCol["Author"] == null)
                        {
                            listItemsCol["Author"] = "";
                        }
                        else
                        {
                            FieldUserValue userAuthor = (FieldUserValue)listItemsCol["Author"];
                            dr["Author"] = userAuthor.LookupValue;
                        }
                        if (listItemsCol["Approved_x0020_By"] == null)
                        {
                            listItemsCol["Approved_x0020_By"] = "";
                        }
                        else
                        {
                            FieldUserValue userAuthor = (FieldUserValue)listItemsCol["Approved_x0020_By"];
                            dr["Approved_x0020_By"] = userAuthor.LookupValue;
                        }
                        dr["RegeneratePO"] = listItemsCol["RegeneratePO"];
                        dr["ID"] = listItemsCol["ID"];
                        dr["JobID"] = listItemsCol["JobID"];
                        dr["CostCentreID"] = listItemsCol["CostCentreID"];
                        dr["Created"] = listItemsCol["Created"];
                        dr["Complete"] = listItemsCol["Complete"];
                        dr["Recharge"] = listItemsCol["Recharge"];
                        dr["RechargeID"] = listItemsCol["RechargeID"];
                        dr["Recharge_x0020_Amount"] = listItemsCol["Recharge_x0020_Amount"];
                        dr["RechargeAMSupID"] = listItemsCol["RechargeAMSupID"];
                        dr["RechargeNZSupID"] = listItemsCol["RechargeNZSupID"];
                        dr["ReasonDescription"] = listItemsCol["ReasonDescription"];
                        dr["RechargeSupplierID"] = listItemsCol["RechargeSupId"];
                        dr["CancelETS"] = listItemsCol["CancelETS"];
                        dt.Rows.Add(dr);
                    }
                }

                dbConnection conn = new dbConnection();
                DataTable tempTable = null;
                tempTable = conn.executeSelectNoParameter(sqlQuery);
                ListItemCreationInformation itemCreateInfo = null;
                ListItem oListItem = null;
                List oList = ctx.Web.Lists.GetByTitle(appSettingsKey);

                if (key == "SuppliersList" || key == "JobsDataList" || key == "ClientData" || key == "SuppliersList2")
                {
                    Excel.Application xlApp;
                    Excel.Workbook xlWorkBook;
                    Excel.Worksheet xlWorkSheet;
                    object misValue = System.Reflection.Missing.Value;

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Add(misValue);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    {
                        DataRow[] drExists = dt.Select("JobNumber = '" + tempTable.Rows[i][0].ToString() + "'");
                        if (drExists != null && drExists.Length > 0)
                        {
                            Console.WriteLine("Found - " + tempTable.Rows[i][0].ToString());
                            if (key == "JobsDataList")
                            {
                                xlWorkSheet.Cells[i + 1, 1] = "Found Job Number - ";
                                xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            }
                            else if (key == "SuppliersList")
                            {
                                xlWorkSheet.Cells[i + 1, 1] = "Found Supplier Code - ";
                                xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            }
                            else if (key == "ClientData")
                            {
                                xlWorkSheet.Cells[i + 1, 1] = "Found Client with JobId - ";
                                xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            }
                            else if (key == "SuppliersList2")
                            {
                                xlWorkSheet.Cells[i + 1, 1] = "Found Supplier Code - ";
                                xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Inserting - " + tempTable.Rows[i][0].ToString());
                            itemCreateInfo = new ListItemCreationInformation();
                            oListItem = oList.AddItem(itemCreateInfo);
                            if (key == "JobsDataList")
                            {
                                oListItem["Title"] = tempTable.Rows[i][0].ToString();
                                oListItem["Job_x0020_Address"] = tempTable.Rows[i][1].ToString();
                                oListItem["Job_x0020_Supervisor"] = tempTable.Rows[i][2].ToString();
                                oListItem["ConstructionManager"] = tempTable.Rows[i][3].ToString();
                                xlWorkSheet.Cells[i + 1, 1] = "Inserting Job Number - ";
                                xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            }
                            else if (key == "SuppliersList")
                            {
                                oListItem["Title"] = tempTable.Rows[i][0].ToString();
                                oListItem["SupplierCode"] = tempTable.Rows[i][0].ToString();
                                oListItem["SupplierName"] = tempTable.Rows[i][1].ToString();
                                oListItem["SupplierEmail"] = tempTable.Rows[i][2].ToString();
                                //oListItem["ListGroup"] = tempTable.Rows[i][3].ToString();
                                oListItem["ListGroup"] = Int32.Parse(tempTable.Rows[i][3].ToString());
                                xlWorkSheet.Cells[i + 1, 1] = "Inserting Supplier Code - ";
                                xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            }
                            //else if (key == "SuppliersList2")
                            //{
                            //    oListItem["SupId"] = Int32.Parse(tempTable.Rows[i]["l_entity_id"].ToString());
                            //    oListItem["SupName"] = tempTable.Rows[i]["s_name"].ToString();
                            //    xlWorkSheet.Cells[i + 1, 1] = "Inserting Supplier Code - ";
                            //    xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            //}
                            else if (key == "ClientData")
                            {
                                //oListItem["l_job_id"] = tempTable.Rows[i][0].ToString();
                                oListItem["JobId"] = Int32.Parse(tempTable.Rows[i][0].ToString());
                                oListItem["Client"] = tempTable.Rows[i][1].ToString();
                                oListItem["Contact"] = tempTable.Rows[i][2].ToString();
                                oListItem["Salutation"] = tempTable.Rows[i][3].ToString();
                                oListItem["JobNumber"] = tempTable.Rows[i][4].ToString();
                                oListItem["JobAddress"] = tempTable.Rows[i][5].ToString();
                                oListItem["JobStatus"] = tempTable.Rows[i][6].ToString();
                                xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            }

                            oListItem.Update();
                            ctx.ExecuteQuery();
                        }
                    }
                    //xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_Inserting" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.SaveAs(ConfigurationManager.AppSettings.Get("LogLocation") + "\\SqlToSpLog_Inserting" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);                    
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    gl.releaseObject(xlWorkSheet);
                    gl.releaseObject(xlWorkBook);
                    gl.releaseObject(xlApp);
                }
                else if (key == "ETSData")
                {
                    string job, eTSNo, itemsDescription, selectedJob, costCentre, reasonCode, supplier, deliveryDetails, supplierID, deliveryDate, price, gST, createdBy, approvedBy, regeneratePO, jobID, eTSId, costCentreID, created, recharge, RechargeID, RechargeAmount, RechargeAMSupID, RechargeNZSupID, ReasonDescription, CancelETS,status;
                    float id, complete;
                    int RechargeSupplierID;

                    Excel.Application xlApp;
                    Excel.Workbook xlWorkBook;
                    Excel.Worksheet xlWorkSheet;
                    object misValue = System.Reflection.Missing.Value;

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Add(misValue);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow[] drExists = tempTable.Select("ETSId = '" + dt.Rows[i][1].ToString() + "'");
                        if (drExists != null && drExists.Length > 0)
                        {
                            Console.WriteLine("Found - " + dt.Rows[i][1].ToString());
                            xlWorkSheet.Cells[i + 1, 1] = "Found EtsId - ";
                            xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i][1].ToString();
                        }
                        else
                        {
                            try
                            {
                                Console.WriteLine("Inserting - " + dt.Rows[i][1].ToString());
                                xlWorkSheet.Cells[i + 1, 1] = "Inserting EtsId - ";
                                xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i][1].ToString();

                                job = dt.Rows[i][0].ToString();
                                eTSId = dt.Rows[i][1].ToString();

                                eTSNo = "E" + eTSId;
                                //if (Int32.Parse(eTSId) > 999) { eTSNo = "E0000" + eTSId; }
                                //else if (Int32.Parse(eTSId) > 99) { eTSNo = "E00000" + eTSId; }
                                //else if (Int32.Parse(eTSId) > 9) { eTSNo = "E00000" + eTSId; }
                                //else { eTSNo = "E" + eTSId; }
                                itemsDescription = dt.Rows[i][2].ToString();
                                selectedJob = dt.Rows[i][3].ToString();
                                costCentre = dt.Rows[i][4].ToString();
                                reasonCode = dt.Rows[i][5].ToString();
                                supplier = dt.Rows[i][6].ToString();
                                deliveryDetails = dt.Rows[i][7].ToString();
                                supplierID = dt.Rows[i][8].ToString();
                                deliveryDate = string.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? DateTime.Today.ToString() : dt.Rows[i][9].ToString();
                                price = string.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? "0" : dt.Rows[i][10].ToString();
                                gST = dt.Rows[i][11].ToString();
                                createdBy = dt.Rows[i][12].ToString();
                                approvedBy = dt.Rows[i][13].ToString();
                                regeneratePO = string.IsNullOrEmpty(dt.Rows[i][14].ToString()) ? DateTime.Today.ToString() : dt.Rows[i][14].ToString();
                                id = float.Parse(dt.Rows[i][15].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                jobID = dt.Rows[i][16].ToString();
                                costCentreID = dt.Rows[i][17].ToString();
                                created = string.IsNullOrEmpty(dt.Rows[i][18].ToString()) ? DateTime.Today.ToString() : dt.Rows[i][18].ToString();
                                //complete = float.Parse(dt.Rows[i][19].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                complete = float.Parse(string.IsNullOrEmpty(dt.Rows[i][19].ToString()) ? "0" : dt.Rows[i][19].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                recharge = dt.Rows[i][20].ToString();
                                RechargeID = dt.Rows[i][21].ToString();
                                RechargeAmount = string.IsNullOrEmpty(dt.Rows[i][22].ToString()) ? "0" : dt.Rows[i][22].ToString(); //dt.Rows[i][29].ToString();
                                RechargeAMSupID = dt.Rows[i][23].ToString();
                                RechargeNZSupID = dt.Rows[i][24].ToString();
                                ReasonDescription = dt.Rows[i][25].ToString();
                                RechargeSupplierID = Int32.Parse(string.IsNullOrEmpty(dt.Rows[i][26].ToString()) ? "0" : dt.Rows[i][26].ToString());
                                CancelETS = dt.Rows[i][27].ToString();
                                status = dt.Rows[i][28].ToString();

                                //insert
                                string sclearsqlIns = string.Concat("INSERT INTO [WatersunData].[dbo].[ETSDataExportDemo]" +
                                                                       "([Job], [ETS No], [ItemsDescription], [Selected Job], [Cost Centre], [Reason Code], [Supplier], [DeliveryDetails], [SupplierID], [DeliveryDate], [Price], [GST], [Created By], [Approved By], [RegeneratePO], [ID], [JobID], [ETSId], [CostCentreID], [Created], [Complete], [Recharge], [RechargeID], [RechargeAmount], [RechargeAMSupID], [RechargeNZSupID], [ReasonDescription], [RechargeSupId], [CancelETS], [Status])" +
                                                                       "VALUES (@job, @eTSNo, @itemsDescription, @selectedJob, @costCentre, @reasonCode, @supplier, @deliveryDetails, @supplierID, @deliveryDate, @price, @gST, @createdBy, @approvedBy, @regeneratePO, @id, @jobID, @eTSId, @costCentreID, @created, @complete, @recharge,  @RechargeID, @RechargeAmount,   @RechargeAMSupID,  @RechargeNZSupID,  @ReasonDescription, @RechargeSupplierID ,@CancelETS,@status)");
                                SqlParameter[] parameterUpd = {                                

                                                    new SqlParameter("@job", SqlDbType.NVarChar) { Value = job },
                                                    new SqlParameter("@eTSNo", SqlDbType.NVarChar) { Value = eTSNo },
                                                    new SqlParameter("@itemsDescription", SqlDbType.NVarChar) { Value = itemsDescription },
                                                    new SqlParameter("@selectedJob", SqlDbType.NVarChar) { Value = selectedJob },
                                                    new SqlParameter("@costCentre", SqlDbType.NVarChar) { Value = costCentre },
                                                    new SqlParameter("@reasonCode", SqlDbType.NVarChar) { Value = reasonCode },
                                                    new SqlParameter("@supplier", SqlDbType.NVarChar) { Value = supplier },
                                                    new SqlParameter("@deliveryDetails", SqlDbType.NVarChar) { Value = deliveryDetails },
                                                    new SqlParameter("@supplierID", SqlDbType.NVarChar) { Value = supplierID },
                                                    new SqlParameter("@deliveryDate", SqlDbType.DateTime) { Value = deliveryDate },
                                                    new SqlParameter("@price", SqlDbType.Money) { Value = price },
                                                    new SqlParameter("@gST", SqlDbType.NVarChar) { Value = gST },
                                                    new SqlParameter("@createdBy", SqlDbType.NVarChar) { Value = createdBy },
                                                    new SqlParameter("@approvedBy", SqlDbType.NVarChar) { Value = approvedBy },
                                                    new SqlParameter("@regeneratePO", SqlDbType.DateTime) { Value = regeneratePO },
                                                    new SqlParameter("@id", SqlDbType.Float) { Value = id },
                                                    new SqlParameter("@jobID", SqlDbType.NVarChar) { Value = jobID },
                                                    new SqlParameter("@eTSId", SqlDbType.NVarChar) { Value = eTSId },
                                                    new SqlParameter("@costCentreID", SqlDbType.NVarChar) { Value = costCentreID },
                                                    new SqlParameter("@created", SqlDbType.DateTime) { Value = created },
                                                    new SqlParameter("@complete", SqlDbType.Float) { Value = complete },
                                                    new SqlParameter("@recharge", SqlDbType.NVarChar) { Value = recharge },
                                                    new SqlParameter("@RechargeID", SqlDbType.NVarChar) { Value = RechargeID },
                                                    new SqlParameter("@RechargeAmount", SqlDbType.Money) { Value = RechargeAmount },
                                                    new SqlParameter("@RechargeAMSupID", SqlDbType.NVarChar) { Value = RechargeAMSupID },
                                                    new SqlParameter("@RechargeNZSupID", SqlDbType.NVarChar) { Value = RechargeNZSupID },
                                                    new SqlParameter("@ReasonDescription", SqlDbType.NVarChar) { Value = ReasonDescription },
                                                    new SqlParameter("@RechargeSupplierID", SqlDbType.Int) { Value = RechargeSupplierID },
                                                    new SqlParameter("@CancelETS", SqlDbType.NVarChar) { Value = CancelETS },
                                                    new SqlParameter("@status", SqlDbType.NVarChar) { Value = status }

                                                         };
                                bool isInsert = conn.executeInsertQuery(sclearsqlIns, parameterUpd);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                GlobalLogic.ExceptionHandle(e, "SqlSpConnect" + "---" + "Export ETS to DB");
                            }
                        }
                    }
                    //xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_Inserting" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.SaveAs(ConfigurationManager.AppSettings.Get("LogLocation") + "\\SqlToSpLog_Inserting" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    gl.releaseObject(xlWorkSheet);
                    gl.releaseObject(xlWorkBook);
                    gl.releaseObject(xlApp);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                GlobalLogic.ExceptionHandle(e, "SqlSpConnect");
            }
            finally
            {
                //dt.Dispose();
            }
        }

        /// <summary>
        /// Updates Jobs and ETS details
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="userName"></param>
        /// <param name="passwordString"></param>
        /// <param name="key"></param>
        /// <param name="appSettingsKey"></param>
        /// <param name="columnName"></param>
        /// <param name="columnName2"></param>
        /// <param name="sqlQuery"></param>
        private static void UpdateSqlSpConnect(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, string columnName2, string sqlQuery)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                //ListItemCollection getListItemsCol = gl.getListData(tenant, userName, passwordString, appSettingsKey);

                #region Get List Item Collection from SP

                ListItemCollection getListItemsCol = null;
                //ListItemCollection getListItemsCol = null;
                List getList = ctx.Web.Lists.GetByTitle(appSettingsKey);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query></Query></View>";
                ListItemCollection getListItemsCollection = getList.GetItems(camlQuery);
                ctx.Load(getListItemsCollection);
                ctx.ExecuteQuery();

                if (getListItemsCollection != null && getListItemsCollection.Count > 0)
                {
                    getListItemsCol = getListItemsCollection;
                }

                #endregion

                DataTable dt = new DataTable();

                if (key == "JobsSuppList")
                {
                    dt.Columns.Add("ID");
                    dt.Columns.Add("JobNumber");                    
                    dt.Columns.Add("SuppName");
                    dt.Columns.Add("ConstructionManager");
                    dt.Columns.Add("JobAddress");
                    if (getListItemsCol != null)
                    {
                        foreach (ListItem listItemsCol in getListItemsCol)
                        {
                            DataRow dr = dt.NewRow();
                            dr["JobNumber"] = listItemsCol[columnName];                            
                            dr["SuppName"] = listItemsCol[columnName2];
                            dr["ConstructionManager"] = listItemsCol["ConstructionManager"];
                            dr["ID"] = listItemsCol["ID"];
                            dr["JobAddress"] = listItemsCol["Job_x0020_Address"];
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["JobNumber"] = "";                        
                        dr["SuppName"] = "";
                        dr["ConstructionManager"] = "";
                        dr["ID"] = "";
                        dr["JobAddress"] = "";
                        dt.Rows.Add(dr);
                    }
                }
                else if (key == "UpdateEtsList")
                {
                    dt.Columns.Add("ID");
                    dt.Columns.Add(columnName);
                    dt.Columns.Add(columnName2);
                    if (getListItemsCol != null)
                    {
                        foreach (ListItem listItemsCol in getListItemsCol)
                        {
                            DataRow dr = dt.NewRow();
                            dr[columnName] = listItemsCol[columnName];
                            dr[columnName2] = listItemsCol[columnName2];
                            dr["ID"] = listItemsCol["ID"];
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr[columnName] = "";
                        dr[columnName2] = "";
                        dr["ID"] = "";
                        dt.Rows.Add(dr);
                    }
                }

                dbConnection conn = new dbConnection();
                DataTable tempTable = null;
                tempTable = conn.executeSelectNoParameter(sqlQuery);
                ListItemCreationInformation itemCreateInfo = null;
                ListItem oListItem = null;
                List oList = ctx.Web.Lists.GetByTitle(appSettingsKey);

                if (key == "JobsSuppList" || key == "UpdateEtsList")
                {
                    Excel.Application xlApp;
                    Excel.Workbook xlWorkBook;
                    Excel.Worksheet xlWorkSheet;
                    object misValue = System.Reflection.Missing.Value;

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Add(misValue);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    if (key == "JobsSuppList")
                    {
                        for (int i = 0; i < tempTable.Rows.Count; i++)
                        {
                            DataRow[] drExists = dt.Select("JobNumber = '" + tempTable.Rows[i][0].ToString() + "'");
                            if (drExists != null && drExists.Length > 0)
                            {
                                DataRow[] drExists2 = dt.Select("JobNumber = '" + tempTable.Rows[i][0].ToString() + "'" 
                                                                    + " AND SuppName = '" + tempTable.Rows[i][2].ToString() + "'" 
                                                                    + "AND ConstructionManager = '" + tempTable.Rows[i][3].ToString() + "'"
                                                                    + " AND JobAddress = '" + tempTable.Rows[i]["filteredAddress"].ToString() + "'");
                                if (drExists2 != null && drExists2.Length > 0)
                                {
                                    Console.WriteLine("Found - " + tempTable.Rows[i][0].ToString());
                                    xlWorkSheet.Cells[i + 1, 1] = "Found Job Number - ";
                                    xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                                }
                                else
                                {
                                    Console.WriteLine("Update - " + tempTable.Rows[i][0].ToString());
                                    oListItem = oList.GetItemById(drExists[0].ItemArray[0].ToString());
                                    oListItem["Job_x0020_Supervisor"] = tempTable.Rows[i][2].ToString();
                                    oListItem["ConstructionManager"] = tempTable.Rows[i][3].ToString();
                                    oListItem["Job_x0020_Address"] = tempTable.Rows[i]["filteredAddress"].ToString();
                                    xlWorkSheet.Cells[i + 1, 1] = "Updating Supervisor - ";
                                    xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                                    oListItem.Update();
                                    ctx.ExecuteQuery();
                                }
                            }
                        }
                    }
                    else if (key == "UpdateEtsList")
                    {
                        for (int i = 0; i < tempTable.Rows.Count; i++)
                        {
                            DataRow[] drExists = dt.Select(columnName + " = '" + tempTable.Rows[i][0].ToString() + "'");
                            if (drExists != null && drExists.Length > 0)
                            {
                                //DataRow[] drExists2 = dt.Select(columnName + " = '" + tempTable.Rows[i][0].ToString() + "'" + " AND " + columnName2 + " = '" + Int32.Parse(tempTable.Rows[i][1].ToString()));
                                DataRow[] drExists2 = dt.Select(columnName + " = '" + tempTable.Rows[i][0].ToString() + "'" + " AND " + columnName2 + " = " + Int32.Parse(tempTable.Rows[i][1].ToString()));
                                if (drExists2 != null && drExists2.Length > 0)
                                {
                                    Console.WriteLine("Found - " + tempTable.Rows[i][0].ToString());
                                    xlWorkSheet.Cells[i + 1, 1] = "Found ETSId - ";
                                    xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                                }
                                else
                                {
                                    Console.WriteLine("Update - " + tempTable.Rows[i][0].ToString());
                                    oListItem = oList.GetItemById(drExists[0].ItemArray[0].ToString());
                                    oListItem[columnName2] = tempTable.Rows[i][1].ToString();
                                    xlWorkSheet.Cells[i + 1, 1] = "Updating Complete Field - ";
                                    xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                                    oListItem.Update();
                                    ctx.ExecuteQuery();
                                }
                            }
                        }
                    }
                    //xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_Updating" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.SaveAs(ConfigurationManager.AppSettings.Get("LogLocation") + "\\SqlToSpLog_Updating" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    gl.releaseObject(xlWorkSheet);
                    gl.releaseObject(xlWorkBook);
                    gl.releaseObject(xlApp);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                GlobalLogic.ExceptionHandle(e, "Update ETS / ETS-Suppliers");
            }
            finally
            {
                //dt.Dispose();
            }
        }
        /*
        private static void CreateListWFs(string sqlQuery)
        {
            //string tenant = "https://networkintegration.sharepoint.com/sites/Development";
            string tenant = "https://networkintegration.sharepoint.com/sites/365Build/Watersun/WatersunJobs";
            string userName = "andrew@365build.com.au";
            string passwordString = "ch@lleng3r";
            string listName;// = "Test1234567";
            //string wfAssoc = "05639090-c09B-478A-B1F8-611718539D7F";//"05639090-c09b-478a-b1f8-611718539d7f";

            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);
                dbConnection conn = new dbConnection();
                DataTable tempTable = conn.executeSelectNoParameter(sqlQuery);
                //string query = @" select l_job_id ,s_job_num  ,l_cstL_call_id ,sLogisticsActivity  ,d_called_fBest ,d_calledFor_fBest ,d_start_fBest,d_complete_fBest  ,d_start_fMan ,d_called_fMan ,d_calledFor_fMan ,d_complete_fMan,d_called_fBas ,d_calledFor_fBas ,d_start_fBas,d_complete_fBas,d_called_act,d_calledFor_act, d_start_act ,d_complete_act from [WatersunData].[dbo].[vJobsCalledForDates] ";

                //Get all user names
                UserCollection user = ctx.Web.SiteUsers;
                //List<string> siteSupName = new List<string>();

                ctx.Load(user);
                ctx.ExecuteQuery();
                int u = 0;
                List<UserValues> uValue = new List<UserValues>(10000);
                foreach (User usr in user)
                //for (int u = 0; u < user.Count; u++)
                {
                    UserValues uv = new UserValues();
                    uv.Email = usr.Email;
                    //uv.Email = user.[u]["Email"];
                    uv.Id = usr.Id;
                    uv.LoginName = usr.LoginName;
                    uv.Title = usr.Title;

                    //UserValues[] uValue = new UserValues[10000];
                    //uValue[u] = uv;
                    //u = u + 1;

                    //List<UserValues> uValue = new List<UserValues>(10000);
                    uValue.Add(uv);
                }

                //Get All List Names
                Web web = ctx.Web;
                ctx.Load(web.Lists,
                             lists => lists.Include(list => list.Title, // For each list, retrieve Title and Id. 
                                                    list => list.Id));
                ctx.ExecuteQuery();

                List<string> colName = new List<string>();
                foreach (List list in web.Lists)
                {
                    colName.Add(list.Title);
                }

                // Create Lists
                if (tempTable != null && tempTable.Rows.Count > 0)
                {
                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    {
                        string jobNum = tempTable.Rows[i][0].ToString();
                        listName = tempTable.Rows[i][1].ToString() + "_Data";
                        //listName = "JobsMasterList";

                        //bool listExists = false;
                        //for (int k = 0; i < colName.Count; k++)
                        //{
                        //    if (listName == colName[k].ToString())
                        //    {
                        //        listExists = true;
                        //        //Console.WriteLine("List:- " + listName + " already exists");
                        //    } 
                        //}
                        if (2 == 1)
                        {
                            Console.WriteLine("List:- " + listName + " already exists");
                        }
                        else
                        {
                            if (gl.createList(ctx, listName, (int)ListTemplateType.TasksWithTimelineAndHierarchy))
                            {
                                if (gl.createListColumns(ctx, listName))
                                {
                                    if (gl.createListView(ctx, listName))
                                    {
                                        Guid guid = gl.getListGuid(ctx, listName);
                                        if (guid != Guid.Empty)
                                        //if (gl.getListGuid(ctx, listName) != Guid.Empty)
                                        {
                                            string query = string.Concat("select l_job_id ,s_job_num  ,l_cstL_call_id ,sLogisticsActivity  ,d_called_fBest ,d_calledFor_fBest ,d_start_fBest,d_complete_fBest  ,d_start_fMan ,d_called_fMan ,d_calledFor_fMan ,d_complete_fMan,d_called_fBas ,d_calledFor_fBas ,d_start_fBas,d_complete_fBas,d_called_act,d_calledFor_act, d_start_act , d_complete_act, Supervisor from [WatersunData].[dbo].[vJobsCalledForDates] " +
                                                                                 "WHERE s_job_num = @s_job_num");
                                            SqlParameter[] parameter = {                                
                                                        new SqlParameter("@s_job_num", SqlDbType.VarChar) { Value = jobNum }
                                                         };
                                            DataTable calledforDatesData = conn.executeSelectQuery(query, parameter);
                                            if (gl.createListItems(ctx, calledforDatesData, listName, uValue))
                                            {
                                                gl.addWorkflowSubscription(ctx, listName, guid);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //}
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void SqlToSpDelays(string sqlQuery)
        {
            string tenant = "https://networkintegration.sharepoint.com/sites/Development/SchedulingWeb/";
            string userName = "andrew@365build.com.au";
            string passwordString = "ch@lleng3r";
            string listName;

            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);
                dbConnection conn = new dbConnection();
                DataTable tempTable = conn.executeSelectNoParameter(sqlQuery);

                // Create SubSites
                if (tempTable != null && tempTable.Rows.Count > 0)
                {
                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    { //ctx = gl.ConnectSP(tenant, userName, passwordString);
                        string jobNum = tempTable.Rows[i][0].ToString();
                        listName = tempTable.Rows[i][1].ToString();
                        if (gl.createSubsite(ctx, listName))
                        {
                            string tenantSubSite = tenant + listName + "/";
                            ClientContext ctxSubSite = gl.ConnectSP(tenantSubSite, userName, passwordString);
                            if (gl.activateFeature(ctxSubSite))
                            {
                                if (gl.createList(ctxSubSite, listName, (int)ListTemplateType.GenericList))
                                {
                                    if (gl.createDelayListColumns(ctxSubSite, listName))
                                    {
                                        if (gl.createDelayListView(ctxSubSite, listName))
                                        {
                                            string query = string.Concat("select Start, To, Reason, Title from [WatersunData].[dbo].[vJobsCalledForDates] " +
                                                                                 "WHERE s_job_num = @s_job_num");
                                            SqlParameter[] parameter = {                                
                                                        new SqlParameter("@s_job_num", SqlDbType.VarChar) { Value = jobNum }
                                                         };
                                            DataTable delayJobsData = conn.executeSelectQuery(query, parameter);
                                            if (gl.createDelayListItems(ctxSubSite, delayJobsData, listName))
                                            {
                                                if (gl.createList(ctxSubSite, "JobMetadata", (int)ListTemplateType.GenericList))
                                                {
                                                    if (gl.createDelayMetadataListView(ctxSubSite, "JobMetadata"))
                                                    {
                                                        string queryMetadata = string.Concat("select JobNum, Client, Delay, Week, Overall, Address, Forcast from [WatersunData].[dbo].[vJobsCalledForDates] " +
                                                                                             "WHERE s_job_num = @s_job_num");
                                                        SqlParameter[] parameterMetadata = {                                
                                                                    new SqlParameter("@s_job_num", SqlDbType.VarChar) { Value = jobNum }
                                                                     };
                                                        DataTable delayJobsMetadata = conn.executeSelectQuery(queryMetadata, parameterMetadata);
                                                        if (gl.createDelayMetadataListItems(ctxSubSite, delayJobsMetadata, "JobMetadata"))
                                                        {
                                                            //Console.WriteLine("Completed for job :- "+listName);
                                                            //gl.addWorkflowSubscription(ctx, listName, guid);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
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

        private static void SqlToSpJobs(string sqlQuery)
        {
            //string tenant = "https://networkintegration.sharepoint.com/sites/Development";
            string tenant = "https://networkintegration.sharepoint.com/sites/365Build/Watersun/WatersunJobs";
            string userName = "andrew@365build.com.au";
            string passwordString = "ch@lleng3r";
            string listName;

            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);
                dbConnection conn = new dbConnection();

                #region Metadata
                //string sqlQueryMetadata = "select top 10 JobNum,JobAddr, client,delay, week,overall, JobDelayLink,JobLink  from [WatersunData].[dbo].[vJobDelay] order by jobnum desc";
                string sqlQueryMetadata = @"select JobNum,JobAddr, client,delay, week,overall, JobDelayLink,JobLink,Supervisor,ConstructionManager,JobsDelay,JobsDetail  from [WatersunData].[dbo].[vJobDelay]
                                            where JobNum in ('5225','5213',	'5211',	'5193',	'5172',	'5137',	'5133',	'5088',	'5072',	'5066')";
                //where left(JobNum,1) in (3,4,5) order by JobNum
                DataTable tempTableMetadata = conn.executeSelectNoParameter(sqlQueryMetadata);

                //UserValues: Begin
                UserCollection user = ctx.Web.SiteUsers;
                ctx.Load(user);
                ctx.ExecuteQuery();
                List<UserValues> uValue = new List<UserValues>(10000);
                foreach (User usr in user)
                {
                    UserValues uv = new UserValues();
                    uv.Email = usr.Email;
                    uv.Id = usr.Id;
                    uv.LoginName = usr.LoginName;
                    uv.Title = usr.Title;
                    uValue.Add(uv);
                }
                //UserValues: End
                //Metadata insert Columns: Begin
                List<ColumnTypes> colNameMetadata = new List<ColumnTypes>(10000);
                ListItemCollection getListItemsCol = gl.getListDataVal(ctx, "MetadataListColumns");
                if (getListItemsCol != null)
                {
                    foreach (ListItem listItemsCol in getListItemsCol)
                    {
                        ColumnTypes colNames = new ColumnTypes();

                        if (listItemsCol["columnName"] == null) { colNames.columnName = ""; } else { colNames.columnName = listItemsCol["columnName"].ToString(); }
                        if (listItemsCol["columnType"] == null) { colNames.columnType = ""; } else { colNames.columnType = listItemsCol["columnTypes"].ToString(); }
                        if (listItemsCol["isLookup"] == null) { colNames.isLookup = false; } else { colNames.isLookup = Convert.ToBoolean(listItemsCol["isLookup"].ToString()); }
                        if (listItemsCol["lookupListGuid"] == null) { colNames.lookupListGuid = ""; } else { colNames.lookupListGuid = listItemsCol["lookupListGuid"].ToString(); }
                        if (listItemsCol["lookupColumnName"] == null) { colNames.lookupColumnName = ""; } else { colNames.lookupColumnName = listItemsCol["lookupColumnName"].ToString(); }
                        if (listItemsCol["lookupListName"] == null) { colNames.lookupListName = ""; } else { colNames.lookupListName = listItemsCol["lookupListName"].ToString(); }
                        colNameMetadata.Add(colNames);
                    }
                }
                //Metadata insert Columns: End
                //Metadata Update Columns: Begin
                List<ColumnTypes> colNameMetadataUpdate = new List<ColumnTypes>(10000);
                ListItemCollection getListItemsColUpdate = gl.getListDataVal(ctx, "MetadataListColumnsUpdate");
                if (getListItemsColUpdate != null)
                {
                    foreach (ListItem listItemsCol in getListItemsColUpdate)
                    {
                        ColumnTypes colNames = new ColumnTypes();

                        if (listItemsCol["columnName"] == null) { colNames.columnName = ""; } else { colNames.columnName = listItemsCol["columnName"].ToString(); }
                        if (listItemsCol["columnType"] == null) { colNames.columnType = ""; } else { colNames.columnType = listItemsCol["columnTypes"].ToString(); }
                        if (listItemsCol["isLookup"] == null) { colNames.isLookup = false; } else { colNames.isLookup = Convert.ToBoolean(listItemsCol["isLookup"].ToString()); }
                        if (listItemsCol["lookupListGuid"] == null) { colNames.lookupListGuid = ""; } else { colNames.lookupListGuid = listItemsCol["lookupListGuid"].ToString(); }
                        if (listItemsCol["lookupColumnName"] == null) { colNames.lookupColumnName = ""; } else { colNames.lookupColumnName = listItemsCol["lookupColumnName"].ToString(); }
                        if (listItemsCol["lookupListName"] == null) { colNames.lookupColumnName = ""; } else { colNames.lookupColumnName = listItemsCol["lookupListName"].ToString(); }
                        colNameMetadataUpdate.Add(colNames);
                    }
                }
                DataTable dtSPMetadatListItems = gl.getSPListDataTable(ctx, "JobTasks", gl, colNameMetadataUpdate); //Demo
                //Metadata Update Columns: End

                if (tempTableMetadata != null && tempTableMetadata.Rows.Count > 0)
                {
                    if (gl.createListItemsGeneric(ctx, tempTableMetadata, dtSPMetadatListItems, "JobTasks", colNameMetadata, colNameMetadataUpdate, "JobNum", uValue)) //Demo
                    {
                        //Success
                    }
                }
                #endregion

                #region Delay
                string sqlQueryJobsDelay = "select Reason,Comments,DelayDate,ToDelayDate,DelayId From [WatersunData].[dbo].[vPerJobDelay]";
                //string sqlQueryJobsDelay = "select JobNumber,Reason,Comments,Name,DelayDate,ToDelayDate,Delay,DelAllowedWd,DelayId From [WatersunData].[dbo].[vPerJobDelay]";
                DataTable tempTableJobsDelay = conn.executeSelectNoParameter(sqlQueryJobsDelay);

                //Delay insert Columns: Begin
                List<ColumnTypes> colNameJobsDelay = new List<ColumnTypes>(10000);
                ListItemCollection getJobsDelayListItemsCol = gl.getListDataVal(ctx, "DelayListColumns");
                if (getJobsDelayListItemsCol != null)
                {
                    foreach (ListItem listItemsCol in getJobsDelayListItemsCol)
                    {
                        ColumnTypes colNames = new ColumnTypes();

                        if (listItemsCol["columnName"] == null) { colNames.columnName = ""; } else { colNames.columnName = listItemsCol["columnName"].ToString(); }
                        if (listItemsCol["columnType"] == null) { colNames.columnType = ""; } else { colNames.columnType = listItemsCol["columnTypes"].ToString(); }
                        if (listItemsCol["isLookup"] == null) { colNames.isLookup = false; } else { colNames.isLookup = Convert.ToBoolean(listItemsCol["isLookup"].ToString()); }
                        if (listItemsCol["lookupListGuid"] == null) { colNames.lookupListGuid = ""; } else { colNames.lookupListGuid = listItemsCol["lookupListGuid"].ToString(); }
                        if (listItemsCol["lookupColumnName"] == null) { colNames.lookupColumnName = ""; } else { colNames.lookupColumnName = listItemsCol["lookupColumnName"].ToString(); }
                        if (listItemsCol["lookupListName"] == null) { colNames.lookupListName = ""; } else { colNames.lookupListName = listItemsCol["lookupListName"].ToString(); }
                        colNameJobsDelay.Add(colNames);
                    }
                }
                //Delay insert Columns: End
                //Delay Update Columns: Begin
                List<ColumnTypes> colNameJobsDelayUpdate = new List<ColumnTypes>(10000);
                ListItemCollection getJobsDelayListItemsColUpdate = gl.getListDataVal(ctx, "DelayListColumnsUpdate");
                if (getJobsDelayListItemsColUpdate != null)
                {
                    foreach (ListItem listItemsCol in getJobsDelayListItemsColUpdate)
                    {
                        ColumnTypes colNames = new ColumnTypes();

                        if (listItemsCol["columnName"] == null) { colNames.columnName = ""; } else { colNames.columnName = listItemsCol["columnName"].ToString(); }
                        if (listItemsCol["columnType"] == null) { colNames.columnType = ""; } else { colNames.columnType = listItemsCol["columnTypes"].ToString(); }
                        if (listItemsCol["isLookup"] == null) { colNames.isLookup = false; } else { colNames.isLookup = Convert.ToBoolean(listItemsCol["isLookup"].ToString()); }
                        if (listItemsCol["lookupListGuid"] == null) { colNames.lookupListGuid = ""; } else { colNames.lookupListGuid = listItemsCol["lookupListGuid"].ToString(); }
                        if (listItemsCol["lookupColumnName"] == null) { colNames.lookupColumnName = ""; } else { colNames.lookupColumnName = listItemsCol["lookupColumnName"].ToString(); }
                        if (listItemsCol["lookupListName"] == null) { colNames.lookupListName = ""; } else { colNames.lookupListName = listItemsCol["lookupListName"].ToString(); }
                        colNameJobsDelayUpdate.Add(colNames);
                    }
                }
                //DataTable dtSPDelayListItems = gl.getSPListDataTable(ctx, "Demo", gl, colNameJobsDelayUpdate);
                //Delay Update Columns: End

                if (tempTableMetadata != null && tempTableMetadata.Rows.Count > 0)
                {
                    for (int i = 0; i < tempTableMetadata.Rows.Count; i++)
                    {
                        string listDelayName = tempTableMetadata.Rows[i][0].ToString() + "_Delay";
                        DataTable dtSPDelayListItems = gl.getSPListDataTable(ctx, listDelayName, gl, colNameJobsDelayUpdate);
                        if (gl.createList(ctx, listDelayName, (int)ListTemplateType.TasksWithTimelineAndHierarchy))
                        {
                            if (gl.createListColumnsGeneric(ctx, listDelayName, colNameJobsDelay))
                            {
                                if (gl.createListViewGeneric(ctx, listDelayName, colNameJobsDelay))
                                {
                                    if (gl.removeColFromView(ctx, listDelayName, "TasksCreated"))
                                    { }
                                }
                            }
                        }
                        string queryJobsDelayItems = string.Concat("select Reason,Comments,DelayDate,ToDelayDate,DelayId,JobNumber From [WatersunData].[dbo].[vPerJobDelay]" +
                                                             "WHERE JobNumber = @JobNumber");
                        SqlParameter[] parameter = {                                
                                                                    new SqlParameter("@JobNumber", SqlDbType.VarChar) { Value = tempTableMetadata.Rows[i][0].ToString() }
                                                               };
                        DataTable delayJobsSQLData = conn.executeSelectQuery(queryJobsDelayItems, parameter);
                        if (gl.createListItemsGeneric(ctx, delayJobsSQLData, dtSPDelayListItems, listDelayName, colNameJobsDelay, colNameJobsDelayUpdate, "DelayId", uValue))

                        //if (gl.createListItemsGeneric(ctx, tempTableJobsDelay, dtSPDelayListItems, listDelayName, colNameJobsDelay, colNameJobsDelayUpdate, "DelayId"))
                        {
                            //Success
                        }
                    }
                }
                #endregion

                #region Jobs
                //                string sqlJobsQuery = @"
                //                                                    SELECT  
                //                	                                    [JobNumber]
                //                	                                    ,[JobAddress]
                //                                                    FROM [WatersunData].[dbo].[vJobsRefined]
                //                                                    where left(JobNumber,1) in (3,4,5)
                //                                                    ";
                //                CreateListWFs(sqlJobsQuery);

                //Jobs insert Columns: Begin
                List<ColumnTypes> colNameJobsDetails = new List<ColumnTypes>(10000);
                ListItemCollection getJobsDetailsListItemsCol = gl.getListDataVal(ctx, "JobsListColumns");
                if (getJobsDetailsListItemsCol != null)
                {
                    foreach (ListItem listItemsCol in getJobsDetailsListItemsCol)
                    {
                        ColumnTypes colNames = new ColumnTypes();

                        if (listItemsCol["columnName"] == null) { colNames.columnName = ""; } else { colNames.columnName = listItemsCol["columnName"].ToString(); }
                        if (listItemsCol["columnType"] == null) { colNames.columnType = ""; } else { colNames.columnType = listItemsCol["columnTypes"].ToString(); }
                        if (listItemsCol["isLookup"] == null) { colNames.isLookup = false; } else { colNames.isLookup = Convert.ToBoolean(listItemsCol["isLookup"].ToString()); }
                        if (listItemsCol["lookupListGuid"] == null) { colNames.lookupListGuid = ""; } else { colNames.lookupListGuid = listItemsCol["lookupListGuid"].ToString(); }
                        if (listItemsCol["lookupColumnName"] == null) { colNames.lookupColumnName = ""; } else { colNames.lookupColumnName = listItemsCol["lookupColumnName"].ToString(); }
                        if (listItemsCol["lookupListName"] == null) { colNames.lookupListName = ""; } else { colNames.lookupListName = listItemsCol["lookupListName"].ToString(); }
                        colNameJobsDetails.Add(colNames);
                    }
                }
                //Jobs insert Columns: End
                //Jobs Update Columns: Begin
                List<ColumnTypes> colNameJobsDetailsUpdate = new List<ColumnTypes>(10000);
                ListItemCollection getJobsDetailsListItemsColUpdate = gl.getListDataVal(ctx, "JobsListColumnsUpdate");
                if (getJobsDetailsListItemsColUpdate != null)
                {
                    foreach (ListItem listItemsCol in getJobsDetailsListItemsColUpdate)
                    {
                        ColumnTypes colNames = new ColumnTypes();

                        if (listItemsCol["columnName"] == null) { colNames.columnName = ""; } else { colNames.columnName = listItemsCol["columnName"].ToString(); }
                        if (listItemsCol["columnType"] == null) { colNames.columnType = ""; } else { colNames.columnType = listItemsCol["columnTypes"].ToString(); }
                        if (listItemsCol["isLookup"] == null) { colNames.isLookup = false; } else { colNames.isLookup = Convert.ToBoolean(listItemsCol["isLookup"].ToString()); }
                        if (listItemsCol["lookupListGuid"] == null) { colNames.lookupListGuid = ""; } else { colNames.lookupListGuid = listItemsCol["lookupListGuid"].ToString(); }
                        if (listItemsCol["lookupColumnName"] == null) { colNames.lookupColumnName = ""; } else { colNames.lookupColumnName = listItemsCol["lookupColumnName"].ToString(); }
                        if (listItemsCol["lookupListName"] == null) { colNames.lookupListName = ""; } else { colNames.lookupListName = listItemsCol["lookupListName"].ToString(); }
                        colNameJobsDetailsUpdate.Add(colNames);
                    }
                }
                //DataTable dtSPDelayListItems = gl.getSPListDataTable(ctx, "Demo", gl, colNameJobsDelayUpdate);
                //Jobs Update Columns: End

                //string query = string.Concat("select l_job_id ,s_job_num  ,l_cstL_call_id ,sLogisticsActivity  ,d_called_fBest ,d_calledFor_fBest ,d_start_fBest,d_complete_fBest  ,d_start_fMan ,d_called_fMan ,d_calledFor_fMan ,d_complete_fMan,d_called_fBas ,d_calledFor_fBas ,d_start_fBas,d_complete_fBas,d_called_act,d_calledFor_act, d_start_act , d_complete_act, Supervisor from [WatersunData].[dbo].[vJobsCalledForDates] " +
                //                                     "WHERE s_job_num = @s_job_num");
                //SqlParameter[] parameter = {                                
                //                        new SqlParameter("@s_job_num", SqlDbType.VarChar) { Value = jobNum }
                //                         };

                if (tempTableMetadata != null && tempTableMetadata.Rows.Count > 0)
                {
                    for (int i = 0; i < tempTableMetadata.Rows.Count; i++)
                    {
                        string listDelayName = tempTableMetadata.Rows[i][0].ToString() + "_Data";
                        DataTable dtSPJobsListItems = gl.getSPListDataTable(ctx, listDelayName, gl, colNameJobsDetailsUpdate);
                        if (gl.createList(ctx, listDelayName, (int)ListTemplateType.TasksWithTimelineAndHierarchy))
                        {
                            if (gl.createListColumnsGeneric(ctx, listDelayName, colNameJobsDetails))
                            {
                                if (gl.createListViewGeneric(ctx, listDelayName, colNameJobsDetails))
                                {
                                    //if (gl.removeColFromView(ctx, listDelayName, "TasksCreated"))
                                    //{ }
                                }
                            }
                        }
                        string queryJobsItems = string.Concat("select JobId, JobNum, CstCallId, Activity, called, calledfor, start, complete, Supervisor, Supplier from [WatersunData].[dbo].[vJobsCalledForDates] " +
                                                             "WHERE JobNum = @JobNum");
                        SqlParameter[] parameter = {                                
                                                                    new SqlParameter("@JobNum", SqlDbType.VarChar) { Value = tempTableMetadata.Rows[i][0].ToString() }
                                                               };
                        DataTable jobsSQLData = conn.executeSelectQuery(queryJobsItems, parameter);
                        if (gl.createListItemsGeneric(ctx, jobsSQLData, dtSPJobsListItems, listDelayName, colNameJobsDetails, colNameJobsDetailsUpdate, "CstCallId", uValue))
                        //if (gl.createListItemsGeneric(ctx, tempTableJobsDelay, dtSPDelayListItems, listDelayName, colNameJobsDelay, colNameJobsDelayUpdate, "DelayId"))
                        {
                            //Success
                        }
                    }
                }

                #endregion

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        */
        
        /// <summary>
        /// Updates and Inserts Call Forward Job queue
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="userName"></param>
        /// <param name="passwordString"></param>
        /// <param name="key"></param>
        /// <param name="appSettingsKey"></param>
        /// <param name="columnName"></param>
        /// <param name="tempTable"></param>
        private static void SqlToSpCreateJobsMetadata(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, DataTable tempTable)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                //ListItemCollection getListItemsCol = gl.getListDataVal(ctx, appSettingsKey);                
                #region Get List Item Collection from SP

                ListItemCollection getListItemsCol = null;
                List getList = ctx.Web.Lists.GetByTitle(appSettingsKey);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query></Query></View>";
                ListItemCollection getListItemsCollection = getList.GetItems(camlQuery);
                ctx.Load(getListItemsCollection);
                ctx.ExecuteQuery();

                if (getListItemsCollection != null && getListItemsCollection.Count > 0)
                {
                    getListItemsCol = getListItemsCollection;
                }

                #endregion

                //UserValues: Begin
                List<UserValues> uValue = null;
                List<UserValues> wshSiteUsers = null;

                try
                {
                    UserCollection user = ctx.Web.SiteUsers;                                        
                    ctx.Load(user);
                    ctx.ExecuteQuery();

                    //List<UserValues> uValue = new List<UserValues>(10000);
                    uValue = new List<UserValues>(10000);
                    foreach (User usr in user)
                    {
                        UserValues uv = new UserValues();
                        uv.Email = usr.Email;
                        uv.Id = usr.Id;
                        uv.LoginName = usr.LoginName;
                        uv.Title = usr.Title;
                        uValue.Add(uv);
                    }

                    Web webObj = ctx.Web;
                    List siteUserInfoList = webObj.SiteUserInfoList;
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = "";
                    IEnumerable<ListItem> itemColl = ctx.LoadQuery(siteUserInfoList.GetItems(query));
                    ctx.ExecuteQuery();

                    wshSiteUsers = new List<UserValues>(10000);
                    foreach (var item in itemColl)
                    {
                        Console.WriteLine("ID:{0}  Email:{1} Title:{2}   Name:{3}", item.Id, item["EMail"], item["Title"], item["Name"]);
                        UserValues uv = new UserValues();
                        //uv.Email =         if(item["EMail"] != null){ item["EMail"].ToString()};
                        //uv.Id =            Int32.Parse(item["Id"].ToString());
                        //uv.LoginName =     item["LoginName"].ToString();
                        //uv.Title =         item["Title"].ToString();

                        if (item["EMail"] != null) { uv.Email = item["EMail"].ToString(); }     else { uv.Email = ""; }
                        //if (item["EMail"] != null) { uv.Id =        Int32.Parse(item["Id"].ToString()); }
                        uv.Id = item.Id;
                        if (item["Name"] != null) { uv.LoginName = item["Name"].ToString(); }   else { uv.LoginName = ""; }
                        if (item["Title"] != null) { uv.Title = item["Title"].ToString(); }     else { uv.Title = ""; }

                        wshSiteUsers.Add(uv);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //UserValues: End

                DataTable dt = new DataTable();

                dt.Columns.Add("ID");
                //dt.Columns.Add("JobNum");
                dt.Columns.Add("Title");
                dt.Columns.Add("JobAddr");
                dt.Columns.Add("Client");
                dt.Columns.Add("Delay");
                dt.Columns.Add("Week");
                dt.Columns.Add("Overall");
                dt.Columns.Add("JobDelayLink");
                dt.Columns.Add("JobLink");
                dt.Columns.Add("Supervisor");
                dt.Columns.Add("SupervisorLookupId");
                dt.Columns.Add("SupervisorLookupValue");
                dt.Columns.Add("ConstructionManager");
                dt.Columns.Add("ConstructionManagerLookupId");
                dt.Columns.Add("ConstructionManagerLookupValue");
                dt.Columns.Add("JobsDelay");
                dt.Columns.Add("JobsDetail");
                if (getListItemsCol != null)
                {
                    foreach (ListItem listItemsCol in getListItemsCol)
                    {
                        DataRow dr = dt.NewRow();
                        //dr["JobNum"] =                  listItemsCol["JobNum"];
                        dr["Title"] = listItemsCol["Title"];
                        dr["ID"] = listItemsCol["ID"];
                        dr["JobAddr"] = listItemsCol["JobAddr"];
                        dr["Client"] = listItemsCol["Client"];
                        dr["Delay"] = listItemsCol["Delay"];
                        dr["Week"] = listItemsCol["Week"];
                        dr["Overall"] = listItemsCol["Overall"];
                        dr["JobDelayLink"] = listItemsCol["JobDelayLink"];
                        dr["JobLink"] = listItemsCol["JobLink"];
                        dr["Supervisor"] = listItemsCol["Supervisor"];
                        
                        //var supervisorLkp = listItemsCol["Supervisor"] as FieldLookupValue;
                        if (listItemsCol["Supervisor"] == null)
                        {
                            dr["SupervisorLookupId"] = "";
                            dr["SupervisorLookupValue"] = "";
                        }
                        else
                        {
                            dr["SupervisorLookupId"] = ((FieldUserValue)listItemsCol["Supervisor"]).LookupId;
                            dr["SupervisorLookupValue"] = ((FieldUserValue)listItemsCol["Supervisor"]).LookupValue; 
                        }
                        if (listItemsCol["ConstructionManager"] == null)
                        {
                            dr["ConstructionManagerLookupId"] = "";
                            dr["ConstructionManagerLookupValue"] = "";
                        }
                        else
                        {
                            dr["ConstructionManagerLookupId"] = ((FieldUserValue)listItemsCol["ConstructionManager"]).LookupId;
                            dr["ConstructionManagerLookupValue"] = ((FieldUserValue)listItemsCol["ConstructionManager"]).LookupValue; 
                        }

                        dr["ConstructionManager"] = listItemsCol["ConstructionManager"];
                        dr["JobsDelay"] = listItemsCol["JobsDelay"];
                        dr["JobsDetail"] = listItemsCol["JobsDetail"];

                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    //dr["JobNum"] = "";
                    dr["Title"] = "";
                    dr["ID"] = "";
                    dr["JobAddr"] = "";
                    dr["Client"] = "";
                    dr["Delay"] = "";
                    dr["Week"] = "";
                    dr["Overall"] = "";
                    dr["JobDelayLink"] = "";
                    dr["JobLink"] = "";
                    dr["Supervisor"] = "";
                    dr["ConstructionManager"] = "";
                    dr["JobsDelay"] = "";
                    dr["JobsDetail"] = "";
                    dr["SupervisorLookupId"] = "";
                    dr["SupervisorLookupValue"] = "";
                    dr["ConstructionManagerLookupId"] = "";
                    dr["ConstructionManagerLookupValue"] = "";

                    dt.Rows.Add(dr);
                }

                //dbConnection conn = new dbConnection();
                //DataTable tempTable = null;
                //tempTable = conn.executeSelectNoParameter(sqlQuery);
                ListItemCreationInformation itemCreateInfo = null;
                ListItem oListItem = null;
                List oList = ctx.Web.Lists.GetByTitle(appSettingsKey);


                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    try
                    {
                        //DataRow[] drExists = dt.Select("JobNum = '" + tempTable.Rows[i]["JobNum"].ToString() + "'");
                        DataRow[] drExists = dt.Select("Title = '" + tempTable.Rows[i]["JobNum"].ToString() + "'");
                        if (drExists != null && drExists.Length > 0)
                        {
                            //var rowExists = dt.AsEnumerable().Where(x => x.Field<string>("Title") == tempTable.Rows[i]["JobNum"].ToString()
                            //                                                && x.Field<string>("Delay") == tempTable.Rows[i]["Delay"].ToString()
                            //                                                && x.Field<string>("Overall") == tempTable.Rows[i]["Overall"].ToString()
                            //                                                && x.Field<string>("Week") == tempTable.Rows[i]["Week"].ToString()
                            //                                                && x.Field<string>("SupervisorLookupValue") == tempTable.Rows[i]["Supervisor"].ToString()
                            //                                                && x.Field<string>("ConstructionManagerLookupValue") == tempTable.Rows[i]["ConstructionManager"].ToString()
                            //                                             );
                            var rowExists = drExists.AsEnumerable().Where(x => x.Field<string>("Title") == tempTable.Rows[i]["JobNum"].ToString()
                                                                            && x.Field<string>("Delay") == tempTable.Rows[i]["Delay"].ToString()
                                                                            && x.Field<string>("Overall") == tempTable.Rows[i]["Overall"].ToString()
                                                                            && x.Field<string>("Week") == tempTable.Rows[i]["Week"].ToString()
                                                                            && x.Field<string>("SupervisorLookupValue") == tempTable.Rows[i]["Supervisor"].ToString()
                                                                            && x.Field<string>("ConstructionManagerLookupValue") == tempTable.Rows[i]["ConstructionManager"].ToString()
                                                                         );

                            //tempTable.Rows[i]["Supervisor"].ToString()

                            DataTable drExists1 = null;
                            if (rowExists.Any())
                            { drExists1 = rowExists.CopyToDataTable(); }

                            //if (drExists1 != null && drExists1.Length > 0)
                            if (drExists1 != null && drExists1.Rows.Count > 0)
                            {
                                Console.WriteLine("Found - " + tempTable.Rows[i][0].ToString());
                                //xlWorkSheet.Cells[i + 1, 1] = "Found Job Number - ";
                                //xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            }
                            else
                            {
                                Console.WriteLine("Update - " + tempTable.Rows[i][0].ToString() + " ------ " +
                                                    "Supervisor :- " + dt.Rows[i]["SupervisorLookupValue"] + " ------ "
                                                    + "ConstructionManager :- " + dt.Rows[i]["ConstructionManagerLookupValue"]);
                                
                                //if ((drExists[0]["SupervisorLookupValue"] != null) && (drExists[0]["ConstructionManagerLookupValue"] != null))
                                FieldUserValue ssUserValue = new FieldUserValue();
                                FieldUserValue cmUserValue = new FieldUserValue();

                                foreach (UserValues uVal in wshSiteUsers)
                                {
                                    if (uVal.Title.ToString() == tempTable.Rows[i]["Supervisor"].ToString())
                                    {
                                        //FieldUserValue userValue = new FieldUserValue();
                                        ssUserValue.LookupId = uVal.Id;
                                        break;
                                    }
                                }
                                foreach (UserValues uVal in wshSiteUsers)
                                //foreach (UserValues uVal in uValue)
                                {
                                    if (uVal.Title.ToString() == tempTable.Rows[i]["ConstructionManager"].ToString())
                                    {
                                        //FieldUserValue userValue = new FieldUserValue();
                                        cmUserValue.LookupId = uVal.Id;
                                        break;
                                    }
                                }

                                //if (1==1)
                                if (ssUserValue.LookupId != 0 && cmUserValue.LookupId != 0)
                                {
                                    oListItem = oList.GetItemById(drExists[0]["ID"].ToString());

                                    oListItem["Delay"] = tempTable.Rows[i]["Delay"].ToString();
                                    oListItem["Overall"] = tempTable.Rows[i]["Overall"].ToString();
                                    oListItem["Week"] = tempTable.Rows[i]["Week"].ToString();

                                    //vaibhav  //foreach (UserValues uVal in uValue)

                                    oListItem["Supervisor"] = ssUserValue;
                                    oListItem["ConstructionManager"] = cmUserValue;

                                    oListItem.Update();
                                    ctx.ExecuteQuery();

                                    xlWorkSheet.Cells[i + 1, 1] = "Updating - ";
                                    xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();

                                }
                                gl.releaseObject(ssUserValue);
                                gl.releaseObject(cmUserValue);
                                //xlWorkSheet.Cells[i + 1, 1] = "Updating - ";
                                //xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Inserting - " + tempTable.Rows[i][0].ToString());
                            itemCreateInfo = new ListItemCreationInformation();
                            oListItem = oList.AddItem(itemCreateInfo);

                            //oListItem["JobNum"] = tempTable.Rows[i]["JobNum"].ToString();
                            oListItem["Title"] = tempTable.Rows[i]["JobNum"].ToString();
                            oListItem["JobAddr"] = tempTable.Rows[i]["JobAddr"].ToString();
                            oListItem["Client"] = tempTable.Rows[i]["Client"].ToString();
                            oListItem["Delay"] = tempTable.Rows[i]["Delay"].ToString();
                            oListItem["Week"] = tempTable.Rows[i]["Week"].ToString();
                            oListItem["Overall"] = tempTable.Rows[i]["Overall"].ToString();

                            FieldUrlValue url1 = new FieldUrlValue();
                            url1.Url = tempTable.Rows[i]["JobDelayLink"].ToString();
                            url1.Description = tempTable.Rows[i]["JobDelayLink"].ToString();
                            oListItem["JobDelayLink"] = url1;

                            FieldUrlValue url2 = new FieldUrlValue();
                            url2.Url = tempTable.Rows[i]["JobLink"].ToString();
                            url2.Description = tempTable.Rows[i]["JobLink"].ToString();
                            oListItem["JobLink"] = url2;

                            foreach (UserValues uVal in uValue)
                            {
                                if (uVal.Title.ToString() == tempTable.Rows[i]["Supervisor"].ToString())
                                {
                                    FieldUserValue userValue = new FieldUserValue();
                                    userValue.LookupId = uVal.Id;
                                    oListItem["Supervisor"] = userValue;
                                    break;
                                }
                            }
                            foreach (UserValues uVal in uValue)
                            {
                                if (uVal.Title.ToString() == tempTable.Rows[i]["ConstructionManager"].ToString())
                                {
                                    FieldUserValue userValue = new FieldUserValue();
                                    userValue.LookupId = uVal.Id;
                                    oListItem["ConstructionManager"] = userValue;
                                    break;
                                }
                            }	 
                            oListItem["JobsDelay"] = tempTable.Rows[i]["JobsDelay"].ToString();
                            oListItem["JobsDetail"] = tempTable.Rows[i]["JobsDetail"].ToString();

                            oListItem.Update();
                            ctx.ExecuteQuery();

                            xlWorkSheet.Cells[i + 1, 1] = "Inserting - ";
                            xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Metadata" + "---" + "Insertng new Call Forwards");
                    }
                }
                //_ddMMyyyy_HHmmss

                //xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_Metadata" + DateTime.Now.ToString("dd_MM_yyyy hh.mm.ss.fff tt") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.SaveAs(ConfigurationManager.AppSettings.Get("LogLocation") + "\\SqlToSpLog_Metadata" + DateTime.Now.ToString("dd_MM_yyyy hh.mm.ss.fff tt") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                gl.releaseObject(xlWorkSheet);
                gl.releaseObject(xlWorkBook);
                gl.releaseObject(xlApp);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //GlobalLogic gl = new GlobalLogic();
                GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Metadata" + "---" + "Starting operation on Call Forward List");
            }
            finally
            {
                //dt.Dispose();
            }
        }

        /// <summary>
        /// Creates Delay List for each job in SP (SharePoint) and insert delay records from Framework DB to SP List
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="userName"></param>
        /// <param name="passwordString"></param>
        /// <param name="key"></param>
        /// <param name="appSettingsKey"></param>
        /// <param name="columnName"></param>
        /// <param name="tempTable"></param>
        /// <param name="queryJobsDelayItems"></param>
        private static void SqlToSpCreateJobsDelayData(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, DataTable tempTable, string queryJobsDelayItems)
        {
            #region 1
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                #region UserValues
                //UserValues: Begin
                List<UserValues> uValue = null;
                try
                {
                    UserCollection user = ctx.Web.SiteUsers;
                    ctx.Load(user);
                    ctx.ExecuteQuery();
                    uValue = new List<UserValues>(10000);
                    foreach (User usr in user)
                    {
                        UserValues uv = new UserValues();
                        uv.Email = usr.Email;
                        uv.Id = usr.Id;
                        uv.LoginName = usr.LoginName;
                        uv.Title = usr.Title;
                        uValue.Add(uv);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //UserValues: End
                #endregion

                if (tempTable != null && tempTable.Rows.Count > 0)
                {
                    for (int j = 0; j < tempTable.Rows.Count; j++)
                    {
                        bool listCreated = false;
                        try
                        {                            
                            string listDelayName = tempTable.Rows[j][0].ToString() + "_Delay";

                            listCreated = false;
                            listCreated = gl.createList(ctx, listDelayName, (int)ListTemplateType.TasksWithTimelineAndHierarchy);

                            //if (gl.createList(ctx, listDelayName, (int)ListTemplateType.TasksWithTimelineAndHierarchy))
                            if (listCreated)
                            {
                                Web web = ctx.Web;
                                List list = web.Lists.GetByTitle(listDelayName);
                                List listLkp = web.Lists.GetByTitle("DelayReason");
                                ctx.Load(listLkp);
                                ctx.ExecuteQuery();

                                #region Create Columns
                                try
                                {                                    
                                    Field field1 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "DelayId" + "' Type='Text' />", true, AddFieldOptions.DefaultValue);
                                    FieldText fld1 = ctx.CastTo<FieldText>(field1);
                                    fld1.Update();

                                    //Field field2 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "JobNumber" + "' Type='Text' />", true, AddFieldOptions.DefaultValue);
                                    Field field2 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "JobNumber" + "' Type='Text' ><Default>" + tempTable.Rows[j]["JobNum"].ToString() + "</Default></Field>", true, AddFieldOptions.DefaultValue);
                                    FieldText fld2 = ctx.CastTo<FieldText>(field2);
                                    fld2.Update();

                                    //var lookupFieldXml3 = "<Field DisplayName='" + "Reason" + "' Type=\"Lookup\" />";
                                    Guid lkpFldGuid3 = Guid.NewGuid();
                                    var lookupFieldXml3 = "<Field ID='{" + lkpFldGuid3 + "}' DisplayName='" + "Reason" + "' Type=\"Lookup\" />";
                                    var field3 = list.Fields.AddFieldAsXml(lookupFieldXml3, false, AddFieldOptions.AddToAllContentTypes);
                                    var fld3 = ctx.CastTo<FieldLookup>(field3);
                                    fld3.LookupList = listLkp.Id.ToString(); //DelayReason List :- "F7E19A58-6BF4-4EFC-B57B-ACDDABCB9634";
                                    fld3.LookupField = "DelayReason"; //"SupplierName";
                                    fld3.Update();

                                    Guid lkpFldGuid3Ref = Guid.NewGuid();
                                    var lookupFieldXml3Ref = string.Concat("<Field Type='Lookup' DisplayName='ReasonId' List='" + listLkp.Id.ToString() //"{F7E19A58-6BF4-4EFC-B57B-ACDDABCB9634}"
                                        //+ "' ShowField='ID' FieldRef='" + lkpFldGuid3 + "'" + " ID='" + lkpFldGuid3Ref + "' />");
                                                            + "' ShowField='RsnId' FieldRef='" + lkpFldGuid3 + "'" + " ID='" + lkpFldGuid3Ref + "' />");
                                    var field3Ref = list.Fields.AddFieldAsXml(lookupFieldXml3Ref, false, AddFieldOptions.AddToAllContentTypes);



                                    Field field4 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "DelayDate" + "' Type='DateTime' Format='DateOnly' ><Default>[Today]</Default></Field>", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld4 = ctx.CastTo<FieldDateTime>(field4);
                                    fld4.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld4.Update();

                                    Field field5 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "ToDelayDate" + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld5 = ctx.CastTo<FieldDateTime>(field5);
                                    fld5.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld5.Update();

                                    Field field6 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "AreYouSure" + "' Type='Boolean' ><Default>0</Default></Field>", true, AddFieldOptions.DefaultValue);

                                    //Field field7 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "JobId" + "' Type='Text' />", true, AddFieldOptions.DefaultValue);
                                    Field field7 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "JobId" + "' Type='Text' ><Default>" + tempTable.Rows[j]["JobId"].ToString() + "</Default></Field>", true, AddFieldOptions.DefaultValue);

                                    FieldText fld7 = ctx.CastTo<FieldText>(field7);
                                    fld7.Update();

                                    Field field8 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "CstId" + "' Type='Text' />", true, AddFieldOptions.DefaultValue);
                                    FieldText fld8 = ctx.CastTo<FieldText>(field8);
                                    fld8.Update();

                                    ctx.ExecuteQuery();                                    
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Delay Data" + "---" + "Create Delay List Columns");
                                }
                                #endregion
                                
                                #region Rename Title Column
                                try
                                {
                                    FieldCollection collField = list.Fields;
                                    Field oneField = collField.GetByInternalNameOrTitle("Title");
                                    oneField.Title = "Comments"; //new column name replace of Title
                                    oneField.Required = false;
                                    oneField.DefaultValue = "test";
                                    oneField.Update();

                                    Field oneField2 = collField.GetByInternalNameOrTitle("AreYouSure");
                                    //oneField.Title = "Comments"; //new column name replace of Title
                                    oneField2.Required = true;
                                    oneField2.DefaultValue = "";
                                    oneField2.Update();

                                    ctx.Load(collField);
                                    ctx.Load(oneField);
                                    ctx.Load(oneField2);
                                    ctx.ExecuteQuery();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Delay Data" + "---" + "Rename Title Column");
                                }
                                #endregion

                                #region Create View
                                try
                                {
                                    ViewCollection viewColl = list.Views;

                                    string[] viewFields = { "Checkmark", "PercentComplete", "Reason", "Title", "DelayDate", "ToDelayDate", "AreYouSure" };
                                    ViewCreationInformation creationInfo = new ViewCreationInformation();
                                    creationInfo.Title = "TasksCreated";
                                    creationInfo.RowLimit = 50;
                                    creationInfo.ViewFields = viewFields;
                                    creationInfo.ViewTypeKind = Microsoft.SharePoint.Client.ViewType.None;
                                    creationInfo.SetAsDefaultView = true;
                                    creationInfo.Paged = true;
                                    viewColl.Add(creationInfo);

                                    ctx.ExecuteQuery();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Delay Data" + "---" + "Create List View");
                                }
                                #endregion

                            }
                            #region Create and Update Task list Items
                            try
                            {
                                //ListItemCollection getListItemsCol = gl.getListDataVal(ctx, listDelayName);
                                #region Get List Item Collection from SP

                                ListItemCollection getListItemsCol = null;
                                List getList = ctx.Web.Lists.GetByTitle(listDelayName);
                                CamlQuery camlQuery = new CamlQuery();
                                camlQuery.ViewXml = "<View><Query></Query></View>";
                                ListItemCollection getListItemsCollection = getList.GetItems(camlQuery);
                                ctx.Load(getListItemsCollection);
                                ctx.ExecuteQuery();

                                if (getListItemsCollection != null && getListItemsCollection.Count > 0)
                                {
                                    getListItemsCol = getListItemsCollection;
                                }

                                #endregion

                                DataTable dt = new DataTable();

                                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                                //DateTime cstTime;

                                dt.Columns.Add("ID");
                                dt.Columns.Add("DelayId");
                                dt.Columns.Add("JobNumber");
                                dt.Columns.Add("Reason");
                                dt.Columns.Add("ReasonId");
                                dt.Columns.Add("ReasonValue");
                                dt.Columns.Add("Title");
                                dt.Columns.Add("DelayDate");
                                dt.Columns.Add("ToDelayDate");
                                dt.Columns.Add("AreYouSure");
                                dt.Columns.Add("JobId");
                                dt.Columns.Add("CstId");

                                if (getListItemsCol != null)
                                {
                                    foreach (ListItem listItemsCol in getListItemsCol)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["ID"] = listItemsCol["ID"];
                                        dr["DelayId"] = listItemsCol["DelayId"];
                                        dr["JobNumber"] = listItemsCol["JobNumber"];
                                        dr["Reason"] = listItemsCol["Reason"];

                                        var reasonLkp = listItemsCol["Reason"] as FieldLookupValue;
                                        if (reasonLkp != null)
                                        {
                                            dr["ReasonId"] = reasonLkp.LookupId;
                                            dr["ReasonValue"] = reasonLkp.LookupValue;
                                        }

                                        if (listItemsCol["Title"] == null) { dr["Title"] = "No Title"; }
                                        else { dr["Title"] = listItemsCol["Title"]; }

                                        if (listItemsCol["DelayDate"] == null) { dr["DelayDate"] = DateTime.Now; }
                                        else { dr["DelayDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["DelayDate"].ToString()), cstZone); }

                                        if (listItemsCol["ToDelayDate"] == null) { dr["ToDelayDate"] = DateTime.Now; }
                                        else
                                        { //dr["ToDelayDate"] = Convert.ToDateTime(listItemsCol["ToDelayDate"].ToString()).AddHours(10); 
                                            dr["ToDelayDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["ToDelayDate"].ToString()), cstZone);
                                        }

                                        dr["AreYouSure"] = listItemsCol["AreYouSure"];
                                        dr["CstId"] = listItemsCol["CstId"];
                                        dr["JobId"] = listItemsCol["JobId"];

                                        dt.Rows.Add(dr);
                                    }
                                }
                                else
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["ID"] = "";
                                    dr["DelayId"] = "";
                                    dr["JobNumber"] = "";
                                    dr["Reason"] = "";
                                    dr["ReasonId"] = "";
                                    dr["ReasonValue"] = "";
                                    dr["Title"] = "";
                                    dr["DelayDate"] = "";
                                    dr["ToDelayDate"] = "";
                                    dr["AreYouSure"] = "";
                                    dr["JobId"] = "";
                                    dr["CstId"] = "";

                                    dt.Rows.Add(dr);
                                }

                                //dbConnection conn = new dbConnection();
                                //DataTable tempTable = null;
                                //tempTable = conn.executeSelectNoParameter(sqlQuery);
                                ListItemCreationInformation itemCreateInfo = null;
                                ListItem oListItem = null;
                                List oList = ctx.Web.Lists.GetByTitle(listDelayName);

                                Excel.Application xlApp;
                                Excel.Workbook xlWorkBook;
                                Excel.Worksheet xlWorkSheet;
                                object misValue = System.Reflection.Missing.Value;

                                xlApp = new Excel.Application();
                                xlWorkBook = xlApp.Workbooks.Add(misValue);
                                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                                dbConnection conn = new dbConnection();
                                ////Counter
                                string queryId = string.Concat("SELECT [ID],[jobNumber],[jobId] ,[cstId] ,[SpId],[delayId] ,[dCreated] FROM [WatersunData].[dbo].[CounterDelay]" +
                                                                                 "WHERE jobNumber = @JobNum");
                                SqlParameter[] parameterDelayId = {                                
                                                                    new SqlParameter("@JobNum", SqlDbType.VarChar) { Value = tempTable.Rows[j]["JobNum"].ToString() }
                                                               };
                                DataTable delayIdCounterTbl = conn.executeSelectQuery(queryId, parameterDelayId);
                                ////

                                SqlParameter[] parameter = {                                
                                                                    new SqlParameter("@JobNumber", SqlDbType.VarChar) { Value = tempTable.Rows[j][0].ToString() }
                                                               };
                                DataTable delayJobsSQLData = conn.executeSelectQuery(queryJobsDelayItems, parameter);

                                if (1==1) // if (getListItemsCol != null && getListItemsCol.Count > 0)
                                {
                                    for (int i = 0; i < delayJobsSQLData.Rows.Count; i++)
                                    {
                                        try
                                        {
                                            DataRow[] drExists = dt.Select("DelayId = '" + delayJobsSQLData.Rows[i]["DelayId"].ToString() + "'");
                                            if (drExists != null && drExists.Length > 0)
                                            {
                                                //DataRow[] drExists1 = dt.Select("JobNum = '" + tempTable.Rows[i][0].ToString() + "'" + " AND delay = '" + tempTable.Rows[i][1].ToString() + "'");
                                                //DataRow[] drExists1 = dt.Select("DelayId = '" + delayJobsSQLData.Rows[i]["DelayId"].ToString() + "'" + " AND Title = '" + delayJobsSQLData.Rows[i]["Title"].ToString() + "'");

                                                string comments = "";
                                                if (delayJobsSQLData.Rows[i]["Comments"].ToString() == "") { comments = "No Title"; }
                                                else { comments = delayJobsSQLData.Rows[i]["Comments"].ToString(); }

                                                Console.WriteLine("-------------------------------------------------------------------------------------0");



                                                var rowExists = dt.AsEnumerable().Where(x => x.Field<string>("DelayId") == delayJobsSQLData.Rows[i]["DelayId"].ToString()
                                                    //&& x.Field<string>("Title") == comments
                                                                                                && x.Field<string>("ReasonValue") == delayJobsSQLData.Rows[i]["Reason"].ToString()
                                                                                                && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("DelayDate"))).ToString("dd/MM/yyyy")) ==
                                                                                                        Convert.ToDateTime((Convert.ToDateTime(delayJobsSQLData.Rows[i]["DelayDate"].ToString())).ToString("dd/MM/yyyy"))
                                                                                                && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("ToDelayDate"))).ToString("dd/MM/yyyy")) ==
                                                    //Convert.ToDateTime((Convert.ToDateTime(delayJobsSQLData.Rows[i]["ToDelayDate"].ToString()).AddHours(-10)).ToString("dd/MM/yyyy"))
                                                                                                        Convert.ToDateTime((Convert.ToDateTime(delayJobsSQLData.Rows[i]["ToDelayDate"].ToString())).ToString("dd/MM/yyyy"))
                                                                                             );

                                                ////////////////////Console.WriteLine( "DelayId :-"  +  dt.Rows[i]["DelayId"]);
                                                ////////////////////Console.WriteLine("Title :-" + dt.Rows[i]["Title"]);
                                                ////////////////////Console.WriteLine("ReasonValue :-" + dt.Rows[i]["ReasonValue"]);
                                                ////////////////////Console.WriteLine("DelayDate :-" + dt.Rows[i]["DelayDate"]);
                                                ////////////////////Console.WriteLine("ToDelayDate :-" + dt.Rows[i]["ToDelayDate"]);

                                                ////////////////////Console.WriteLine("DelayId :-" + delayJobsSQLData.Rows[i]["DelayId"]);
                                                ////////////////////Console.WriteLine("Title :-" + comments);
                                                ////////////////////Console.WriteLine("ReasonValue :-" + delayJobsSQLData.Rows[i]["Reason"]);
                                                ////////////////////Console.WriteLine("DelayDate :-" + delayJobsSQLData.Rows[i]["DelayDate"]);
                                                ////////////////////Console.WriteLine("ToDelayDate :-" + delayJobsSQLData.Rows[i]["ToDelayDate"]);
                                                Console.WriteLine("-------------------------------------------------------------------------------------1");

                                                DataTable drExists1 = null;
                                                Console.WriteLine("-------------------------------------------------------------------------------------2");
                                                if (rowExists.Any())
                                                {
                                                    drExists1 = rowExists.CopyToDataTable();
                                                    Console.WriteLine("-------------------------------------------------------------------------------------3");
                                                }

                                                if (drExists1 != null && drExists1.Rows.Count > 0)
                                                //if (drExists1 != null && drExists1.Length > 0)
                                                {
                                                    Console.WriteLine("-------------------------------------------------------------------------------------4");
                                                    Console.WriteLine("Found Delay List :- " + listDelayName + " and Delay :- " + delayJobsSQLData.Rows[i]["DelayId"].ToString());
                                                    Console.WriteLine("-------------------------------------------------------------------------------------5");
                                                    //////////////////////xlWorkSheet.Cells[i + 1, 1] = "Found Job Number - ";
                                                    //////////////////////xlWorkSheet.Cells[i + 1, 2] = delayJobsSQLData.Rows[i]["JobNumber"].ToString();
                                                    //////////////////////xlWorkSheet.Cells[i + 1, 3] = "Found Delay :- ";
                                                    //////////////////////xlWorkSheet.Cells[i + 1, 4] = delayJobsSQLData.Rows[i]["DelayId"].ToString();
                                                }
                                                else
                                                {
                                                    #region Original Code
                                                    //Console.WriteLine("Updating - " + delayJobsSQLData.Rows[i]["DelayId"].ToString());
                                                    ////////oListItem = oList.GetItemById(drExists[0].ItemArray[0].ToString());                                                    
                                                    //oListItem = oList.GetItemById(drExists[0]["ID"].ToString());

                                                    //oListItem["Title"] = delayJobsSQLData.Rows[i]["Comments"].ToString();
                                                    //oListItem["DelayDate"] = Convert.ToDateTime(delayJobsSQLData.Rows[i]["DelayDate"].ToString());
                                                    //oListItem["ToDelayDate"] = Convert.ToDateTime(delayJobsSQLData.Rows[i]["ToDelayDate"].ToString());
                                                    //oListItem["Reason"] = gl.GetLookupFieldValue(ctx, delayJobsSQLData.Rows[i]["Reason"].ToString(), "DelayReason", "DelayReason", "Text");

                                                    //oListItem.Update();
                                                    //ctx.ExecuteQuery();

                                                    //xlWorkSheet.Cells[i + 1, 1] = "Updating - ";
                                                    //xlWorkSheet.Cells[i + 1, 2] = delayJobsSQLData.Rows[i][0].ToString();
                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Inserting - " + delayJobsSQLData.Rows[i]["DelayId"].ToString());
                                                itemCreateInfo = new ListItemCreationInformation();

                                                //ListItem oListItem1 = oList.AddItem(itemCreateInfo);
                                                //oListItem1["Reason"] = gl.GetLookupFieldValue(ctx, delayJobsSQLData.Rows[i]["Reason"].ToString(), "DelayReason", "DelayReason", "Text");
                                                //oListItem1.Update();

                                                oListItem = oList.AddItem(itemCreateInfo);

                                                oListItem["Reason"] = gl.GetLookupFieldValue(ctx, delayJobsSQLData.Rows[i]["Reason"].ToString(), "DelayReason", "DelayReason", "Text");
                                                oListItem.Update();

                                                //oListItem["Reason"] = gl.GetLookupFieldValue(ctx, delayJobsSQLData.Rows[i]["Reason"].ToString(), "DelayReason", "DelayReason", "Text");
                                                //oListItem.Update();
                                                oListItem["DelayId"] = delayJobsSQLData.Rows[i]["DelayId"].ToString();
                                                oListItem["JobNumber"] = delayJobsSQLData.Rows[i]["JobNumber"].ToString();
                                                oListItem["Title"] = delayJobsSQLData.Rows[i]["Comments"].ToString();

                                                if (delayJobsSQLData.Rows[i]["DelayDate"].ToString() == "") { oListItem["DelayDate"] = null; }
                                                else { oListItem["DelayDate"] = Convert.ToDateTime(delayJobsSQLData.Rows[i]["DelayDate"].ToString()); }

                                                if (delayJobsSQLData.Rows[i]["ToDelayDate"].ToString() == "") { oListItem["ToDelayDate"] = null; }
                                                else { oListItem["ToDelayDate"] = Convert.ToDateTime(delayJobsSQLData.Rows[i]["ToDelayDate"].ToString()); }

                                                //oListItem["AreYouSure"] = delayJobsSQLData.Rows[i]["AreYouSure"].ToString();
                                                oListItem["CstId"] = delayJobsSQLData.Rows[i]["CstId"].ToString();
                                                oListItem["JobId"] = delayJobsSQLData.Rows[i]["JobId"].ToString();

                                                oListItem.Update();

                                                ctx.ExecuteQuery();

                                                //xlWorkSheet.Cells[i + 1, 1] = "Inserting - ";
                                                //xlWorkSheet.Cells[i + 1, 2] = delayJobsSQLData.Rows[i][0].ToString();

                                                xlWorkSheet.Cells[i + 1, 1] = "Inserting Job Number - ";
                                                xlWorkSheet.Cells[i + 1, 2] = delayJobsSQLData.Rows[i]["JobNumber"].ToString();
                                                xlWorkSheet.Cells[i + 1, 1] = "Inserting DelayId - ";
                                                xlWorkSheet.Cells[i + 1, 2] = delayJobsSQLData.Rows[i]["DelayId"].ToString();
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                            GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Delay Data" + "---" + listDelayName);
                                        }
                                    }
                                }
                                //xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_Delay_" +listDelayName+ DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.SaveAs(ConfigurationManager.AppSettings.Get("LogLocation") + "\\SqlToSpLog_Delay_" + listDelayName + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.Close(true, misValue, misValue);
                                xlApp.Quit();

                                gl.releaseObject(xlWorkSheet);
                                gl.releaseObject(xlWorkBook);
                                gl.releaseObject(xlApp);
                                //}
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Delay Data" + "---" + "Create and Update Task list Items" + "---" + listDelayName);
                            }
                            #endregion

                            #region Associate WF

                            //if (listCreated)
                            //{
                            //    Guid listId = gl.getListGuid(ctx, listDelayName);                                
                            //    gl.addWorkflowSubscription(ctx, listDelayName, listId);
                            //}

                            #endregion
                        }
                        catch (Exception e) 
                        {
                            Console.WriteLine(e.Message);
                            GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Delay Data" + "---" + "Starting operation on Delay List");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //GlobalLogic gl = new GlobalLogic();
                GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Delay Data" + "---" + "Region 1");
            }
            finally
            {
                //dt.Dispose();
            }
            #endregion
        }

        /// <summary>
        /// Creates Call Forward Job List for each job in SP (SharePoint) and insert build program records from Framework DB to SP List
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="userName"></param>
        /// <param name="passwordString"></param>
        /// <param name="key"></param>
        /// <param name="appSettingsKey"></param>
        /// <param name="columnName"></param>
        /// <param name="tempTable"></param>
        /// <param name="queryJobsDelayItems"></param>
        private static void SqlToSpCreateJobsData(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, DataTable tempTable, string queryJobsDelayItems)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                #region UserValues
                //UserValues: Begin
                List<UserValues> uValue = null;
                try
                {
                    UserCollection user = ctx.Web.SiteUsers;
                    ctx.Load(user);
                    ctx.ExecuteQuery();
                    uValue = new List<UserValues>(10000);
                    foreach (User usr in user)
                    {
                        UserValues uv = new UserValues();
                        uv.Email = usr.Email;
                        uv.Id = usr.Id;
                        uv.LoginName = usr.LoginName;
                        uv.Title = usr.Title;
                        uValue.Add(uv);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //UserValues: End
                #endregion

                if (tempTable != null && tempTable.Rows.Count > 0)
                {
                    for (int j = 0; j < tempTable.Rows.Count; j++)
                    {
                        #region Create Lists
                        try
                        {
                            string listDelayName = tempTable.Rows[j][0].ToString() + "_Data";
                            if (gl.createList(ctx, listDelayName, (int)ListTemplateType.TasksWithTimelineAndHierarchy))
                            {
                                    Web web = ctx.Web;
                                    List list = web.Lists.GetByTitle(listDelayName);
                                    List listLkp = web.Lists.GetByTitle("SuppliersList");
                                    ctx.Load(listLkp);
                                    ctx.ExecuteQuery();

                                #region Create Columns
                                try
                                {                                    
                                    //JobId, JobNum, CstCallId, Activity, called, calledfor, start, complete, Supervisor, Supplier, Duration
                                    Field field1 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "CstCallId" + "' Type='Text' />", true, AddFieldOptions.DefaultValue);
                                    FieldText fld1 = ctx.CastTo<FieldText>(field1);
                                    fld1.Update();

                                    Field field2 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "JobNum" + "' Type='Text' />", true, AddFieldOptions.DefaultValue);
                                    FieldText fld2 = ctx.CastTo<FieldText>(field2);
                                    fld2.Update();

                                    Field field3 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "Called" + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld3 = ctx.CastTo<FieldDateTime>(field3);
                                    fld3.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld3.Update();

                                    Field field4 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "CalledFor" + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld4 = ctx.CastTo<FieldDateTime>(field4);
                                    fld4.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld4.Update();

                                    Field field5 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "Start" + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld5 = ctx.CastTo<FieldDateTime>(field5);
                                    fld5.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld5.Update();

                                    Field field6 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "Complete" + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld6 = ctx.CastTo<FieldDateTime>(field6);
                                    fld6.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld6.Update();

                                    Guid lkpFldGuid7 = Guid.NewGuid();
                                    var lookupFieldXml7 = "<Field ID='{" + lkpFldGuid7 + "}' DisplayName='" + "SupName" + "' Type=\"Lookup\" />";
                                    var field7 = list.Fields.AddFieldAsXml(lookupFieldXml7, false, AddFieldOptions.AddToAllContentTypes);
                                    var fld7 = ctx.CastTo<FieldLookup>(field7);
                                    fld7.LookupList = listLkp.Id.ToString(); //SuppliersList :- "0AA3B4BD-0303-4AF4-AE7A-1DFA9E59B0C7"
                                    fld7.LookupField = "SupName";
                                    fld7.Update();

                                    Guid lkpFldGuid7Ref = Guid.NewGuid();
                                    var lookupFieldXml7Ref = string.Concat("<Field Type='Lookup' DisplayName='SupId' List='" + listLkp.Id.ToString() //"{0AA3B4BD-0303-4AF4-AE7A-1DFA9E59B0C7}"
                                                            + "' ShowField='SupId' FieldRef='" + lkpFldGuid7 + "'" + " ID='" + lkpFldGuid7Ref + "' />");
                                    var field7Ref = list.Fields.AddFieldAsXml(lookupFieldXml7Ref, false, AddFieldOptions.AddToAllContentTypes);

                                    Field field8 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "Supervisor" + "' Type='User' />", true, AddFieldOptions.DefaultValue);
                                    FieldUser fld8 = ctx.CastTo<FieldUser>(field8);

                                    Field field9 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "Duration" + "' Type='Number' />", true, AddFieldOptions.DefaultValue);
                                    FieldText fld9 = ctx.CastTo<FieldText>(field9);
                                    fld9.Update();

                                    Field field10 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "CC" + "' Type='Text' />", true, AddFieldOptions.DefaultValue);
                                    FieldText fld10 = ctx.CastTo<FieldText>(field10);
                                    fld10.Update();



                                    Field field11 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "CalledBest" + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld11 = ctx.CastTo<FieldDateTime>(field11);
                                    fld11.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld11.Update();

                                    Field field12 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "CalledForBest" + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld12 = ctx.CastTo<FieldDateTime>(field12);
                                    fld12.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld12.Update();

                                    Field field13 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "StartBest" + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld13 = ctx.CastTo<FieldDateTime>(field13);
                                    fld13.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld13.Update();

                                    Field field14 = list.Fields.AddFieldAsXml("<Field DisplayName='" + "CompleteBest" + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                                    FieldDateTime fld14 = ctx.CastTo<FieldDateTime>(field14);
                                    fld14.DisplayFormat = DateTimeFieldFormatType.DateOnly;
                                    fld14.Update();

                                    ctx.ExecuteQuery();                                    
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Data" + "---" + "Create Jobs List Columns");
                                }
                                #endregion

                                #region Rename Title Column
                                try
                                {
                                    FieldCollection collField = list.Fields;
                                    Field oneField = collField.GetByInternalNameOrTitle("Title");
                                    oneField.Title = "Activity"; //new column name replace of Title
                                    oneField.Required = false;
                                    //oneField.DefaultValue = "test";
                                    oneField.Update();
                                    ctx.Load(collField);
                                    ctx.Load(oneField);
                                    ctx.ExecuteQuery();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Data" + "---" + "Rename Jobs List Columns");
                                }
                                #endregion

                                #region Create View Best Dates
                                try
                                {
                                    ViewCollection viewColl = list.Views;

                                    string[] viewFields = { "Checkmark", "PercentComplete", "CC", "Title", "SupName", "CalledBest", "CalledForBest", "StartBest", "CompleteBest", "Duration" };
                                    ViewCreationInformation creationInfo = new ViewCreationInformation();
                                    creationInfo.Title = "Best Possible Dates";
                                    creationInfo.RowLimit = 50;
                                    creationInfo.ViewFields = viewFields;
                                    creationInfo.ViewTypeKind = Microsoft.SharePoint.Client.ViewType.None;
                                    //creationInfo.SetAsDefaultView = true;
                                    creationInfo.Paged = true;
                                    viewColl.Add(creationInfo);

                                    ctx.ExecuteQuery();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Data" + "---" + "Create Jobs List View for Best Possible Dates");
                                }
                                #endregion

                                #region Create View
                                try
                                {
                                    ViewCollection viewColl = list.Views;

                                    string[] viewFields = { "Checkmark", "PercentComplete", "CC", "Title", "SupName", "Called", "CalledFor", "Start", "Complete", "Duration" };
                                    ViewCreationInformation creationInfo = new ViewCreationInformation();
                                    creationInfo.Title = "Actual Dates";
                                    creationInfo.RowLimit = 50;
                                    creationInfo.ViewFields = viewFields;
                                    creationInfo.ViewTypeKind = Microsoft.SharePoint.Client.ViewType.None;
                                    creationInfo.SetAsDefaultView = true;
                                    creationInfo.Paged = true;
                                    viewColl.Add(creationInfo);

                                    ctx.ExecuteQuery();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Data" + "---" + "Create Jobs List View for Actual Possible Dates");
                                }
                                #endregion
                                
                            }
                            #region Create and Update Task list Items
                            try
                            {
                                //ListItemCollection getListItemsCol = gl.getListDataVal(ctx, listDelayName);
                                #region Get List Item Collection from SP

                                ListItemCollection getListItemsCol = null;
                                List getList = ctx.Web.Lists.GetByTitle(listDelayName);
                                CamlQuery camlQuery = new CamlQuery();
                                camlQuery.ViewXml = "<View><Query></Query></View>";
                                ListItemCollection getListItemsCollection = getList.GetItems(camlQuery);
                                ctx.Load(getListItemsCollection);
                                ctx.ExecuteQuery();

                                if (getListItemsCollection != null && getListItemsCollection.Count > 0)
                                {
                                    getListItemsCol = getListItemsCollection;
                                }

                                #endregion

                                DataTable dt = new DataTable();

                                //TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");

                                dt.Columns.Add("ID");
                                dt.Columns.Add("CstCallId");
                                dt.Columns.Add("JobNum");
                                dt.Columns.Add("Called");
                                dt.Columns.Add("CalledFor");
                                dt.Columns.Add("Start");
                                dt.Columns.Add("Complete");

                                dt.Columns.Add("CalledBest");
                                dt.Columns.Add("CalledForBest");
                                dt.Columns.Add("StartBest");
                                dt.Columns.Add("CompleteBest");

                                dt.Columns.Add("Supplier");
                                dt.Columns.Add("Title");
                                dt.Columns.Add("Supervisor");
                                dt.Columns.Add("Duration");

                                if (getListItemsCol != null)
                                {
                                    foreach (ListItem listItemsCol in getListItemsCol)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["ID"] = listItemsCol["ID"];
                                        dr["CstCallId"] = listItemsCol["CstCallId"];
                                        dr["JobNum"] = listItemsCol["JobNum"];

                                        if (listItemsCol["Called"] == null)
                                        {
                                            //DateTime? value = null;
                                        }
                                        else { dr["Called"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Called"].ToString()), cstZone); }

                                        if (listItemsCol["CalledFor"] == null) { }//{ dr["CalledFor"] = DateTime.Now; }
                                        else { dr["CalledFor"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["CalledFor"].ToString()), cstZone); }

                                        if (listItemsCol["Start"] == null) { }//{ dr["Start"] = DateTime.Now; }
                                        else { dr["Start"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Start"].ToString()), cstZone); }

                                        if (listItemsCol["Complete"] == null) { }//{ dr["Complete"] = DateTime.Now; }
                                        else { dr["Complete"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Complete"].ToString()), cstZone); }


                                        if (listItemsCol["CalledBest"] == null)
                                        {
                                            //DateTime? value = null;
                                        }
                                        else { dr["CalledBest"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["CalledBest"].ToString()), cstZone); }

                                        if (listItemsCol["CalledForBest"] == null) { }//{ dr["CalledFor"] = DateTime.Now; }
                                        else { dr["CalledForBest"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["CalledForBest"].ToString()), cstZone); }

                                        if (listItemsCol["StartBest"] == null) { }//{ dr["Start"] = DateTime.Now; }
                                        else { dr["StartBest"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["StartBest"].ToString()), cstZone); }

                                        if (listItemsCol["CompleteBest"] == null) { }//{ dr["Complete"] = DateTime.Now; }
                                        else { dr["CompleteBest"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["CompleteBest"].ToString()), cstZone); }

                                        var supLkp = listItemsCol["SupName"] as FieldLookupValue;
                                        if (supLkp != null)
                                        {
                                            //dr["ReasonId"] = supLkp.LookupId;
                                            dr["Supplier"] = supLkp.LookupValue;
                                        }

                                        if (listItemsCol["Title"] == null) { dr["Title"] = "No Title"; }
                                        else { dr["Title"] = listItemsCol["Title"]; }

                                        dr["Supervisor"] = listItemsCol["Supervisor"];
                                        dr["Duration"] = listItemsCol["Duration"];

                                        dt.Rows.Add(dr);
                                    }
                                }
                                else
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["ID"] = "";
                                    dr["CstCallId"] = "";
                                    dr["JobNum"] = "";
                                    dr["Called"] = "";
                                    dr["CalledFor"] = "";
                                    dr["Start"] = "";
                                    dr["Complete"] = "";

                                    dr["CalledBest"] = "";
                                    dr["CalledForBest"] = "";
                                    dr["StartBest"] = "";
                                    dr["CompleteBest"] = "";

                                    dr["Supplier"] = "";
                                    dr["Title"] = "";
                                    dr["Supervisor"] = "";
                                    dr["Duration"] = "";

                                    dt.Rows.Add(dr);
                                }

                                //dbConnection conn = new dbConnection();
                                //DataTable tempTable = null;
                                //tempTable = conn.executeSelectNoParameter(sqlQuery);
                                ListItemCreationInformation itemCreateInfo = null;
                                ListItem oListItem = null;
                                List oList = ctx.Web.Lists.GetByTitle(listDelayName);

                                Excel.Application xlApp;
                                Excel.Workbook xlWorkBook;
                                Excel.Worksheet xlWorkSheet;
                                object misValue = System.Reflection.Missing.Value;

                                xlApp = new Excel.Application();
                                xlWorkBook = xlApp.Workbooks.Add(misValue);
                                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                                dbConnection conn = new dbConnection();
                                SqlParameter[] parameter = {                                
                                                                    new SqlParameter("@JobNum", SqlDbType.VarChar) { Value = tempTable.Rows[j][0].ToString() }
                                                               };
                                DataTable jobsSQLData = conn.executeSelectQuery(queryJobsDelayItems, parameter);


                                if (1==1)  //if (getListItemsCol != null && getListItemsCol.Count > 0)
                                {
                                    for (int i = 0; i < jobsSQLData.Rows.Count; i++)
                                    {
                                        try
                                        {
                                            DataRow[] drExists = dt.Select("CstCallId = '" + jobsSQLData.Rows[i]["CstCallId"].ToString() + "'");
                                            if (drExists != null && drExists.Length > 0)
                                            {
                                                Console.WriteLine("Found Call Forwards List :- " + listDelayName + " and Delay :- " + jobsSQLData.Rows[i]["CstCallId"].ToString());
                                                //////////////xlWorkSheet.Cells[i + 1, 1] = "Found Job Number - ";
                                                //////////////xlWorkSheet.Cells[i + 1, 2] = jobsSQLData.Rows[i]["JobNum"].ToString();
                                                //////////////xlWorkSheet.Cells[i + 1, 3] = "Found Delay :- ";
                                                //////////////xlWorkSheet.Cells[i + 1, 4] = jobsSQLData.Rows[i]["CstCallId"].ToString();
                                                #region Found Job Items/Tasks
                                                /*
                                            //var drExists12345 = (from m in dt.AsEnumerable()
                                            //                     where m.Field<string>("CstCallId") == jobsSQLData.Rows[i]["CstCallId"].ToString()
                                            //                       && m.Field<DateTime>("Called") == Convert.ToDateTime(jobsSQLData.Rows[i]["Called"].ToString()).AddHours(10)
                                            //                     select m);


                                            //var drExists11 = dt.AsEnumerable().Where(x => x.Field<string>("CstCallId") == "427708" && x.Field<string>("JobNum") == "5032").FirstOrDefault();                                            
                                            //var drExists11111 = dt.AsEnumerable().Where(x => x.Field<string>("CstCallId") == "427708" && x.Field<string>("JobNum") == "5032").CopyToDataTable<DataRow>();
                                            //DataTable drExists1 = dt.AsEnumerable().Where(x => x.Field<string>("CstCallId") == "427708" && x.Field<string>("JobNum") == "5032").CopyToDataTable();

                                            
                                            string activity = "";
                                            if (jobsSQLData.Rows[i]["Activity"].ToString() == "") { activity = "No Title"; }
                                            else { activity = jobsSQLData.Rows[i]["Activity"].ToString(); }
                                            
                                            #region Commented working linq dates
                                            
                                            //var rowExists = dt.AsEnumerable().Where(x => x.Field<string>("CstCallId") == jobsSQLData.Rows[i]["CstCallId"].ToString()
                                            //                                                && x.Field<string>("Title") == activity //jobsSQLData.Rows[i]["Activity"].ToString()
                                            //                                                && x.Field<string>("Supplier") == jobsSQLData.Rows[i]["Supplier"].ToString()
                                            //                                                && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("Called"))).ToString("dd/MM/yyyy")) == 
                                            //                                                        Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["Called"].ToString())).ToString("dd/MM/yyyy"))
                                            //                                                        //Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["Called"].ToString()).AddHours(-10)).ToString("dd/MM/yyyy"))
                                            //                                                && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("CalledFor"))).ToString("dd/MM/yyyy")) == 
                                            //                                                        Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["CalledFor"].ToString())).ToString("dd/MM/yyyy"))
                                            //                                                && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("Start"))).ToString("dd/MM/yyyy")) ==     
                                            //                                                        Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["Start"].ToString())).ToString("dd/MM/yyyy"))
                                            //                                                && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("Complete"))).ToString("dd/MM/yyyy")) == 
                                            //                                                        Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["Complete"].ToString())).ToString("dd/MM/yyyy"))                                            
                                            //                                                //&& x.Field<string>("Called").Substring(0, 10) == (Convert.ToDateTime(jobsSQLData.Rows[i]["Called"].ToString()).AddHours(-10)).ToString("dd/MM/yyyy")
                                            //                                                //&& x.Field<string>("Complete") == (Convert.ToDateTime(jobsSQLData.Rows[i]["Complete"].ToString()).AddHours(-10)).ToString()
                                            //                                             );
                                            
                                            #endregion

                                            #region Commented Best Possible Dates Linq
                                            //var rowExists = dt.AsEnumerable().Where(x => x.Field<string>("CstCallId") == jobsSQLData.Rows[i]["CstCallId"].ToString()
                                            //                            && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("CalledBest"))).ToString("dd/MM/yyyy")) ==
                                            //                                    Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["CalledBest"].ToString())).ToString("dd/MM/yyyy"))
                                            //                            && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("CalledForBest"))).ToString("dd/MM/yyyy")) ==
                                            //                                    Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["CalledForBest"].ToString())).ToString("dd/MM/yyyy"))
                                            //                            && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("StartBest"))).ToString("dd/MM/yyyy")) ==
                                            //                                    Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["StartBest"].ToString())).ToString("dd/MM/yyyy"))
                                            //                            && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("CompleteBest"))).ToString("dd/MM/yyyy")) ==
                                            //                                    Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["CompleteBest"].ToString())).ToString("dd/MM/yyyy"))
                                            //                        );
                                            #endregion

                                            var rowExists = dt.AsEnumerable().Where(x => x.Field<string>("CstCallId") == jobsSQLData.Rows[i]["CstCallId"].ToString()
                                                                        && Convert.ToDateTime((Convert.ToDateTime(x.Field<string>("Complete"))).ToString("dd/MM/yyyy")) ==
                                                                                Convert.ToDateTime((Convert.ToDateTime(jobsSQLData.Rows[i]["complete"].ToString())).ToString("dd/MM/yyyy"))
                                                                    );

                                            DataTable drExists1 = null;
                                            if (rowExists.Any())
                                            {drExists1 = rowExists.CopyToDataTable();}

                                            #region TimeConversion

                                            /////////////
                                            ////////Microsoft.SharePoint.Client.TimeZone timeZone = ctx.Web.RegionalSettings.TimeZone;
                                            ////////var filedTime = timeZone.LocalTimeToUTC(Convert.ToDateTime(dt.Rows[i]["Called"].ToString()));

                                            //////////DateTime utcTime1 = new DateTime(2008, 6, 19, 7, 0, 0);
                                            ////////DateTime utcTime1 = new DateTime(
                                            ////////                Int32.Parse((Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("yyyy"))
                                            ////////                , Int32.Parse((Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("MM"))
                                            ////////                , Int32.Parse((Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("dd"))
                                            ////////                , Int32.Parse((Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("hh"))
                                            ////////                , 0
                                            ////////                , 0
                                            ////////                );
                                            ////////utcTime1 = DateTime.SpecifyKind(utcTime1, DateTimeKind.Utc);
                                            ////////DateTimeOffset utcTime2 = utcTime1;
                                            ////////Console.WriteLine("Converted {0} {1} to a DateTimeOffset value of {2}",
                                            ////////                  utcTime1,
                                            ////////                  utcTime1.Kind.ToString(),
                                            ////////                  utcTime2);


                                            ////////TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                                            ////////DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(dt.Rows[i]["Called"].ToString()), cstZone);
                                            ////////Console.WriteLine("The date and time are {0} ---------- {1}.",
                                            ////////                  cstTime,
                                            ////////                  cstZone.IsDaylightSavingTime(cstTime) ?
                                            ////////                          cstZone.DaylightName : cstZone.StandardName);

                                            /////////////

                                            ////////TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time");
                                            ////////DateTime dateTime = TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTime(Convert.ToDateTime(drExists1.Rows[i]["Called"].ToString()), timeZoneInfo), timeZoneInfo);

                                            ////////Console.WriteLine("Sharepoint List Called Date without changes :- "+dt.Rows[i]["Called"].ToString());
                                            ////////Console.WriteLine(jobsSQLData.Rows[i]["Called"].ToString());
                                            ////////Console.WriteLine(dateTime.ToString("yyyy-MM-dd HH-mm-ss"));
                                            ////////Console.WriteLine(dateTime.ToString());


                                            ////////Console.WriteLine(Convert.ToDateTime(drExists1.Rows[i]["Called"].ToString()).ToUniversalTime());
                                            ////////Console.WriteLine("Added 10 min: - "+Convert.ToDateTime(drExists1.Rows[i]["Called"].ToString()).AddHours(10));

                                            ////////Console.WriteLine(System.TimeZone.CurrentTimeZone.ToUniversalTime(Convert.ToDateTime(drExists1.Rows[i]["Called"].ToString())));

                                            ////////Console.WriteLine("SharePoint List without change " + drExists1.Rows[i]["Called"].ToString());
                                            ////////Console.WriteLine("SharePoint List US Format  "+(
                                            ////////                TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTime(Convert.ToDateTime(drExists1.Rows[i]["Called"].ToString()), 
                                            ////////                TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")), 
                                            ////////                TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time"))).ToString());
                                            ////////Console.WriteLine("SharePoint List Australian Format  " + (
                                            ////////                TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTime(Convert.ToDateTime(drExists1.Rows[i]["Called"].ToString()),
                                            ////////                TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time")),
                                            ////////                TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"))).ToString());

                                            ////////Console.WriteLine("SQL without change " + jobsSQLData.Rows[i]["Called"].ToString());
                                            ////////Console.WriteLine("SQL US Format  " + (
                                            ////////                TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTime(Convert.ToDateTime(jobsSQLData.Rows[i]["Called"].ToString()),
                                            ////////                TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")),
                                            ////////                TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time"))).ToString());
                                            ////////Console.WriteLine("SQL Australian Format  " + (
                                            ////////                TimeZoneInfo.ConvertTime(TimeZoneInfo.ConvertTime(Convert.ToDateTime(jobsSQLData.Rows[i]["Called"].ToString()),
                                            ////////                TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time")),
                                            ////////                TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"))).ToString()); 
                                            #endregion

                                            if (drExists1 != null && drExists1.Rows.Count > 0)
                                            {
                                                Console.WriteLine("Found - " + jobsSQLData.Rows[i]["CstCallId"].ToString());
                                                xlWorkSheet.Cells[i + 1, 1] = "Found Job Number - ";
                                                xlWorkSheet.Cells[i + 1, 2] = jobsSQLData.Rows[i][0].ToString();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Updating Best Possible dates into SQL for CstCallId - " + jobsSQLData.Rows[i]["CstCallId"].ToString());
                                                xlWorkSheet.Cells[i + 1, 1] = "Updating - ";
                                                xlWorkSheet.Cells[i + 1, 2] = jobsSQLData.Rows[i][0].ToString();
                                                
                                                //oListItem = oList.GetItemById(drExists[0].ItemArray[0].ToString());
                                                oListItem = oList.GetItemById(drExists[0]["ID"].ToString());

                                                //oListItem["Title"] = jobsSQLData.Rows[i]["Activity"].ToString();
                                                //oListItem["Supplier"] = jobsSQLData.Rows[i]["Supplier"].ToString();
                                                //oListItem["SupName"] = gl.GetLookupFieldValue(ctx, jobsSQLData.Rows[i]["SupName"].ToString(), "SuppliersList", "SupName", "Number");
                                                //oListItem.Update();

                                                oListItem["CalledBest"] = Convert.ToDateTime(jobsSQLData.Rows[i]["CalledBest"].ToString());
                                                oListItem["CalledForBest"] = Convert.ToDateTime(jobsSQLData.Rows[i]["CalledForBest"].ToString());
                                                oListItem["StartBest"] = Convert.ToDateTime(jobsSQLData.Rows[i]["StartBest"].ToString());
                                                oListItem["CompleteBest"] = Convert.ToDateTime(jobsSQLData.Rows[i]["CompleteBest"].ToString());

                                                oListItem.Update();
                                                ctx.ExecuteQuery();
                                            }
                                            
                                            */
                                                #endregion
                                            }
                                            else
                                            {
                                                Console.WriteLine("Inserting Call forwards List :- " + listDelayName + " and Delay :- " + jobsSQLData.Rows[i]["CstCallId"].ToString());
                                                itemCreateInfo = new ListItemCreationInformation();

                                                ListItemCollection getSelectiveListItemsCol = gl.getListSelectiveDataVal(ctx, listDelayName, jobsSQLData.Rows[i]["Activity"].ToString());
                                                if (getSelectiveListItemsCol != null && getSelectiveListItemsCol.Count > 0)
                                                {
                                                    foreach (ListItem listItemsCol in getSelectiveListItemsCol)
                                                    {
                                                        //Int32.Parse(listItemsCol["CstCallId"].ToString());
                                                        DataRow[] drExistsCstCallId = jobsSQLData.Select("CstCallId = '" + listItemsCol["CstCallId"].ToString() + "'");
                                                        if (drExistsCstCallId != null && drExistsCstCallId.Length > 0)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            ListItem oListItemDelete = oList.GetItemById(Int32.Parse(listItemsCol["ID"].ToString()));
                                                            oListItemDelete.DeleteObject();
                                                            ctx.ExecuteQuery();
                                                        }
                                                    }
                                                }
                                                /////////////////////

                                                oListItem = oList.AddItem(itemCreateInfo);

                                                //oListItem["SupName"] = gl.GetLookupFieldValue(ctx, jobsSQLData.Rows[i]["Supplier"].ToString(), "SuppliersList", "SupName", "Text");
                                                oListItem["SupName"] = gl.GetLookupFieldValueId(ctx, jobsSQLData.Rows[i]["Supplier"].ToString(), "SuppliersList", "SupName", "Text", jobsSQLData.Rows[i]["SupplierId"].ToString(), "SupId", "Number", gl, listDelayName);

                                                if (oListItem["SupName"] != null)
                                                {
                                                    oListItem.Update();

                                                    oListItem["CC"] = jobsSQLData.Rows[i]["s_costCentreCode"].ToString();
                                                    oListItem["CstCallId"] = jobsSQLData.Rows[i]["CstCallId"].ToString();
                                                    oListItem["JobNum"] = jobsSQLData.Rows[i]["JobNum"].ToString();
                                                    oListItem["Title"] = jobsSQLData.Rows[i]["Activity"].ToString();

                                                    if (jobsSQLData.Rows[i]["Called"].ToString() == "") { oListItem["Called"] = null; }
                                                    else { oListItem["Called"] = Convert.ToDateTime(jobsSQLData.Rows[i]["Called"].ToString()); }

                                                    if (jobsSQLData.Rows[i]["CalledFor"].ToString() == "") { oListItem["CalledFor"] = null; }
                                                    else { oListItem["CalledFor"] = Convert.ToDateTime(jobsSQLData.Rows[i]["CalledFor"].ToString()); }

                                                    if (jobsSQLData.Rows[i]["Start"].ToString() == "") { oListItem["Start"] = null; }
                                                    else { oListItem["Start"] = Convert.ToDateTime(jobsSQLData.Rows[i]["Start"].ToString()); }

                                                    if (jobsSQLData.Rows[i]["Complete"].ToString() == "") { oListItem["Complete"] = null; }
                                                    else { oListItem["Complete"] = Convert.ToDateTime(jobsSQLData.Rows[i]["Complete"].ToString()); }


                                                    if (jobsSQLData.Rows[i]["CalledBest"].ToString() == "") { oListItem["CalledBest"] = null; }
                                                    else { oListItem["CalledBest"] = Convert.ToDateTime(jobsSQLData.Rows[i]["CalledBest"].ToString()); }

                                                    if (jobsSQLData.Rows[i]["CalledForBest"].ToString() == "") { oListItem["CalledForBest"] = null; }
                                                    else { oListItem["CalledForBest"] = Convert.ToDateTime(jobsSQLData.Rows[i]["CalledForBest"].ToString()); }

                                                    if (jobsSQLData.Rows[i]["StartBest"].ToString() == "") { oListItem["StartBest"] = null; }
                                                    else { oListItem["StartBest"] = Convert.ToDateTime(jobsSQLData.Rows[i]["StartBest"].ToString()); }

                                                    if (jobsSQLData.Rows[i]["CompleteBest"].ToString() == "") { oListItem["CompleteBest"] = null; }
                                                    else { oListItem["CompleteBest"] = Convert.ToDateTime(jobsSQLData.Rows[i]["CompleteBest"].ToString()); }

                                                    oListItem["PercentComplete"] = Int32.Parse(jobsSQLData.Rows[i]["b_complete"].ToString()) / 100;

                                                    //oListItem["Supplier"] = jobsSQLData.Rows[i]["Supplier"].ToString();

                                                    foreach (UserValues uVal in uValue)
                                                    {
                                                        if (uVal.Title.ToString() == jobsSQLData.Rows[i]["Supervisor"].ToString())
                                                        {
                                                            FieldUserValue userValue = new FieldUserValue();
                                                            userValue.LookupId = uVal.Id;
                                                            oListItem["Supervisor"] = userValue;
                                                            break;
                                                        }
                                                    }
                                                    oListItem["Duration"] = jobsSQLData.Rows[i]["ReqDays"].ToString();

                                                    //oListItem.Update();

                                                    //ctx.ExecuteQuery();

                                                    xlWorkSheet.Cells[i + 1, 1] = "Inserting - ";
                                                    xlWorkSheet.Cells[i + 1, 2] = jobsSQLData.Rows[i]["JobNum"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 3] = jobsSQLData.Rows[i]["CstCallId"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 4] = jobsSQLData.Rows[i]["Supplier"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 5] = jobsSQLData.Rows[i]["SupplierId"].ToString();

                                                    var supNameLkp = oListItem["SupName"] as FieldLookupValue;
                                                    if (supNameLkp != null)
                                                    {
                                                        xlWorkSheet.Cells[i + 1, 6] = supNameLkp.LookupId;
                                                        //xlWorkSheet.Cells[i + 1, 7] = supNameLkp.LookupValue;
                                                    }

                                                    oListItem.Update();
                                                    ctx.ExecuteQuery();
                                                }
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                            GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Data" + "---" + listDelayName + "---" + jobsSQLData.Rows[i]["CstCallId"].ToString());
                                        }
                                    }
                                }
                                //xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_" + listDelayName + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.SaveAs(ConfigurationManager.AppSettings.Get("LogLocation") + "\\SqlToSpLog_" + listDelayName + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.Close(true, misValue, misValue);
                                xlApp.Quit();

                                gl.releaseObject(xlWorkSheet);
                                gl.releaseObject(xlWorkBook);
                                gl.releaseObject(xlApp);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Data" + "---" + listDelayName + "---" + "Create and Update Task list Items");
                            }
                            #endregion

                            #region Associate WF

                            //////string workflowName = "WF-" + listDelayName;

                            //////Web wfWeb = ctx.Web;
                            //////WorkflowServicesManager wfServicesManager = new WorkflowServicesManager(ctx, wfWeb);
                            //////WorkflowDeploymentService wfDeploymentService = wfServicesManager.GetWorkflowDeploymentService();
                            //////WorkflowDefinitionCollection wfDefinitions = wfDeploymentService.EnumerateDefinitions(false);
                            //////ctx.Load(wfDefinitions, wfDefs => wfDefs.Where(wfd => wfd.DisplayName == workflowName));
                            //////ctx.ExecuteQuery();

                            //////if (wfDefinitions.Count > 0 && wfDefinitions != null)
                            //////{
                            //////    //WorkflowDefinition wfDefinition = wfDefinitions.First();
                            //////    //Guid listId = gl.getListGuid(ctx, listDelayName);
                            //////    //gl.addWorkflowSubscription(ctx, listDelayName, listId);        
                            //////}

                            //string workflowName = "WF-" + listDelayName;

                            //Web ctxWeb = ctx.Web;
                            ////Workflow Services Manager which will handle all the workflow interaction.
                            //WorkflowServicesManager wfServicesManager = new WorkflowServicesManager(ctx, ctxWeb);

                            ////Deployment Service which holds all the Workflow Definitions deployed to the site
                            //WorkflowDeploymentService wfDeploymentService = wfServicesManager.GetWorkflowDeploymentService();

                            ////Get all the definitions from the Deployment Service, or get a specific definition using the GetDefinition method.
                            //WorkflowDefinitionCollection wfDefinitions = wfDeploymentService.EnumerateDefinitions(false);

                            //ctx.Load(wfDefinitions, wfDefs => wfDefs.Where(wfd => wfd.DisplayName == workflowName));

                            //ctx.ExecuteQuery();

                            //WorkflowDefinition wfDefinition = wfDefinitions.First();

                            ////The Subscription service is used to get all the Associations currently on the SPSite
                            //WorkflowSubscriptionService wfSubscriptionService = wfServicesManager.GetWorkflowSubscriptionService();

                            //// get all workflow associations
                            //var workflowAssociations = wfSubscriptionService.EnumerateSubscriptionsByDefinition(wfDefinition.Id);
                            //ctx.Load(workflowAssociations);
                            //ctx.ExecuteQuery();
                            //foreach (var association in workflowAssociations)
                            //{

                            //    //this will remove the association
                            //    Console.WriteLine(association.Name);
                            //    //wfSubscriptionService.DeleteSubscription(association.Id);
                            //    //ctx.ExecuteQuery();
                            //}

                            #endregion
                        }
                        catch (Exception e) 
                        { 
                            Console.WriteLine(e.Message);
                            GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Data" + "---" + "Starting operation on Delay List");
                        }
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //GlobalLogic gl = new GlobalLogic();
                GlobalLogic.ExceptionHandle(e, "Sql To Sp Create Jobs Data");
            }
            finally
            {
                //dt.Dispose();
            }
        }

        /// <summary>
        /// Update Delays in SP (SharePoint) or Framework DB, it compares the changes made either in SP or Framework DB.
        /// If a new record is created in SP then write it to SQL DB
        /// If the latest changes in a particular record are made in database then writes them to SP or if the latest changes are made in SP then update Framework DB
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="userName"></param>
        /// <param name="passwordString"></param>
        /// <param name="key"></param>
        /// <param name="appSettingsKey"></param>
        /// <param name="columnName"></param>
        /// <param name="tempTable"></param>
        /// <param name="queryJobsDelayItems"></param>
        private static void SpToSqlCreateJobsDelayData(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, DataTable tempTable, string queryJobsDelayItems)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                ListItemCollection getListItemsCol = null;
                DataTable dt = null;

                #region UserValues
                //UserValues: Begin
                List<UserValues> uValue = null;
                try
                {
                    UserCollection user = ctx.Web.SiteUsers;
                    ctx.Load(user);
                    ctx.ExecuteQuery();
                    uValue = new List<UserValues>(10000);
                    foreach (User usr in user)
                    {
                        UserValues uv = new UserValues();
                        uv.Email = usr.Email;
                        uv.Id = usr.Id;
                        uv.LoginName = usr.LoginName;
                        uv.Title = usr.Title;
                        uValue.Add(uv);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //UserValues: End
                #endregion

                if (tempTable != null && tempTable.Rows.Count > 0)
                {
                    for (int j = 0; j < tempTable.Rows.Count; j++)
                    {
                        try
                        {
                            string listDelayName = tempTable.Rows[j][0].ToString() + "_Delay";
                            #region Create and Update Task list Items
                            try
                            {
                                //getListItemsCol = gl.getListDataVal(ctx, listDelayName);
                                #region Get List Item Collection from SP
                                getListItemsCol = null;
                                //ListItemCollection getListItemsCol = null;
                                List getList = ctx.Web.Lists.GetByTitle(listDelayName);
                                CamlQuery camlQuery = new CamlQuery();
                                camlQuery.ViewXml = "<View><Query></Query></View>";
                                ListItemCollection getListItemsCollection = getList.GetItems(camlQuery);
                                ctx.Load(getListItemsCollection);
                                ctx.ExecuteQuery();

                                if (getListItemsCollection != null && getListItemsCollection.Count > 0)
                                {
                                    getListItemsCol = getListItemsCollection;
                                }

                                #endregion

                                dt = new DataTable();
                                ////ListItemCollection getListItemsCol = gl.getListDataVal(ctx, listDelayName);
                                ////DataTable dt = new DataTable();                                

                                //TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");                                
                                //DateTime cstTime;

                                dt.Columns.Add("ID");
                                dt.Columns.Add("DelayId");
                                dt.Columns.Add("JobNumber");
                                dt.Columns.Add("Reason");
                                dt.Columns.Add("ReasonId");
                                dt.Columns.Add("ReasonValue");

                                dt.Columns.Add("ReasonSqlId");
                                dt.Columns.Add("ReasonSqlIdId");
                                dt.Columns.Add("ReasonSqlValue");

                                dt.Columns.Add("Title");
                                dt.Columns.Add("DelayDate");
                                dt.Columns.Add("ToDelayDate");
                                dt.Columns.Add("AreYouSure");
                                dt.Columns.Add("JobId");
                                dt.Columns.Add("CstId");
                                dt.Columns.Add("timeCreated");
                                dt.Columns.Add("timeModified");

                                if (getListItemsCol != null)
                                {
                                    foreach (ListItem listItemsCol in getListItemsCol)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["ID"] = listItemsCol["ID"];
                                        dr["DelayId"] = listItemsCol["DelayId"];
                                        dr["JobNumber"] = listItemsCol["JobNumber"];

                                        dr["Reason"] = listItemsCol["Reason"];
                                        var reasonLkp = listItemsCol["Reason"] as FieldLookupValue;
                                        if (reasonLkp != null)
                                        {
                                            dr["ReasonId"] = reasonLkp.LookupId;
                                            dr["ReasonValue"] = reasonLkp.LookupValue;
                                        }

                                        dr["ReasonSqlId"] = listItemsCol["ReasonId"];
                                        var reasonSqlIdLkp = listItemsCol["ReasonId"] as FieldLookupValue;
                                        if (reasonLkp != null)
                                        {
                                            dr["ReasonSqlIdId"] = reasonSqlIdLkp.LookupId;
                                            dr["ReasonSqlValue"] = reasonSqlIdLkp.LookupValue;
                                        }


                                        dr["Title"] = listItemsCol["Title"];

                                        dr["DelayDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["DelayDate"].ToString()), cstZone);

                                        if (listItemsCol["ToDelayDate"] == null)
                                        {
                                            dr["ToDelayDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["DelayDate"].ToString()), cstZone);
                                        }
                                        else
                                        { //dr["ToDelayDate"] = Convert.ToDateTime(listItemsCol["ToDelayDate"].ToString()).AddHours(10); 
                                            dr["ToDelayDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["ToDelayDate"].ToString()), cstZone);
                                        }


                                        if (listItemsCol["Modified"] == null) { dr["timeModified"] = null; }
                                        else { dr["timeModified"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Modified"].ToString()), cstZone); }

                                        if (listItemsCol["Created"] == null) { dr["timeCreated"] = null; }
                                        else { dr["timeCreated"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Created"].ToString()), cstZone); }

                                        dr["AreYouSure"] = listItemsCol["AreYouSure"];
                                        dr["CstId"] = listItemsCol["CstId"];
                                        dr["JobId"] = listItemsCol["JobId"];

                                        dt.Rows.Add(dr);
                                    }
                                }
                                else
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["ID"] = "";
                                    dr["DelayId"] = "";
                                    dr["JobNumber"] = "";
                                    dr["Reason"] = "";
                                    dr["ReasonId"] = "";
                                    dr["ReasonValue"] = "";
                                    dr["Title"] = "";
                                    dr["DelayDate"] = "";
                                    dr["ToDelayDate"] = "";
                                    dr["AreYouSure"] = "";
                                    dr["CstId"] = "";
                                    dr["JobId"] = "";

                                    dr["ReasonSqlId"] = "";
                                    dr["ReasonSqlIdId"] = "";
                                    dr["ReasonSqlValue"] = "";
                                    dr["timeCreated"] = "";
                                    dr["timeModified"] = "";


                                    dt.Rows.Add(dr);
                                }

                                //dbConnection conn = new dbConnection();
                                //DataTable tempTable = null;
                                //tempTable = conn.executeSelectNoParameter(sqlQuery);
                                ListItemCreationInformation itemCreateInfo = null;
                                ListItem oListItem = null;
                                List oList = ctx.Web.Lists.GetByTitle(listDelayName);

                                Excel.Application xlApp;
                                Excel.Workbook xlWorkBook;
                                Excel.Worksheet xlWorkSheet;
                                object misValue = System.Reflection.Missing.Value;

                                xlApp = new Excel.Application();
                                xlWorkBook = xlApp.Workbooks.Add(misValue);
                                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                                dbConnection conn = new dbConnection();
                                SqlParameter[] parameter = {                                
                                                                    new SqlParameter("@JobNumber", SqlDbType.VarChar) { Value = tempTable.Rows[j][0].ToString() }
                                                               };
                                //DataTable delayJobsSQLData = conn.executeSelectQuery(queryJobsDelayItems, parameter);
                                DataTable delayJobsSQLData = new DataTable();
                                using (SqlConnection con = new SqlConnection(@"Server=dbserver;Database=FworkSQLEcm;User Id=etssys; Password=c4ndy4u"))
                                {
                                    SqlCommand myCommand = new SqlCommand();
                                    DataTable dataTable = new DataTable();
                                    SqlDataAdapter myAdapter = new SqlDataAdapter();
                                    //myCommand.Connection = openConnection();
                                    if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
                                    {
                                        con.Open();
                                        myCommand.Connection = con;
                                    }
                                    myCommand.CommandTimeout = 120;
                                    myCommand.CommandText = queryJobsDelayItems;
                                    myCommand.Parameters.AddRange(parameter);
                                    myCommand.ExecuteNonQuery();
                                    myAdapter.SelectCommand = myCommand;
                                    myAdapter.Fill(dataTable);

                                    myAdapter.Dispose();
                                    delayJobsSQLData = dataTable;
                                }
                                string userComments, reason, delayDate, toDelayDate, cstId, jobId;
                                int reasonSqlId;

                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    try
                                    {
                                        //DataRow[] drExists = delayJobsSQLData.Select("DelayId = '" + dt.Rows[i]["DelayId"].ToString() + "'");
                                        DataRow[] drExists = delayJobsSQLData.Select("DelayId = '" + dt.Rows[i]["DelayId"].ToString() + "'");
                                        if (drExists != null && drExists.Length > 0)
                                        {
                                            /*var rowExists = delayJobsSQLData.AsEnumerable().Where(x => x.Field<string>("DelayId") == dt.Rows[i]["DelayId"].ToString()
                                                //   && x.Field<string>("Comments") == dt.Rows[i]["Title"].ToString()//title
                                                                                            && x.Field<string>("Reason") == dt.Rows[i]["ReasonValue"].ToString()
                                                                                            //&& (Convert.ToDateTime(x.Field<string>("DelayDate"))).ToString("dd/MM/yyyy") ==
                                                                                            && x.Field<DateTime>("DelayDate").ToString("dd/MM/yyyy") ==
                                                                                                    (Convert.ToDateTime(dt.Rows[i]["DelayDate"].ToString())).ToString("dd/MM/yyyy")
                                                                                            && x.Field<DateTime>("ToDelayDate").ToString("dd/MM/yyyy") ==
                                                                                                    (Convert.ToDateTime(dt.Rows[i]["ToDelayDate"].ToString())).ToString("dd/MM/yyyy")
                                                                                                );*/

                                            var rowExists = delayJobsSQLData.AsEnumerable().Where(x => x.Field<string>("DelayId") == dt.Rows[i]["DelayId"].ToString()
                                                //var rowExists = delayJobsSQLData.AsEnumerable().Where(x => x.Field<int>("DelayId") == Int32.Parse(dt.Rows[i]["DelayId"].ToString())
                                                //   && x.Field<string>("Comments") == dt.Rows[i]["Title"].ToString()//title
                                                                                            && x.Field<string>("Reason") == dt.Rows[i]["ReasonValue"].ToString()
                                                //&& (Convert.ToDateTime(x.Field<string>("DelayDate"))).ToString("dd/MM/yyyy") ==
                                                                                            && x.Field<DateTime>("DelayDate").ToString("dd/MM/yyyy") ==
                                                                                                    (Convert.ToDateTime(dt.Rows[i]["DelayDate"].ToString())).ToString("dd/MM/yyyy")
                                                                                            && x.Field<DateTime>("ToDelayDate").ToString("dd/MM/yyyy") ==
                                                                                                    (Convert.ToDateTime(dt.Rows[i]["ToDelayDate"].ToString())).ToString("dd/MM/yyyy")
                                                                                                );                                 

                                            DataTable drExists1 = null;
                                            if (rowExists.Any())
                                            { drExists1 = rowExists.CopyToDataTable(); }

                                            if (drExists1 != null && drExists1.Rows.Count > 0)
                                            //if (drExists1 != null && drExists1.Length > 0)
                                            {
                                                Console.WriteLine("SP - Found Delay List :- " + listDelayName + " and Delay :- " + dt.Rows[i]["DelayId"].ToString());
                                                ////////////xlWorkSheet.Cells[i + 1, 1] = "Found Job Number - ";
                                                ////////////xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["JobNumber"].ToString();
                                                ////////////xlWorkSheet.Cells[i + 1, 3] = "Found Delay :- ";
                                                ////////////xlWorkSheet.Cells[i + 1, 4] = dt.Rows[i]["DelayId"].ToString();
                                            }
                                            else
                                            {
                                                DateTime compareSQLDate;
                                                if (Convert.ToDateTime(delayJobsSQLData.Rows[i]["timeModified"].ToString()) > Convert.ToDateTime(delayJobsSQLData.Rows[i]["dateModified"].ToString()))
                                                {
                                                    compareSQLDate = Convert.ToDateTime(delayJobsSQLData.Rows[i]["timeModified"].ToString());
                                                }
                                                else
                                                {
                                                    compareSQLDate = Convert.ToDateTime(delayJobsSQLData.Rows[i]["dateModified"].ToString());
                                                }

                                                Console.WriteLine("SP Time:- " + Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString()).ToString());
                                                Console.WriteLine("SQL Time:- " + compareSQLDate);

                                                if (Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString()) > compareSQLDate)
                                                //if (Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString()) > Convert.ToDateTime(delayJobsSQLData.Rows[i]["timeModified"].ToString()))
                                                {
                                                    #region update delay from SP to SQL

                                                    //Console.WriteLine("SP - Updating Delay List :- " + listDelayName + " and Delay :- " + dt.Rows[i]["Delay - Id"].ToString());
                                                    Console.WriteLine("SP - Updating Delay List :- " + listDelayName + " and Delay :- " + dt.Rows[i][ConfigurationManager.AppSettings.Get("DelayToSqlUpdateCheckParam")].ToString());

                                                    userComments = dt.Rows[i]["Title"].ToString();
                                                    delayDate = (Convert.ToDateTime(dt.Rows[i]["DelayDate"].ToString())).ToString("dd'/'MM'/'yyyy");
                                                    jobId = dt.Rows[i]["JobId"].ToString();
                                                    if (dt.Rows[i]["ToDelayDate"].ToString() == "")
                                                    {
                                                        toDelayDate = (Convert.ToDateTime(dt.Rows[i]["DelayDate"].ToString())).ToString("dd'/'MM'/'yyyy"); ; ;
                                                    }
                                                    else
                                                    {
                                                        toDelayDate = (Convert.ToDateTime(dt.Rows[i]["ToDelayDate"].ToString())).ToString("dd'/'MM'/'yyyy");
                                                    }
                                                    reasonSqlId = Int32.Parse(dt.Rows[i]["ReasonSqlValue"].ToString().Substring(0, dt.Rows[i]["ReasonSqlValue"].ToString().IndexOf('.')));

                                                    DateTime d_modified = Convert.ToDateTime(DateTime.Now.ToString("dd'/'MM'/'yyyy"));
                                                    DateTime t_modified = DateTime.Now;
                                                    //DateTime t_modified = Convert.ToDateTime(DateTime.Now.ToString("_ddMMyyyy_HHmmss"));

                                                    string sclearsqlIns = @"UPDATE FworkSQLEcm.dbo.cst_dly 
                                                                        SET l_cst_dlyReas_id=@P11,l_user_modified_id=@P2,d_modified=@P3,t_modified=@P4,f_concurrency=@P5 ,d_delay=@P7,d_to=@P8, s_notes_derived=@P9, m_notes=@P10 
                                                                        WHERE l_cst_dly_id=@P6 ";

                                                    SqlParameter[] parameterUpd = {      
                                                                     new SqlParameter("@P11", SqlDbType.Int) { Value = reasonSqlId },
                                                                     new SqlParameter("@P2", SqlDbType.Int) { Value = 1805381 },
                                                                     new SqlParameter("@P3", SqlDbType.DateTime2) { Value = d_modified },
                                                                     new SqlParameter("@P4", SqlDbType.DateTime2) { Value = t_modified },
                                                                     new SqlParameter("@P5", SqlDbType.SmallInt) { Value = -1 },
                                                                     new SqlParameter("@P6", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["DelayId"].ToString()) },
                                                                     new SqlParameter("@P7", SqlDbType.DateTime2) { Value = delayDate },                                                                     
                                                                     new SqlParameter("@P8", SqlDbType.DateTime2) { Value = toDelayDate },
                                                                     new SqlParameter("@P9", SqlDbType.NVarChar) { Value = userComments },
                                                                     new SqlParameter("@P10", SqlDbType.NText) { Value = userComments }
                                                                              };
                                                    bool isInsert = conn.executeUpdateQuery(sclearsqlIns, parameterUpd);

                                                    xlWorkSheet.Cells[i + 1, 1] = "Updating Job Number - ";
                                                    xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["JobNumber"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 3] = "Updating Delay :- ";
                                                    xlWorkSheet.Cells[i + 1, 4] = dt.Rows[i]["DelayId"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 5] = "SP Time:- ";
                                                    xlWorkSheet.Cells[i + 1, 6] = Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString()).ToString();
                                                    xlWorkSheet.Cells[i + 1, 5] = "SQL Time:- ";
                                                    xlWorkSheet.Cells[i + 1, 6] = compareSQLDate;

                                                    #endregion
                                                }
                                                else 
                                                {
                                                    Console.WriteLine("Updating Delay dates into SP for DelayId - " + dt.Rows[i]["DelayId"].ToString() + "Job Number" + dt.Rows[i]["JobNumber"].ToString());

                                                    oListItem = oList.GetItemById(dt.Rows[i]["ID"].ToString());

                                                    var delayDateExists = delayJobsSQLData.AsEnumerable().Where(x => x.Field<string>("DelayId") == dt.Rows[i]["DelayId"].ToString()
                                                                                                    && x.Field<DateTime>("DelayDate").ToString("dd/MM/yyyy") ==
                                                                                                            (Convert.ToDateTime(dt.Rows[i]["DelayDate"].ToString())).ToString("dd/MM/yyyy")
                                                                                                        );
                                                    DataTable delayDateExistsdt = null;
                                                    if (delayDateExists.Any())
                                                    { delayDateExistsdt = delayDateExists.CopyToDataTable(); }
                                                    if (delayDateExistsdt != null && delayDateExistsdt.Rows.Count > 0){}
                                                    else
                                                    {
                                                        oListItem["DelayDate"] = Convert.ToDateTime(delayJobsSQLData.Rows[i]["DelayDate"].ToString());
                                                    }


                                                    var toDelayDateExists = delayJobsSQLData.AsEnumerable().Where(x => x.Field<string>("DelayId") == dt.Rows[i]["DelayId"].ToString()
                                                                                                    && x.Field<DateTime>("ToDelayDate").ToString("dd/MM/yyyy") ==
                                                                                                          (Convert.ToDateTime(dt.Rows[i]["ToDelayDate"].ToString())).ToString("dd/MM/yyyy")
                                                                                                        );
                                                    DataTable toDelayDateExistsdt = null;
                                                    if (toDelayDateExists.Any())
                                                    { toDelayDateExistsdt = toDelayDateExists.CopyToDataTable(); }
                                                    if (toDelayDateExistsdt != null && toDelayDateExistsdt.Rows.Count > 0) { }
                                                    else
                                                    {
                                                        oListItem["ToDelayDate"] = Convert.ToDateTime(delayJobsSQLData.Rows[i]["ToDelayDate"].ToString());
                                                    }


                                                    
                                                    var reasonDateExists = delayJobsSQLData.AsEnumerable().Where(x => x.Field<string>("DelayId") == dt.Rows[i]["DelayId"].ToString()
                                                                                                                && x.Field<string>("Reason") == dt.Rows[i]["ReasonValue"].ToString()
                                                                                                        );
                                                    DataTable reasonDateExistsdt = null;
                                                    if (reasonDateExists.Any())
                                                    { reasonDateExistsdt = reasonDateExists.CopyToDataTable(); }
                                                    if (reasonDateExistsdt != null && reasonDateExistsdt.Rows.Count > 0) { }
                                                    else
                                                    {
                                                        oListItem["Reason"] = gl.GetLookupFieldValue(ctx, delayJobsSQLData.Rows[i]["Reason"].ToString(), "DelayReason", "DelayReason", "Text");
                                                        oListItem.Update();
                                                    }




                                                    oListItem.Update();
                                                    ctx.ExecuteQuery();

                                                    xlWorkSheet.Cells[i + 1, 1] = "Update Job Number - ";
                                                    xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["JobNumber"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 3] = "Update DelayId :- ";
                                                    xlWorkSheet.Cells[i + 1, 4] = dt.Rows[i]["DelayId"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 5] = "SP Time:- ";
                                                    xlWorkSheet.Cells[i + 1, 6] = Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString()).ToString();
                                                    xlWorkSheet.Cells[i + 1, 5] = "SQL Time:- ";
                                                    xlWorkSheet.Cells[i + 1, 6] = compareSQLDate;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if( ! String.IsNullOrEmpty(dt.Rows[i]["Id"].ToString()) )
                                            {
                                                //Console.WriteLine("Inserting - " + listDelayName + "-----" + dt.Rows[i]["ID"].ToString());                                                
                                                Console.WriteLine("SP - Inserting Delay List into SQL:- " + listDelayName + " and Id :- " + dt.Rows[i]["DelayToSqlInsertCheckParam"].ToString());

                                                #region New Code
                                                userComments = dt.Rows[i]["Title"].ToString();
                                                delayDate = (Convert.ToDateTime(dt.Rows[i]["DelayDate"].ToString())).ToString("dd'/'MM'/'yyyy");
                                                jobId = dt.Rows[i]["JobId"].ToString();
                                                //jobId = dt.Rows[i]["JobNumber"].ToString();
                                                if (dt.Rows[i]["ToDelayDate"].ToString() == "")
                                                {
                                                    toDelayDate = (Convert.ToDateTime(dt.Rows[i]["DelayDate"].ToString())).ToString("dd'/'MM'/'yyyy"); ; ;
                                                }
                                                else
                                                {
                                                    toDelayDate = (Convert.ToDateTime(dt.Rows[i]["ToDelayDate"].ToString())).ToString("dd'/'MM'/'yyyy"); ;
                                                }

                                                reasonSqlId = Int32.Parse(dt.Rows[i]["ReasonSqlValue"].ToString().Substring(0, dt.Rows[i]["ReasonSqlValue"].ToString().IndexOf('.')));

                                                string sclearsqlIns = "[WatersunData].dbo.sp_createJobDelays";


                                                #region Commented SQL Parameters
                                                //int jobIdValue = Int32.Parse(dt.Rows[i]["JobId"].ToString())          ;
                                                //       jobNumber = dt.Rows[i]["JobNumber"].ToString()               ;
                                                //       int cstIdVale = Int32.Parse(dt.Rows[i]["JobId"].ToString())           ;
                                                //       int SpId= Int32.Parse(dt.Rows[i]["Id"].ToString())               ;
                                                //       l_context_id = 2                                             ;
                                                //       l_cst_dlyClaim_id = 0                                        ;
                                                //       l_cst_pCallDay_id = 0                                        ;
                                                //       l_cst_dlyReas_id= -3                                         ;
                                                //       l_delay_role_gl_id= -478                                     ;
                                                //       l_delay_e_id = -1                                            ;
                                                //       l_delay_ec_id = -1                                           ;
                                                //       f_openClaim = 0                                              ;
                                                //       f_claimable = -1                                             ;
                                                //       l_user_created_id= 1805381                                   ;
                                                //       l_user_modified_id = 1805381                                 ;
                                                //       l_user_owner_id = 0                                          ;
                                                //       f_concurrency = -1                                           ;
                                                //       s_notes_supplier = null                                      ;
                                                //       d_delay= delayDate                                           ;
                                                //       d_to= toDelayDate                                            ;
                                                //       d_claimed= null                                              ;
                                                //       s_notes_derived= dt.Rows[i]["Title"].ToString()              ;
                                                //       m_notes = dt.Rows[i]["Title"].ToString();





                                                //SqlParameter[] parameterUpd1 = { new SqlParameter("@jobId", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["JobId"].ToString()) } };
                                                //SqlParameter[] parameterUpd2 = { new SqlParameter("@jobNumber", SqlDbType.NVarChar) { Value = dt.Rows[i]["JobNumber"].ToString() }, };
                                                //SqlParameter[] parameterUpd3 = { new SqlParameter("@cstId", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["JobId"].ToString()) }, };
                                                //SqlParameter[] parameterUpd4 = { new SqlParameter("@SpId", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["Id"].ToString()) }, };
                                                //SqlParameter[] parameterUpd5 = { new SqlParameter("@l_context_id", SqlDbType.Int) { Value = 2 }, };
                                                //SqlParameter[] parameterUpd6 = { new SqlParameter("@l_cst_dlyClaim_id", SqlDbType.Int) { Value = 0 }, };
                                                //SqlParameter[] parameterUpd7 = { new SqlParameter("@l_cst_pCallDay_id", SqlDbType.Int) { Value = 0 }, };
                                                //SqlParameter[] parameterUpd8 = { new SqlParameter("@l_cst_dlyReas_id", SqlDbType.Int) { Value = -3 }, };
                                                //SqlParameter[] parameterUpd9 = { new SqlParameter("@l_delay_role_gl_id", SqlDbType.Int) { Value = -478 }, };
                                                //SqlParameter[] parameterUpd10 = { new SqlParameter("@l_delay_e_id", SqlDbType.Int) { Value = -1 }, };
                                                //SqlParameter[] parameterUpd11 = { new SqlParameter("@l_delay_ec_id", SqlDbType.Int) { Value = -1 }, };
                                                //SqlParameter[] parameterUpd12 = { new SqlParameter("@f_openClaim", SqlDbType.Int) { Value = 0 }, };
                                                //SqlParameter[] parameterUpd13 = { new SqlParameter("@f_claimable", SqlDbType.Int) { Value = -1 }, };
                                                //SqlParameter[] parameterUpd14 = { new SqlParameter("@l_user_created_id", SqlDbType.Int) { Value = 1805381 }, };
                                                //SqlParameter[] parameterUpd15 = { new SqlParameter("@l_user_modified_id", SqlDbType.Int) { Value = 1805381 }, };
                                                //SqlParameter[] parameterUpd16 = { new SqlParameter("@l_user_owner_id", SqlDbType.Int) { Value = 0 }, };
                                                //SqlParameter[] parameterUpd17 = { new SqlParameter("@f_concurrency", SqlDbType.Int) { Value = -1 }, };
                                                //SqlParameter[] parameterUpd18 = { new SqlParameter("@s_notes_supplier", SqlDbType.NVarChar) { Value = "" }, };
                                                //SqlParameter[] parameterUpd19 = { new SqlParameter("@d_delay", SqlDbType.DateTime2) { Value = delayDate }, };
                                                //SqlParameter[] parameterUpd20 = { new SqlParameter("@d_to", SqlDbType.DateTime2) { Value = toDelayDate }, };
                                                //SqlParameter[] parameterUpd21 = { new SqlParameter("@d_claimed", SqlDbType.Int) { Value = "" }, };
                                                //SqlParameter[] parameterUpd22 = { new SqlParameter("@s_notes_derived", SqlDbType.NVarChar) { Value = dt.Rows[i]["Title"].ToString() }, };
                                                //SqlParameter[] parameterUpd23 = { new SqlParameter("@m_notes", SqlDbType.NVarChar) { Value = dt.Rows[i]["Title"].ToString() } };
                                                #endregion

                                                SqlParameter[] parameterUpd = {
                                                   new SqlParameter("@jobId", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["JobId"].ToString()) },
                                                   new SqlParameter("@jobNumber", SqlDbType.NVarChar) { Value = dt.Rows[i]["JobNumber"].ToString() },
                                                   new SqlParameter("@cstId", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["JobId"].ToString()) },
                                                   new SqlParameter("@SpId", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["Id"].ToString()) },
                                                   new SqlParameter("@l_context_id", SqlDbType.Int) { Value = 2 },
                                                   new SqlParameter("@l_cst_dlyClaim_id", SqlDbType.Int) { Value = 0 },
                                                   new SqlParameter("@l_cst_pCallDay_id", SqlDbType.Int) { Value = 0 },
                                                   new SqlParameter("@l_cst_dlyReas_id", SqlDbType.Int) { Value = reasonSqlId }, ///////////////// -3
                                                   new SqlParameter("@l_delay_role_gl_id", SqlDbType.Int) { Value = -478 },
                                                   new SqlParameter("@l_delay_e_id", SqlDbType.Int) { Value = -1 },
                                                   new SqlParameter("@l_delay_ec_id", SqlDbType.Int) { Value = -1 },
                                                   new SqlParameter("@f_openClaim", SqlDbType.Int) { Value = 0 },
                                                   new SqlParameter("@f_claimable", SqlDbType.Int) { Value = -1 },
                                                   new SqlParameter("@l_user_created_id", SqlDbType.Int) { Value = 1805381 },
                                                   new SqlParameter("@l_user_modified_id", SqlDbType.Int) { Value = 1805381 },
                                                   new SqlParameter("@l_user_owner_id", SqlDbType.Int) { Value = 0 },
                                                   new SqlParameter("@f_concurrency", SqlDbType.Int) { Value = -1 },
                                                   //new SqlParameter("@s_notes_supplier", SqlDbType.NVarChar) { Value = null },
                                                   //new SqlParameter("@s_notes_supplier", SqlDbType.NVarChar) { Value = "test" },
                                                   new SqlParameter("@d_delay", SqlDbType.DateTime2) { Value = delayDate },
                                                   new SqlParameter("@d_to", SqlDbType.DateTime2) { Value = toDelayDate },
                                                   //new SqlParameter("@d_claimed", SqlDbType.Int) { Value = null },
                                                   //new SqlParameter("@d_claimed", SqlDbType.DateTime2) { Value = delayDate },
                                                   new SqlParameter("@s_notes_derived", SqlDbType.NVarChar) { Value = dt.Rows[i]["Title"].ToString() },
                                                   new SqlParameter("@m_notes", SqlDbType.NVarChar) { Value = dt.Rows[i]["Title"].ToString() }
                                                         };

                                                bool isInsert = conn.executeInsertQuerySP(sclearsqlIns, parameterUpd);

                                                sendEmail(ctx, listDelayName, dt.Rows[i]["JobNumber"].ToString());
                                               
                                                xlWorkSheet.Cells[i + 1, 1] = "Updating Job Number - ";
                                                xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["JobNumber"].ToString();
                                                xlWorkSheet.Cells[i + 1, 3] = "Updating Delay :- ";
                                                xlWorkSheet.Cells[i + 1, 4] = dt.Rows[i]["DelayId"].ToString();

                                                #endregion
                                            }
                                          

                                            #region Old Code Date comparison LINQ LAMBDA
                                            ////////toDelayDate = string.IsNullOrEmpty(dt.Rows[i]["ToDelayDate"].ToString()) ? dt.Rows[i]["DelayDate"].ToString() : dt.Rows[i]["ToDelayDate"].ToString();
                                            //////toDelayDate = string.IsNullOrEmpty((Convert.ToDateTime(dt.Rows[i]["ToDelayDate"].ToString())).ToString("dd/MM/yyyy"))
                                            //////                                        ? (Convert.ToDateTime(dt.Rows[i]["ToDelayDate"].ToString())).ToString("dd/MM/yyyy") : (Convert.ToDateTime(dt.Rows[i]["DelayDate"].ToString())).ToString("dd/MM/yyyy");
                                            //toDelayDate = string.IsNullOrEmpty((Convert.ToDateTime(dt.Rows[i]["ToDelayDate"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss"))
                                            //                                        ? (Convert.ToDateTime(dt.Rows[i]["ToDelayDate"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss")
                                            //                                                    : (Convert.ToDateTime(dt.Rows[i]["DelayDate"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss");
                                            #endregion


                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);                                        
                                        GlobalLogic.ExceptionHandle(e, " Sp To Sql Create Jobs Delay Data " + "---"
                                                                                                            + listDelayName + "---" + dt.Rows[i]["Id"].ToString() + "---"
                                                                                                            + " Framework Time Modified :- " + Convert.ToDateTime(delayJobsSQLData.Rows[i]["timeModified"].ToString()) + "-----"
                                                                                                            + " Framework Date Modified :- " + Convert.ToDateTime(delayJobsSQLData.Rows[i]["dateModified"].ToString()) + "-----"
                                                                                                            + " SharePoint Time Modified :- " + dt.Rows[i]["timeModified"].ToString());
                                    }
                                }
                                //xlWorkBook.SaveAs("C:\\Log\\SpToSqlLog_" + listDelayName  +DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.SaveAs(ConfigurationManager.AppSettings.Get("LogLocation") + "\\SpToSqlLog_" + listDelayName + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.Close(true, misValue, misValue);
                                xlApp.Quit();

                                gl.releaseObject(xlWorkSheet);
                                gl.releaseObject(xlWorkBook);
                                gl.releaseObject(xlApp);
                                //}
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                GlobalLogic.ExceptionHandle(e, "Sp To Sql Create Jobs Delay Data" + "---" + listDelayName + "---" + "Create and Update Task list Items");
                            }
                            #endregion
                        }
                        catch (Exception e) { 
                            Console.WriteLine(e.Message);
                            GlobalLogic.ExceptionHandle(e, "Sp To Sql Create Jobs Delay Data" + "---" + "Starting operation on Delay List");
                        }

                        SqlToSpUpdateDelayId2(ctx, dt, tempTable.Rows[j]["JobNum"].ToString(), queryId);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //GlobalLogic gl = new GlobalLogic();
                GlobalLogic.ExceptionHandle(e, "Sp To Sql Create Jobs Delay Data");
            }
            finally
            {
                //dt.Dispose();
            }
        }

        /// <summary>
        /// Update Build program job's task details in SP (SharePoint) or Framework DB, it compares the changes made either in SP or Framework DB.
        /// If the latest changes in a particular record are made in database then writes them to SP or if the latest changes are made in SP then update Framework DB
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="userName"></param>
        /// <param name="passwordString"></param>
        /// <param name="key"></param>
        /// <param name="appSettingsKey"></param>
        /// <param name="columnName"></param>
        /// <param name="tempTable"></param>
        /// <param name="queryJobsDelayItems"></param>
        private static void SpToSqlCreateJobsData(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, DataTable tempTable, string queryJobsDelayItems)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                #region UserValues
                //UserValues: Begin
                List<UserValues> uValue = null;
                try
                {
                    UserCollection user = ctx.Web.SiteUsers;
                    ctx.Load(user);
                    ctx.ExecuteQuery();
                    uValue = new List<UserValues>(10000);
                    foreach (User usr in user)
                    {
                        UserValues uv = new UserValues();
                        uv.Email = usr.Email;
                        uv.Id = usr.Id;
                        uv.LoginName = usr.LoginName;
                        uv.Title = usr.Title;
                        uValue.Add(uv);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //UserValues: End
                #endregion

                if (tempTable != null && tempTable.Rows.Count > 0)
                {
                    for (int j = 0; j < tempTable.Rows.Count; j++)
                    {
                        #region Create Lists
                        try
                        {
                            string listDelayName = tempTable.Rows[j][0].ToString() + "_Data";

                            #region Create and Update Task list Items
                            try
                            {
                                //ListItemCollection getListItemsCol = gl.getListDataVal(ctx, listDelayName);
                                #region Get List Item Collection from SP

                                ListItemCollection getListItemsCol = null;
                                List getList = ctx.Web.Lists.GetByTitle(listDelayName);
                                CamlQuery camlQuery = new CamlQuery();
                                camlQuery.ViewXml = "<View><Query></Query></View>";
                                ListItemCollection getListItemsCollection = getList.GetItems(camlQuery);
                                ctx.Load(getListItemsCollection);
                                ctx.ExecuteQuery();

                                if (getListItemsCollection != null && getListItemsCollection.Count > 0)
                                {
                                    getListItemsCol = getListItemsCollection;
                                }

                                #endregion

                                DataTable dt = new DataTable();

                                //TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");

                                dt.Columns.Add("ID");
                                dt.Columns.Add("CstCallId");
                                dt.Columns.Add("JobNum");
                                dt.Columns.Add("Called");
                                dt.Columns.Add("CalledFor");
                                dt.Columns.Add("Start");
                                dt.Columns.Add("Complete");
                                dt.Columns.Add("SupName");
                                dt.Columns.Add("SupNameId");
                                dt.Columns.Add("SupNameValue");
                                dt.Columns.Add("SupId");
                                dt.Columns.Add("SupIdId");
                                dt.Columns.Add("SupIdValue");
                                dt.Columns.Add("Title");
                                dt.Columns.Add("CC");
                                dt.Columns.Add("Supervisor");
                                dt.Columns.Add("Duration");
                                dt.Columns.Add("timeCreated");
                                dt.Columns.Add("timeModified");

                                if (getListItemsCol != null)
                                {
                                    foreach (ListItem listItemsCol in getListItemsCol)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["ID"] = listItemsCol["ID"];
                                        dr["CstCallId"] = listItemsCol["CstCallId"];
                                        dr["JobNum"] = listItemsCol["JobNum"];

                                        if (listItemsCol["Called"] == null) { dr["Called"] = null; }
                                        else { dr["Called"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Called"].ToString()), cstZone); }

                                        if (listItemsCol["CalledFor"] == null) { dr["CalledFor"] = null; }
                                        else { dr["CalledFor"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["CalledFor"].ToString()), cstZone); }

                                        if (listItemsCol["Start"] == null) { dr["Start"] = null; }
                                        else { dr["Start"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Start"].ToString()), cstZone); }

                                        if (listItemsCol["Complete"] == null) { dr["Complete"] = null; }
                                        else { dr["Complete"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Complete"].ToString()), cstZone); }

                                        if (listItemsCol["Modified"] == null) { dr["timeModified"] = null; }
                                        else { dr["timeModified"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Modified"].ToString()), cstZone); }

                                        if (listItemsCol["Created"] == null) { dr["timeCreated"] = null; }
                                        else { dr["timeCreated"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Created"].ToString()), cstZone); }

                                        var supLkp = listItemsCol["SupName"] as FieldLookupValue;
                                        if (supLkp != null)
                                        {
                                            dr["SupNameId"] = supLkp.LookupId;
                                            dr["SupNameValue"] = supLkp.LookupValue;
                                        }

                                        var supIdLkp = listItemsCol["SupId"] as FieldLookupValue;
                                        if (supIdLkp != null)
                                        {
                                            dr["SupIdId"] = supIdLkp.LookupId;
                                            dr["SupIdValue"] = supIdLkp.LookupValue;
                                            dr["SupIdValue"] = float.Parse(dr["SupIdValue"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                            
                                            //float.Parse(dr["SupIdValue"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                            //(Int32.Parse(dt.Rows[i]["SupIdValue"].ToString().Substring(0, dt.Rows[i]["SupIdValue"].ToString().IndexOf('.')))).ToString();
                                        }

                                        dr["Title"] = listItemsCol["Title"];
                                        dr["CC"] = listItemsCol["CC"];
                                        dr["Supervisor"] = listItemsCol["Supervisor"];
                                        dr["Duration"] = listItemsCol["Duration"];

                                        dt.Rows.Add(dr);
                                    }
                                }
                                else
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["ID"] = "";
                                    dr["CstCallId"] = "";
                                    dr["JobNum"] = "";
                                    dr["Called"] = "";
                                    dr["CalledFor"] = "";
                                    dr["Start"] = "";
                                    dr["Complete"] = "";
                                    dr["SupName"] = "";
                                    dr["SupNameId"] = "";
                                    dr["SupNameValue"] = "";
                                    dr["SupId"] = "";
                                    dr["SupIdId"] = "";
                                    dr["SupIdValue"] = "";
                                    dr["CC"] = "";
                                    dr["Title"] = "";
                                    dr["Supervisor"] = "";
                                    dr["Duration"] = "";
                                    dr["timeCreated"] = "";
                                    dr["timeModified"] = "";

                                    dt.Rows.Add(dr);
                                }

                                //dbConnection conn = new dbConnection();
                                //DataTable tempTable = null;
                                //tempTable = conn.executeSelectNoParameter(sqlQuery);
                                ListItemCreationInformation itemCreateInfo = null;
                                ListItem oListItem = null;
                                List oList = ctx.Web.Lists.GetByTitle(listDelayName);

                                Excel.Application xlApp;
                                Excel.Workbook xlWorkBook;
                                Excel.Worksheet xlWorkSheet;
                                object misValue = System.Reflection.Missing.Value;

                                xlApp = new Excel.Application();
                                xlWorkBook = xlApp.Workbooks.Add(misValue);
                                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                                dbConnection conn = new dbConnection();
                                SqlParameter[] parameter = {                                
                                                                    new SqlParameter("@JobNum", SqlDbType.VarChar) { Value = tempTable.Rows[j][0].ToString() }
                                                               };
                                //DataTable jobsSQLData = conn.executeSelectQuery(queryJobsDelayItems, parameter);
                                DataTable jobsSQLDataWithRows = conn.executeSelectQuery(queryJobsDelayItems, parameter);

                                DataTable jobsSQLData =  new DataTable();;

                                jobsSQLData.Columns.Add("cstCallId");
                                jobsSQLData.Columns.Add("called");
                                jobsSQLData.Columns.Add("calledfor");
                                jobsSQLData.Columns.Add("start");
                                jobsSQLData.Columns.Add("complete");
                                jobsSQLData.Columns.Add("supplier");
                                jobsSQLData.Columns.Add("supplierId");
                                jobsSQLData.Columns.Add("dateModified");
                                jobsSQLData.Columns.Add("timeModified");
                                if (jobsSQLDataWithRows != null)
                                {
                                    for (int i = 0; i < jobsSQLDataWithRows.Rows.Count; i++)
                                    {
                                        DataRow dr = jobsSQLData.NewRow();
                                        dr["cstCallId"] = jobsSQLDataWithRows.Rows[i]["cstCallId"];
                                        dr["supplier"] = jobsSQLDataWithRows.Rows[i]["supplier"];
                                        dr["supplierId"] = jobsSQLDataWithRows.Rows[i]["supplierId"];

                                        if (String.IsNullOrEmpty(jobsSQLDataWithRows.Rows[i]["called"].ToString())) { dr["called"] = ""; }
                                        else { dr["called"] = (Convert.ToDateTime(jobsSQLDataWithRows.Rows[i]["called"].ToString())).ToString("dd/MM/yyyy"); }

                                        if (String.IsNullOrEmpty(jobsSQLDataWithRows.Rows[i]["calledfor"].ToString())) { dr["calledfor"] = ""; }
                                        else { dr["calledfor"] = (Convert.ToDateTime(jobsSQLDataWithRows.Rows[i]["calledfor"].ToString())).ToString("dd/MM/yyyy"); }

                                        if (String.IsNullOrEmpty(jobsSQLDataWithRows.Rows[i]["start"].ToString())) { dr["start"] = ""; }
                                        else { dr["start"] = (Convert.ToDateTime(jobsSQLDataWithRows.Rows[i]["start"].ToString())).ToString("dd/MM/yyyy"); }

                                        if (String.IsNullOrEmpty(jobsSQLDataWithRows.Rows[i]["complete"].ToString())) { dr["complete"] = ""; }
                                        else { dr["complete"] = (Convert.ToDateTime(jobsSQLDataWithRows.Rows[i]["complete"].ToString())).ToString("dd/MM/yyyy"); }

                                        if (String.IsNullOrEmpty(jobsSQLDataWithRows.Rows[i]["dateModified"].ToString())) { dr["dateModified"] = ""; }
                                        else { dr["dateModified"] = (Convert.ToDateTime(jobsSQLDataWithRows.Rows[i]["dateModified"].ToString())).ToString(); }

                                        if (String.IsNullOrEmpty(jobsSQLDataWithRows.Rows[i]["timeModified"].ToString())) { dr["timeModified"] = ""; }
                                        else { dr["timeModified"] = (Convert.ToDateTime(jobsSQLDataWithRows.Rows[i]["timeModified"].ToString())).ToString(); }

                                        jobsSQLData.Rows.Add(dr);
                                    }
                                }
                                else
                                {
                                    DataRow dr = jobsSQLData.NewRow();
                                    dr["cstCallId"] = "";
                                    dr["supplier"] = "";
                                    dr["supplierId"] = "";
                                    dr["called"] = "";
                                    dr["calledfor"] = "";
                                    dr["start"] = "";
                                    dr["complete"] = "";
                                    dr["dateModified"] = "";
                                    dr["timeModified"] = ""; 

                                    jobsSQLData.Rows.Add(dr);
                                }

                                string called, calledFor, start, complete, cc, supplier, activity;
                                int cstCallId, supId;
                                string c = "";
                                string cf = "";
                                string s = "";
                                string cm = "";
                                string dM = "";
                                string dC = "";

                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    try
                                    {
                                        DataRow[] drExists = jobsSQLData.Select("CstCallId = '" + dt.Rows[i]["CstCallId"].ToString() + "'");
                                        if (drExists != null && drExists.Length > 0)
                                        {
                                            #region working code

                                            //if (string.IsNullOrEmpty(dt.Rows[i]["called"].ToString())) { c = ""; }
                                            //else { c = (Convert.ToDateTime(dt.Rows[i]["called"].ToString())).ToString("dd/MM/yyyy"); }

                                            //if (string.IsNullOrEmpty(dt.Rows[i]["calledFor"].ToString())) { cf = ""; }
                                            //else { cf = (Convert.ToDateTime(dt.Rows[i]["calledFor"].ToString())).ToString("dd/MM/yyyy"); }

                                            //if (string.IsNullOrEmpty(dt.Rows[i]["start"].ToString())) { s = ""; }
                                            //else { s = (Convert.ToDateTime(dt.Rows[i]["start"].ToString())).ToString("dd/MM/yyyy"); }
                                            
                                            //if (string.IsNullOrEmpty(dt.Rows[i]["complete"].ToString())) { cm = ""; }
                                            //else { cm = (Convert.ToDateTime(dt.Rows[i]["complete"].ToString())).ToString("dd/MM/yyyy"); }


                                            //var rowExists = jobsSQLData.AsEnumerable().Where(x => x.Field<string>("cstCallId") == dt.Rows[i]["CstCallId"].ToString()
                                            //    //&& x.Field<string>("Activity") == dt.Rows[i]["Title"].ToString()
                                            //    //&& x.Field<string>("s_costCentreCode") == dt.Rows[i]["CC"].ToString()

                                            //                                                && x.Field<string>("Supplier") == dt.Rows[i]["SupNameValue"].ToString()
                                            //                                                && x.Field<string>("Called") ==
                                            //                                                        c//(Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("dd/MM/yyyy")
                                            //                                                && x.Field<string>("CalledFor") ==
                                            //                                                        cf//(Convert.ToDateTime(dt.Rows[i]["CalledFor"].ToString())).ToString("dd/MM/yyyy")
                                            //                                                && x.Field<string>("Start") ==
                                            //                                                        s//(Convert.ToDateTime(dt.Rows[i]["Start"].ToString())).ToString("dd/MM/yyyy")
                                            //                                                && x.Field<string>("Complete") ==
                                            //                                                        cm//(Convert.ToDateTime(dt.Rows[i]["Complete"].ToString())).ToString("dd/MM/yyyy")
                                            //                                             );

                                            //#region commected linq
                                            ////var rowExists = jobsSQLData.AsEnumerable().Where(x => x.Field<string>("CstCallId") == dt.Rows[i]["CstCallId"].ToString()
                                            ////    //&& x.Field<string>("Activity") == dt.Rows[i]["Title"].ToString()
                                            ////    //&& x.Field<string>("s_costCentreCode") == dt.Rows[i]["CC"].ToString()

                                            ////                                                && x.Field<string>("Supplier") == dt.Rows[i]["SupNameValue"].ToString()
                                            ////                                                && x.Field<DateTime>("Called").ToString("dd/MM/yyyy") ==
                                            ////                                                        (Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("dd/MM/yyyy")
                                            ////                                                && x.Field<DateTime>("CalledFor").ToString("dd/MM/yyyy") ==
                                            ////                                                        (Convert.ToDateTime(dt.Rows[i]["CalledFor"].ToString())).ToString("dd/MM/yyyy")
                                            ////                                                && x.Field<DateTime>("Start").ToString("dd/MM/yyyy") ==
                                            ////                                                        (Convert.ToDateTime(dt.Rows[i]["Start"].ToString())).ToString("dd/MM/yyyy")
                                            ////                                                && x.Field<DateTime>("Complete").ToString("dd/MM/yyyy") ==
                                            ////                                                        (Convert.ToDateTime(dt.Rows[i]["Complete"].ToString())).ToString("dd/MM/yyyy")
                                            ////                                             );
                                            //#endregion

                                            //DataTable drExists1 = null;
                                            //if (rowExists.Any())
                                            //{ drExists1 = rowExists.CopyToDataTable(); }

                                            //if (drExists1 != null && drExists1.Rows.Count > 0)
                                            //{
                                            //    Console.WriteLine("Found - " + dt.Rows[i]["CstCallId"].ToString());
                                            //    xlWorkSheet.Cells[i + 1, 1] = "Found Job Number - ";
                                            //    xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["CstCallId"].ToString();
                                            //}
                                            //else
                                            //{
                                            //    Console.WriteLine("Updating - " + dt.Rows[i]["CstCallId"].ToString());
                                            //    xlWorkSheet.Cells[i + 1, 1] = "Updating - ";
                                            //    xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["CstCallId"].ToString();

                                            //    //////cstCallId = Int32.Parse(dt.Rows[i]["CstCallId"].ToString());
                                            //    //////called = (Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss");
                                            //    //////calledFor = (Convert.ToDateTime(dt.Rows[i]["CalledFor"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss");
                                            //    //////start = (Convert.ToDateTime(dt.Rows[i]["Start"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss");
                                            //    //////complete = (Convert.ToDateTime(dt.Rows[i]["Complete"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss");

                                            //    cstCallId = Int32.Parse(dt.Rows[i]["CstCallId"].ToString());
                                            //    //called = (Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("dd/MM/yyyy");
                                            //    //calledFor = (Convert.ToDateTime(dt.Rows[i]["CalledFor"].ToString())).ToString("dd/MM/yyyy");
                                            //    //start = (Convert.ToDateTime(dt.Rows[i]["Start"].ToString())).ToString("dd/MM/yyyy");
                                            //    //complete = (Convert.ToDateTime(dt.Rows[i]["Complete"].ToString())).ToString("dd/MM/yyyy");

                                            //    //if (c == "") { called = SqlDateTime.Null; } else { called = c; }
                                            //    called = c; 
                                            //    calledFor = cf;
                                            //    start = s;
                                            //    complete = cm; 

                                            //    cc = dt.Rows[i]["CC"].ToString();
                                            //    supId = Int32.Parse(dt.Rows[i]["SupIdValue"].ToString().Substring(0, dt.Rows[i]["SupIdValue"].ToString().IndexOf('.')));
                                            //    supplier = dt.Rows[i]["SupNameValue"].ToString();
                                            //    activity = dt.Rows[i]["Title"].ToString();

                                            //    //System.DateTime.ParseExact(dt.Rows[i]["ToDelayDate"].ToString(), "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                            //    DateTime d_modified = Convert.ToDateTime(DateTime.Now.ToString("dd'/'MM'/'yyyy"));
                                            //    DateTime t_modified = DateTime.Now;

                                            //    string sclearsqlIns = string.Concat("exec [WatersunData].[dbo].[sp_UpdateJobsDateNdSup] "
                                            //        //+"8632,475546,'5921',@datee,@datee,@datee,@datee");
                                            //                                        + "@supId,@cstCallId,@jobNo,@called,@calledFor,@start,@complete");

                                            //    SqlParameter[] parameterUpd = {
                                            //        new SqlParameter("@supId", SqlDbType.Int) { Value = supId },
                                            //        new SqlParameter("@cstCallId", SqlDbType.Int) { Value = cstCallId },
                                            //        new SqlParameter("@jobNo", SqlDbType.VarChar) { Value = dt.Rows[i]["JobNum"].ToString() },
                                            //        new SqlParameter("@called", SqlDbType.DateTime) { Value = (called ==""?null:"") },
                                            //        new SqlParameter("@calledFor", SqlDbType.DateTime) { Value = (calledFor ==""?null:"") },
                                            //        new SqlParameter("@start", SqlDbType.DateTime) { Value = (start ==""?null:"") },
                                            //        new SqlParameter("@complete", SqlDbType.DateTime) { Value = (complete ==""?null:"") }
                                            //             };

                                            //    if (called == "")
                                            //    {
                                            //        parameterUpd[3].Value = DBNull.Value;
                                            //    }
                                            //    else
                                            //    {
                                            //        parameterUpd[3].Value = called;
                                            //    }

                                            //    if (calledFor == "")
                                            //    {
                                            //        parameterUpd[4].Value = DBNull.Value;
                                            //    }
                                            //    else
                                            //    {
                                            //        parameterUpd[4].Value = calledFor;
                                            //    }

                                            //    if (start == "")
                                            //    {
                                            //        parameterUpd[5].Value = DBNull.Value;
                                            //    }
                                            //    else
                                            //    {
                                            //        parameterUpd[5].Value = start;
                                            //    }

                                            //    if (complete == "")
                                            //    {
                                            //        parameterUpd[6].Value = DBNull.Value;
                                            //    }
                                            //    else
                                            //    {
                                            //        parameterUpd[6].Value = complete;
                                            //    }

                                            //    bool isInsert = conn.executeUpdateQuery(sclearsqlIns, parameterUpd);

                                            //    //oListItem.Update();
                                            //    //ctx.ExecuteQuery();
                                            //}
                                            #endregion

                                            if (string.IsNullOrEmpty(dt.Rows[i]["called"].ToString())) { c = ""; }
                                            else { c = (Convert.ToDateTime(dt.Rows[i]["called"].ToString())).ToString("dd/MM/yyyy"); }

                                            if (string.IsNullOrEmpty(dt.Rows[i]["calledFor"].ToString())) { cf = ""; }
                                            else { cf = (Convert.ToDateTime(dt.Rows[i]["calledFor"].ToString())).ToString("dd/MM/yyyy"); }

                                            if (string.IsNullOrEmpty(dt.Rows[i]["start"].ToString())) { s = ""; }
                                            else { s = (Convert.ToDateTime(dt.Rows[i]["start"].ToString())).ToString("dd/MM/yyyy"); }

                                            if (string.IsNullOrEmpty(dt.Rows[i]["complete"].ToString())) { cm = ""; }
                                            else { cm = (Convert.ToDateTime(dt.Rows[i]["complete"].ToString())).ToString("dd/MM/yyyy"); }

                                            if (string.IsNullOrEmpty(dt.Rows[i]["timeModified"].ToString())) { dM = ""; }
                                            else { dM = (Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString())).ToString(); }

                                            if (string.IsNullOrEmpty(dt.Rows[i]["timeCreated"].ToString())) { dC = ""; }
                                            else { dC = (Convert.ToDateTime(dt.Rows[i]["timeCreated"].ToString())).ToString(); }

                                            #region Check if exists of not

                                            //var rowExists = jobsSQLData.AsEnumerable().Where(x => x.Field<string>("cstCallId") == dt.Rows[i]["CstCallId"].ToString()
                                            //                                                && x.Field<string>("Supplier") == dt.Rows[i]["SupNameValue"].ToString()
                                            //                                                && x.Field<string>("Called") ==
                                            //                                                        c//(Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("dd/MM/yyyy")
                                            //                                                && x.Field<string>("CalledFor") ==
                                            //                                                        cf//(Convert.ToDateTime(dt.Rows[i]["CalledFor"].ToString())).ToString("dd/MM/yyyy")
                                            //                                                && x.Field<string>("Start") ==
                                            //                                                        s//(Convert.ToDateTime(dt.Rows[i]["Start"].ToString())).ToString("dd/MM/yyyy")
                                            //                                                && x.Field<string>("Complete") ==
                                            //                                                        cm//(Convert.ToDateTime(dt.Rows[i]["Complete"].ToString())).ToString("dd/MM/yyyy")
                                            //                                             );

                                            var rowExists = jobsSQLData.AsEnumerable().Where(x => x.Field<string>("cstCallId") == dt.Rows[i]["CstCallId"].ToString()
                                                //&& x.Field<string>("supplierId") == (Int32.Parse(dt.Rows[i]["SupIdValue"].ToString().Substring(0, dt.Rows[i]["SupIdValue"].ToString().IndexOf('.')))).ToString()
                                                && x.Field<string>("supplierId") == dt.Rows[i]["SupIdValue"].ToString()
                                                //&& x.Field<string>("Supplier") == dt.Rows[i]["SupNameValue"].ToString()
                                                && x.Field<string>("Called") ==
                                                        c  //(Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("dd/MM/yyyy")
                                                && x.Field<string>("CalledFor") ==
                                                        cf  //(Convert.ToDateTime(dt.Rows[i]["CalledFor"].ToString())).ToString("dd/MM/yyyy")
                                                && x.Field<string>("Start") ==
                                                        s  //(Convert.ToDateTime(dt.Rows[i]["Start"].ToString())).ToString("dd/MM/yyyy")
                                                && x.Field<string>("Complete") ==
                                                        cm  //(Convert.ToDateTime(dt.Rows[i]["Complete"].ToString())).ToString("dd/MM/yyyy")
                                             );

                                            
                                            //if (jobsSQLData.Rows[i]["supplierId"].ToString() == (Int32.Parse(dt.Rows[i]["SupIdValue"].ToString().Substring(0, dt.Rows[i]["SupIdValue"].ToString().IndexOf('.')))).ToString()) 
                                            if (jobsSQLData.Rows[i]["supplierId"].ToString() == dt.Rows[i]["SupIdValue"].ToString())
                                            { 
                                                Console.WriteLine("Matched Supplier"); 
                                            }
                                            //if (jobsSQLData.Rows[i]["Supplier"].ToString() == dt.Rows[i]["SupNameValue"].ToString()) { Console.WriteLine("Matched Supplier"); }
                                            if (jobsSQLData.Rows[i]["Called"].ToString() == c) { Console.WriteLine("Matched Called"); }
                                            if (jobsSQLData.Rows[i]["CalledFor"].ToString() == cf) { Console.WriteLine("Matched CalledFor"); }
                                            if (jobsSQLData.Rows[i]["Start"].ToString() == s) { Console.WriteLine("Matched Start"); }
                                            if (jobsSQLData.Rows[i]["Complete"].ToString() == cm) { Console.WriteLine("Matched Complete"); }

                                            #region check if the columns have changed
                                            //Sup
                                            int existsSup = 0;
                                            var rowExistsSup = jobsSQLData.AsEnumerable().Where(x => x.Field<string>("cstCallId") == dt.Rows[i]["CstCallId"].ToString()
                                                                    //&& x.Field<string>("Supplier") == dt.Rows[i]["SupNameValue"].ToString()
                                                                    && x.Field<string>("supplierId") == dt.Rows[i]["SupIdValue"].ToString()
                                             );
                                            DataTable drExistsSup = null;
                                            if (rowExistsSup.Any())
                                            { drExistsSup = rowExistsSup.CopyToDataTable(); }
                                            if (drExistsSup != null && drExistsSup.Rows.Count > 0)
                                            { existsSup = 1; }

                                            //Called
                                            int existsCalled = 0;
                                            var rowExistsCalled = jobsSQLData.AsEnumerable().Where(x => x.Field<string>("cstCallId") == dt.Rows[i]["CstCallId"].ToString()
                                                                    && x.Field<string>("Called") == c
                                             );
                                            DataTable drExistsCalled = null;
                                            if (rowExistsCalled.Any())
                                            { drExistsCalled = rowExistsCalled.CopyToDataTable(); }
                                            if (drExistsCalled != null && drExistsCalled.Rows.Count > 0)
                                            { existsCalled = 1; }

                                            //CalledFor
                                            int existsCalledFor = 0;
                                            var rowExistsCalledFor = jobsSQLData.AsEnumerable().Where(x => x.Field<string>("cstCallId") == dt.Rows[i]["CstCallId"].ToString()
                                                                    && x.Field<string>("CalledFor") == cf
                                             );
                                            DataTable drExistsCalledFor = null;
                                            if (rowExistsCalledFor.Any())
                                            { drExistsCalledFor = rowExistsCalledFor.CopyToDataTable(); }
                                            if (drExistsCalledFor != null && drExistsCalledFor.Rows.Count > 0)
                                            { existsCalledFor = 1; }

                                            //Start
                                            int existsStart = 0;
                                            var rowExistsStart = jobsSQLData.AsEnumerable().Where(x => x.Field<string>("cstCallId") == dt.Rows[i]["CstCallId"].ToString()
                                                                    && x.Field<string>("Start") == s
                                             );
                                            DataTable drExistsStart = null;
                                            if (rowExistsStart.Any())
                                            { drExistsStart = rowExistsStart.CopyToDataTable(); }
                                            if (drExistsStart != null && drExistsStart.Rows.Count > 0)
                                            { existsStart = 1; }

                                            //Complete
                                            int existsComplete = 0;
                                            var rowExistsComplete = jobsSQLData.AsEnumerable().Where(x => x.Field<string>("cstCallId") == dt.Rows[i]["CstCallId"].ToString()
                                                                    && x.Field<string>("Complete") == cm
                                             );
                                            DataTable drExistsComplete = null;
                                            if (rowExistsComplete.Any())
                                            { drExistsComplete = rowExistsComplete.CopyToDataTable(); }
                                            if (drExistsComplete != null && drExistsComplete.Rows.Count > 0)
                                            { existsComplete = 1; }                                                                                        

                                            //Compare 4 Dates and Supplier
                                            DataTable drExists1 = null;
                                            if (rowExists.Any())
                                            { drExists1 = rowExists.CopyToDataTable(); }

                                            #endregion

                                            if (drExists1 != null && drExists1.Rows.Count > 0)
                                            {
                                                Console.WriteLine("Found Jobs Data - " + listDelayName + "------" + dt.Rows[i]["CstCallId"].ToString());
                                                //////////////xlWorkSheet.Cells[i + 1, 1] = "Found Job Number - ";
                                                //////////////xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["JobNum"].ToString();
                                                //////////////xlWorkSheet.Cells[i + 1, 3] = "Found Delay :- ";
                                                //////////////xlWorkSheet.Cells[i + 1, 4] = dt.Rows[i]["CstCallId"].ToString();
                                            }
                                            else
                                            {
                                                DateTime compareSQLDate;
                                                if (Convert.ToDateTime(jobsSQLData.Rows[i]["timeModified"].ToString()) > Convert.ToDateTime(jobsSQLData.Rows[i]["dateModified"].ToString()))
                                                {
                                                    compareSQLDate = Convert.ToDateTime(jobsSQLData.Rows[i]["timeModified"].ToString());
                                                }
                                                else
                                                {
                                                    compareSQLDate = Convert.ToDateTime(jobsSQLData.Rows[i]["dateModified"].ToString());
                                                }

                                                if (Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString()) > compareSQLDate)
                                                //if (Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString()) > Convert.ToDateTime(jobsSQLData.Rows[i]["timeModified"].ToString()))
                                                {
                                                    #region Update SP to SQL
                                                    //Console.WriteLine("Updating Jobs Data - " + listDelayName + "------" + dt.Rows[i]["Cst - Call - Id"].ToString());
                                                    Console.WriteLine("Updating Jobs Data - " + listDelayName + "------" + dt.Rows[i][ConfigurationManager.AppSettings.Get("JobsToSqlCheckParam")].ToString());

                                                    cstCallId = Int32.Parse(dt.Rows[i]["CstCallId"].ToString());
                                                    called = c;
                                                    calledFor = cf;
                                                    start = s;
                                                    complete = cm;

                                                    //5468
                                                    //459998
                                                    //Vaibhav Hayden
                                                    if ((c == "" && cm != "") || (cf == "" && cm != "") || (s == "" && cm != "")) 
                                                    {
                                                        Console.WriteLine("complete date can not be entered");
                                                    }

                                                    cc = dt.Rows[i]["CC"].ToString();
                                                    //supId = Int32.Parse(dt.Rows[i]["SupIdValue"].ToString().Substring(0, dt.Rows[i]["SupIdValue"].ToString().IndexOf('.')));
                                                    supId = Int32.Parse(dt.Rows[i]["SupIdValue"].ToString());
                                                    supplier = dt.Rows[i]["SupNameValue"].ToString();
                                                    activity = dt.Rows[i]["Title"].ToString();

                                                    //System.DateTime.ParseExact(dt.Rows[i]["ToDelayDate"].ToString(), "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                    DateTime d_modified = Convert.ToDateTime(DateTime.Now.ToString("dd'/'MM'/'yyyy"));
                                                    DateTime t_modified = DateTime.Now;

                                                    string sclearsqlIns = string.Concat("exec [WatersunData].[dbo].[sp_UpdateJobsDateNdSup] "
                                                                                        + "@supId,@cstCallId,@jobNo,@called,@calledFor,@start,@complete,@existsSup,@existsCalled,@existsCalledFor,@existsStart,@existsComplete");

                                                    SqlParameter[] parameterUpd = {
                                                    new SqlParameter("@supId", SqlDbType.Int) { Value = supId },
                                                    new SqlParameter("@cstCallId", SqlDbType.Int) { Value = cstCallId },
                                                    new SqlParameter("@jobNo", SqlDbType.VarChar) { Value = dt.Rows[i]["JobNum"].ToString() },
                                                    new SqlParameter("@called", SqlDbType.DateTime) { Value = (called ==""?null:"") },
                                                    new SqlParameter("@calledFor", SqlDbType.DateTime) { Value = (calledFor ==""?null:"") },
                                                    new SqlParameter("@start", SqlDbType.DateTime) { Value = (start ==""?null:"") },
                                                    new SqlParameter("@complete", SqlDbType.DateTime) { Value = (complete ==""?null:"") },
                                                    
                                                    new SqlParameter("@existsSup", SqlDbType.Int) { Value = existsSup },
                                                    new SqlParameter("@existsCalled", SqlDbType.Int) { Value = existsCalled },
                                                    new SqlParameter("@existsCalledFor", SqlDbType.Int) { Value = existsCalledFor },
                                                    new SqlParameter("@existsStart", SqlDbType.Int) { Value = existsStart },
                                                    new SqlParameter("@existsComplete", SqlDbType.Int) { Value = existsComplete }
                                                         };

                                                    if (called == "")
                                                    {
                                                        parameterUpd[3].Value = DBNull.Value;
                                                    }
                                                    else
                                                    {
                                                        parameterUpd[3].Value = called;
                                                    }

                                                    if (calledFor == "")
                                                    {
                                                        parameterUpd[4].Value = DBNull.Value;
                                                    }
                                                    else
                                                    {
                                                        parameterUpd[4].Value = calledFor;
                                                    }

                                                    if (start == "")
                                                    {
                                                        parameterUpd[5].Value = DBNull.Value;
                                                    }
                                                    else
                                                    {
                                                        parameterUpd[5].Value = start;
                                                    }

                                                    if (complete == "")
                                                    {
                                                        parameterUpd[6].Value = DBNull.Value;
                                                    }
                                                    else
                                                    {
                                                        parameterUpd[6].Value = complete;
                                                    }

                                                    bool isInsert = conn.executeUpdateQuery(sclearsqlIns, parameterUpd);

                                                    #region Update 100 Percent field
                                                    if (existsComplete == 0)
                                                    {
                                                        Console.WriteLine("updating 100 percent filed for :- " + dt.Rows[i]["CstCallId"].ToString());
                                                        oListItem = oList.GetItemById(dt.Rows[i]["ID"].ToString());
                                                        //oListItem["Checkmark"] = 1;
                                                        oListItem["PercentComplete"] = 1;
                                                        oListItem.Update();
                                                        ctx.ExecuteQuery();
                                                    }
                                                    #endregion

                                                    xlWorkSheet.Cells[i + 1, 1] = "Update Job Number - ";
                                                    xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["JobNum"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 3] = "Update CstCallId :- ";
                                                    xlWorkSheet.Cells[i + 1, 4] = dt.Rows[i]["CstCallId"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 5] = "Update Call Forwards SP to SQL";
                                                    xlWorkSheet.Cells[i + 1, 6] = "SP Time:- ";
                                                    xlWorkSheet.Cells[i + 1, 7] = Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString()).ToString();
                                                    xlWorkSheet.Cells[i + 1, 8] = "SQL Time:- ";
                                                    xlWorkSheet.Cells[i + 1, 9] = compareSQLDate;
                                                    #endregion
                                                }
                                                else 
                                                {
                                                    Console.WriteLine("Updating Best Possible dates and Supplier into SP for CstCallId - " + dt.Rows[i]["CstCallId"].ToString() + " Job Number " + dt.Rows[i]["JobNum"].ToString());

                                                    //oListItem = oList.GetItemById(drExists[0]["ID"].ToString());
                                                    oListItem = oList.GetItemById(dt.Rows[i]["ID"].ToString());
                                                    if (existsSup == 0)
                                                    {
                                                        oListItem["SupName"] = gl.GetLookupFieldValueId(ctx, jobsSQLData.Rows[i]["Supplier"].ToString(), "SuppliersList", "SupName", "Text", jobsSQLData.Rows[i]["SupplierId"].ToString(), "SupId", "Number", gl, listDelayName);
                                                    }
                                                    if (existsCalled == 0)
                                                    {
                                                        if (string.IsNullOrEmpty(jobsSQLData.Rows[i]["called"].ToString())) { oListItem["Called"] = null; }
                                                        else { oListItem["Called"] = Convert.ToDateTime(jobsSQLData.Rows[i]["called"].ToString()); }

                                                        //else { oListItem["Called"] = (Convert.ToDateTime(jobsSQLData.Rows[i]["called"].ToString())).ToString("dd/MM/yyyy"); }
                                                        
                                                        //oListItem["Called"] = Convert.ToDateTime(jobsSQLData.Rows[i]["called"].ToString());
                                                    }
                                                    if (existsCalledFor == 0)
                                                    {
                                                        if (string.IsNullOrEmpty(jobsSQLData.Rows[i]["calledfor"].ToString())) { oListItem["CalledFor"] = null; }
                                                        else { oListItem["CalledFor"] = Convert.ToDateTime(jobsSQLData.Rows[i]["calledfor"].ToString()); }
                                                                                                                                                                       
                                                        //else { oListItem["CalledFor"] = (Convert.ToDateTime(jobsSQLData.Rows[i]["calledfor"].ToString())).ToString("dd/MM/yyyy"); }

                                                        //oListItem["CalledFor"] = Convert.ToDateTime(jobsSQLData.Rows[i]["calledfor"].ToString());
                                                    }
                                                    if (existsStart == 0)
                                                    {
                                                        if (string.IsNullOrEmpty(jobsSQLData.Rows[i]["start"].ToString())) { oListItem["Start"] = null; }
                                                        else { oListItem["Start"] = Convert.ToDateTime(jobsSQLData.Rows[i]["start"].ToString()); }
                                                        
                                                        //else { oListItem["Start"] = (Convert.ToDateTime(jobsSQLData.Rows[i]["start"].ToString())).ToString("dd/MM/yyyy"); }

                                                        //oListItem["Start"] = Convert.ToDateTime(jobsSQLData.Rows[i]["start"].ToString());
                                                    }
                                                    if (existsComplete == 0)
                                                    {
                                                        if (string.IsNullOrEmpty(jobsSQLData.Rows[i]["complete"].ToString())) { oListItem["Complete"] = null; }
                                                        else { oListItem["Complete"] = Convert.ToDateTime(jobsSQLData.Rows[i]["complete"].ToString()); }
                                                        
                                                        //else { oListItem["Complete"] = (Convert.ToDateTime(jobsSQLData.Rows[i]["complete"].ToString())).ToString("dd/MM/yyyy"); }

                                                        //oListItem["Complete"] = Convert.ToDateTime(jobsSQLData.Rows[i]["complete"].ToString());

                                                        #region Update 100 Percent field

                                                            //Console.WriteLine("updating 100 percent filed for :- " + dt.Rows[i]["CstCallId"].ToString());
                                                            //oListItem = oList.GetItemById(dt.Rows[i]["ID"].ToString());
                                                            ////oListItem["Checkmark"] = 1;
                                                            //oListItem["PercentComplete"] = 1;
                                                            //oListItem.Update();
                                                            //ctx.ExecuteQuery();

                                                        #endregion
                                                    }
                                                    oListItem.Update();
                                                    ctx.ExecuteQuery();

                                                    if (existsComplete == 0)
                                                    {
                                                        #region Update 100 Percent field
                                                        //if (existsComplete == 0)
                                                        //{
                                                        Console.WriteLine("updating 100 percent filed for :- " + dt.Rows[i]["CstCallId"].ToString());
                                                        oListItem = oList.GetItemById(dt.Rows[i]["ID"].ToString());
                                                        //oListItem["Checkmark"] = 1;
                                                        oListItem["PercentComplete"] = 1;
                                                        oListItem.Update();
                                                        ctx.ExecuteQuery();
                                                        //}
                                                        #endregion
                                                    }

                                                    xlWorkSheet.Cells[i + 1, 1] = "Update Job Number - ";
                                                    xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["JobNum"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 3] = "Update CstCallId :- ";
                                                    xlWorkSheet.Cells[i + 1, 4] = dt.Rows[i]["CstCallId"].ToString();
                                                    xlWorkSheet.Cells[i + 1, 5] = "Update Call Forwards SQL to SP";
                                                    xlWorkSheet.Cells[i + 1, 6] = "SP Time:- ";
                                                    xlWorkSheet.Cells[i + 1, 7] = Convert.ToDateTime(dt.Rows[i]["timeModified"].ToString()).ToString();
                                                    xlWorkSheet.Cells[i + 1, 8] = "SQL Time:- ";
                                                    xlWorkSheet.Cells[i + 1, 9] = compareSQLDate;
                                                }
                                            }
                                            #endregion

                                        }                                            
                                        else
                                        {
                                            if (!String.IsNullOrEmpty(dt.Rows[i]["Id"].ToString()))
                                            {
                                                Console.WriteLine("Inserting Jobs Data - " + listDelayName + "------" + dt.Rows[i]["CstCallId"].ToString());

                                                #region Insert Job Items commented
                                                //called = (Convert.ToDateTime(dt.Rows[i]["Called"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss");
                                                //calledFor = (Convert.ToDateTime(dt.Rows[i]["CalledFor"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss");
                                                //start = (Convert.ToDateTime(dt.Rows[i]["Start"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss");
                                                //complete = (Convert.ToDateTime(dt.Rows[i]["Complete"].ToString())).ToString("dd'/'MM'/'yyyy HH:mm:ss");
                                                //supId = Int32.Parse(dt.Rows[i]["SupIdValue"].ToString().Substring(0, dt.Rows[i]["SupIdValue"].ToString().IndexOf('.')));
                                                //supplier = dt.Rows[i]["SupNameValue"].ToString();
                                                //activity = dt.Rows[i]["Title"].ToString();
                                                //cc = dt.Rows[i]["CC"].ToString();

                                                //string sclearsqlIns = "[WatersunData].dbo.sp_createJobDelays";

                                                //SqlParameter[] parameterUpd = {
                                                //       new SqlParameter("@jobId", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["JobId"].ToString()) },
                                                //       new SqlParameter("@jobNumber", SqlDbType.NVarChar) { Value = dt.Rows[i]["JobNumber"].ToString() },
                                                //       new SqlParameter("@cstId", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["JobId"].ToString()) },
                                                //       new SqlParameter("@SpId", SqlDbType.Int) { Value = Int32.Parse(dt.Rows[i]["Id"].ToString()) },
                                                //       new SqlParameter("@cc", SqlDbType.NVarChar) { Value = cc },
                                                //       new SqlParameter("@activity", SqlDbType.NVarChar) { Value = activity },
                                                //       new SqlParameter("@userId", SqlDbType.Int) { Value = 1805381 },
                                                //       new SqlParameter("@supId", SqlDbType.Int) { Value = supId },
                                                //       new SqlParameter("@dateToday", SqlDbType.Int) { Value = DateTime.Today },
                                                //       new SqlParameter("@dateTimeTod", SqlDbType.Int) { Value = DateTime.Now },
                                                //       new SqlParameter("@called", SqlDbType.DateTime) { Value = called },
                                                //       new SqlParameter("@called", SqlDbType.DateTime) { Value = called },
                                                //       new SqlParameter("@called", SqlDbType.DateTime) { Value = called },
                                                //        new SqlParameter("@calledFor", SqlDbType.DateTime) { Value = calledFor },
                                                //        new SqlParameter("@start", SqlDbType.DateTime) { Value = start },
                                                //        new SqlParameter("@complete", SqlDbType.DateTime) { Value = complete },
                                                //             };

                                                //bool isInsert = conn.executeInsertQuerySP(sclearsqlIns, parameterUpd);


                                                //EXEC [WatersunData].dbo.sp_createJobDates @jobid1, @jobNumber1,@cstId1,@SpId1,'0535','Paving',1805381,3921,@dateToday,@dateTimeTod

                                                #endregion

                                                //xlWorkSheet.Cells[i + 1, 1] = "Inserting - ";
                                                //xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["CstCallId"].ToString();
                                                //xlWorkSheet.Cells[i + 1, 2] = listDelayName;
                                                xlWorkSheet.Cells[i + 1, 1] = "Insert Job Number - ";
                                                xlWorkSheet.Cells[i + 1, 2] = dt.Rows[i]["JobNum"].ToString();
                                                xlWorkSheet.Cells[i + 1, 3] = "Insert SP Id:- ";
                                                xlWorkSheet.Cells[i + 1, 4] = dt.Rows[i]["ID"].ToString();
                                                xlWorkSheet.Cells[i + 1, 5] = "Insert Job :- ";
                                                xlWorkSheet.Cells[i + 1, 6] = dt.Rows[i]["ID"].ToString();
                                            }
                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                        GlobalLogic.ExceptionHandle(e, "Sp To Sql Update Jobs Data" + "---"
                                                                                                            + listDelayName + "---" + dt.Rows[i]["Id"].ToString() + "---" + dt.Rows[i]["CstCallId"].ToString() + "---"
                                                                                                            + " Framework Time Modified :- " + Convert.ToDateTime(jobsSQLData.Rows[i]["timeModified"].ToString()) + "-----"
                                                                                                            + " Framework Date Modified :- " + Convert.ToDateTime(jobsSQLData.Rows[i]["dateModified"].ToString()) + "-----"
                                                                                                            + " SharePoint Time Modified :- " + dt.Rows[i]["timeModified"].ToString());
                                    }
                                }
                                //xlWorkBook.SaveAs("C:\\Log\\SpToSqlJobsLog_" + listDelayName + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.SaveAs(ConfigurationManager.AppSettings.Get("LogLocation") + "\\SpToSqlJobsLog_" + listDelayName + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.Close(true, misValue, misValue);
                                xlApp.Quit();

                                gl.releaseObject(xlWorkSheet);
                                gl.releaseObject(xlWorkBook);
                                gl.releaseObject(xlApp);
                                //}
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                GlobalLogic.ExceptionHandle(e, "Sp To Sql Update Jobs Data" + "---" + "Create and Update Task list Items");
                            }
                            #endregion
                        }
                        catch (Exception e)
                        { 
                            Console.WriteLine(e.Message);
                            GlobalLogic.ExceptionHandle(e, "Sp To Sql Update Jobs Data" + "---" + "Starting operation on Jobs List");
                        }
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //GlobalLogic gl = new GlobalLogic();
                GlobalLogic.ExceptionHandle(e, "Sp To Sql Update Jobs Data");
            }
            finally
            {
                //dt.Dispose();
            }
        }
        /*
        private static void SqlToSpUpdateDelayId(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, DataTable tempTable, string queryJobsDelayItems)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                if (tempTable != null && tempTable.Rows.Count > 0)
                {
                    for (int j = 0; j < tempTable.Rows.Count; j++)
                    {
                        try
                        {
                            string listDelayName = tempTable.Rows[j][0].ToString() + "_Delay";
                            #region Update DelayId
                            try
                            {
                                ListItemCollection getListItemsCol = gl.getListDataVal(ctx, listDelayName);
                                DataTable dt = new DataTable();

                                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                                //DateTime cstTime;

                                dt.Columns.Add("ID");
                                dt.Columns.Add("DelayId");
                                dt.Columns.Add("JobNumber");
                                dt.Columns.Add("Reason");
                                dt.Columns.Add("ReasonId");
                                dt.Columns.Add("ReasonValue");
                                dt.Columns.Add("Title");
                                dt.Columns.Add("DelayDate");
                                dt.Columns.Add("ToDelayDate");
                                dt.Columns.Add("AreYouSure");
                                dt.Columns.Add("JobId");
                                dt.Columns.Add("CstId");

                                if (getListItemsCol != null)
                                {
                                    foreach (ListItem listItemsCol in getListItemsCol)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["ID"] = listItemsCol["ID"];
                                        dr["DelayId"] = listItemsCol["DelayId"];
                                        dr["JobNumber"] = listItemsCol["JobNumber"];
                                        dr["Reason"] = listItemsCol["Reason"];

                                        var reasonLkp = listItemsCol["Reason"] as FieldLookupValue;
                                        if (reasonLkp != null)
                                        {
                                            dr["ReasonId"] = reasonLkp.LookupId;
                                            dr["ReasonValue"] = reasonLkp.LookupValue;
                                        }

                                        if (listItemsCol["Title"] == null) { dr["Title"] = "No Title"; }
                                        else { dr["Title"] = listItemsCol["Title"]; }

                                        if (listItemsCol["DelayDate"] == null) { dr["DelayDate"] = DateTime.Now; }
                                        else { dr["DelayDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["DelayDate"].ToString()), cstZone); }

                                        if (listItemsCol["ToDelayDate"] == null) { dr["ToDelayDate"] = DateTime.Now; }
                                        else
                                        { //dr["ToDelayDate"] = Convert.ToDateTime(listItemsCol["ToDelayDate"].ToString()).AddHours(10); 
                                            dr["ToDelayDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["ToDelayDate"].ToString()), cstZone);
                                        }

                                        dr["AreYouSure"] = listItemsCol["AreYouSure"];
                                        dr["CstId"] = listItemsCol["CstId"];
                                        dr["JobId"] = listItemsCol["JobId"];

                                        dt.Rows.Add(dr);
                                    }
                                }
                                else
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["ID"] = "";
                                    dr["DelayId"] = "";
                                    dr["JobNumber"] = "";
                                    dr["Reason"] = "";
                                    dr["ReasonId"] = "";
                                    dr["ReasonValue"] = "";
                                    dr["Title"] = "";
                                    dr["DelayDate"] = "";
                                    dr["ToDelayDate"] = "";
                                    dr["AreYouSure"] = "";
                                    dr["JobId"] = "";
                                    dr["CstId"] = "";

                                    dt.Rows.Add(dr);
                                }

                                //dbConnection conn = new dbConnection();
                                //DataTable tempTable = null;
                                //tempTable = conn.executeSelectNoParameter(sqlQuery);
                                ListItemCreationInformation itemCreateInfo = null;
                                ListItem oListItem = null;
                                List oList = ctx.Web.Lists.GetByTitle(listDelayName);

                                Excel.Application xlApp;
                                Excel.Workbook xlWorkBook;
                                Excel.Worksheet xlWorkSheet;
                                object misValue = System.Reflection.Missing.Value;

                                xlApp = new Excel.Application();
                                xlWorkBook = xlApp.Workbooks.Add(misValue);
                                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                                dbConnection conn = new dbConnection();
                                SqlParameter[] parameter = {                                
                                                                    new SqlParameter("@JobNum", SqlDbType.VarChar) { Value = tempTable.Rows[j]["JobNum"].ToString() }
                                                               };
                                DataTable delayJobsSQLData = conn.executeSelectQuery(queryJobsDelayItems, parameter);

                                for (int i = 0; i < delayJobsSQLData.Rows.Count; i++)
                                {
                                    try
                                    {
                                        DataRow[] drExists = dt.Select("Id = '" + delayJobsSQLData.Rows[i]["SpId"].ToString() + "'");
                                        if (drExists != null && drExists.Length > 0)
                                        {
                                            DataRow[] drExists1 = dt.Select("DelayId = '" + delayJobsSQLData.Rows[i]["delayId"].ToString() + "'");
                                            if (drExists1 != null && drExists1.Length > 0)
                                            {
                                                //Do Nothing
                                            }
                                            else
                                            {
                                                Console.WriteLine("Updating - " + delayJobsSQLData.Rows[i]["DelayId"].ToString());
                                                oListItem = oList.GetItemById(drExists[0]["ID"].ToString());

                                                oListItem["DelayId"] = delayJobsSQLData.Rows[i]["delayId"].ToString();

                                                oListItem.Update();
                                                ctx.ExecuteQuery();

                                                xlWorkSheet.Cells[i + 1, 1] = "Updating - ";
                                                xlWorkSheet.Cells[i + 1, 2] = delayJobsSQLData.Rows[i]["jobId"].ToString();
                                                xlWorkSheet.Cells[i + 1, 3] = delayJobsSQLData.Rows[i]["delayId"].ToString();
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                }
                                xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_JobDelay" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.Close(true, misValue, misValue);
                                xlApp.Quit();

                                gl.releaseObject(xlWorkSheet);
                                gl.releaseObject(xlWorkBook);
                                gl.releaseObject(xlApp);
                                //}
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            #endregion
                        }
                        catch (Exception e) { Console.WriteLine(e.Message); }



                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                //dt.Dispose();
            }
        }
        */

        /// <summary>
        /// For each delay created in SP there is a relation unique ID created in Framework DB, this function manages the same.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dt"></param>
        /// <param name="jobNum"></param>
        /// <param name="queryJobsDelayItems"></param>
        private static void SqlToSpUpdateDelayId2(ClientContext ctx, DataTable dt, string jobNum, string queryJobsDelayItems)
        {

            try
            {
                GlobalLogic gl = new GlobalLogic();
                string listDelayName = jobNum + "_Delay";
                //#region Update DelayId

                dbConnection conn = new dbConnection();
                SqlParameter[] parameter = {                                
                                                       new SqlParameter("@JobNum", SqlDbType.VarChar) { Value = jobNum }//JobNum
                                                  };
                DataTable delayJobsSQLData = conn.executeSelectQuery(queryJobsDelayItems, parameter);
                ListItem oListItem = null;
                List oList = ctx.Web.Lists.GetByTitle(listDelayName);                

                for (int i = 0; i < delayJobsSQLData.Rows.Count; i++)
                {
                    try
                    {
                        DataRow[] drExists = dt.Select("Id = '" + delayJobsSQLData.Rows[i]["SpId"].ToString() + "'");
                        if (drExists != null && drExists.Length > 0)
                        {
                            DataRow[] drExists1 = dt.Select("DelayId = '" + delayJobsSQLData.Rows[i]["delayId"].ToString() + "'");
                            if (drExists1 != null && drExists1.Length > 0)
                            {
                                Console.WriteLine("Found - " + delayJobsSQLData.Rows[i]["DelayId"].ToString());
                                //Do Nothing
                            }
                            else
                            {
                                Console.WriteLine("Updating - " + delayJobsSQLData.Rows[i]["DelayId"].ToString());
                                oListItem = oList.GetItemById(drExists[0]["ID"].ToString());

                                oListItem["DelayId"] = delayJobsSQLData.Rows[i]["delayId"].ToString();

                                oListItem.Update();
                                ctx.ExecuteQuery();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        GlobalLogic.ExceptionHandle(e, "Sql To Sp Update Delay Id2" + "---" + "Starting operation on Jobs List");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //GlobalLogic gl = new GlobalLogic();
                GlobalLogic.ExceptionHandle(e, "Sp To Sql Update Jobs Data" + "---" + "Starting operation on Jobs List");
            }
        }
        /*
        private static void SqlToSpUpdateCstCallId(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, DataTable tempTable, string queryJobsDelayItems)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                #region UserValues
                //UserValues: Begin
                List<UserValues> uValue = null;
                try
                {
                    UserCollection user = ctx.Web.SiteUsers;
                    ctx.Load(user);
                    ctx.ExecuteQuery();
                    uValue = new List<UserValues>(10000);
                    foreach (User usr in user)
                    {
                        UserValues uv = new UserValues();
                        uv.Email = usr.Email;
                        uv.Id = usr.Id;
                        uv.LoginName = usr.LoginName;
                        uv.Title = usr.Title;
                        uValue.Add(uv);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //UserValues: End
                #endregion

                if (tempTable != null && tempTable.Rows.Count > 0)
                {
                    for (int j = 0; j < tempTable.Rows.Count; j++)
                    {
                        #region Create Lists
                        try
                        {
                            string listDelayName = tempTable.Rows[j][0].ToString() + "_Data";
                            if (gl.createList(ctx, listDelayName, (int)ListTemplateType.TasksWithTimelineAndHierarchy))
                            {
                                Web web = ctx.Web;
                                List list = web.Lists.GetByTitle(listDelayName);
                                List listLkp = web.Lists.GetByTitle("SuppliersList");
                                ctx.Load(listLkp);
                                ctx.ExecuteQuery();

                            }
                            #region Create and Update Task list Items
                            try
                            {
                                ListItemCollection getListItemsCol = gl.getListDataVal(ctx, listDelayName);
                                DataTable dt = new DataTable();

                                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");

                                dt.Columns.Add("ID");
                                dt.Columns.Add("CstCallId");
                                dt.Columns.Add("JobNum");
                                dt.Columns.Add("Called");
                                dt.Columns.Add("CalledFor");
                                dt.Columns.Add("Start");
                                dt.Columns.Add("Complete");
                                dt.Columns.Add("Supplier");
                                dt.Columns.Add("Title");
                                dt.Columns.Add("Supervisor");
                                dt.Columns.Add("Duration");

                                if (getListItemsCol != null)
                                {
                                    foreach (ListItem listItemsCol in getListItemsCol)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["ID"] = listItemsCol["ID"];
                                        dr["CstCallId"] = listItemsCol["CstCallId"];
                                        dr["JobNum"] = listItemsCol["JobNum"];
                                        //dr["Called"] = listItemsCol["Called"];
                                        //dr["CalledFor"] = listItemsCol["CalledFor"];
                                        //dr["Start"] = listItemsCol["Start"];
                                        //dr["Complete"] = listItemsCol["Complete"];

                                        //var reasonLkp = listItemsCol["Reason"] as FieldLookupValue;
                                        //if (reasonLkp != null)
                                        //{
                                        //    dr["ReasonId"] = reasonLkp.LookupId;
                                        //    dr["ReasonValue"] = reasonLkp.LookupValue;
                                        //}

                                        if (listItemsCol["Called"] == null) { dr["Called"] = DateTime.Now; }
                                        else { dr["Called"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Called"].ToString()), cstZone); }

                                        if (listItemsCol["CalledFor"] == null) { dr["CalledFor"] = DateTime.Now; }
                                        else { dr["CalledFor"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["CalledFor"].ToString()), cstZone); }

                                        if (listItemsCol["Start"] == null) { dr["Start"] = DateTime.Now; }
                                        else { dr["Start"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Start"].ToString()), cstZone); }

                                        if (listItemsCol["Complete"] == null) { dr["Complete"] = DateTime.Now; }
                                        else { dr["Complete"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(listItemsCol["Complete"].ToString()), cstZone); }

                                        dr["Supplier"] = listItemsCol["Supplier"];

                                        if (listItemsCol["Title"] == null) { dr["Title"] = "No Title"; }
                                        else { dr["Title"] = listItemsCol["Title"]; }

                                        dr["Supervisor"] = listItemsCol["Supervisor"];
                                        dr["Duration"] = listItemsCol["Duration"];

                                        dt.Rows.Add(dr);
                                    }
                                }
                                else
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["ID"] = "";
                                    dr["CstCallId"] = "";
                                    dr["JobNum"] = "";
                                    dr["Called"] = "";
                                    dr["CalledFor"] = "";
                                    dr["Start"] = "";
                                    dr["Complete"] = "";
                                    dr["Supplier"] = "";
                                    dr["Title"] = "";
                                    dr["Supervisor"] = "";
                                    dr["Duration"] = "";

                                    dt.Rows.Add(dr);
                                }

                                //dbConnection conn = new dbConnection();
                                //DataTable tempTable = null;
                                //tempTable = conn.executeSelectNoParameter(sqlQuery);
                                ListItemCreationInformation itemCreateInfo = null;
                                ListItem oListItem = null;
                                List oList = ctx.Web.Lists.GetByTitle(listDelayName);

                                Excel.Application xlApp;
                                Excel.Workbook xlWorkBook;
                                Excel.Worksheet xlWorkSheet;
                                object misValue = System.Reflection.Missing.Value;

                                xlApp = new Excel.Application();
                                xlWorkBook = xlApp.Workbooks.Add(misValue);
                                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                                dbConnection conn = new dbConnection();
                                SqlParameter[] parameter = {                                
                                                                    new SqlParameter("@JobNum", SqlDbType.VarChar) { Value = tempTable.Rows[j][0].ToString() }
                                                               };
                                DataTable jobsSQLData = conn.executeSelectQuery(queryJobsDelayItems, parameter);

                                for (int i = 0; i < jobsSQLData.Rows.Count; i++)
                                {
                                    try
                                    {
                                        DataRow[] drExists = dt.Select("Id = '" + jobsSQLData.Rows[i]["SpId"].ToString() + "'");
                                        if (drExists != null && drExists.Length > 0)
                                        {
                                            //Do Nothing                            
                                        }
                                        else
                                        {
                                            Console.WriteLine("Updating - " + jobsSQLData.Rows[i]["CstCallId"].ToString());
                                            oListItem = oList.GetItemById(drExists[0]["ID"].ToString());

                                            oListItem["CstCallId"] = jobsSQLData.Rows[i]["CstCallId"].ToString();

                                            oListItem.Update();
                                            ctx.ExecuteQuery();

                                            xlWorkSheet.Cells[i + 1, 1] = "Updating - ";
                                            xlWorkSheet.Cells[i + 1, 2] = jobsSQLData.Rows[i]["CstCallId"].ToString();
                                            xlWorkSheet.Cells[i + 1, 3] = jobsSQLData.Rows[i]["CstCallId"].ToString();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                }
                                xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_" + listDelayName + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                //xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_JobData" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                                xlWorkBook.Close(true, misValue, misValue);
                                xlApp.Quit();

                                gl.releaseObject(xlWorkSheet);
                                gl.releaseObject(xlWorkBook);
                                gl.releaseObject(xlApp);
                                //}
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            #endregion

                            #region Associate WF

                            string workflowName = "WF-" + listDelayName;

                            Web wfWeb = ctx.Web;
                            WorkflowServicesManager wfServicesManager = new WorkflowServicesManager(ctx, wfWeb);
                            WorkflowDeploymentService wfDeploymentService = wfServicesManager.GetWorkflowDeploymentService();
                            WorkflowDefinitionCollection wfDefinitions = wfDeploymentService.EnumerateDefinitions(false);
                            ctx.Load(wfDefinitions, wfDefs => wfDefs.Where(wfd => wfd.DisplayName == workflowName));
                            ctx.ExecuteQuery();

                            if (wfDefinitions.Count > 0 && wfDefinitions != null)
                            {
                                //WorkflowDefinition wfDefinition = wfDefinitions.First();
                                //Guid listId = gl.getListGuid(ctx, listDelayName);
                                //gl.addWorkflowSubscription(ctx, listDelayName, listId);        
                            }
                            #endregion
                        }
                        catch (Exception e) { Console.WriteLine(e.Message); }
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                //dt.Dispose();
            }
        }
        */

        /// <summary>
        /// Create and Update supplier for call forward's build program
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="userName"></param>
        /// <param name="passwordString"></param>
        /// <param name="key"></param>
        /// <param name="appSettingsKey"></param>
        /// <param name="columnName"></param>
        /// <param name="sqlQuery"></param>
        private static void SqlSpConnectEntity(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, string sqlQuery)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);
                //ListItemCollection getListItemsCol = gl.getListData(tenant, userName, passwordString, appSettingsKey);

                #region Get List Item Collection from SP

                ListItemCollection getListItemsCol = null;
                //ListItemCollection getListItemsCol = null;
                List getList = ctx.Web.Lists.GetByTitle(appSettingsKey);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query></Query></View>";
                ListItemCollection getListItemsCollection = getList.GetItems(camlQuery);
                ctx.Load(getListItemsCollection);
                ctx.ExecuteQuery();

                if (getListItemsCollection != null && getListItemsCollection.Count > 0)
                {
                    getListItemsCol = getListItemsCollection;
                }

                #endregion

                DataTable dt = new DataTable();

                dt.Columns.Add("SupName");
                dt.Columns.Add("SupNameRef");
                dt.Columns.Add("SupId");
                dt.Columns.Add("ID");                

               if (getListItemsCol != null)
               {
                   foreach (ListItem listItemsCol in getListItemsCol)
                   {
                       DataRow dr = dt.NewRow();
                       dr["SupName"] = listItemsCol["SupName"];
                       dr["SupNameRef"] = listItemsCol["SupNameRef"];
                       dr["SupId"] = listItemsCol["SupId"];
                       dr["ID"] = listItemsCol["ID"];
                       dt.Rows.Add(dr);
                   }
               }
               else
               {
                   DataRow dr = dt.NewRow();
                   dr["SupName"] = "";
                   dr["SupNameRef"] = "";
                   dr["SupId"] = "";
                   dr["ID"] = "";
                   dt.Rows.Add(dr);
               }                

                dbConnection conn = new dbConnection();
                DataTable tempTable = null;
                tempTable = conn.executeSelectNoParameter(sqlQuery);
                ListItemCreationInformation itemCreateInfo = null;
                ListItem oListItem = null;
                List oList = ctx.Web.Lists.GetByTitle(appSettingsKey);
               
                    Excel.Application xlApp;
                    Excel.Workbook xlWorkBook;
                    Excel.Worksheet xlWorkSheet;
                    object misValue = System.Reflection.Missing.Value;

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Add(misValue);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    {
                        DataRow[] drExists = dt.Select("SupId = '" + tempTable.Rows[i]["l_entity_id"].ToString() + "'");
                        if (drExists != null && drExists.Length > 0)
                        {
                            //Console.WriteLine("Found - " + tempTable.Rows[i]["l_entity_id"].ToString());
                            //xlWorkSheet.Cells[i + 1, 1] = "Found Supplier Code - ";
                            //xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();

                            var rowExists = dt.AsEnumerable().Where(x => x.Field<string>("SupId") == tempTable.Rows[i]["l_entity_id"].ToString()
                                                                            && x.Field<string>("SupName") == tempTable.Rows[i]["s_name"].ToString()
                                                                            && x.Field<string>("SupNameRef") == tempTable.Rows[i]["s_name_ref"].ToString()
                                                                         );

                            DataTable drExists1 = null;
                            if (rowExists.Any())
                            { drExists1 = rowExists.CopyToDataTable(); }

                            if (drExists1 != null && drExists1.Rows.Count > 0)
                            {
                                Console.WriteLine("Found - " + tempTable.Rows[i]["l_entity_id"].ToString());
                                xlWorkSheet.Cells[i + 1, 1] = "Found Enity - ";
                                xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i]["l_entity_id"].ToString();
                            }
                            else
                            {
                                #region Original Code
                                Console.WriteLine("Updating - " + tempTable.Rows[i]["l_entity_id"].ToString());
                                oListItem = oList.GetItemById(drExists[0]["ID"].ToString());

                                oListItem["SupName"] = tempTable.Rows[i]["s_name"].ToString();
                                oListItem["SupNameRef"] = tempTable.Rows[i]["s_name_ref"].ToString();

                                oListItem.Update();
                                ctx.ExecuteQuery();

                                xlWorkSheet.Cells[i + 1, 1] = "Updating - ";
                                xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i]["l_entity_id"].ToString();
                                #endregion
                            }
                        }
                        else
                        {
                            Console.WriteLine("Inserting - " + tempTable.Rows[i]["l_entity_id"].ToString());
                            itemCreateInfo = new ListItemCreationInformation();
                            oListItem = oList.AddItem(itemCreateInfo);                                                        
                            oListItem["SupId"] = Int32.Parse(tempTable.Rows[i]["l_entity_id"].ToString());
                            oListItem["SupName"] = tempTable.Rows[i]["s_name"].ToString();
                            oListItem["SupNameRef"] = tempTable.Rows[i]["s_name_ref"].ToString();
                            xlWorkSheet.Cells[i + 1, 1] = "Inserting Supplier Code - ";
                            xlWorkSheet.Cells[i + 1, 2] = tempTable.Rows[i][0].ToString();

                            oListItem.Update();
                            ctx.ExecuteQuery();
                        }
                    }
                    //xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_Entity" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.SaveAs(ConfigurationManager.AppSettings.Get("LogLocation") + "\\SqlToSpLog_Entity" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    gl.releaseObject(xlWorkSheet);
                    gl.releaseObject(xlWorkBook);
                    gl.releaseObject(xlApp);
                                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //GlobalLogic gl = new GlobalLogic();
                GlobalLogic.ExceptionHandle(e, "Sql To Sp Connect Entity Call Forward Suppliers");
            }
            finally
            {
                //dt.Dispose();
            }
        }

        /// <summary>
        /// Send an email notification when a new delay is created
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="listDelayName"></param>
        /// <param name="jobNum"></param>
        private static void sendEmail(ClientContext ctx, string listDelayName, string jobNum)
        {
            #region sending email for delay
            try
            {
                User user = ctx.Web.EnsureUser(ConfigurationManager.AppSettings.Get("emailId"));//emailId
                ctx.Load(user);
                ctx.ExecuteQuery();

                EmailProperties properties = new EmailProperties();
                properties.To = new string[] { user.LoginName };
                properties.Subject = "A new delay has been added for job :- " + jobNum;// tempTable.Rows[j][0].ToString();
                string body1 = @"
                                                    <!DOCTYPE html>
                                                    <html>
                                                    <head>
                                                    <style>
                                                    table {
                                                        font-family: arial, sans-serif;
                                                        border-collapse: collapse;
                                                        width: 100%;
                                                    }

                                                    td, th {
                                                        text-align: left;
                                                        padding: 8px;
                                                    }

                                                    </style>
                                                    </head>
                                                    <body>

                                                    <table>
                                                      <tr>
                                                        <td>Hi,</td>
                                                      </tr>
                                                      <tr>
                                                        <td></td>
                                                      </tr>
                                                      <tr>
                                                        <td>A new delay has been added at :- <a target=""_blank"" href=""";

                //string body2 = ConfigurationManager.AppSettings.Get("URLJobs");
                string body2 = ConfigurationManager.AppSettings.Get("URLJobs") + "/Lists/" + listDelayName;

                string body3 = @""">Delay Link</a></td>
                                                    <td></td>
                                                    <td></td>
                                                  </tr>
                                                  <tr>
                                                    <td></td>
                                                  </tr>
                                                  <tr>
                                                    <td>Thanks,</td>
                                                  </tr>
                                                  <tr>
                                                    <td>Admin</td>
                                                  </tr>
                                                </table>

                                                </body>
                                                </html>";
                properties.Body = string.Concat(body1 + body2 + body3);
                Utility.SendEmail(ctx, properties);
                ctx.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                GlobalLogic.ExceptionHandle(e, "Send Delay Email");
            }
            #endregion 
        }




        #region old code
        /*
        private static void ConnectOnline(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, string sqlQuery)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var ctx = new ClientContext(tenant))
                {
                    var passWord = new SecureString();
                    foreach (char c in passwordString.ToCharArray()) passWord.AppendChar(c);
                    ctx.Credentials = new SharePointOnlineCredentials(userName, passWord);

                    List getList = ctx.Web.Lists.GetByTitle(appSettingsKey);

                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<View><Query></Query></View>";
                    ListItemCollection getListItemsCol = getList.GetItems(camlQuery);

                    ctx.Load(getListItemsCol);

                    ctx.ExecuteQuery();

                    dt.Columns.Add("JobNumber");

                    if (getListItemsCol != null && getListItemsCol.Count > 0)
                    {
                        foreach (ListItem listItemsCol in getListItemsCol)
                        {
                            DataRow dr = dt.NewRow();
                            dr["JobNumber"] = listItemsCol[columnName];
                            dt.Rows.Add(dr);
                        }
                    }

                    dbConnection conn = new dbConnection();
                    DataTable tempTable = null;
                    tempTable = conn.executeSelectNoParameter(sqlQuery);
                    List oList = null;
                    ListItemCreationInformation itemCreateInfo = null;
                    ListItem oListItem = null;
                    oList = ctx.Web.Lists.GetByTitle(appSettingsKey);

                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    {
                        DataRow[] drExists = dt.Select("JobNumber = '" + tempTable.Rows[i][0].ToString() + "'");
                        if (drExists != null && drExists.Length > 0)
                        {
                            Console.WriteLine("Found - " + tempTable.Rows[i][1].ToString());
                        }
                        else
                        {
                            Console.WriteLine("Inserting - " + tempTable.Rows[i][1].ToString());
                            itemCreateInfo = new ListItemCreationInformation();
                            oListItem = oList.AddItem(itemCreateInfo);
                            if (key == "JobsDataList")
                            {
                                oListItem["Title"] = tempTable.Rows[i][1].ToString();
                                oListItem["Job_x0020_Address"] = tempTable.Rows[i][3].ToString();
                                oListItem["Job_x0020_Supervisor"] = tempTable.Rows[i][7].ToString();
                            }
                            else if (key == "SuppliersList")
                            {
                                oListItem["Title"] = tempTable.Rows[i][0].ToString();
                                oListItem["SupplierCode"] = tempTable.Rows[i][0].ToString();
                                oListItem["SupplierName"] = tempTable.Rows[i][1].ToString();
                                oListItem["SupplierEmail"] = tempTable.Rows[i][2].ToString();
                            }

                            oListItem.Update();
                            ctx.ExecuteQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                dt.Dispose();
            }
        }

        private static void SpToSQL(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, string sqlQuery)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var ctx = new ClientContext(tenant))
                {
                    var passWord = new SecureString();
                    foreach (char c in passwordString.ToCharArray()) passWord.AppendChar(c);
                    ctx.Credentials = new SharePointOnlineCredentials(userName, passWord);

                    List getList = ctx.Web.Lists.GetByTitle(appSettingsKey);

                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<View><Query></Query></View>";
                    ListItemCollection getListItemsCol = getList.GetItems(camlQuery);

                    ctx.Load(getListItemsCol);

                    ctx.ExecuteQuery();

                    dt.Columns.Add("Job");
                    dt.Columns.Add("ETSId");
                    dt.Columns.Add("ItemsDescription");
                    dt.Columns.Add("Selected_x0020_Job");
                    dt.Columns.Add("Cost_x0020_Centre");
                    dt.Columns.Add("Reason_x0020_Code");
                    dt.Columns.Add("Supplier");
                    dt.Columns.Add("DeliveryDetails");
                    dt.Columns.Add("SupplierID");
                    dt.Columns.Add("DeliveryDate");
                    dt.Columns.Add("Price");
                    dt.Columns.Add("GST");
                    dt.Columns.Add("Author");
                    dt.Columns.Add("Approved_x0020_By");
                    dt.Columns.Add("RegeneratePO");
                    dt.Columns.Add("ID");
                    dt.Columns.Add("JobID");
                    dt.Columns.Add("CostCentreID");
                    dt.Columns.Add("Sent");
                    dt.Columns.Add("marked");
                    dt.Columns.Add("Created");
                    dt.Columns.Add("Complete");
                    dt.Columns.Add("Recharge");

                    if (getListItemsCol != null && getListItemsCol.Count > 0)
                    {
                        try
                        {
                            foreach (ListItem listItemsCol in getListItemsCol)
                            {
                                DataRow dr = dt.NewRow();
                                dr["Job"] = listItemsCol["Title"];
                                dr["ETSId"] = listItemsCol["ETSId"];
                                dr["ItemsDescription"] = listItemsCol["ItemsDescription"];
                                dr["Selected_x0020_Job"] = listItemsCol["Selected_x0020_Job"];
                                dr["Cost_x0020_Centre"] = listItemsCol["Cost_x0020_Centre"];
                                dr["Reason_x0020_Code"] = listItemsCol["Reason_x0020_Code"];
                                dr["Supplier"] = listItemsCol["Supplier"];
                                dr["DeliveryDetails"] = listItemsCol["DeliveryDetails"];
                                dr["SupplierID"] = listItemsCol["SupplierID"];
                                dr["DeliveryDate"] = listItemsCol["DeliveryDate"];
                                dr["Price"] = listItemsCol["Price"];
                                dr["GST"] = listItemsCol["GST"];

                                if (listItemsCol["Author"] == null)
                                {
                                    listItemsCol["Author"] = "";
                                }
                                else
                                {
                                    FieldUserValue userAuthor = (FieldUserValue)listItemsCol["Author"];
                                    dr["Author"] = userAuthor.LookupValue;
                                }
                                if (listItemsCol["Approved_x0020_By"] == null)
                                {
                                    listItemsCol["Approved_x0020_By"] = "";
                                }
                                else
                                {
                                    FieldUserValue userAuthor = (FieldUserValue)listItemsCol["Approved_x0020_By"];
                                    dr["Approved_x0020_By"] = userAuthor.LookupValue;
                                }
                                dr["RegeneratePO"] = listItemsCol["RegeneratePO"];
                                dr["ID"] = listItemsCol["ID"];
                                dr["JobID"] = listItemsCol["JobID"];
                                dr["CostCentreID"] = listItemsCol["CostCentreID"];
                                dr["Sent"] = listItemsCol["Sent"];
                                dr["marked"] = listItemsCol["marked"];
                                dr["Created"] = listItemsCol["Created"];
                                dr["Complete"] = listItemsCol["Complete"];
                                dr["Recharge"] = listItemsCol["Recharge"];
                                dt.Rows.Add(dr);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    dbConnection conn = new dbConnection();
                    DataTable tempTable = null;
                    tempTable = conn.executeSelectNoParameter(sqlQuery);
                    List oList = null;
                    ListItemCreationInformation itemCreateInfo = null;
                    ListItem oListItem = null;
                    oList = ctx.Web.Lists.GetByTitle(appSettingsKey);

                    string job, eTSNo, itemsDescription, selectedJob, costCentre, reasonCode, supplier, deliveryDetails, supplierID, deliveryDate, price, gST, createdBy, approvedBy, regeneratePO, jobID, eTSId, costCentreID, created, recharge, itemType, path;
                    float id, sent, marked, complete;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow[] drExists = tempTable.Select("ETSId = '" + dt.Rows[i][1].ToString() + "'");
                        if (drExists != null && drExists.Length > 0)
                        {
                            Console.WriteLine("Found - " + dt.Rows[i][1].ToString());
                        }
                        else
                        {
                            try
                            {
                                Console.WriteLine("Inserting - " + dt.Rows[i][1].ToString());
                                job = dt.Rows[i][0].ToString();
                                eTSId = dt.Rows[i][1].ToString();
                                //=IF(ETSId>999,CONCATENATE("E00000",ETSId),IF(ETSId>99,CONCATENATE("E000000",ETSId),IF(ETSId>9,CONCATENATE("E000000",ETSId),CONCATENATE("E",ETSId))))
                                if (Int32.Parse(eTSId) > 999) { eTSNo = "E0000" + eTSId; }
                                else if (Int32.Parse(eTSId) > 99) { eTSNo = "E00000" + eTSId; }
                                else if (Int32.Parse(eTSId) > 9) { eTSNo = "E00000" + eTSId; }
                                else { eTSNo = "E" + eTSId; }
                                itemsDescription = dt.Rows[i][2].ToString();
                                selectedJob = dt.Rows[i][3].ToString();
                                costCentre = dt.Rows[i][4].ToString();
                                reasonCode = dt.Rows[i][5].ToString();
                                supplier = dt.Rows[i][6].ToString();
                                deliveryDetails = dt.Rows[i][7].ToString();
                                supplierID = dt.Rows[i][8].ToString();
                                deliveryDate = string.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? DateTime.Today.ToString() : dt.Rows[i][9].ToString();
                                price = string.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? "0" : dt.Rows[i][10].ToString();
                                gST = dt.Rows[i][11].ToString();
                                createdBy = dt.Rows[i][12].ToString();
                                approvedBy = dt.Rows[i][13].ToString();
                                regeneratePO = string.IsNullOrEmpty(dt.Rows[i][14].ToString()) ? DateTime.Today.ToString() : dt.Rows[i][14].ToString();
                                id = float.Parse(dt.Rows[i][15].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                jobID = dt.Rows[i][16].ToString();
                                costCentreID = dt.Rows[i][17].ToString();
                                sent = float.Parse(dt.Rows[i][18].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                marked = float.Parse(dt.Rows[i][19].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                created = string.IsNullOrEmpty(dt.Rows[i][20].ToString()) ? DateTime.Today.ToString() : dt.Rows[i][20].ToString();
                                complete = float.Parse(dt.Rows[i][21].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                recharge = dt.Rows[i][22].ToString();
                                itemType = "Item";
                                path = "sites/365Build/Watersun/Lists/l_ETSData";

                                //insert
                                string sclearsqlIns = string.Concat("INSERT INTO [WatersunData].[dbo].[ETSDataExportDemo]" +
                                                                       "([Job], [ETS No], [ItemsDescription], [Selected Job], [Cost Centre], [Reason Code], [Supplier], [DeliveryDetails], [SupplierID], [DeliveryDate], [Price], [GST], [Created By], [Approved By], [RegeneratePO], [ID], [JobID], [ETSId], [CostCentreID], [Sent], [marked], [Created], [Complete], [Recharge], [Item Type], [Path])" +
                                                                       "VALUES (@job, @eTSNo, @itemsDescription, @selectedJob, @costCentre, @reasonCode, @supplier, @deliveryDetails, @supplierID, @deliveryDate, @price, @gST, @createdBy, @approvedBy, @regeneratePO, @id, @jobID, @eTSId, @costCentreID, @sent, @marked, @created, @complete, @recharge, @itemType, @path)");
                                SqlParameter[] parameterUpd = {                                

                                                    new SqlParameter("@job", SqlDbType.NVarChar) { Value = job },
                                                    new SqlParameter("@eTSNo", SqlDbType.NVarChar) { Value = eTSNo },
                                                    new SqlParameter("@itemsDescription", SqlDbType.NVarChar) { Value = itemsDescription },
                                                    new SqlParameter("@selectedJob", SqlDbType.NVarChar) { Value = selectedJob },
                                                    new SqlParameter("@costCentre", SqlDbType.NVarChar) { Value = costCentre },
                                                    new SqlParameter("@reasonCode", SqlDbType.NVarChar) { Value = reasonCode },
                                                    new SqlParameter("@supplier", SqlDbType.NVarChar) { Value = supplier },
                                                    new SqlParameter("@deliveryDetails", SqlDbType.NVarChar) { Value = deliveryDetails },
                                                    new SqlParameter("@supplierID", SqlDbType.NVarChar) { Value = supplierID },
                                                    new SqlParameter("@deliveryDate", SqlDbType.DateTime) { Value = deliveryDate },
                                                    new SqlParameter("@price", SqlDbType.Money) { Value = price },
                                                    new SqlParameter("@gST", SqlDbType.NVarChar) { Value = gST },
                                                    new SqlParameter("@createdBy", SqlDbType.NVarChar) { Value = createdBy },
                                                    new SqlParameter("@approvedBy", SqlDbType.NVarChar) { Value = approvedBy },
                                                    new SqlParameter("@regeneratePO", SqlDbType.DateTime) { Value = regeneratePO },
                                                    new SqlParameter("@id", SqlDbType.Float) { Value = id },
                                                    new SqlParameter("@jobID", SqlDbType.NVarChar) { Value = jobID },
                                                    new SqlParameter("@eTSId", SqlDbType.NVarChar) { Value = eTSId },
                                                    new SqlParameter("@costCentreID", SqlDbType.NVarChar) { Value = costCentreID },
                                                    new SqlParameter("@sent", SqlDbType.Float) { Value = sent },
                                                    new SqlParameter("@marked", SqlDbType.Float) { Value = marked },
                                                    new SqlParameter("@created", SqlDbType.DateTime) { Value = created },
                                                    new SqlParameter("@complete", SqlDbType.Float) { Value = complete },
                                                    new SqlParameter("@recharge", SqlDbType.NVarChar) { Value = recharge },
                                                    new SqlParameter("@itemType", SqlDbType.NVarChar) { Value = itemType },
                                                    new SqlParameter("@path", SqlDbType.NVarChar) { Value = path }
                                                         };
                                bool isInsert = conn.executeInsertQuery(sclearsqlIns, parameterUpd);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                dt.Dispose();
            }
        }
*/
        #endregion old code
    }
}
