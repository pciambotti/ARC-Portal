using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
public partial class Billing_All : System.Web.UI.Page
{
    private String sqlStrARC = Connection.GetConnectionString("ARC_Production", ""); // ARC_Production || ARC_Stage
    private String sqlStrDE = Connection.GetConnectionString("DE_Production", ""); // DE_Production || DE_Stage
    Double _billingCalls = 0;
    Double _billingMinutes = 0;
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Billing";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            rpTimeZone.Text += "-" + ghFunctions.dtUserOffSet.ToString() + " (US Eastern Timezone)";

            DateTime dt = DateTime.UtcNow.AddMonths(-1);
            dtStartDate.Text = ghFunctions.dtGetFirstDayFrom(dt); // DateTime.Now.AddDays(-3).ToString("MM/dd/yyyy");
            dtStartTime.Text = "00:00";
            dt = DateTime.Parse(ghFunctions.dtGetFirstDay()).AddDays(-1);
            dtEndDate.Text = dt.ToString("MM/dd/yyyy");
            dtEndTime.Text = "23:59";

            // GridView_Refresh();
        }
    }
    #region GridView Handling
    /// <summary>
    /// This will be the billing page that Accounting can use to gather the needed data
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        try
        {
            lblDateRange.Text = "from " + dtStartDate.Text + " at " + dtStartTime.Text + " to " + dtEndDate.Text + " at " + dtEndTime.Text;
            DateTime dtStart = DateTime.Now;
            //System.Threading.Thread.Sleep(7000);
            GridView_Refresh();
            //System.Threading.Thread.Sleep(2500);
            DateTime dtEnd = DateTime.Now;
            rpElapsed.Text = "Total Elapsed Time: <b>" + ghFunctions.SecondsTo((dtEnd - dtStart).TotalSeconds) + "</b>";
            rpError.Text = "";
        }
        catch { }
    }
    protected void GridView_Refresh()
    {
        //Session["Messages_GridView"] = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        Billing_Data_Query_ARC_Dedicated(gvBillingARC_Dedicated);
        Billing_Data_Query_ARC_Standard(gvBillingARC_Standard);
        Billing_Data_Query_ARC_Total(gvBillingARC_Total);
        Billing_Data_Query_ARC_Transactions(gvBillingARC_Transactions);

        ghFunctions.WriteToLabel("new", "Blue", gvBillingARC_Dedicated.ID + "Refresh Processed [" + DateTime.Now.ToString("HH:mm:ss") + "]<br />", msgLabel);
    }
    protected void Billing_Data_Query_ARC_Dedicated(GridView gv)
    {
        // Change to this section should be duplicated to this section: Search_Data_Query_Counts
        #region SQL Connection
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                ghFunctions.Donation_Open_Database(con);
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
'Dedicated Main' [type]
,COUNT([j].[callid]) [calls]
,ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0) [time_seconds]
,CONVERT(decimal(18,2),ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0)/60) [time_minutes]
,0 [sort]
FROM (
	SELECT  
	[c].[callid]
	,[c].[logindatetime] [callstartdatetime] 
	,[c].[callenddatetime]
	,CASE
		WHEN [c].[callenddatetime] IS NULL THEN 0 
		ELSE
			CASE
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) > 1200 THEN 1200 -- Max 20 minutes
				ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])
			END
		END [time_seconds]
	FROM [dbo].[call] [c] WITH(NOLOCK)
	LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
	LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid]= [c].[callid]
	LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [c].[dispositionid]= [d].[dispositionid] 
	LEFT OUTER JOIN [dbo].[designation] [s] WITH(NOLOCK) ON [di].[designationid] = [s].[designationid]
	WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x'))
	AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end -- Include Date
	AND [c].[dispositionid] NOT IN (20) -- Exclude Training
	AND ([di].[ccnum] <> '4111111111111111' OR [di].[ccnum] = '' OR [di].[ccnum] IS NULL) -- Exclude Test transactions

	AND RIGHT([c].[dnis],4) NOT IN ('2824','2857','5641','5642','5643','5644') -- Exclude Standard DNIS
	/*
		Here we exclude certain COMPANY and DESIGNATION
		These are included further down
		SELECT * FROM [dbo].[designation] [d] WITH(NOLOCK) WHERE [d].[designationid] IN (180,182,183,184,186)
		SELECT * FROM [dbo].[designation] [d] WITH(NOLOCK) WHERE [d].[name] LIKE '%mis%'
	*/

	AND ([dn].[company] IS NULL OR [dn].[company] NOT IN ('DRTV','Globetrotters'))
	AND ([di].[designationid] IS NULL OR [di].[designationid] IN (35,109,158,169,170)) -- Include Main Designations
) [j]
UNION 
SELECT
'Dedicated ' + [j].[company] [type]
,COUNT([j].[callid]) [calls]
,ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0) [time_seconds]
,CONVERT(decimal(18,2),ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0)/60) [time_minutes]
,1 [sort]
FROM (
	SELECT  
	[c].[callid]
	,[c].[logindatetime] [callstartdatetime] 
	,[c].[callenddatetime]
	,CASE
		WHEN [c].[callenddatetime] IS NULL THEN 0 
		ELSE
			CASE
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) > 1200 THEN 1200 -- Max 20 minutes
				ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])
			END
		END [time_seconds]
	,[di].[designationid]
	,[dn].[company]
	FROM [dbo].[call] [c] WITH(NOLOCK)
	LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
	LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid]= [c].[callid]
	LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [c].[dispositionid]= [d].[dispositionid] 
	LEFT OUTER JOIN [dbo].[designation] [s] WITH(NOLOCK) ON [di].[designationid] = [s].[designationid]
	WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x')) 
	AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end -- Include Date
	AND [c].[dispositionid] NOT IN (20) -- Exclude Training
	AND ([di].[ccnum] <> '4111111111111111' OR [di].[ccnum] = '' OR [di].[ccnum] IS NULL) -- Exclude Test transactions
	AND RIGHT([c].[dnis],4) NOT IN ('2824','2857','5641','5642','5643','5644') -- Exclude Standard DNIS
	AND [dn].[company] IN ('DRTV','Globetrotters') -- Include Specific Company
) [j]
GROUP BY [j].[company]
UNION 
SELECT
'Dedicated ' + [j].[designation_name] [type]
,COUNT([j].[callid]) [calls]
,ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0) [time_seconds]
,CONVERT(decimal(18,2),ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0)/60) [time_minutes]
,[j].[designationid] [sort]
FROM (
	SELECT  
	[c].[callid]
	,[c].[logindatetime] [callstartdatetime] 
	,[c].[callenddatetime]
	,CASE
		WHEN [c].[callenddatetime] IS NULL THEN 0 
		ELSE
			CASE
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) > 1200 THEN 1200 -- Max 20 minutes
				ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])
			END
		END [time_seconds]
	,[di].[designationid]
	,[s].[name] [designation_name]
	FROM [dbo].[call] [c] WITH(NOLOCK)
	LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
	LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid]= [c].[callid]
	LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [c].[dispositionid]= [d].[dispositionid] 
	LEFT OUTER JOIN [dbo].[designation] [s] WITH(NOLOCK) ON [di].[designationid] = [s].[designationid]
	WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x')) 
	AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end -- Include Date
	AND [c].[dispositionid] NOT IN (20) -- Exclude Training
	AND ([di].[ccnum] <> '4111111111111111' OR [di].[ccnum] = '' OR [di].[ccnum] IS NULL) -- Exclude Test transactions
	AND RIGHT([c].[dnis],4) NOT IN ('2824','2857','5641','5642','5643','5644') -- Exclude Standard DNIS
	AND [di].[designationid] NOT IN (35,109,158,169,170) -- Exclude Main Designations
) [j]
GROUP BY [j].[designationid], [j].[designation_name]
ORDER BY [sort]
";
                    cmdText += "\r";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text).AddHours(ghFunctions.dtUserOffSet);
                    DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text).AddHours(ghFunctions.dtUserOffSet);

                    cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                    cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                    #endregion SQL Command Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                    gv.DataBind(); //dv_ivr_file_vc.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    #endregion SQL Command Processing
                }
                #endregion SQL Command

            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Save(ex, "Search_Data_Query");
        }
        #endregion SQL Connection
    }
    protected void Billing_Data_Query_ARC_Standard(GridView gv)
    {
        // Change to this section should be duplicated to this section: Search_Data_Query_Counts
        #region SQL Connection
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                ghFunctions.Donation_Open_Database(con);
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
'Standard Main' [Type]
,COUNT([j].[callid]) [calls]
,ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0) [time_seconds]
,CONVERT(decimal(18,2),ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0)/60) [time_minutes]
,0 [sort]
FROM (
	SELECT  
	[c].[callid]
	,[c].[logindatetime] [callstartdatetime] 
	,[c].[callenddatetime] 
	,CASE
		WHEN [c].[callenddatetime] IS NULL THEN 0 
		ELSE
			CASE
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) > 1200 THEN 1200 -- Max 20 minutes
				ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])
			END
		END [time_seconds]
	FROM [dbo].[call] [c] WITH(NOLOCK)
	LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
	LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid]= [c].[callid]
	WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x')) 
	AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end -- Include Date
	AND [c].[dispositionid] NOT IN (20) -- Exclude Training
	AND ([di].[ccnum] <> '4111111111111111' OR [di].[ccnum] = '' OR [di].[ccnum] IS NULL) -- Exclude Test transactions
	AND RIGHT([c].[dnis],4) IN ('2824','2857','5641','5642','5643','5644') -- Exclude Standard DNIS
	/*
		Here we exclude certain COMPANY and DESIGNATION
		These are included further down
		SELECT * FROM [dbo].[designation] [d] WITH(NOLOCK) WHERE [d].[designationid] IN (180,182,183,184,186)
		SELECT * FROM [dbo].[designation] [d] WITH(NOLOCK) WHERE [d].[name] LIKE '%mis%'
	*/
	AND ([dn].[company] IS NULL OR [dn].[company] NOT IN ('DRTV','Globetrotters'))
	AND ([di].[designationid] IS NULL OR [di].[designationid] IN (35,109,158,169,170)) -- Include Main Designations
) [j]
UNION
SELECT
'Standard ' + [j].[company] [type]
,COUNT([j].[callid]) [calls]
,ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0) [time_seconds]
,CONVERT(decimal(18,2),ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0)/60) [time_minutes]
,1 [sort]
FROM (
	SELECT  
	[c].[callid]
	,[c].[logindatetime] [callstartdatetime] 
	,[c].[callenddatetime] 
	,CASE
		WHEN [c].[callenddatetime] IS NULL THEN 0 
		ELSE
			CASE
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) > 1200 THEN 1200 -- Max 20 minutes
				ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])
			END
		END [time_seconds]
	,[di].[designationid]
	,[dn].[company]
	FROM [dbo].[call] [c] WITH(NOLOCK)
	LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
	LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid]= [c].[callid]
	WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x')) 
	AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end -- Include Date
	AND [c].[dispositionid] NOT IN (20) -- Exclude Training
	AND ([di].[ccnum] <> '4111111111111111' OR [di].[ccnum] = '' OR [di].[ccnum] IS NULL) -- Exclude Test transactions
	AND RIGHT([c].[dnis],4) IN ('2824','2857','5641','5642','5643','5644') -- Exclude Standard DNIS
	AND [dn].[company] IN ('DRTV','Globetrotters') -- Include Specific Company
) [j]
GROUP BY [j].[company]
UNION
SELECT
'Standard ' + [j].[designation_name] [type]
,COUNT([j].[callid]) [calls]
,ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0) [time_seconds]
,CONVERT(decimal(18,2),ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0)/60) [time_minutes]
,[j].[designationid] [sort]
FROM (
	SELECT  
	[c].[callid]
	,[c].[logindatetime] [callstartdatetime] 
	,[c].[callenddatetime] 
	,CASE
		WHEN [c].[callenddatetime] IS NULL THEN 0 
		ELSE
			CASE
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) > 1200 THEN 1200 -- Max 20 minutes
				ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])
			END
		END [time_seconds]
	,[di].[designationid]
	,[s].[name] [designation_name]
	FROM [dbo].[call] [c] WITH(NOLOCK)
	LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
	LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid]= [c].[callid]
	LEFT OUTER JOIN [dbo].[designation] [s] WITH(NOLOCK) ON [di].[designationid] = [s].[designationid]
	WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x')) 
	AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end -- Include Date
	AND [c].[dispositionid] NOT IN (20) -- Exclude Training
	AND ([di].[ccnum] <> '4111111111111111' OR [di].[ccnum] = '' OR [di].[ccnum] IS NULL) -- Exclude Test transactions
	AND RIGHT([c].[dnis],4) IN ('2824','2857','5641','5642','5643','5644') -- Exclude Standard DNIS
	AND [di].[designationid] NOT IN (35,109,158,169,170) -- Exclude Main Designations
) [j]
GROUP BY [j].[designationid], [j].[designation_name]
ORDER BY [sort]
                            ";
                    cmdText += "\r";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text).AddHours(ghFunctions.dtUserOffSet);
                    DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text).AddHours(ghFunctions.dtUserOffSet);

                    cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                    cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                    #endregion SQL Command Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                    gv.DataBind(); //dv_ivr_file_vc.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    #endregion SQL Command Processing
                }
                #endregion SQL Command

            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Save(ex, "Search_Data_Query");
        }
        #endregion SQL Connection
    }
    protected void Billing_Data_Query_ARC_Total(GridView gv)
    {
        // Change to this section should be duplicated to this section: Search_Data_Query_Counts
        #region SQL Connection
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                ghFunctions.Donation_Open_Database(con);
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
'Total Dedicated' [Type]
,COUNT([j].[callid]) [calls]
,ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0) [time_seconds]
,CONVERT(decimal(18,2),ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0)/60) [time_minutes]
,0 [sort]
FROM (
	SELECT  
	[c].[callid]
	,[c].[logindatetime] [callstartdatetime] 
	,[c].[callenddatetime]
	--,CASE WHEN [c].[callenddatetime] IS NULL THEN 0 ELSE CASE WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0 ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) END END [time_seconds]
	,CASE
		WHEN [c].[callenddatetime] IS NULL THEN 0 
		ELSE
			CASE
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) > 1200 THEN 1200 -- Max 20 minutes
				ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])
			END
		END [time_seconds]
	FROM [dbo].[call] [c] WITH(NOLOCK)
	LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
	LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid]= [c].[callid]
	LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [c].[dispositionid]= [d].[dispositionid] 
	LEFT OUTER JOIN [dbo].[designation] [s] WITH(NOLOCK) ON [di].[designationid] = [s].[designationid]
	WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x')) 
	AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end -- Include Date
	AND [c].[dispositionid] NOT IN (20) -- Exclude Training
	AND ([di].[ccnum] <> '4111111111111111' OR [di].[ccnum] = '' OR [di].[ccnum] IS NULL) -- Exclude Test transactions

	AND RIGHT([c].[dnis],4) NOT IN ('2824','2857','5641','5642','5643','5644') -- Include Standard DNIS
) [j]
UNION
SELECT
'Total Standard' [Type]
,COUNT([j].[callid]) [calls]
,ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0) [time_seconds]
,CONVERT(decimal(18,2),ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0)/60) [time_minutes]
,1 [sort]
FROM (
	SELECT  
	[c].[callid]
	,[c].[logindatetime] [callstartdatetime] 
	,[c].[callenddatetime] 
	--,CASE WHEN [c].[callenddatetime] IS NULL THEN 0 ELSE CASE WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0 ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) END END [time_seconds]
	,CASE
		WHEN [c].[callenddatetime] IS NULL THEN 0 
		ELSE
			CASE
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) > 1200 THEN 1200 -- Max 20 minutes
				ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])
			END
		END [time_seconds]
	FROM [dbo].[call] [c] WITH(NOLOCK)
	LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
	LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid]= [c].[callid]
	WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x')) 
	AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end -- Include Date
	AND [c].[dispositionid] NOT IN (20) -- Exclude Training
	AND ([di].[ccnum] <> '4111111111111111' OR [di].[ccnum] = '' OR [di].[ccnum] IS NULL) -- Exclude Test transactions

	AND RIGHT([c].[dnis],4) IN ('2824','2857','5641','5642','5643','5644') -- Exclude Standard DNIS
) [j]
UNION
SELECT
'Total' [Type]
,COUNT([j].[callid]) [calls]
,ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0) [time_seconds]
,CONVERT(decimal(18,2),ISNULL(CONVERT(decimal(18,2),SUM([j].[time_seconds])),0)/60) [time_minutes]
,2 [sort]
FROM (
	SELECT  
	[c].[callid]
	,[c].[logindatetime] [callstartdatetime] 
	,[c].[callenddatetime]
	--,CASE WHEN [c].[callenddatetime] IS NULL THEN 0 ELSE CASE WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0 ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) END END [time_seconds]
	,CASE
		WHEN [c].[callenddatetime] IS NULL THEN 0 
		ELSE
			CASE
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) < 0 THEN 0
				WHEN DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime]) > 1200 THEN 1200 -- Max 20 minutes
				ELSE DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])
			END
		END [time_seconds]
	FROM [dbo].[call] [c] WITH(NOLOCK)
	LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
	LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid]= [c].[callid]
	LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [c].[dispositionid]= [d].[dispositionid] 
	LEFT OUTER JOIN [dbo].[designation] [s] WITH(NOLOCK) ON [di].[designationid] = [s].[designationid]
	WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x')) 
	AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end -- Include Date
	AND [c].[dispositionid] NOT IN (20) -- Exclude Training
	AND ([di].[ccnum] <> '4111111111111111' OR [di].[ccnum] = '' OR [di].[ccnum] IS NULL) -- Exclude Test transactions
) [j]
ORDER BY [sort]
                            ";
                    cmdText += "\r";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text).AddHours(ghFunctions.dtUserOffSet);
                    DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text).AddHours(ghFunctions.dtUserOffSet);

                    cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                    cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                    #endregion SQL Command Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                    gv.DataBind(); //dv_ivr_file_vc.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    #endregion SQL Command Processing
                }
                #endregion SQL Command

            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Save(ex, "Search_Data_Query");
        }
        #endregion SQL Connection
    }
    protected void Billing_Data_Query_ARC_Transactions(GridView gv)
    {
        // Change to this section should be duplicated to this section: Search_Data_Query_Counts
        #region SQL Connection
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                ghFunctions.Donation_Open_Database(con);
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
'Standard Transactions' [type]
,COUNT(DISTINCT([cb].[id])) [count]
,ISNULL(SUM(CASE
	WHEN [cb].[decision] = 'ACCEPT' THEN [cb].[ccauthreply_amount]
	ELSE 0
END),0) [amount_approved]
,00 [sort]
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[donationccinfo] [d] WITH(NOLOCK) ON [d].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [d].[id]
LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x'))
--AND [cb].[createdate] BETWEEN @sp_date_start AND @sp_date_end
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
AND ([dn].[company] != 'Globetrotters' OR [dn].[company] IS NULL)
UNION
SELECT
'Globetrotters Transactions' [type]
,COUNT(DISTINCT([cb].[id])) [count]
,ISNULL(SUM(CASE
	WHEN [cb].[decision] = 'ACCEPT' THEN [cb].[ccauthreply_amount]
	ELSE 0
END),0) [amount_approved]
,1 [sort]
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[donationccinfo] [d] WITH(NOLOCK) ON [d].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [d].[id]
LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
WHERE 1=1 AND [dn].[phonenumber] NOT IN (SELECT [dt].[phonenumber] FROM [dbo].[dnis] [dt] WITH(NOLOCK) WHERE [dt].[city] IN ('801x','805x'))
--AND [cb].[createdate] BETWEEN @sp_date_start AND @sp_date_end
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
AND [dn].[company] = 'Globetrotters'
UNION
SELECT
'Recurring Transactions' [type]
,COUNT(DISTINCT([cb].[id])) [count]
,ISNULL(SUM(CASE
	WHEN [cb].[decision] = 'ACCEPT' THEN [cb].[ccauthreply_amount]
	ELSE 0
END),0) [amount_approved]
,10 [sort]
FROM [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK)
WHERE 1=1
AND [cb].[createdate] BETWEEN @sp_date_start AND @sp_date_end
AND [cb].[source] IN ('RECURRING')
ORDER BY [sort]
                            ";
                    cmdText += "\r";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text).AddHours(ghFunctions.dtUserOffSet);
                    DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text).AddHours(ghFunctions.dtUserOffSet);

                    cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                    cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                    #endregion SQL Command Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                    gv.DataBind(); //dv_ivr_file_vc.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    #endregion SQL Command Processing
                }
                #endregion SQL Command

            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Save(ex, "Search_Data_Query");
        }
        #endregion SQL Connection
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

            if (gv.ID == "gvBillingARC_Dedicated" || gv.ID == "gvBillingARC_Standard")
            {
                double tmpCalls = 0;
                double.TryParse(e.Row.Cells[1].Text, out tmpCalls);
                _billingCalls += tmpCalls;

                double tmpDonations = 0;
                double.TryParse(e.Row.Cells[2].Text, out tmpDonations);
                _billingMinutes += tmpDonations;

            }

        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            if (gv.ID == "gvBillingARC_Dedicated" || gv.ID == "gvBillingARC_Standard")
            {
                e.Row.Cells[0].Text = "Total";
                e.Row.Cells[1].Text = String.Format("{0:N}", _billingCalls);
                e.Row.Cells[2].Text = String.Format("{0:N}", _billingMinutes);
                e.Row.Font.Bold = true;
                _billingCalls = 0;
                _billingMinutes = 0;
            }
        }
    }
    protected void GridView_IndexChanged(object sender, EventArgs e)
    {
    }
    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
    }
    #endregion GridView Handling
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

        ErrorLog.ErrorLog_Save(ex, dv, "Ameriprise Admin Portal", error, spPage, spQS, spURL);
    }
    protected void print_sql(SqlCommand cmd)
    {
        print_sql(cmd, sqlPrint, "append");
    }
    protected void print_sql(SqlCommand cmd, Label lblPrint, String type)
    {
        #region Print SQL
        if (Page.User.IsInRole("System Administrator") == true && Connection.GetConnectionType() == "Local")
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
}
