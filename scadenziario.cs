using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendScadenziario
{
    internal class scadenziario
    {
        public static bool checkAnagraficheToSend()
        {
            bool bOk = false;
            int mo_tm_id = 0;

            SqlConnection connection = new SqlConnection(Connection.ConnectionString);
            connection.Open();

            string sSql = @"SELECT * FROM moduli 
                            WHERE mo_invia = 1 
                            AND mo_confermainvia = 0 
                            AND (SELECT COUNT(mr_mo_id) FROM moduli_RUDT WHERE mr_mo_id = mo_id) = 0";

            SqlCommand cmd = new SqlCommand(sSql, connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                
            }

            return bOk;
        }
    }
}
