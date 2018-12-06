using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Reporting_Transcriptions : System.Web.UI.Page
{
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Reporting Transcriptions";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
    }
    private String sqlStr = Connection.GetConnectionString("Transcriptions", "");
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dtStartDate.Text = DateTime.Today.AddDays(-1).ToString("MM/dd/yyyy");
            dtStartTime.Text = "00:00";
            dtEndDate.Text = DateTime.Today.AddDays(-1).ToString("MM/dd/yyyy");
            dtEndTime.Text = "23:59";
            GridView_TranscriptionCounts(dtStartDate.Text + " " + dtStartTime.Text, dtEndDate.Text + " " + dtEndTime.Text, gvTranscriptions_Counts);
            DDL_ClientID();
        }
        Label lblPower = (Label)Master.FindControl("lblPower");
        if (lblPower != null)
        {
            lblPower.Text = "Powered by Greenwood & Hall";
        }
    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        try
        {
            Error_General.Text = "Running";

            string date_start = "";
            string date_end = "";
            date_start = dtStartDate.Text + " " + dtStartTime.Text;
            date_end = dtEndDate.Text + " " + dtEndTime.Text;

            Error_General.Text += "<br />" + date_start + "<br />" + date_end;

            GridView_TranscriptionCounts(date_start, date_end, gvTranscriptions_Counts);

            Error_General.Text += "<br />Done";
            rpElapsed.Text = "Query duration: 00:00";
            lblFilterDetails.Text = "Date Range: " + date_start + " to " + date_end;
        }
        catch (Exception ex)
        {
            Error_General.Text += "<br />Oops";
            Error_Save(ex, "GridView_Refresh");
        }
    }
    protected void GridView_Export_Excel(object sender, EventArgs e)
    {
        GridView_TranscriptionCounts(dtStartDate.Text + " " + dtStartTime.Text, dtEndDate.Text + " " + dtEndTime.Text, gvTranscriptions_Counts);
        GridViewExportUtil.Export("Transcription-Counts.xls", this.gvTranscriptions_Counts);
    }
    protected void GridView_TranscriptionCounts(String date_start, String date_end, GridView gv)
    {
        if (cbDateBreakdown.Checked)
        {
            gv.Columns[0].Visible = true;
        }
        else
        {
            gv.Columns[0].Visible = false;
        }
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                String cmdText = "";
                cmdText = "[dbo].[portal_custom_stats_transcription_counts]";

                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", date_start));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", date_end));
                if (cbDateBreakdown.Checked)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_breakdown", "1"));
                }
                if (ddlClientID.SelectedIndex > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_clientid", ddlClientID.SelectedValue));
                }
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt; //DetailsView1.DataSource = dt;
                gv.DataBind(); //DetailsView1.DataBind();
                //dtlLabel.Text += "<br />" + gv.ID;
                btnExport.Visible = true;
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GridView_DataBound(Object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        Error_General.Text += "<br />" + gv.ID.ToString() + " Records: [" + gv.Rows.Count.ToString() + "]";
    }
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='none';this.style.background='Gray';";
            //e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.background=this.originalstyle;";

            //http://aspdotnetfaq.com/Faq/How-to-correctly-highlight-GridView-rows-on-Mouse-Hover-in-ASP-NET.aspx
            // when mouse is over the row, save original color to new attribute, and change it to highlight yellow color
            e.Row.Attributes.Add("onmouseover","this.style.cursor='pointer';this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#336699'");

            // when mouse leaves the row, change the bg color to its original value    
            e.Row.Attributes.Add("onmouseout","this.style.backgroundColor=this.originalstyle;");

            e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(this.gvTranscriptions_Counts, "Select$" + e.Row.RowIndex);
        }
    }
    protected void GridView_IndexChanged(object sender, EventArgs e)
    {
        //dtlLabel.Text = gvSearchResults.SelectedIndex.ToString();
        //dtlLabel.Text += " [" + gvSearchResults.SelectedDataKey.Values[0].ToString();
        //dtlLabel.Text += " [" + gvSearchResults.SelectedDataKey.Values[1].ToString();
        GridView gvInt = gvTranscriptions_Counts;
        GridView gv = gvTranscriptions_List;
        try
        {
            #region Get List
            lblDetailsList.Text = "";
            string date = gvInt.SelectedRow.Cells[0].Text;
            string clientid = gvInt.SelectedRow.Cells[1].Text;
            string status = gvInt.SelectedRow.Cells[2].Text;
            string count = gvInt.SelectedRow.Cells[3].Text;
            lblDetailsList.Text += "<br />" + date;
            lblDetailsList.Text += " | " + clientid;
            lblDetailsList.Text += " | " + status;
            lblDetailsList.Text += " | " + count;
            //UpdatePanel3.Update();
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    String cmdText = "";
                    cmdText = "[vestigi].[dbo].[portal_custom_stats_transcription_list]";

                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    string date_start = dtStartDate.Text + " " + dtStartTime.Text;
                    string date_end = dtEndDate.Text + " " + dtEndTime.Text;

                    cmd.Parameters.Add(new SqlParameter("@sp_date_start", date_start));
                    cmd.Parameters.Add(new SqlParameter("@sp_date_end", date_end));
                    cmd.Parameters.Add(new SqlParameter("@sp_clientid", clientid));
                    cmd.Parameters.Add(new SqlParameter("@sp_status", status));
                    #endregion SQL Parameters
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //DetailsView1.DataSource = dt;
                    gv.DataBind(); //DetailsView1.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    btnExport.Visible = true;
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection

            #endregion Get List
        }
        catch (Exception ex)
        {
            Error_Save(ex, "DetailsView Data Error");
        }
    }

    #region DropDownList Configuration
    protected void DDL_ClientID()
    {
        DropDownList ddl = ddlClientID;
        ddlClientID.Items.Clear();
        ListItem li = new ListItem();
        li.Text = "All Clients";
        li.Value = "0";
        ddlClientID.Items.Add(li);
        #region Overall Try
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "[dbo].[portal_custom_stats_transcription_clientid_ddl]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add(new SqlParameter("@sp_source", "All"));
                    #endregion SQL Parameters
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    ddl.DataTextField = "text";
                    ddl.DataValueField = "value";
                    ddl.DataSource = dt;
                    ddl.DataBind();
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Save(ex, "DDL Projects - Medaille - Dashboard");
        }
        #endregion Overall Try
    }
    #endregion DropDownList Configuration


    protected void WriteToLabel(String type, String color, String msg, Label lbl)
    {
        if (lbl != null)
        {
            String spanBlue = "<span style='color: Blue;'>{0}</span>";
            String spanRed = "<span style='color: Blue;'>{0}</span>";
            String spanWhite = "<span style='color: Blue;'>{0}</span>";

            String spanColor = "<span style='color: " + color + ";'>{0}</span>";
            if (type == "add")
            {
                lbl.Text += String.Format(spanColor, msg);
            }
            else if (type == "append")
            {
                lbl.Text = String.Format(spanColor, msg) + lbl.Text;
            }
            else
            {
                lbl.Text = String.Format(spanColor, msg);
            }
        }
    }
    /// <summary>
    /// Testing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="e2"></param>
    protected void sqlupdated(object sender, SqlDataSourceStatusEventArgs e, SqlDataSourceCommandEventArgs e2)
    {
        //e.AffectedRows;
        //e.Command;
        //e.Exception;
        //e.ExceptionHandled;

        //e2.Cancel
        //e2.Command
    }
    protected void Populate_StateProvinceCountry(DropDownList ddlState, DropDownList ddlProvince, DropDownList ddlCountry)
    {
        DataSet myDs = new DataSet();
        myDs.ReadXml(Server.MapPath(@"StateCountry.xml"));
        if (myDs.Tables.Count > 0)
        {
            for (int x = 0; x <= myDs.Tables.Count - 1; x++)
            {
                if (myDs.Tables[x].TableName == "state" && ddlState != null)
                {
                    ddlState.DataSource = myDs.Tables[x];
                    ddlState.DataValueField = "code";
                    ddlState.DataTextField = "name";
                    ddlState.DataBind();
                }
                else if (myDs.Tables[x].TableName == "province" && ddlProvince != null)
                {
                    ddlProvince.DataSource = myDs.Tables[x];
                    ddlProvince.DataValueField = "code";
                    ddlProvince.DataTextField = "name";
                    ddlProvince.DataBind();
                }
                else if (myDs.Tables[x].TableName == "country" && ddlCountry != null)
                {
                    ddlCountry.DataSource = myDs.Tables[x];
                    ddlCountry.DataValueField = "code";
                    ddlCountry.DataTextField = "name";
                    ddlCountry.DataBind();
                    if (!IsPostBack) { ddlCountry.SelectedIndex = 1; }
                }
            }
        }
        else
        {
            //This is a fatal error.. can not load the State/Province/Country
        }
        myDs.Dispose();
    }
    protected void Error_Catch(Exception ex, String error, Label lbl)
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
    protected void Error_Save(Exception ex, String error)
    {
        string sPath = HttpContext.Current.Request.Url.AbsolutePath;
        string[] strarry = sPath.Split('/');
        int lengh = strarry.Length;
        String spPage = strarry[lengh - 1];
        String spURL = HttpContext.Current.Request.Url.ToString();
        String spQS = HttpContext.Current.Request.Url.Query.ToString();
        if (error == null) { error = "General Error"; }

        DetailsView dv = ErrorView;

        ErrorLog.ErrorLog_Save(ex, dv, "Portal: Oracle", error, spPage, spQS, spURL);
    }
}
