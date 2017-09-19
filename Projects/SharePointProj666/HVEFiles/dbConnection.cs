using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Microsoft.SharePoint;

namespace SharePointProj666.HVEFiles
{
    class dbConnection
    {
        private SqlDataAdapter myAdapter;
        private SqlConnection conn;

        /// <constructor>
        /// Initialise Connection
        /// </constructor>
        public dbConnection()
        {
            CustCode cs = new CustCode(); 
            try
            {
               
                
                myAdapter = new SqlDataAdapter();
                conn = new SqlConnection(@"Server=AAESAWS0038\SHAREPOINT;Database=AAES Home;User Id=SQLAdmin; Password=Admin123#");
                
                //conn = new SqlConnection(@"Server=WIN-NJFP7TE48BN\SHAREPOINT;Database=AAES Home;User Id=SQLAdmin; Password=Admin12345#"); //Working
                
                //cs.LogAction("DBConnection for sql : " + "AAES Home");
                //if (ConfigurationManager.ConnectionStrings["SQLDataCon"] != null)
                //{
                //    string conn1  = ConfigurationManager.ConnectionStrings["SQLDataCon"].ConnectionString;
                //    conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLDataCon"].ConnectionString);//"CargillConnection"
                //}
                //conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLDataCon"].ConnectionString);//"CargillConnection"
                // <add name="SQLDataCon" connectionString="Server=WIN-NJFP7TE48BN\SHAREPOINT;Database=AAES Home;Integrated Security=true" />
                //WIN-NJFP7TE48BN\SHAREPOINT
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                //cs.LogAction("DBConnection Error"+e.Message);
                //return null;
            }

            
        }

        /// <method>
        /// Open Database Connection if Closed or Broken
        /// </method>
        private SqlConnection openConnection()
        {
            if (conn.State == ConnectionState.Closed || conn.State ==
                        ConnectionState.Broken)
            {
                conn.Open();
            }
            return conn;
        }
        private SqlConnection closeConnection()
        {

            conn.Close();

            return conn;
        }
        public DataTable executeSelectNoParameter(String _query)
        {
            SqlCommand myCommand = new SqlCommand();
            DataTable dataTable = new DataTable();
            try
            {
                myCommand.CommandType = CommandType.StoredProcedure;   // For stored proc
                myCommand.Connection = openConnection();
                myCommand.CommandText = _query;
                myAdapter.SelectCommand = myCommand;
                myAdapter.Fill(dataTable);
                myAdapter.Dispose();
            }
            catch (SqlException e)
            {
                Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
                return null;
            }
            finally
            {
                myCommand.Connection = closeConnection();
            }
            return dataTable;
        }
        /// <method>
        /// Select Query
        /// </method>
        public DataTable executeSelectQuery(String _query, SqlParameter[] sqlParameter)
        {
            CustCode cs = new CustCode(); 
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

                //cs.LogAction("select command for: "+_query);



            }
            catch (SqlException e)
            {
                Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
                //cs.LogAction("catch for select statement:- " + e.Message);
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
            CustCode cs = new CustCode();
            SqlCommand myCommand = new SqlCommand();
            try
            {
                //myCommand.CommandType = CommandType.StoredProcedure;                
                myCommand.Connection = openConnection();
                myCommand.CommandText = _query;
                myCommand.Parameters.AddRange(sqlParameter);
                myAdapter.InsertCommand = myCommand;
                myCommand.ExecuteNonQuery();
                //cs.LogAction("Insert statement as :- " + _query);
            }
            catch (SqlException e)
            {
                Console.Write("Error - Connection.executeInsertQuery - Query: " + _query + " \nException: \n" + e.StackTrace.ToString());
                //cs.LogAction("Exception as:- " + e.Message);
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
            CustCode cs = new CustCode();
            SqlCommand myCommand = new SqlCommand();
            try
            {
                // myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = openConnection();
                myCommand.CommandText = _query;
                myCommand.Parameters.AddRange(sqlParameter);
                myAdapter.UpdateCommand = myCommand;
                myCommand.ExecuteNonQuery();
                //cs.LogAction("Update Statement as:-" + _query);
            }
            catch (SqlException e)
            {
                Console.Write("Error - Connection.executeUpdateQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
                //cs.LogAction("Ecwption for insert :- " + e.Message);
                return false;
            }
            finally
            {
                myCommand.Dispose();
                myCommand.Connection = closeConnection();
            }
            return true;
        }
    }
}
