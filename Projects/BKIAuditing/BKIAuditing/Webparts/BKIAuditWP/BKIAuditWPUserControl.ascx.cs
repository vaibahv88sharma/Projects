using Microsoft.SharePoint;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace BKIAuditing.Webparts.BKIAuditWP
{
    public partial class BKIAuditWPUserControl : UserControl
    {
        // CheckOut,	CheckIn,	View,	Delete,	Update,	ProfileChange,	ChildDelete,	
        // SchemaChange,	Undelete,	Workflow,	Copy,	Move,	AuditMaskChange,	
        // Search,	ChildMove,	SecGroupCreate,	SecGroupDelete,	SecGroupMemberAdd,	
        // SecGroupMemberDel,	SecRoleDefCreate,	SecRoleDefDelete,	SecRoleDefModify,	SecRoleDefBreakInherit,	
        // SecRoleBindUpdate,	SecRoleBindInherit,	SecRoleBindBreakInherit,	EventsDeleted,	Custom

        protected void Page_Load(object sender, EventArgs e)
        {
            //string documentUrl = "http://spwfe03p-bro/sites/StaffPortal/TestWork1/Pages/Test.aspx";
            string documentUrl = "http://staffportal.myselfserve.com.au/sites/StaffPortal/TestWork1/Pages/Test-Bendigo-TAFE%E2%80%99s-2018-Apprentice-and-Industry-Awards.aspx";
            string siteUrl = "http://spwfe03p-bro/sites/staffportal/TestWork1";
            //string listName = "Auditing Details";//"Pages";
            string listName = "Pages";
            SPList listObj;
            try
            {
                using (SPSite site = new SPSite(documentUrl))//siteUrl
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        string DocLibName = documentUrl.Substring(web.Url.Length + 1).Substring(0, documentUrl.Substring(web.Url.Length + 1).IndexOf("/"));
                        string DocNameWithExtension = documentUrl.Substring(documentUrl.LastIndexOf("/") + 1);
                        string DocName = DocNameWithExtension.Substring(0, DocNameWithExtension.LastIndexOf("."));
                        SPListItem itemObj =  QueryListItem(web, DocLibName);
                        //listObj = (SPDocumentLibrary)web.Lists[listName];

                        //string s = web.Url;
                        //string a = documentUrl.Substring(web.Url.Length + 1, documentUrl.Length - web.Url.Length);    //  documentUrl.Substring(web.Url.Length + 1);

                        SPAuditQuery query = new SPAuditQuery(site);
                        //query.RestrictToList(listObj);
                        //query.RestrictToListItem(itemObj);    
                        //query.AddEventRestriction(SPAuditEventType.View);
                        //query.AddEventRestriction(SPAuditEventType.Search);
                        query.SetRangeStart(DateTime.Now.AddDays(-15));
                        query.SetRangeEnd(DateTime.Now);
                        SPAuditEntryCollection auditCol = web.Audit.GetEntries(query);
                        foreach (SPAuditEntry audit in auditCol)
                        {
                            string docName = audit.DocLocation;  // audit.DocLocation = documentUrl.Substring(documentUrl.LastIndexOf("/")+1)
                            int userID = audit.UserId;
                            string userEmail = (web.AllUsers.GetByID(audit.UserId)).Email;
                            string userName = (web.AllUsers.GetByID(audit.UserId)).Name;
                            string ItemID = Convert.ToString(audit.ItemId);
                            string ItemType = Convert.ToString(audit.ItemType);
                            string EventType = Convert.ToString(audit.Event);
                            DateTime OccuredDate = audit.Occurred;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Catch error in to ULS log.    
            }
        }

        protected SPListItem QueryListItem(SPWeb web, string docLibName) {
            SPQuery query = new SPQuery();
            query.Query = @"<Where>
                              <Eq>
                                 <FieldRef Name='Title' />
                                 <Value Type='Text'>Demo</Value>
                              </Eq>
                           </Where>";
            //string listUrl = "http://spwfe04p-bro/sites/staffportal/Pages";//web.ServerRelativeUrl + "/lists/tasks";
            SPList list = (SPDocumentLibrary)web.GetList(docLibName);
            SPListItemCollection items = list.GetItems(query);
            //foreach (SPListItem item in items)
            //{
            //    //  item.Url
            //    //    item[SPBuiltInFieldId.EncodedAbsUrl]
            //    
            //
            //    Console.WriteLine("{0,-25}  {1,-20}  {2,-25}  {3}",
            //       item["AssignedTo"], item["LinkTitle"], item["DueDate"], item["Priority"]);                
            //}
            return items[0];
        }

        protected void ExportToCSV()
        {
            try
            {
                //string filename = string.Empty;
                //filename = string.Format(Year + "Round" + Round + ".csv");
                //SPFolder myFolder = mySite.Folders[rebateSite.Url + "/" + strCSVFilesList + "/"];
                //MemoryStream mstream = new MemoryStream();
                //StreamWriter sw = new StreamWriter(mstream);
                //// fetch data from document library using spquery
                //SPListItemCollection myItems = myList.GetItems(myQuery);
                //if (myItems.Count > 0)
                //{
                //    foreach (SPListItem myItem in myItems)
                //    {
                //        //reset variables
                //        itemLine = string.Empty;
                //        itemLine2 = string.Empty;
                //        payID = null;
                //        dblClaimAmt = 0;
                //        dblClaimAmt = (Double)myItem["ClaimAmt"];
                //        payID = (String)myItem["PayID"];
                //        itemLine = payID + "," + (-1 * dblClaimAmt);
                //        itemLine2 = payID + "," + dblClaimAmt;
                //        sw.Write(itemLine);
                //        sw.Write(itemLine2);
                //    }// end of for each
                //
                //    sw.Flush();
                //    byte[] contents = new byte[mstream.Length];
                //    mstream.Read(contents, 0, (int)mstream.Length);
                //    SPFile csvFile = claimsFolder.Files.Add(filename, contents, true); //write to document library
                //    mstream.Close();
                //}
            }
            catch (Exception ex) {
            }
        }
    }
}
