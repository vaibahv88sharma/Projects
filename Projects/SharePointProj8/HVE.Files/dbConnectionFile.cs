using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace SharePointProj8.HVE.Files
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
            myAdapter = new SqlDataAdapter();
            //conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLDataCon1"].ConnectionString);
            conn = new SqlConnection(@"Server=AAESAWS0038\SHAREPOINT;Database=AAES Home;User Id=SQLAdmin; Password=Admin123#");
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
