using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportToSPList
{
    class dbConnection
    {

        private SqlDataAdapter myAdapter;
        private SqlConnection conn;

        private SqlDataAdapter myAdapter1;
        private SqlConnection conn1;

        /// <constructor>
        /// Initialise Connection
        /// </constructor>
        public dbConnection()
        {
            try
            {
                myAdapter = new SqlDataAdapter();
                conn = new SqlConnection(@"Server=dbserver;Database=FworkSQLEcm;User Id=etssys; Password=c4ndy4u");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                GlobalLogic.ExceptionHandle(e, "dbConnection");
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
                myCommand.CommandTimeout = 120;
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
                GlobalLogic.ExceptionHandle(e, _query+ "--------------------------executeSelectNoParameter");
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
                myCommand.CommandTimeout = 120;
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
                Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nMessage: " + e.Message + " \nException: " + e.StackTrace.ToString());
                string paramsVal = "";
                foreach (SqlParameter p in sqlParameter)
                { paramsVal += p.ParameterName + " --- " + p.Value + " ------- "; }
                GlobalLogic.ExceptionHandle(e, _query + "------------------"+paramsVal+"executeSelectQuery");
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
                myCommand.CommandTimeout = 120;
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
                string paramsVal = "";
                foreach (SqlParameter p in sqlParameter)
                { paramsVal += p.ParameterName + " --- " + p.Value + " ------- "; }
                GlobalLogic.ExceptionHandle(e, _query + "------------------" + paramsVal + "executeSelectQueryWithSP");
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
                myCommand.CommandTimeout = 120;
                //myCommand.CommandType = CommandType.StoredProcedure;                
                myCommand.Connection = openConnection();
                myCommand.CommandText = _query;
                myCommand.Parameters.AddRange(sqlParameter);
                myAdapter.InsertCommand = myCommand;
                myCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.Write("Error - Connection.executeInsertQuery - Query: " + _query + " \nMessage: \n" + e.Message+ " \nException: \n" + e.StackTrace.ToString());
                string paramsVal = "";
                foreach (SqlParameter p in sqlParameter)
                { paramsVal += p.ParameterName + " --- " + p.Value + " ------- "; }
                GlobalLogic.ExceptionHandle(e, _query + "------------------" + paramsVal + "executeInsertQuery");
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
        /// Insert Query
        /// </method>
        public bool executeInsertQuerySP(String _query, SqlParameter[] sqlParameter)
        {
            SqlCommand myCommand = new SqlCommand();
            try
            {
                myCommand.CommandTimeout = 120;
                myCommand.CommandType = CommandType.StoredProcedure;                
                myCommand.Connection = openConnection();
                myCommand.CommandText = _query;
                myCommand.Parameters.AddRange(sqlParameter);
                myAdapter.InsertCommand = myCommand;
                myCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.Write("Error - Connection.executeInsertQuery - Query: " + _query + " \nMessage: \n" + e.Message + " \nException: \n" + e.StackTrace.ToString());
                string paramsVal = "";
                foreach (SqlParameter p in sqlParameter)
                { paramsVal += p.ParameterName + " --- " + p.Value + " ------- "; }
                GlobalLogic.ExceptionHandle(e, _query + "------------------" + paramsVal + "executeInsertQuerySP");
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
                myCommand.CommandTimeout = 120;
                // myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = openConnection();
                myCommand.CommandText = _query;
                myCommand.Parameters.AddRange(sqlParameter);
                myAdapter.UpdateCommand = myCommand;
                myCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.Write("Error - Connection.executeUpdateQuery - Query: " + _query + " \nMessage: " + e.Message + _query + " \nException: " + e.StackTrace.ToString());
                string paramsVal = "";
                foreach (SqlParameter p in sqlParameter)
                { paramsVal += p.ParameterName + " --- " + p.Value + " ------- "; }
                GlobalLogic.ExceptionHandle(e, _query + "------------------" + paramsVal + "executeUpdateQuery");
                return false;
            }
            finally
            {
                myCommand.Dispose();
                myCommand.Connection = closeConnection();
            }
            return true;
        }

        public bool executeUpdateQuerySP(String _query, SqlParameter[] sqlParameter)
        {
            SqlCommand myCommand = new SqlCommand();
            try
            {
                myCommand.CommandTimeout = 120;
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = openConnection();
                myCommand.CommandText = _query;
                myCommand.Parameters.AddRange(sqlParameter);
                myAdapter.UpdateCommand = myCommand;
                myCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.Write("Error - Connection.executeUpdateQuerySP - Query: " + _query + " \nMessage: " + e.Message + _query + " \nException: " + e.StackTrace.ToString());
                string paramsVal = "";
                foreach (SqlParameter p in sqlParameter)
                { paramsVal += p.ParameterName + " --- " + p.Value + " ------- "; }
                GlobalLogic.ExceptionHandle(e, _query + "------------------" + paramsVal + "executeUpdateQuerySP");
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
