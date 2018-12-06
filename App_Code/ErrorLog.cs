using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ErrorLog
/// </summary>
public class ErrorLog
{
    public ErrorLog()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    static public void ErrorLog_Save(Exception ex, System.Web.UI.WebControls.DetailsView dv)
    {
        ErrorLog_Save(ex, dv, null, null, null, null);
    }
    static public void ErrorLog_Save(Exception ex, System.Web.UI.WebControls.DetailsView dv,
        String spModule)
    {
        ErrorLog_Save(ex, dv, spModule, null, null, null, null);
    }
    static public void ErrorLog_Save(Exception ex, System.Web.UI.WebControls.DetailsView dv,
        String spModule, String spSource)
    {

        ErrorLog_Save(ex, dv, spModule, spSource, null, null, null);
    }
    static public void ErrorLog_Save(Exception ex, System.Web.UI.WebControls.DetailsView dv,
        String spModule, String spSource, String spPage)
    {
        ErrorLog_Save(ex, dv, spModule, spSource, spPage, null, null);
    }
    static public void ErrorLog_Save(Exception ex, System.Web.UI.WebControls.DetailsView dv,
        String spModule, String spSource, String spPage, String spQuery)
    {
        ErrorLog_Save(ex, dv, spModule, spSource, spPage, spQuery, null);
    }
    static public void ErrorLog_Save(Exception ex, System.Web.UI.WebControls.DetailsView dv, String spModule, String spSource, String spPage, String spQuery, String spFullURL)
    {
        #region SQL Connection
        String sqlStr = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
        if (Connection.GetDBMode() == "Stage")
        {
            sqlStr = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                cmd.CommandText = "[dbo].[sp_error_log_net]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                if (spModule == null) { spModule = "Portal"; }
                if (spSource == null) { spSource = "ARC Admin"; }
                cmd.Parameters.Add(new SqlParameter("@SP_Module", spModule));
                cmd.Parameters.Add(new SqlParameter("@SP_Source", spSource));

                if (spPage == null) { spPage = ""; }
                cmd.Parameters.Add(new SqlParameter("@SP_Page", spPage));
                if (spQuery == null) { spQuery = ""; }
                cmd.Parameters.Add(new SqlParameter("@SP_QueryString", spQuery));
                if (spFullURL == null) { spFullURL = ""; }
                cmd.Parameters.Add(new SqlParameter("@SP_FullURL", spFullURL));

                cmd.Parameters.Add(new SqlParameter("@SP_Ex_Message", ex.Message.ToString()));
                cmd.Parameters.Add(new SqlParameter("@SP_Ex_StackTrace", ex.StackTrace.ToString()));
                cmd.Parameters.Add(new SqlParameter("@SP_Ex_Source", ex.Source.ToString()));
                if (ex.InnerException != null)
                {
                    cmd.Parameters.Add(new SqlParameter("@SP_Ex_InnerException", ex.InnerException.ToString()));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@SP_Ex_InnerException", ""));
                }
                cmd.Parameters.Add(new SqlParameter("@SP_Ex_Data", ex.Data.ToString()));
                if (ex.HelpLink != null)
                {
                    cmd.Parameters.Add(new SqlParameter("@SP_Ex_HelpLink", ex.HelpLink.ToString()));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@SP_Ex_HelpLink", ""));
                }
                cmd.Parameters.Add(new SqlParameter("@SP_Ex_TargetSite", ex.TargetSite.ToString()));

                cmd.Parameters.Add(new SqlParameter("@SP_SourceDate", DateTime.Now));

                #endregion SQL Parameters

                #region SQL Processing
                if (dv != null)
                {
                    try
                    {
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        dv.DataSource = dt; //DetailsView1.DataSource = dt;
                        dv.DataBind(); //DetailsView1.DataBind();
                    }
                    catch { cmd.ExecuteNonQuery(); }
                }
                else
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch {  }
                }
                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    static public void ErrorLog_Display(Exception ex, String error, System.Web.UI.WebControls.Label lbl)
    {
        String DisplayMessage = String.Format("<div class='error_div'>"
            + "<table class='error_table'>"
            + "<tr><td>Error<td/><td>{0}</td></tr>"
            + "<tr><td>Message<td/><td>{1}</td></tr>"
            + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
            + "<tr><td>Source<td/><td>{3}</td></tr>"
            + "<tr><td>InnerException<td/><td>{4}</td></tr>"
            + "<tr><td>Data<td/><td>{5}</td></tr>"
            + "</table>"
            + "</div>"
            , error //0
            , ex.Message //1
            , ex.StackTrace //2
            , ex.Source //3
            , ex.InnerException //4
            , ex.Data //5
            , ex.HelpLink
            , ex.TargetSite
            );

        lbl.Text = DisplayMessage;
    }
    static public string ErrorLog_Display_String(Exception ex, String error)
    {
        String DisplayMessage = String.Format("<div class='error_div'>"
            + "<table class='error_table'>"
            + "<tr><td>Error<td/><td>{0}</td></tr>"
            + "<tr><td>Message<td/><td>{1}</td></tr>"
            + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
            + "<tr><td>Source<td/><td>{3}</td></tr>"
            + "<tr><td>InnerException<td/><td>{4}</td></tr>"
            + "<tr><td>Data<td/><td>{5}</td></tr>"
            + "</table>"
            + "</div>"
            , error //0
            , ex.Message //1
            , ex.StackTrace //2
            , ex.Source //3
            , ex.InnerException //4
            , ex.Data //5
            , ex.HelpLink
            , ex.TargetSite
            );

        //lbl = DisplayMessage;
        return DisplayMessage;
    }
}
