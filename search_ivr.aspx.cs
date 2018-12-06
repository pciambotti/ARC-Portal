using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class search_ivr : System.Web.UI.Page
{
    private Boolean isAdmin = false;
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "IVR Search";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
        isAdmin = ghUser.identity_is_admin_ivr();
    }
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
        if (!IsPostBack)
        {
            //Label8.Text += "<br />" + this.Page.User.Identity.Name;
            GridView_Refresh();
            //lookup_zipcode(); -- Why is this here?
            // Clear Date/Time Fields
        }
    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        GridView_Refresh();
    }
    protected void GridView_Refresh()
    {
        try
        {
            Int32 UserID = Convert.ToInt32(Session["userid"].ToString()); // This shouldn't be here, should be global
            #region Populate the grids
            // This is grand total, should not be refreshed every time
            GV_Data_Total_Counts(UserID, this.Page.User.Identity.Name, gvTotalCounts);
            // This populates the current query totals
            GV_Data_Query_Counts(UserID, this.Page.User.Identity.Name, gvQueryTotalCounts);
            // This populates the mind grid with record list
            GV_Data_Query(UserID, this.Page.User.Identity.Name, gvSearchResults);
            #endregion Populate the grids

            // Clear the "All" Panels
            ExpiredAllRecord.Visible = false;
            ClearAllRecord.Visible = false;
            DiscardAllRecord.Visible = false;
            lblAllRecordConfirm.Text = "";

            //Label8.Text = "Test1";
            Error_General.Text = "";
            // Clear the Result Grid
            gvSearchResults.SelectedIndex = -1;

            
            // Clear the Details View
            DetailsView_Clear();
            // Clear Details View Menu
            btnClearRecord.Visible = false;
            btnDiscardRecord.Visible = false;
            btnClearRecordConfirm.Enabled = true;
            btnClearRecordCancel.Enabled = true;
            ClearRecord.Visible = false;
            lblClearRecordConfirm.Text = "";

            btnDiscardRecord.Visible = false;
            btnDiscardRecordConfirm.Enabled = true;
            btnDiscardRecordCancel.Enabled = true;
            DiscardRecord.Visible = false;
            lblDiscardRecordConfirm.Text = "";
            btnExport.Visible = true;

            Session["gridrefresh"] = DateTime.UtcNow.ToString();
            // Show the "All" Buttons
            int cntTotal = 0;
            int cntPending = 0;
            int cntInvalid = 0;
            int cntCleared = 0;
            int cntDiscarded = 0;
            try
            {
                Int32.TryParse(gvQueryTotalCounts.Rows[0].Cells[1].Text, out cntTotal); // Total
                Int32.TryParse(gvQueryTotalCounts.Rows[0].Cells[3].Text, out cntPending); // Pending
                Int32.TryParse(gvQueryTotalCounts.Rows[0].Cells[4].Text, out cntInvalid); // Invalid
                Int32.TryParse(gvQueryTotalCounts.Rows[0].Cells[5].Text, out cntCleared); // Cleared
                Int32.TryParse(gvQueryTotalCounts.Rows[0].Cells[6].Text, out cntDiscarded); // Discarded

                //rpElapsed.Text = cntTotal.ToString();
                //rpElapsed.Text += " | " + gvQueryTotalCounts.Rows[0].Cells[2].Text; // Completed
                //rpElapsed.Text += " | " + cntPending.ToString();
                //rpElapsed.Text += " | " + cntInvalid.ToString();
            }
            catch { }
            //rpElapsed.Text += " | Others";
            if (IsPostBack)
            {
                divUndoDiscardAllRecord.Visible = false;
                divUndoClearAllRecord.Visible = false;
                divClearAllRecord.Visible = false;
                divDiscardAllRecord.Visible = false;
                if (isAdmin)
                {
                    if ((cntPending > 0 || cntInvalid > 0) && (cntTotal == (cntPending + cntInvalid)))
                    {
                        divClearAllRecord.Visible = true;
                        divDiscardAllRecord.Visible = true;
                    }
                    else if (cntCleared > 0 && (cntTotal == cntCleared))
                    {
                        divUndoClearAllRecord.Visible = true;
                    }
                    else if (cntDiscarded > 0 && (cntTotal == cntDiscarded))
                    {
                        divUndoDiscardAllRecord.Visible = true;
                    }
                    else
                    {

                    }
                }
            }
        
        }
        catch (Exception ex)
        {
            Label8.Text = "Error processing the request";
            Error_Save(ex, "GridView_Refresh");
        }
    }
    protected void GridView_Export_Excel(object sender, EventArgs e)
    {
        Int32 UserID = Convert.ToInt32(Session["userid"].ToString());
        gvSearchResults.AllowPaging = false;
        GV_Data_Query(UserID, this.Page.User.Identity.Name, gvSearchResults);
        GridViewExportUtil.Export("IRV-Call-Records.xls", this.gvSearchResults);
    }
    protected string GV_Query_Builder()
    {
        string sqlBuilder = "";
        // This is a hard coded date that signifies a change over in procedure;
        // we are not concerned with records before this date
        sqlBuilder += "AND ([r].[calldate] >= '20121207' OR [r].[status] = 5)" + "\n";
        // sqlBuilder += "AND [r].[started] < DATEADD(hh,-1,GETUTCDATE())" + "\n";
        // sqlBuilder += "AND [r].[started] < DATEADD(n,-15,GETUTCDATE())" + "\n";
        //rpElapsed.Text = strFileTypes;
        //rpElapsed.Text += " | " + ddlFileTypes.Text;
        //sqlBuilder += "AND [r].[status] IN (0,99)" + "\n";
        #region ddlCallType
        if (ddlCallType.Text.Length > 0)
        {
            string strCallTypes = "";
            foreach (ListItem li in ddlCallType.Items)
            {
                if (li.Value == "completed" && li.Selected) { strCallTypes += "1,"; }
                if (li.Value == "pending" && li.Selected) { strCallTypes += "0,"; }
                if (li.Value == "invalid" && li.Selected) { strCallTypes += "0,"; }
                if (li.Value == "cleared" && li.Selected) { strCallTypes += "99,"; }
                if (li.Value == "discarded" && li.Selected) { strCallTypes += "98,"; }
            }

            if (strCallTypes.EndsWith(","))
            {
                strCallTypes = strCallTypes.Substring(0, strCallTypes.Length - 1);
            }
            if (!strCallTypes.Contains("1"))
            {
                sqlBuilder += "AND [r].[callid] IS NULL" + "\n";
            }
            sqlBuilder += "AND [r].[status] IN (" + strCallTypes + ")" + "\n";
        }
        else
        {
            sqlBuilder += "AND [r].[status] IN (0,99)" + "\n";
        }
        #endregion ddlCallType
        #region ddlFileTypes
        if (ddlFileTypes.Text.Length > 0)
        {
            string strFileTypes = "";
            foreach (ListItem li in ddlFileTypes.Items)
            {
                if (li.Selected) { strFileTypes += li.Value + ","; }
            }
            if (strFileTypes.EndsWith(","))
            {
                strFileTypes = strFileTypes.Substring(0, strFileTypes.Length - 1);
            }
            if (strFileTypes.Contains("VC"))
            {
                sqlBuilder += "AND [vc].[recordid] IS NOT NULL" + "\n";
            }
            if (strFileTypes.Contains("CC"))
            {
                sqlBuilder += "AND [cc].[recordid] IS NOT NULL" + "\n";
            }
            if (strFileTypes.Contains("RANI"))
            {
                sqlBuilder += "AND [rn].[recordid] IS NOT NULL" + "\n";
            }
            if (strFileTypes.Contains("OP"))
            {
                sqlBuilder += "AND [op].[recordid] IS NOT NULL" + "\n";
            }
            if (strFileTypes.Contains("CT"))
            {
                sqlBuilder += "AND [ct].[recordid] IS NOT NULL" + "\n";
            }
        }
        #endregion ddlFileTypes
        #region ddlFileMissing
        if (ddlFileMissing.Text.Length > 0)
        {
            string strFileMissing = "";
            foreach (ListItem li in ddlFileMissing.Items)
            {
                if (li.Selected) { strFileMissing += li.Value + ","; }
            }

            if (strFileMissing.EndsWith(","))
            {
                strFileMissing = strFileMissing.Substring(0, strFileMissing.Length - 1);
            }

            if (strFileMissing.Contains("VC"))
            {
                sqlBuilder += "AND [vc].[recordid] IS NULL" + "\n";
            }
            if (strFileMissing.Contains("CC"))
            {
                sqlBuilder += "AND [cc].[recordid] IS NULL" + "\n";
            }
            if (strFileMissing.Contains("RANI"))
            {
                sqlBuilder += "AND [rn].[recordid] IS NULL" + "\n";
            }
            if (strFileMissing.Contains("OP"))
            {
                sqlBuilder += "AND [op].[recordid] IS NULL" + "\n";
            }
            if (strFileMissing.Contains("CT"))
            {
                sqlBuilder += "AND [ct].[recordid] IS NULL" + "\n";
            }
        
        }
        #endregion ddlFileMissing
        if (dtStartDate.Text.Length > 0)
        {
            string startdate = DateTime.Parse(dtStartDate.Text).ToString("yyyyMMdd");
            sqlBuilder += "AND [r].[calldate] >= '" + startdate + "'" + "\n";
        }
        if (dtEndDate.Text.Length > 0)
        {
            string enddate = DateTime.Parse(dtEndDate.Text).ToString("yyyyMMdd");
            sqlBuilder += "AND [r].[calldate] <= '" + enddate + "'" + "\n";
        }
        if (dtStartTime.Text.Length > 0 && dtEndTime.Text.Length > 0)
        {
            string starttime = dtStartTime.Text + ":00";
            string endtime = dtEndTime.Text + ":59";
            sqlBuilder += "AND [r].[calltime] >= '" + starttime + "'" + "\n";
            sqlBuilder += "AND [r].[calltime] <= '" + endtime + "'" + "\n";
        }

        sqlBuilder += "";
        return sqlBuilder;
    }
    protected void GV_Data_Query(Int32 UserID, String UserName, GridView gv)
    {
        // Change to this section should be duplicated to this section: GV_Data_Query_Counts
        #region SQL Connection
        try
        {
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    #region SQL String Builder
                    string sqlBuilder = "";
                    sqlBuilder += @"
                    SELECT
                    TOP (@sp_top)
                    [r].[sourceid]
                    ,[r].[recordid]
                    ,[r].[calldate]
                    ,[r].[calltime]
                    ,[r].[ani]
                    --,RIGHT(ISNULL([vc].[dnis],ISNULL([cc].[dnis],(ISNULL([rn].[dnis],ISNULL([op].[dnis],ISNULL([ct].[dnis],'0000')))))),4) [dnis]
	                ,CASE
		                WHEN LEN([vc].[dnis]) >= 4 THEN [vc].[dnis]
		                WHEN LEN([cc].[dnis]) >= 4 THEN [cc].[dnis]
		                WHEN LEN([rn].[dnis]) >= 4 THEN [rn].[dnis]
		                WHEN LEN([op].[dnis]) >= 4 THEN [op].[dnis]
		                WHEN LEN([ct].[dnis]) >= 4 THEN [ct].[dnis]
		                --WHEN LEN([main].[dnis]) >= 4 THEN [main].[dnis]
		                WHEN LEN([drtv].[dnis]) >= 4 THEN [drtv].[dnis]
	                END [dnis]
                    ,[r].[started]
	                ,CASE WHEN [vc].[recordid] IS NULL THEN ' ' ELSE 'V' END + ''
	                + CASE WHEN [cc].[recordid] IS NULL THEN ' ' ELSE 'C' END + ''
	                + CASE WHEN [rn].[recordid] IS NULL THEN ' ' ELSE 'R' END + ''
	                + CASE WHEN [op].[recordid] IS NULL THEN ' ' ELSE 'O' END + ''
	                + CASE WHEN [ct].[recordid] IS NULL THEN ' ' ELSE 'T' END + ''
	                --+ CASE WHEN [main].[recordid] IS NULL THEN ' ' ELSE 'M' END + ''
	                + CASE WHEN [drtv].[recordid] IS NULL THEN ' ' ELSE 'D' END
	                [files]
                    ,CASE
                    	WHEN [r].[status] = 0 THEN 'Invalid'
                    	WHEN [r].[status] = 1 THEN 'Completed'
                    	WHEN [r].[status] = 98 THEN 'Discarded'
                    	WHEN [r].[status] = 99 THEN 'Cleared'
                    	ELSE CONVERT(varchar(10),[r].[status])
                    END [status]
                    ,[r].[started]
                    FROM [dbo].[ivr_record] [r] WITH(NOLOCK)
                    LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]
                    LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]
                    LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]
                    LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]
                    LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]
	                --LEFT OUTER JOIN [dbo].[ivr_record_main] [main] WITH(NOLOCK) ON [main].[sourceid] = [r].[sourceid] AND [main].[recordid] = [r].[recordid] AND [main].[calldate] = [r].[calldate] AND [main].[calltime] = [r].[calltime] AND [main].[ani] = [r].[ani]
	                LEFT OUTER JOIN [dbo].[ivr_record_drtv] [drtv] WITH(NOLOCK) ON [drtv].[sourceid] = [r].[sourceid] AND [drtv].[recordid] = [r].[recordid] AND [drtv].[calldate] = [r].[calldate] AND [drtv].[calltime] = [r].[calltime] AND [drtv].[ani] = [r].[ani]
                    WHERE 1=1
";


                    sqlBuilder += GV_Query_Builder();
                    //sqlBuilder += "--ORDER BY [r].[status], [r].[started] DESC, [r].[calldate],[r].[calltime],[r].[recordid]" + "\n";
                    sqlBuilder += @"
                    --AND ([main].[recordid] IS NOT NULL OR ([vc].[recordid] IS NOT NULL OR [cc].[recordid] IS NOT NULL OR [rn].[recordid] IS NOT NULL OR [op].[recordid] IS NOT NULL OR [ct].[recordid] IS NOT NULL))
                    AND (([vc].[recordid] IS NOT NULL OR [cc].[recordid] IS NOT NULL OR [rn].[recordid] IS NOT NULL OR [op].[recordid] IS NOT NULL OR [ct].[recordid] IS NOT NULL))
                    AND ([drtv].[recordid] IS NOT NULL OR ([vc].[recordid] IS NOT NULL OR [cc].[recordid] IS NOT NULL OR [rn].[recordid] IS NOT NULL OR [op].[recordid] IS NOT NULL OR [ct].[recordid] IS NOT NULL))
                    AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)
                    ORDER BY [r].[calldate],[r].[calltime],[r].[recordid]
";
                    #endregion SQL String Builder

                    String cmdText = "";
                    cmdText = sqlBuilder;// "[dbo].[sp_ivr_search_get]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add(new SqlParameter("@sp_top", Convert.ToInt32(ddlTop.SelectedValue.ToString())));
                    //cmd.Parameters.Add(new SqlParameter("@sp_calltype", ddlTop.SelectedValue));
                    //cmd.Parameters.Add(new SqlParameter("@sp_receivedfiles", ddlTop.SelectedValue));
                    //cmd.Parameters.Add(new SqlParameter("@sp_startdate", ddlTop.SelectedValue));
                    //cmd.Parameters.Add(new SqlParameter("@sp_starttime", ddlTop.SelectedValue));
                    //cmd.Parameters.Add(new SqlParameter("@sp_enddate", ddlTop.SelectedValue));
                    //cmd.Parameters.Add(new SqlParameter("@sp_endtime", ddlTop.SelectedValue));
                    #endregion SQL Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                    gv.DataBind(); //dv_ivr_file_vc.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
        }
        catch (Exception ex)
        {
            Error_Save(ex, "GV_Data_Query");
        }
        #endregion SQL Connection
    }
    protected void GV_Data_Query_Counts(Int32 UserID, String UserName, GridView gv)
    {
        #region SQL Connection
        try
        {
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    #region SQL String Builder
                    string sqlBuilder = "";

                    sqlBuilder += "SELECT" + "\n";
                    sqlBuilder += "MIN([r].[calldate]) [since]" + "\n";
                    sqlBuilder += ",COUNT([r].[sourceid]) [total]" + "\n";
                    sqlBuilder += ",COUNT (CASE WHEN [r].[callid] IS NOT NULL THEN [r].[sourceid] ELSE NULL END) [completed]" + "\n";
                    sqlBuilder += ",COUNT (CASE WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NOT NULL AND ([r].[file_vc] IS NOT NULL) THEN [r].[sourceid] ELSE NULL END) [pending]" + "\n";
                    sqlBuilder += ",COUNT (CASE" + "\n";
                    sqlBuilder += "			WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NULL AND [r].[status] NOT IN (98,99) THEN [r].[sourceid]" + "\n";
                    sqlBuilder += "			WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NOT NULL AND [r].[status] NOT IN (98,99) AND ([r].[file_vc] IS NULL AND [r].[status] NOT IN (99)) THEN [r].[sourceid]" + "\n";
                    sqlBuilder += "		ELSE NULL END) [invalid]" + "\n";
                    sqlBuilder += ",COUNT (CASE WHEN [r].[status] IN (99) THEN [r].[sourceid] ELSE NULL END) [cleared]" + "\n";
                    sqlBuilder += ",COUNT (CASE WHEN [r].[status] IN (98) THEN [r].[sourceid] ELSE NULL END) [discarded]" + "\n";
                    sqlBuilder += "" + "\n";
                    sqlBuilder += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "WHERE 1=1" + "\n";
                    sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                    // 
                    sqlBuilder += GV_Query_Builder();
                    
                    sqlBuilder += "";

                    #endregion SQL String Builder
                    String cmdText = "";
                    cmdText = sqlBuilder;// "[dbo].[sp_ivr_search_get]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    //cmd.Parameters.Add(new SqlParameter("@sp_top", Convert.ToInt32(ddlTop.SelectedValue.ToString())));
                    #endregion SQL Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                    gv.DataBind(); //dv_ivr_file_vc.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
        }
        catch (Exception ex)
        {
            Error_Save(ex, "GV_Data_Query_Counts");
        }
        #endregion SQL Connection
    }
    protected void GV_Data_Total_Counts(Int32 UserID, String UserName, GridView gv)
    {
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region SQL String Builder
                string sqlBuilder = "";

                sqlBuilder += "SELECT" + "\n";
                sqlBuilder += "MIN([r].[calldate]) [since]" + "\n";
                sqlBuilder += ",COUNT([r].[sourceid]) [total]" + "\n";
                sqlBuilder += ",COUNT (CASE WHEN [r].[callid] IS NOT NULL THEN [r].[sourceid] ELSE NULL END) [completed]" + "\n";
                sqlBuilder += ",COUNT (CASE WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NOT NULL AND ([r].[file_vc] IS NOT NULL) THEN [r].[sourceid] ELSE NULL END) [pending]" + "\n";
                sqlBuilder += ",COUNT (CASE" + "\n";
                sqlBuilder += "			WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NULL AND [r].[status] NOT IN (98,99) THEN [r].[sourceid]" + "\n";
                sqlBuilder += "			WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NOT NULL AND [r].[status] NOT IN (98,99) AND ([r].[file_vc] IS NULL AND [r].[status] NOT IN (99)) THEN [r].[sourceid]" + "\n";
                sqlBuilder += "		ELSE NULL END) [invalid]" + "\n";
                sqlBuilder += ",COUNT (CASE WHEN [r].[status] IN (99) THEN [r].[sourceid] ELSE NULL END) [cleared]" + "\n";
                sqlBuilder += ",COUNT (CASE WHEN [r].[status] IN (98) THEN [r].[sourceid] ELSE NULL END) [discarded]" + "\n";
                sqlBuilder += "" + "\n";
                sqlBuilder += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "WHERE 1=1" + "\n";
                sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                // 
                sqlBuilder += "AND [r].[calldate] >= '20121207'";

                sqlBuilder += "";

                #endregion SQL String Builder
                String cmdText = "";
                cmdText = sqlBuilder; // "[dbo].[ivr_processing_records_get_count]";
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                #endregion SQL Parameters
                print_sql(cmd); // Will print for Admin in Local
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt;
                gv.DataBind();
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GridView_DataBound(Object sender, EventArgs e)
    {
        Label8.Text = " Records: [" + gvSearchResults.Rows.Count.ToString() + "]";
        if (gvSearchResults.PageCount > 0)
        {
            Label8.Text += " - Pages: [" + gvSearchResults.PageCount.ToString() + "]";
            Label8.Text += " - Approx Total: [" + (gvSearchResults.PageCount * gvSearchResults.Rows.Count).ToString() + "]";
            // Retrieve the pager row.
            //GridViewRow pagerRow = gvSearchResults.BottomPagerRow;
            GridViewRow pagerRow = gvSearchResults.TopPagerRow;
            // Retrieve the DropDownList and Label controls from the row.
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
            if (pageList != null)
            {
                // Create the values for the DropDownList control based on 
                // the  total number of pages required to display the data
                // source.
                for (int i = 0; i < gvSearchResults.PageCount; i++)
                {
                    // Create a ListItem object to represent a page.
                    int pageNumber = i + 1;
                    ListItem item = new ListItem(pageNumber.ToString());
                    // If the ListItem object matches the currently selected
                    // page, flag the ListItem object as being selected. Because
                    // the DropDownList control is recreated each time the pager
                    // row gets created, this will persist the selected item in
                    // the DropDownList control.   
                    if (i == gvSearchResults.PageIndex)
                    {
                        item.Selected = true;
                    }
                    // Add the ListItem object to the Items collection of the 
                    // DropDownList.
                    pageList.Items.Add(item);
                }
            }
            if (pageLabel != null)
            {
                // Calculate the current page number.
                int currentPage = gvSearchResults.PageIndex + 1;
                // Update the Label control with the current page information.
                pageLabel.Text = "Page " + currentPage.ToString() +
                  " of " + gvSearchResults.PageCount.ToString();
            }
            if (gvSearchResults.PageIndex > 0)
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = true;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = true;
            }
            else
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = false;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = false;
            }

            if (gvSearchResults.PageCount != (gvSearchResults.PageIndex + 1))
            {
                pagerRow.Cells[0].FindControl("lnkNextPage").Visible = true;
                pagerRow.Cells[0].FindControl("lnkLastPage").Visible = true;
            }
            else
            {
                pagerRow.Cells[0].FindControl("lnkNextPage").Visible = false;
                pagerRow.Cells[0].FindControl("lnkLastPage").Visible = false;
            }
        }
    }
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='none';this.style.background='Gray';";
            //e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.background=this.originalstyle;";

            //http://aspdotnetfaq.com/Faq/How-to-correctly-highlight-GridView-rows-on-Mouse-Hover-in-ASP-NET.aspx
            // when mouse is over the row, save original color to new attribute, and change it to highlight yellow color
            e.Row.Attributes.Add("onmouseover",
                "this.style.cursor='pointer';this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#336699'");

            // when mouse leaves the row, change the bg color to its original value    
            e.Row.Attributes.Add("onmouseout",
                "this.style.backgroundColor=this.originalstyle;");

            e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(this.gvSearchResults, "Select$" + e.Row.RowIndex);
        }
    }
    protected void GridView_IndexChanged_Old(object sender, EventArgs e)
    {
        dtlLabel.Text = "Feature not enabled: " + DateTime.Now.ToString("HH:mm:ss");
    }
    protected void GridView_IndexChanged(object sender, EventArgs e)
    {
        dtlLabel.Text = gvSearchResults.SelectedIndex.ToString();
        //dtlLabel.Text += " [" + gvSearchResults.SelectedDataKey.Values[0].ToString();
        //dtlLabel.Text += " [" + gvSearchResults.SelectedDataKey.Values[1].ToString();
        try
        {
            Int32 sourceid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
            Int32 recordid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[1].ToString());
            String calldate = gvSearchResults.SelectedDataKey.Values[2].ToString();
            String calltime = gvSearchResults.SelectedDataKey.Values[3].ToString();
            String ani = gvSearchResults.SelectedDataKey.Values[4].ToString();
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_vc);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_cc);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_rn);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_op);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_ct);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_main);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_drtv);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_record);
            
            if (isAdmin) { btnClearRecord.Visible = true; }
            if (gvSearchResults.SelectedRow.Cells[7].Text.ToString() == "Cleared")
            {
                btnClearRecord.Enabled = false;
            }
            else { btnClearRecord.Enabled = true; }
            btnClearRecordConfirm.Enabled = true;
            btnClearRecordCancel.Enabled = true;
            ClearRecord.Visible = false;
            lblClearRecordConfirm.Text = "";

            if (isAdmin) { btnDiscardRecord.Visible = true; }
            btnDiscardRecordConfirm.Enabled = true;
            btnDiscardRecordCancel.Enabled = true;
            DiscardRecord.Visible = false;
            lblDiscardRecordConfirm.Text = "";

        }
        catch (Exception ex)
        {
            Error_Save(ex, "DetailsView Data Error");
            btnClearRecord.Visible = false;
            btnClearRecordConfirm.Enabled = true;
            btnClearRecordCancel.Enabled = true;
            ClearRecord.Visible = false;
            lblClearRecordConfirm.Text = "";

            btnDiscardRecord.Visible = false;
            btnDiscardRecordConfirm.Enabled = true;
            btnDiscardRecordCancel.Enabled = true;
            DiscardRecord.Visible = false;
            lblDiscardRecordConfirm.Text = "";
        }
    }
    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //Session["EventList_GridView_SelectedIndex"] = null;
        //Session["EventList_GridView_PageIndex"] = null;
        Label8.Text = e.NewPageIndex.ToString();
        gvSearchResults.SelectedIndex = -1;
        gvSearchResults.PageIndex = e.NewPageIndex;
        GV_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);
    }
    protected void GridView_PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
    {

        // Retrieve the pager row.
        GridViewRow pagerRow = gvSearchResults.TopPagerRow;
        // Retrieve the PageDropDownList DropDownList from the bottom pager row.
        DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
        // Set the PageIndex property to display that page selected by the user.
        gvSearchResults.SelectedIndex = -1;
        gvSearchResults.PageIndex = pageList.SelectedIndex;
        GV_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);

    }
    protected void DetailsView_Clear()
    {
        if (dv_ivr_file_vc.Rows.Count > 0) dv_ivr_file_vc.DataBind();
        if (dv_ivr_file_cc.Rows.Count > 0) dv_ivr_file_cc.DataBind();
        if (dv_ivr_file_rn.Rows.Count > 0) dv_ivr_file_rn.DataBind();
        if (dv_ivr_file_op.Rows.Count > 0) dv_ivr_file_op.DataBind();
        if (dv_ivr_file_ct.Rows.Count > 0) dv_ivr_file_ct.DataBind();
        if (dv_ivr_file_main.Rows.Count > 0) dv_ivr_file_main.DataBind();
        if (dv_ivr_file_drtv.Rows.Count > 0) dv_ivr_file_drtv.DataBind();
        if (dv_ivr_file_record.Rows.Count > 0) dv_ivr_file_record.DataBind();
        
    }
    #region All Clear
    protected void Processing_All_Clear(object sender, EventArgs e)
    {
        if (Session["gridrefresh"] == null)
        {
            ClearAllRecord.Visible = false;
            divClearAllRecord.Visible = false;
            DiscardAllRecord.Visible = false;
            divDiscardAllRecord.Visible = false;

            ExpiredAllRecord.Visible = true;
        }
        else if (DateTime.Parse(Session["gridrefresh"].ToString()) > DateTime.UtcNow.AddMinutes(-1))
        {
            DiscardAllRecord.Visible = false;
            ClearAllRecord.Visible = true;
        }
        else
        {
            ClearAllRecord.Visible = false;
            DiscardAllRecord.Visible = false;
            ExpiredAllRecord.Visible = true;
        }
        
    }
    protected void Processing_All_Clear_Confirm(object sender, EventArgs e)
    {
        lblAllRecordConfirm.Text = "";
        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
        #region Using: SqlConnection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region SQL String Builder
                // This is for the UPDATE
                string sqlBuilder = "";
                sqlBuilder += "" + "\n";
                sqlBuilder += "UPDATE [dbo].[ivr_record]" + "\n";
                sqlBuilder += "SET [status] = 99" + "\n";
                sqlBuilder += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "WHERE 1=1" + "\n";
                sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                sqlBuilder += GV_Query_Builder();
                sqlBuilder += "";
                #endregion SQL String Builder
                #region SQL String Builder2
                // This is for the Action Log INSERT
                string sqlBuilder2 = "";
                sqlBuilder2 += "" + "\n";
                sqlBuilder2 += "INSERT INTO [dbo].[ivr_record_action_log]" + "\n";
                sqlBuilder2 += "([sourceid],[recordid],[calldate],[calltime],[ani],[actor],[action],[createdate])" + "\n";
                sqlBuilder2 += "SELECT" + "\n";
                sqlBuilder2 += "[r].[sourceid]" + "\n";
                sqlBuilder2 += ",[r].[recordid]" + "\n";
                sqlBuilder2 += ",[r].[calldate]" + "\n";
                sqlBuilder2 += ",[r].[calltime]" + "\n";
                sqlBuilder2 += ",[r].[ani]" + "\n";
                sqlBuilder2 += ",@sp_actor" + "\n";
                sqlBuilder2 += ",@sp_action" + "\n";
                sqlBuilder2 += ",GETUTCDATE()" + "\n";
                sqlBuilder2 += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "WHERE 1=1" + "\n";
                sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                sqlBuilder2 += GV_Query_Builder();
                #endregion SQL String Builder2
                cmd.CommandText = sqlBuilder;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                //cmd.Parameters.Add(new SqlParameter("@sp_top", Convert.ToInt32(ddlTop.SelectedValue.ToString())));
                #endregion SQL Parameters
                print_sql(cmd); // Will print for Admin in Local
                #region SQL Processing
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int affected = cmd.ExecuteNonQuery();
                lblAllRecordConfirm.Text = " | Records Updated: " + affected.ToString();
                if (affected > 0)
                {
                    ClearAllRecord.Visible = false;
                    divClearAllRecord.Visible = false;
                    DiscardAllRecord.Visible = false;
                    divDiscardAllRecord.Visible = false;
                    #region SQL - Action Log INSERT
                    cmd.CommandText = sqlBuilder2;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    if (Session["userid"] == null) { ghUser.identity_get_userid(); }
                    cmd.Parameters.Add(new SqlParameter("@sp_actor", Session["userid"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_action", "100099")); // Clear
                    //cmd.Parameters.Add(new SqlParameter("@sp_createdate", ""));
                    #endregion SQL Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Processing
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    int inserted = cmd.ExecuteNonQuery();
                    lblAllRecordConfirm.Text += " | Log Inserted: " + inserted.ToString();
                    #endregion SQL Processing
                    #endregion SQL - Action Log INSERT
                }
                else
                {
                    lblClearAllConfirm.Text = "The query resulted in 0 records updated; you can refresh the page and try again.<br />If the problem persists, contact G&H.";
                }
                #endregion SQL Processing
                // This populates the current query totals
                GV_Data_Query_Counts(0, this.Page.User.Identity.Name, gvQueryTotalCounts);
                // This populates the mind grid with record list
                GV_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);
            }
        }
        #endregion Using: SqlConnection
    }
    protected void Processing_All_Clear_Cancel(object sender, EventArgs e)
    {
        ClearAllRecord.Visible = false;
    }
    #endregion All Clear
    #region Undo All Clear
    protected void Processing_All_UndoClear(object sender, EventArgs e)
    {
        if (Session["gridrefresh"] == null)
        {
            UndoClearAllRecord.Visible = false;
            divUndoClearAllRecord.Visible = false;

            ExpiredAllRecord.Visible = true;
        }
        else if (DateTime.Parse(Session["gridrefresh"].ToString()) > DateTime.UtcNow.AddMinutes(-1))
        {
            UndoClearAllRecord.Visible = true;
        }
        else
        {
            UndoClearAllRecord.Visible = false;
            ExpiredAllRecord.Visible = true;
        }

    }
    protected void Processing_All_UndoClear_Confirm(object sender, EventArgs e)
    {
        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
        #region Using: SqlConnection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region SQL String Builder
                string sqlBuilder = "";
                sqlBuilder += "" + "\n";
                sqlBuilder += "UPDATE [dbo].[ivr_record]" + "\n";
                sqlBuilder += "SET [status] = 0" + "\n"; // From 99
                sqlBuilder += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "WHERE 1=1" + "\n";
                sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                sqlBuilder += GV_Query_Builder();
                sqlBuilder += "";
                #endregion SQL String Builder
                #region SQL String Builder2
                // This is for the Action Log INSERT
                string sqlBuilder2 = "";
                sqlBuilder2 += "" + "\n";
                sqlBuilder2 += "INSERT INTO [dbo].[ivr_record_action_log]" + "\n";
                sqlBuilder2 += "([sourceid],[recordid],[calldate],[calltime],[ani],[actor],[action],[createdate])" + "\n";
                sqlBuilder2 += "SELECT" + "\n";
                sqlBuilder2 += "[r].[sourceid]" + "\n";
                sqlBuilder2 += ",[r].[recordid]" + "\n";
                sqlBuilder2 += ",[r].[calldate]" + "\n";
                sqlBuilder2 += ",[r].[calltime]" + "\n";
                sqlBuilder2 += ",[r].[ani]" + "\n";
                sqlBuilder2 += ",@sp_actor" + "\n";
                sqlBuilder2 += ",@sp_action" + "\n";
                sqlBuilder2 += ",GETUTCDATE()" + "\n";
                sqlBuilder2 += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "WHERE 1=1" + "\n";
                sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                sqlBuilder2 += GV_Query_Builder();
                #endregion SQL String Builder2
                cmd.CommandText = sqlBuilder;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                //cmd.Parameters.Add(new SqlParameter("@sp_top", Convert.ToInt32(ddlTop.SelectedValue.ToString())));
                #endregion SQL Parameters
                print_sql(cmd); // Will print for Admin in Local
                #region SQL Processing
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int affected = cmd.ExecuteNonQuery();
                lblAllRecordConfirm.Text = " | Records Updated: " + affected.ToString();
                if (affected > 0)
                {
                    UndoClearAllRecord.Visible = false;
                    divUndoClearAllRecord.Visible = false;
                    UndoDiscardAllRecord.Visible = false;
                    divUndoDiscardAllRecord.Visible = false;
                    #region SQL - Action Log INSERT
                    cmd.CommandText = sqlBuilder2;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    if (Session["userid"] == null) { ghUser.identity_get_userid(); }
                    cmd.Parameters.Add(new SqlParameter("@sp_actor", Session["userid"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_action", "101099")); // Clear - Undo (?)
                    //cmd.Parameters.Add(new SqlParameter("@sp_createdate", ""));
                    #endregion SQL Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Processing
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    int inserted = cmd.ExecuteNonQuery();
                    lblAllRecordConfirm.Text += " | Log Inserted: " + inserted.ToString();
                    #endregion SQL Processing
                    #endregion SQL - Action Log INSERT
                }
                else
                {
                    lblClearAllConfirm.Text = "The query resulted in 0 records updated; you can refresh the page and try again.<br />If the problem persists, contact G&H.";
                }
                #endregion SQL Processing
                // This populates the current query totals
                GV_Data_Query_Counts(0, this.Page.User.Identity.Name, gvQueryTotalCounts);
                // This populates the mind grid with record list
                GV_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);
            }
        }
        #endregion Using: SqlConnection
    }
    protected void Processing_All_UndoClear_Cancel(object sender, EventArgs e)
    {
        UndoClearAllRecord.Visible = false;
    }
    #endregion Undo All Clear
    #region All Discard
    protected void Processing_All_Discard(object sender, EventArgs e)
    {
        if (Session["gridrefresh"] == null)
        {
            ClearAllRecord.Visible = false;
            DiscardAllRecord.Visible = false;
            ExpiredAllRecord.Visible = true;
        }
        else if (DateTime.Parse(Session["gridrefresh"].ToString()) > DateTime.UtcNow.AddMinutes(-1))
        {
            DiscardAllRecord.Visible = true;
            ClearAllRecord.Visible = false;
        }
        else
        {
            ClearAllRecord.Visible = false;
            DiscardAllRecord.Visible = false;
            ExpiredAllRecord.Visible = true;
        }
        
    }
    protected void Processing_All_Discard_Confirm(object sender, EventArgs e)
    {
        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
        try
        {
            #region Using: SqlConnection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    #region SQL String Builder
                    string sqlBuilder = "";

                    sqlBuilder += "" + "\n";
                    sqlBuilder += "UPDATE [dbo].[ivr_record]" + "\n";
                    sqlBuilder += "SET [status] = 98" + "\n";
                    sqlBuilder += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                    sqlBuilder += "WHERE 1=1" + "\n";
                    sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                    sqlBuilder += GV_Query_Builder();

                    sqlBuilder += "";

                    #endregion SQL String Builder
                    #region SQL String Builder2
                    // This is for the Action Log INSERT
                    string sqlBuilder2 = "";
                    sqlBuilder2 += "" + "\n";
                    sqlBuilder2 += "INSERT INTO [dbo].[ivr_record_action_log]" + "\n";
                    sqlBuilder2 += "([sourceid],[recordid],[calldate],[calltime],[ani],[actor],[action],[createdate])" + "\n";
                    sqlBuilder2 += "SELECT" + "\n";
                    sqlBuilder2 += "[r].[sourceid]" + "\n";
                    sqlBuilder2 += ",[r].[recordid]" + "\n";
                    sqlBuilder2 += ",[r].[calldate]" + "\n";
                    sqlBuilder2 += ",[r].[calltime]" + "\n";
                    sqlBuilder2 += ",[r].[ani]" + "\n";
                    sqlBuilder2 += ",@sp_actor" + "\n";
                    sqlBuilder2 += ",@sp_action" + "\n";
                    sqlBuilder2 += ",GETUTCDATE()" + "\n";
                    sqlBuilder2 += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                    sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                    sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                    sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                    sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                    sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                    sqlBuilder2 += "WHERE 1=1" + "\n";
                    sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                    sqlBuilder2 += GV_Query_Builder();
                    #endregion SQL String Builder2
                    cmd.CommandText = sqlBuilder;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    //cmd.Parameters.Add(new SqlParameter("@sp_top", Convert.ToInt32(ddlTop.SelectedValue.ToString())));
                    #endregion SQL Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Processing
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    int affected = cmd.ExecuteNonQuery();
                    lblAllRecordConfirm.Text = "Records Updated: " + affected.ToString();
                    if (affected > 0)
                    {
                        ClearAllRecord.Visible = false;
                        divClearAllRecord.Visible = false;
                        DiscardAllRecord.Visible = false;
                        divDiscardAllRecord.Visible = false;
                        #region SQL - Action Log INSERT
                        cmd.CommandText = sqlBuilder2;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #region SQL Parameters
                        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
                        cmd.Parameters.Add(new SqlParameter("@sp_actor", Session["userid"].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@sp_action", "100098")); // Discard
                        //cmd.Parameters.Add(new SqlParameter("@sp_createdate", ""));
                        #endregion SQL Parameters
                        print_sql(cmd); // Will print for Admin in Local
                        #region SQL Processing
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        int inserted = cmd.ExecuteNonQuery();
                        lblAllRecordConfirm.Text += " | Log Inserted: " + inserted.ToString();
                        #endregion SQL Processing
                        #endregion SQL - Action Log INSERT
                    }
                    else
                    {
                        lblClearAllConfirm.Text = "The query resulted in 0 records updated; you can refresh the page and try again.<br />If the problem persists, contact G&H.";
                    }
                    #endregion SQL Processing
                    // This populates the current query totals
                    GV_Data_Query_Counts(0, this.Page.User.Identity.Name, gvQueryTotalCounts);
                    // This populates the mind grid with record list
                    GV_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);
                }
            }
            #endregion Using: SqlConnection
        }
        catch (Exception ex)
        {
            Error_Save(ex, "Processing All Discard Confirm");
        }
    }
    protected void Processing_All_Discard_Cancel(object sender, EventArgs e)
    {
        DiscardAllRecord.Visible = false;
    }
    #endregion Record Discard
    #region Undo All Discard
    protected void Processing_All_UndoDiscard(object sender, EventArgs e)
    {
        if (Session["gridrefresh"] == null)
        {
            UndoDiscardAllRecord.Visible = false;
            ExpiredAllRecord.Visible = true;
        }
        else if (DateTime.Parse(Session["gridrefresh"].ToString()) > DateTime.UtcNow.AddMinutes(-1))
        {
            UndoDiscardAllRecord.Visible = true;
        }
        else
        {
            UndoDiscardAllRecord.Visible = false;
            ExpiredAllRecord.Visible = true;
        }

    }
    protected void Processing_All_UndoDiscard_Confirm(object sender, EventArgs e)
    {
        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
        #region Using: SqlConnection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region SQL String Builder
                string sqlBuilder = "";
                sqlBuilder += "" + "\n";
                sqlBuilder += "UPDATE [dbo].[ivr_record]" + "\n";
                sqlBuilder += "SET [status] = 0" + "\n"; // From 98
                sqlBuilder += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                sqlBuilder += "WHERE 1=1" + "\n";
                sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                sqlBuilder += GV_Query_Builder();
                sqlBuilder += "";
                #endregion SQL String Builder
                #region SQL String Builder2
                // This is for the Action Log INSERT
                string sqlBuilder2 = "";
                sqlBuilder2 += "" + "\n";
                sqlBuilder2 += "INSERT INTO [dbo].[ivr_record_action_log]" + "\n";
                sqlBuilder2 += "([sourceid],[recordid],[calldate],[calltime],[ani],[actor],[action],[createdate])" + "\n";
                sqlBuilder2 += "SELECT" + "\n";
                sqlBuilder2 += "[r].[sourceid]" + "\n";
                sqlBuilder2 += ",[r].[recordid]" + "\n";
                sqlBuilder2 += ",[r].[calldate]" + "\n";
                sqlBuilder2 += ",[r].[calltime]" + "\n";
                sqlBuilder2 += ",[r].[ani]" + "\n";
                sqlBuilder2 += ",@sp_actor" + "\n";
                sqlBuilder2 += ",@sp_action" + "\n";
                sqlBuilder2 += ",GETUTCDATE()" + "\n";
                sqlBuilder2 += "FROM [dbo].[ivr_record] [r] WITH(NOLOCK)" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_vc] [vc] WITH(NOLOCK) ON [vc].[sourceid] = [r].[sourceid] AND [vc].[recordid] = [r].[recordid] AND [vc].[calldate] = [r].[calldate] AND [vc].[calltime] = [r].[calltime] AND [vc].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_rani] [rn] WITH(NOLOCK) ON [rn].[sourceid] = [r].[sourceid] AND [rn].[recordid] = [r].[recordid] AND [rn].[calldate] = [r].[calldate] AND [rn].[calltime] = [r].[calltime] AND [rn].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_opt] [op] WITH(NOLOCK) ON [op].[sourceid] = [r].[sourceid] AND [op].[recordid] = [r].[recordid] AND [op].[calldate] = [r].[calldate] AND [op].[calltime] = [r].[calltime] AND [op].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "LEFT OUTER JOIN [dbo].[ivr_record_cat] [ct] WITH(NOLOCK) ON [ct].[sourceid] = [r].[sourceid] AND [ct].[recordid] = [r].[recordid] AND [ct].[calldate] = [r].[calldate] AND [ct].[calltime] = [r].[calltime] AND [ct].[ani] = [r].[ani]" + "\n";
                sqlBuilder2 += "WHERE 1=1" + "\n";
                sqlBuilder += "AND ([r].[file_vc] IS NOT NULL OR [r].[file_cc] IS NOT NULL OR [r].[file_rani] IS NOT NULL OR [r].[file_opt] IS NOT NULL OR [r].[file_cat] IS NOT NULL)\n";
                sqlBuilder2 += GV_Query_Builder();
                #endregion SQL String Builder2
                cmd.CommandText = sqlBuilder;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                //cmd.Parameters.Add(new SqlParameter("@sp_top", Convert.ToInt32(ddlTop.SelectedValue.ToString())));
                #endregion SQL Parameters
                print_sql(cmd); // Will print for Admin in Local
                #region SQL Processing
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int affected = cmd.ExecuteNonQuery();
                lblAllRecordConfirm.Text = "Records Updated: " + affected.ToString();
                if (affected > 0)
                {
                    UndoClearAllRecord.Visible = false;
                    divUndoClearAllRecord.Visible = false;
                    UndoDiscardAllRecord.Visible = false;
                    divUndoDiscardAllRecord.Visible = false;
                    #region SQL - Action Log INSERT
                    cmd.CommandText = sqlBuilder2;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    if (Session["userid"] == null) { ghUser.identity_get_userid(); }
                    cmd.Parameters.Add(new SqlParameter("@sp_actor", Session["userid"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@sp_action", "101098")); // Discard - Undo (?)
                    //cmd.Parameters.Add(new SqlParameter("@sp_createdate", ""));
                    #endregion SQL Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Processing
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    int inserted = cmd.ExecuteNonQuery();
                    lblAllRecordConfirm.Text += " | Log Inserted: " + inserted.ToString();
                    #endregion SQL Processing
                    #endregion SQL - Action Log INSERT
                }
                else
                {
                    lblUndoClearAllConfirm.Text = "The query resulted in 0 records updated; you can refresh the page and try again.<br />If the problem persists, contact G&H.";
                }
                #endregion SQL Processing
                // This populates the current query totals
                GV_Data_Query_Counts(0, this.Page.User.Identity.Name, gvQueryTotalCounts);
                // This populates the mind grid with record list
                GV_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);
            }
        }
        #endregion Using: SqlConnection
    }
    protected void Processing_All_UndoDiscard_Cancel(object sender, EventArgs e)
    {
        UndoDiscardAllRecord.Visible = false;
    }
    #endregion Undo Record Discard

    /// <summary>
    /// Control how the DetailsView is handled for each individual section
    /// This providers a much higher level of validation and security
    /// Also allows for full customization of what the update command does
    /// http://www.c-sharpcorner.com/uploadfile/raj1979/using-Asp-Net-detailsview-control-without-sqldatasource/
    /// </summary>
    /// <param name="UserID"></param>
    #region Details View Handling
    /// <summary>
    /// Get the data
    /// </summary>
    /// <param name="UserID"></param>
    protected void DetailsView_Data(Int32 sourceid, Int32 recordid, String calldate, String calltime, String ani, DetailsView dv)
    {
        Label16.Text = String.Format("{0}|{1}|{2}|{3}|{4}"
            , sourceid
            , recordid
            , calldate
            , calltime
            , ani
            );
           
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                String cmdText = "";
                if (dv.ID == "dv_ivr_file_vc") { cmdText = "[dbo].[sp_ivr_search_get_vc]"; }
                if (dv.ID == "dv_ivr_file_cc") { cmdText = "[dbo].[sp_ivr_search_get_cc]"; }
                if (dv.ID == "dv_ivr_file_rn") { cmdText = "[dbo].[sp_ivr_search_get_rn]"; }
                if (dv.ID == "dv_ivr_file_op") { cmdText = "[dbo].[sp_ivr_search_get_op]"; }
                if (dv.ID == "dv_ivr_file_ct") { cmdText = "[dbo].[sp_ivr_search_get_ct]"; }
                if (dv.ID == "dv_ivr_file_main") { cmdText = "[dbo].[sp_ivr_search_get_main]"; }
                if (dv.ID == "dv_ivr_file_drtv") { cmdText = "[dbo].[sp_ivr_search_get_drtv]"; }
                if (dv.ID == "dv_ivr_file_record") { cmdText = "[dbo].[sp_ivr_search_get_info]"; }
                
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_sourceid", sourceid));
                cmd.Parameters.Add(new SqlParameter("@sp_recordid", recordid));
                cmd.Parameters.Add(new SqlParameter("@sp_calldate", calldate));
                cmd.Parameters.Add(new SqlParameter("@sp_calltime", calltime));
                cmd.Parameters.Add(new SqlParameter("@sp_ani", ani));
                #endregion SQL Parameters
                print_sql(cmd); // Will print for Admin in Local
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                dv.DataBind(); //dv_ivr_file_vc.DataBind();
                //dtlLabel.Text += "<br />" + dv.ID;
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    /// <summary>
    /// 
    /// </summary>
    protected void DetailsView_DataBound(object sender, EventArgs e)
    {
        #region DataBound Action
        DetailsView dv = (DetailsView)sender;
        #region DataBound Action for dv_ivr_file_vc
        if (dv.ID == "dv_ivr_file_vc")
        {
            // Nothing
        }
        #endregion DataBound Action for dv_ivr_file_vc
        #region DataBound Action for dv_ivr_file_cc
        else if (dv.ID == "dv_ivr_file_cc")
        {
            // Nothing
        }
        #endregion DataBound Action for dv_ivr_file_cc
        #region DataBound Action for dv_ivr_file_rn
        else if (dv.ID == "dv_ivr_file_rn")
        {
            // Nothing
        }
        #endregion DataBound Action for dv_ivr_file_rn
        #region DataBound Action for dv_ivr_file_op
        else if (dv.ID == "dv_ivr_file_op")
        {
            // Populate the State/Country Drop Down
            #region State/Country Populate if Edit Mode
            if (dv_ivr_file_op.CurrentMode == DetailsViewMode.Edit)
            {
                Label lblState = (Label)dv_ivr_file_op.FindControl("State");

                DropDownList dvState = (DropDownList)dv_ivr_file_op.FindControl("ddlState");
                DropDownList dvCountry = (DropDownList)dv_ivr_file_op.FindControl("ddlCountry");
                if (dvState != null && dvCountry != null)
                {
                    //dtlLabel.Text = "Found DDL 1";
                    if (lblState != null)
                    {
                        //dtlLabel.Text += " [" + lblState.Text + "]";
                    }
                    try
                    {
                        Populate_StateProvinceCountry(dvState, null, dvCountry);
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, "dv_ivr_file_op - DDL State/Country Populate Error");
                    }
                }
                else if (dvState != null)
                {
                    //dtlLabel.Text = "Found DDL 2";
                    try
                    {
                        Populate_StateProvinceCountry(dvState, null, null);
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, "dv_ivr_file_op - DDL State Populate Error");
                    }
                }
                else if (dvCountry != null)
                {
                    //dtlLabel.Text = "Found DDL 3";
                    try
                    {
                        Populate_StateProvinceCountry(null, null, dvCountry);
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, "dv_ivr_file_op - DDL Country Populate Error");
                    }
                }
                else
                {
                    //dtlLabel.Text = "Did not find DDL";
                    #region DeBug Code
                    //dtlLabel.Text += " [" + dv_ivr_file_op.Rows.Count.ToString() + "]";
                    //foreach (DetailsViewRow dvr in dv_ivr_file_op.Rows)
                    //{
                    //    dtlLabel.Text += "<br />" + dvr.Cells.Count.ToString();
                    //    dtlLabel.Text += " - " + dvr.Cells[0].Text.ToString();
                    //    dtlLabel.Text += " - " + dvr.Cells[0].Controls.Count.ToString();
                    //    dtlLabel.Text += " - " + dvr.Cells[1].Controls.Count.ToString();
                    //    //dtlLabel.Text += " - " + dvr.
                    //    foreach (Control ctl in dvr.Cells[1].Controls)
                    //    {
                    //        dtlLabel.Text += "<br />---" + ctl.ClientID.ToString();
                    //    }

                    //    //dvr.TemplateControl.FindControl
                    //    DropDownList dvState2 = (DropDownList)dvr.Cells[1].TemplateControl.FindControl("ddlState");
                    //    if (dvState2 != null)
                    //    {
                    //        //dtlLabel.Text = "Found DDL";
                    //        dtlLabel.Text += " [" + dvr.ID.ToString() + "]";
                    //        break;
                    //    }
                    //} 
                    #endregion
                }
                //dtlLabel.Text += " finished searching";

            }
            #endregion
            else
            {
                ////dtlLabel.Text = "Other Mode";
            }

        }
        #endregion DataBound Action for dv_ivr_file_op
        #region DataBound Action for dv_ivr_file_ct
        else if (dv.ID == "dv_ivr_file_ct")
        {
        }
        #endregion DataBound Action for dv_ivr_file_ct
        #endregion DataBound Action
    }
    protected void DetailsView_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        #region ItemCommand Action
        if (e.CommandName == "Clear")
        {
            gvSearchResults.SelectedIndex = -1;
            dv_ivr_file_vc.DataBind();
            dv_ivr_file_cc.DataBind();
            dv_ivr_file_rn.DataBind();
            dv_ivr_file_op.DataBind();
            dv_ivr_file_ct.DataBind();
            dv_ivr_file_main.DataBind();
            dv_ivr_file_drtv.DataBind();
            dv_ivr_file_record.DataBind();
        }
        if (e.CommandName == "Update")
        {
            DetailsView dv = (DetailsView)sender;
            WriteToLabel("new", "Blue", dv.ID + " Update Processed [" + DateTime.Now.ToString("HH:mm:ss") + "]<br />", dtlLabel);
        }
        //dtlLabel.Text = e.CommandName;
        #endregion ItemCommand Action
    }
    protected void DetailsView_ModeChanging(object sender, DetailsViewModeEventArgs e)
    {
        #region ModeChanging Action
        DetailsView dv = (DetailsView)sender;
        dv.ChangeMode(e.NewMode);
        try
        {
            Int32 callid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
            Int32 donorid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[1].ToString());
            //DetailsView_Data(callid, donorid, dv);
        }
        catch (Exception ex)
        {
            Error_Save(ex, "DetailsView_ModeChanging");
        }
        if (e.NewMode == DetailsViewMode.Edit)
        {
            dv.AllowPaging = false;
        }
        else
        {
            dv.AllowPaging = true;
        }
        #endregion ModeChanging Action
    }
    /// <summary>
    /// Here we perform the update action
    /// There is some slight validation, all done in the backend code
    /// If we update successfully, we switch the DV to ReadOnly mode
    /// If we fail to update, we leave the data as is and give the user an error
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DetailsView_ItemUpdating2(object sender, DetailsViewUpdateEventArgs e)
    {
        dtlLabel.Text = "DetailsView_ItemUpdating";
        DetailsView dv = (DetailsView)sender;
        dv.ChangeMode(DetailsViewMode.ReadOnly);
        Int32 callid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
        Int32 donorid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[1].ToString());
        //DetailsView_Data(callid, donorid, dv);
    }
    protected void DetailsView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        Boolean update = false;
        String error = "";
        #region ItemUpdating Action
        DetailsView dv = (DetailsView)sender;
        #region ItemUpdating Action for dv_ivr_file_vc
        if (dv.ID == "dv_ivr_file_vc")
        {
            #region Try: dv_ivr_file_vc Update
            try
            {
                DropDownList uptRole = (DropDownList)dv.FindControl("ddlRole");
                DropDownList uptClient = (DropDownList)dv.FindControl("ddlClient");
                String UserID = ((HiddenField)dv.FindControl("UserID")).Value;
                String RoleID = ((HiddenField)dv.FindControl("RoleID")).Value;
                String ClientID = ((HiddenField)dv.FindControl("ClientID")).Value;
                String ModuleID = ((HiddenField)dv.FindControl("ModuleID")).Value;

                if (uptRole != null) { update = true; dtlLabel.Text += "<br />Role: " + uptRole.SelectedValue; }
                if (update)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "[dbo].[user_update_user_credentials]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@Role", uptRole.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@RoleOld", RoleID));
                        cmd.Parameters.Add(new SqlParameter("@Client", uptClient.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@ClientOld", ClientID));
                        //cmd.Parameters.Add(new SqlParameter("@ModuleOld", ModuleID));
                        DetailsView_UpdateRecord(cmd);
                    }
                }
            }
            #endregion Try: dv_ivr_file_vc Update
            #region Catch: dv_ivr_file_vc Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Credentials");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: dv_ivr_file_vc Update
        }
        #endregion ItemUpdating Action for dv_ivr_file_vc
        #region ItemUpdating Action for dv_ivr_file_cc
        else if (dv.ID == "dv_ivr_file_cc")
        {
            #region Try: dv_ivr_file_cc Update
            try
            {
                DropDownList uptPrefix = (DropDownList)dv.FindControl("ddlPrefix");
                TextBox uptFirstName = (TextBox)dv.FindControl("FirstName");
                TextBox uptMiddleName = (TextBox)dv.FindControl("MiddleName");
                TextBox uptLastName = (TextBox)dv.FindControl("LastName");
                DropDownList uptSuffix = (DropDownList)dv.FindControl("ddlSuffix");

                if (uptFirstName != null
                    && uptFirstName.Text.Length > 0
                    && uptLastName != null
                    && uptLastName.Text.Length > 0
                    )
                {
                    update = true;
                }

                if (uptPrefix != null) { dtlLabel.Text += "<br />Prefix: " + uptPrefix.SelectedValue; } else { update = false; }
                if (uptFirstName != null) { dtlLabel.Text += "<br />FirstName: " + uptFirstName.Text; } else { update = false; }
                if (uptMiddleName != null) { dtlLabel.Text += "<br />MiddleName: " + uptMiddleName.Text; } else { update = false; }
                if (uptLastName != null) { dtlLabel.Text += "<br />LastName: " + uptLastName.Text; } else { update = false; }
                if (uptSuffix != null) { dtlLabel.Text += "<br />Suffix: " + uptSuffix.SelectedValue; } else { update = false; }

                if (update)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        Int32 UserID = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
                        cmd.CommandText = "[dbo].[user_update_user_details]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@SP_UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@uptPrefix", uptPrefix.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@uptFirstName", uptFirstName.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@uptMiddleName", uptMiddleName.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@uptLastName", uptLastName.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@uptSuffix", uptSuffix.SelectedValue));
                        DetailsView_UpdateRecord(cmd);
                    }
                }
            }
            #endregion Try: dv_ivr_file_cc Update
            #region Catch: dv_ivr_file_cc Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Details");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: dv_ivr_file_cc Update
        }
        #endregion ItemUpdating Action for dv_ivr_file_cc
        #region ItemUpdating Action for dv_ivr_file_rn
        else if (dv.ID == "dv_ivr_file_rn")
        {
            #region Try: dv_ivr_file_rn Update
            try
            {
                TextBox uptPhoneArea = (TextBox)dv.FindControl("PhoneArea");
                TextBox uptPhonePrefix = (TextBox)dv.FindControl("PhonePrefix");
                TextBox uptPhoneSuffix = (TextBox)dv.FindControl("PhoneSuffix");

                if (uptPhoneArea != null
                    && uptPhoneArea.Text.Length == 3
                    && uptPhonePrefix != null
                    && uptPhonePrefix.Text.Length == 3
                    && uptPhoneSuffix != null
                    && uptPhoneSuffix.Text.Length == 4
                    )
                {
                    update = true;
                }
                else
                {
                    error = "You can not blank out the phone number at this time.";
                }
                if (uptPhoneArea != null) { dtlLabel.Text += "<br />PhoneArea: " + uptPhoneArea.Text; } else { update = false; }
                if (uptPhonePrefix != null) { dtlLabel.Text += "<br />PhonePrefix: " + uptPhonePrefix.Text; } else { update = false; }
                if (uptPhoneSuffix != null) { dtlLabel.Text += "<br />PhoneSuffix: " + uptPhoneSuffix.Text; } else { update = false; }
                if (update)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        Int32 UserID = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
                        cmd.CommandText = "[dbo].[user_update_user_phone]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@SP_UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@uptPhoneArea", uptPhoneArea.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptPhonePrefix", uptPhonePrefix.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptPhoneSuffix", uptPhoneSuffix.Text));
                        DetailsView_UpdateRecord(cmd);
                    }
                }
            }
            #endregion Try: dv_ivr_file_rn Update
            #region Catch: dv_ivr_file_rn Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Phone");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: dv_ivr_file_rn Update
        }
        #endregion ItemUpdating Action for dv_ivr_file_rn
        #region ItemUpdating Action for dv_ivr_file_op
        else if (dv.ID == "dv_ivr_file_op")
        {
            #region Try: dv_ivr_file_op Update
            try
            {
                TextBox uptAddress1 = (TextBox)dv.FindControl("Address1");
                TextBox uptAddress2 = (TextBox)dv.FindControl("Address2");
                TextBox uptAddress3 = (TextBox)dv.FindControl("Address3");
                TextBox uptCity = (TextBox)dv.FindControl("City");
                TextBox uptZip = (TextBox)dv.FindControl("Zip");
                DropDownList uptState = (DropDownList)dv.FindControl("ddlState");
                DropDownList uptCountry = (DropDownList)dv.FindControl("ddlCountry");

                if (uptAddress1 != null
                    && uptAddress2 != null
                    && uptAddress3 != null
                    && uptCity != null
                    && uptState != null
                    && uptZip != null
                    && uptCountry != null
                    )
                {
                    update = true;
                }

                #region This is a Server Side check - If we failed, the code is broken (Possible Browser Related Issue)
                if (uptAddress1 != null) { dtlLabel.Text += "<br />Address1: " + uptAddress1.Text; } else { update = false; }
                if (uptAddress2 != null) { dtlLabel.Text += "<br />Address2: " + uptAddress2.Text; } else { update = false; }
                if (uptAddress3 != null) { dtlLabel.Text += "<br />Address3: " + uptAddress3.Text; } else { update = false; }
                if (uptCity != null) { dtlLabel.Text += "<br />City: " + uptCity.Text; } else { update = false; }
                if (uptState != null) { dtlLabel.Text += "<br />State: " + uptState.SelectedValue; } else { update = false; }
                if (uptZip != null) { dtlLabel.Text += "<br />Zip: " + uptZip.Text; } else { update = false; }
                if (uptCountry != null) { dtlLabel.Text += "<br />Country: " + uptCountry.SelectedValue; } else { update = false; }
                #endregion This is a Server Side check - If we failed, the code is broken (Possible Browser Related Issue)

                if (update)
                {
                    // Here we should process the validation
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        Int32 UserID = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
                        cmd.CommandText = "[dbo].[user_update_user_address]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@SP_UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@uptAddress1", uptAddress1.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptAddress2", uptAddress2.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptAddress3", uptAddress3.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptCity", uptCity.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptState", uptState.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@uptZip", uptZip.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptCountry", uptCountry.SelectedValue));
                        DetailsView_UpdateRecord(cmd);
                    }
                }
            }
            #endregion Try: dv_ivr_file_op Update
            #region Catch: dv_ivr_file_op Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Address");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: dv_ivr_file_op Update

        }
        #endregion ItemUpdating Action for dv_ivr_file_op
        if (update)
        {
            dv.ChangeMode(DetailsViewMode.ReadOnly);
            Int32 callid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
            Int32 donorid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[1].ToString());
            //DetailsView_Data(callid, donorid, dv);
            GV_Data_Query(0, "", gvSearchResults);
        }
        else
        {
            WriteToLabel("add", "Cyan", "<br /><br />There was an error updating your record.<br />Please review the below message:", dtlLabel);
            WriteToLabel("add", "Red", "<br /><br />" + error, dtlLabel);
        }
        #endregion ItemUpdating Action
    }
    protected void DetailsView_UpdateRecord(SqlCommand cmd)
    {
        #region Using: SqlConnection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            using (cmd)
            {
                cmd.Connection = con;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;

                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            try
                            {
                                WriteToLabel("add", "Blue", "<br />" + sqlRdr[0].ToString(), dtlLabel);
                            }
                            catch
                            {
                                WriteToLabel("add", "Red", "<br />" + "oops?", dtlLabel);
                            }
                        }
                    }
                    else
                    {
                        WriteToLabel("add", "Red", "<br />" + "No Rows", dtlLabel);
                    }
                }
            }
        }
        #endregion Using: SqlConnection
    }
    protected void DetailsView_ItemUpdated(object sender, EventArgs e)
    {
        #region ItemUpdated Action
        dtlLabel.Text = "DetailsView_ItemUpdated";
        #endregion ItemUpdated Action
    }
    #endregion Details View Handling
    #region Record Clear
    protected void Processing_Record_Clear(object sender, EventArgs e)
    {
        ClearRecord.Visible = true;
    }
    protected void Processing_Record_Clear_Confirm(object sender, EventArgs e)
    {
        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
        #region Using: SqlConnection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                cmd.CommandText = "[dbo].[sp_ivr_search_clear]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                Int32 sourceid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
                Int32 recordid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[1].ToString());
                String calldate = gvSearchResults.SelectedDataKey.Values[2].ToString();
                String calltime = gvSearchResults.SelectedDataKey.Values[3].ToString();
                String ani = gvSearchResults.SelectedDataKey.Values[4].ToString();
                cmd.Parameters.Add(new SqlParameter("@sp_sourceid", sourceid));
                cmd.Parameters.Add(new SqlParameter("@sp_recordid", recordid));
                cmd.Parameters.Add(new SqlParameter("@sp_calldate", calldate));
                cmd.Parameters.Add(new SqlParameter("@sp_calltime", calltime));
                cmd.Parameters.Add(new SqlParameter("@sp_ani", ani));
                cmd.Parameters.Add(new SqlParameter("@sp_userid", Session["userid"].ToString()));
                #endregion SQL Parameters
                print_sql(cmd); // Will print for Admin in Local
                #region SQL Processing
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int affected = cmd.ExecuteNonQuery();
                btnClearRecordConfirm.Enabled = false;
                btnClearRecordCancel.Enabled = false;
                lblClearRecordConfirm.Text = "Records Updated: " + affected.ToString();
                #endregion SQL Processing
                GV_Data_Query(0, "", gvSearchResults);
            }
        }
        #endregion Using: SqlConnection
    }
    protected void Processing_Record_Clear_Cancel(object sender, EventArgs e)
    {
        ClearRecord.Visible = false;
    }
    #endregion Record Clear
    #region Record Discard
    protected void Processing_Record_Discard(object sender, EventArgs e)
    {
        DiscardRecord.Visible = true;
    }
    protected void Processing_Record_Discard_Confirm(object sender, EventArgs e)
    {
        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
        #region Using: SqlConnection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                cmd.CommandText = "[dbo].[sp_ivr_search_discard]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                Int32 sourceid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
                Int32 recordid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[1].ToString());
                String calldate = gvSearchResults.SelectedDataKey.Values[2].ToString();
                String calltime = gvSearchResults.SelectedDataKey.Values[3].ToString();
                String ani = gvSearchResults.SelectedDataKey.Values[4].ToString();
                cmd.Parameters.Add(new SqlParameter("@sp_sourceid", sourceid));
                cmd.Parameters.Add(new SqlParameter("@sp_recordid", recordid));
                cmd.Parameters.Add(new SqlParameter("@sp_calldate", calldate));
                cmd.Parameters.Add(new SqlParameter("@sp_calltime", calltime));
                cmd.Parameters.Add(new SqlParameter("@sp_ani", ani));
                cmd.Parameters.Add(new SqlParameter("@sp_userid", Session["userid"].ToString()));
                #endregion SQL Parameters
                print_sql(cmd); // Will print for Admin in Local
                #region SQL Processing
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int affected = cmd.ExecuteNonQuery();
                btnDiscardRecordConfirm.Enabled = false;
                btnDiscardRecordCancel.Enabled = false;
                lblDiscardRecordConfirm.Text = "Records Updated: " + affected.ToString();
                #endregion SQL Processing
                GV_Data_Query(0, "", gvSearchResults);
                gvSearchResults.SelectedIndex = -1;
            }
        }
        #endregion Using: SqlConnection
    }
    protected void Processing_Record_Discard_Cancel(object sender, EventArgs e)
    {
        DiscardRecord.Visible = false;
    }
    #endregion Record Discard
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

        ErrorLog.ErrorLog_Save(ex, dv, "Portal: ARC", error, spPage, spQS, spURL);
    }

    protected void lookup_zipcode()
    {
        // Get the logged in users userid
        // This should be retrieved during the login process
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SqlCommand cmd
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Populate the SQL Command
                    string sqlBuilder = "";
                    sqlBuilder += "SELECT" + "\n";
                    sqlBuilder += "TOP 1" + "\n";
                    sqlBuilder += "[zip]" + "\n";
                    sqlBuilder += ",[latitude]" + "\n";
                    sqlBuilder += ",[longitude]" + "\n";
                    sqlBuilder += ",[city]" + "\n";
                    sqlBuilder += ",[state]" + "\n";
                    sqlBuilder += ",[abbr]" + "\n";
                    sqlBuilder += "FROM [dbo].[zipData]" + "\n";
                    sqlBuilder += "WHERE [zip] = @sp_postalcode" + "\n";
                    sqlBuilder += "";
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = sqlBuilder;
                    cmd.CommandType = CommandType.Text;
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@sp_postalcode", "10459"));
                    string cmdText = "\n" + cmd.CommandText;
                    bool cmdFirst = true;
                    foreach (SqlParameter param in cmd.Parameters)
                    {
                        cmdText += "\n" + ((cmdFirst) ? "" : ",") + param.ParameterName + " = " + ((param.Value != null) ? "'" + param.Value.ToString() + "'" : "default");
                        cmdFirst = false;
                    }
                    #endregion Populate the SQL Params
                    #region Process SQL Command - Try
                    Label8.Text = "Results:";
                    try
                    {
                        Label8.Text += "|Try";
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            Label8.Text += "|Reader";
                            if (sqlRdr.HasRows)
                            {
                                Label8.Text += "|Rows";
                                while (sqlRdr.Read())
                                {
                                    Label8.Text += "<br />" + sqlRdr["city"].ToString();
                                    Label8.Text += "<br />" + sqlRdr["abbr"].ToString();
                                }
                            }
                            else
                            {
                                Label8.Text += "|Blank";
                                //arcNewID = 0;
                            }
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                    }
                    #endregion Process SQL Command - Catch
                }
                #endregion SqlCommand cmd
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Save(ex, "User Get UserID");
        }
    }
    protected void print_sql(SqlCommand cmd)
    {
        print_sql(cmd, sqlPrint, "append");
    }
    protected void print_sql(SqlCommand cmd, Label lblPrint, String type)
    {
        #region Print SQL
        // 
        // Connection.GetConnectionType() == "Local"
        if (1 == 2)
        {
            if (Page.User.IsInRole("System Administrator") == true && Page.User.Identity.Name == "nciambotti@greenwoodhall.com")
            {
                String sqlToText = "";
                sqlToText += cmd.CommandText.ToString().Replace("\n", "<br />");
                foreach (SqlParameter p in cmd.Parameters)
                {
                    sqlToText += "<br />" + p.ParameterName + " = " + p.Value.ToString() + " [" + p.DbType.ToString() + "]";
                }
                // new == we make this a new write | else we append
                if (type == "new") { lblPrint.Text = ""; }
                //lblPrint.Text = "<hr />" + sqlToText + "<hr />" + lblPrint.Text;
                lblPrint.Text = String.Format("<hr />Print: {0}<br />{1}{2}", DateTime.UtcNow.ToString(), sqlToText, lblPrint.Text);
            }

        }
        #endregion Print SQL
    }

}
