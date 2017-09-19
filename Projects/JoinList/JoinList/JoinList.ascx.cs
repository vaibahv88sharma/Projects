using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Linq;
using Microsoft.SharePoint;

namespace JoinList.JoinList
{
    [ToolboxItemAttribute(false)]
    public partial class JoinList : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public JoinList()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            {
                SPWeb web = site.OpenWeb();
                SPQuery query = new SPQuery();

                #region City List
                /*
                query.Query = @"<Where>
                                  <Eq>
                                     <FieldRef Name='Name' />
                                     <Value Type='Text'>Suresh</Value>
                                  </Eq>
                               </Where>";
                query.Joins = @"<Join Type = 'LEFT' ListAlias='City List'>                
                                 <Eq>
                                     <FieldRef Name='City' RefType='Id'/>
                                     <FieldRef Name='City List' RefType='ID'/>
                                 </Eq>
                                </Join>";
                query.ProjectedFields = @"<Field Name='ZipCode' Type='Lookup' List='City List' ShowField='ZipCode' />";
                 * */
                #endregion City List

                #region City
              //  /*
                query.Query = @"<Where>
                                  <Eq>
                                     <FieldRef Name='Name' />
                                     <Value Type='Text'>Rajendar</Value>
                                  </Eq>
                               </Where>";
                query.Joins = @"<Join Type = 'INNER' ListAlias='City'>                
                                 <Eq>
                                     <FieldRef Name='City' RefType='Id'/>
                                     <FieldRef List='City' Name='ID'/>
                                 </Eq>
                                </Join>
                                <Join Type = 'INNER' ListAlias='CountryName'>                
                                 <Eq>
                                     <FieldRef List='City' Name='ResidenceCountry' RefType='Id'/>
                                     <FieldRef List='CountryName' Name='ID'/>
                                 </Eq>
                                </Join>";
                query.ProjectedFields = @"<Field Name='ZipCode' Type='Lookup' List='City' ShowField='ZipCode' />
                                          <Field Name='ResidenceCountry' Type='Lookup' List='CountryName' ShowField='ResidenceCountry' />";
            //     */
                #endregion City List                

                SPListItemCollection items = web.Lists["Employee"].GetItems(query);
                foreach (SPListItem curItem in items)
                {
                    lblStatus.Text = curItem["Name"].ToString() + " Zip code is :" + curItem["ZipCode"].ToString() + " Country is :" + curItem["ResidenceCountry"].ToString();
                }
            }
        }
    }
}
