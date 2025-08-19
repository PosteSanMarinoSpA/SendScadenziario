using System;
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

/// <summary>
/// Descrizione di riepilogo per Connection
/// </summary>
public class Connection
{
    public static string connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["db"].ToString();    

    public static string ConnectionString
    {
        get { return connectionstring; }        
    }    

    public static SqlConnection OpenConnection()
    {
        string sConn = ConnectionString;
 
        SqlConnection conn = new SqlConnection(sConn);
        conn.Open();

        return conn;
    }
        
    public static void CloseConnection(SqlConnection conn)
    {
        if (conn != null && conn.State == ConnectionState.Open)
        {
            conn.Close();
        }
    }    
}