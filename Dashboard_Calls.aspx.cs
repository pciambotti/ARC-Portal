using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChartDirector;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using ClosedXML.Excel.Drawings;
using System.Net;
public partial class Dashboard_Calls : System.Web.UI.Page
{
    private String sqlStrARC = Connection.GetConnectionString("ARC_Production", ""); // ARC_Production || ARC_Stage
    private String sqlStrDE = Connection.GetConnectionString("DE_Production", ""); // DE_Production || DE_Stage
    private String sqlStr = Connection.GetConnectionString("MiddleWare", "");
    private Int32 countCalls = 0;
    private Int32 clientID = 0; // This is not used
    private bool campaigns = false;
    private bool skills = false;
    private bool agents = false;

    protected void Page_PreInit(Object sender, EventArgs e)
    {
        if (Session["userid"] == null) { ghUser.identity_get_userid(); ghUser.identity_get_client(); Response.Redirect("~/Dashboard.aspx"); }
        Master.PageTitle = "Dashboard";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
        if (ghFunctions.dtUserOffSet == 0)
        {
            /// Switch this to a user determined variable
            /// Possibly in the MasterPage
            Int32 dtOffSet = 5;
            DateTime dtCurrent = DateTime.Now;
            System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
            if (localZone.IsDaylightSavingTime(dtCurrent))
            {
                dtOffSet = 4;
            }
            else
            {
                dtOffSet = 5;
            }
            ghFunctions.dtUserOffSet = dtOffSet;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        //{
        //    lblClientCampaigns3.Text = String.Format("<br />Now [{0}]", DateTime.Now.ToString("HH:mm:ss"));
        //}
        if (!IsPostBack)
        {
            #region Client Filter
            // If a CLIENT logs in, we need to restrict which clients they see
            // if (Page.User.IsInRole("Client"))
            if (!Page.User.IsInRole("System Administrator"))
            {
                if (Session["clientname"] == null || Session["clientname"].ToString().Length == 0) { ghUser.identity_get_client(); }
                if (Session["clientname"] == null || Session["clientname"].ToString().Length == 0)
                {
                    clientID = -1;
                    lblClientCampaigns2.Text = String.Format("<br />Client not supported [{0}]", DateTime.Now.ToString("HH:mm:ss"));
                }
                else
                {
                    lblClientCampaigns2.Text = "<br />Client: " + Session["clientname"].ToString();
                    // lblClientCampaigns2.Text += "<br />Client: " + Session["userid"].ToString();
                }
            }
            if (Page.User.IsInRole("System Administrator"))
            {
                Session["clientname"] = "";
                lblClientCampaigns2.Text = "<br />Client: " + "Admin";
            }
            #endregion Client Filter

            DropDown_Clients();
            Client_Selected();

            rpTimeZone.Text += "-" + ghFunctions.dtUserOffSet.ToString() + " (US Eastern Timezone)";

            #region Chart Image
            if (Session["imgNameDash"] == null || !IsPostBack)
            {
                // Get the Image Name for Charts
                //this.Page.User.Identity.Name
                //if (Page.User.IsInRole("System Administrator") == true && Page.User.Identity.Name == "nciambotti@greenwoodhall.com")
                //userid
                
                string uName = Page.User.Identity.Name;
                if (uName.IndexOf("@") > 0)
                {
                    uName = uName.Substring(0, uName.IndexOf("@"));
                }

                string aName = "";
                if (Page.User.IsInRole("System Administrator") == true) aName = "SA";
                else if (Page.User.IsInRole("Administrator") == true) aName = "AM";
                else if (Page.User.IsInRole("Manager") == true) aName = "MA";
                else if (Page.User.IsInRole("Advisor") == true) aName = "AD";
                else if (Page.User.IsInRole("Agent") == true) aName = "AG";
                else if (Page.User.IsInRole("Client") == true) aName = "CL";
                else aName = "AO";
                string uID = Session["userid"].ToString().PadLeft(2, '0');

                String imgName = "";
                imgName = String.Format("{0}_{1}_{2}_[client]_dashboard_reporting_[type].png", uName, uID, aName);
                Session["imgNameDash"] = imgName;
            }
            #endregion Chart Image
            // If a CLIENT logs in, we need to restrict which clients they see
            // filter_clients
            #region Refresh Timer
            gvCR_tglTimer.Checked = true;
            gvCountReport_Timer1.Enabled = false;
            gvCR_lblTimer.Text = "stopped";
            gvCR_lblTimer.ForeColor = System.Drawing.Color.Red;

            if (!gvCountReport_Timer1.Enabled) { ScriptManager.RegisterClientScriptBlock(gvCountReport_Timer, this.Page.GetType(), "RefreshCountdown", "RefreshCountdownStart('gvCR_refreshCountdown',0);", true); }
            //GridView_Refresh_Data(0, this.Page.User.Identity.Name, gvCountReport, null);
            #endregion Refresh Timer
        }
        if (ddlCallCampaigns.SelectedIndex != -1 && ddlCallCampaigns.SelectedValue.Length > 0) { campaigns = true; }
        if (ddlCallSkills.SelectedIndex != -1 && ddlCallSkills.SelectedValue.Length > 0) { skills = true; }
        if (ddlCallAgents.SelectedIndex != -1 && ddlCallAgents.SelectedValue.Length > 0) { agents = true; }
        if (!IsPostBack)
        {
            GridView_Stats_Dashboard();
            if (gvStatsDashboard.Rows.Count == 1)
            {
                DateTime dtEnd;
                if (DateTime.TryParse(gvStatsDashboard.Rows[0].Cells[2].Text, out dtEnd))
                {
                    dtStartDate.Text = dtEnd.AddDays(-1).ToString("MM/dd/yyyy");
                    dtEndDate.Text = dtEnd.AddDays(-1).ToString("MM/dd/yyyy");
                }
            }
        }
    }
    protected void GridView_Stats_Dashboard()
    {
        GridView gv = gvStatsDashboard;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {

            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                #region Build cmdText
                String cmdText = ghQueries.dashboard_total_stats_dynamic(campaigns, skills, agents);
                #endregion Build cmdText
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = 3;

                #region Campaign
                if (ddlCallCampaigns.SelectedIndex != -1 && ddlCallCampaigns.SelectedValue.Length > 0)
                {
                    DataTable clientCampaigns = dt_Item_Campaigns();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Campaign
                #region Skill
                if (ddlCallSkills.SelectedIndex != -1 && ddlCallSkills.SelectedValue.Length > 0)
                {
                    DataTable itemSkills = dt_Item_Skills();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_skills", itemSkills);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Skill
                #region Agent
                if (ddlCallAgents.SelectedIndex != -1 && ddlCallAgents.SelectedValue.Length > 0)
                {
                    DataTable itemSkills = dt_Item_Agents();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_agents", itemSkills);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Agent
                #endregion SQL Parameters
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
    protected void gvReport_TimerToggle(object sender, EventArgs e)
    {
        CheckBox tglCheck = (CheckBox)sender;
        if (tglCheck != null)
        {
            Timer tglTimer = null;
            Label tglLabel = null;
            String refreshCnt = "";
            System.Web.UI.HtmlControls.HtmlGenericControl divReport = null;
            if (tglCheck.ID == "gvCR_tglTimer")
            {
                tglTimer = gvCountReport_Timer1;
                tglLabel = gvCR_lblTimer;
                refreshCnt = "gvCR_refreshCountdown";
                divReport = gvCountReport_Timer;
            }
            if (tglTimer != null)
            {
                if (tglCheck.Checked)
                {
                    tglLabel.Text = "stopped";
                    tglLabel.ForeColor = System.Drawing.Color.Red;
                    tglTimer.Enabled = false;
                    ScriptManager.RegisterClientScriptBlock(divReport, this.Page.GetType(), "RefreshCountdown", "RefreshCountdownStart('" + refreshCnt + "',0);", true);
                }
                else
                {
                    Session["PageLoad"] = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");
                    rpElapsed.ForeColor = System.Drawing.Color.Black;

                    tglLabel.Text = "active";
                    tglLabel.ForeColor = System.Drawing.Color.Green;
                    tglTimer.Enabled = true;
                    tglTimer.Interval = 500;
                    int scnds = tglTimer.Interval;
                    if (scnds > 0) { scnds = scnds / 1000; }
                    ScriptManager.RegisterClientScriptBlock(divReport, this.Page.GetType(), "RefreshCountdown", "RefreshCountdownStart('" + refreshCnt + "'," + scnds.ToString() + ");", true);
                }
            }
        }
    }
    protected void gvCountReport_TimerTick(object sender, EventArgs e)
    {
        bool run = true;
        Timer tickTimer = (Timer)sender;
        // If we can not recognize the timer.. oops
        if (tickTimer != null)
        {
            tickTimer.Interval = 10000;
            if (tickTimer.ID == "gvCountReport_Timer1")
            {

            }
            // 
            #region The elapsed time part to ensure we do not run for ever
            if (Session["PageLoad"] != null)
            {
                DateTime dt = DateTime.Parse(Session["PageLoad"].ToString());
                Double scnds = (DateTime.UtcNow - dt).TotalSeconds;
                Double scnds_max = 3600;
                rpElapsed.Text = "Elapsed: " + ghFunctions.SecondsTo(scnds) + " | ";
                if (scnds > scnds_max)
                {
                    run = false;
                    rpElapsed.Text += " hard stop";
                    gvCR_tglTimer.Checked = true;
                    tickTimer.Enabled = false;
                    gvCR_lblTimer.Text = "stopped";
                    gvCR_lblTimer.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    Double scnds_left = (scnds_max - scnds);
                    rpElapsed.Text += " automated timers will stop in " + ghFunctions.SecondsTo(scnds_left);
                    if (scnds_left < 300) { rpElapsed.ForeColor = System.Drawing.Color.Red; }
                }
            }
            #endregion The elapsed time part to ensure we do not run for ever
            if (run) { Dashboard_Refresh(); }
        }
        else
        {
            rpElapsed.Text = "Error";
        }
    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {

        lblWeekDay.Text = "";
        if (dtStartDate.Text == dtEndDate.Text)
        {
            DateTime dtDayOfWeek = DateTime.Parse(dtStartDate.Text);
            lblWeekDay.Text = dtDayOfWeek.DayOfWeek.ToString();
        }

        Dashboard_Refresh();
        UpdatePanel1.Update();
    }
    protected void Dashboard_Refresh()
    {
        #region Timer
        gvCR_lstRefresh.Text = DateTime.UtcNow.ToString("HH:mm:ss");
        if (gvCountReport_Timer1.Enabled)
        {
            int scnds = gvCountReport_Timer1.Interval;
            if (scnds > 0) { scnds = scnds / 1000; }
            ScriptManager.RegisterClientScriptBlock(gvCountReport_Timer, this.Page.GetType(), "RefreshCountdown", "RefreshCountdownStart('gvCR_refreshCountdown'," + scnds.ToString() + ");", true);
        }
        else { ScriptManager.RegisterClientScriptBlock(gvCountReport_Timer, this.Page.GetType(), "RefreshCountdown", "RefreshCountdownStart('gvCR_refreshCountdown',0);", true); }
        #endregion Timer
        lblLoadTime.Text = "";
        //btnCallDispositions.Visible = false;
        try
        {
            // if (ddlCallCampaigns.SelectedIndex == -1)
            // if (!campaigns && !skills && !agents)
            // We can't allow non campaign selection because that's how we filter calls for [clients]
            // Unless we're an Admin
            bool refreshDashboard = true;
            if (Page.User.IsInRole("System Administrator") == true || Page.User.IsInRole("Administrator") == true)
            {
                if (!campaigns && !skills && !agents)
                {
                    rpMessage.Text = "You must select a List Filter (Campapign, Skill, or Agent).";
                    Dashboard_Reset();
                    refreshDashboard = false;
                }

            }
            else if (ddlCallCampaigns.SelectedIndex == -1)
            {
                rpMessage.Text = "You must select at least 1 campaign to continue.";
                Dashboard_Reset();
                refreshDashboard = false;
            }

            if (refreshDashboard)
            {
                DateTime dt = DateTime.UtcNow;
                rpMessage.Text = "";
                lblMessage.Text = "Date Range: " + dtStartDate.Text + " " + dtStartTime.Text + " to " + dtEndDate.Text + " " + dtEndTime.Text;

                Data_Call_Counts();
                #region Have Calls
                if (countCalls > 0)
                {
                    rpMessage.Text = "";

                    pnlCCPerformance.Visible = true; //btnCCPerformance.Visible = true;
                    pnlIntervalAbandon.Visible = true;
                    pnlIntervalAnswer.Visible = true; //btnInterval.Visible = true;
                    btnExportFull.Visible = true;
                    pnlCallTimes.Visible = true;

                    Data_Call_Dispositions();
                    Data_Campaign_Count();
                    Data_Support_Level();
                }
                #endregion Have Calls
                #region No Calls
                else
                {
                    rpMessage.Text = "Returned filters produced no call records.";
                    pnlCCPerformance.Visible = false; //btnCCPerformance.Visible = false;
                    pnlIntervalAbandon.Visible = false;
                    pnlIntervalAnswer.Visible = false; //btnInterval.Visible = false;
                    btnExportFull.Visible = false;
                    pnlCallTimes.Visible = false;
                    gvCallDispositions.DataBind();
                    totalDispos.Text = "";
                }
                #endregion No Calls
                DateTime dtEnd = DateTime.UtcNow;
                TimeSpan t = (dtEnd - dt);
                lblLoadTime.Text += "<br />Load Time: " + string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
            }
        }
        catch (Exception ex)
        {
            //Error_Save(ex, "GridView_Refresh");
            Error_Display(ex, "GridView_Refresh", msgLabel);
        }
    }
    protected void Data_Call_Counts()
    {
        GridView gv = gvCallDispositions;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region Build cmdText
                String cmdText = ghQueries.dashboard_call_counts_dynamic(campaigns, skills, agents);
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                dtFrom = dtOffSetAdd(dtFrom);
                dtTo = dtOffSetAdd(dtTo);

                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = 3;
                cmd.Parameters.Add("@sp_offset", SqlDbType.Int).Value = ghFunctions.dtUserOffSet;

                //DataTable clientCampaigns = dt_Client_Campaigns();
                //SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                //tvpParam.SqlDbType = SqlDbType.Structured;
                //tvpParam.TypeName = "dbo.de_campaigns";

                #region Campaign
                if (ddlCallCampaigns.SelectedIndex != -1 && ddlCallCampaigns.SelectedValue.Length > 0)
                {
                    DataTable clientCampaigns = dt_Item_Campaigns();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Campaign
                #region Skill
                if (ddlCallSkills.SelectedIndex != -1 && ddlCallSkills.SelectedValue.Length > 0)
                {
                    DataTable itemSkills = dt_Item_Skills();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_skills", itemSkills);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Skill
                #region Agent
                if (ddlCallAgents.SelectedIndex != -1 && ddlCallAgents.SelectedValue.Length > 0)
                {
                    DataTable itemSkills = dt_Item_Agents();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_agents", itemSkills);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Agent
                #endregion SQL Parameters
                GridView_Stats_Query(cmd);

                //print_sql(cmd); // Will print for Admin in Local
                double totalCalls = 0;
                double answered_90 = 0;
                double answered_120 = 0;
                double answered_120p = 0;
                double totalAnswered = 0;
                double abandon_90 = 0;
                double abandon_120 = 0;
                double abandon_120p = 0;
                double totalAbandoned = 0;
                #region SQL Processing - Reader
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            #region process sqlRdr
                            totalCalls += Convert.ToDouble(sqlRdr["total_calls"].ToString());
                            countCalls = Convert.ToInt32(totalCalls);

                            answered_90 += Convert.ToDouble(sqlRdr["answered_90"].ToString());
                            answered_120 += Convert.ToDouble(sqlRdr["answered_120"].ToString());
                            answered_120p += Convert.ToDouble(sqlRdr["answered_120p"].ToString());
                            totalAnswered = answered_90 + answered_120 + answered_120p;

                            abandon_90 += Convert.ToDouble(sqlRdr["abandon_90"].ToString());
                            abandon_120 += Convert.ToDouble(sqlRdr["abandon_120"].ToString());
                            abandon_120p += Convert.ToDouble(sqlRdr["abandon_120p"].ToString());
                            totalAbandoned = abandon_90 + abandon_120 + abandon_120p;

                            cntTotalCalls.Text = totalCalls.ToString();
                            cntTotalAnswered.Text = totalAnswered.ToString();
                            cntTotalAbandoned.Text = totalAbandoned.ToString();

                            perTotalCalls.Text = String.Format("{0:P2}", GetPercent(totalCalls, totalCalls));
                            perTotalAnswered.Text = String.Format("{0:P2}", GetPercent(totalAnswered, totalCalls));
                            perTotalAbandoned.Text = String.Format("{0:P2}", GetPercent(totalAbandoned, totalCalls));

                            // cntAnswerPerf.Text = answered_90.ToString();
                            // perAnswerPerf.Text = String.Format("{0:P2}", GetPercent(answered_90, totalAnswered));
                            cntSupportLevel.Value = (GetPercent(answered_90, totalAnswered) * 100).ToString();

                            cntAnswered90.Text = answered_90.ToString();
                            perAnswered90.Text = String.Format("{0:P2}", GetPercent(answered_90, totalAnswered));
                            cntAnswered120.Text = answered_120.ToString();
                            perAnswered120.Text = String.Format("{0:P2}", GetPercent(answered_120, totalAnswered));
                            cntAnswered120p.Text = answered_120p.ToString();
                            perAnswered120p.Text = String.Format("{0:P2}", GetPercent(answered_120p, totalAnswered));

                            cntAbandoned90.Text = abandon_90.ToString();
                            perAbandoned90.Text = String.Format("{0:P2}", GetPercent(abandon_90, totalAbandoned));
                            cntAbandoned120.Text = abandon_120.ToString();
                            perAbandoned120.Text = String.Format("{0:P2}", GetPercent(abandon_120, totalAbandoned));
                            cntAbandoned120p.Text = abandon_120p.ToString();
                            perAbandoned120p.Text = String.Format("{0:P2}", GetPercent(abandon_120p, totalAbandoned));


                            avgSpeedAnswer.Text = String.IsNullOrEmpty(sqlRdr["queue_avg"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["queue_avg"].ToString());
                            totalSpeedAnswer.Text = String.IsNullOrEmpty(sqlRdr["queue_total"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["queue_total"].ToString());

                            avgTalkTime.Text = String.IsNullOrEmpty(sqlRdr["talk_avg"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["talk_avg"].ToString());
                            totalTalkTime.Text = String.IsNullOrEmpty(sqlRdr["talk_total"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["talk_total"].ToString());

                            avgAbandonedTime.Text = String.IsNullOrEmpty(sqlRdr["abandon_avg"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["abandon_avg"].ToString());
                            totalAbandonedTime.Text = String.IsNullOrEmpty(sqlRdr["abandon_total"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["abandon_total"].ToString());

                            avgAfterCallTime.Text = String.IsNullOrEmpty(sqlRdr["wrapup_avg"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["wrapup_avg"].ToString());
                            totalAfterCallTime.Text = String.IsNullOrEmpty(sqlRdr["wrapup_total"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["wrapup_total"].ToString());

                            avgQueueTime.Text = String.IsNullOrEmpty(sqlRdr["queue_avg"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["queue_avg"].ToString());
                            totalQueueTime.Text = String.IsNullOrEmpty(sqlRdr["queue_total"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["queue_total"].ToString());

                            avgHandleTime.Text = String.IsNullOrEmpty(sqlRdr["handle_avg"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["handle_avg"].ToString());
                            totalHandleTime.Text = String.IsNullOrEmpty(sqlRdr["handle_total"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["handle_total"].ToString());

                            avgIVRTime.Text = String.IsNullOrEmpty(sqlRdr["ivr_avg"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["ivr_avg"].ToString());
                            totalIVRTime.Text = String.IsNullOrEmpty(sqlRdr["ivr_total"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["ivr_total"].ToString());

                            avgThirdPartyTime.Text = String.IsNullOrEmpty(sqlRdr["third_avg"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["third_avg"].ToString());
                            totalThirdPartyTime.Text = String.IsNullOrEmpty(sqlRdr["third_total"].ToString()) ? "00:00:00" : ghFunctions.SecondsTo(sqlRdr["third_total"].ToString());
                            #endregion process sqlRdr
                        }
                    }
                    else
                    {
                        cntSupportLevel.Value = "-1";
                    }
                }
                #endregion SQL Processing - Reader

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GridView_Stats_Query(SqlCommand cmd)
    {
        GridView gv = gvStatsQuery;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Processing
            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            gv.DataSource = dt;
            gv.DataBind();
            #endregion SQL Processing
        }
        #endregion SQL Connection
    }
    protected void Data_Call_Dispositions()
    {
        GridView gv = gvCallDispositions;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }

                cmd.CommandTimeout = 600;
                #region Build cmdText
                String cmdText = ghQueries.dashboard_call_dispositions_dynamic(campaigns, skills, agents);
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                dtFrom = dtOffSetAdd(dtFrom);
                dtTo = dtOffSetAdd(dtTo);

                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = 3;
                cmd.Parameters.Add("@sp_offset", SqlDbType.Int).Value = ghFunctions.dtUserOffSet;

                //DataTable clientCampaigns = dt_Client_Campaigns();
                //SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                //tvpParam.SqlDbType = SqlDbType.Structured;
                //tvpParam.TypeName = "dbo.de_campaigns";

                #region Campaign
                if (ddlCallCampaigns.SelectedIndex != -1 && ddlCallCampaigns.SelectedValue.Length > 0)
                {
                    DataTable clientCampaigns = dt_Item_Campaigns();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Campaign
                #region Skill
                if (ddlCallSkills.SelectedIndex != -1 && ddlCallSkills.SelectedValue.Length > 0)
                {
                    DataTable itemSkills = dt_Item_Skills();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_skills", itemSkills);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Skill
                #region Agent
                if (ddlCallAgents.SelectedIndex != -1 && ddlCallAgents.SelectedValue.Length > 0)
                {
                    DataTable itemSkills = dt_Item_Agents();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_agents", itemSkills);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Agent

                #endregion SQL Parameters
                #region SQL Processing - GridView
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                int dCount = DataTable_Row_Sum_Values(dt, 2);
                hfDispoCount.Value = dCount.ToString();
                totalDispos.Text = "Total Disposition Count: " + dCount.ToString();
                gv.DataSource = dt;
                gv.DataBind();
                //if (dCount > 0) btnCallDispositions.Visible = true;
                #endregion SQL Processing - GridView

            }
            #endregion SQL Command
        }
        #endregion SQL Connection

    }
    protected void Data_Campaign_Count()
    {
        GridView gv = gvCampaignCount;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                #region Build cmdText
                String cmdText = ghQueries.dashboard_campaign_count_dynamic(campaigns, skills, agents);
                #endregion Build cmdText
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();

                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                dtFrom = dtOffSetAdd(dtFrom);
                dtTo = dtOffSetAdd(dtTo);

                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = 3;
                cmd.Parameters.Add("@sp_offset", SqlDbType.Int).Value = ghFunctions.dtUserOffSet;

                #region Campaign
                if (ddlCallCampaigns.SelectedIndex != -1 && ddlCallCampaigns.SelectedValue.Length > 0)
                {
                    DataTable clientCampaigns = dt_Item_Campaigns();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Campaign
                #region Skill
                if (ddlCallSkills.SelectedIndex != -1 && ddlCallSkills.SelectedValue.Length > 0)
                {
                    DataTable itemSkills = dt_Item_Skills();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_skills", itemSkills);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Skill
                #region Agent
                if (ddlCallAgents.SelectedIndex != -1 && ddlCallAgents.SelectedValue.Length > 0)
                {
                    DataTable itemSkills = dt_Item_Agents();
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_agents", itemSkills);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.de_items";
                }
                #endregion Agent
                #endregion SQL Parameters
                #region SQL Processing - GridView
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                int dCount = DataTable_Row_Sum_Values(dt, 1);
                hfCampaignCount.Value = dCount.ToString();
                totalDispos.Text = "Total Count: " + dCount.ToString();
                gv.DataSource = dt;
                gv.DataBind();
                if (dCount > 0) gv.Visible = true;
                #endregion SQL Processing - GridView

            }
            #endregion SQL Command
        }
        #endregion SQL Connection

    }
    protected void Data_Support_Level()
    {
        double sprtlvl = 0;
        Double.TryParse(cntSupportLevel.Value, out sprtlvl);
        //double sprtlvl = Convert.ToDouble(cntSupportLevel.Value);

        int chartIndex = 0;
        // The value to display on the meter
        double value = sprtlvl;

        // The background and border colors of the meters
        //int[] bgColor = { 0x88CCFF, 0xFFDDDD };
        int[] bgColor = { 0xf5f5f5, 0xFFDDDD };
        int[] borderColor = { 0xcccccc, 0x880000 };

        // Create an AngularMeter object of size 300 x 200 pixels with transparent background
        // AngularMeter m = new AngularMeter(300, 200, Chart.Transparent);
        AngularMeter m = new AngularMeter(250, 200, Chart.Transparent);

        // Center at (150, 150), scale radius = 124 pixels, scale angle -90 to +90 degrees
        //m.setMeter(150, 150, 124, -90, 90);
        m.setMeter(125, 150, 100, -90, 90);

        // Background gradient color with brighter color at the center
        double[] bgGradient = { 0, m.adjustBrightness(bgColor[chartIndex], 3), 0.75, bgColor[chartIndex] }
            ;

        // Add a scale background of 148 pixels radius using the background gradient, with a 13 pixel
        // thick border
        m.addScaleBackground(124, m.relativeRadialGradient(bgGradient), 13, borderColor[chartIndex]);

        // Meter scale is 0 - 100, with major tick every 20 units, minor tick every 10 units, and micro tick every 5 units
        //m.setScale(0, 100, 20, 10, 5);
        // Meter scale is 0 - 100, with out major tick, minor tick every 10 units, and micro tick every 5 units
        m.setScale(0, 100, 0, 10, 5);

        // Set the scale label style to 15pt Arial Italic. Set the major/minor/micro tick lengths to
        // 16/16/10 pixels pointing inwards, and their widths to 2/1/1 pixels.
        m.setLabelStyle("Arial Italic", 16);
        m.setTickLength(-16, -16, -10);
        m.setLineWidth(0, 2, 1, 1);

        // Demostrate different types of color scales and putting them at different positions
        double[] smoothColorScale = { 0, 0xff0000, 25, 0xdddd00, 50, 0x3333ff, 75, 0x0088ff, 100, 0x00ff00 };
        Int32 vColor = 0x00ff00;
        if (value < 25) vColor = 0xff0000;
        if (value >= 25 && value < 50) vColor = 0xdddd00;
        if (value >= 50 && value < 75) vColor = 0x3333ff;
        if (value >= 75 && value < 90) vColor = 0x0088ff;

        if (chartIndex == 0)
        {
            // Add the smooth color scale at the default position
            m.addColorScale(smoothColorScale);
            // Add a red (0xff0000) triangular pointer starting from 38% and ending at 60% of scale
            // radius, with a width 6 times the default
            m.addPointer2(value, 0x595959, -1, Chart.TriangularPointer2, 0.38, 0.6, 6);
        }
        else
        {
            // Add the smooth color scale starting at radius 124 with zero width and ending at radius
            // 124 with 16 pixels inner width
            m.addColorScale(smoothColorScale, 124, 0, 124, -16);
            // Add a red (0xff0000) pointer
            m.addPointer2(value, 0xFF0000);
        }

        // Configure a large "pointer cap" to be used as the readout circle at the center. The cap
        // radius and border width is set to 33% and 4% of the meter scale radius. The cap color is dark
        // blue (0x000044). The border color is light blue (0x66bbff) with a 60% brightness gradient
        // effect.
        m.setCap2(Chart.Transparent, 0xbfbfbf, 0x666666, 0.6, 0, 0.33, 0.04);

        // Add value label at the center with light blue (0x66ddff) 28pt Arial Italic font
        m.addText(125, 150, m.formatValue(value, "{value|0}"), "Arial Italic", 24, vColor, Chart.Center).setMargin(0);

        // Output the chart
        // http://www.advsofteng.com/doc/cdnetdoc/outputfile.htm
        chartSupportLevel.Image = m.makeWebImage(Chart.PNG);
        string iName = Session["imgNameDash"].ToString().Replace("[type]", "sl").Replace("[client]", ddlCallClients.SelectedValue.PadLeft(3, '0'));
        m.makeChart(Server.MapPath("/offline/charts/" + iName));
    }
    protected Int32 DataTable_Row_Sum_Values(DataTable dt, Int32 col)
    {
        int dtSum = 0;
        foreach (DataRow r in dt.Rows)
        {
            dtSum += Convert.ToInt32(r[col].ToString());
        }
        return dtSum;
    }
    protected void Client_Index_Changed(object sender, EventArgs e)
    {
        Client_Selected();
        GridView_Stats_Dashboard();
    }
    protected void Dashboard_Reset()
    {
        #region Reset the Dashboard
        // This is done to prevent the user from causing an export error
        // Since the chart images are saved based on the client id
        pnlCCPerformance.Visible = false;
        pnlIntervalAbandon.Visible = false;
        pnlIntervalAnswer.Visible = false;
        btnExportFull.Visible = false;
        pnlCallTimes.Visible = false;
        gvCallDispositions.DataBind();

        totalDispos.Text = "";
        UpdatePanel1.Update();
        #endregion Reset the Dashboard

    }
    #region Drop Down Lists
    protected void DropDown_Clients()
    {
        ListBox ddl = ddlCallClients;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                #region Build cmdText
                String cmdText = ghQueries.portal_ddl_clients(0);
                #endregion Build cmdText
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_clientid", "0"));
                if (Page.User.IsInRole("Client"))
                {
                    cmd.Parameters.Add("@sp_client", SqlDbType.VarChar, 100).Value = Session["clientname"].ToString();
                    lblClientCampaigns2.Text += "|1";
                }
                else if (Session["clientname"] != null && Session["clientname"].ToString().Length > 0)
                {
                    cmd.Parameters.Add("@sp_client", SqlDbType.VarChar, 100).Value = Session["clientname"].ToString();
                    lblClientCampaigns2.Text += "|2";
                }
                #endregion SQL Parameters
                #region SQL Processing - DropDown
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                ddl.DataTextField = "name";
                ddl.DataValueField = "clientid";

                ddl.Items.Clear();
                ddl.DataSource = dt;
                ddl.DataBind();

                #region Select Deafult or Session Client
                if (ddl.Items.Count > 0)
                {
                    if (Session["selected_client"] != null)
                    {
                        foreach (ListItem li in ddl.Items)
                        {
                            if (li.Value.Contains(Session["selected_client"].ToString())) li.Selected = true;
                        }
                    }
                    if (ddl.SelectedIndex == -1)
                    {
                        if (ddl.Items.Count > 1)
                        {
                            foreach (ListItem li in ddl.Items)
                            {
                                if (li.Text.Contains("Greenwood")) li.Selected = true;
                            }

                        }
                        else if (ddl.Items.Count > 0)
                        {
                            ddl.Items[0].Selected = true;
                        }
                    }
                }
                #endregion Select Deafult or Session Client
                #endregion SQL Processing - DropDown

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Client_Selected()
    {
        if (ddlCallClients.Items.Count > 0)
        {
            int clientid = 0;
            if (ddlCallClients.SelectedIndex != -1) clientid = Convert.ToInt32(ddlCallClients.SelectedValue);
            Session["selected_client"] = clientid;

            DropDown_Campaigns(clientid);
            DropDown_Skill(clientid);
            DropDown_Agent(clientid);

            lblClientCampaigns.Text = "Client: " + ddlCallClients.SelectedItem.Text;
            lblCampaignsCount.Text = ddlCallCampaigns.Items.Count.ToString();
            lblSkillsCount.Text = ddlCallSkills.Items.Count.ToString();
            lblAgentsCount.Text = ddlCallAgents.Items.Count.ToString();

            Dashboard_Reset();

        }
    }
    protected void DropDown_Campaigns(int clientid)
    {
        ListBox ddl = ddlCallCampaigns;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                #region Build cmdText
                String cmdText = ghQueries.portal_ddl_campaigns(clientid);
                #endregion Build cmdText
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_clientid", clientid));
                #endregion SQL Parameters
                #region SQL Processing - DropDown
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                ddl.DataTextField = "campaign";
                ddl.DataValueField = "campaignid";

                ddl.Items.Clear();
                ddl.DataSource = dt;
                ddl.DataBind();

                // Select all Items
                foreach (ListItem li in ddl.Items)
                {
                    if (clientid == 0)
                    {
                        if (li.Text.Contains("ARC ")) li.Selected = true;
                        if (li.Text.Contains("American Red ")) li.Selected = true;

                    }
                    else
                    {
                        li.Selected = true;
                    }
                }
                #endregion SQL Processing - DropDown

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void DropDown_Skill(int clientid)
    {
        ListBox ddl = ddlCallSkills;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                ghFunctions.Donation_Open_Database(con);
                cmd.CommandTimeout = 600;
                String cmdText = ghQueries.portal_ddl_skills(clientid);
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();

                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_clientid", clientid));
                #endregion SQL Parameters

                #region SQL Processing - DropDown
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                ddl.DataTextField = "skill";
                ddl.DataValueField = "skillid";

                ddl.Items.Clear();
                ddl.DataSource = dt;
                ddl.DataBind();

                // Select all Items
                //foreach (ListItem li in ddl.Items)
                //{
                //    if (li.Text.Contains("ARC ")) li.Selected = true;
                //    if (li.Text.Contains("American Red ")) li.Selected = true;
                //}
                #endregion SQL Processing - DropDown

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void DropDown_Agent(int clientid)
    {
        ListBox ddl = ddlCallAgents;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                ghFunctions.Donation_Open_Database(con);
                cmd.CommandTimeout = 600;
                String cmdText = ghQueries.portal_ddl_agents(clientid);
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_clientid", clientid));
                #endregion SQL Parameters

                #region SQL Processing - DropDown
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                ddl.DataTextField = "agent";
                ddl.DataValueField = "agentid";

                ddl.Items.Clear();
                ddl.DataSource = dt;
                ddl.DataBind();

                // Select all Items
                //foreach (ListItem li in ddl.Items)
                //{
                //    if (li.Text.Contains("ARC ")) li.Selected = true;
                //    if (li.Text.Contains("American Red ")) li.Selected = true;
                //}
                #endregion SQL Processing - DropDown

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    #endregion Drop Down Lists
    #region Export
    protected void Custom_Export_Excel_Dashboard(object sender, EventArgs e)
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
        String fileName = "Call-Statistics";
        XLWorkbook wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(fileName);
        // Starting Column and Row for Dashboard
        int sRow = 1; int sCol = 1; // A1
        #region Insert - Logo
        ws.Range(sRow, sCol, sRow + 3, sCol + 6).Merge();
        using (WebClient wc = new WebClient())
        {
            // Logo
            //byte[] bytes = wc.DownloadData(Server.MapPath("/images/gh.png"));
            byte[] bytes = wc.DownloadData(Server.MapPath("/images/GHNew.png"));
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
        var cr = ws.Range(sRow, sCol, sRow, sCol + 2);
        #region Date Range
        cr.Value = "Start Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 3, sRow, sCol + 3 + 2);
        cr.Value = dtStartDate.Text + " " + dtStartTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        sRow = sRow + 1;
        cr = ws.Range(sRow, sCol, sRow, sCol + 2);
        cr.Value = "End Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 3, sRow, sCol + 3 + 2);
        cr.Value = dtEndDate.Text + " " + dtEndTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        #endregion Date Range
        sRow = sRow + 2;
        #region Insert - Chart - Support Level
        ws.Cell(sRow, sCol).Value = "Support Level";
        ws.Cell(sRow, sCol).Style.Font.Bold = true;
        ws.Cell(sRow, sCol).Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 3).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        using (WebClient wc = new WebClient())
        {
            // Support Level Chart
            string iName = Session["imgNameDash"].ToString().Replace("[type]", "sl").Replace("[client]", ddlCallClients.SelectedValue.PadLeft(3, '0'));

            byte[] bytes = wc.DownloadData(Server.MapPath("/offline/charts/" + iName));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

                XLPicture pic = new XLPicture
                {
                    NoChangeAspect = true,
                    NoMove = true,
                    NoResize = true,
                    ImageStream = fIn,
                    Name = "Support Level"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow + 2 };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 4, RowId = sRow + 14 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        #endregion Insert - Chart - Support Level
        sCol = sCol + 6;
        #region Table Grids
        ws.Cell(sRow, sCol).Value = "Call Center Performance";
        ws.Range(sRow, sCol, sRow, sCol + 1).Merge();
        ws.Cell(sRow, sCol).Style.Font.Bold = true;
        ws.Cell(sRow, sCol).Style.Font.FontSize = 12;

        int tRow = 0;
        int maxRow = 0;
        #region Column
        tRow = sRow;
        tRow += 2;
        Cell_Format_01(ws, tRow, sCol, lblTotalCalls, cntTotalCalls, perTotalCalls);
        tRow += 2;
        Cell_Format_01(ws, tRow, sCol, lblTotalAnswered, cntTotalCalls, perTotalCalls);
        tRow += 2;
        Cell_Format_01(ws, tRow, sCol, lblTotalAbandoned, cntTotalAbandoned, perTotalAbandoned);

        tRow += 3;
        ws.Cell(tRow, sCol).Value = "Abandon Intervals";
        ws.Range(tRow, sCol, tRow, sCol + 1).Merge();
        ws.Cell(tRow, sCol).Style.Font.Bold = true;
        ws.Cell(tRow, sCol).Style.Font.FontSize = 12;
        tRow += 2;
        Cell_Format_01(ws, tRow, sCol, lblAbandoned90, cntAbandoned90, perAbandoned90);
        tRow += 2;
        Cell_Format_01(ws, tRow, sCol, lblAbandoned120, cntAbandoned120, perAbandoned120);
        tRow += 2;
        Cell_Format_01(ws, tRow, sCol, lblAbandoned120p, cntAbandoned120p, perAbandoned120p);

        tRow += 3;
        ws.Cell(tRow, sCol).Value = "Answer Intervals";
        ws.Range(tRow, sCol, tRow, sCol + 1).Merge();
        ws.Cell(tRow, sCol).Style.Font.Bold = true;
        ws.Cell(tRow, sCol).Style.Font.FontSize = 12;
        tRow += 2;
        Cell_Format_01(ws, tRow, sCol, lblAnswered90, cntAnswered90, perAnswered90);
        tRow += 2;
        Cell_Format_01(ws, tRow, sCol, lblAnswered120, cntAnswered120, perAnswered120);
        tRow += 2;
        Cell_Format_01(ws, tRow, sCol, lblAnswered120p, cntAnswered120p, perAnswered120p);
        #endregion Column
        maxRow = tRow;
        #region Column
        // Cell_Format_02(ws, sRow + 17, sCol, lblAbandonedTime, avgAbandonedTime);
        tRow = sRow;
        sCol = sCol + 3;
        ws.Cell(sRow, sCol).Value = "Call Times";
        ws.Range(sRow, sCol, sRow, sCol + 1).Merge();
        ws.Cell(sRow, sCol).Style.Font.Bold = true;
        ws.Cell(sRow, sCol).Style.Font.FontSize = 12;

        // protected void Cell_Format_03
        tRow += 2;
        Cell_Format_03(ws, tRow, sCol, lblSpeedAnswer, avgSpeedAnswer, totalSpeedAnswer);
        tRow += 2;
        Cell_Format_03(ws, tRow, sCol, lblTalkTime, avgTalkTime, totalTalkTime);
        tRow += 2;
        Cell_Format_03(ws, tRow, sCol, lblAbandonedTime, avgAbandonedTime, totalAbandonedTime);
        tRow += 2;
        Cell_Format_03(ws, tRow, sCol, lblAfterCallTime, avgAfterCallTime, totalAfterCallTime);
        tRow += 2;
        Cell_Format_03(ws, tRow, sCol, lblQueueTime, avgQueueTime, totalQueueTime);
        tRow += 2;
        Cell_Format_03(ws, tRow, sCol, lblHandleTime, avgHandleTime, totalHandleTime);
        tRow += 2;
        Cell_Format_03(ws, tRow, sCol, lblIVRTime, avgIVRTime, totalIVRTime);
        tRow += 2;
        Cell_Format_03(ws, tRow, sCol, lblThirdPartyTime, avgThirdPartyTime, totalThirdPartyTime);

        #endregion Column
        if (tRow > maxRow) maxRow = tRow;
        #endregion Table Grids
        sCol = sCol + 3;
        #region Grid - Call Dispositions
        ws.Cell(sRow, sCol).Value = "Call Disposition Detail";
        ws.Range(sRow, sCol, sRow, sCol + 2).Merge();
        ws.Cell(sRow, sCol).Style.Font.Bold = true;
        ws.Cell(sRow, sCol).Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
        GridView gv = gvCallDispositions;
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
                    string cntrls = "";
                    foreach (Control c in gvRow.Cells[i].Controls)
                    {
                        if (c.GetType() == typeof(Label))
                        {
                            cntrls = ((Label)c).Text;
                        }
                    }
                    var num = decimal.Parse(cntrls.TrimEnd(new char[] { '%', ' ' })) / 100M;
                    cl.Value = num;
                    cl.Style.NumberFormat.Format = "0%";
                }
                else
                {
                    cl.Value = gvRow.Cells[i].Text;
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
        if (dRow > maxRow) maxRow = dRow;
        maxRow += 3;
        #region Wrap Up - Save/Download the File
        // Center Column: 7, 8, 9, 11
        ws.Range(1, 7, maxRow, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        // ws.Columns(7, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        // ws.Columns(10, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        //ws.Cell(sRow + 1, sCol + 3).Value = "Done";

        ws.Rows().AdjustToContents();
        //ws.Columns().AdjustToContents();
        #region Custom Width
        ws.Column(1).Width = 10;
        ws.Column(2).Width = 7.25;
        ws.Column(3).Width = 4.25;
        ws.Column(4).Width = 18.5;

        ws.Columns(7, 8).Width = 18;
        ws.Columns(10, 11).Width = 18;

        // ws.Column(13).Width = 20;
        // ws.Column(14).Width = 30;
        ws.Columns(13, 15).AdjustToContents();
        if (ws.Column(13).Width < 30) ws.Column(13).Width = 30;
        #endregion Custom Width

        ws.ShowGridLines = false;
        if (ddlCallClients.SelectedIndex != -1)
        {
            fileName = ddlCallClients.SelectedItem.Text + "-" + fileName + "";
        }
        else
        {
            fileName = "Dashboard-Reporting-" + fileName;
        }
        //fileName = fileName.Replace(" ", "_");
        fileName = ghFunctions.GetSafeFilename(fileName);

        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}{1}.xlsx", fileName, DateTime.Now.ToString("-yyyyMMdd-HHmmss")));

        using (MemoryStream memoryStream = new MemoryStream())
        {
            wb.SaveAs(memoryStream);
            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
            memoryStream.Close();
        }

        HttpContext.Current.Response.End();
        #endregion Wrap Up - Save/Download the File
    }
    protected void GridView_Export_Excel(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        GridView gv = null;
        String lbl = "";
        if (btn.ID == "btnCallDispositions")
        {
            gv = this.gvCallDispositions;
            lbl = "Call-Disposition-Details";
        }

        if (gv != null)
        {
            rpMessage.Text = "Exporting [" + lbl + "] Grid";
            GridViewExportUtil.ClosedXMLExport("" + lbl + "", gv);
        }
        else
        {
            rpMessage.Text = "Un-configured export click.";
        }
    }
    protected void GridView_Export_Excel_Old(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        GridView gv = null;
        String lbl = "";
        if (btn.ID == "btnCallDispositions")
        {
            gv = this.gvCallDispositions;
            lbl = "Call-Disposition-Details";
        }

        if (gv != null)
        {
            rpMessage.Text = "Exporting [" + lbl + "] Grid";
            GridViewExportUtil.Export("Dashboard-Reporting-" + lbl + ".xls", gv);
        }
        else
        {
            rpMessage.Text = "Un-configured export click.";
        }
    }
    protected void Panel_Export_Excel(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        Panel pl = null;
        String lbl = "";
        if (btn.ID == "btnCCPerformance")
        {
            pl = this.pnlCCPerformanceExport;
            lbl = "Call-Center-Performance";
        }
        else if (btn.ID == "btnInterval")
        {
            pl = this.pnlIntervalExport;
            lbl = "Call-Interval";
        }

        if (pl != null)
        {
            rpMessage.Text = "Exporting [" + lbl + "] Grid";
            GridViewExportUtil.ExportPanel("Dashboard-Reporting-" + lbl + ".xls", pl);
        }
        else
        {
            rpMessage.Text = "Un-configured export click.";
        }
    }
    protected void Cell_Format_01(IXLWorksheet ws, Int32 row, Int32 col, Label lbl, Label lbl2, Label lbl3)
    {
        // ws.Cell(row, col)
        //ws.Cell(row, col).Value = lbl.Text;
        //ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //ws.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        //ws.Cell(row, col + 1).Value = "%";
        //ws.Cell(row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //ws.Cell(row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        //ws.Range(row, col, row, col + 1).Style.Font.Bold = true;
        //ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Cell(row, col).Value = lbl.Text;
        ws.Range(row, col, row, col + 1).Merge();
        ws.Range(row, col, row, col + 1).Style.Font.Bold = true;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;

        row++;
        ws.Cell(row, col).Value = lbl2.Text;
        ws.Cell(row, col).Style.NumberFormat.Format = "#,##0";
        ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        var num = decimal.Parse(lbl3.Text.TrimEnd(new char[] { '%', ' ' })) / 100M;
        ws.Cell(row, col + 1).Value = num;
        ws.Cell(row, col + 1).Style.NumberFormat.Format = "0%";
        ws.Cell(row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;

    }
    protected void Cell_Format_02(IXLWorksheet ws, Int32 row, Int32 col, Label lbl, Label lbl2)
    {
        // ws.Cell(row, col)
        ws.Cell(row, col).Value = lbl.Text;
        ws.Range(row, col, row, col + 1).Merge();
        ws.Range(row, col, row, col + 1).Style.Font.Bold = true;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;

        row++;
        ws.Cell(row, col).Value = lbl2.Text;
        ws.Range(row, col, row, col + 1).Merge();
        ws.Range(row, col, row, col + 1).Style.NumberFormat.Format = "HH:mm:ss";
        ws.Range(row, col, row, col + 1).Style.Font.Bold = true;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;
    }
    protected void Cell_Format_03(IXLWorksheet ws, Int32 row, Int32 col, Label lbl, Label lbl2, Label lbl3)
    {
        // lbl == Label
        // lbl2 == 
        ws.Cell(row, col).Value = lbl.Text;
        ws.Range(row, col, row, col + 1).Merge();
        ws.Range(row, col, row, col + 1).Style.Font.Bold = true;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;

        row++;
        ws.Cell(row, col).Value = lbl2.Text + " avg";
        ws.Cell(row, col).RichText.Substring(lbl2.Text.Length, 4).SetFontColor(XLColor.Blue);

        // ws.Cell(row, col).Style.NumberFormat.Format = "#,##0";
        ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.DarkGray;

        // var num = decimal.Parse(lbl3.Text.TrimEnd(new char[] { '%', ' ' })) / 100M;
        ws.Cell(row, col + 1).Value = lbl3.Text + " total";
        ws.Cell(row, col + 1).RichText.Substring(lbl3.Text.Length, 6).SetFontColor(XLColor.Orange);

        //ws.Cell(row, col + 1).Style.NumberFormat.Format = "0%";
        ws.Cell(row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;

    }
    #endregion Export
    protected void GoTo_Other_Dashboard(object sender, EventArgs e)
    {
        Response.Redirect("~/Dashboard_Calls.aspx");
        //or
        //Server.Transfer("~/Dashboard.aspx");
    }

    /// <summary>
    /// This functions should be put into their own file
    /// 
    /// </summary>
    protected Double GetPercent(double left, double right)
    {
        double rtrn = 0;
        if (left > 0 && right > 0)
        {
            rtrn = left / right;
        }
        return rtrn;
    }
    protected String dispo_percent(String count)
    {
        double cnt = Convert.ToInt32(count);
        double total = Convert.ToInt32(hfDispoCount.Value);

        return String.Format("{0:P2}", GetPercent(cnt,total));
    }
    protected String campaign_percent(String count)
    {
        double cnt = Convert.ToInt32(count);
        double total = Convert.ToInt32(hfCampaignCount.Value);

        return String.Format("{0:P2}", GetPercent(cnt, total));
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

        //ErrorLog.ErrorLog_Save(ex, dv, "Ameriprise Admin Portal", error, spPage, spQS, spURL);
    }
    public DateTime dtConverted(DateTime dt)
    {
        // Convert the supplied DT back to UTC based on PST
        Int32 dtOffSet = 7;
        DateTime dtCurrent = DateTime.Now;
        System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
        if (localZone.IsDaylightSavingTime(dtCurrent))
        {
            dtOffSet = 7;
        }
        else
        {
            dtOffSet = 8;
        }
        dtOffSet = 0;
        return dt.AddHours(dtOffSet);
    }
    protected void print_sql(SqlCommand cmd)
    {
        print_sql(cmd, sqlPrint, "append");
    }
    protected void print_sql(SqlCommand cmd, String type)
    {
        //ghFunctions.print_sql(cmd, sqlPrint, type);
        print_sql(cmd, sqlPrint, type);
    }
    protected void print_sql(SqlCommand cmd, Label lblPrint, String type)
    {
        #region Print SQL
        // 
        // Connection.GetConnectionType() == "Local"
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
        #endregion Print SQL
    }
    protected DataTable dt_Client_Campaigns()
    {
        // We create a table variable
        // To pass the campaigns selected
        // Need to see if this breaks with large selection

        DataTable clientCampaigns = new DataTable();// = CategoriesDataTable.GetChanges(DataRowState.Added);
        clientCampaigns.Columns.Add(new DataColumn("campaignid", typeof(Int64)));
        DataRow dr;
        foreach (ListItem li in ddlCallCampaigns.Items)
        {
            if (li.Selected)
            {
                dr = clientCampaigns.NewRow(); dr["campaignid"] = li.Value; clientCampaigns.Rows.Add(dr);
            }
        }
        return clientCampaigns;
    }
    protected DataTable dt_Item_Campaigns()
    {
        // Create a table variable
        // To pass the campaigns selected
        // Need to see if this breaks with large selection

        //StringBuilder sbSkills = new StringBuilder();
        //foreach (ListItem li in ddlCallSkills.Items) { if (li.Selected) { sbSkills.Append("," + li.Value); } }

        DataTable clientCampaigns = new DataTable();// = CategoriesDataTable.GetChanges(DataRowState.Added);
        clientCampaigns.Columns.Add(new DataColumn("itemid", typeof(Int64)));
        DataRow dr;
        foreach (ListItem li in ddlCallCampaigns.Items)
        {
            if (li.Selected)
            {
                dr = clientCampaigns.NewRow(); dr["itemid"] = li.Value; clientCampaigns.Rows.Add(dr);
            }
        }
        return clientCampaigns;
    }
    protected DataTable dt_Item_Skills()
    {
        // Create a table variable
        DataTable clientCampaigns = new DataTable();// = CategoriesDataTable.GetChanges(DataRowState.Added);
        clientCampaigns.Columns.Add(new DataColumn("itemid", typeof(Int64)));
        DataRow dr;
        foreach (ListItem li in ddlCallSkills.Items)
        {
            if (li.Selected)
            {
                dr = clientCampaigns.NewRow(); dr["itemid"] = li.Value; clientCampaigns.Rows.Add(dr);
            }
        }
        return clientCampaigns;
    }
    protected DataTable dt_Item_Agents()
    {
        // Create a table variable
        DataTable clientCampaigns = new DataTable();// = CategoriesDataTable.GetChanges(DataRowState.Added);
        clientCampaigns.Columns.Add(new DataColumn("itemid", typeof(Int64)));
        DataRow dr;
        foreach (ListItem li in ddlCallAgents.Items)
        {
            if (li.Selected)
            {
                dr = clientCampaigns.NewRow(); dr["itemid"] = li.Value; clientCampaigns.Rows.Add(dr);
            }
        }
        return clientCampaigns;
    }

    public DateTime dtOffSetAdd(DateTime dt)
    {
        // Convert based on Users Offset
        Int32 dtOffSet = ghFunctions.dtUserOffSet;
        return dt.AddHours(dtOffSet);
    }

}