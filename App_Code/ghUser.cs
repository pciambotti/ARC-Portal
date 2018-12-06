using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
/// <summary>
/// Summary description for ghUser
/// </summary>
public class ghUser
{
    static private String sqlStrInt = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    public ghUser()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    static public void identity_get_userid()
    {
        // Get the logged in users userid
        // This should be retrieved during the login process
        try
        {
            #region SQL Connection
            if (Connection.GetDBMode() == "Stage")
            {
                sqlStrInt = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
            }
            using (SqlConnection con = new SqlConnection(sqlStrInt))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    String cmdText = "";
                    cmdText = "[portal_user].[dbo].[user_get_userid]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    //cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    cmd.Parameters.Add(new SqlParameter("@sp_username", HttpContext.Current.User.Identity.Name));
                    #endregion SQL Parameters
                    //print_sql(cmd); // Will print for Admin in Local
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                HttpContext.Current.Session["userid"] = sqlRdr["userid"].ToString();
                            }
                        }
                    }
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            ErrorLog.ErrorLog_Save(ex, null, null, "User Get UserID");
        }
    }
    static public bool identity_is_admin_ivr()
    {
        bool tgl = false;
        if ((HttpContext.Current.User.IsInRole("System Administrator") == true && HttpContext.Current.User.Identity.Name.Contains("@greenwoodhall.com"))
            || (HttpContext.Current.User.IsInRole("Manager") == true && HttpContext.Current.User.Identity.Name.Contains("@patriotllc.com"))
            || HttpContext.Current.User.Identity.Name == "jburrell@greenwoodhall.com"
            || HttpContext.Current.User.Identity.Name == "cstevenson@greenwoodhall.comx"
            )
        {
            tgl = true;
        }
        return tgl;
    }
    static public bool identity_is_admin()
    {
        bool tgl = false;
        if ((HttpContext.Current.User.IsInRole("System Administrator") == true
            || HttpContext.Current.User.IsInRole("Administrator") == true
            || HttpContext.Current.User.IsInRole("Manager") == true
            || HttpContext.Current.User.IsInRole("Advisor") == true
            ) && (HttpContext.Current.User.Identity.Name == "nciambotti@greenwoodhall.com"
                || HttpContext.Current.User.Identity.Name == "amartin@greenwoodhall.com"
                || HttpContext.Current.User.Identity.Name == "cvaldez@greenwoodhall.com"
                || HttpContext.Current.User.Identity.Name == "jburrell@greenwoodhall.com"
                || HttpContext.Current.User.Identity.Name.Contains("@patriotllc.com")
                || HttpContext.Current.User.Identity.Name.Contains("@answernet.com")
                )
            )
        {
            tgl = true;
        }
        if (HttpContext.Current.User.Identity.Name == "cstevenson@greenwoodhall.com"
            || HttpContext.Current.User.Identity.Name == "cstevenson@greenwoodhall.com"
            )
        {
            tgl = true;
        }
        // tgl = false; // DeBug tester
        return tgl;
    }
    static public bool identity_is_admin_super()
    {
        if (HttpContext.Current.User.IsInRole("System Administrator") == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    static public bool identity_is_recording_admin()
    {
        bool tgl = false;
        if (HttpContext.Current.User.Identity.Name.Contains("@greenwoodhall.com") || HttpContext.Current.User.Identity.Name.Contains("@patriotllc.com"))
        {
            tgl = true;
        }
        else if (HttpContext.Current.User.Identity.Name.Contains("@redcross") || HttpContext.Current.User.Identity.Name.Contains("@cdrfg"))
        {
            // We allow ARC and CDR to access recordings
            tgl = true;
        }
        return tgl;
    }
    static public string identity_get_username(string userid)
    {
        // Get the username from userid
        string username = userid;
        if (userid == "0" || userid == "")
        {
            username = "system";
        }
        else
        {
            try
            {
                #region SQL Connection
                if (Connection.GetDBMode() == "Stage")
                {
                    sqlStrInt = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
                }
                using (SqlConnection con = new SqlConnection(sqlStrInt))
                {
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        cmd.CommandTimeout = 600;
                        String cmdText = "";
                        cmdText = "SELECT TOP 1 [u].[username] FROM [portal_user].[dbo].[user] [u] WITH(NOLOCK) WHERE [u].[userid] = @sp_userid";
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #region SQL Parameters
                        //cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@sp_userid", userid));
                        #endregion SQL Parameters
                        //print_sql(cmd, "append"); // Will print for Admin in Local
                        #region SQL Processing
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    username = sqlRdr["username"].ToString();
                                    username = username.Replace("@greenwoodhall.com", "");
                                }
                            }
                        }
                        #endregion SQL Processing
                    }
                    #endregion SQL Command
                }
                #endregion SQL Connection
            }
            catch (Exception ex)
            {
                //Error_Catch(ex, "User Get UserID", DeBug_Footer);
                ErrorLog.ErrorLog_Save(ex, null, null, "User Get UserID");
            }
        }
        return username;
    }
    static public void identity_get_client()
    {
        // Get the client name from the userid
        try
        {
            #region SQL Connection
            if (Connection.GetDBMode() == "Stage")
            {
                sqlStrInt = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
            }
            using (SqlConnection con = new SqlConnection(sqlStrInt))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    #region Build cmdText
                    String cmdText = "";
                    cmdText = @"
                                SELECT
                                TOP 1
                                [ucs].[description] [client]
                                FROM [dbo].[user_client] [uc] WITH(NOLOCK)
                                JOIN [dbo].[user_clients] [ucs] WITH(NOLOCK) ON [ucs].[clientid] = [uc].[clientid]
                                WHERE 1=1
                                AND [uc].[userid] = @sp_userid
                                ";
                    #endregion Build cmdText
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add(new SqlParameter("@sp_userid", HttpContext.Current.Session["userid"].ToString()));
                    #endregion SQL Parameters
                    //print_sql(cmd, "append"); // Will print for Admin in Local
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                HttpContext.Current.Session["clientname"] = sqlRdr["client"].ToString();
                                HttpContext.Current.Session["clientcache"] = DateTime.UtcNow.ToString(); // How long we cache it for
                            }
                        }
                    }
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            //Error_Catch(ex, "User Get UserID", DeBug_Footer);
            //Error_Save(ex, "User Get UserID");
        }
    }
    static public bool identity_can_edit()
    {
        if (HttpContext.Current.User.IsInRole("System Administrator") == true
            || HttpContext.Current.User.IsInRole("Administrator") == true
            || (HttpContext.Current.User.IsInRole("Advisor") == true && HttpContext.Current.User.Identity.Name != "agent2014")
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    static public void SecureCheck()
    {
        if (!HttpContext.Current.Request.IsSecureConnection
                && !HttpContext.Current.Request.IsLocal
                && !HttpContext.Current.Request.Url.ToString().Contains("192.168.")
                && !HttpContext.Current.Request.Url.ToString().Contains("ciambotti-dsk")
                && !HttpContext.Current.Request.Url.ToString().Contains("mylocal")
        )
        {
            String redir = HttpContext.Current.Request.Url.ToString().Replace("http:", "https:");
            HttpContext.Current.Response.Redirect(@redir);
        }
    }
}
