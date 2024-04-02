using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeCreatives_KDSPJ.Connections
{
    internal class OracleHelper
    {
        public static OracleConnection GetCon()
        {
            string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=182.180.159.89)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=SCAR)));User Id=KRC;Password=KRC;";

            int num1 = 0;
            int num2 = 5;
            int num3 = 10;
            while (true)
            {
                try
                {
                    OracleConnection con = new OracleConnection(connectionString);
                    con.Open();
                    return con;
                }
                catch
                {
                    if (num1 < num2)
                    {
                        ++num1;
                      //  Log.Information(string.Format("Failed to connect to Oracle (attempt {0}/{1}). Retrying in {2} seconds...", (object)num1, (object)num2, (object)num3));
                        Thread.Sleep(num3 * 1000);
                    }
                    else
                        throw;
                }
            }
        }

        public static DataTable SelectRec(string query)
        {
            OracleConnection con = OracleHelper.GetCon();
            using (con)
            {
                OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(query, con);
                DataTable dataTable = new DataTable();
                oracleDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public static bool ExistRec(OracleConnection con, string query)
        {
            bool flag = false;
            if (new OracleCommand(query, con).ExecuteReader().HasRows)
                flag = true;
            return flag;
        }

        public static DateTime GetMaxOpenDate()
        {
            DateTime now = DateTime.Now;
            return now.Hour >= 6 ? now : now.AddDays(-1.0);
        }

        public static string GetValue(OracleConnection con, string query)
        {
            OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(query, con);
            DataTable dataTable = new DataTable();
            oracleDataAdapter.Fill(dataTable);
            return Convert.ToString(dataTable.Rows[0][0]);
        }

        public static string Get_transact_Status(OracleConnection con, string query)
        {
            string transactStatus = (string)null;
            OracleDataReader oracleDataReader = new OracleCommand(query, con).ExecuteReader();
            if (oracleDataReader.HasRows)
            {
                oracleDataReader.Read();
                transactStatus = oracleDataReader.GetString(0);
            }
            oracleDataReader.Close();
            return transactStatus;
        }

        public static void CURD(string spname, params OracleParameter[] parmaters)
        {
            OracleConnection con = OracleHelper.GetCon();
            using (con)
            {
                OracleTransaction oracleTransaction = con.BeginTransaction();
                OracleCommand oracleCommand = new OracleCommand(spname, con);
                oracleCommand.Parameters.AddRange((Array)parmaters);
                try
                {
                    oracleCommand.Transaction = oracleTransaction;
                    oracleCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    oracleTransaction.Rollback();
                   // Log.Information(ex.Message);
                }
                finally
                {
                    oracleTransaction.Commit();
                }
            }
        }
    }
}
