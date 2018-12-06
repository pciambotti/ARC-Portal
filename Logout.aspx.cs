using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;

public partial class Logout : System.Web.UI.Page
{
    private String sqlStr = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (Connection.GetDBMode() == "Stage")
        {
            sqlStr = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        // Add SQL Logout
        Logout_Submit_SQL();
        FormsAuthentication.SignOut();
        Session.Abandon();
        Response.Redirect("Login.aspx", true);

    }
    protected void Logout_Submit_SQL()
    {
        try
        {
            String UserName = this.Page.User.Identity.Name;
            String SessionKey = Session["SessionKey"].ToString();
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    String cmdText = "";
                    cmdText = "[dbo].[user_logout]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add(new SqlParameter("@SP_UserName", UserName));
                    cmd.Parameters.Add(new SqlParameter("@SP_SessionKey", SessionKey));
                    #endregion SQL Parameters
                    int success = cmd.ExecuteNonQuery();
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch
        {
        }
    }

}
