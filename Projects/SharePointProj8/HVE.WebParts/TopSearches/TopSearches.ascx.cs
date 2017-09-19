using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SharePointProj8.HVE.WebParts.TopSearches
{
    [ToolboxItemAttribute(false)]
    public partial class TopSearches : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TopSearches()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

         protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ////if (e.Row.RowType == DataControlRowType.DataRow)
            ////{
            ////    HyperLink link = new HyperLink();
            ////    link.Text = "This is a link!";
            ////    link.NavigateUrl = "Navigate somewhere based on data: " + e.Row.DataItem;
            ////    e.Row.Cells[1].Controls.Add(link);
            ////}
        }
         protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
         { }
         

        protected void Page_Load(object sender, EventArgs e)
        {
           // Button1.Visible = false;
            try
            {
                if (!Page.IsPostBack)
                { //AES_Search_Service_Application_LinksStoreDB_07d1b3d709b844288e1c87626eb41904   //HVEDev_Search_Service_Application_LinksStoreDB_151f333c38ce4fd3a6527d79d5532774                   
//////////////                    string quer = @"Select top 10
//////////////                                      querystring, COUNT(queryString) as CountValue
//////////////	                                  from  [HVEDev_Search_Service_Application_LinksStoreDB_151f333c38ce4fd3a6527d79d5532774].[dbo].[MSSQLogPageImpressionQuery]
//////////////	                                  where queryString !='' 
//////////////			                                and queryString IS NOT NULL 
//////////////											and queryString not like 'path:%' 			                               
//////////////			                                and queryString not like 'siteid:%'  
//////////////	                                  group by queryString
//////////////                                      order by CountValue desc";
                    string quer = @"Select top 10
                                      querystring, COUNT(queryString) as CountValue
	                                  from  [HVEStage_Search_Service_Application_LinksStoreDB_ac061569c0804ca68d18ac6f23bb2758].[dbo].[MSSQLogPageImpressionQuery]
	                                  where queryString !='' 
			                                and queryString IS NOT NULL 
											and queryString not like 'path:%' 			                               
			                                and queryString not like 'siteid:%'  
	                                  group by queryString
                                      order by CountValue desc";
                    HVE.Files.dbConnection conn = new HVE.Files.dbConnection();
                    DataTable tempTabale = null;
                    tempTabale = conn.executeSelectNoParameter(quer);

                   
                    #region GridView Bound Columns

                    DataTable dt = new DataTable();
                    for (int i = 0; i < tempTabale.Rows.Count; i++)
                    {
                        DataColumn dcol = new DataColumn("Col" + (i + 1), typeof(System.String));
                        dcol.AutoIncrement = true;
                        dt.Columns.Add(dcol);
                    }
                    DataRow drow = dt.NewRow();
                    //drow["Col1"] = 1;// "Row-";
                    dt.Rows.Add(drow);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();



                    #endregion

                    #region LiteralControl
                    StringBuilder htmlStr = new StringBuilder("");
                    string hrefLinks = "http://win-njfp7te48bn/sites/HVEDev3Search/Pages/search.aspx?k=";
                    for (int i = 0; i < tempTabale.Rows.Count; i++)
                    {
                        //htmlStr.Append("<table><tr></tr></table>");
                        switch (i)
                        {
                            case 0:
                                string hyperLink1 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:xx-large; '><a href='" + hyperLink1 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append(" | ");
                                htmlStr.Append("&nbsp");
                                break;
                            case 1:
                                string hyperLink2 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:x-large; '><a href='" + hyperLink2 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append(" | ");
                                htmlStr.Append("&nbsp");
                                break;
                            case 2:
                                string hyperLink3 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:large; '><a href='" + hyperLink3 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append(" | ");
                                htmlStr.Append("&nbsp");
                                break;
                            case 3:
                                string hyperLink4 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:large; '><a href='" + hyperLink4 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append(" | ");
                                htmlStr.Append("&nbsp");
                                break;
                            case 4:
                                string hyperLink5 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:larger; '><a href='" + hyperLink5 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append(" | ");
                                htmlStr.Append("&nbsp");
                                break;
                            case 5:
                                string hyperLink6 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:medium; '><a href='" + hyperLink6 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append(" | ");
                                htmlStr.Append("&nbsp");
                                break;
                            case 6:
                                string hyperLink7 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:small; '><a href='" + hyperLink7 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append(" | ");
                                htmlStr.Append("&nbsp");
                                break;
                            case 7:
                                string hyperLink8 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:small; '><a href='" + hyperLink8 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append(" | ");
                                htmlStr.Append("&nbsp");
                                break;
                            case 8:
                                string hyperLink9 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:smaller; '><a href='" + hyperLink9 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append(" | ");
                                htmlStr.Append("&nbsp");
                                break;
                            case 9:
                                string hyperLink10 = hrefLinks + tempTabale.Rows[i][0];
                                htmlStr.Append("<span style='font-size:smaller; '><a href='" + hyperLink10 + "' title='" + tempTabale.Rows[i][0] + "' >" + tempTabale.Rows[i][0] + "</a></span>");
                                htmlStr.Append("&nbsp");
                                break;
                            default:
                                Console.WriteLine("Default case");
                                break;
                        }

                    }
                    LiteralText.Text = htmlStr.ToString();


                }
                    #endregion
            }
            catch (System.Exception excep)
            {
                //Label1.Text += excep.Message;
            }
        }

    }
}
