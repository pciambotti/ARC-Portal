using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections.Specialized;

/// <summary>
/// Summary description for Connection
/// </summary>
public class Connection
{
    /// <summary>
    /// This is the connection class
    /// Here we will have any type of connection
    /// Based on the server the app is being run from, the connection will change
    /// WAN is typically a dev enviroment, where the app is connecting to a remote server
    /// LAN is typically a live enviroment, where the app is connecting to a server on it's private net
    /// </summary>
    //static string myHost = System.Net.Dns.GetHostName();
    //static string myIP = System.Net.Dns.GetHostEntry(myHost).AddressList[0].ToString();
    static public string myHost = System.Net.Dns.GetHostName();
    static public string myIP = GetMyIP();
    static private String GetMyIP()
    {
        String myIP2 = "";
        for (int i = 0; i < System.Net.Dns.GetHostEntry(myHost).AddressList.Count(); i++)
        {
            if (System.Net.Dns.GetHostEntry(myHost).AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                myIP2 = System.Net.Dns.GetHostEntry(myHost).AddressList[i].ToString();
                break;
            }
        }
        return myIP2;
    }

    static public string capKey { get; set; }
    static public string userIP() { return myIP; }

    static public string SmtpHost() { return ConfigurationManager.AppSettings["SmtpHost"]; }
    static public string SmtpPort() { return ConfigurationManager.AppSettings["SmtpPort"]; }
    static public string SmtpUsername() { return ConfigurationManager.AppSettings["SmtpUsername"]; }
    static public string SmtpPassword() { return ConfigurationManager.AppSettings["SmtpPassword"]; }

    static public string GetConnectionType()
    {
        String DB_LAN = ConfigurationManager.AppSettings["DB_LAN"];
        if (myHost.Contains("gh-developer") || myIP.Contains("0.0.0")) { return "Local"; }
        else if (myIP.Contains(DB_LAN)) { return "LAN"; }
        else { return "OTHER"; }
    }
    static public String GetDBName()
    {
        return "[arcweb]";
    }
    static public String GetDBMode()
    {
        return ConfigurationManager.AppSettings["DBMode"]; // Live | Stage
    }
    static public String GetSiteMode()
    {
        // Maintenance == Maintenance Mode, no one allowed to login.
        return ConfigurationManager.AppSettings["SiteMode"];
    }
    /// <summary>
    /// Standard Database Connection
    /// Will use LAN if local host is detected
    /// Will use WAN if no local host
    /// </summary>
    /// <returns></returns>
    static public String GetConnectionString(String CN_ID_Name, String CN_Source)
    {
        String CN_ID = "";
        String DB_Server = "";
        String DB_Name = "";
        String DB_UserName = "";
        String DB_Password = "";
        String DB_LAN = ConfigurationManager.AppSettings["DB_LAN"];

        if (CN_Source != "LAN" && CN_Source != "WAN")
        {
            CN_Source = "WAN";
            if (myIP.Contains(DB_LAN)) { CN_Source = "LAN"; }
        }
        else
        {
            if (myIP.Contains(DB_LAN)) { CN_Source = "LAN"; }
        }
        if (CN_ID_Name == "Default")
        {
            CN_ID = ConfigurationManager.AppSettings["DB_CN_DEFAULT"];
        }
        else
        {
            CN_ID = ConfigurationManager.AppSettings[CN_ID_Name];
        }
        #region Connection
        DB_Server = ConfigurationManager.AppSettings["DB" + CN_ID + "_" + CN_Source];
        DB_Name = ConfigurationManager.AppSettings["DB" + CN_ID + "_NAME"];
        DB_UserName = ConfigurationManager.AppSettings["DB" + CN_ID + "_USER"];
        DB_Password = ConfigurationManager.AppSettings["DB" + CN_ID + "_PASS"];
        #endregion Connection

        String DB_Connection = String.Format("Server={0};Database={1};Uid={2};Pwd={3};MultipleActiveResultSets={4}"
            , DB_Server // 0 - Server
            , DB_Name // 1 - Database
            , DB_UserName // 2 - Uid
            , DB_Password // 3 - Pwd
            , "True" // 4 - MultipleActiveResultSets
            );

        return DB_Connection;
    }
    static public string GetSmtpHost()
    {
        /// <summary>
        /// SMTP Connection
        /// </summary>
        String HS_LAN = ConfigurationManager.AppSettings["HS_LAN"];
        String HS_WAN = ConfigurationManager.AppSettings["HS_WAN"];
        if (myHost.Contains("gh-702843f7e966") || myIP.Contains("192.168.")) { return HS_LAN; } else { return HS_WAN; }
    }
}
