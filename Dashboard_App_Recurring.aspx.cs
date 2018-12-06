using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChartDirector;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using System.IO;
using System.Net;
public partial class Dashboard_App_Recurring : System.Web.UI.Page
{
    //private String sqlStr = Connection.GetConnectionString("MiddleWare", "");
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    private String sqlStrPortal = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    private Int32 countCalls = 0;
    private String strClient = "ARC"; //ddlCallClients.SelectedValue.PadLeft(3, '0')
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Dashboard IVR";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);

        if (Connection.GetDBMode() == "Stage")
        {
            sqlStrPortal = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        // If we lose session, it can cause errors.
        if (Session["userid"] == null)
        {
            identity_get_userid();
            Response.Redirect("~/Dashboard_App_IVR.aspx");
        }
        if (!IsPostBack)
        {
            #region Chart Image
            if (Session["imgNameSel"] == null || !IsPostBack)
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
                imgName = String.Format("{0}_{1}_{2}_[client]_dashboard_app_ivr_[type].png", uName, uID, aName);
                Session["imgNameSel"] = imgName;
            }
            #endregion Chart Image

            if (dtStartDate.Text == "") dtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            if (dtEndDate.Text == "") dtEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            if (dtStartTime.Text == "") dtStartTime.Text = "00:00";
            if (dtEndTime.Text == "") dtEndTime.Text = "23:59";


            GridView_Refresh();
        }
        Service_Status();

    }
    protected void Service_Status()
    {
        #region Service Status
        int serviceid = 10000003; // Stage 10000003, Live 10000004
        if (Connection.GetDBMode() == "Live") serviceid = 10000004;
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrPortal))
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
                                TOP 1
                                [name]
                                ,[status]
                                ,[message]
                                ,[datetimestamp]
                                FROM [dbo].[app_service_status] WITH(NOLOCK)
                                WHERE [serviceid] = @sp_serviceid
                            ";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add(new SqlParameter("@sp_serviceid", serviceid));
                    #endregion SQL Parameters

                    #region SQL Processing - Reader
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            #region Read the Service Status
                            while (sqlRdr.Read())
                            {
                                lblServiceName.Text = sqlRdr["name"].ToString();
                                if (sqlRdr["status"].ToString() == "10100") // Runnin
                                {
                                    service_panel.BackColor = System.Drawing.Color.LawnGreen; lblServiceStatus.Text = "RUNNING";

                                }
                                else if (sqlRdr["status"].ToString() == "10200") // Error but not stopped
                                {
                                    service_panel.BackColor = System.Drawing.Color.Yellow; lblServiceStatus.Text = "ERROR-RUNNING";
                                }
                                else if (sqlRdr["status"].ToString() == "10300") // Stopped
                                {
                                    service_panel.BackColor = System.Drawing.Color.Red; lblServiceStatus.Text = "STOPPED";
                                }
                                else
                                {
                                    service_panel.BackColor = System.Drawing.Color.Yellow; // Error but not stopped
                                    lblServiceStatus.Text = String.Format("ERROR-[{0}]", sqlRdr["status"].ToString());
                                }
                                //lblServiceDate.Text = sqlRdr["datetimestamp"].ToString();// DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                lblServiceDate.Text = DateTime.Parse(sqlRdr["datetimestamp"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff");
                                lblServiceMsg.Text = sqlRdr["message"].ToString();
                            }
                            #endregion Read the Service Status
                        }
                        else
                        {
                            lblServiceName.Text = "Unknown";
                            service_panel.BackColor = System.Drawing.Color.Red; // Error but not stopped
                            lblServiceStatus.Text = "Unknown";
                            lblServiceDate.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            lblServiceMsg.Text = "Configuration issue; no service was detected.";
                        }
                    }
                    #endregion SQL Processing - Reader

                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Display(ex, "Service_Status", msgLabel);
        }
        #endregion Service Status
    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {

        GridView_Refresh();
    }
    protected void GridView_Refresh()
    {

        try
        {
            Service_Status();
            UpdatePanel3.Update();
            Data_Counts();
            Data_File_Details();
            Data_File_Counts();
            Data_Record_Details();
            Data_Record_Status();
        }
        catch (Exception ex)
        {
            Error_Display(ex, "GridView_Refresh", msgLabel);
        }
        btnExportFull.Visible = true;
    }
    protected void Data_Counts()
    {
        GridView gv = gvCallDispositions;
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
                cmdText = "";
                cmdText += @"
                            SELECT
                            'files' [type]
                            ,(SELECT COUNT([filename]) FROM [dbo].[ivr_file] WHERE [start] BETWEEN @sp_date_start AND @sp_date_end) [count]
                            UNION
                            SELECT
                            'records' [type]
                            ,(SELECT SUM([records]) FROM [dbo].[ivr_file] WHERE [start] BETWEEN @sp_date_start AND @sp_date_end) [count]
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                
                #endregion SQL Parameters
                //print_sql(cmd); // Will print for Admin in Local
                #region SQL Processing - Reader
                DBTable chartTable = new DBTable(cmd.ExecuteReader());
                Data_Counts_Chart(chartTable);
                #endregion SQL Processing - Reader

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Data_Counts_Chart(DBTable table)
    {
        // The data for the pie chart
        //double[] data = { 89, 18, 15, 12, 8, 54, 35 };
        double[] data = table.getCol(1);

        // The labels for the pie chart
        //string[] labels = { "Labor", "Licenses", "Taxes", "Legal", "Insurance", "Facilities", "Production" };
        string[] labels = table.getColAsString(0);

        // Create a PieChart object of size 450 x 270 pixels
        PieChart c = new PieChart(504, 260);
        // Transparent Background
        c.setBackground(Chart.Transparent);
        //c.setBackground(0xcccccc); 

        // Set the center of the pie at (150, 100) and the radius to 80 pixels
        c.setPieSize(155, 130, 120);

        // add a legend box where the top left corner is at (330, 50)
        c.addLegend(315, 10).setText("{label}: {value} ({percent|0}%)");

        // modify the sector label format to show percentages only
        //c.setLabelFormat("{percent}%");
        // Disable the sector labels by setting the color to Transparent
        c.setLabelStyle("", 8, Chart.Transparent);

        // Set the pie data and the pie labels
        c.setData(data, labels);

        // Use rounded edge shading, with a 1 pixel white (FFFFFF) border
        c.setSectorStyle(Chart.RoundedEdgeShading, 0xffffff, 1);

        // Output the chart
        chartCallType.Image = c.makeWebImage(Chart.PNG);
        // Include tool tip for the chart
        chartCallType.ImageMap = c.getHTMLImageMap("", "", "title='{label}: {value} Calls ({percent|0}%)'");

        string iName = Session["imgNameSel"].ToString().Replace("[type]", "ct").Replace("[client]", strClient);
        c.makeChart(Server.MapPath("/offline/charts/" + iName));
    }
    protected void Data_File_Details()
    {
        try
        {
            GridView gv = gvCampaignCount;
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
                    cmdText = "";
                    cmdText += @"
                            SELECT
                            [filename]
                            ,SUM([processed]) [processed]
                            ,SUM([duplicate]) [duplicate]
                            ,SUM([invalid]) [invalid]
                            ,SUM([corrupted]) [corrupted]
                            ,SUM([error]) [error]
                            ,SUM([errornet]) [errornet]
                            ,SUM([other]) [other]
                            FROM [dbo].[ivr_file]
                            WHERE 1=1
                            AND [start] BETWEEN @sp_date_start AND @sp_date_end
                            AND [filename] LIKE CASE WHEN @sp_filename IS NULL THEN [filename] ELSE '%' + @sp_filename + '%' END
                            GROUP BY [filename]
                            ";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    int sp_top = 50000;
                    cmd.Parameters.Add(new SqlParameter("@sp_top", sp_top));
                    DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                    DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                    lblCallDetails.Text = String.Format("From: {0} To: {1}", dtFrom, dtTo);
                    cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                    cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                    //if (tbFileName.Text.Trim().Length > 0)
                    //{
                    //    cmd.Parameters.Add(new SqlParameter("@sp_filename", tbFileName.Text.Trim()));
                    //}
                    //else
                    //{
                        cmd.Parameters.Add(new SqlParameter("@sp_filename", ""));
                    //}
                    #endregion SQL Parameters

                    #region SQL Processing - GridView
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    int dCount = dt.Rows.Count;
                    gv.DataSource = dt;
                    gv.DataBind();

                    #endregion SQL Processing - GridView

                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Display(ex, "GridView_Refresh", msgLabel);
        }
    }
    protected void Data_Record_Status()
    {
        try
        {
            GridView gv = gvRecordStatus;
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
                    cmdText = "";
                    cmdText += @"
SELECT
MIN([r].[calldate]) [since]
,COUNT([r].[sourceid]) [total]
,COUNT (CASE WHEN [r].[callid] IS NOT NULL THEN [r].[sourceid] ELSE NULL END) [completed]
,COUNT (CASE WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NOT NULL AND ([r].[file_vc] IS NOT NULL OR [r].[status] = 99) THEN [r].[sourceid] ELSE NULL END) [pending]
,COUNT (CASE
			WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NULL AND [r].[status] NOT IN (98) THEN [r].[sourceid]
			WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NOT NULL AND [r].[status] NOT IN (98) AND ([r].[file_vc] IS NULL AND [r].[status] NOT IN (99)) THEN [r].[sourceid]
		ELSE NULL END) [invalid]
,COUNT (CASE WHEN [r].[status] IN (99) THEN [r].[sourceid] ELSE NULL END) [cleared]
,COUNT (CASE WHEN [r].[status] IN (98) THEN [r].[sourceid] ELSE NULL END) [discarded]

,COUNT (CASE WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NOT NULL AND [r].[file_vc] IS NOT NULL THEN [r].[sourceid] ELSE NULL END) [cc_pending]
,ISNULL(SUM(CASE WHEN [r].[callid] IS NULL AND [r].[file_cc] IS NOT NULL AND [r].[file_vc] IS NOT NULL THEN [cc].[amount] ELSE NULL END),'0.00') [cc_amount]
,COUNT (CASE WHEN [r].[file_vc] IS NOT NULL THEN [r].[sourceid] ELSE NULL END) [file_vc]
,COUNT (CASE WHEN [r].[file_cc] IS NOT NULL THEN [r].[sourceid] ELSE NULL END) [file_cc]
,COUNT (CASE WHEN [r].[file_rani] IS NOT NULL THEN [r].[sourceid] ELSE NULL END) [file_rani]
,COUNT (CASE WHEN [r].[file_opt] IS NOT NULL THEN [r].[sourceid] ELSE NULL END) [file_opt]
,COUNT (CASE WHEN [r].[file_cat] IS NOT NULL THEN [r].[sourceid] ELSE NULL END) [file_cat]
FROM [dbo].[ivr_record] [r] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[ivr_record_cc] [cc] WITH(NOLOCK) ON [cc].[sourceid] = [r].[sourceid] AND [cc].[recordid] = [r].[recordid] AND [cc].[calldate] = [r].[calldate] AND [cc].[calltime] = [r].[calltime] AND [cc].[ani] = [r].[ani]
WHERE 1=1
--AND [r].[calldate] >= CONVERT(varchar,@sp_date_start,112)
AND (
[r].[started] BETWEEN @sp_date_start AND @sp_date_end
OR [r].[completed] BETWEEN @sp_date_start AND @sp_date_end
OR [r].[file_vc] BETWEEN @sp_date_start AND @sp_date_end
OR [r].[file_cc] BETWEEN @sp_date_start AND @sp_date_end
OR [r].[file_rani] BETWEEN @sp_date_start AND @sp_date_end
OR [r].[file_opt] BETWEEN @sp_date_start AND @sp_date_end
OR [r].[file_cat] BETWEEN @sp_date_start AND @sp_date_end
)
                            ";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    int sp_top = 50000;
                    cmd.Parameters.Add(new SqlParameter("@sp_top", sp_top));
                    DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                    DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                    lblCallDetails.Text = String.Format("From: {0} To: {1}", dtFrom, dtTo);
                    cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                    cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                    #endregion SQL Parameters

                    #region SQL Processing - GridView
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    int dCount = dt.Rows.Count;
                    gv.DataSource = dt;
                    gv.DataBind();

                    #endregion SQL Processing - GridView

                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Display(ex, "GridView_Refresh", msgLabel);
        }
    }
    
    protected void Data_File_Counts()
    {
        GridView gv = gvCallDispositions;
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
                cmdText = "";
                cmdText += @"
SELECT
'records' [type]
,(SELECT COUNT([recordid]) FROM [dbo].[ivr_record] WHERE ([started] BETWEEN @sp_date_start AND @sp_date_end OR [completed] BETWEEN @sp_date_start AND @sp_date_end)) [count]
UNION
SELECT
'file_vc' [type]
,(SELECT COUNT([file_vc]) FROM [dbo].[ivr_record] WHERE [file_vc] BETWEEN @sp_date_start AND @sp_date_end) [count]
UNION
SELECT
'file_cc' [type]
,(SELECT COUNT([file_cc]) FROM [dbo].[ivr_record] WHERE [file_cc] BETWEEN @sp_date_start AND @sp_date_end) [count]
UNION
SELECT
'file_rani' [type]
,(SELECT COUNT([file_rani]) FROM [dbo].[ivr_record] WHERE [file_rani] BETWEEN @sp_date_start AND @sp_date_end) [count]
UNION
SELECT
'file_opt' [type]
,(SELECT COUNT([file_opt]) FROM [dbo].[ivr_record] WHERE [file_opt] BETWEEN @sp_date_start AND @sp_date_end) [count]
UNION
SELECT
'file_cat' [type]
,(SELECT COUNT([file_cat]) FROM [dbo].[ivr_record] WHERE [file_cat] BETWEEN @sp_date_start AND @sp_date_end) [count]
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                #endregion SQL Parameters
                //print_sql(cmd); // Will print for Admin in Local
                #region SQL Processing - Reader
                DBTable chartTable = new DBTable(cmd.ExecuteReader());
                Data_File_Counts_Chart(chartTable);
                #endregion SQL Processing - Reader

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Data_File_Counts_Chart(DBTable table)
    {
        // The labels (Designation) Element
        string[] labels = table.getColAsString(0);
        // The Count Data Element
        double[] data01 = table.getCol(1);

        #region Bar Labels
        // Create a XYChart object of size 700 x 255 pixels
        XYChart c = new XYChart(700, 255);
        // Transparent Background
        c.setBackground(Chart.Transparent);

        // Set default text color to dark grey (0x333333)
        c.setColor(Chart.TextColor, 0x333333);

        // Set the plotarea at (70, 20) and of size 500 x 300 pixels, with transparent background and
        // border and light grey (0xcccccc) horizontal grid lines
        //c.setPlotArea(60, 20, 620, 200, Chart.Transparent, -1, -1, 0xcccccc, 0xcccccc);
        //c.setPlotArea(60, 20, 620, 200, Chart.Transparent, -1, Chart.Transparent, 0xcccccc);
        int pArea = 590;
        c.setPlotArea(50, 30, pArea, 180, Chart.Transparent, -1, Chart.Transparent, 0xcccccc);

        // Set the x and y axis stems to transparent and the label font to 12pt Arial
        c.xAxis().setColors(Chart.Transparent);
        c.xAxis().setLabelStyle("Arial", 10).setMaxWidth(pArea / labels.Length);
        c.yAxis().setColors(Chart.Transparent);
        c.yAxis().setLabelStyle("Arial", 10);

        #region Basic Single Bar
        // Add a blue (0x6699bb) bar chart layer using the given data
        BarLayer layer = c.addBarLayer(data01, 0x6699bb);
        // Use bar gradient lighting with the light intensity from 0.8 to 1.3
        layer.setBorderColor(Chart.Transparent, Chart.barLighting(0.8, 1.3));
        // Set rounded corners for bars
        layer.setRoundedCorners();
        // Display labela on top of bars using 12pt Arial font
        layer.setAggregateLabelStyle("Arial", 10);
        layer.setAggregateLabelFormat("{value|0,}");
        #endregion


        // Set the labels on the x axis.
        c.xAxis().setLabels(labels);

        // For the automatic y-axis labels, set the minimum spacing to 40 pixels.
        c.yAxis().setTickDensity(40);

        // Add a title to the y axis using dark grey (0x555555) 14pt Arial Bold font
        c.yAxis().setTitle("Count", "Arial Bold", 12, 0x555555);
        c.yAxis().setTitlePos(Chart.Top);
        double data01Max = 0;
        foreach (double val in data01) { if (val > data01Max) data01Max = val; if (data01Max > 1000) break; }
        if (data01Max > 1000) c.yAxis().setLabelFormat("{={value}/1000|0}K");


        //.setText("{={value}/1000|0}K");

        #endregion Bar Labels

        // Output the chart
        chartDesignationCount.Image = c.makeWebImage(Chart.PNG);
        // Include tool tip for the chart
        chartDesignationCount.ImageMap = c.getHTMLImageMap("", "", "title='Designation {xLabel}: {value}'");

        string iName = Session["imgNameSel"].ToString().Replace("[type]", "dc").Replace("[client]", strClient);
        c.makeChart(Server.MapPath("/offline/charts/" + iName));


    }
    protected void Data_Record_Details()
    {
        GridView gv = gvCallDetails;
        GridView gvEx = gvCallDetailsExport;
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
                cmdText = "";
                cmdText += @"
                            SELECT
                            TOP (@sp_top)
                            [recordid]
                            ,[calldate]
                            ,[calltime]
                            ,[ani]
                            ,[status]
                            ,[callid]
                            ,[donationccinfoid]
                            ,[file_vc]
                            ,[file_cc]
                            ,[file_rani]
                            ,[file_opt]
                            ,[file_cat]
                            ,[started]
                            ,[completed]
                            FROM [dbo].[ivr_record]
                            WHERE 1=1
                            AND (
                            [started] BETWEEN @sp_date_start AND @sp_date_end
                            OR [completed] BETWEEN @sp_date_start AND @sp_date_end
                            OR [file_vc] BETWEEN @sp_date_start AND @sp_date_end
                            OR [file_cc] BETWEEN @sp_date_start AND @sp_date_end
                            OR [file_rani] BETWEEN @sp_date_start AND @sp_date_end
                            OR [file_opt] BETWEEN @sp_date_start AND @sp_date_end
                            OR [file_cat] BETWEEN @sp_date_start AND @sp_date_end
                            )
                            ORDER BY [recordid], [calldate], [calltime], [ani]
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                int sp_top = 50000;
                cmd.Parameters.Add(new SqlParameter("@sp_top", sp_top));
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                lblCallDetails.Text = String.Format("From: {0} To: {1}", dtFrom, dtTo);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                #endregion SQL Parameters

                #region SQL Processing - GridView
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                int dCount = dt.Rows.Count;
                gv.DataSource = dt;
                gv.DataBind();
                if (dCount > 0)
                {
                    btnCallDetails.Visible = true;
                    if (gv.Rows.Count != gvEx.Rows.Count)
                    {
                        gvEx.DataSource = dt;
                        gvEx.DataBind();
                    }
                }
                #endregion SQL Processing - GridView

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GridView_DataBound(Object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.PageCount > 0)
        {
            // Retrieve the pager row.
            GridViewRow pagerRow = gv.TopPagerRow;
            // Retrieve the DropDownList and Label controls from the row.
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
            if (pageList != null)
            {
                // Create the values for the DropDownList control based on 
                // the  total number of pages required to display the data
                // source.
                for (int i = 0; i < gv.PageCount; i++)
                {
                    // Create a ListItem object to represent a page.
                    int pageNumber = i + 1;
                    ListItem item = new ListItem(pageNumber.ToString());
                    // If the ListItem object matches the currently selected
                    // page, flag the ListItem object as being selected. Because
                    // the DropDownList control is recreated each time the pager
                    // row gets created, this will persist the selected item in
                    // the DropDownList control.   
                    if (i == gv.PageIndex)
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
                int currentPage = gv.PageIndex + 1;
                // Update the Label control with the current page information.
                pageLabel.Text = "Page " + currentPage.ToString() +
                  " of " + gv.PageCount.ToString();
            }
            if (gv.PageIndex > 0)
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = true;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = true;
            }
            else
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = false;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = false;
            }

            if (gv.PageCount != (gv.PageIndex + 1))
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
    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv != null)
        {
            gv.SelectedIndex = -1;
            gv.PageIndex = e.NewPageIndex;
            //GridView_Data(0, this.Page.User.Identity.Name, gvResults);
            //donation_admin.Visible = false;
            Data_Record_Details();
        }
    }
    protected void GridView_PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
    {
        // Work with the Page Template Drop Down List
        DropDownList ddl = (DropDownList)sender;
        GridView gv = (GridView)ddl.GetParentOfType(typeof(GridView)); //gvCallDetails;
        //lblCallDetails.Text = "DDL: " + ddl.ID + "| GV: " + gv.ID; // DeBug to show the ID of the DropDownList and GridView
        if (gv != null)
        {
            // Retrieve the pager row.
            GridViewRow pagerRow = gv.TopPagerRow;
            // Retrieve the PageDropDownList DropDownList from the bottom pager row.
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            // Set the PageIndex property to display that page selected by the user.
            gv.SelectedIndex = -1;
            gv.PageIndex = pageList.SelectedIndex;
            //GridView_Data(0, this.Page.User.Identity.Name, gvResults);
            Data_Record_Details();
        }
    }

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
        ws.Range(sRow, sCol, sRow + 3, sCol + 3).Merge();
        using (WebClient wc = new WebClient())
        {
            // Logo
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
        #region Insert - Chart - Counts
        ws.Cell(sRow, sCol).Value = "Counts";
        ws.Cell(sRow, sCol).Style.Font.Bold = true;
        ws.Cell(sRow, sCol).Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 8).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        using (WebClient wc = new WebClient())
        {
            // Counts Chart
            string iNameCH = Session["imgNameSel"].ToString().Replace("[type]", "ct").Replace("[client]", strClient);

            byte[] bytes = wc.DownloadData(Server.MapPath("/offline/charts/" + iNameCH));
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
                    Name = "Counts"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow + 2 };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 7, RowId = sRow + 15 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        #endregion Insert - Chart - Counts
        int dRow = sRow; int dCol = sCol + 10; int dColT = dCol;
        GridView gv;
        #region Grid - Record Status
        ws.Cell(dRow, dCol).Value = "Record Status";
        ws.Range(dRow, dCol, dRow, dCol + 2).Merge();
        ws.Cell(dRow, dCol).Style.Font.Bold = true;
        ws.Cell(dRow, dCol).Style.Font.FontSize = 12;
        ws.Range(dRow, dCol, dRow, dCol + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        //int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
        dRow = dRow + 2; dCol = sCol + 10; dColT = dCol;
        gv = gvRecordStatus;
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
        #endregion Grid - Record Status
        dRow = dRow + 2;
        #region Grid - File Details
        ws.Cell(dRow, dCol).Value = "File Detail";
        ws.Range(dRow, dCol, dRow, dCol + 2).Merge();
        ws.Cell(dRow, dCol).Style.Font.Bold = true;
        ws.Cell(dRow, dCol).Style.Font.FontSize = 12;
        ws.Range(dRow, dCol, dRow, dCol + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        //int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
        dRow = dRow + 2; dCol = sCol + 10; dColT = dCol;
        gv = gvCampaignCount;
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
        #endregion Grid - File Details
        sRow = sRow + 16;
        #region Insert - Chart - File Counts
        ws.Cell(sRow, sCol).Value = "File Counts";
        ws.Cell(sRow, sCol).Style.Font.Bold = true;
        ws.Cell(sRow, sCol).Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 8).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        using (WebClient wc = new WebClient())
        {
            // File Counts
            string iNameAH = Session["imgNameSel"].ToString().Replace("[type]", "dc").Replace("[client]", strClient);

            byte[] bytes = wc.DownloadData(Server.MapPath("/offline/charts/" + iNameAH));
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
                    Name = "File Counts"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow + 2 };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 9, RowId = sRow + 15 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        #endregion Insert - Chart - File Counts
        sRow = sRow + 16;
        //
        #region Wrap Up - Save/Download the File
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        //ws.Cell(sRow + 1, sCol + 3).Value = "Done";

        ws.Rows().AdjustToContents();
        ws.Columns().AdjustToContents();
        ws.Columns(1, 4).Width = 10;
        ws.Columns(5, 9).Width = 11;
        ws.Columns(11, 14).Width = 18;
        ws.ShowGridLines = false;
        fileName = "Dashboard-IVR-" + fileName;

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
    protected void Custom_Export_Excel_RecordDetails(object sender, EventArgs e)
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
        String fileName = "Record-Details";
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
        ws.Range(sRow, sCol, sRow, sCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
        GridView gv = gvCallDetailsExport;
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
                    if (gvRow.Cells[i].Text != "&nbsp;")
                    {
                        //
                        if (gv.HeaderRow.Cells[i].Text == "Amount")
                        {
                            cl.Value = gvRow.Cells[i].Text;
                            cl.Style.NumberFormat.Format = "$#,##0.00";
                        }
                        else if (gv.HeaderRow.Cells[i].Text == "Date")
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
        #endregion Process each Disposition Row
        #endregion Grid - Call Dispositions
        #region Wrap Up - Save/Download the File
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        ws.Rows().AdjustToContents();
        ws.Columns().AdjustToContents();
        ws.Columns(1, 3).Width = 9;
        ws.Column(4).Width = 13;
        //3 * 
        ws.ShowGridLines = false;
        fileName = "Dashboard-Reporting-" + fileName;

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
    
    protected String EpochTo_FormatTime_Local(String Epoch)
    {
        var utcDate = DateTime.Now.ToUniversalTime();
        long baseTicks = 621355968000000000;
        long tickResolution = 10000000;
        long epoch = Convert.ToInt32(Epoch);
        long epochTicks = (epoch * tickResolution) + baseTicks;

        int TZ = -7;
        return new DateTime(epochTicks, DateTimeKind.Utc).AddHours(TZ).ToString("MM/dd/yyyy hh:mm:ss tt");
    }
    protected string DateToFormatTime(DateTime dt)
    {
        return dt.ToString("MM/dd/yyyy hh:mm:ss tt");
    }
    protected DateTime EpochToDate_Local(String Epoch)
    {
        var utcDate = DateTime.Now.ToUniversalTime();
        long baseTicks = 621355968000000000;
        long tickResolution = 10000000;
        long epoch = Convert.ToInt32(Epoch);
        long epochTicks = (epoch * tickResolution) + baseTicks;

        int TZ = -7;
        return new DateTime(epochTicks, DateTimeKind.Utc).AddHours(TZ);
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
    #region Identity Functions
    protected void identity_get_userid()
    {
        // Get the logged in users userid
        // This should be retrieved during the login process
        // if (Session["userid"] == null) { identity_get_userid(); }
        // cmd.Parameters.Add(new SqlParameter("@sp_actor", Session["userid"].ToString()));
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrPortal))
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

                    cmd.Parameters.Add(new SqlParameter("@sp_username", this.Page.User.Identity.Name));
                    #endregion SQL Parameters
                    //print_sql(cmd, "append"); // Will print for Admin in Local
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                Session["userid"] = sqlRdr["userid"].ToString();
                                //Label8.Text += "<br />UserID: " + Session["userid"].ToString();
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
    protected string identity_get_username(string userid)
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
                using (SqlConnection con = new SqlConnection(sqlStrPortal))
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
                        print_sql(cmd, "append"); // Will print for Admin in Local
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
                Error_Save(ex, "User Get UserID");
            }
        }
        return username;
    }
    protected bool identity_is_admin()
    {
        if (Page.User.IsInRole("System Administrator") == true
            || Page.User.IsInRole("Administrator") == true
            || Page.User.IsInRole("Manager") == true
            || (Page.User.IsInRole("Advisor") == true && Page.User.Identity.Name != "agent2014")
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected bool identity_is_admin_super()
    {
        if (Page.User.IsInRole("System Administrator") == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected bool identity_can_edit()
    {
        if (Page.User.IsInRole("System Administrator") == true
            || Page.User.IsInRole("Administrator") == true
            || (Page.User.IsInRole("Advisor") == true && Page.User.Identity.Name != "agent2014")
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion Identity Functions

}