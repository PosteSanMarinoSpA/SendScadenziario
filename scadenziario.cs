using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SendScadenziario
{
    internal class scadenziario
    {
        public static bool checkScadenzeToSend()
        {
            bool bOk = false;
            int mo_tm_id = 0;

            SqlConnection connection = new SqlConnection(Connection.ConnectionString);
            connection.Open();

            string sSql = @"SELECT ct_id, ca_descrizione, ct_societa, ct_servizio, ct_data_inizio, ct_data_fine, cs_sc_periodo_gg, DATEADD(DAY, -cs_sc_periodo_gg, ct_data_fine) AS ct_data_alert, ct_email_alert, ct_note, cs_sc_periodo_gg, cs_inviato
                            FROM contratti
                            INNER JOIN contratti_categoria ON ca_id = ct_ca_id
                            INNER JOIN contratti_scadenza_invio ON ct_id = cs_ct_id
                            WHERE DATEADD(DAY, -cs_sc_periodo_gg, ct_data_fine) = CONVERT(date, GETDATE())
                            AND cs_inviato=0";

            SqlCommand cmd = new SqlCommand(sSql, connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                Contratti c = new Contratti();
                c.ct_id = Convert.ToInt32(row["ct_id"].ToString());
                c.ca_descrizione = Convert.ToString(row["ca_descrizione"].ToString());
                c.ct_societa = Convert.ToString(row["ct_societa"].ToString());
                c.ct_servizio = Convert.ToString(row["ct_servizio"].ToString());
                c.ct_data_inizio = Convert.ToDateTime(row["ct_data_inizio"].ToString());
                c.ct_data_fine = Convert.ToDateTime(row["ct_data_fine"].ToString());
                c.ct_data_alert = Convert.ToDateTime(row["ct_data_alert"].ToString());
                c.cs_sc_periodo_gg = Convert.ToInt32(row["cs_sc_periodo_gg"].ToString());
                c.ct_note = Convert.ToString(row["ct_note"].ToString());
                c.cs_sc_periodo_gg = Convert.ToInt32(row["cs_sc_periodo_gg"].ToString());
                c.cs_inviato = Convert.ToBoolean(row["cs_inviato"].ToString());
                c.ct_email_alert = Convert.ToString(row["ct_email_alert"].ToString());

                SendEmail(c, connection);
            }

            return bOk;
        }

        public static void SendEmail(Contratti c, SqlConnection connection)
        {
            if (!string.IsNullOrEmpty(c.ct_email_alert.Trim()))
            {
               try
               {
                    string html = "";
                    string oggetto = "AVVISO DI SCADENZA - " + c.ct_societa.Trim().ToUpper();

                    html = File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["template_email_moduli"].ToString(), Encoding.UTF8);
                    html = html.Replace("{DATAORA}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    html = html.Replace("{ID}", c.ct_id.ToString());
                    html = html.Replace("{TIPOLOGIA}", c.ca_descrizione);
                    html = html.Replace("{SOCIETA}", c.ct_societa);
                    html = html.Replace("{DESCRIZIONE}", c.ct_servizio);
                    html = html.Replace("{DATAINIZIO}", c.ct_data_inizio.ToString("dd/MM/yyyy"));
                    html = html.Replace("{DATAFINE}", c.ct_data_fine.ToString("dd/MM/yyyy"));
                    html = html.Replace("{DATAPREAVVISO}", c.ct_data_alert.ToString("dd/MM/yyyy"));
                    html = html.Replace("{GGPREAVVISO}", c.cs_sc_periodo_gg.ToString());
                    html = html.Replace("{NOTE}", c.ct_note);

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["emailInfo"].ToString(), System.Configuration.ConfigurationManager.AppSettings["nameFromInfo"].ToString());

                    string[] emailArray = c.ct_email_alert.Trim().Split(';');

                    foreach (string email in emailArray)
                    {
                        string trimmedEmail = email.Trim(); // Rimuove spazi inutili
                        if(!trimmedEmail.Equals(""))
                        { 
                            message.To.Add(trimmedEmail);
                        }
                    }
                    
                    //message.Bcc.Add("daniele.merli@poste.sm");
                    //message.Bcc.Add("d.merli@gmail.com");
                    
                    message.Subject = oggetto;
                    message.IsBodyHtml = true;
                    message.Body = html;
                    
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = System.Configuration.ConfigurationManager.AppSettings["SMTP"].ToString();                    
                    //smtpClient.UseDefaultCredentials = false;
                    var basicCredential = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["USERNAME_moduli"].ToString(), System.Configuration.ConfigurationManager.AppSettings["PASSWORD_moduli"].ToString());
                    smtpClient.Credentials = basicCredential;
                    smtpClient.Send(message);

                    // aggiorna record di invio e-mail                    
                    int row = 0;

                    string sSql = @"UPDATE contratti_scadenza_invio SET cs_data_invio=getDate(), cs_inviato=1
                                    WHERE cs_ct_id=@cs_ct_id AND cs_sc_periodo_gg=@cs_sc_periodo_gg";

                    SqlCommand cmd = new SqlCommand(sSql, connection);
                    cmd.Parameters.Add("@cs_ct_id", SqlDbType.Int).Value = c.ct_id;
                    cmd.Parameters.Add("@cs_sc_periodo_gg", SqlDbType.Int).Value = c.cs_sc_periodo_gg;

                    row = cmd.ExecuteNonQuery();                  
                }
                catch (Exception ex)
                {

                    // aggiorna record di invio e-mail
                    int row = 0;

                    string sSql = @"UPDATE contratti_scadenza_invio SET cs_data_invio=getDate(), cs_inviato=1
                                    WHERE cs_ct_id=@cs_ct_id AND cs_sc_periodo_gg=@cs_sc_periodo_gg";

                    SqlCommand cmd = new SqlCommand(sSql, connection);
                    cmd.Parameters.Add("@cs_ct_id", SqlDbType.Int).Value = c.ct_id;
                    cmd.Parameters.Add("@cs_sc_periodo_gg", SqlDbType.Int).Value = c.cs_sc_periodo_gg;

                    row = cmd.ExecuteNonQuery();
                }
            }
        }
    }    
}
