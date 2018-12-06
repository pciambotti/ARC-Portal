using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
public partial class Reporting_Stats : System.Web.UI.Page
{
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Reporting Stats";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
    }
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    protected void Page_Load(object sender, EventArgs e)
    {
        //SqlDataSource1.ConnectionString = Connection.GetConnectionString("Default", "");
        //SqlDataSource2.ConnectionString = Connection.GetConnectionString("Default", "");
        if (!IsPostBack)
        {
            bool tglClient = true; bool tglVendor = false; bool tglAdmin = false;
            filter_reports.Visible = true;
            if (this.Page.User.Identity.Name.Contains("@patriotllc.com"))
            {
                tglClient = false;
                tglVendor = true;
            }
            else if (this.Page.User.Identity.Name.Contains("@redcross.com"))
            {
                tglClient = true;
                tglVendor = false;
            }
            else if (this.Page.User.Identity.Name == "nciambotti@greenwoodhall.com" && Request["t"] != null && Request["t"].ToString().Contains("tgl"))
            {
                tglClient = false;
                tglVendor = false;
                tglAdmin = false;
                if (Request["t"].ToString().Contains("tglClient")) tglClient = true;
                if (Request["t"].ToString().Contains("tglVendor")) tglVendor = true;
                if (Request["t"].ToString().Contains("tglAdmin")) tglAdmin = true;
            }
            else if (this.Page.User.Identity.Name.Contains("@greenwoodhall.com"))
            {
                tglClient = false;
                tglVendor = false;
                tglAdmin = true;
            }
            if (tglClient)
            {
                // Clients see a list of reports
                filter_monthlycounts.Visible = true;
                filter_dispositions.Visible = true;
                filter_designations.Visible = true;
                filter_lines.Visible = true;
                filter_lines_designations.Visible = true;
            }
            else if (tglVendor)
            {
                // Vendors see a list of reports
                filter_monthlycounts.Visible = true;
                filter_callcounts.Visible = true;
                filter_hourlystats.Visible = true;
                filter_dispositions.Visible = true;
                filter_designations.Visible = true;
                filter_dnis.Visible = true;
                filter_lines.Visible = true;
                filter_lines_designations.Visible = true;
            }
            else if (tglAdmin)
            {
                // Anyone else is GH or Admin
                filter_monthlycounts.Visible = true;
                filter_callcounts.Visible = true;
                filter_hourlystats.Visible = true;
                filter_dispositions.Visible = true;
                filter_designations.Visible = true;
                filter_creditcard.Visible = true;
                filter_dnis.Visible = true;
                filter_dnisdesignation.Visible = true;
                filter_lines.Visible = true;
                filter_lines_designations.Visible = true;
            }

            rpTimeZone.Text += "-" + ghFunctions.dtUserOffSet.ToString() + " (US Eastern Timezone)";

            // Set the Time to Univision Telethon if before the start of Telemundo
            if (DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet) < DateTime.Parse("2017-09-24 18:59"))
            {
                dtStartDate.Text = "09/23/2017";
                dtStartTime.Text = "19:00";
                dtEndDate.Text = DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet).AddDays(+0).ToString("MM/dd/yyyy");
                dtEndTime.Text = "23:59";
            }
            else if (DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet) < DateTime.Parse("2017-09-25 02:00"))
            {
                dtStartDate.Text = "09/24/2017";
                dtStartTime.Text = "19:00";
                dtEndDate.Text = DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet).AddDays(+0).ToString("MM/dd/yyyy");
                dtEndTime.Text = "23:59";
            }
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
            DateTime dtStart = DateTime.Now;
            Error_General.Text = "";

            DateTime date_start = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text).AddHours(ghFunctions.dtUserOffSet);
            DateTime date_end = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text).AddHours(ghFunctions.dtUserOffSet);



            /// Run up to 3 reports - the JavaScript validation should prevent the user from selecting more than 3
            /// However just in case - we only run the first 3 selected
            /// 
            int rprts = 0;
            if (rprts < 3 && cbMonthlyCounts.Checked)
            {
                rprts++;
                Gridview_Data(0, this.Page.User.Identity.Name, gvMonthlyCounts, date_start, date_end);

            }
            if (rprts < 3 && cbCallCounts.Checked)
            {
                rprts++;
                Gridview_Data(0, this.Page.User.Identity.Name, gvCallRecords, date_start, date_end);
            }
            if (rprts < 3 && cbHourlySats.Checked)
            {
                rprts++;
                if ((date_end - date_start).TotalDays <= 2)
                {
                    gvCall_StatsHourly.Visible = true;
                    GridView_StatsHourly(date_start, date_end, gvCall_StatsHourly);
                    lblCall_StatsHourly.Text = "";
                }
                else
                {
                    gvCall_StatsHourly.Visible = false;
                    lblCall_StatsHourly.Text = "<br />Hourly stats are only shown if the date range is 2 or less days.";
                }
            }
            if (rprts < 3 && cbDispositions.Checked)
            {
                rprts++;
                Gridview_Data(0, this.Page.User.Identity.Name, gvDispositions, date_start, date_end);
            }
            if (rprts < 3 && cbDesignations.Checked)
            {
                rprts++;
                Gridview_Data(0, this.Page.User.Identity.Name, gvDesignations, date_start, date_end);
            }
            if (rprts < 3 && cbCreditCard.Checked)
            {
                rprts++;
                Gridview_Data(0, this.Page.User.Identity.Name, gvCreditCard, date_start, date_end);
            }
            if (rprts < 3 && cbDNIS.Checked)
            {
                rprts++;
                Gridview_Data(0, this.Page.User.Identity.Name, gvDNIS, date_start, date_end);
            }
            if (rprts < 3 && cbDNISDesignation.Checked)
            {
                rprts++;
                Gridview_Data(0, this.Page.User.Identity.Name, gvDNIS_Designation, date_start, date_end);
            }
            if (rprts < 3 && cbLines.Checked)
            {
                rprts++;
                Gridview_Data(0, this.Page.User.Identity.Name, gvLines, date_start, date_end);
            }
            if (rprts < 3 && cbLinesDesignations.Checked)
            {
                rprts++;
                Gridview_Data(0, this.Page.User.Identity.Name, gvLinesDesignations, date_start, date_end);
            }
            Error_General.Text += "<br />Done";
            if (rprts == 0) Error_General.Text = "<br />Select a report to run";
            DateTime dtEnd = DateTime.Now;
            // rpElapsed.Text = "Query duration: 00:00";
            TimeSpan dtElapsed = dtEnd - dtStart;
            //rpElapsed.Text = "Total Elapsed Time: <b>" + SecondsTo((dtEnd - dtStart).TotalMilliseconds) + "</b>";
            rpElapsed.Text = "Query Time: <b>" + dtElapsed.ToString() + "</b>";
            //lblFilterDetails.Text = "" + date_start + " to " + date_end;
            lblFilterDetails.Text = "Date Range: " + date_start.AddHours(-ghFunctions.dtUserOffSet).ToString("yyyy-MM-dd HH:mm:ss") + " to " + date_end.AddHours(-ghFunctions.dtUserOffSet).ToString("yyyy-MM-dd HH:mm:ss");
        }
        catch (Exception ex)
        {
            Error_General.Text += "<br />Oops";
            Error_Save(ex, "GridView_Refresh");
        }
    }
    
    protected void CallRecords_Export_Excel(object sender, EventArgs e)
    {
    }
    protected void Gridview_Data(Int32 UserID, String UserName, GridView gv, DateTime dtStart, DateTime dtEnd)
    {
        String cmdText = "";
        if (gv.ID == "gvCallRecords") cmdText = "[dbo].[portal_custom_stats_calls]";
        if (gv.ID == "gvDispositions") cmdText = "[dbo].[portal_custom_stats_dispositions]";
        if (gv.ID == "gvDesignations") cmdText = "[dbo].[portal_custom_stats_designations]";
        if (gv.ID == "gvCreditCard") cmdText = "[dbo].[portal_custom_stats_donations]";
        if (gv.ID == "gvDNIS") cmdText = "[dbo].[portal_custom_stats_dnis]";
        if (gv.ID == "gvDNIS_Designation") cmdText = "[dbo].[portal_custom_stats_dnis_designation]";
        if (gv.ID == "gvLines") cmdText = "[dbo].[portal_custom_stats_lines]";
        if (gv.ID == "gvLinesDesignations") cmdText = "[dbo].[portal_custom_stats_lines_designations]";

        #region gvMonthlyCounts
        if (gv.ID == "gvMonthlyCounts")
        {
            cmdText = @"
SELECT
YEAR(DATEADD(hh,-@sp_tz_offset,[c].[logindatetime])) [year]
,MONTH(DATEADD(hh,-@sp_tz_offset,[c].[logindatetime])) [month]
,DATENAME(month, CAST(CONVERT(varchar,YEAR(DATEADD(hh,-@sp_tz_offset,[c].[logindatetime]))) + '-' + CONVERT(varchar,MONTH(DATEADD(hh,-@sp_tz_offset,[c].[logindatetime]))) + '-' + '01' AS datetime)) [month_name]
--,COUNT([c].[callid]) [calls]
,COUNT(DISTINCT([c].[callid])) [calls_total]
,COUNT(DISTINCT(CASE
	WHEN [dn].[company] IS NULL OR [dn].[company] NOT IN ('DRTV') THEN [c].[callid]
	ELSE NULL
END)) [calls_main]
,COUNT(DISTINCT(CASE
	WHEN [dn].[company] = 'DRTV' THEN [c].[callid]
	ELSE NULL
END)) [calls_drtv]
,COUNT(DISTINCT(CASE
	WHEN [cb].[id] IS NOT NULL THEN [c].[callid]
	ELSE NULL
END)) [donations]
,COUNT(DISTINCT(CASE
	WHEN [cb].[id] IS NOT NULL AND ([dn].[company] IS NULL OR [dn].[company] NOT IN ('DRTV')) THEN [c].[callid]
	ELSE NULL
END)) [donations_main]
,COUNT(DISTINCT(CASE
	WHEN [cb].[id] IS NOT NULL AND [dn].[company] = 'DRTV' THEN [c].[callid]
	ELSE NULL
END)) [donations_drtv]
,SUM(CASE
	WHEN [cb].[decision] = 'ACCEPT' THEN [cb].[ccauthreply_amount]
	ELSE 0
END) [amount_approved]
,SUM(CASE
	WHEN [cb].[decision] = 'ACCEPT' AND [dn].[company] = 'DRTV' THEN [cb].[ccauthreply_amount]
	ELSE 0
END) [amount_approved_drtv]
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[DonationCCInfo] [d] WITH(NOLOCK) ON [d].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [d].[id]
LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
WHERE 1=1
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
GROUP BY YEAR(DATEADD(hh,-@sp_tz_offset,[c].[logindatetime])), MONTH(DATEADD(hh,-@sp_tz_offset,[c].[logindatetime]))
ORDER BY YEAR(DATEADD(hh,-@sp_tz_offset,[c].[logindatetime])), MONTH(DATEADD(hh,-@sp_tz_offset,[c].[logindatetime]))
";
        }
        #endregion gvMonthlyCounts

        if (cmdText.Length > 0)
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (gv.ID == "gvMonthlyCounts")
                    {
                        cmd.CommandType = CommandType.Text;
                    }
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtStart;
                    cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtEnd;
                    cmd.Parameters.Add("@sp_tz_offset", SqlDbType.Int).Value = ghFunctions.dtUserOffSet;
                    #endregion SQL Parameters
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //DetailsView1.DataSource = dt;
                    gv.DataBind(); //DetailsView1.DataBind();
                                   //dtlLabel.Text += "<br />" + gv.ID;

                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
    }
    protected void GridView_StatsHourly(DateTime dtStart, DateTime dtEnd, GridView gv)
    {
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                String cmdText = "";
                cmdText = "[dbo].[portal_custom_stats_calls_hourly]";

                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtStart;
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtEnd;
                cmd.Parameters.Add("@sp_tz_offset", SqlDbType.Int).Value = ghFunctions.dtUserOffSet;
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt; //DetailsView1.DataSource = dt;
                gv.DataBind(); //DetailsView1.DataBind();
                //dtlLabel.Text += "<br />" + gv.ID;

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
    protected String SecondsTo(Double Seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(Seconds);

        String rtrn = String.Format("{0}:{1}:{2}",
            Math.Floor(time.TotalHours).ToString().PadLeft(2, '0'),
            time.Minutes.ToString().PadLeft(2, '0'),
            time.Seconds.ToString().PadLeft(2, '0'));

        return rtrn;
    }
    protected String MillisecondsTo(Double Seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(Seconds);

        String rtrn = String.Format("{0}:{1}:{2}",
            Math.Floor(time.TotalHours).ToString().PadLeft(2, '0'),
            time.Minutes.ToString().PadLeft(2, '0'),
            time.Seconds.ToString().PadLeft(2, '0'));

        return rtrn;
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
