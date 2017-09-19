using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using System.Data.SqlClient;
using System.Text;


namespace Proj1.HVE.WebParts.HVEGetSQL1
{
    [ToolboxItemAttribute(false)]
    public partial class HVEGetSQL1 : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public HVEGetSQL1()
        {
            try
            {
                myAdapter = new SqlDataAdapter();
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLDataCon"].ConnectionString);//"CargillConnection"
            }
            catch (Exception e)
            {
                Label9.Text += e.Message;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                { 
                    string quer = @"Select top 10
                                      querystring, COUNT(queryString) as CountValue
	                                  from  [Search_Service_Application_1_LinksStoreDB_85b9e99e32794b6c9785675b4fb6757f].[dbo].[MSSQLogPageImpressionQuery]
	                                  where queryString !='' 
			                                and queryString IS NOT NULL 
			                                and queryString not like 'path:\""http://%' 
			                                and queryString not like 'path:\""https://%' 
			                                and queryString not like 'siteid:%'  
	                                  group by queryString
                                      order by CountValue desc";
                    dbConnection conn = new dbConnection();
                    DataTable tempTabale = null;
                     tempTabale = executeSelectNoParameter(quer);
                    //GridView1.DataSource = tempTabale;
                    //GridView1.DataBind();
                     //foreach (DataRow row in tempTabale.Rows) {}

                    string linkVal = @"http://aespaspsas/sites/HVEDevSite/_layouts/15/osssearchresults.aspx?u=http%3A%2F%2Faespaspsas%2Fsites%2FHVEDevSite&k=";
                     StringBuilder htmlStr = new StringBuilder("This is string builder");
                     
                     for (int i = 0; i < tempTabale.Rows.Count; i++)
                     {                          
                         switch (i)
                         {
                             case 0:
                                 htmlStr.Append("<p><font size='7'>" + tempTabale.Rows[i][0] + "</font>");
                                 //htmlStr.Append(@"<a href='<%#Eval(\""tempTabale.Rows[i][0]\"")%>' target='_blank'>Visit W3Schools</a>");
                                 
                                 //htmlStr.Append("<p><font size='7'>" + tempTabale.Rows[i][0] + "</font></p>");
                                 htmlStr.Append("&nbsp");
                                 break;
                             case 1:
                                 htmlStr.Append("<font size='7'>" + tempTabale.Rows[i][0] + "</font>");
                                 htmlStr.Append("&nbsp");
                                 break;
                             case 2:
                                 htmlStr.Append("<font size='6'>" + tempTabale.Rows[i][0] + "</font>");
                                 htmlStr.Append("&nbsp");
                                 break;
                             case 3:
                                 htmlStr.Append("<font size='6'>" + tempTabale.Rows[i][0] + "</font>");
                                 htmlStr.Append("&nbsp");
                                 break;
                             case 4:
                                 htmlStr.Append("<font size='6'>" + tempTabale.Rows[i][0] + "</font>");
                                 htmlStr.Append("&nbsp");
                                 break;
                             case 5:
                                 htmlStr.Append("<font size='5'>" + tempTabale.Rows[i][0] + "</font>");
                                 htmlStr.Append("&nbsp");
                                 break;
                             case 6:
                                 htmlStr.Append("<font size='5'>" + tempTabale.Rows[i][0] + "</font>");
                                 htmlStr.Append("&nbsp");
                                 break;
                             case 7:
                                 htmlStr.Append("<font size='5'>" + tempTabale.Rows[i][0] + "</font>");
                                 htmlStr.Append("&nbsp");
                                 break;
                             case 8:
                                 htmlStr.Append("<font size='4'>" + tempTabale.Rows[i][0] + "</font>");
                                 htmlStr.Append("&nbsp");
                                 break;
                             case 9:
                                 htmlStr.Append("<font size='4'>" + tempTabale.Rows[i][0] + "</font></p>");
                                 htmlStr.Append("&nbsp");
                                 break;
               //              case 10:
               //                  htmlStr.Append("<p><font size='4'>" + tempTabale.Rows[i][0] + "</font></p>");
               ////                  htmlStr.Append("&nbsp");
               //                  break;
                             default:
                                 Console.WriteLine("Default case");
                                 break;
                         }

                     }
                     LiteralText.Text = htmlStr.ToString();
                    Repeater1.DataSource = tempTabale;
                    Repeater1.DataBind();
                }
            }
            catch (System.Exception excep)
            {
                Label1.Text += excep.Message;
            }

        }
                private SqlDataAdapter myAdapter;
        private SqlConnection conn;

        /// <constructor>
        /// Initialise Connection
        /// </constructor>
        //public dbConnection()
        //{
        //    myAdapter = new SqlDataAdapter();
        //    conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLDataCon"].ConnectionString);//"CargillConnection"
        //}

        /// <method>
        /// Open Database Connection if Closed or Broken
        /// </method>
        private SqlConnection openConnection()
        {
            try
            {
                if (conn.State == ConnectionState.Closed || conn.State ==
                            ConnectionState.Broken)
                {
                    conn.Open();
                }
            }
            catch (System.Exception excep)
            {
                Label2.Text = excep.Message;
            }
            return conn;
        }
        private SqlConnection closeConnection()
        {
            try
            {
                conn.Close();
            }
            catch (System.Exception excep)
            {
                Label3.Text = excep.Message;
            }
            return conn;
        }
        public DataTable executeSelectNoParameter(String _query)
        {
            
                SqlCommand myCommand = new SqlCommand();
                DataTable dataTable = new DataTable();
                try
                {
                    //myCommand.CommandType = CommandType.StoredProcedure;   
                    myCommand.Connection = openConnection();
                    myCommand.CommandText = _query;
                    myAdapter.SelectCommand = myCommand;
                    myAdapter.Fill(dataTable);
                    myAdapter.Dispose();
                }
                catch (SqlException e)
                {
                    Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
                    Label4.Text = e.Message;
                    return null;
                }
                finally
                {
                    myCommand.Connection = closeConnection();
                }
                return dataTable;
            
           
            //return dataTable;
        }
        /// <method>
        /// Select Query
        /// </method>
        public DataTable executeSelectQuery(String _query, SqlParameter[] sqlParameter)
        {
           
                SqlCommand myCommand = new SqlCommand();
                DataTable dataTable = new DataTable();

                try
                {
                    // myCommand.CommandType = CommandType.StoredProcedure;
                    myCommand.Connection = openConnection();
                    myCommand.CommandText = _query;
                    myCommand.Parameters.AddRange(sqlParameter);
                    myCommand.ExecuteNonQuery();
                    myAdapter.SelectCommand = myCommand;
                    myAdapter.Fill(dataTable);


                    myAdapter.Dispose();

                }
                catch (SqlException e)
                {
                    Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
                    Label5.Text = e.Message;
                    return null;
                }
                finally
                {
                    myCommand.Connection = closeConnection();
                }
                return dataTable;
            
           
        }

        public DataTable executeSelectQueryWithSP(String _query, SqlParameter[] sqlParameter)
        {
           
                SqlCommand myCommand = new SqlCommand();
                DataTable dataTable = new DataTable();

                try
                {
                    myCommand.CommandType = CommandType.StoredProcedure;
                    myCommand.Connection = openConnection();
                    myCommand.CommandText = _query;
                    myCommand.Parameters.AddRange(sqlParameter);
                    myCommand.ExecuteNonQuery();
                    myAdapter.SelectCommand = myCommand;
                    myAdapter.Fill(dataTable);


                    myAdapter.Dispose();



                }
                catch (SqlException e)
                {
                    Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
                    Label6.Text = e.Message;
                    return null;
                }
                finally
                {
                    myCommand.Connection = closeConnection();
                }
                return dataTable;
            
           
        }

        /// <method>
        /// Insert Query
        /// </method>
        public bool executeInsertQuery(String _query, SqlParameter[] sqlParameter)
        {
            
                SqlCommand myCommand = new SqlCommand();
                try
                {
                    //myCommand.CommandType = CommandType.StoredProcedure;                
                    myCommand.Connection = openConnection();
                    myCommand.CommandText = _query;
                    myCommand.Parameters.AddRange(sqlParameter);
                    myAdapter.InsertCommand = myCommand;
                    myCommand.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.Write("Error - Connection.executeInsertQuery - Query: " + _query + " \nException: \n" + e.StackTrace.ToString());
                    Label7.Text = e.Message;
                    return false;
                }
                finally
                {
                    myCommand.Dispose();
                    myCommand.Connection = closeConnection();
                }
                return true;
            
           
        }

        /// <method>
        /// Update Query
        /// </method>
        public bool executeUpdateQuery(String _query, SqlParameter[] sqlParameter)
        {
           
                SqlCommand myCommand = new SqlCommand();
                try
                {
                    // myCommand.CommandType = CommandType.StoredProcedure;
                    myCommand.Connection = openConnection();
                    myCommand.CommandText = _query;
                    myCommand.Parameters.AddRange(sqlParameter);
                    myAdapter.UpdateCommand = myCommand;
                    myCommand.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.Write("Error - Connection.executeUpdateQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
                    Label8.Text = e.Message;
                    return false;
                }
                finally
                {
                    myCommand.Dispose();
                    myCommand.Connection = closeConnection();
                }
                return true;
           
            
        }
        protected void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
