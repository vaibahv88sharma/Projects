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
using Microsoft.SharePoint.Client.Workflow;

namespace ExportToSPList
{
    class GlobalLogic
    {
        public ClientContext ConnectSP(string tenant, string userName, string passwordString)
        {
            ClientContext context = null;
            try
            {
                ClientContext ctx = new ClientContext(tenant);
                var passWord = new SecureString();
                foreach (char c in passwordString.ToCharArray()) passWord.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(userName, passWord);
                context = ctx;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return context;
        }

        public ListItemCollection getListData(string tenant, string userName, string passwordString, string appSettingsKey)
        {
            ListItemCollection getListItemsCollection = null;
            try
            {
                GlobalLogic gl = new GlobalLogic();
                ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                List getList = ctx.Web.Lists.GetByTitle(appSettingsKey);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query></Query></View>";
                ListItemCollection getListItemsCol = getList.GetItems(camlQuery);
                ctx.Load(getListItemsCol);
                ctx.ExecuteQuery();

                if (getListItemsCol != null && getListItemsCol.Count > 0)
                {
                    getListItemsCollection = getListItemsCol;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return getListItemsCollection;
        }

        public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }


        public Boolean createSubsite(ClientContext ctx, string appSettingsKey)
        {
            bool success = false;
            try
            {
                WebCreationInformation creation = new WebCreationInformation();
                creation.Url = appSettingsKey;
                creation.Title = appSettingsKey;
                Web newWeb = ctx.Web.Webs.Add(creation);
                // Retrieve the new web information. 
                ctx.Load(newWeb, w => w.Title);
                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public Boolean activateFeature(ClientContext ctx)
        {
            //22a9ef51-737b-4ff2-9346-694633fe4416
            bool success = false;
            try
            {
                var featureId = new Guid("22a9ef51-737b-4ff2-9346-694633fe4416");
                var features = ctx.Web.Features;
                features.Add(featureId, true, FeatureDefinitionScope.None);
                ctx.ExecuteQuery();

                //FeatureCollection webFeatures = ctx.Web.Features;
                //ctx.Load(webFeatures);
                //ctx.ExecuteQuery();
                //foreach (var f in webFeatures)
                //{
                //    Console.WriteLine(f.DefinitionId + "----" + f.Context.ApplicationName);
                //}
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public Boolean createList(ClientContext ctx, string appSettingsKey, int templateType)
        {
            bool success = false;
            try
            {
                Web hostWeb = ctx.Web;
                ListCreationInformation olist = new ListCreationInformation();

                olist.Title = appSettingsKey;
                olist.Description = appSettingsKey;
                olist.TemplateType = templateType;
                //olist.TemplateType = (int)ListTemplateType.TasksWithTimelineAndHierarchy;
                hostWeb.Lists.Add(olist);                
                ctx.ExecuteQuery();

                List list = hostWeb.Lists.GetByTitle(appSettingsKey);
                Field field = list.Fields.GetByTitle("Title");
                field.Required = false;
                field.Update();
                ctx.Load(field);
                ctx.ExecuteQuery();

                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
            //return getListItemsCollection;
        }

        public Boolean createListColumns(ClientContext ctx, string appSettingsKey)
        {
            bool success = false;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);

                //Name=\"OrdenFase\" DisplayName=\"Orden Fase\"

                Field field1 = list.Fields.AddFieldAsXml("<Field DisplayName='SrNo' Type='Number' />", true, AddFieldOptions.DefaultValue);
                FieldNumber fld1 = ctx.CastTo<FieldNumber>(field1);
                fld1.Update();

                Field field2 = list.Fields.AddFieldAsXml("<Field DisplayName='CostCode' Type='Number' />", true, AddFieldOptions.DefaultValue);
                FieldNumber fld2 = ctx.CastTo<FieldNumber>(field2);
                fld2.Update();

                Field field3 = list.Fields.AddFieldAsXml("<Field DisplayName='Comments' Type='Text' />", true, AddFieldOptions.DefaultValue);
                FieldText fld3 = ctx.CastTo<FieldText>(field3);
                fld3.Update();

                /////////////////////////////////////
                var lookupFieldXml1 = "<Field DisplayName=\"Supplier\" Type=\"Lookup\" />";
                var lkpField1 = list.Fields.AddFieldAsXml(lookupFieldXml1, false, AddFieldOptions.AddToAllContentTypes);
                var lookupField1 = ctx.CastTo<FieldLookup>(lkpField1);
                lookupField1.LookupList = "51E19AE8-B48D-498A-9FEF-F7E61B366A53"; //sourceLookupList.Id.ToString();
                lookupField1.LookupField = "SupplierName";
                lookupField1.Update();

                //Field userField1 = list.Fields.AddFieldAsXml("<Field DisplayName='Supplier' Type='User' />", true, AddFieldOptions.DefaultValue);
                //FieldUser user1 = ctx.CastTo<FieldUser>(userField1);
                //user1.Update(); 


                //var lookupFieldXml2 = "<Field DisplayName=\"SiteSupervisor\" Type=\"Lookup\" />";
                //var lkpField2 = list.Fields.AddFieldAsXml(lookupFieldXml2, false, AddFieldOptions.AddToAllContentTypes);
                //var lookupField2 = ctx.CastTo<FieldLookup>(lkpField2);
                //lookupField2.LookupList = "51E19AE8-B48D-498A-9FEF-F7E61B366A53"; //sourceLookupList.Id.ToString();
                //lookupField2.LookupField = "SupplierName";
                //lookupField2.Update();
                /////////////////////////////////////
                Field userField1 = list.Fields.AddFieldAsXml("<Field DisplayName='SiteSupervisor' Type='User' />", true, AddFieldOptions.DefaultValue);
                FieldUser user1 = ctx.CastTo<FieldUser>(userField1);
                user1.Update(); 
               
                Field dateField1 = list.Fields.AddFieldAsXml("<Field DisplayName='Called' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                FieldDateTime date1 = ctx.CastTo<FieldDateTime>(dateField1);
                date1.Update();

                Field dateField2 = list.Fields.AddFieldAsXml("<Field DisplayName='CalledFor' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                FieldDateTime date2 = ctx.CastTo<FieldDateTime>(dateField2);
                date2.Update();

                Field dateField3 = list.Fields.AddFieldAsXml("<Field DisplayName='CalledStart' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                FieldDateTime date3 = ctx.CastTo<FieldDateTime>(dateField3);
                date3.Update();

                Field dateField4 = list.Fields.AddFieldAsXml("<Field DisplayName='Completion' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                FieldDateTime date4 = ctx.CastTo<FieldDateTime>(dateField4);
                date4.Update();

                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public Boolean createListView(ClientContext ctx, string appSettingsKey)
        {
            bool success = false;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);
    
                ViewCollection viewColl = list.Views;
                string[] viewFields = { "ID", "Checkmark", "PercentComplete", "SrNo", "CostCode", "Title", "Supplier", "SiteSupervisor", "Called", "CalledFor", "CalledStart", "Completion", "Comments" };
                ViewCreationInformation creationInfo = new ViewCreationInformation();  
                creationInfo.Title = "TasksCreated";  
                creationInfo.RowLimit = 50;  
                creationInfo.ViewFields = viewFields;  
                creationInfo.ViewTypeKind = ViewType.None;  
                creationInfo.SetAsDefaultView = true;  
                viewColl.Add(creationInfo);  
                
                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public Boolean removeColFromView(ClientContext ctx, string appSettingsKey, string viewName)
        {
            bool success = false;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);
                View view = list.Views.GetByTitle(viewName);
                ViewFieldCollection viewFields = view.ViewFields;
                viewFields.Remove("DelayId");
                view.Update();
                viewFields.Remove("JobNumber");
                view.Update();
                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public static FieldLookupValue GetLookupValue(ClientContext clientContext, string value, string lookupListName, string lookupFieldName, string lookupFieldType)
        {
            List list = null;
            FieldLookupValue lookupValue = null;

            try
            {
                list = clientContext.Web.Lists.GetByTitle(lookupListName);
                if (list != null)
                {
                    CamlQuery camlQueryForItem = new CamlQuery();
                    camlQueryForItem.ViewXml = string.Format(@"<View>
                  <Query>
                      <Where>
                         <Eq>
                             <FieldRef Name='{0}'/>
                             <Value Type='{1}'>{2}</Value>
                         </Eq>
                       </Where>
                   </Query>
            </View>", lookupFieldName, lookupFieldType, value);

                    ListItemCollection listItems = list.GetItems(camlQueryForItem);
                    clientContext.Load(listItems, items => items.Include
                                                      (listItem => listItem["ID"],
                                                       listItem => listItem[lookupFieldName]));
                    clientContext.ExecuteQuery();

                    if (listItems != null)
                    {
                        try
                        {
                            ListItem item = listItems[0];
                            lookupValue = new FieldLookupValue();
                            lookupValue.LookupId = Int32.Parse(item["ID"].ToString());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            lookupValue = new FieldLookupValue();
                            lookupValue.LookupId = 2412; 
                        }
                    }
                    //else
                    //{
                    //    lookupValue = new FieldLookupValue();
                    //    lookupValue.LookupId = 2412;
                    //}
                }
            }
            catch (Exception e)
            { Console.WriteLine(e.Message); }

            return lookupValue;
        }

        public Boolean createListItems(ClientContext ctx, DataTable calledforDatesData, string listName, List<UserValues> uValue)
        {
            bool success = false;
            try
            {
                ListItemCreationInformation itemCreateInfo = null;
                ListItem oListItem = null;
                List oList = ctx.Web.Lists.GetByTitle(listName);
                for (int i = 0; i < calledforDatesData.Rows.Count; i++)
                {
                    itemCreateInfo = new ListItemCreationInformation();
                    oListItem = oList.AddItem(itemCreateInfo);

                    oListItem["Title"] = calledforDatesData.Rows[i]["sLogisticsActivity"].ToString();
                    oListItem["Called"] =       Convert.ToDateTime( calledforDatesData.Rows[i]["d_called_fBest"].ToString()    )  ;
                    oListItem["CalledFor"] =    Convert.ToDateTime( calledforDatesData.Rows[i]["d_calledFor_fBest"].ToString() )  ;
                    oListItem["CalledStart"] =  Convert.ToDateTime( calledforDatesData.Rows[i]["d_start_fBest"].ToString()     )  ;
                    oListItem["Completion"] =  Convert.ToDateTime( calledforDatesData.Rows[i]["d_complete_fBest"].ToString()  )  ;

                    //for (int u = 0; u < uValue.Count; u++)
                    //{
                    //    UserValues uVal = uValue[u];
                    //    if (calledforDatesData.Rows[i]["sLogisticsActivity"].ToString() == uVal.Title.ToString())
                    //    {
                    //        oListItem["SiteSupervisor"] = Convert.ToDateTime(calledforDatesData.Rows[i]["Supervisor"].ToString());
                    //    }
                    //}
                    foreach (UserValues uVal in uValue)
                    {
                        if (uVal.Title.ToString() == calledforDatesData.Rows[i]["Supervisor"].ToString())
                        {
                            FieldUserValue userValue = new FieldUserValue();
                            userValue.LookupId = uVal.Id;
                            oListItem["SiteSupervisor"] = userValue;
                            break;
                        }
                    }

                    oListItem.Update();
                    ctx.ExecuteQuery();
                }
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        //Delay List
        public Boolean createDelayListColumns(ClientContext ctx, string appSettingsKey)
        {
            bool success = false;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);

                Field dateField1 = list.Fields.AddFieldAsXml("<Field DisplayName='Start' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                FieldDateTime date1 = ctx.CastTo<FieldDateTime>(dateField1);
                date1.Update();

                Field dateField2 = list.Fields.AddFieldAsXml("<Field DisplayName='To' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                FieldDateTime date2 = ctx.CastTo<FieldDateTime>(dateField2);
                date2.Update();

                var lookupFieldXml1 = "<Field DisplayName=\"Reason\" Type=\"Lookup\" />";
                var lkpField1 = list.Fields.AddFieldAsXml(lookupFieldXml1, false, AddFieldOptions.AddToAllContentTypes);
                var lookupField1 = ctx.CastTo<FieldLookup>(lkpField1);
                lookupField1.LookupList = "51E19AE8-B48D-498A-9FEF-F7E61B366A53"; //sourceLookupList.Id.ToString();
                lookupField1.LookupField = "ReasonCodesField";
                lookupField1.Update();

                //Field field3 = list.Fields.AddFieldAsXml("<Field DisplayName='Comments' Type='Text' />", true, AddFieldOptions.DefaultValue);
                //FieldText fld3 = ctx.CastTo<FieldText>(field3);
                //fld3.Update();

                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public Boolean createDelayListView(ClientContext ctx, string appSettingsKey)
        {
            bool success = false;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);

                ViewCollection viewColl = list.Views;
                string[] viewFields = { "ID", "Start", "To", "Reason", "Title" };
                ViewCreationInformation creationInfo = new ViewCreationInformation();
                creationInfo.Title = "Reason";
                creationInfo.RowLimit = 50;
                creationInfo.ViewFields = viewFields;
                creationInfo.ViewTypeKind = ViewType.None;
                creationInfo.SetAsDefaultView = true;
                viewColl.Add(creationInfo);

                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public Boolean createDelayListItems(ClientContext ctx, DataTable calledforDatesData, string listName)
        {
            bool success = false;
            try
            {
                ListItemCreationInformation itemCreateInfo = null;
                ListItem oListItem = null;
                List oList = ctx.Web.Lists.GetByTitle(listName);
                for (int i = 0; i < calledforDatesData.Rows.Count; i++)
                {
                    itemCreateInfo = new ListItemCreationInformation();
                    oListItem = oList.AddItem(itemCreateInfo);

                    oListItem["Title"] = calledforDatesData.Rows[i]["Title"].ToString();
                    oListItem["Start"] = Convert.ToDateTime(calledforDatesData.Rows[i]["Start"].ToString());
                    oListItem["To"] = Convert.ToDateTime(calledforDatesData.Rows[i]["To"].ToString());
                    oListItem["Reason"] = calledforDatesData.Rows[i]["Reason"].ToString();

                    oListItem.Update();
                    ctx.ExecuteQuery();
                }
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        //Delay Metadata List
        public Boolean createDelayMetadataListColumns(ClientContext ctx, string appSettingsKey)
        {
            bool success = false;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);

                Field field1 = list.Fields.AddFieldAsXml("<Field DisplayName='JobNum' Type='Text' />", true, AddFieldOptions.DefaultValue);
                FieldText fld1 = ctx.CastTo<FieldText>(field1);
                fld1.Update();

                Field field2 = list.Fields.AddFieldAsXml("<Field DisplayName='Client' Type='Text' />", true, AddFieldOptions.DefaultValue);
                FieldText fld2 = ctx.CastTo<FieldText>(field2);
                fld2.Update();

                Field field3 = list.Fields.AddFieldAsXml("<Field DisplayName='Delay' Type='Text' />", true, AddFieldOptions.DefaultValue);
                FieldText fld3 = ctx.CastTo<FieldText>(field3);
                fld3.Update();

                Field field4 = list.Fields.AddFieldAsXml("<Field DisplayName='Week' Type='Text' />", true, AddFieldOptions.DefaultValue);
                FieldText fld4 = ctx.CastTo<FieldText>(field4);
                fld4.Update();

                Field field5 = list.Fields.AddFieldAsXml("<Field DisplayName='Overall' Type='Text' />", true, AddFieldOptions.DefaultValue);
                FieldText fld5 = ctx.CastTo<FieldText>(field5);
                fld5.Update();

                Field field6 = list.Fields.AddFieldAsXml("<Field DisplayName='Address' Type='Text' />", true, AddFieldOptions.DefaultValue);
                FieldText fld6 = ctx.CastTo<FieldText>(field6);
                fld6.Update();

                Field field7 = list.Fields.AddFieldAsXml("<Field DisplayName='Forcast' Type='Text' />", true, AddFieldOptions.DefaultValue);
                FieldText fld7 = ctx.CastTo<FieldText>(field7);
                fld7.Update();

                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public Boolean createDelayMetadataListView(ClientContext ctx, string appSettingsKey)
        {
            bool success = false;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);

                ViewCollection viewColl = list.Views;
                string[] viewFields = { "ID", "JobNum", "Client", "Delay", "Week", "Overall", "Address", "Forcast" };
                ViewCreationInformation creationInfo = new ViewCreationInformation();
                creationInfo.Title = "JobsData";
                creationInfo.RowLimit = 50;
                creationInfo.ViewFields = viewFields;
                creationInfo.ViewTypeKind = ViewType.None;
                creationInfo.SetAsDefaultView = true;
                viewColl.Add(creationInfo);

                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public Boolean createDelayMetadataListItems(ClientContext ctx, DataTable calledforDatesData, string listName)
        {
            bool success = false;
            try
            {
                ListItemCreationInformation itemCreateInfo = null;
                ListItem oListItem = null;
                List oList = ctx.Web.Lists.GetByTitle(listName);
                for (int i = 0; i < calledforDatesData.Rows.Count; i++)
                {
                    itemCreateInfo = new ListItemCreationInformation();
                    oListItem = oList.AddItem(itemCreateInfo);

                    oListItem["JobNum"] = calledforDatesData.Rows[i]["JobNum"].ToString();
                    oListItem["Client"] = calledforDatesData.Rows[i]["Client"].ToString();
                    oListItem["Delay"] = calledforDatesData.Rows[i]["Delay"].ToString();
                    oListItem["Week"] = calledforDatesData.Rows[i]["Week"].ToString();
                    oListItem["Overall"] = calledforDatesData.Rows[i]["Overall"].ToString();
                    oListItem["Address"] = calledforDatesData.Rows[i]["Address"].ToString();
                    oListItem["Forcast"] = calledforDatesData.Rows[i]["Forcast"].ToString();
                    
                    oListItem.Update();
                    ctx.ExecuteQuery();
                }
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        // Adding WF
        public Guid getListGuid(ClientContext ctx, string appSettingsKey)
        {
            Guid success = Guid.Empty;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);

                ctx.Load(list);
                ctx.ExecuteQuery();
                //Guid id = list.Id;
                success = list.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public void addWorkflowSubscription(ClientContext clientContext, string listname, Guid targetListGuid)
        {            
            //Name of the SharePoint2013 List Workflow
            string workflowName = "SplitTask WOrkflow";
            //GUID of list on which to create the subscription (association);

            //Guid targetListGuid = new Guid("1145D77B-EB61-4C54-87A7-5B8FEEE05F07"); //fc50af29-8ae5-4303-bad1-213151818215
            //Name of the new Subscription (association)
            string newSubscriptionName = "WF-" + listname;   
            string workflowHistoryListID = ConfigurationManager.AppSettings.Get("workflowHistoryListID");
            string taskListID = ConfigurationManager.AppSettings.Get("taskListID");
            // SplitTask WOrkflow Tasks List :- 474B006B-72FC-4F0A-8FCD-48EC7244C56A
            // Workflow History List :-334FB71F-573A-40DB-A45B-05228BE1499D
            // Old Task List:- D631230F-23C1-41FA-AE02-7714BE79ED0D
            // Old WF History List :- A0D1C88E-3F4A-44EF-AE1D-1B9450D3652B 

                Web web = clientContext.Web;
                //Workflow Services Manager which will handle all the workflow interaction.
                WorkflowServicesManager wfServicesManager = new WorkflowServicesManager(clientContext, web);
                //Deployment Service which holds all the Workflow Definitions deployed to the site
                WorkflowDeploymentService wfDeploymentService = wfServicesManager.GetWorkflowDeploymentService();
                //Get all the definitions from the Deployment Service, or get a specific definition using the GetDefinition method.
                WorkflowDefinitionCollection wfDefinitions = wfDeploymentService.EnumerateDefinitions(false);

                clientContext.Load(wfDefinitions, wfDefs => wfDefs.Where(wfd => wfd.DisplayName == workflowName));
                clientContext.ExecuteQuery();

                WorkflowDefinition wfDefinition = wfDefinitions.First();

                //The Subscription service is used to get all the Associations currently on the SPSite
                WorkflowSubscriptionService wfSubscriptionService = wfServicesManager.GetWorkflowSubscriptionService();
                //The subscription (association)
                WorkflowSubscription wfSubscription = new WorkflowSubscription(clientContext);
                wfSubscription.DefinitionId = wfDefinition.Id;
                wfSubscription.Enabled = true;
                wfSubscription.Name = newSubscriptionName;

                var startupOptions = new List<string>();
                // automatic start
                startupOptions.Add("ItemAdded");
                startupOptions.Add("ItemUpdated");
                // manual start
                startupOptions.Add("WorkflowStart");
                // set the workflow start settings
                wfSubscription.EventTypes = startupOptions;
                // set the associated task and history lists
                wfSubscription.SetProperty("HistoryListId", workflowHistoryListID);

                wfSubscription.SetProperty("TaskListId", taskListID);
                //Create the Association
                wfSubscriptionService.PublishSubscriptionForList(wfSubscription, targetListGuid);

                clientContext.ExecuteQuery();
        }

        //New
        public ListItemCollection getListDataVal(ClientContext ctx, string appSettingsKey)
        {
            ListItemCollection getListItemsCollection = null;
            try
            {
                //GlobalLogic gl = new GlobalLogic();
                //ClientContext ctx = gl.ConnectSP(tenant, userName, passwordString);

                List getList = ctx.Web.Lists.GetByTitle(appSettingsKey);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query></Query></View>";
                ListItemCollection getListItemsCol = getList.GetItems(camlQuery);
                ctx.Load(getListItemsCol);
                ctx.ExecuteQuery();

                if (getListItemsCol != null && getListItemsCol.Count > 0)
                {
                    getListItemsCollection = getListItemsCol;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return getListItemsCollection;
        }

        //public DataTable getSPListDataTable(ClientContext ctx, string appSettingsKey, GlobalLogic gl, string uniqueColName)
        //{
        //    DataTable dtSPMetadatListItems = new DataTable();
        //    try
        //    {
        //        ListItemCollection getSPMetadatListItems = gl.getListDataVal(ctx, appSettingsKey);
        //        //dtSPMetadatListItems = new DataTable();

        //        dtSPMetadatListItems.Columns.Add(uniqueColName);
        //        if (getSPMetadatListItems != null)
        //        {
        //            foreach (ListItem listItemsCol in getSPMetadatListItems)
        //            {
        //                DataRow dr = dtSPMetadatListItems.NewRow();
        //                dr[uniqueColName] = listItemsCol[uniqueColName];
        //                dtSPMetadatListItems.Rows.Add(dr);
        //            }
        //        }
        //        else
        //        {
        //            DataRow dr = dtSPMetadatListItems.NewRow();
        //            dr[uniqueColName] = "";
        //            dtSPMetadatListItems.Rows.Add(dr);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    return dtSPMetadatListItems;
        //}

        public DataTable getSPListDataTable(ClientContext ctx, string appSettingsKey, GlobalLogic gl, List<ColumnTypes> colNameUpdate)
        {
            DataTable dtSPMetadatListItems = new DataTable();
            try
            {
                ListItemCollection getSPMetadatListItems = gl.getListDataVal(ctx, appSettingsKey);

                foreach (ColumnTypes colName in colNameUpdate)
                {
                    dtSPMetadatListItems.Columns.Add(colName.columnName.ToString());
                }

                if (getSPMetadatListItems != null)
                {
                    foreach (ListItem listItemsCol in getSPMetadatListItems)
                    {
                        DataRow dr = dtSPMetadatListItems.NewRow();
                        foreach (ColumnTypes colName in colNameUpdate)
                        {
                            dr[colName.columnName.ToString()] = listItemsCol[colName.columnName.ToString()];
                        }
                        
                        dtSPMetadatListItems.Rows.Add(dr);
                    }
                }
                else
                {
                    DataRow dr = dtSPMetadatListItems.NewRow();
                    foreach (ColumnTypes colName in colNameUpdate)
                    {
                        dr[colName.columnName.ToString()] = "";
                    }                    
                    dtSPMetadatListItems.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return dtSPMetadatListItems;
        }


        public Boolean createListColumnsGeneric(ClientContext ctx, string appSettingsKey, List<ColumnTypes> colNames)
        {
            bool success = false;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);

                //for (int u = 0; u < uValue.Count; u++)
                //{
                //    UserValues uVal = uValue[u];
                //    if (calledforDatesData.Rows[i]["sLogisticsActivity"].ToString() == uVal.Title.ToString())
                //    {
                //        oListItem["SiteSupervisor"] = Convert.ToDateTime(calledforDatesData.Rows[i]["Supervisor"].ToString());
                //    }
                //}
                foreach (ColumnTypes colName in colNames)
                {
                    if (colName.columnType.ToString() == "1")
                    {
                        Field field = list.Fields.AddFieldAsXml("<Field DisplayName='" + colName.columnName.ToString() + "' Type='Text' />", true, AddFieldOptions.DefaultValue);
                        FieldText fld = ctx.CastTo<FieldText>(field);                        
                        fld.Update();
                    }
                    else if (colName.columnType.ToString() == "2")
                    {
                        Field field = list.Fields.AddFieldAsXml("<Field DisplayName='" + colName.columnName.ToString() + "' Type='Number' />", true, AddFieldOptions.DefaultValue);
                        FieldNumber fld = ctx.CastTo<FieldNumber>(field);
                        fld.Update();
                    }
                    else if (colName.columnType.ToString() == "3")
                    {
                        var lookupFieldXml1 = "<Field DisplayName='" + colName.columnName.ToString() + "' Type=\"Lookup\" />";
                        var field = list.Fields.AddFieldAsXml(lookupFieldXml1, false, AddFieldOptions.AddToAllContentTypes);
                        var fld = ctx.CastTo<FieldLookup>(field);
                        fld.LookupList = colName.lookupListGuid.ToString(); //"51E19AE8-B48D-498A-9FEF-F7E61B366A53"; //sourceLookupList.Id.ToString();
                        fld.LookupField = colName.lookupColumnName.ToString(); //"SupplierName";
                        fld.Update();
                    }
                    else if (colName.columnType.ToString() == "4")
                    {
                        Field field = list.Fields.AddFieldAsXml("<Field DisplayName='" + colName.columnName.ToString() + "' Type='User' />", true, AddFieldOptions.DefaultValue);
                        FieldUser fld = ctx.CastTo<FieldUser>(field);
                        fld.Update();
                    }
                    else if (colName.columnType.ToString() == "5")
                    {
                        Field field = list.Fields.AddFieldAsXml("<Field DisplayName='" + colName.columnName.ToString() + "' Type='DateTime' />", true, AddFieldOptions.DefaultValue);
                        FieldDateTime fld = ctx.CastTo<FieldDateTime>(field);
                        fld.Update();
                    }
                    else if (colName.columnType.ToString() == "6")
                    {
                        Field field = list.Fields.AddFieldAsXml("<Field DisplayName='" + colName.columnName.ToString() + "' Type='URL' />", true, AddFieldOptions.DefaultValue);
                        FieldUrl fld = ctx.CastTo<FieldUrl>(field);
                        fld.Update();
                    }
                }                

                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public Boolean createListViewGeneric(ClientContext ctx, string appSettingsKey, List<ColumnTypes> colNames)
        {
            bool success = false;
            try
            {
                Web web = ctx.Web;
                List list = web.Lists.GetByTitle(appSettingsKey);

                ViewCollection viewColl = list.Views;

                string[] viewFields = new string[colNames.Count+3];
                viewFields[0] = "Checkmark";
                viewFields[1] = "PercentComplete";
                viewFields[2] = "Title";
                for (int u = 3; u < colNames.Count+3; u++)
                {
                    ColumnTypes uVal = colNames[u-3];
                    viewFields[u] = uVal.columnName.ToString();
                }
                
                //string[] viewFields = { "ID", "Checkmark", "PercentComplete", "SrNo", "CostCode", "Title", "Supplier", "SiteSupervisor", "Called", "CalledFor", "CalledStart", "Completion", "Comments" };
                ViewCreationInformation creationInfo = new ViewCreationInformation();
                creationInfo.Title = "TasksCreated";
                creationInfo.RowLimit = 50;
                creationInfo.ViewFields = viewFields;
                creationInfo.ViewTypeKind = ViewType.None;
                creationInfo.SetAsDefaultView = true;
                viewColl.Add(creationInfo);

                ctx.ExecuteQuery();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

        //public Boolean createListItemsGeneric(ClientContext ctx, DataTable dataSQL, DataTable dataSP, string listName, List<ColumnTypes> colNames, List<ColumnTypes> colNamesUpdate)
        //{
        //    bool success = false;
        //    try
        //    {
        //        ListItemCreationInformation itemCreateInfo = null;
        //        ListItem oListItem = null;
        //        List oList = ctx.Web.Lists.GetByTitle(listName);
        //        for (int i = 0; i < dataSQL.Rows.Count; i++)
        //        {
        //            itemCreateInfo = new ListItemCreationInformation();
        //            oListItem = oList.AddItem(itemCreateInfo);

        //            foreach (ColumnTypes colName in colNames)
        //            {
        //                if (colName.columnType.ToString() == "1")
        //                {
        //                    oListItem[colName.columnName] = dataSQL.Rows[i][colName.columnName].ToString();
        //                }
        //                else if (colName.columnType.ToString() == "2")
        //                {
        //                    oListItem[colName.columnName] = Int32.Parse(dataSQL.Rows[i][colName.columnName].ToString());
        //                }
        //                else if (colName.columnType.ToString() == "5")
        //                {
        //                    oListItem[colName.columnName] = Convert.ToDateTime(dataSQL.Rows[i][colName.columnName].ToString());
        //                }
        //                else if (colName.columnType.ToString() == "6")
        //                {
        //                    FieldUrlValue url = new FieldUrlValue();
        //                    url.Url = dataSQL.Rows[i][colName.columnName].ToString();
        //                    url.Description = dataSQL.Rows[i][colName.columnName].ToString();
        //                    //oListItem["URL"] = url;
        //                    oListItem[colName.columnName] = url;
        //                }
        //            }

        //            oListItem.Update();
        //            ctx.ExecuteQuery();
        //        }
        //        success = true;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    return success;
        //}

        public Boolean createListItemsGeneric(ClientContext ctx, DataTable dataSQL, DataTable dataSP, string listName, List<ColumnTypes> colNames, List<ColumnTypes> colNamesUpdate, string uniqueCol, List<UserValues> uValue)
        {
            bool success = false;
            try
            {
                ListItemCreationInformation itemCreateInfo = null;
                ListItem oListItem = null;
                List oList = ctx.Web.Lists.GetByTitle(listName);
                string linqQuery, colValues;
                for (int i = 0; i < dataSQL.Rows.Count; i++)
                {
                    linqQuery = "";
                    colValues = "";
                    //DataRow[] drExists = dataSP.Select("JobNum = '" + dataSQL.Rows[i]["JobNum"].ToString() + "'");
                    DataRow[] drExists = dataSP.Select(uniqueCol + " = '" + dataSQL.Rows[i][uniqueCol].ToString() + "'");
                    if (drExists != null && drExists.Length > 0)
                    {
                        for (int u = 1; u < colNamesUpdate.Count; u++)
                        {
                            ColumnTypes colName = colNamesUpdate[u];
                            colValues += dataSQL.Rows[i][colName.columnName.ToString()].ToString();
                            if (colName.columnType.ToString() == "1")
                            {
                                linqQuery += colName.columnName.ToString() + " = '" + dataSQL.Rows[i][colName.columnName.ToString()].ToString() + "'";
                                // oListItem[colName.columnName] = dataSQL.Rows[i][colName.columnName].ToString();
                            }
                            else if (colName.columnType.ToString() == "2")
                            {
                                linqQuery += colName.columnName.ToString() + " = " + Int32.Parse(dataSQL.Rows[i][colName.columnName.ToString()].ToString());
                                //oListItem[colName.columnName] = Int32.Parse(dataSQL.Rows[i][colName.columnName].ToString());
                            }
                            else if (colName.columnType.ToString() == "3")
                            {
                                //linqQuery += colName.columnName.ToString() + " = '" + dataSQL.Rows[i][colName.columnName.ToString()].ToString() + "'";
                                //linqQuery += colName.columnName.ToString() + " = " +  Convert.ToDateTime((dataSQL.Rows[i][colName.columnName.ToString()].ToString()));
                                linqQuery += colName.columnName.ToString() + " = '" + dataSQL.Rows[i][colName.columnName.ToString()].ToString() + "'";
                            }
                            else if (colName.columnType.ToString() == "5")
                            {
                                //linqQuery += colName.columnName.ToString() + " = '" + dataSQL.Rows[i][colName.columnName.ToString()].ToString() + "'";
                                //linqQuery += colName.columnName.ToString() + " = " +  Convert.ToDateTime((dataSQL.Rows[i][colName.columnName.ToString()].ToString()));
                                linqQuery += colName.columnName.ToString() + " = " + Convert.ToDateTime(dataSQL.Rows[i][colName.columnName].ToString());
                            }
                            if (u != colNamesUpdate.Count - 1)
                            {
                                linqQuery += " AND ";
                                colValues += " AND ";
                            }
                        }
                        DataRow[] drExists1 = dataSP.Select(linqQuery);
                        if (drExists1 != null && drExists1.Length > 0)
                        {
                            Console.WriteLine("Found - " + dataSQL.Rows[i][uniqueCol].ToString());
                        }
                        else
                        {
                            try
                            {
                                Console.WriteLine("Updating - " + dataSQL.Rows[i][uniqueCol].ToString());
                                oListItem = oList.GetItemById(drExists[0].ItemArray[0].ToString());
                                //for (int j = 1; j < colNamesUpdate.Count; j++) // Skipping j=0 for ID column defined in Excel
                                //{
                                //    ColumnTypes colName = colNamesUpdate[j];
                                //    oListItem[colName.columnName.ToString()] = dataSQL.Rows[i][colName.columnName.ToString()].ToString();
                                //}
                                for (int u = 1; u < colNamesUpdate.Count; u++)
                                {
                                    ColumnTypes colNameUpdate = colNamesUpdate[u];
                                    if (colNameUpdate.columnType.ToString() == "1")
                                    {
                                        oListItem[colNameUpdate.columnName.ToString()] = dataSQL.Rows[i][colNameUpdate.columnName.ToString()].ToString();
                                    }
                                    else if (colNameUpdate.columnType.ToString() == "2")
                                    {
                                        oListItem[colNameUpdate.columnName.ToString()] = Int32.Parse(dataSQL.Rows[i][colNameUpdate.columnName.ToString()].ToString());
                                    }
                                    else if (colNameUpdate.columnType.ToString() == "5")
                                    {
                                        oListItem[colNameUpdate.columnName.ToString()] = Convert.ToDateTime(dataSQL.Rows[i][colNameUpdate.columnName.ToString()].ToString());
                                    }
                                }
                                oListItem.Update();
                                ctx.ExecuteQuery();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            Console.WriteLine("Inserting - " + dataSQL.Rows[i][uniqueCol].ToString());
                            itemCreateInfo = new ListItemCreationInformation();
                            ListItem oListItem1 = oList.AddItem(itemCreateInfo);
                            foreach (ColumnTypes colName in colNames)
                            {
                                if (colName.columnType.ToString() == "1")
                                {
                                    oListItem1[colName.columnName] = dataSQL.Rows[i][colName.columnName].ToString();
                                }
                                else if (colName.columnType.ToString() == "2")
                                {
                                    oListItem1[colName.columnName] = Int32.Parse(dataSQL.Rows[i][colName.columnName].ToString());
                                }
                                else if (colName.columnType.ToString() == "3")
                                {
                                    //oListItem1[colName.columnName] = GetLookupValue(ctx, dataSQL.Rows[i][colName.columnName].ToString(), colName.lookupColumnName, colName.lookupColumnName, "Text");
                                    oListItem1[colName.columnName] = GetLookupValue(ctx, dataSQL.Rows[i][colName.columnName].ToString(), colName.lookupListName, colName.lookupColumnName, "Text");
                                }
                                else if (colName.columnType.ToString() == "4")
                                {
                                    foreach (UserValues uVal in uValue)
                                    {
                                        if (uVal.Title.ToString() == dataSQL.Rows[i][colName.columnName].ToString())
                                        {
                                            FieldUserValue userValue = new FieldUserValue();
                                            userValue.LookupId = uVal.Id;
                                            oListItem1["Supervisor"] = userValue;
                                            break;
                                        }
                                    }
                                }
                                else if (colName.columnType.ToString() == "5")
                                {
                                    //oListItem1[colName.columnName] = Convert.ToDateTime(dataSQL.Rows[i][colName.columnName].ToString());
                                    if (dataSQL.Rows[i][colName.columnName].ToString() == "")
                                        oListItem1[colName.columnName] = null;
                                    else
                                        oListItem1[colName.columnName] = Convert.ToDateTime(dataSQL.Rows[i][colName.columnName].ToString());
                                }
                                else if (colName.columnType.ToString() == "6")
                                {
                                    FieldUrlValue url = new FieldUrlValue();
                                    url.Url = dataSQL.Rows[i][colName.columnName].ToString();
                                    url.Description = dataSQL.Rows[i][colName.columnName].ToString();
                                    //oListItem["URL"] = url;
                                    oListItem1[colName.columnName] = url;
                                }
                            }
                            oListItem1.Update();
                            ctx.ExecuteQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return success;
        }

    }


    public class UserValues
    {
        //Console.WriteLine(usr.Email + "---" + usr.Id + "---" + usr.LoginName + "---" + usr.Title + "---" + usr.UserId);
        public string Email { get; set; }
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string Title { get; set; }
    }

    public class ColumnTypes
    {
        //Console.WriteLine(usr.Email + "---" + usr.Id + "---" + usr.LoginName + "---" + usr.Title + "---" + usr.UserId);
        public string columnName { get; set; }
        public string columnType { get; set; }
        public bool isLookup { get; set; }
        public string lookupListGuid { get; set; }
        public string lookupColumnName { get; set; }
        public string lookupListName { get; set; }
    }

}

