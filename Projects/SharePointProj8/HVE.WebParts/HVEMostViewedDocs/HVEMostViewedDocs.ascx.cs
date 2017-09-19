using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Text;
using System.Collections.Generic;
using System.Data;

namespace Proj1.HVE.WebParts.HVEMostViewedDocs
{
    [ToolboxItemAttribute(false)]
    public partial class HVEMostViewedDocs : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public HVEMostViewedDocs()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }
        public StringBuilder htmlStr = new StringBuilder("This is string builder for Most Viewed Docs <br><br>");
        string var1, var2, var3, var4;
        protected void Page_Load(object sender, EventArgs e)
        {
            Page_Load1();
            //Page_Load2();
            //Page_Load3();
        }

        protected void Page_Load1()
        {
            #region t1
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    SPSite site = new SPSite("http://win-njfp7te48bn/sites/HVEDev");
                    using (SPWeb web = site.OpenWeb())
                    {
                        lbltest.Text += "<br/>" + web.Title.ToString();
                        SPList list = web.Lists["Documents"]; //Documents           
                        SPListItemCollection coll = list.GetItems();
                        Dictionary<string, int> dictionary = new Dictionary<string, int>();
                        foreach (SPListItem item in coll)
                        {
                            SPAuditQuery spQuery = new SPAuditQuery(site);
                            spQuery.RestrictToListItem(item);
                            SPAuditEntryCollection auditCol = site.Audit.GetEntries(spQuery);

                            string docName = "";
                            int counter = 0;
                            foreach (SPAuditEntry entry in auditCol)
                            {
                                if (entry.ItemType == SPAuditItemType.Document && entry.Event == SPAuditEventType.View)
                                {
                                    try
                                    {
                                        var1 = entry.DocLocation.Substring(entry.DocLocation.LastIndexOf("/"));
                                        var2 = var1.Substring(var1.LastIndexOf("/"));
                                        var3 = var2.Substring(1);
                                        var4 = var3.Substring(var3.LastIndexOf('.') + 1);
                                        if (var4 != "aspx")
                                        {
                                            if (entry.EventSource == SPAuditEventSource.SharePoint)
                                            {
                                                if (docName != var3)
                                                {
                                                    docName = var3;
                                                    counter = 1;
                                                    dictionary.Add(var3, 1);
                                                }
                                                else
                                                {
                                                    if (dictionary.TryGetValue(var3, out counter))
                                                    {
                                                        dictionary[var3] = counter + 1;
                                                    }
                                                    counter = counter + 1;
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ee)
                                    {
                                        Label1.Text = ee.Message;
                                    }

                                }
                            }
                        }
                        foreach (KeyValuePair<string, int> pair in dictionary)
                        {
                            htmlStr.Append(("Document Name: " + pair.Key.ToString() + "  -  " + "Views Count: " + pair.Value.ToString()) + "<br>");
                        }

                    }

                });
            }
            catch (Exception eee)
            {
                Console.WriteLine(eee.Message);
            }
            LiteralText.Text = htmlStr.ToString();
            #endregion
        }
        protected void Page_Load2()
        {
            //#region t1
            //try
            //{
            //    SPSecurity.RunWithElevatedPrivileges(delegate()
            //    {
            //        SPSite site = new SPSite("http://win-njfp7te48bn/sites/HVEDev");
            //        using (SPWeb web = site.OpenWeb())
            //        {
            //            lbltest.Text += "<br/>" + web.Title.ToString();
            //            SPList list = web.Lists["DocLib1"]; //Documents           
            //            SPListItemCollection coll = list.GetItems();
            //            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            //            foreach (SPListItem item in coll)
            //            {
            //                SPAuditQuery spQuery = new SPAuditQuery(site);
            //                spQuery.RestrictToListItem(item);
            //                SPAuditEntryCollection auditCol = site.Audit.GetEntries(spQuery);

            //                string docName = "";
            //                int counter = 0;
            //                foreach (SPAuditEntry entry in auditCol)
            //                {
            //                    if (entry.ItemType == SPAuditItemType.Document && entry.Event == SPAuditEventType.View)
            //                    {
            //                        try
            //                        {
            //                            var1 = entry.DocLocation.Substring(entry.DocLocation.LastIndexOf("/"));
            //                            var2 = var1.Substring(var1.LastIndexOf("/"));
            //                            var3 = var2.Substring(1);
            //                            var4 = var3.Substring(var3.LastIndexOf('.') + 1);
            //                            if (var4 != "aspx")
            //                            {
            //                                if (entry.EventSource == SPAuditEventSource.SharePoint)
            //                                {
            //                                    if (docName != var3)
            //                                    {
            //                                        docName = var3;
            //                                        counter = 1;
            //                                        dictionary.Add(var3, 1);
            //                                    }
            //                                    else
            //                                    {
            //                                        if (dictionary.TryGetValue(var3, out counter))
            //                                        {
            //                                            dictionary[var3] = counter + 1;
            //                                        }
            //                                        counter = counter + 1;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        catch (Exception ee)
            //                        {
            //                            Label1.Text = ee.Message;
            //                        }

            //                    }
            //                }
            //            }
            //            foreach (KeyValuePair<string, int> pair in dictionary)
            //            {
            //                htmlStr.Append(("Document Name: " + pair.Key.ToString() + "  -  " + "Views Count: " + pair.Value.ToString()) + "<br>");
            //            }

            //        }

            //    });
            //}
            //catch (Exception eee)
            //{
            //    Console.WriteLine(eee.Message);
            //}
            //LiteralText.Text = htmlStr.ToString();
            //#endregion
        }
        protected void Page_Load3()
        {
            //#region t1
            //try
            //{
            //    SPSecurity.RunWithElevatedPrivileges(delegate()
            //    {
            //        SPSite site = new SPSite("http://win-njfp7te48bn/sites/HVEDev");
            //        using (SPWeb web = site.OpenWeb())
            //        {
            //            lbltest.Text += "<br/>" + web.Title.ToString();
            //            SPList list = web.Lists["DocLib2"]; //Documents           
            //            SPListItemCollection coll = list.GetItems();
            //            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            //            foreach (SPListItem item in coll)
            //            {
            //                SPAuditQuery spQuery = new SPAuditQuery(site);
            //                spQuery.RestrictToListItem(item);
            //                SPAuditEntryCollection auditCol = site.Audit.GetEntries(spQuery);

            //                string docName = "";
            //                int counter = 0;
            //                foreach (SPAuditEntry entry in auditCol)
            //                {
            //                    if (entry.ItemType == SPAuditItemType.Document && entry.Event == SPAuditEventType.View)
            //                    {
            //                        try
            //                        {
            //                            var1 = entry.DocLocation.Substring(entry.DocLocation.LastIndexOf("/"));
            //                            var2 = var1.Substring(var1.LastIndexOf("/"));
            //                            var3 = var2.Substring(1);
            //                            var4 = var3.Substring(var3.LastIndexOf('.') + 1);
            //                            if (var4 != "aspx")
            //                            {
            //                                if (entry.EventSource == SPAuditEventSource.SharePoint)
            //                                {
            //                                    if (docName != var3)
            //                                    {
            //                                        docName = var3;
            //                                        counter = 1;
            //                                        dictionary.Add(var3, 1);
            //                                    }
            //                                    else
            //                                    {
            //                                        if (dictionary.TryGetValue(var3, out counter))
            //                                        {
            //                                            dictionary[var3] = counter + 1;
            //                                        }
            //                                        counter = counter + 1;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        catch (Exception ee)
            //                        {
            //                            Label1.Text = ee.Message;
            //                        }

            //                    }
            //                }
            //            }
            //            foreach (KeyValuePair<string, int> pair in dictionary)
            //            {
            //                htmlStr.Append(("Document Name: " + pair.Key.ToString() + "  -  " + "Views Count: " + pair.Value.ToString()) + "<br>");
            //            }

            //        }

            //    });
            //}
            //catch (Exception eee)
            //{
            //    Console.WriteLine(eee.Message);
            //}
            //LiteralText.Text = htmlStr.ToString();
            //#endregion
        }              
    }
}
