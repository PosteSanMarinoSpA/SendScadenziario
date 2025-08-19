using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Cryptography;
using System.Configuration;

namespace SendScadenziario
{
    public class Contratti
    {         
        public int ct_id { get; set; }
        public string ca_descrizione { get; set; }
        public string ct_societa { get; set; }
        public string ct_servizio { get; set; }
        public DateTime ct_data_inizio { get; set; }
        public DateTime ct_data_fine { get; set; }
        public DateTime ct_data_alert { get; set; }
        public string ct_note { get; set; }
        public int cs_sc_periodo_gg { get; set; }
        public bool cs_inviato { get; set; }
        public string ct_email_alert { get; set; }        
    }    

    public class ContrattiViewModel
    {
        public ContrattiViewModel()
        {
            forms = new List<Contratti>();
            form = new Contratti();
        }
        public List<Contratti> forms { get; set; }
        public Contratti form { get; set; }
    }
}
