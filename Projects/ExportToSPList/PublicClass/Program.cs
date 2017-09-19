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

namespace ExportToSPList
{
    class Program
    {
        //public static DataTable dt = new DataTable();
        static void Main(string[] args)
        {
            //ExportToExcel ex = new ExportToExcel();
            CreateListWF lw = new CreateListWF();
            //lw.CreateListWFs();
            //ex.SaveToExcel();

            //WF wf = new WF();
            //wf.AssignWF();

            //ReadAllSettings();

            //            string sqlQuery1 = @"
            //                                    SELECT  
            //	                                    [JobNumber]
            //	                                    ,[JobAddress]
            //                                    FROM [WatersunData].[dbo].[vJobsRefined]
            //                                    where left(JobNumber,1) in (3,4,5)
            //                                    ";
            //            CreateListWFs(sqlQuery1);

            SqlToSpJobs("select * from [WatersunData].[dbo].[ETSDataExportDemo]");
        }

        static void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                ////////string tenant = "https://enterpriseuser.sharepoint.com/sites/worksite";
                ////////string userName = "officeuser@enterpriseuser.onmicrosoft.com";
                ////////string passwordString = "india@123";
                string tenant = appSettings.Get("URL");
                string userName = appSettings.Get("UserName");
                string passwordString = appSettings.Get("Password");

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
                                                  FROM [WatersunData].[dbo].[vSupplierList] order by [GroupList], [SupplierName]
                                                ";

                                SqlSpConnect(tenant, userName, passwordString, key, appSettings[key], appSettings["SuppliersColumn"], sqlQuery);
                                break;
                            case "SuppliersList2":
                                Console.WriteLine(appSettings["SuppliersList2"]);
                                sqlQuery = @"
                                                SELECT [Supplier_Code]
                                                      ,[SupplierName]
                                                      ,[AccountEmail]
                                                      ,[GroupList]
                                                  FROM [WatersunData].[dbo].[vSupplierList] order by [GroupList], [SupplierName]
                                                ";
                                tenant = "https://networkintegration.sharepoint.com/sites/365Build/Watersun/WatersunJobs";
                                SqlSpConnect(tenant, userName, passwordString, key, appSettings[key], appSettings["SuppliersColumn2"], sqlQuery);
                                break;
                            case "ETSData":
                                Console.WriteLine(appSettings["ETSData"]);
                                #region query
                                sqlQuery = @"
                                                SELECT 
                                                    [Job], [ETS No], [ItemsDescription], [Selected Job], [Cost Centre], [Reason Code], [Supplier], [DeliveryDetails], [SupplierID], [DeliveryDate], [Price], [GST], [Created By], [Approved By], [Purchase Order], [RegeneratePO], [ID], [JobID], [ETSId], [CostCentreID], [Created], [Complete], [Recharge], [RechargeID], [RechargeAmount], [RechargeAMSupID], [RechargeNZSupID], [ReasonDescription], [RechargeSupId]
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

        private static void SqlSpConnect(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, string sqlQuery)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);
                ListItemCollection getListItemsCol = gl.getListData(tenant, userName, passwordString, appSettingsKey);

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
                            else if (key == "SuppliersList2")
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
                    xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_Inserting" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    gl.releaseObject(xlWorkSheet);
                    gl.releaseObject(xlWorkBook);
                    gl.releaseObject(xlApp);
                }
                else if (key == "ETSData")
                {
                    string job, eTSNo, itemsDescription, selectedJob, costCentre, reasonCode, supplier, deliveryDetails, supplierID, deliveryDate, price, gST, createdBy, approvedBy, regeneratePO, jobID, eTSId, costCentreID, created, recharge, RechargeID, RechargeAmount, RechargeAMSupID, RechargeNZSupID, ReasonDescription;
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
                                created = string.IsNullOrEmpty(dt.Rows[i][18].ToString()) ? DateTime.Today.ToString() : dt.Rows[i][18].ToString();
                                complete = float.Parse(dt.Rows[i][19].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                recharge = dt.Rows[i][20].ToString();
                                RechargeID = dt.Rows[i][21].ToString();
                                RechargeAmount = string.IsNullOrEmpty(dt.Rows[i][22].ToString()) ? "0" : dt.Rows[i][22].ToString(); //dt.Rows[i][29].ToString();
                                RechargeAMSupID = dt.Rows[i][23].ToString();
                                RechargeNZSupID = dt.Rows[i][24].ToString();
                                ReasonDescription = dt.Rows[i][25].ToString();
                                RechargeSupplierID = Int32.Parse(string.IsNullOrEmpty(dt.Rows[i][26].ToString()) ? "0" : dt.Rows[i][26].ToString());

                                //insert
                                string sclearsqlIns = string.Concat("INSERT INTO [WatersunData].[dbo].[ETSDataExportDemo]" +
                                                                       "([Job], [ETS No], [ItemsDescription], [Selected Job], [Cost Centre], [Reason Code], [Supplier], [DeliveryDetails], [SupplierID], [DeliveryDate], [Price], [GST], [Created By], [Approved By], [RegeneratePO], [ID], [JobID], [ETSId], [CostCentreID], [Created], [Complete], [Recharge], [RechargeID], [RechargeAmount], [RechargeAMSupID], [RechargeNZSupID], [ReasonDescription], [RechargeSupId])" +
                                                                       "VALUES (@job, @eTSNo, @itemsDescription, @selectedJob, @costCentre, @reasonCode, @supplier, @deliveryDetails, @supplierID, @deliveryDate, @price, @gST, @createdBy, @approvedBy, @regeneratePO, @id, @jobID, @eTSId, @costCentreID, @created, @complete, @recharge,  @RechargeID, @RechargeAmount,   @RechargeAMSupID,  @RechargeNZSupID,  @ReasonDescription, @RechargeSupplierID )");
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
                                                    new SqlParameter("@RechargeSupplierID", SqlDbType.Int) { Value = RechargeSupplierID }

                                                         };
                                bool isInsert = conn.executeInsertQuery(sclearsqlIns, parameterUpd);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                    xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_Inserting" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
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
            }
            finally
            {
                //dt.Dispose();
            }
        }

        private static void UpdateSqlSpConnect(string tenant, string userName, string passwordString, string key, string appSettingsKey, string columnName, string columnName2, string sqlQuery)
        {
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                ListItemCollection getListItemsCol = gl.getListData(tenant, userName, passwordString, appSettingsKey);

                DataTable dt = new DataTable();

                if (key == "JobsSuppList")
                {
                    dt.Columns.Add("ID");
                    dt.Columns.Add("JobNumber");
                    dt.Columns.Add("SuppName");
                    dt.Columns.Add("ConstructionManager");
                    if (getListItemsCol != null)
                    {
                        foreach (ListItem listItemsCol in getListItemsCol)
                        {
                            DataRow dr = dt.NewRow();
                            dr["JobNumber"] = listItemsCol[columnName];
                            dr["SuppName"] = listItemsCol[columnName2];
                            dr["ConstructionManager"] = listItemsCol["ConstructionManager"];
                            dr["ID"] = listItemsCol["ID"];
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
                                DataRow[] drExists2 = dt.Select("JobNumber = '" + tempTable.Rows[i][0].ToString() + "'" + " AND SuppName = '" + tempTable.Rows[i][2].ToString() + "'" + "AND ConstructionManager = '" + tempTable.Rows[i][3].ToString() + "'");
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
                    xlWorkBook.SaveAs("C:\\Log\\SqlToSpLog_Updating" + DateTime.Now.ToString("_ddMMyyyy_HHmmss") + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
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
            }
            finally
            {
                //dt.Dispose();
            }
        }

        private static void CreateListWFs(string sqlQuery)
        {
            //string tenant = "https://networkintegration.sharepoint.com/sites/Development";
            string tenant = "https://networkintegration.sharepoint.com/sites/365Build/Watersun/WatersunJobs";        
            string userName = "andrew@365build.com.au";
            string passwordString = "187Ch@lleng3r";
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
            string passwordString = "187Ch@lleng3r";
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
            string passwordString = "187Ch@lleng3r";
            string listName;

            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);
                dbConnection conn = new dbConnection();

                #region Metadata
                //string sqlQueryMetadata = "select top 10 JobNum,JobAddr, client,delay, week,overall, JobDelayLink,JobLink  from [WatersunData].[dbo].[vJobDelay] order by jobnum desc";
                string sqlQueryMetadata = @"select JobNum,JobAddr, client,delay, week,overall, JobDelayLink,JobLink,Supervisor,ConstructionManager  from [WatersunData].[dbo].[vJobDelay]
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
                        //if (gl.createListItemsGeneric(ctx, delayJobsSQLData, dtSPDelayListItems, listDelayName, colNameJobsDelay, colNameJobsDelayUpdate, "DelayId", uValue))

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


        //private static void CreateListWFs(string sqlQuery)
        //{
        //    string tenant = "https://networkintegration.sharepoint.com/sites/365Build/Watersun";//"https://mytenant.sharepoint.com/sites/test";
        //    string userName = "andrew@365build.com.au";
        //    string passwordString = "187Ch@lleng3r";
        //    string listName;// = "Test1234567";
        //    //string wfAssoc = "05639090-c09B-478A-B1F8-611718539D7F";//"05639090-c09b-478a-b1f8-611718539d7f";

        //    try
        //    {
        //        GlobalLogic gl = new GlobalLogic();
        //        ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);
        //        dbConnection conn = new dbConnection();
        //        DataTable tempTable = conn.executeSelectNoParameter(sqlQuery);
        //        //string query = @" select l_job_id ,s_job_num  ,l_cstL_call_id ,sLogisticsActivity  ,d_called_fBest ,d_calledFor_fBest ,d_start_fBest,d_complete_fBest  ,d_start_fMan ,d_called_fMan ,d_calledFor_fMan ,d_complete_fMan,d_called_fBas ,d_calledFor_fBas ,d_start_fBas,d_complete_fBas,d_called_act,d_calledFor_act, d_start_act ,d_complete_act from [WatersunData].[dbo].[vJobsCalledForDates] ";

        //        //Get all user names
        //        UserCollection user = ctx.Web.SiteUsers;
        //        //List<string> siteSupName = new List<string>();

        //        ctx.Load(user);
        //        ctx.ExecuteQuery();
        //        int u = 0;
        //        List<UserValues> uValue = new List<UserValues>(10000);
        //        foreach (User usr in user)
        //        //for (int u = 0; u < user.Count; u++)
        //        {
        //            UserValues uv = new UserValues();
        //            uv.Email = usr.Email;
        //            //uv.Email = user.[u]["Email"];
        //            uv.Id = usr.Id;
        //            uv.LoginName = usr.LoginName;
        //            uv.Title = usr.Title;

        //            //UserValues[] uValue = new UserValues[10000];
        //            //uValue[u] = uv;
        //            //u = u + 1;

        //            //List<UserValues> uValue = new List<UserValues>(10000);
        //            uValue.Add(uv);
        //        }

        //        //Get All List Names
        //        Web web = ctx.Web;
        //        ctx.Load(web.Lists,
        //                     lists => lists.Include(list => list.Title, // For each list, retrieve Title and Id. 
        //                                            list => list.Id));
        //        ctx.ExecuteQuery();

        //        List<string> colName = new List<string>();
        //        foreach (List list in web.Lists)
        //        {
        //            colName.Add(list.Title);
        //        }

        //        // Create Lists
        //        if (tempTable != null && tempTable.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < tempTable.Rows.Count; i++)
        //            {
        //                string jobNum = tempTable.Rows[i][0].ToString();
        //                listName = tempTable.Rows[i][1].ToString();
        //                //listName = "JobsMasterList";

        //                //bool listExists = false;
        //                //for (int k = 0; i < colName.Count; k++)
        //                //{
        //                //    if (listName == colName[k].ToString())
        //                //    {
        //                //        listExists = true;
        //                //        //Console.WriteLine("List:- " + listName + " already exists");
        //                //    } 
        //                //}
        //                if (2 == 1)
        //                {
        //                    Console.WriteLine("List:- " + listName + " already exists");
        //                }
        //                else
        //                {
        //                    if (gl.createList(ctx, listName))
        //                    {
        //                        if (gl.createListColumns(ctx, listName))
        //                        {
        //                            if (gl.createListView(ctx, listName))
        //                            {
        //                                Guid guid = gl.getListGuid(ctx, listName);
        //                                if (guid != Guid.Empty)
        //                                //if (gl.getListGuid(ctx, listName) != Guid.Empty)
        //                                {
        //                                    string query = string.Concat("select l_job_id ,s_job_num  ,l_cstL_call_id ,sLogisticsActivity  ,d_called_fBest ,d_calledFor_fBest ,d_start_fBest,d_complete_fBest  ,d_start_fMan ,d_called_fMan ,d_calledFor_fMan ,d_complete_fMan,d_called_fBas ,d_calledFor_fBas ,d_start_fBas,d_complete_fBas,d_called_act,d_calledFor_act, d_start_act , d_complete_act, Supervisor from [WatersunData].[dbo].[vJobsCalledForDates] " +
        //                                                                         "WHERE s_job_num = @s_job_num");
        //                                    SqlParameter[] parameter = {                                
        //                                                new SqlParameter("@s_job_num", SqlDbType.VarChar) { Value = jobNum }
        //                                                 };
        //                                    DataTable calledforDatesData = conn.executeSelectQuery(query, parameter);
        //                                    if (gl.createListItems(ctx, calledforDatesData, listName, uValue))
        //                                    {
        //                                        gl.addWorkflowSubscription(ctx, listName, guid);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                //}
        //            }

        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //}

        #region old code
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

        /*
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
                    //dt.Columns.Add("-----Purchase Order]  ");
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
                            dr["Author"] = listItemsCol["Author"];
                            dr["Approved_x0020_By"] = listItemsCol["Approved_x0020_By"];
                            //dr["-----Purchase Order]  "] = listItemsCol["-----Purchase Order]  "];
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
                                foreach (DataRow rr in dt.Rows)
                                {
                                    job = rr["Job"].ToString();
                                    eTSId = rr["ETSId"].ToString();

                                    if (Int32.Parse(eTSId) > 999) { eTSNo = "E00000" + eTSId; }
                                    else if (Int32.Parse(eTSId) > 99) { eTSNo = "E000000" + eTSId; }
                                    else if (Int32.Parse(eTSId) > 9) { eTSNo = "E000000" + eTSId; }
                                    else { eTSNo = "E" + eTSId; }

                                    itemsDescription = rr["ItemsDescription"].ToString();
                                    selectedJob = rr["Selected_x0020_Job"].ToString();
                                    costCentre = rr["Cost_x0020_Centre"].ToString();
                                    reasonCode = rr["Reason_x0020_Code"].ToString();
                                    supplier = rr["Supplier"].ToString();
                                    deliveryDetails = rr["DeliveryDetails"].ToString();
                                    supplierID = rr["SupplierID"].ToString();
                                    deliveryDate = string.IsNullOrEmpty(rr["DeliveryDate"].ToString()) ? DateTime.Today.ToString() : rr["DeliveryDate"].ToString();
                                    price = string.IsNullOrEmpty(rr["Price"].ToString()) ? "0" : rr["Price"].ToString();
                                    gST = rr["GST"].ToString();
                                    createdBy = rr["Author"].ToString();
                                    approvedBy = rr["Approved_x0020_By"].ToString();
                                    //string purchaseOrder    = "";
                                    regeneratePO = string.IsNullOrEmpty(rr["RegeneratePO"].ToString()) ? DateTime.Today.ToString() : rr["RegeneratePO"].ToString();
                                    id = float.Parse(rr["ID"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                    jobID = rr["JobID"].ToString();
                                    costCentreID = rr["CostCentreID"].ToString();
                                    sent = float.Parse(rr["Sent"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                    marked = float.Parse(rr["marked"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                    created = string.IsNullOrEmpty(rr["Created"].ToString()) ? DateTime.Today.ToString() : rr["Created"].ToString();
                                    complete = float.Parse(rr["Complete"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                    recharge = rr["Recharge"].ToString();
                                    itemType = "Item";
                                    path = "sites/365Build/Watersun/Lists/l_ETSData";
                                    //
                                    // job =                rr["Job"].ToString();
                                    // eTSId =              rr["ETSId"].ToString();

                                    // if (Int32.Parse(eTSId) > 999) { eTSNo = "E00000" + eTSId; }
                                    //     else if (Int32.Parse(eTSId) > 99) { eTSNo = "E000000" + eTSId; }
                                    //     else if (Int32.Parse(eTSId) > 9) { eTSNo = "E000000" + eTSId; }
                                    //     else { eTSNo = "E" + eTSId; }

                                    // itemsDescription =   rr["ItemsDescription"].ToString();
                                    // selectedJob =        rr["Selected_x0020_Job"].ToString();
                                    // costCentre =         rr["Cost_x0020_Centre"].ToString();
                                    // reasonCode =         rr["Reason_x0020_Code"].ToString();
                                    // supplier =           rr["Supplier"].ToString();
                                    // deliveryDetails =    rr["DeliveryDetails"].ToString();
                                    // supplierID =         rr["SupplierID"].ToString();
                                    // deliveryDate =       string.IsNullOrEmpty(rr["DeliveryDate"].ToString()) ? DateTime.Today.ToString() : rr["DeliveryDate"].ToString();
                                    // price =              string.IsNullOrEmpty(rr["Price"].ToString()) ? "0" : rr["Price"].ToString();
                                    // gST =                rr["GST"].ToString();
                                    // createdBy =          rr["Author"].ToString();
                                    // approvedBy =         rr["Approved_x0020_By"].ToString();
                                    ////string purchaseOrder    = "";
                                    // regeneratePO =       string.IsNullOrEmpty(rr["RegeneratePO"].ToString()) ? DateTime.Today.ToString() : rr["RegeneratePO"].ToString();
                                    // id =                 float.Parse(rr["ID"].ToString(), CultureInfo.InvariantCulture.NumberFormat);                                     
                                    // jobID =              rr["JobID"].ToString();
                                    // costCentreID =       rr["CostCentreID"].ToString();
                                    // sent =               float.Parse(rr["Sent"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                    // marked =             float.Parse(rr["marked"].ToString(), CultureInfo.InvariantCulture.NumberFormat);                                    
                                    // created =            string.IsNullOrEmpty(rr["Created"].ToString()) ? DateTime.Today.ToString() : rr["Created"].ToString();
                                    // complete =           float.Parse(rr["Complete"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                    // recharge =           rr["Recharge"].ToString();
                                    // itemType =           "Item";
                                    // path =               "sites/365Build/Watersun/Lists/l_ETSData";
                                    //
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
