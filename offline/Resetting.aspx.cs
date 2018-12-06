using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class offline_Resetting : System.Web.UI.Page
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
        //Label1.Text = "What?";
        if (Request["k"] != null)
        {
            panel_Reset.Visible = true;
            panel_Error.Visible = false;
            panel_Done.Visible = false;
            Resetting_Validate(Request["k"].ToString());
        }
        else
        {
            panel_Reset.Visible = false;
            panel_Error.Visible = true;
            panel_Done.Visible = false;
            lbl_ErrorTitle.Text = "Link Expired";
            lbl_Error1.Text = "The link you are attempting to use has already expired.";
            lbl_Error2.Text = "Please visit the login page to reset your password.";

            lbl_ErrorTitle.Text = "Link Failure";
            lbl_Error1.Text = "You must use a valid link to use this tool.";
            lbl_Error2.Text = "Please visit the login page to reset your password.";
        }
    }
    protected void Resetting_Validate(String key)
    {
        // Validate the key
        try
        {
            #region SQL Connection
            Boolean oDebug = false;
            if (Connection.GetDBMode() == "Stage") oDebug = true;
            if (oDebug) { sqlStr = "PS_Stage"; }
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    String cmdText = "";
                    cmdText = "[dbo].[user_password_reminder_validate]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add(new SqlParameter("@SP_Key", key));
                    #endregion SQL Parameters
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                //sqlResponse = sqlRdr["Response"].ToString();
                                //sqlResponseFull = sqlRdr["ResponseFull"].ToString();
                                //sqlUserName = sqlRdr["UserName"].ToString();
                                //sqlRole = sqlRdr["Role"].ToString();
                                if (sqlRdr["Response"].ToString() == "Success")
                                {
                                    panel_Reset.Visible = true;
                                    panel_Error.Visible = false;
                                    Label1.Text = "Confirm your username and enter your new password.";
                                }
                                else
                                {
                                    panel_Reset.Visible = false;
                                    panel_Error.Visible = true;
                                    panel_Done.Visible = false;
                                    lbl_ErrorTitle.Text = "Link Expired";
                                    lbl_Error1.Text = "The link you are attempting to use has already expired.";
                                    lbl_Error2.Text = "Please visit the login page to reset your password.";
                                    lbl_Error2.Text += "<br />" + sqlRdr["Response"].ToString() + "<br />";
                                    lbl_Error2.Text += sqlRdr["ResponseFull"].ToString() + "<br />";
                                }
                            }
                        }

                    }
                    //Label1.Text += "<br />" + dv.ID;
                    #endregion SQL Processing

                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Display(ex, "Validate Key", Label1);
            Error_Save(ex, "Validate Key");
        }
    }
    protected void Resetting_Submit(object sender, EventArgs e)
    {
        // Validate the key
        try
        {
            #region SQL Connection
            Boolean oDebug = false;
            if (Connection.GetDBMode() == "Stage") oDebug = true;
            if (oDebug) { sqlStr = "PS_Stage"; }
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    String cmdText = "";
                    cmdText = "[dbo].[user_password_reminder_reset]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add(new SqlParameter("@SP_Key", Request["k"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@SP_Username", Username.Text));
                    cmd.Parameters.Add(new SqlParameter("@SP_Password", Password.Text));
                    #endregion SQL Parameters
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                //sqlResponse = sqlRdr["Response"].ToString();
                                //sqlResponseFull = sqlRdr["ResponseFull"].ToString();
                                //sqlUserName = sqlRdr["UserName"].ToString();
                                //sqlRole = sqlRdr["Role"].ToString();
                                if (sqlRdr["Response"].ToString() == "Success")
                                {
                                    //Label1.Text = "Your password has been reset, please proceed to login screen.";
                                    panel_Reset.Visible = false;
                                    panel_Done.Visible = true;
                                }
                                else
                                {
                                    Label1.Text = "There was an error resetting your password:";
                                }
                            }
                        }

                    }
                    //Label1.Text += "<br />" + dv.ID;
                    #endregion SQL Processing

                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Display(ex, "Reset Password", Label1);
            Error_Save(ex, "Reset Password");
        }
    }
    protected void Error_Save(Exception ex, String error)
    {
        string sPath = HttpContext.Current.Request.Url.AbsolutePath;
        string[] strarry = sPath.Split('/');
        int lengh = strarry.Length;
        String spPage = strarry[lengh - 1];
        String spURL = HttpContext.Current.Request.Url.ToString();
        String spQS = HttpContext.Current.Request.Url.Query.ToString();
        if (error == null) { error = "General Error"; }

        DetailsView dv = null; // ErrorView;

        ErrorLog.ErrorLog_Save(ex, dv, "ARC Admin Portal", error, spPage, spQS, spURL);
    }
    protected void Error_Display(Exception ex, String error, Label lbl)
    {
        lbl.Text = String.Format("<table class='table_error'>"
            + "<tr><td>Error<td/><td>{0}</td></tr>"
            + "<tr><td>Message<td/><td>{1}</td></tr>"
            + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
            + "<tr><td>Source<td/><td>{3}</td></tr>"
            + "<tr><td>InnerException<td/><td>{4}</td></tr>"
            + "<tr><td>Data<td/><td>{5}</td></tr>"
            + "</table>"
            , error //0
            , ex.Message //1
            , ex.StackTrace //2
            , ex.Source //3
            , ex.InnerException //4
            , ex.Data //5
            , ex.HelpLink
            , ex.TargetSite
            );

        //ErrorLog.ErrorLog(ex);

    }
}
