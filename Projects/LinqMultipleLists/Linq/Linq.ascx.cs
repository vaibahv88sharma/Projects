using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;


namespace LinqMultipleLists.Linq
{
    [ToolboxItemAttribute(false)]
    public partial class Linq : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public Linq()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Linq 3 Lists using EntitiesDataContext
            StringBuilder writerToLiteral = new StringBuilder();
            try
            {
                writerToLiteral.Append("<table>");
                writerToLiteral.Append("<tr><td>Project Name</td><td>Employee Name</td><td>Employee Location</td><td>Employee ID</td></tr>");
                using (EntitiesDataContext dc = new EntitiesDataContext("http://sp:1220/sites/cslteam"))
                //using (EntitiesDataContext dc = new EntitiesDataContext(SPContext.Current.Web.Url)
                {
                    EntityList<ProjectsItem> Proj = dc.GetList<ProjectsItem>("Projects");
                    var q = from emp in Proj.ToList()
                            //where emp.JoiningPeriod < 10 // orderby em.Title 
                            select new
                                {
                                    emp.Title,
                                    //emp.EmployeeInProject,
                                    EmployeeName = emp.EmployeeInProject.Title,
                                    EmployeeLocation = emp.EmployeeInProject.CurrentLocation.Title,
                                    EmployeeID = emp.EmployeeInProject.EmployeeID,
                                    EmployeeState = emp.EmployeeInProject.CurrentLocation.Province,
                                };
                    foreach (var qValue in q)
                    {
                        writerToLiteral.Append("<tr><td>");
                        writerToLiteral.Append(qValue.Title);
                        writerToLiteral.Append("</td><td>");
                        writerToLiteral.Append(qValue.EmployeeName);
                        writerToLiteral.Append("</td><td>");
                        writerToLiteral.Append(qValue.EmployeeLocation);
                        writerToLiteral.Append("</td><td>");
                        writerToLiteral.Append(qValue.EmployeeID);
                        writerToLiteral.Append("</td></tr>");
                    }
                }
            }
            catch (Exception x)
            {
                writerToLiteral.Append("<tr><td>");
                writerToLiteral.Append(x.Message);
                writerToLiteral.Append("</td></tr>");
            }
            finally
            {
                writerToLiteral.Append("</table>");
                ListData.Text = writerToLiteral.ToString();
            }
            #endregion Linq 2 Lists using EntitiesDataContext
        }
    }
}
