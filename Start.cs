using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SendScadenziario
{
    internal class Starts
    {
        static void Main(string[] args)
        {
            bool bOk = true;
            // verifica se ci sono scadenze da spedire e invia una e-mail
            bOk = scadenziario.checkScadenzeToSend();
        }
    }
}