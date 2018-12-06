using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using System.IO;
using System.Net;
using System.Configuration;
using System.Collections.Generic;
public partial class Search_Calls : System.Web.UI.Page
{
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    private String sqlStrDE = Connection.GetConnectionString("DE_Production", ""); // Not used
    private String sqlStrR = Connection.GetConnectionString("RECORDINGS", ""); // Not used
    private String sqlCon = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    private int tempRow = 0;
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Search Calls";
        Page.Title = "Search Calls";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
        if (Connection.GetDBMode() == "Stage")
        {
            sqlCon = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        lblErrorDV.Text = "";
        if (!IsPostBack)
        {
            DateTime sp_datestart = DateTime.Parse(DateTime.UtcNow.AddDays(-10).ToString("MM/dd/yyyy 00:00:00"));
            dtStartDate.Text = sp_datestart.ToString("MM/dd/yyyy");
            Search_Grid_Refresh();
            if (!IsPostBack && Request["interaction"] != null)
            {
                if (gvSearchGrid.Rows.Count > 0)
                {
                    gvSearchGrid.SelectedIndex = 0;
                    if (gvSearchGrid.SelectedIndex >= 0) GridView_IndexChanged(gvSearchGrid, e);
                }
            }
        }

    }
    protected void Search_Grid_Refresh()
    {
        Search_Fetch(gvSearchGrid);
        Search_Fetch_Recordings(gvSearchGrid);
    }
    protected void Search_Fetch_Recordings(GridView gv)
    {
        List<ghStaticClass.recording_cache> rcrdcachelist = new List<ghStaticClass.recording_cache>();
        ghStaticClass.recording_cache rcrdnew = new ghStaticClass.recording_cache();
        #region Recordings
        /// Loop through the Grid and Update the Recording Column
        /// We do this after the databound so we only hit the Recording DB once
        if (gv.Rows.Count > 0)
        {
            lblRecordings.Text = "<br />Has Rows";
            int rws = 0;
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrR))
            {
                #region Process Rows
                foreach (GridViewRow dr in gv.Rows)
                {
                    rws++;
                    /// Get the callid if there is one
                    /// Get the recording column and cell
                    /// Fetch the Recording Count
                    /// Update the column
                    /// 
                    #region Get the Call ID
                    Int32 callid = -1;
                    for (int i = 0; i < dr.Cells.Count; i++)
                    {
                        if (gv.HeaderRow.Cells[i].Text == "call.call_id")
                        {
                            if (!Int32.TryParse(dr.Cells[i].Text, out callid)) callid = 0;
                            break;
                        }
                    }
                    #endregion Get the Call ID
                    #region Process CallID
                    if (callid > 0)
                    {
                        rcrdnew = new ghStaticClass.recording_cache();
                        rcrdnew.callid = callid;
                        for (int i = 0; i < dr.Cells.Count; i++)
                        {
                            if (gv.HeaderRow.Cells[i].Text == "recordings")
                            {
                                // We should have control here, we need to update the label
                                if (dr.Cells[i].HasControls())
                                {
                                    foreach (Control cntrl in dr.Cells[i].Controls)
                                    {
                                        if (cntrl.GetType().ToString() == "System.Web.UI.LiteralControl")
                                        {

                                        }
                                        else
                                        {
                                            Label lbl = (Label)cntrl;
                                            if (lbl != null && lbl.ID == "recordings")
                                            {
                                                #region Find record in Session Cache
                                                rcrdnew = RecordingCount_From_Cache_Session(rcrdnew, lbl);
                                                #endregion Find record in Session Cache

                                                #region Find record in SQL Cache
                                                if (!rcrdnew.fetched) rcrdnew = RecordingCount_From_Cache_SQL(rcrdnew, lbl);
                                                #endregion Find record in SQL Cache
                                                
                                                #region Find record in MiddleWare
                                                if (!rcrdnew.fetched) rcrdnew = RecordingCount_From_MiddleWare_SQL(con, rcrdnew, lbl);
                                                #endregion Find record in MiddleWare
                                                
                                                #region Fetch it from MiddleWare
                                                if (rcrdnew.fetched)
                                                {
                                                    lbl.Text = rcrdnew.count.ToString();
                                                    //lbl.Text += rcrdnew.source;
                                                    // Add it to the Session && SQL
                                                    rcrdcachelist.Add(rcrdnew);
                                                    if (rcrdnew.source != "DE-SQL")
                                                    {
                                                        RecordingCount_AddTo_Cache_SQL(rcrdnew); // This is redundant if we fetched from DE SQL
                                                        //lbl.Text += "i";
                                                    }
                                                }
                                                #endregion Fetch it from MiddleWare
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion Process CallID
                }
                #endregion Process Rows
            }
            #endregion SQL Connection
            lblRecordings.Text = "<br />Rows: " + rws.ToString();
            Session["rcrdcachelist"] = rcrdcachelist;
        }
        #endregion Recordings
        #region Test List
        bool tstlist = false;
        if (tstlist)
        {
            lblCache.Text = "Playing with List";
            foreach (ghStaticClass.recording_cache rcrdcache in rcrdcachelist)
            {
                lblCache.Text += String.Format("<br />{0}|{1}|{2}|{3}|{4}"
                    , rcrdcache.callid
                    , rcrdcache.count
                    , rcrdcache.lastupdate
                    , rcrdcache.fetched
                    , rcrdcache.source
                    );
            }
            lblCache.Text += "<hr />Playing with Cache";
            if (Session["rcrdcachelist"] != null)
            {
                var rcrdcachelist_cache = (List<ghStaticClass.recording_cache>)HttpContext.Current.Session["rcrdcachelist"];
                {
                    foreach (ghStaticClass.recording_cache rcrdcache in rcrdcachelist_cache)
                    {
                        lblCache.Text += String.Format("<br />{0}|{1}|{2}|{3}|{4}"
                            , rcrdcache.callid
                            , rcrdcache.count
                            , rcrdcache.lastupdate
                            , rcrdcache.fetched
                            , rcrdcache.source
                            );

                    }
                }
            }
            sqlPrint.Text = ""; // Clearing the debug for now... messy
        }
        #endregion Test List
    }
    protected ghStaticClass.recording_cache RecordingCount_From_Cache_Session(ghStaticClass.recording_cache rcrdrtrn, Label lbl)
    {
        /// Pull the record from the Session if you can
        /// Check against 20 minutes for the session; that's the session timeout as well
        #region Find record in Session Cache
        if (Session["rcrdcachelist"] != null)
        {
            try
            {
                var rcrdcachelist_cache = (List<ghStaticClass.recording_cache>)HttpContext.Current.Session["rcrdcachelist"];
                foreach (ghStaticClass.recording_cache rcrdcache in rcrdcachelist_cache)
                {
                    if (rcrdrtrn.callid == rcrdcache.callid)
                    {
                        if ((DateTime.UtcNow - rcrdcache.lastupdate).TotalMinutes < 2)
                        {
                            rcrdrtrn.count = rcrdcache.count;
                            rcrdrtrn.lastupdate = rcrdcache.lastupdate;
                            rcrdrtrn.source = "SESSION";
                            rcrdrtrn.fetched = true;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Error_Display(ex, "RecordingCount_From_Cache_Session", msgLabel);
            }

        }
        #endregion Find record in Session Cache
        return rcrdrtrn;
    }
    protected ghStaticClass.recording_cache RecordingCount_From_Cache_SQL(ghStaticClass.recording_cache rcrdrtrn, Label lbl)
    {
        #region Find Record in SQL Cache
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region Build cmdText
                String cmdText = "";
                cmdText += @"
                                IF EXISTS(SELECT TOP 1 1 FROM [dbo].[five9_recording_count_cache] [cr] WITH(NOLOCK) WHERE [cr].[CallId] = @sp_callid)
                                BEGIN
	                                SELECT
	                                TOP 1
	                                [cr].[count]
	                                ,[cr].[lastupdate]
	                                FROM [dbo].[five9_recording_count_cache] [cr] WITH(NOLOCK)
	                                WHERE [cr].[CallId] = @sp_callid
                                END
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                #region SQL Parameters
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = rcrdrtrn.callid;
                #endregion SQL Parameters
                print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            if (Int32.TryParse(sqlRdr["count"].ToString(), out rcrdrtrn.count)
                                && DateTime.TryParse(sqlRdr["lastupdate"].ToString(), out rcrdrtrn.lastupdate))
                            {
                                lbl.Text = sqlRdr["count"].ToString();
                                lbl.Text += "s";
                                rcrdrtrn.source = "DE-SQL";
                                rcrdrtrn.lastupdate = DateTime.UtcNow;
                                rcrdrtrn.fetched = true;
                            }
                        }
                    }
                }
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
        #endregion Find Record in SQL Cache
        return rcrdrtrn;
    }
    protected ghStaticClass.recording_cache RecordingCount_From_MiddleWare_SQL(SqlConnection con, ghStaticClass.recording_cache rcrdrtrn, Label lbl)
    {
        String recordings = "-1";
        #region SQL Command
        using (SqlCommand cmd = new SqlCommand("", con))
        {
            if (con.State == ConnectionState.Closed) { con.Open(); }
            cmd.CommandTimeout = 600;
            #region Build cmdText
            String cmdText = "";
            cmdText += @"
                        IF EXISTS(SELECT TOP 1 1 FROM [dbo].[CallRecording] [cr] WITH(NOLOCK) WHERE [cr].[CallId] = @sp_callid)
                        BEGIN
	                        SELECT
	                        COUNT([cr].[CallRecordingId]) [count]
	                        FROM [dbo].[CallRecording] [cr] WITH(NOLOCK)
	                        WHERE [cr].[CallId] = @sp_callid
                        END
                        ELSE
                        BEGIN
	                        SELECT 0 [count]
                        END
                            ";
            #endregion Build cmdText
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            #region SQL Parameters
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = rcrdrtrn.callid;
            #endregion SQL Parameters
            print_sql(cmd, "append"); // Will print for Admin in Local
            #region SQL Processing
            var sqlScalar = cmd.ExecuteScalar();
            if (sqlScalar != null)
            {
                recordings = sqlScalar.ToString();
                if (Int32.TryParse(recordings, out rcrdrtrn.count))
                {
                    rcrdrtrn.source = "MIDDLE";
                    rcrdrtrn.lastupdate = DateTime.UtcNow;
                    rcrdrtrn.fetched = true;

                }
            }
            #endregion SQL Processing
        }
        #endregion SQL Command
        return rcrdrtrn;
    }
    protected bool RecordingCount_AddTo_Cache_SQL(ghStaticClass.recording_cache rcrdrinsrt)
    {
        bool tosql = false;
        #region Find Record in SQL Cache
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region Build cmdText
                String cmdText = "";
                cmdText += @"
                                IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[five9_recording_count_cache] [cr] WITH(NOLOCK) WHERE [cr].[CallId] = @sp_callid)
                                BEGIN
	                                INSERT INTO [dbo].[five9_recording_count_cache] ([callid], [count], [lastupdate])
	                                SELECT
		                                @sp_callid
		                                ,@sp_count
		                                ,@sp_lastupdate
                                END
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                #region SQL Parameters
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = rcrdrinsrt.callid;
                cmd.Parameters.Add("@sp_count", SqlDbType.Int).Value = rcrdrinsrt.count;
                cmd.Parameters.Add("@sp_lastupdate", SqlDbType.DateTime).Value = rcrdrinsrt.lastupdate;
                #endregion SQL Parameters
                print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Processing
                int sqlNonQuery = cmd.ExecuteNonQuery();
                if (sqlNonQuery > 0)
                {
                    tosql = true;
                }
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
        #endregion Find Record in SQL Cache
        return tosql;
    }
    protected void Recording_Fetch_Record(GridView gv)
    {
        Int32 sp_callid = -1;
        #region Process Rows
        GridViewRow dr = gvSearchGrid.SelectedRow;
        #region Get the Call ID
        for (int i = 0; i < dr.Cells.Count; i++)
        {
            if (gvSearchGrid.HeaderRow.Cells[i].Text == "call.call_id")
            {
                if (!Int32.TryParse(dr.Cells[i].Text, out sp_callid)) sp_callid = -1;
                break;
            }
        }
        #endregion Get the Call ID

        #endregion Process Rows
        if (sp_callid > 0)
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrR))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
                        IF EXISTS(SELECT TOP 1 1 FROM [dbo].[CallRecording] [cr] WITH(NOLOCK) WHERE [cr].[CallId] = @sp_callid)
                        BEGIN
                            SELECT
                            TOP 10
                            [cr].[CallRecordingId] [recordingid]
                            ,[cr].[DateCreated]
                            ,[cr].[FileName]
                            ,[cr].[SessionId]
                            ,[cr].[CallId]
                            ,[cr].[RecordingURL]
                            ,[cr].[LocalPath]
                            ,[cr].[Five9AuthorityCallDataId]
                            FROM [dbo].[CallRecording] [cr] WITH(NOLOCK)
                            WHERE [cr].[CallId] = @sp_callid
                        END
                            ";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    #region SQL Parameters
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = sp_callid;
                    #endregion SQL Parameters
                    print_sql(cmd, "append"); // Will print for Admin in Local
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt;
                    gv.DataBind();
                    if (dt.Rows.Count > 0)
                    {
                        lblRecordingsGrid.Text = "Call Recordings";
                    }
                    else
                    {
                        gv.DataSource = null;
                        gv.DataBind();
                        lblRecordingsGrid.Text = "";
                    }
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
    }
    protected void Search_Fetch(GridView gv)
    {
        DateTime sp_datestart = DateTime.UtcNow.AddDays(-1); // = DateTime.Parse(DateTime.UtcNow.AddDays(-1).ToString("MM/dd/yyyy 00:00:00"));
        DateTime sp_dateend = DateTime.UtcNow.AddDays(+1); // = DateTime.Parse(DateTime.UtcNow.ToString("MM/dd/yyyy 23:59:59"));
        if (!DateTime.TryParse(dtStartDate.Text + " " + dtStartTime.Text, out sp_datestart))
        {
            sp_datestart = DateTime.UtcNow.AddDays(-1);
        }
        if (!DateTime.TryParse(dtEndDate.Text + " " + dtEndTime.Text, out sp_dateend))
        {
            sp_dateend = DateTime.UtcNow.AddDays(+1);
        }
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region Build cmdText
                String cmdText = "";
                cmdText += @"
                        SELECT
                        TOP (@sp_top)
                        '--------------' [interactions]
                        ,[i].[companyid]
                        ,[i].[interactionid]
                        ,[i].[interactiontype]
                        ,[i].[createdate]
                        ,'--------------' [five9_calls]
                        ,[fc].[call.call_id]
                        ,[fc].[call.campaign_name]
                        ,[fc].[call.skill_name]
                        ,[fc].[call.length]
                        ,'--------------' [five9_calls_disposition]
                        ,[fcd].[call.call_id] [fcd.call_id]
                        ,[fcd].[agent.full_name]
                        ,[fcd].[call.disposition_name]
                        ,'--------------' [interactions_arc]
                        ,[ia].[arc.callid] [ia.callid]
                        ,[ia].[arc.disposition_name]
                        FROM [dbo].[interactions] [i] WITH(NOLOCK)
                        LEFT OUTER JOIN [dbo].[five9_calls] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
                        LEFT OUTER JOIN [dbo].[five9_calls_disposition] [fcd] WITH(NOLOCK) ON [fcd].[companyid] = [i].[companyid] AND [fcd].[interactionid] = [i].[interactionid]
                        LEFT OUTER JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
                        WHERE 1=1
                            ";

                if (!IsPostBack && Request["interaction"] != null)
                {

                    cmdText += "AND [i].[interactionid] = @sp_interactionid\r";
                }
                else
                {
                    cmdText += "AND [i].[createdate] BETWEEN @sp_datestart AND @sp_dateend\r";
                }
                cmdText += "ORDER BY [i].[interactionid] DESC\r";
                cmdText += "\r";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                #region SQL Parameters
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = ddlTop.SelectedValue;
                //cmd.Parameters.Add(new SqlParameter("@sp_top", SqlDbType.Int, 1).Value = 25);
                // categoriesAdapter.SelectCommand.Parameters.Add("@CategoryName", SqlDbType.VarChar, 80).Value = "toasters";

                //var p = new SqlParameter("@sp_datestart", SqlDbType.DateTime);
                //p.Value = sp_datestart;
                //cmd.Parameters.Add(p);
                //p = new SqlParameter("@sp_dateend", SqlDbType.DateTime);
                //p.Value = sp_dateend;
                //cmd.Parameters.Add(p);

                //cmd.Parameters.Add(new SqlParameter("@sp_datestart", SqlDbType.DateTime, 1).Value = sp_datestart);
                //cmd.Parameters.Add(new SqlParameter("@sp_dateend", SqlDbType.DateTime, 1).Value = sp_dateend);
                if (!IsPostBack && Request["interaction"] != null)
                {
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.VarChar, 32).Value = Request["interaction"].ToString();
                }
                else
                {
                    cmd.Parameters.Add("@sp_datestart", SqlDbType.DateTime).Value = sp_datestart;
                    cmd.Parameters.Add("@sp_dateend", SqlDbType.DateTime).Value = sp_dateend;
                }
                #endregion SQL Parameters
                print_sql(cmd, "append"); // Will print for Admin in Local
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt;
                gv.DataBind();
                if (dt.Rows.Count > 0)
                {
                    btnExport.Visible = true;
                    gvSearchExport.DataSource = dt;
                    gvSearchExport.DataBind();
                }
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Load_Checklist()
    {
        String strMessage = "";
        String strToCheck = "";
        Int32 errCode = 0;
        String errLast = "";
        Boolean oDebug = false;
        if (Connection.GetDBMode() == "Stage") oDebug = true;
        lblSystemMessage.Text = "Checking resources...";
        #region Cybersource
        strMessage += ("<br />" + Repeat('\t', 1) + "Checking Cybersource");
        strMessage += ("<br />" + Repeat('\t', 2) + "DB Mode: " + Connection.GetDBMode());
        #region Cybersource Keys
        strToCheck = ConfigurationManager.AppSettings["cybs.keysDirectory"];
        if (System.IO.Directory.Exists(strToCheck)) // checking if keys folder exists
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Key Path OK");
            string strpath = ConfigurationManager.AppSettings["cybs.keysDirectory"];
            string strfile = ConfigurationManager.AppSettings["cybs.merchantID"];
            if (System.IO.File.Exists(strpath + @"\" + strfile + ".p12"))
            {
                strMessage += ("<br />" + Repeat('\t', 2) + "Key File OK");
            }
            else
            {
                strMessage += ("<br />" + Repeat('\t', 2) + "Key File FAILED");
                errCode++;
                errLast = "Cyb Key File Failed";
            }
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Key Path FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Path: " + strToCheck);
            strMessage += ("<br />" + Repeat('\t', 3) + "PathS: " + Server.MapPath("~"));
            errCode++;
            errLast = "Cyb Key Path Failed";
        }
        #endregion Cybersource Keys
        #region Cybersource Logs
        strToCheck = ConfigurationManager.AppSettings["cybs.logDirectory"];
        if (System.IO.Directory.Exists(strToCheck)) // checking if keys folder exists
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Log Path OK");
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Log Path FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Path: " + strToCheck);
            errCode++;
            errLast = "Cyb Log Path Failed";
        }
        #endregion Cybersource Logs
        #region Cybersource Account
        // If DeBug we should be using gnh160325
        // If Live we should be using lms150201
        String cbMerchantID = "";
        String cbProduction = "";
        String cbLog = "";
        // If we are on LIVE DB we expect lms150201
        // If we are on STAGE DB we expect gnh160325
        // If we are on portal. / portalnew. we expect LIVE DB
        // If we are on portal. / portalnew. we expect STAGE DB
        if (Connection.GetDBMode() == "Live")
        {
            cbMerchantID = "lms150201";
            cbProduction = "true";
            cbLog = "false";
        }
        if (Connection.GetDBMode() == "Stage")
        {
            cbMerchantID = "gnh160325";
            cbProduction = "false";
            cbLog = "true";
        }
        #region Merch ID
        strToCheck = ConfigurationManager.AppSettings["cybs.merchantID"];
        if (strToCheck == cbMerchantID)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "MerchantID OK");
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "MerchantID FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Current: " + strToCheck + " | Expected: " + cbMerchantID);
            errCode++;
            errLast = "Cyb Merch ID Failed";
        }
        #endregion Merch ID
        #region Production
        strToCheck = ConfigurationManager.AppSettings["cybs.sendToProduction"];
        if (strToCheck == cbProduction)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Production OK");
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Production FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Current: " + strToCheck + " | Expected: " + cbProduction);
            errCode++;
            errLast = "Cyb Production Failed";
        }
        #endregion Production
        #region Log
        strToCheck = ConfigurationManager.AppSettings["cybs.enableLog"];
        if (strToCheck == cbLog)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Log OK");
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Log FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Current: " + strToCheck + " | Expected: " + cbLog);
            errCode++;
            errLast = "Cyb Log Failed";
        }
        #endregion Production

        #endregion Cybersource Account
        #endregion Cybersource
        #region Database
        strMessage += ("<br />" + Repeat('\t', 1) + "Checking Database");
        // We need to check both ARC and PORTAL databases
        #region Portal DB
        try
        {
            using (SqlConnection con = new SqlConnection(sqlCon))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                if (con.State == ConnectionState.Open)
                {
                    strMessage += ("<br />" + Repeat('\t', 2) + "Portal Database OK");
                    con.Close();
                }
                else
                {
                    strMessage += ("<br />" + Repeat('\t', 2) + "Portal Database FAILED");
                    errCode++;
                    errLast = "Portal Database Failed";
                }
            }
        }
        catch (Exception ex)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Portal Database ERROR");
            errCode++;
            errLast = "Portal Database Error\n" + ex.Message;
        }

        #endregion Portal DB
        #region ARC DB
        sqlCon = "ARC_Live"; if (oDebug) { sqlCon = "ARC_Stage"; }
        try
        {
            using (SqlConnection con = new SqlConnection(Connection.GetConnectionString(sqlCon, "")))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                if (con.State == ConnectionState.Open)
                {
                    strMessage += ("<br />" + Repeat('\t', 2) + "ARC Database OK");
                    con.Close();
                }
                else
                {
                    strMessage += ("<br />" + Repeat('\t', 2) + "ARC Database FAILED");
                    errCode++;
                    errLast = "ARC Database Failed";
                }
            }
        }
        catch (Exception ex)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "ARC Database ERROR");
            errCode++;
            errLast = "ARC Database Error\n" + ex.Message;
        }

        #endregion ARC DB

        #endregion Database
        lblSystemMessage.Text += "<br />" + strMessage.Replace("		", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
        /*
        "Key&nbsp;Path&nbsp;FAILED"
        */

    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        try
        {
            Error_General.Text = "";
            gvSearchGrid.SelectedIndex = -1;
            Search_Grid_Refresh();
            DetailsView_Clear();
        }
        catch (Exception ex)
        {
            Error_Save(ex, "GridView_Refresh");
        }
    }
    protected void Custom_Export_Excel_SearchGrid(object sender, EventArgs e)
    {
        /// This will be a fully customized export using ClosedXML
        /// We need to add each cell individually
        /// So this will allow us complete control
        /// Use file: F:\ciambotti\greenwoodhall\MiddleWare\sql\dashboard\Dashboard-Export.xlsx
        /// http://stackoverflow.com/questions/12267421/closedxml-working-with-percents-1-decimal-place-and-rounding
        /// https://closedxml.codeplex.com/wikipage?title=Merging%20Cells&referringTitle=Documentation
        /// https://techatplay.wordpress.com/2013/11/05/closedxml-an-easier-way-of-using-openxml/
        /// https://programmershandbook.wordpress.com/2015/03/20/create-closedxml-excel/
        /// ws.Cell("A4").SetValue("25").SetDataType(XLCellValues.Number);
        String fileName = "Search-Grid";
        XLWorkbook wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(fileName);
        // Starting Column and Row for Dashboard
        int sRow = 1; int sCol = 1; // A1
        #region Insert - Logo
        ws.Range(sRow, sCol, sRow + 3, sCol + 3).Merge();
        using (WebClient wc = new WebClient())
        {
            byte[] bytes = wc.DownloadData(Server.MapPath("/images/ghnew.png"));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

                XLPicture pic = new XLPicture
                {
                    NoChangeAspect = false,
                    NoMove = false,
                    NoResize = false,
                    ImageStream = fIn,
                    Name = "Logo"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 4, RowId = sRow + 3 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        sRow = sRow + 4;
        ws.Cell(sRow, sCol).Active = true;
        #endregion Insert - Logo
        var cl = ws.Cell(sRow, sCol);
        var cr = ws.Range(sRow, sCol, sRow, sCol + 1);
        #region Date Range
        cr.Value = "Start Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 2, sRow, sCol + 2 + 1);
        cr.Value = dtStartDate.Text + " " + dtStartTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        sRow = sRow + 1;
        cr = ws.Range(sRow, sCol, sRow, sCol + 1);
        cr.Value = "End Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 2, sRow, sCol + 2 + 1);
        cr.Value = dtEndDate.Text + " " + dtEndTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        #endregion Date Range
        sRow = sRow + 2;
        #region Grid - Call Dispositions
        cl = ws.Cell(sRow, sCol);
        cl.Value = "Record Details";
        cl.Style.Font.Bold = true;
        cl.Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 5).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
        GridView gv = gvSearchExport; // gvSearchGrid

        foreach (TableCell cell in gv.HeaderRow.Cells)
        {
            ws.Cell(dRow, dColT).Value = cell.Text;
            ws.Cell(dRow, dColT).Style.Font.Bold = true;
            ws.Cell(dRow, dColT).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(dRow, dColT).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(dRow, dColT).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(dRow, dColT).Style.Border.OutsideBorderColor = XLColor.DarkGray;
            dColT++;
        }
        dRow++;
        #region Process each Disposition Row
        foreach (GridViewRow gvRow in gv.Rows)
        {
            dColT = dCol;

            for (int i = 0; i < gvRow.Cells.Count; i++)
            {
                cl = ws.Cell(dRow, dColT);
                if (gvRow.Cells[i].HasControls())
                {
                    //string cntrls = "";
                    //foreach (Control c in gvRow.Cells[i].Controls)
                    //{
                    //    if (c.GetType() == typeof(Label))
                    //    {
                    //        cntrls = ((Label)c).Text;
                    //    }
                    //}
                    ////var num = decimal.Parse(cntrls.TrimEnd(new char[] { '%', ' ' })) / 100M;
                    //var num = cntrls;
                    //cl.Value = num;
                    ////cl.Style.NumberFormat.Format = "0%";
                    //cl.Style.NumberFormat.Format = "@";
                }
                else
                {
                    if (gvRow.Cells[i].Text != "&nbsp;")
                    {
                        //
                        if (gv.HeaderRow.Cells[i].Text == "Amount")
                        {
                            cl.Value = gvRow.Cells[i].Text;
                            cl.Style.NumberFormat.Format = "$#,##0.00";
                        }
                        else if (gv.HeaderRow.Cells[i].Text == "CreateDate")
                        {
                            cl.Value = gvRow.Cells[i].Text;
                            cl.Style.NumberFormat.Format = "MM/dd/yyyy hh:mm";
                        }
                        else
                        {
                            cl.Value = gvRow.Cells[i].Text;
                        }
                        cl.Value = gvRow.Cells[i].Text;
                    }
                }

                cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;

                dColT++;
            }
            dRow++;
        }
        #endregion Process each Disposition Row
        #endregion Grid - Call Dispositions
        #region Wrap Up - Save/Download the File
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        ws.Rows().AdjustToContents();
        ws.Columns().AdjustToContents();
        // We want 40 width for the logo
        ws.Column(1).Width = 10;
        ws.Column(2).Width = 7.25;
        ws.Column(3).Width = 4.25;
        ws.Column(4).Width = 18.5;

        ws.ShowGridLines = false;
        fileName = "ARC-Portal-" + fileName;

        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}{1}.xlsx", fileName.Replace(" ", "_"), DateTime.Now.ToString("-yyyyMMdd-HHmmss")));

        using (MemoryStream memoryStream = new MemoryStream())
        {
            wb.SaveAs(memoryStream);
            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
            memoryStream.Close();
        }

        HttpContext.Current.Response.End();
        #endregion Wrap Up - Save/Download the File
    }
    protected void Custom_Export_Excel_Details(object sender, EventArgs e)
    {
        ///// This will be a fully customized export using ClosedXML
        ///// We need to add each cell individually
        ///// So this will allow us complete control
        ///// Use file: F:\ciambotti\greenwoodhall\MiddleWare\sql\dashboard\Dashboard-Export.xlsx
        ///// http://stackoverflow.com/questions/12267421/closedxml-working-with-percents-1-decimal-place-and-rounding
        ///// https://closedxml.codeplex.com/wikipage?title=Merging%20Cells&referringTitle=Documentation
        ///// https://techatplay.wordpress.com/2013/11/05/closedxml-an-easier-way-of-using-openxml/
        ///// https://programmershandbook.wordpress.com/2015/03/20/create-closedxml-excel/
        ///// ws.Cell("A4").SetValue("25").SetDataType(XLCellValues.Number);
        //String fileName = "Record-Details-";
        //fileName += gvSearchGrid.SelectedDataKey["callid"].ToString();
        //XLWorkbook wb = new XLWorkbook();
        //var ws = wb.Worksheets.Add(fileName);
        //// Starting Column and Row for Dashboard
        //int sRow = 1; int sCol = 1; // A1
        //#region Insert - Logo
        //ws.Range(sRow, sCol, sRow + 3, sCol + 3).Merge();
        //using (WebClient wc = new WebClient())
        //{
        //    byte[] bytes = wc.DownloadData(Server.MapPath("/images/ghnew.png"));
        //    using (MemoryStream ms = new MemoryStream(bytes))
        //    {
        //        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

        //        var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

        //        XLPicture pic = new XLPicture
        //        {
        //            NoChangeAspect = false,
        //            NoMove = false,
        //            NoResize = false,
        //            ImageStream = fIn,
        //            Name = "Logo"
        //        };
        //        XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow };
        //        pic.AddMarker(fMark);
        //        fMark = new XLMarker { ColumnId = sCol + 4, RowId = sRow + 3 };
        //        pic.AddMarker(fMark);
        //        ws.AddPicture(pic);

        //        img.Dispose();
        //        fIn.Dispose();
        //    }
        //}
        //sRow = sRow + 4;
        //ws.Cell(sRow, sCol).Active = true;
        //#endregion Insert - Logo
        //var cl = ws.Cell(sRow, sCol);
        //var cr = ws.Range(sRow, sCol, sRow, sCol + 1);
        //#region Date Range
        //cr.Value = "Start Date";
        //cr.Merge();
        //cr.Style.Font.Bold = true;
        //cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        //cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        //cr = ws.Range(sRow, sCol + 2, sRow, sCol + 2 + 1);
        //cr.Value = dtStartDate.Text + " " + dtStartTime.Text;
        //cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        //cr.Merge();
        //cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        //cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        //sRow = sRow + 1;
        //cr = ws.Range(sRow, sCol, sRow, sCol + 1);
        //cr.Value = "End Date";
        //cr.Merge();
        //cr.Style.Font.Bold = true;
        //cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        //cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        //cr = ws.Range(sRow, sCol + 2, sRow, sCol + 2 + 1);
        //cr.Value = dtEndDate.Text + " " + dtEndTime.Text;
        //cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        //cr.Merge();
        //cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        //cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        //#endregion Date Range
        //sRow = sRow + 2;
        //cl = ws.Cell(sRow, sCol);
        //cl.Value = "Record Details";
        //cl.Style.Font.Bold = true;
        //cl.Style.Font.FontSize = 12;
        //ws.Range(sRow, sCol, sRow, sCol + 5).Merge();
        //ws.Range(sRow, sCol, sRow, sCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        //#region Load the Views
        //// Details View - Left Side
        //int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
        //Excel_Export_DetailsView(dvCallDetails, ws, cr, dRow, dCol, dColT);
        //// Details View
        //dRow = tempRow + 1;
        //Excel_Export_DetailsView(dvPaymentDetails, ws, cr, dRow, dCol, dColT);
        //// Details View
        //dRow = tempRow + 1;
        //Excel_Export_DetailsView(dvContactDetails, ws, cr, dRow, dCol, dColT);
        //// Details View
        //dRow = tempRow + 1;
        //Excel_Export_DetailsView(dvRefundDetails, ws, cr, dRow, dCol, dColT);
        //// Details View
        //dRow = tempRow + 1;
        //Excel_Export_DetailsView(dvGiftDetails, ws, cr, dRow, dCol, dColT);
        //// Details View
        //dRow = tempRow + 1;
        //Excel_Export_DetailsView(dvSustainerDetails, ws, cr, dRow, dCol, dColT);

        //// Details View - Right Side
        //dRow = sRow + 2; dCol = sCol + 6; dColT = dCol;
        //Excel_Export_DetailsView(dvDonorDetails, ws, cr, dRow, dCol, dColT);
        //// Details View
        //dRow = tempRow + 1;
        //Excel_Export_DetailsView(dvTributeDetails, ws, cr, dRow, dCol, dColT);
        //// Details View
        //dRow = tempRow + 1;
        //Excel_Export_DetailsView(dvADUFile, ws, cr, dRow, dCol, dColT);
        //// Details View
        //dRow = tempRow + 1;
        //Excel_Export_DetailsView(dvRemoveDetails, ws, cr, dRow, dCol, dColT);

        //// Gift List
        //// Recurring List
        //dRow = sRow + 0; dCol = sCol + 12; dColT = dCol;
        //Excel_Export_GridView(gvRecurringListExport, "Recurring List", ws, cl, cr, dRow, dCol, dColT);
        //#endregion Load the Views

        //#region Wrap Up - Save/Download the File
        //if (dRow < sRow + 23) dRow = sRow + 23;
        //ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        //sRow = dRow;

        //ws.Rows().AdjustToContents();
        //ws.Columns().AdjustToContents();
        //// We want 40 width for the logo
        //ws.Column(1).Width = 10;
        //ws.Column(2).Width = 7.25;
        //ws.Column(3).Width = 4.25;
        //ws.Column(4).Width = 18.5;

        //ws.Column(6).Width = 10;
        //ws.Column(7).Width = 7.25;
        //ws.Column(8).Width = 4.25;
        //ws.Column(9).Width = 18.5;

        //ws.ShowGridLines = false;
        //fileName = "ARC-Portal-" + fileName;

        //HttpContext.Current.Response.Clear();
        //HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}{1}.xlsx", fileName.Replace(" ", "_"), DateTime.Now.ToString("-yyyyMMdd-HHmmss")));

        //using (MemoryStream memoryStream = new MemoryStream())
        //{
        //    wb.SaveAs(memoryStream);
        //    memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
        //    memoryStream.Close();
        //}

        //HttpContext.Current.Response.End();
        //#endregion Wrap Up - Save/Download the File
    }
    protected void Excel_Export_DetailsView(DetailsView dv, IXLWorksheet ws, IXLRange cr, int dRow, int dCol, int dColT)
    {
        #region Details View
        //if (dv.Rows[0].Cells[0].Text != "&nbsp;")
        if (dv.Rows.Count > 0)
        {
            // Each DetailsView has a Header, 2 Columns, Multiple Rows
            // There can be variable information in any place
            //DetailsView dv = dvCallDetails;
            cr = ws.Range(dRow, dColT, dRow, dColT + 4);
            cr.Value = dv.HeaderText;
            cr.Merge();
            cr.Style.Font.Bold = true;
            cr.Style.Fill.BackgroundColor = XLColor.LightGray;
            cr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
            dRow++;
            #region DV Rows
            // We need to ensure the 1st row is formatted even if no record details
            foreach (TableRow dvRow in dv.Rows)
            {
                dColT = dCol;
                #region DV Cells
                for (int i = 0; i < dvRow.Cells.Count; i++)
                {
                    cr = ws.Range(dRow, dColT, dRow, dColT + 1);
                    if (i == 1) cr = ws.Range(dRow, dColT, dRow, dColT + 2);
                    if (dvRow.Cells[i].HasControls())
                    {
                        string cntrls = "";
                        foreach (Control c in dvRow.Cells[i].Controls)
                        {
                            if (c.GetType() == typeof(Label))
                            {
                                cntrls = ((Label)c).Text;
                            }
                        }
                        var num = cntrls;
                        cr.Value = num;
                    }
                    else
                    {
                        if (dvRow.Cells[i].Text.Contains("No [") && dvRow.Cells[i].Text.Contains("] details;"))
                        {
                            cr = ws.Range(dRow, dColT, dRow, dColT + 4);
                        }
                        if (dvRow.Cells[i].Text != "&nbsp;")
                        {
                            cr.Value = dvRow.Cells[i].Text;
                        }
                    }
                    cr.Merge();
                    cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                    cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
                    cr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    //cr.Style.Alignment.WrapText = true; // Does not work due to merged cells
                    dColT++;
                    dColT++;
                }
                #endregion DV Cells
                dRow++;
            }
            #endregion DV Rows
            tempRow = dRow;
        }
        #endregion Details View
    }
    protected void Excel_Export_GridView(GridView gv, String Title, IXLWorksheet ws, IXLCell cl, IXLRange cr, int dRow, int dCol, int dColT)
    {
        if (gv.Rows.Count > 0)
        {
            #region Grid View
            cl = ws.Cell(dRow, dCol);
            cl.Value = Title;
            cl.Style.Font.Bold = true;
            cl.Style.Font.FontSize = 12;
            ws.Range(dRow, dCol, dRow, dCol + 5).Merge();
            ws.Range(dRow, dCol, dRow, dCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            dRow++;
            dRow++;

            foreach (TableCell cell in gv.HeaderRow.Cells)
            {
                if (cell.Text != "&nbsp;")
                {
                    ws.Cell(dRow, dColT).Value = cell.Text;
                    ws.Cell(dRow, dColT).Style.Font.Bold = true;
                    ws.Cell(dRow, dColT).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(dRow, dColT).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(dRow, dColT).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(dRow, dColT).Style.Border.OutsideBorderColor = XLColor.DarkGray;
                    dColT++;
                }
            }
            dRow++;
            #region Process each Row
            foreach (GridViewRow gvRow in gv.Rows)
            {
                dColT = dCol;

                for (int i = 0; i < gvRow.Cells.Count; i++)
                {
                    cl = ws.Cell(dRow, dColT);
                    if (gvRow.Cells[i].HasControls())
                    {
                        string cntrls = "";
                        foreach (Control c in gvRow.Cells[i].Controls)
                        {
                            if (c.GetType() == typeof(Label))
                            {
                                cntrls = ((Label)c).Text;
                            }
                        }
                        if (cntrls != "&nbsp;")
                        {
                            //var num = decimal.Parse(cntrls.TrimEnd(new char[] { '%', ' ' })) / 100M;
                            var num = cntrls;
                            cl.Value = num;
                            //cl.Style.NumberFormat.Format = "0%";
                            //cl.Style.NumberFormat.Format = "@";
                            //cl.Value = gvRow.Cells[i].Text;
                        }
                    }
                    else
                    {
                        if (gvRow.Cells[i].Text != "&nbsp;")
                        {
                            if (gv.HeaderRow.Cells[i].Text == "Amount")
                            {
                                cl.Value = gvRow.Cells[i].Text;
                                cl.Style.NumberFormat.Format = "$#,##0.00";
                            }
                            else if (gv.HeaderRow.Cells[i].Text == "CreateDate")
                            {
                                cl.Value = gvRow.Cells[i].Text;
                                cl.Style.NumberFormat.Format = "MM/dd/yyyy hh:mm";
                            }
                            else
                            {
                                cl.Value = gvRow.Cells[i].Text;
                            }
                        }
                    }

                    cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                    cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;

                    dColT++;
                }
                dRow++;
            }
            #endregion Process each Row
            #endregion Grid View
            tempRow = dRow;
        }
    }
    protected void GridView_DataBound(Object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.ID == "gvSearchGrid")
        {
            #region gvSearchGrid
            lblSearchGrid.Text = " Records: [" + gvSearchGrid.Rows.Count.ToString() + "]";
            if (gvSearchGrid.PageCount > 0)
            {
                lblSearchGrid.Text += " - Pages: [" + gvSearchGrid.PageCount.ToString() + "]";
                lblSearchGrid.Text += " - Approx Total: [" + (gvSearchGrid.PageCount * gvSearchGrid.Rows.Count).ToString() + "]";
                // Retrieve the pager row.
                //GridViewRow pagerRow = gvSearchGrid.BottomPagerRow;
                GridViewRow pagerRow = gvSearchGrid.TopPagerRow;
                // Retrieve the DropDownList and Label controls from the row.
                DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("gvSearchGridPageDropDownList");
                Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
                if (pageList != null)
                {
                    // Create the values for the DropDownList control based on 
                    // the  total number of pages required to display the data
                    // source.
                    for (int i = 0; i < gvSearchGrid.PageCount; i++)
                    {
                        // Create a ListItem object to represent a page.
                        int pageNumber = i + 1;
                        ListItem item = new ListItem(pageNumber.ToString());
                        // If the ListItem object matches the currently selected
                        // page, flag the ListItem object as being selected. Because
                        // the DropDownList control is recreated each time the pager
                        // row gets created, this will persist the selected item in
                        // the DropDownList control.   
                        if (i == gvSearchGrid.PageIndex)
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
                    int currentPage = gvSearchGrid.PageIndex + 1;
                    // Update the Label control with the current page information.
                    pageLabel.Text = "Page " + currentPage.ToString() +
                      " of " + gvSearchGrid.PageCount.ToString();
                }
                if (gvSearchGrid.PageIndex > 0)
                {
                    pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = true;
                    pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = true;
                }
                else
                {
                    pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = false;
                    pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = false;
                }

                if (gvSearchGrid.PageCount != (gvSearchGrid.PageIndex + 1))
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
            #endregion gvSearchGrid
        }
    }
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //http://aspdotnetfaq.com/Faq/How-to-correctly-highlight-GridView-rows-on-Mouse-Hover-in-ASP-NET.aspx
            // when mouse is over the row, save original color to new attribute, and change it to highlight yellow color
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#336699'");

            // when mouse leaves the row, change the bg color to its original value    
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");

            e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv, "Select$" + e.Row.RowIndex);
        }
    }
    protected void GridView_IndexChanged(object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.ID == "gvSearchGrid")
        {
            #region gvSearchGrid
            try
            {
                // Admin_Clear();
                DetailsView_Data(dvInteraction);
                DetailsView_Data(dvInteractionARC);
                DetailsView_Data(dvFive9Calls);
                DetailsView_Data(dvFive9CallsDisposition);
                Recording_Fetch_Record(gvRecordingsGrid);
                btnExportDetails.Visible = true;
                if (ghUser.identity_is_admin()) pAdminFunctions.Visible = true;
            }
            catch (Exception ex)
            {
                Error_Save(ex, "DetailsView Data Error");
            }
            #endregion gvSearchGrid
        }
    }
    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.ID == "gvSearchGrid")
        {
            #region gvSearchGrid
            // Admin_Clear();
            //Session["EventList_GridView_SelectedIndex"] = null;
            //Session["EventList_GridView_PageIndex"] = null;
            lblSearchGrid.Text = e.NewPageIndex.ToString();
            gvSearchGrid.SelectedIndex = -1;
            gvSearchGrid.PageIndex = e.NewPageIndex;
            Search_Grid_Refresh();
            #endregion gvSearchGrid
        }
    }
    protected void GridView_PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
    {
        DropDownList gv = (DropDownList)sender;
        if (gv.ID == "gvSearchGridPageDropDownList")
        {
            #region gvSearchGrid
            // Retrieve the pager row.
            GridViewRow pagerRow = gvSearchGrid.TopPagerRow;
            // Retrieve the PageDropDownList DropDownList from the bottom pager row.
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("gvSearchGridPageDropDownList");
            // Set the PageIndex property to display that page selected by the user.
            gvSearchGrid.SelectedIndex = -1;
            gvSearchGrid.PageIndex = pageList.SelectedIndex;
            Search_Grid_Refresh();
            #endregion gvSearchGrid
        }

    }
    protected void DetailsView_Clear()
    {
        if (dvInteraction.Rows.Count > 0) dvInteraction.DataBind();
        if (dvInteractionARC.Rows.Count > 0) dvInteractionARC.DataBind();
        if (dvFive9Calls.Rows.Count > 0) dvFive9Calls.DataBind();
        if (dvFive9CallsDisposition.Rows.Count > 0) dvFive9CallsDisposition.DataBind();
        if (gvRecordingsGrid.Rows.Count > 0) { gvRecordingsGrid.DataBind(); lblRecordingsGrid.Text = ""; }
        btnExportDetails.Visible = false;
        // Admin_Clear();
    }
    #region Details View Handling
    protected void DetailsView_Data(DetailsView dv)
    {
        //Int32 companyid = Convert.ToInt32(gvSearchGrid.SelectedDataKey.Values[0].ToString());
        //Int32 interactionid = Convert.ToInt32(gvSearchGrid.SelectedDataKey.Values[0].ToString());

        Int32 sp_companyid = 0;
        Int32 sp_interactionid = 0;
        try
        {
            if (gvSearchGrid.SelectedIndex != -1)
            {
                Int32.TryParse(gvSearchGrid.SelectedDataKey["companyid"].ToString(), out sp_companyid);
                Int32.TryParse(gvSearchGrid.SelectedDataKey["interactionid"].ToString(), out sp_interactionid);
            }
            if (sp_companyid > 0 && sp_companyid > 0)
            {
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(sqlStrDE))
                {
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        cmd.CommandTimeout = 600;
                        #region Build cmdText
                        String cmdText = "";
                        #region dvInteraction
                        if (dv.ID == "dvInteraction")
                        {
                            cmdText = @"
                                        SELECT
                                        TOP 1
                                        '--------------' [interactions]
                                        ,[i].[companyid]
                                        ,[i].[interactionid]
                                        ,[i].[interactiontype]
                                        ,[i].[createdate]
                                        ,[i].[resourcetype]
                                        ,[i].[resourceid]
                                        ,[i].[originator]
                                        ,[i].[destinator]
                                        ,[i].[duration]
                                        ,[i].[status]
                                        FROM [dbo].[interactions] [i] WITH(NOLOCK)
                                        WHERE 1=1
                                        AND [i].[companyid] = @sp_companyid
                                        AND [i].[interactionid] = @sp_interactionid
                                        ";
                        }
                        #endregion dvInteraction
                        #region dvInteractionARC
                        if (dv.ID == "dvInteractionARC")
                        {
                            cmdText = @"
                                        SELECT
                                        TOP 1
                                        '--------------' [interactions_arc]
                                        ,[ia].[companyid]
                                        ,[ia].[interactionid]
                                        ,[ia].[arc.callid]
                                        ,[ia].[createdate]
                                        ,[ia].[arc.disposition_id]
                                        ,[ia].[arc.disposition_name]
                                        FROM [dbo].[interactions_arc] [ia] WITH(NOLOCK)
                                        WHERE 1=1
                                        AND [ia].[companyid] = @sp_companyid
                                        AND [ia].[interactionid] = @sp_interactionid
                                        ";
                        }
                        #endregion dvInteractionARC
                        #region dvFive9Calls
                        if (dv.ID == "dvFive9Calls")
                        {
                            cmdText = @"
                                        SELECT
                                        TOP 1
                                        '--------------' [five9_calls]
                                        ,[fc].[companyid]
                                        ,[fc].[interactionid]
                                        ,[fc].[call.call_id]
                                        ,[fc].[call.campaign_id]
                                        ,[fc].[createdate]
                                        ,[fc].[call.skill_id]
                                        ,[fc].[call.type]
                                        ,[fc].[call.dnis]
                                        ,[fc].[call.ani]
                                        ,[fc].[call.mediatype]
                                        ,[fc].[call.number]
                                        ,[fc].[call.session_id]
                                        ,[fc].[call.campaign_name]
                                        ,[fc].[call.skill_name]
                                        ,[fc].[call.comments]
                                        ,[fc].[call.start_timestamp]
                                        ,[fc].[call.end_timestamp]
                                        ,[fc].[call.tcpa_date_of_consent]
                                        ,[fc].[call.queue_time]
                                        ,[fc].[call.hold_time]
                                        ,[fc].[call.park_time]
                                        ,[fc].[call.wrapup_time]
                                        ,[fc].[call.bill_time]
                                        ,[fc].[call.handle_time]
                                        ,[fc].[call.length]
                                        FROM [dbo].[five9_calls] [fc] WITH(NOLOCK)
                                        WHERE 1=1
                                        AND [fc].[companyid] = @sp_companyid
                                        AND [fc].[interactionid] = @sp_interactionid
                                        ";
                        }
                        #endregion dvFive9Calls
                        #region dvFive9CallsDisposition
                        if (dv.ID == "dvFive9CallsDisposition")
                        {
                            cmdText = @"
                                        SELECT
                                        TOP 1
                                        '--------------' [five9_calls_disposition]
                                        ,[fcd].[companyid]
                                        ,[fcd].[interactionid]
                                        ,[fcd].[call.call_id]
                                        ,[fcd].[agent.id]
                                        ,[fcd].[createdate]
                                        ,[fcd].[agent.full_name]
                                        ,[fcd].[agent.station_id]
                                        ,[fcd].[agent.station_type]
                                        ,[fcd].[agent.user_name]
                                        ,[fcd].[call.disposition_id]
                                        ,[fcd].[call.disposition_name]
                                        FROM [dbo].[five9_calls_disposition] [fcd] WITH(NOLOCK)
                                        WHERE 1=1
                                        AND [fcd].[companyid] = @sp_companyid
                                        AND [fcd].[interactionid] = @sp_interactionid
                                        ";
                        }
                        #endregion dvFive9CallsDisposition
                        #endregion Build cmdText
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #region SQL Parameters
                        cmd.Parameters.Add(new SqlParameter("@sp_companyid", sp_companyid));
                        cmd.Parameters.Add(new SqlParameter("@sp_interactionid", sp_interactionid));
                        #endregion SQL Parameters
                        #region SQL Processing
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        dv.DataSource = dt;
                        dv.DataBind();
                        #endregion SQL Processing

                    }
                    #endregion SQL Command
                }
                #endregion SQL Connection

            }
            else
            {
                lblSystemMessage.Text = "Somethign went wrong.. no Company or Interaction";
            }
        }
        catch (Exception ex)
        {
            lblSystemMessage.Text = "Somethign went wrong.. Error with Company or Interaction";
            Error_Display(ex, "DetailsView_Data | " + dv.ID, msgLabel);
        }


    }
    protected void DetailsView_DataExchange_Data(Int32 CallID, DetailsView dv)
    {
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region Build cmdText
                String cmdText = "";
                #region dvInteraction
                if (dv.ID == "dvInteraction") {
                    cmdText = @"
SELECT
TOP 1
[i].[companyid]
,[i].[interactionid]
,[ia].[arc.callid]
,[ia].[arc.disposition_name]
,[fcd].[agent.full_name]
,[fcd].[call.disposition_id]
,[fcd].[call.disposition_name]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_calls_disposition] [fcd] WITH(NOLOCK) ON [fcd].[interactionid] = [i].[interactionid] AND [fcd].[companyid] = [i].[companyid]
WHERE [ia].[arc.callid] = @sp_arc_callid
";
                }
                #endregion
                #region dvInteractionARC
                if (dv.ID == "dvInteractionARC")
                {
                    cmdText = @"
SELECT
TOP 1
[i].[companyid]
,[i].[interactionid]
,[ia].[arc.callid]
,[ia].[arc.disposition_name]
,[fc].[call.ani]
,[fc].[call.dnis]
,[fc].[call.call_id]
,[fc].[call.campaign_name]
,[fc].[call.skill_name]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_calls] [fc] WITH(NOLOCK) ON [fc].[interactionid] = [i].[interactionid] AND [fc].[companyid] = [i].[companyid]
WHERE [ia].[arc.callid] = @sp_arc_callid
";
                }
                #endregion
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_arc_callid", CallID));
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dv.DataSource = dt;
                dv.DataBind();
                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void DetailsView_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        // Admin_Clear();
        //if (Page.User.IsInRole("System Administrator")) pAdminFunctions.Visible = true;
        if (ghUser.identity_is_admin()) pAdminFunctions.Visible = true;
        DetailsView dv = (DetailsView)sender;
        if (dv.ID == "dvInteractionARC")
        {
            if (e.CommandName == "Refund")
            {
                //Refund_Start(dv, e);
            }
        }
        else if (dv.ID == "dvPaymentDetailsRecurring")
        {
            if (e.CommandName == "Refund")
            {
                //Refund_Start(dv, e);
            }
        }
        else if (dv.ID == "dvSustainerDetails")
        {
            if (e.CommandName == "Modify")
            {
                //Modify_Sustainer(dv, e);
            }
        }
    }
    protected void GridView_Data2(Int32 CallID, Int32 DonorID, GridView gv, Panel pnl)
    {
        gv.SelectedIndex = -1;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region Build cmdText
                String cmdText = "";
                if (gv.ID == "gvGiftList") { cmdText = "[dbo].[portal_call_search_get_gift_list]"; }
                if (gv.ID == "gvRecurringList") { cmdText = "[dbo].[portal_call_search_get_recurring_list]"; }
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_callid", CallID));
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt;
                gv.DataBind();
                if (dt.Rows.Count > 0)
                {
                    pnl.Visible = true;
                    gv.Visible = true;
                    //gvRecurringListExport.DataSource = dt;
                    //gvRecurringListExport.DataBind();
                }
                else
                {
                    pnl.Visible = false;
                    gv.Visible = false;
                    //gvRecurringListExport.DataSource = null;
                    //gvRecurringListExport.DataBind();
                }

                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    #endregion Details View Handling

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

        //Error_Save(ex, error);
        //ErrorLog.ErrorLog(ex);
        //pnlError.Visible = true;
    }
    protected string Repeat(char character, int numberOfIterations)
    {
        return "".PadLeft(numberOfIterations, character);
    }
    protected void print_sql(SqlCommand cmd, String type)
    {
        ghFunctions.print_sql(cmd, sqlPrint, type);
        //lblGraphStatsHeaderNote.Text = "Last Refreshed: " + DateTime.Now.ToString("hh:mm:ss tt");
    }
    protected void Dump()
    {
        #region dv find control and stuff
        //string decision = dvPaymentDetails.Rows[2].Cells[0].Text;

        //lblRefundTemp.Text += "<br />CallID " + callid.ToString();
        //int i = 0;
        //lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + dvPaymentDetails.Rows[i].Cells[1].Text;
        //i = 1;
        //lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + dvPaymentDetails.Rows[i].Cells[1].Text;
        //if (dvPaymentDetails.Rows[i].Cells[0].Text == "cbid")
        //{
        //    // Error check here
        //    lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + ((Label)dvPaymentDetails.Rows[i].FindControl("lbl_cbid")).Text;
        //}
        //i = 2;
        //lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + dvPaymentDetails.Rows[i].Cells[1].Text;
        //i = 3;
        //lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + dvPaymentDetails.Rows[i].Cells[1].Text;
        #endregion
        #region old refund processing
        #region Old Code
        ////lblCallID.Text = sqlRdr["callid"].ToString();
        //lblExternalID.Text = sqlRdr["ExternalID"].ToString();
        //lblStatus.Text = sqlRdr["Status"].ToString();
        //CreateDate.Text = sqlRdr["CreateDate"].ToString();
        //RequestID.Text = sqlRdr["requestID"].ToString();
        //RequestToken.Text = sqlRdr["requestToken"].ToString();
        //ReferenceNum.Text = sqlRdr["merchantReferenceCode"].ToString();
        //Amount.Text = sqlRdr["ccAuthReply_amount"].ToString();
        //AmountOriginal.Value = sqlRdr["ccAuthReply_amount"].ToString();
        //// Last 4 of Card Number
        //CardNumber.Text = sqlRdr["ccnum"].ToString().Substring(sqlRdr["ccnum"].ToString().Length - 4, 4);
        //FirstName.Text = sqlRdr["fname"].ToString();
        //LastName.Text = sqlRdr["lname"].ToString();
        //DateTime dtChargeDate;
        //DateTime.TryParse(CreateDate.Text, out dtChargeDate);
        //if (dtChargeDate != null)
        //{
        //    dtlLabel.Text = (dtChargeDate - DateTime.UtcNow).TotalDays.ToString();
        //    if ((DateTime.UtcNow - dtChargeDate).TotalDays > foLimit)
        //    {
        //        RefundType.Value = "Stand Alone";
        //        pnl_standalone.Visible = true;
        //        CardNumberFull.Value = sqlRdr["ccnum"].ToString();
        //        CardMonth.Text = sqlRdr["ccexpmonth"].ToString();
        //        CardYear.Text = sqlRdr["ccexpyear"].ToString();
        //        /*
        //         * 001 == Visa == 2
        //         * 002 == MasterCard == 3
        //         * 003 == American Express == 4
        //         * 004 == Discover == 5
        //         */
        //        if (CardNumberFull.Value.Length > 1)
        //        {
        //            switch (CardNumberFull.Value.Substring(0, 1))
        //            {
        //                case "4":
        //                    CardType.Text = "Visa";
        //                    CardTypeFull.Value = "001";
        //                    break;
        //                case "5":
        //                    CardType.Text = "MasterCard";
        //                    CardTypeFull.Value = "002";
        //                    break;
        //                case "3":
        //                    CardType.Text = "American Express";
        //                    CardTypeFull.Value = "003";
        //                    break;
        //                case "6":
        //                    CardType.Text = "Discover";
        //                    CardTypeFull.Value = "004";
        //                    break;
        //            }
        //        }
        //        Address1.Text = sqlRdr["address"].ToString();
        //        Address2.Text = sqlRdr["suitenumber"].ToString();
        //        //Address3.Text = sqlRdr[""].ToString();
        //        City.Text = sqlRdr["city"].ToString();
        //        ddlState.Text = sqlRdr["state"].ToString();
        //        Zip.Text = sqlRdr["zip"].ToString();
        //        ddlCountry.Text = "USA";
        //    }
        //    else
        //    {
        //        RefundType.Value = "Follow On";
        //    }
        //}
        //if (sqlRdr["Status"].ToString() == "Settled" || sqlRdr["Status"].ToString() == "Approved")
        //{
        //    btnRefundSubmit.Enabled = true;
        //}
        //else
        //{
        //    dtlLabel.Text = "The record is not in a valid refundable status.";
        //    dtlLabel.ForeColor = System.Drawing.Color.Red;
        //}
        #endregion Old Code

        #endregion
    }

}
