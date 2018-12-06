using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class Dashboard_Reporting : System.Web.UI.Page
{
    private String sqlStrARC = Connection.GetConnectionString("ARC_Production", ""); // ARC_Production || ARC_Stage
    private String sqlStrDE = Connection.GetConnectionString("DE_Production", ""); // DE_Production || DE_Stage
    private String sqlStrRec = Connection.GetConnectionString("RECORDINGS", ""); // 
    private String sqlStrInt = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage

    private Int32 countCalls = 0;
    private String strClient = "ARC"; //ddlCallClients.SelectedValue.PadLeft(3, '0')
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        if (Session["userid"] == null) { identity_get_userid(); Response.Redirect("~/Dashboard_Reporting.aspx"); }
        Master.PageTitle = "DRTV Reporting";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
        if (Connection.GetDBMode() == "Stage")
        {
            sqlStrARC = Connection.GetConnectionString("ARC_Stage", ""); // ARC_Production || ARC_Stage
            sqlStrDE = Connection.GetConnectionString("DE_Stage", ""); // DE_Production || DE_Stage
            sqlStrInt = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
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
        // If we lose session, it can cause errors.
        if (!IsPostBack)
        {
            rpTimeZone.Text += "-" + ghFunctions.dtUserOffSet.ToString() + " (US Eastern Timezone)";
            #region Chart Image
            if (Session["imgNameSel"] == null || !IsPostBack)
            {
                // Get the Image Name for Charts
                //this.Page.User.Identity.Name
                //if (Page.User.IsInRole("System Administrator") == true && Page.User.Identity.Name == "nciambotti@greenwoodhall.com")
                //userid
                if (Session["userid"] == null) { identity_get_userid(); }
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
                Session["imgNameSel"] = imgName;
            }
            #endregion Chart Image
            // If Today == 1st of Month, go back 7 days
            // dtStartDate.Text = ghFunctions.dtGetFirstDay(); // DateTime.Now.AddDays(-3).ToString("MM/dd/yyyy");

            // If Monday - we go back to Friday <> SUnday
            DateTime dtStart = DateTime.UtcNow.AddDays(0).AddHours(-ghFunctions.dtUserOffSet);

            rpMessage.Text = "Day Of Week: " + dtStart.DayOfWeek.ToString();
            if (dtStart.DayOfWeek.ToString() == "Monday")
            {
                dtStartDate.Text = dtStart.AddDays(-3).ToString("MM/dd/yyyy"); // yyyy-MM-dd
                dtEndDate.Text = dtStart.AddDays(-1).ToString("MM/dd/yyyy"); // yyyy-MM-dd
            }
            else
            {
                dtStartDate.Text = dtStart.AddDays(-1).ToString("MM/dd/yyyy"); // yyyy-MM-dd
                dtEndDate.Text = dtStart.AddDays(-1).ToString("MM/dd/yyyy"); // yyyy-MM-dd

            }

            // dtStartDate.Text = DateTime.Now.AddDays(-15).ToString("MM/dd/yyyy");
            dtStartTime.Text = "00:00:00";
            dtEndTime.Text = "23:59:59";
            Dashboard_Refresh();

            // rpElapsed.Text = "UTC: " + DateTime.UtcNow.AddHours(0).ToString("yyyy-MM-dd HH:mm:ss.fff");
            // rpElapsed.Text += "| Offset: " + DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet).ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
    protected void Dashboard_Refresh(object sender, EventArgs e)
    {
        Dashboard_Clear();
        Dashboard_Refresh();
    }
    protected void Dashboard_Data_Export(object sender, EventArgs e)
    {
        try
        {
            /// Reporting Export
            /// Based on the button clicked
            /// MMS-CLIENTABBRV-MMDDYY (ex. MMS-ARC-081216)
            /// Dashboard_Data_Export_

            GridView gv = null;
            String fileName = "";
            Button btn = (Button)sender;
            DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);

            if (btn.ID == "btnReportDRTVMMS")
            {
                Dashboard_Data_Export_Excel_DRTV("MMS");
            }
            else if (btn.ID == "btnReportDRTVMMScsv")
            {
                fileName = "MMS-ARC-" + dtTo.ToString("MMddyy");
                gv = gvReportDRTVMMSExport;
                Dashboard_Data_Export_CSV_DRTV(gv, fileName);
            }
            else if (btn.ID == "btnReportDRTVRNO")
            {
                Dashboard_Data_Export_Excel_DRTV("RNO");
            }
            else if (btn.ID == "btnReportDRTVRNOcsv")
            {
                fileName = "RNO-ARC-" + dtTo.ToString("MMddyy");
                gv = gvReportDRTVRNOExport;
                Dashboard_Data_Export_CSV_DRTV(gv, fileName);
            }
            else if (btn.ID == "btnReportMainMMS")
            {
                Dashboard_Data_Export_Excel_DRTV("MainMMS");
            }
            else if (btn.ID == "btnReportMainMMScsv")
            {
                fileName = "800-ARC-MMS-" + dtTo.ToString("MMddyy");
                gv = gvReportMainMMSExport;
                Dashboard_Data_Export_CSV_DRTV(gv, fileName);
            }
            else if (btn.ID == "btnReportMainRNO")
            {
                Dashboard_Data_Export_Excel_DRTV("MainRNO");
            }
            else if (btn.ID == "btnReportMainRNOcsv")
            {
                fileName = "800-ARC-RNO-" + dtTo.ToString("MMddyy");
                gv = gvReportMainRNOExport;
                Dashboard_Data_Export_CSV_DRTV(gv, fileName);
            }
            else if (btn.ID == "btnReportDRTVCallHandling")
            {
                Dashboard_Data_Export_Excel_DRTV("CALL_HANDLING");
            }
            else if (btn.ID == "btnReportDRTVMasterFile")
            {
                Dashboard_Data_Export_Excel_DRTV("MASTER_FILE");
            }
            else if (btn.ID == "btnReportDRTVFulfillmentDetail")
            {
                Dashboard_Data_Export_Excel_DRTV("FULFILLMENT_DETAIL");
            }
            else if (btn.ID == "btnReportDRTVFulfillmentDetailcsv")
            {
                fileName = "ARC_Import_" + dtTo.ToString("yyyyMMddHHMMss") + "_Details";
                gv = gvReportDRTVFulfillmentDetailExport;
                Dashboard_Data_Export_CSV_DRTV(gv, fileName);
            }
            else if (btn.ID == "btnReportDRTVFulfillmentTransactionc")
            {
                Dashboard_Data_Export_Excel_DRTV("FULFILLMENT_TRANSACTION");
            }
            else if (btn.ID == "btnReportDRTVFulfillmentTransactioncsv")
            {
                fileName = "ARC_Import_" + dtTo.ToString("yyyyMMddHHMMss") + "_Transactions";
                gv = gvReportDRTVFulfillmentTransactionExport;
                Dashboard_Data_Export_CSV_DRTV(gv, fileName);
            }
            else
            {
                // 
                // Exception
                throw new Exception("Unhandled report");
            }
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "Dashboard - Export - DRTV MMS", msgLabel);
        }
    }
    protected void Dashboard_Refresh()
    {
        try
        {
            lblMessage.Text = "UTC Dates";
            DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
            DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
            dtFrom = dtOffSetAdd(dtFrom);
            dtTo = dtOffSetAdd(dtTo);
            lblMessage.Text += "<br />" + dtFrom.ToString();
            lblMessage.Text += "<br />" + dtTo.ToString();
            lblMessage.Text += "<br />";


            if (cbReportMMS.Checked) { report_drtv_mms.Visible = true; Dashboard_Data_Reporting(gvReportDRTVMMS, gvReportDRTVMMSExport, lblReportDRTVMMS); }
            else { report_drtv_mms.Visible = false; }

            if (cbReportRNO.Checked) { report_drtv_rno.Visible = true; Dashboard_Data_Reporting(gvReportDRTVRNO, gvReportDRTVRNOExport, lblReportDRTVRNO); }
            else { report_drtv_rno.Visible = false; }

            if (cbReportMainMMS.Checked) { report_main_mms.Visible = true; Dashboard_Data_Reporting(gvReportMainMMS, gvReportMainMMSExport, lblReportMainMMS); }
            else { report_main_mms.Visible = false; }

            if (cbReportMainRNO.Checked) { report_main_rno.Visible = true; Dashboard_Data_Reporting(gvReportMainRNO, gvReportMainRNOExport, lblReportMainRNO); }
            else { report_main_rno.Visible = false; }

            if (cbReportCallHandling.Checked) { report_drtv_callhandling.Visible = true; Dashboard_Data_Reporting_DRTV_CallHandling(); }
            else { report_drtv_callhandling.Visible = false; }

            if (cbReportMasterFile.Checked) { report_drtv_masterfile.Visible = true; Dashboard_Data_Reporting_DRTV_MasterFile(); }
            else { report_drtv_masterfile.Visible = false; }

            if (cbReportFulfillmentDetail.Checked) { report_drtv_fulfillment_detail.Visible = true; Dashboard_Data_Reporting(gvReportDRTVFulfillmentDetail, gvReportDRTVFulfillmentDetailExport, lblReportDRTVFulfillmentDetail); }
            else { report_drtv_fulfillment_detail.Visible = false; }

            if (cbReportFulfillmentTransaction.Checked) { report_drtv_fulfillment_transaction.Visible = true; Dashboard_Data_Reporting(gvReportDRTVFulfillmentTransaction, gvReportDRTVFulfillmentTransactionExport, lblReportDRTVFulfillmentTransaction); }
            else { report_drtv_fulfillment_transaction.Visible = false; }
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "Dashboard - Refresh", msgLabel);
        }
    }
    protected void Dashboard_Clear()
    {
        try
        {
            gvReportDRTVMMS.DataBind();
            gvReportDRTVMMSExport.DataBind();
            lblReportDRTVMMS.Text = "0";

            gvReportDRTVRNO.DataBind();
            gvReportDRTVRNOExport.DataBind();
            lblReportDRTVRNO.Text = "0";

            gvReportMainMMS.DataBind();
            gvReportMainMMSExport.DataBind();
            lblReportMainMMS.Text = "0";
            gvReportMainRNO.DataBind();
            gvReportMainRNOExport.DataBind();
            lblReportMainRNO.Text = "0";

            gvReportDRTVFulfillmentDetail.DataBind();
            gvReportDRTVFulfillmentDetailExport.DataBind();
            lblReportDRTVFulfillmentDetail.Text = "0";

            gvReportDRTVFulfillmentTransaction.DataBind();
            gvReportDRTVFulfillmentTransactionExport.DataBind();
            lblReportDRTVFulfillmentTransaction.Text = "0";
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "Dashboard - Clear", msgLabel);
        }
    }
    protected void Dashboard_Data_Reporting(GridView gv, GridView gvEx, Label lblRprt)
    {
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                #region Build cmdText
                String cmdText = "";
                if (gv.ID == "gvReportDRTVMMS")
                {
                    btnReportDRTVMMS.Visible = true;
                    btnReportDRTVMMScsv.Visible = true;
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
                            SELECT
                            TOP (@sp_top)
                            [c].[callid]
                            ,[di].[id] [donationid]
                            ,[c].[LoginDateTime]
                            ,[dn].[phonenumber]
                            ,[ci].[zip]
                            ,CASE
                                WHEN [d].[displayname] = 'Initiated' THEN 'Training'
                                WHEN [d].[displayname] IN ('Pledge','Pledge [Sustainer]') AND ([ci].[zip] IS NULL OR LEN([ci].[zip]) = 0) THEN 'Pledge [No Address]'
                                ELSE [d].[displayname]
                            END [disposition]
                            ,[di].[DonationAmount] [Amount]
                            ,[c].[ani]
                            FROM [dbo].[call] [c] WITH(NOLOCK)
                            LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
                            LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [d].[dispositionid] = [c].[dispositionid]
                            LEFT OUTER JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
                            LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
                            LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
                            LEFT OUTER JOIN [dbo].[merchantresponse] [mr] WITH(NOLOCK) ON [mr].[donationccinfoid] = [di].[id]
                            LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
                            WHERE 1=1
                            AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
                            ";
                    if (Connection.GetDBMode() != "Stage")
                    {
                        cmdText += "\r";
                        cmdText += @"
                            AND ([di].[ccnum] IS NULL OR [di].[ccnum] NOT IN ('4111111111111111'))
                            AND [c].[ani] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201') -- Test/Internal ANIs
                            ";
                    }
                    cmdText += "\r";
                    cmdText += @"
                            --AND [c].[dnis] IN (SELECT CASE WHEN LEN([c].[dnis]) = 4 THEN [dl].[dnis] ELSE [dl].[line] END FROM [dbo].[dnis] [dl] WHERE [dl].[company] = 'DRTV')
                            AND [dn].[company] = 'DRTV'
                            AND ([di].[id] IS NOT NULL
                                OR ([d].[displayname] = 'Wants More Sustainer Information' AND LEN([ci].[address]) > 0)
                                )
                            AND [c].[dispositionid] NOT IN (49)
AND [c].[callid] NOT IN (3352782,3352783,3352784,3352785,3352785,3352798,3352822,3352823,3352824,3352825,3352828,3352829,3352832,3352834,3352836,3352838,3352839,3352842,3352843,3352845,3352846,3352847,3352848,3352849,3352850,3352851,3352852,3352853,3352854,3352855,3352856,3352859,3352862,3352864,3352865,3352866,3352867,3352868,3352869,3352870,3352872,3352873,3352874,3352875,3352876,3352877,3352878,3352879,3352880,3352881,3352882,3352884,3352887,3352889,3352890,3352891,3352892,3352894,3352896,3352897,3352898,3352900,3352901,3352902,3352904,3352905,3352906,3352919,3352922,3352923,3352926,3352930,3352931,3352933,3352985,3353024,3353027,3353028,3353031,3353032,3353033,3353035,3353036,3353037,3353038,3353039,3353042,3353047,3353049,3353050,3353051,3353052,3353053,3353056,3353063,3353064,3353065,3353065,3353067,3353068,3353069,3353070,3353071,3353073,3353074,3353075,3353076,3353077,3353078,3353079,3353080,3353081,3353082,3353083,3353084,3353085,3353086,3353087,3353088,3353089,3353090,3353091,3353094,3353095,3353095,3353096,3353098,3353099,3353101,3353102,3353103,3353104,3353105,3353106,3353107,3353108,3353110,3353111,3353112,3353113,3353114,3353115,3353116,3353122,3353133,3353134,3353136,3353137,3353187,3353197,3353200,3353203,3353230,3353254,3353255,3353261,3353262,3353265,3353266,3353267,3353313,3353353,3353398,3353432,3353435,3353459,3353470,3353480,3353485)
                            ORDER BY [c].[callid]
                            ";

                    #endregion Build cmdText
                }
                else if (gv.ID == "gvReportDRTVRNO")
                {
                    btnReportDRTVRNO.Visible = true;
                    btnReportDRTVRNOcsv.Visible = true;
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
                            SELECT
                            TOP (@sp_top)
                            [c].[callid]
                            ,[di].[id] [donationid]
                            ,[c].[LoginDateTime]
                            ,[dn].[phonenumber]
                            ,[ci].[zip]
                            ,CASE
                                WHEN [d].[displayname] = 'Initiated' THEN 'Training'
                                WHEN [d].[displayname] IN ('Pledge','Pledge [Sustainer]') AND ([ci].[zip] IS NULL OR LEN([ci].[zip]) = 0) THEN 'Pledge [No Address]'
                                ELSE [d].[displayname]
                            END [disposition]
                            ,[di].[DonationAmount] [Amount]
                            ,[c].[ani]
                            FROM [dbo].[call] [c] WITH(NOLOCK)
                            LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
                            LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [d].[dispositionid] = [c].[dispositionid]
                            LEFT OUTER JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
                            LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
                            LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
                            LEFT OUTER JOIN [dbo].[merchantresponse] [mr] WITH(NOLOCK) ON [mr].[donationccinfoid] = [di].[id]
                            LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
                            WHERE 1=1
                            AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
                            ";
                    if (Connection.GetDBMode() != "Stage")
                    {
                        cmdText += "\r";
                        cmdText += @"
                            AND ([di].[ccnum] IS NULL OR [di].[ccnum] NOT IN ('4111111111111111'))
                            AND [c].[ani] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201') -- Test/Internal ANIs
                            ";
                    }
                    cmdText += "\r";
                    cmdText += @"
                            AND [dn].[company] = 'DRTV'
AND [c].[callid] NOT IN (3352782,3352783,3352784,3352785,3352785,3352798,3352822,3352823,3352824,3352825,3352828,3352829,3352832,3352834,3352836,3352838,3352839,3352842,3352843,3352845,3352846,3352847,3352848,3352849,3352850,3352851,3352852,3352853,3352854,3352855,3352856,3352859,3352862,3352864,3352865,3352866,3352867,3352868,3352869,3352870,3352872,3352873,3352874,3352875,3352876,3352877,3352878,3352879,3352880,3352881,3352882,3352884,3352887,3352889,3352890,3352891,3352892,3352894,3352896,3352897,3352898,3352900,3352901,3352902,3352904,3352905,3352906,3352919,3352922,3352923,3352926,3352930,3352931,3352933,3352985,3353024,3353027,3353028,3353031,3353032,3353033,3353035,3353036,3353037,3353038,3353039,3353042,3353047,3353049,3353050,3353051,3353052,3353053,3353056,3353063,3353064,3353065,3353065,3353067,3353068,3353069,3353070,3353071,3353073,3353074,3353075,3353076,3353077,3353078,3353079,3353080,3353081,3353082,3353083,3353084,3353085,3353086,3353087,3353088,3353089,3353090,3353091,3353094,3353095,3353095,3353096,3353098,3353099,3353101,3353102,3353103,3353104,3353105,3353106,3353107,3353108,3353110,3353111,3353112,3353113,3353114,3353115,3353116,3353122,3353133,3353134,3353136,3353137,3353187,3353197,3353200,3353203,3353230,3353254,3353255,3353261,3353262,3353265,3353266,3353267,3353313,3353353,3353398,3353432,3353435,3353459,3353470,3353480,3353485)
                            ORDER BY [c].[callid]
                            ";
                    #endregion Build cmdText
                }
                else if (gv.ID == "gvReportMainMMS")
                {
                    btnReportMainMMS.Visible = true;
                    btnReportMainMMScsv.Visible = true;
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
                            SELECT
                            TOP (@sp_top)
                            [c].[callid]
                            ,[di].[id] [donationid]
                            ,[c].[LoginDateTime]
                            ,[dn].[phonenumber]
                            ,[ci].[zip]
                            ,CASE WHEN [d].[displayname] = 'Initiated' THEN 'Training' ELSE [d].[displayname] END [disposition]
                            ,[di].[DonationAmount] [Amount]
                            ,[c].[ani]
                            FROM [dbo].[call] [c] WITH(NOLOCK)
                            LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
                            LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [d].[dispositionid] = [c].[dispositionid]
                            LEFT OUTER JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
                            LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
                            LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
                            LEFT OUTER JOIN [dbo].[merchantresponse] [mr] WITH(NOLOCK) ON [mr].[donationccinfoid] = [di].[id]
                            LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
                            WHERE 1=1
                            AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
                            ";
                    if (Connection.GetDBMode() != "Stage")
                    {
                        cmdText += "\r";
                        cmdText += @"
                            AND ([di].[ccnum] IS NULL OR [di].[ccnum] NOT IN ('4111111111111111'))
                            AND [c].[ani] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201') -- Test/Internal ANIs
                            ";
                    }
                    cmdText += "\r";
                    cmdText += @"
                            --AND [c].[dnis] IN (SELECT CASE WHEN LEN([c].[dnis]) = 4 THEN [dl].[dnis] ELSE [dl].[line] END FROM [dbo].[dnis] [dl] WHERE [dl].[company] = 'DRTV')
                            AND ([dn].[company] <> 'DRTV' OR [dn].[company] IS NULL)
                            AND ([di].[id] IS NOT NULL
                                OR ([d].[displayname] = 'Wants More Sustainer Information' AND LEN([ci].[address]) > 0)
                                )
                            ORDER BY [c].[callid]
                            ";

                    #endregion Build cmdText
                }
                else if (gv.ID == "gvReportMainRNO")
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
                            SELECT
                            TOP (@sp_top)
                            [c].[callid]
                            ,[di].[id] [donationid]
                            ,[c].[LoginDateTime]
                            ,[dn].[phonenumber]
                            ,[ci].[zip]
                            ,CASE WHEN [d].[displayname] = 'Initiated' THEN 'Training' ELSE [d].[displayname] END [disposition]
                            ,[di].[DonationAmount] [Amount]
                            ,[c].[ani]
                            FROM [dbo].[call] [c] WITH(NOLOCK)
                            LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
                            LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [d].[dispositionid] = [c].[dispositionid]
                            LEFT OUTER JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
                            LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
                            LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
                            LEFT OUTER JOIN [dbo].[merchantresponse] [mr] WITH(NOLOCK) ON [mr].[donationccinfoid] = [di].[id]
                            LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
                            WHERE 1=1
                            AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
                            ";
                    if (Connection.GetDBMode() != "Stage")
                    {
                        cmdText += "\r";
                        cmdText += @"
                            AND ([di].[ccnum] IS NULL OR [di].[ccnum] NOT IN ('4111111111111111'))
                            AND [c].[ani] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201') -- Test/Internal ANIs
                            ";
                    }
                    cmdText += "\r";
                    cmdText += @"
                            AND ([dn].[company] <> 'DRTV' OR [dn].[company] IS NULL)
                            ORDER BY [c].[callid]
                            ";
                    #endregion Build cmdText
                    btnReportMainRNO.Visible = true;
                    btnReportMainRNOcsv.Visible = true;
                }
                else if (gv.ID == "gvReportDRTVFulfillmentDetail")
                {
                    // btnReportDRTVFulfillmentDetail.Visible = true;
                    btnReportDRTVFulfillmentDetailcsv.Visible = true;
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
SELECT
TOP (@sp_top)
[c].[callid] AS [DETAIL_ID]
,[c].[callid] AS [TRANSACTION_ID] -- [di].[id]
,'Donation' AS [LINE_ITEM_TYPE]
/*
Disposition	Appeal Code
Pledge				ARC_one_time
Pledge [Sustainer]	ARC_monthly
Information Only 	ARC_More_Info
Pledge Follow Up	ARC_delayed_1
Pledge [Sustainer] Follow Up	ARC_delayed_M

ALL: AND [c].[dispositionid] IN (10,32,41,42,43,46,47,48)
PLEDGE: AND [c].[dispositionid] IN (42,47,48)
41	Donation
42	Pledge
43	Special Cause Pledge
46	Sustainer
47	Pledge [Sustainer]
49	Pledge [No Address]

48	Wants More Sustainer Information
10	Information Only

32	2009 Holiday Gift Catalog Donation
*/
,'' AS [PRODUCT_SKU] -- FUN CODE ??
,ISNULL([di].[donationamount],0) AS [AMOUNT]
,'' AS [QUANTITY]
,'' AS [FUND_ID] -- Blank
,'' AS [PLEDGE_ID] -- Blank
,'' AS [PLEDGE_START_DATE] -- Blank
,'' AS [PLEDGE_TYPE] -- Blank
,'' AS [PLEDGE_FREQUENCY] -- Blank
,'' AS [PLEDGE_AUTOMATIC_PAYMENT] -- Blank
,'' AS [PLEDGE_PROGRAM] -- Blank
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
WHERE 1=1
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
AND [dn].[company] = 'DRTV'
AND [c].[dispositionid] IN (42,47,48,49)
AND [di].[donationamount] > 0
                            ";
                    if (Connection.GetDBMode() != "Stage")
                    {
                        cmdText += "\r";
                        cmdText += @"
                            AND ([di].[ccnum] IS NULL OR [di].[ccnum] NOT IN ('4111111111111111'))
                            AND [c].[ani] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201') -- Test/Internal ANIs
                            ";
                    }
                    cmdText += "\r";
                    cmdText += @"
AND [c].[callid] NOT IN (3352782,3352783,3352784,3352785,3352785,3352798,3352822,3352823,3352824,3352825,3352828,3352829,3352832,3352834,3352836,3352838,3352839,3352842,3352843,3352845,3352846,3352847,3352848,3352849,3352850,3352851,3352852,3352853,3352854,3352855,3352856,3352859,3352862,3352864,3352865,3352866,3352867,3352868,3352869,3352870,3352872,3352873,3352874,3352875,3352876,3352877,3352878,3352879,3352880,3352881,3352882,3352884,3352887,3352889,3352890,3352891,3352892,3352894,3352896,3352897,3352898,3352900,3352901,3352902,3352904,3352905,3352906,3352919,3352922,3352923,3352926,3352930,3352931,3352933,3352985,3353024,3353027,3353028,3353031,3353032,3353033,3353035,3353036,3353037,3353038,3353039,3353042,3353047,3353049,3353050,3353051,3353052,3353053,3353056,3353063,3353064,3353065,3353065,3353067,3353068,3353069,3353070,3353071,3353073,3353074,3353075,3353076,3353077,3353078,3353079,3353080,3353081,3353082,3353083,3353084,3353085,3353086,3353087,3353088,3353089,3353090,3353091,3353094,3353095,3353095,3353096,3353098,3353099,3353101,3353102,3353103,3353104,3353105,3353106,3353107,3353108,3353110,3353111,3353112,3353113,3353114,3353115,3353116,3353122,3353133,3353134,3353136,3353137,3353187,3353197,3353200,3353203,3353230,3353254,3353255,3353261,3353262,3353265,3353266,3353267,3353313,3353353,3353398,3353432,3353435,3353459,3353470,3353480,3353485)
ORDER BY [c].[callid]
                            ";

                    #endregion Build cmdText
                }
                else if (gv.ID == "gvReportDRTVFulfillmentTransaction")
                {
                    // btnReportDRTVFulfillmentTransaction.Visible = true;
                    btnReportDRTVFulfillmentTransactioncsv.Visible = true;
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
SELECT
TOP (@sp_top)
[c].[callid] AS [TRANSACTION_ID]
,'DRTV' AS [CHANNEL]
,'' AS [DONOR_ID] -- Blank
,[c].[logindatetime] AS [TRANSACTION_DATE] -- Use transaction date [Actual transaction date is different from CALL date in IVR records]
,CASE
	WHEN [c].[dispositionid] IN (41,42,43,49) THEN 'ARC_one_time'
	WHEN [c].[dispositionid] IN (46,47) THEN 'ARC_monthly'
	WHEN [c].[dispositionid] IN (10,48) THEN 'ARC_More_Info'
	ELSE 'N/A'
END AS [APPEAL_CODE]
/*
Disposition	Appeal Code
Pledge				ARC_one_time
Pledge [Sustainer]	ARC_monthly
Information Only 	ARC_More_Info
Pledge Follow Up	ARC_delayed_1
Pledge [Sustainer] Follow Up	ARC_delayed_M

ALL: AND [c].[dispositionid] IN (10,32,41,42,43,46,47,48)
PLEDGE: AND [c].[dispositionid] IN (42,47,48)
41	Donation
42	Pledge
43	Special Cause Pledge
46	Sustainer
47	Pledge [Sustainer]
49	Pledge [No Address]

48	Wants More Sustainer Information
10	Information Only

32	2009 Holiday Gift Catalog Donation
*/
,[d].[displayname] AS [SEGMENT_CODE]
,CASE WHEN [ci].[prefix] > 0 THEN [ci].[prefix] ELSE '' END AS [TITLE]
,[dbo].[fn_titlecase]([ci].[fname]) AS [FIRST_NAME]
,'' AS [MIDDLE_INITIAL]
,[dbo].[fn_titlecase]([ci].[lname]) AS [LAST_NAME]
,'' AS [SUFFIX]
,'' AS [SALUTATION]
,'' AS [SPOUSE]
,'' AS [COMPANY]
,[dbo].[fn_titlecase]([ci].[address]) AS [ADDRESS1]
,CASE WHEN LEN([ci].[suitenumber]) > 0 THEN (CASE WHEN [ci].[suitetype] > 0 AND LEN([ci].[suitenumber]) > 0 THEN (SELECT TOP 1 [st].[suitetype] + ' ' FROM [dbo].[suitetype] [st] WITH(NOLOCK) WHERE [st].[suitetypevalue] = [ci].[suitetype]) ELSE '' END) + [ci].[suitenumber] ELSE '' END AS [ADDRESS2] -- Use SuiteType too
,'' AS [ADDRESS3]
,UPPER([ci].[city]) AS [CITY]
,[ci].[state] AS [STATE_PROV]
,[ci].[zip] AS [POSTAL_CODE]
,ISNULL([ci].[country],'US') AS [COUNTRY]
,[ci].[email] AS [EMAIL] -- Which Email?
,[ci].[hphone] AS [PHONE] -- Check other Phones?
,'' AS [BIRTH_DATE]
,'' AS [ORIGIN_SOURCE]
,'' AS [FLAGS]
,'' AS [CUSTOM_DATA]
,'' AS [NOTES]
,ISNULL([di].[donationamount],0) AS [TOTAL_TRANSACTION_AMOUNT]
,CASE WHEN [di].[donationamount] IS NULL OR [di].[donationamount] <= 0 THEN NULL ELSE '0.00' END AS [SHIPPING_HANDLING_CHARGE]
,CASE WHEN [di].[donationamount] IS NULL OR [di].[donationamount] <= 0 THEN NULL ELSE '0.00' END AS [TAX_AMOUNT]
,CASE WHEN [di].[donationamount] IS NULL OR [di].[donationamount] <= 0 THEN NULL ELSE 'Check' END AS [PAYMENT_METHOD] -- Required.  Cash, Check, CreditCard, EFT
,'' AS [NUMBER] -- Do they really need CC Number? CVV is not possible - Also we do not do billing and shipping addresses typically when CC is present except for Holiday Giving
,'' AS [EXTRA_NUMBER]
,'' AS [CC_EXPIRATION_DATE]
,'' AS [CC_APPROVAL_INFORMATION]
,'' AS [BILLING_NAME]
,'' AS [BILLING_ADDRESS1]
,'' AS [BILLING_ADDRESS2]
,'' AS [BILLING_ADDRESS3]
,'' AS [BILLING_CITY]
,'' AS [BILLING_STATE_PROV]
,'' AS [BILLING_POSTAL_CODE]
,'' AS [BILLING_COUNTRY]
,'' AS [SHIPPING_METHOD]
,'' AS [SHIPTO_NAME]
,'' AS [SHIPTO_ADDRESS1]
,'' AS [SHIPTO_ADDRESS2]
,'' AS [SHIPTO_ADDRESS3]
,'' AS [SHIPTO_CITY]
,'' AS [SHIPTO_STATE_PROV]
,'' AS [SHIPTO_POSTAL_CODE]
,'' AS [SHIPTO_COUNTRY]
,'' AS [MEDIA_OUTLET]
,'' AS [MEDIA_PROGRAM]
,'' AS [ALLOW_CONDITIONAL_REPSONSES]
,'' AS [RESPONSE_LIST]
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [d].[dispositionid] = [c].[dispositionid]
LEFT OUTER JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
LEFT OUTER JOIN [dbo].[merchantresponse] [mr] WITH(NOLOCK) ON [mr].[donationccinfoid] = [di].[id]
LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[suitetype] [st] WITH(NOLOCK) ON [st].[suitetypevalue] = [ci].[suitetype]
LEFT OUTER JOIN [dbo].[cctypelookup] [pt] WITH(NOLOCK) ON [pt].[cctype] = [di].[cctype]
WHERE 1=1
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
AND [dn].[company] = 'DRTV'
AND [c].[dispositionid] IN (42,47,48,49)
                            ";
                    if (Connection.GetDBMode() != "Stage")
                    {
                        cmdText += "\r";
                        cmdText += @"
                            AND ([di].[ccnum] IS NULL OR [di].[ccnum] NOT IN ('4111111111111111'))
                            AND [c].[ani] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201') -- Test/Internal ANIs
                            ";
                    }
                    cmdText += "\r";
                    cmdText += @"
AND [c].[callid] NOT IN (3352782,3352783,3352784,3352785,3352785,3352798,3352822,3352823,3352824,3352825,3352828,3352829,3352832,3352834,3352836,3352838,3352839,3352842,3352843,3352845,3352846,3352847,3352848,3352849,3352850,3352851,3352852,3352853,3352854,3352855,3352856,3352859,3352862,3352864,3352865,3352866,3352867,3352868,3352869,3352870,3352872,3352873,3352874,3352875,3352876,3352877,3352878,3352879,3352880,3352881,3352882,3352884,3352887,3352889,3352890,3352891,3352892,3352894,3352896,3352897,3352898,3352900,3352901,3352902,3352904,3352905,3352906,3352919,3352922,3352923,3352926,3352930,3352931,3352933,3352985,3353024,3353027,3353028,3353031,3353032,3353033,3353035,3353036,3353037,3353038,3353039,3353042,3353047,3353049,3353050,3353051,3353052,3353053,3353056,3353063,3353064,3353065,3353065,3353067,3353068,3353069,3353070,3353071,3353073,3353074,3353075,3353076,3353077,3353078,3353079,3353080,3353081,3353082,3353083,3353084,3353085,3353086,3353087,3353088,3353089,3353090,3353091,3353094,3353095,3353095,3353096,3353098,3353099,3353101,3353102,3353103,3353104,3353105,3353106,3353107,3353108,3353110,3353111,3353112,3353113,3353114,3353115,3353116,3353122,3353133,3353134,3353136,3353137,3353187,3353197,3353200,3353203,3353230,3353254,3353255,3353261,3353262,3353265,3353266,3353267,3353313,3353353,3353398,3353432,3353435,3353459,3353470,3353480,3353485)
ORDER BY [c].[callid]
                            ";

                    #endregion Build cmdText
                }


                #endregion Build cmdText
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                int sp_top = 50000;
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                dtFrom = dtOffSetAdd(dtFrom);
                dtTo = dtOffSetAdd(dtTo);

                cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = sp_top;
                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                #endregion SQL Parameters

                #region SQL Processing - GridView
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                int dCount = dt.Rows.Count;
                lblRprt.Text = dCount.ToString();
                gv.DataSource = dt;
                gv.DataBind();

                gvEx.DataSource = dt;
                gvEx.DataBind();

                if (dCount > 0) { } // Not filtered
                #endregion SQL Processing - GridView

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Dashboard_Data_Reporting_DRTV_CallHandling()
    {
        DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
        DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);

        GridView gv = gvReportDRTVCallHandlingCalls;
        GridView gvEx = gvReportDRTVCallHandlingCallsExport;

        GridView gv2 = gvReportDRTVCallHandlingDon;
        GridView gv2Ex = gvReportDRTVCallHandlingDonExport;

        #region Declare DT
        DataTable dtCalls = new DataTable();
        DataTable dtDonations = new DataTable();
        dtCalls.Columns.AddRange(new DataColumn[9] {
                            new DataColumn("date", typeof(DateTime)),
                            new DataColumn("ivr_calls", typeof(string)),
                            new DataColumn("calls", typeof(string)),
                            new DataColumn("abandoned", typeof(string)),
                            new DataColumn("contacted", typeof(string)),
                            new DataColumn("abandoned_per", typeof(string)),
                            new DataColumn("answer_avg", typeof(string)),
                            new DataColumn("talk_avg",typeof(string)),
                            new DataColumn("ivr_duration_avg",typeof(string))
        });

        dtDonations.Columns.AddRange(new DataColumn[10] {
                            new DataColumn("date", typeof(DateTime)),
                            new DataColumn("calls", typeof(string)),
                            new DataColumn("calls_donation", typeof(string)),
                            new DataColumn("contacted", typeof(string)),
                            new DataColumn("calls_non", typeof(string)),
                            new DataColumn("donations_cc", typeof(string)),
                            new DataColumn("donations_cc_conv", typeof(string)),
                            new DataColumn("donations_avg", typeof(string)),
                            new DataColumn("pledge", typeof(string)),
                            new DataColumn("pledge_avg",typeof(string))
        });
        #endregion Declare DT
        DateTime dt1 = DateTime.Parse(dtFrom.ToString("yyyy-MM-dd"));
        DateTime dt2 = DateTime.Parse(dtTo.ToString("yyyy-MM-dd"));
        double days = (dt2 - dt1).TotalDays;

        int cnt = 0;
        while (dt1 <= dt2)
        {

            dtCalls.Rows.Add(dt1, "0", "0", "0", "0", "0", "0");
            dtDonations.Rows.Add(dt1, "0", "0", "0", "0", "0", "0", "0", "0", "0");

            dt1 = dt1.AddDays(1);
            if (cnt > 30) break;
        }

        int dCount = 0;
        dtFrom = dtOffSetAdd(dtFrom);
        dtTo = dtOffSetAdd(dtTo);

        #region SQL Connection - DE
        using (SqlConnection con = new SqlConnection(sqlStrDE))
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
DECLARE @sp_skills dbo.de_campaigns
INSERT INTO @sp_skills
	SELECT 102000006 

DECLARE @tmp_Days TABLE ([date] varchar(10)
							,[calls] int
							,[contacted] int
							,[abandoned] int
							,[answer_avg] int
							,[talk_avg] int
							)

DECLARE @Time1 DATETIME, @Time2 DATETIME
SET @Time1 = DATEADD(hh,-@sp_offset,@sp_date_start)
SET @Time2 = DATEADD(hh,-@sp_offset,@sp_date_end)

WHILE @Time1 < @Time2
BEGIN
	INSERT INTO @tmp_Days SELECT CONVERT(varchar(10),@Time1,101), 0, 0, 0, 0, 0
	SET @Time1 = DATEADD(d,1,@Time1)
END

	SELECT
	ISNULL([r].[date],[d].[date]) [date]
	,ISNULL([r].[calls],0) [calls]
	,ISNULL([r].[contacted],0) [contacted]
	,ISNULL([r].[abandoned],0) [abandoned]
	,ISNULL([r].[answer_avg],0) [answer_avg]
	,ISNULL([r].[talk_avg],0) [talk_avg]
	FROM @tmp_Days [d]
	LEFT OUTER JOIN (
		SELECT
		CONVERT(varchar(10),DATEADD(hh,-@sp_offset,[i].[datestart]),101) [date]
		,COUNT([i].[interactionid]) [calls]
		,COUNT(CASE WHEN [fct].[talk_time] > 0 THEN 1 ELSE NULL END) [contacted]
		,COUNT(CASE WHEN [fct].[talk_time] <= 0 OR [fct].[talk_time] IS NULL THEN 1 ELSE NULL END) [abandoned]
		,AVG(CASE WHEN [fct].[talk_time] > 0 AND [fct].[queue_time] > 0THEN [fct].[queue_time] ELSE NULL END) [answer_avg]
		,AVG(CASE WHEN [fct].[talk_time] > 0 THEN [fct].[talk_time] ELSE NULL END) [talk_avg]
		FROM [dbo].[interactions] [i] WITH(NOLOCK)
		JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
		JOIN [dbo].[five9_call_counts] [fcc] WITH(NOLOCK) ON [fcc].[companyid] = [i].[companyid] AND [fcc].[interactionid] = [i].[interactionid]
		JOIN [dbo].[five9_call_time] [fct] WITH(NOLOCK) ON [fct].[companyid] = [i].[companyid] AND [fct].[interactionid] = [i].[interactionid]
		WHERE 1=1
		AND [i].[companyid] = @sp_companyid
		AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
		AND [fc].[skillid] IN (SELECT [spc].[campaignid] FROM @sp_skills [spc])
		AND [i].[originator] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201')
		GROUP BY CONVERT(varchar(10),DATEADD(hh,-@sp_offset,[i].[datestart]),101)
		--ORDER BY CONVERT(varchar(10),DATEADD(hh,-@sp_offset,[i].[datestart]),101)
	) [r] ON [d].[date] = [r].[date]
	ORDER BY [d].[date]

                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = 3;

                cmd.Parameters.Add("@sp_offset", SqlDbType.Int).Value = ghFunctions.dtUserOffSet;
                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                #endregion SQL Parameters
                // print_sql(cmd);
                #region SQL Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            foreach (DataRow dtRow in dtCalls.Rows)
                            {
                                //sqlPrint.Text += "<br />Match:" + DateTime.Parse(dtRow["date"].ToString()).ToString("yyyyMMdd");
                                //sqlPrint.Text += " | " + DateTime.Parse(sqlRdr["date"].ToString()).ToString("yyyyMMdd");
                                if (DateTime.Parse(dtRow["date"].ToString()).ToString("yyyyMMdd") == DateTime.Parse(sqlRdr["date"].ToString()).ToString("yyyyMMdd"))
                                {
                                    dtRow["calls"] = sqlRdr["calls"].ToString();
                                    dtRow["abandoned"] = sqlRdr["abandoned"].ToString();
                                    dtRow["contacted"] = sqlRdr["contacted"].ToString();
                                    dtRow["answer_avg"] = sqlRdr["answer_avg"].ToString();
                                    dtRow["talk_avg"] = sqlRdr["talk_avg"].ToString();

                                }
                            }
                            foreach (DataRow dtRow in dtDonations.Rows)
                            {
                                if (DateTime.Parse(dtRow["date"].ToString()).ToString("yyyyMMdd") == DateTime.Parse(sqlRdr["date"].ToString()).ToString("yyyyMMdd"))
                                {
                                    dtRow["calls"] = sqlRdr["contacted"].ToString();
                                    dtRow["contacted"] = sqlRdr["contacted"].ToString();
                                }
                            }
                        }
                    }
                }
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection - DE


        #region SQL Connection - ARC
        using (SqlConnection con = new SqlConnection(sqlStrARC))
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
DECLARE @tmp_Days TABLE ([date] varchar(10)
							,[count_calls] bigint
							,[count_donations] bigint
							,[count_pledges] bigint
							,[amount_donation_avg] money
							,[amount_pledge_avg] money
							,[count_cc] bigint
							,[time_avg] bigint
							)


DECLARE @Time1 DATETIME, @Time2 DATETIME
SET @Time1 = @sp_date_start
SET @Time2 = @sp_date_end

WHILE @Time1 <= @Time2
BEGIN
	INSERT INTO @tmp_Days SELECT CONVERT(varchar(10),@Time1,101), 0, 0, 0, 0, 0, 0, 0
	SET @Time1 = DATEADD(d,1,@Time1)
END

SELECT
ISNULL([r].[date],[d].[date]) [date]
,ISNULL([r].[count_calls],0) [count_calls]
,ISNULL([r].[count_donations],0) [count_donations]
,ISNULL([r].[count_pledges],0) [count_pledges]
,ISNULL([r].[amount_donation_avg],0) [amount_donation_avg]
,ISNULL([r].[amount_pledge_avg],0) [amount_pledge_avg]
,ISNULL([r].[count_cc],0) [count_cc]
,ISNULL([r].[time_avg],0) [time_avg]
FROM @tmp_Days [d]
LEFT OUTER JOIN (
SELECT
CONVERT(varchar(10),DATEADD(hh,-@sp_offset,[c].[logindatetime]),101) [date]
,COUNT([c].[callid]) [count_calls]
,COUNT(CASE
	WHEN [di].[id] IS NOT NULL THEN [di].[id]
	WHEN [d].[displayname] LIKE '%pledge%' THEN [c].[callid]
	ELSE NULL
END) [count_donations]
,COUNT(CASE
	WHEN [d].[displayname] LIKE '%pledge%' THEN [c].[callid]
	ELSE NULL
END) [count_pledges]
,ISNULL(AVG(CASE
	WHEN [di].[cctype] IN (2,3,4,5,6) THEN [di].[DonationAmount]
	ELSE NULL
END),0) [amount_donation_avg]
,ISNULL(AVG(CASE
	WHEN [di].[cctype] IN (1,0) THEN [di].[DonationAmount]
	ELSE NULL
END),0) [amount_pledge_avg]
,COUNT(CASE
	WHEN [di].[cctype] IN (2,3,4,5,6) THEN [di].[id]
	ELSE NULL
END) [count_cc]
,AVG(DATEDIFF(s, [c].[logindatetime], [c].[callenddatetime])) [time_avg]
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [d].[dispositionid] = [c].[dispositionid]
LEFT OUTER JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
--LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
--LEFT OUTER JOIN [dbo].[merchantresponse] [mr] WITH(NOLOCK) ON [mr].[donationccinfoid] = [di].[id]
LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
WHERE 1=1
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
AND [dn].[company] = 'DRTV'
                            ";
                if (Connection.GetDBMode() != "Stage")
                {
                    cmdText += "\r";
                    cmdText += @"
AND ([di].[ccnum] IS NULL OR [di].[ccnum] NOT IN ('4111111111111111'))
AND [c].[ani] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201') -- Test/Internal ANIs
                            ";
                }
                cmdText += "\r";
                cmdText += @"
GROUP BY CONVERT(varchar(10),DATEADD(hh,-@sp_offset,[c].[logindatetime]),101)
--ORDER BY CONVERT(varchar(10),DATEADD(hh,-@sp_offset,[c].[logindatetime]),101)
) [r] ON [d].[date] = [r].[date]
AND [c].[callid] NOT IN (3352782,3352783,3352784,3352785,3352785,3352798,3352822,3352823,3352824,3352825,3352828,3352829,3352832,3352834,3352836,3352838,3352839,3352842,3352843,3352845,3352846,3352847,3352848,3352849,3352850,3352851,3352852,3352853,3352854,3352855,3352856,3352859,3352862,3352864,3352865,3352866,3352867,3352868,3352869,3352870,3352872,3352873,3352874,3352875,3352876,3352877,3352878,3352879,3352880,3352881,3352882,3352884,3352887,3352889,3352890,3352891,3352892,3352894,3352896,3352897,3352898,3352900,3352901,3352902,3352904,3352905,3352906,3352919,3352922,3352923,3352926,3352930,3352931,3352933,3352985,3353024,3353027,3353028,3353031,3353032,3353033,3353035,3353036,3353037,3353038,3353039,3353042,3353047,3353049,3353050,3353051,3353052,3353053,3353056,3353063,3353064,3353065,3353065,3353067,3353068,3353069,3353070,3353071,3353073,3353074,3353075,3353076,3353077,3353078,3353079,3353080,3353081,3353082,3353083,3353084,3353085,3353086,3353087,3353088,3353089,3353090,3353091,3353094,3353095,3353095,3353096,3353098,3353099,3353101,3353102,3353103,3353104,3353105,3353106,3353107,3353108,3353110,3353111,3353112,3353113,3353114,3353115,3353116,3353122,3353133,3353134,3353136,3353137,3353187,3353197,3353200,3353203,3353230,3353254,3353255,3353261,3353262,3353265,3353266,3353267,3353313,3353353,3353398,3353432,3353435,3353459,3353470,3353480,3353485)
ORDER BY [d].[date]
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add("@sp_offset", SqlDbType.Int).Value = ghFunctions.dtUserOffSet;
                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                #endregion SQL Parameters
                // print_sql(cmd);
                #region SQL Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            foreach (DataRow dtRow in dtDonations.Rows)
                            {
                                if (DateTime.Parse(dtRow["date"].ToString()).ToString("yyyyMMdd") == DateTime.Parse(sqlRdr["date"].ToString()).ToString("yyyyMMdd"))
                                {
                                    int tCalls = Int32.Parse(dtRow["calls"].ToString());
                                    int tDon = Int32.Parse(sqlRdr["count_donations"].ToString());

                                    dtRow["calls_donation"] = sqlRdr["count_donations"].ToString();

                                    //dtRow["calls_conv"] = sqlRdr["calls_conv"].ToString();
                                    //dtRow["calls_non"] = (tCalls - tDon).ToString();
                                    dtRow["donations_cc"] = sqlRdr["count_cc"].ToString();
                                    //dtRow["donations_cc_conv"] = sqlRdr["donations_cc_conv"].ToString();

                                    dtRow["donations_avg"] = sqlRdr["amount_donation_avg"].ToString();
                                    dtRow["pledge"] = sqlRdr["count_pledges"].ToString();
                                    dtRow["pledge_avg"] = sqlRdr["amount_pledge_avg"].ToString();

                                }
                            }
                        }
                    }
                }
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection - ARC

        #region SQL Connection - ARC IVR
        using (SqlConnection con = new SqlConnection(sqlStrARC))
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
		CONVERT(varchar(10),DATEADD(hh,-@sp_offset,[i].[datestart]),101) [date]
		,COUNT([i].[recordid]) [calls]
		,AVG([i].[duration]) [duration_avg]
	FROM (
		SELECT
		[r].[recordid]
		,CONVERT(datetime, [r].[calldate] + ' ' + [r].[calltime]) [datestart]
		,[r].[ani]
		,ISNULL([main].[dnis],[drtv].[dnis]) [dnis]
		,ISNULL([main].[duration],[drtv].[duration]) [duration]
		,[r].[callid]
		,CASE
			-- ==>> -4 EDT -5 EST
			WHEN CONVERT(datetime, LEFT([r].[calldate],4) + '-' + RIGHT(LEFT([r].[calldate],6),2) + '-' + RIGHT([r].[calldate],2) + ' ' + [r].[calltime]) BETWEEN dbo.fn_dst_start(YEAR(CONVERT(datetime, LEFT([r].[calldate],4) + '-' + RIGHT(LEFT([r].[calldate],6),2) + '-' + RIGHT([r].[calldate],2) + ' ' + [r].[calltime]))) AND dbo.fn_dst_end(YEAR(CONVERT(datetime, LEFT([r].[calldate],4) + '-' + RIGHT(LEFT([r].[calldate],6),2) + '-' + RIGHT([r].[calldate],2) + ' ' + [r].[calltime]))) THEN 4
			ELSE 5
		END [offset_current]
		FROM [dbo].[ivr_record] [r] WITH(NOLOCK)
		LEFT OUTER JOIN [dbo].[ivr_record_main] [main] WITH(NOLOCK) ON [main].[sourceid] = [r].[sourceid] AND [main].[recordid] = [r].[recordid] AND [main].[calldate] = [r].[calldate] AND [main].[calltime] = [r].[calltime] AND [main].[ani] = [r].[ani]
		LEFT OUTER JOIN [dbo].[ivr_record_drtv] [drtv] WITH(NOLOCK) ON [drtv].[sourceid] = [r].[sourceid] AND [drtv].[recordid] = [r].[recordid] AND [drtv].[calldate] = [r].[calldate] AND [drtv].[calltime] = [r].[calltime] AND [drtv].[ani] = [r].[ani]
		WHERE 1=1
		AND ([main].[recordid] IS NOT NULL OR [drtv].[recordid] IS NOT NULL)
	) [i]
	WHERE 1=1
	AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
	AND [i].[ani] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201')
	AND [i].[dnis] IN (SELECT [d].[phonenumber] FROM [dbo].[dnis] [d] WITH(NOLOCK) WHERE [company] = 'DRTV')
	GROUP BY CONVERT(varchar(10),DATEADD(hh,-@sp_offset,[i].[datestart]),101)
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add("@sp_offset", SqlDbType.Int).Value = 0; // IVR is in ET
                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                #endregion SQL Parameters
                // print_sql(cmd);
                #region SQL Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            foreach (DataRow dtRow in dtCalls.Rows)
                            {
                                if (DateTime.Parse(dtRow["date"].ToString()).ToString("yyyyMMdd") == DateTime.Parse(sqlRdr["date"].ToString()).ToString("yyyyMMdd"))
                                {
                                    int tCalls = Int32.Parse(dtRow["calls"].ToString());
                                    int tDuration = Int32.Parse(sqlRdr["duration_avg"].ToString());

                                    dtRow["ivr_calls"] = sqlRdr["calls"].ToString();
                                    dtRow["ivr_duration_avg"] = sqlRdr["duration_avg"].ToString();
                                }
                            }
                        }
                    }
                }
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection - ARC  IVR

        dCount = dtDonations.Rows.Count;
        lblReportDRTVCallHandling.Text = dCount.ToString();
        btnReportDRTVCallHandling.Visible = true;

        gv.DataSource = dtCalls;
        gv.DataBind();

        gvEx.DataSource = dtCalls;
        gvEx.DataBind();

        gv2.DataSource = dtDonations;
        gv2.DataBind();

        gv2Ex.DataSource = dtDonations;
        gv2Ex.DataBind();




    }
    protected void Dashboard_Data_Reporting_DRTV_MasterFile()
    {
        GridView gv = gvReportDRTVMasterFile;
        GridView gvEx = gvReportDRTVMasterFileExport;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
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
[c].[logindatetime]
,[c].[dnis]
,[c].[ani]
,CASE
	WHEN LEN([c].[logindatetime]) > 0 AND LEN([c].[callenddatetime]) > 0 AND [c].[callenddatetime] > [c].[logindatetime] THEN DATEDIFF(s,[c].[logindatetime],[c].[callenddatetime])
	ELSE 0
END [duration]
,[c].[callid]
,[di].[id] [donationid]
,[ci].[prefix] [title]
,[ci].[fname]
,[ci].[lname]
,dbo.fn_titlecase([ci].[address]) [address]
,dbo.fn_titlecase(CASE
	WHEN LEN([ci].[suitenumber]) = 0 OR [suitenumber] IS NULL THEN NULL
	ELSE CASE WHEN [st].[suitetype] IS NOT NULL AND [st].[suitetype] <> 'None' THEN [st].[suitetype] + ' ' ELSE '# ' END + [ci].[suitenumber]
END) [suitenumber]
,dbo.fn_titlecase([ci].[city]) [city]
,[ci].[state]
,[ci].[zip]
,[ci].[country]
,[ci].[hphone] [phonenumber]
,[ci].[phone2]
,CASE WHEN [d].[displayname] = 'Initiated' THEN 'Training' ELSE [d].[displayname] END [disposition]
,[pt].[ccname] [payment]
,[di].[donationamount]
,[pt].[ccname] [cctype]
,[ci].[email]
,[ci].[receiveupdatesyn]
,[ci].[receipt_email]
,(
SELECT
TOP 1
CASE
	WHEN [dn1].[line] = [c].[dnis] THEN [dn1].[phonenumber]
	ELSE [dn1].[line]
END [tollfree]
FROM [dbo].[dnis] [dn1] WITH(NOLOCK)
WHERE 1=1
AND (
	[dn1].[dnis] = [c].[dnis]
	OR [dn1].[line] = [c].[dnis]
	OR [dn1].[phonenumber] = [c].[dnis]
	OR [dn1].[dnis] = RIGHT([c].[dnis],4)
	)
) [tollfree]
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[dnis] [dn] WITH(NOLOCK) ON CASE WHEN LEN([c].[dnis]) = 4 THEN [dn].[dnis] WHEN [dn].[company] = 'DRTV' THEN [dn].[line] ELSE [dn].[phonenumber] END = [c].[dnis]
LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [d].[dispositionid] = [c].[dispositionid]
LEFT OUTER JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
--LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
--LEFT OUTER JOIN [dbo].[merchantresponse] [mr] WITH(NOLOCK) ON [mr].[donationccinfoid] = [di].[id]
LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[suitetype] [st] WITH(NOLOCK) ON [st].[suitetypevalue] = [ci].[suitetype]
LEFT OUTER JOIN [dbo].[cctypelookup] [pt] WITH(NOLOCK) ON [pt].[cctype] = [di].[cctype]

WHERE 1=1
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
AND [dn].[company] = 'DRTV'
                            ";
                if (Connection.GetDBMode() != "Stage")
                {
                    cmdText += "\r";
                    cmdText += @"
AND ([di].[ccnum] IS NULL OR [di].[ccnum] NOT IN ('4111111111111111'))
AND [c].[ani] NOT IN ('2132191673','3102535019','6572320953','8054488206','9995550000','9792021145','7142859201') -- Test/Internal ANIs
                            ";
                }
                cmdText += "\r";
                cmdText += @"
AND [c].[callid] NOT IN (3352782,3352783,3352784,3352785,3352785,3352798,3352822,3352823,3352824,3352825,3352828,3352829,3352832,3352834,3352836,3352838,3352839,3352842,3352843,3352845,3352846,3352847,3352848,3352849,3352850,3352851,3352852,3352853,3352854,3352855,3352856,3352859,3352862,3352864,3352865,3352866,3352867,3352868,3352869,3352870,3352872,3352873,3352874,3352875,3352876,3352877,3352878,3352879,3352880,3352881,3352882,3352884,3352887,3352889,3352890,3352891,3352892,3352894,3352896,3352897,3352898,3352900,3352901,3352902,3352904,3352905,3352906,3352919,3352922,3352923,3352926,3352930,3352931,3352933,3352985,3353024,3353027,3353028,3353031,3353032,3353033,3353035,3353036,3353037,3353038,3353039,3353042,3353047,3353049,3353050,3353051,3353052,3353053,3353056,3353063,3353064,3353065,3353065,3353067,3353068,3353069,3353070,3353071,3353073,3353074,3353075,3353076,3353077,3353078,3353079,3353080,3353081,3353082,3353083,3353084,3353085,3353086,3353087,3353088,3353089,3353090,3353091,3353094,3353095,3353095,3353096,3353098,3353099,3353101,3353102,3353103,3353104,3353105,3353106,3353107,3353108,3353110,3353111,3353112,3353113,3353114,3353115,3353116,3353122,3353133,3353134,3353136,3353137,3353187,3353197,3353200,3353203,3353230,3353254,3353255,3353261,3353262,3353265,3353266,3353267,3353313,3353353,3353398,3353432,3353435,3353459,3353470,3353480,3353485)
ORDER BY [c].[callid]
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                int sp_top = 50000;
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                dtFrom = dtOffSetAdd(dtFrom);
                dtTo = dtOffSetAdd(dtTo);

                cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = sp_top;
                cmd.Parameters.Add("@sp_offset", SqlDbType.Int).Value = ghFunctions.dtUserOffSet;
                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtFrom;
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtTo;
                #endregion SQL Parameters
                // print_sql(cmd);
                #region SQL Processing - GridView
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                int dCount = dt.Rows.Count;
                lblReportDRTVMasterFile.Text = dCount.ToString();
                gv.DataSource = dt;
                gv.DataBind();

                btnReportDRTVMasterFile.Visible = true;
                gvEx.DataSource = dt;
                gvEx.DataBind();

                if (dCount > 0) { } // Not filtered
                #endregion SQL Processing - GridView

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Dashboard_Data_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //http://aspdotnetfaq.com/Faq/How-to-correctly-highlight-GridView-rows-on-Mouse-Hover-in-ASP-NET.aspx
            // when mouse is over the row, save original color to new attribute, and change it to highlight yellow color
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#336699'");

            // when mouse leaves the row, change the bg color to its original value    
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");
        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            #region Footer - gvReportDRTVCallHandlingCalls
            if (gv.ID == "gvReportDRTVCallHandlingCalls" || gv.ID == "gvReportDRTVCallHandlingCallsExport")
            {
                int tIVRCalls = 0;
                int tCalls = 0;
                int tContacted = 0;
                int tAbandoned = 0;

                List<int> lstInclude = new List<int>();
                lstInclude.Add(1);
                lstInclude.Add(2);
                lstInclude.Add(3);
                lstInclude.Add(4);

                string tmp = "";
                foreach (GridViewRow gvRow in gv.Rows)
                {
                    #region Go through Row Cells
                    for (int i = 0; i < gvRow.Cells.Count; i++)
                    {
                        tmp = "";
                        if (!lstInclude.Contains(i)) { continue; }
                        #region Get Value
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
                            tmp = cntrls;
                        }
                        else
                        {
                            tmp = gvRow.Cells[i].Text;
                        }
                        #endregion Get Value
                        if (tmp.Length > 0)
                        {
                            int tmpCount = 0;
                            if (Int32.TryParse(tmp, out tmpCount))
                            {
                                if (i == 1) { tIVRCalls += tmpCount; }
                                if (i == 2) { tCalls += tmpCount; }
                                if (i == 3) { tContacted += tmpCount; }
                                if (i == 4) { tAbandoned += tmpCount; }
                            }
                        }
                    }
                    #endregion Go through Row Cells
                }
                e.Row.Cells[0].Text = "Total";
                e.Row.Cells[1].Text = tIVRCalls.ToString();
                e.Row.Cells[2].Text = tCalls.ToString();
                e.Row.Cells[3].Text = tContacted.ToString();
                e.Row.Cells[4].Text = tAbandoned.ToString();
                e.Row.Font.Bold = true;
            }
            #endregion Footer - gvReportDRTVCallHandlingCalls

            #region Footer - gvReportDRTVCallHandlingDon
            if (gv.ID == "gvReportDRTVCallHandlingDon" || gv.ID == "gvReportDRTVCallHandlingDonExport")
            {
                int tCalls = 0;
                int tDonations = 0;
                int tNonDonations = 0;
                int tCCDonations = 0;
                int tPledge = 0;

                List<int> lstInclude = new List<int>();
                lstInclude.Add(1);
                lstInclude.Add(2);
                lstInclude.Add(4);
                lstInclude.Add(5);
                lstInclude.Add(8);

                string tmp = "";
                foreach (GridViewRow gvRow in gv.Rows)
                {
                    #region Go through Row Cells
                    for (int i = 0; i < gvRow.Cells.Count; i++)
                    {
                        tmp = "";
                        if (!lstInclude.Contains(i)) { continue; }
                        #region Get Value
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
                            tmp = cntrls;
                        }
                        else
                        {
                            tmp = gvRow.Cells[i].Text;
                        }
                        #endregion Get Value
                        if (tmp.Length > 0)
                        {
                            int tmpCount = 0;
                            if (Int32.TryParse(tmp, out tmpCount))
                            {
                                if (i == 1) { tCalls += tmpCount; }
                                if (i == 2) { tDonations += tmpCount; }
                                if (i == 4) { tNonDonations += tmpCount; }
                                if (i == 5) { tCCDonations += tmpCount; }
                                if (i == 8) { tPledge += tmpCount; }
                            }
                        }
                    }
                    #endregion Go through Row Cells
                }
                e.Row.Cells[0].Text = "Total";
                e.Row.Cells[1].Text = tCalls.ToString();
                e.Row.Cells[2].Text = tDonations.ToString();
                e.Row.Cells[4].Text = tNonDonations.ToString();
                e.Row.Cells[5].Text = tCCDonations.ToString();
                e.Row.Cells[8].Text = tPledge.ToString();
                e.Row.Font.Bold = true;
            }
            #endregion Footer - gvReportDRTVCallHandlingDon
        }
    }

    protected void Dashboard_Data_Export_Excel_DRTV(String type)
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

        GridView gv;
        GridView gv2 = null;
        String fileName;
        String reportTitle;
        if (type == "MMS")
        {
            gv = gvReportDRTVMMSExport;
            fileName = "DRTV-MMS";
            reportTitle = "ARC DRTV MMS (" + lblReportDRTVMMS.Text + " records)";
        }
        else if (type == "MainMMS")
        {
            gv = gvReportMainMMSExport;
            fileName = "Main-MMS";
            reportTitle = "ARC Main MMS (" + lblReportMainMMS.Text + " records)";
        }
        else if (type == "RNO")
        {
            gv = gvReportDRTVRNOExport;
            fileName = "DRTV-RNO";
            reportTitle = "ARC DRTV RNO (" + lblReportDRTVRNO.Text + " records)";
        }
        else if (type == "MainRNO")
        {
            gv = gvReportMainRNOExport;
            fileName = "Main-RNO";
            reportTitle = "ARC Main RNO (" + lblReportMainRNO.Text + " records)";
        }
        else if (type == "CALL_HANDLING")
        {
            gv = gvReportDRTVCallHandlingCallsExport;
            gv2 = gvReportDRTVCallHandlingDonExport;
            fileName = "DRTV-CALL-HANDLING";
            reportTitle = "ARC Call Handling (" + lblReportDRTVCallHandling.Text + " records)";
        }
        else if (type == "MASTER_FILE")
        {
            gv = gvReportDRTVMasterFileExport;
            fileName = "DRTV-MASTER-FILE";
            reportTitle = "ARC Master File (" + lblReportDRTVMasterFile.Text + " records)";
        }
        else if (type == "FULFILLMENT_DETAIL")
        {
            gv = gvReportDRTVFulfillmentDetailExport;
            fileName = "DRTV-FULFILLMENT-DETAIL";
            reportTitle = "ARC DRTV Fulfillment Detail (" + lblReportDRTVFulfillmentDetail.Text + " records)";
        }
        else if (type == "FULFILLMENT_TRANSACTION")
        {
            gv = gvReportDRTVFulfillmentTransactionExport;
            fileName = "DRTV-FULFILLMENT-TRANSACTION";
            reportTitle = "ARC DRTV Fulfillment Transaction (" + lblReportDRTVFulfillmentTransaction.Text + " records)";
        }
        else
        {
            throw new Exception("Not supported Grid");
        }

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
        #region Grid - Process
        cl = ws.Cell(sRow, sCol);
        cl.Value = reportTitle;
        cl.Style.Font.Bold = true;
        cl.Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 5).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        #region Grid - Header
        int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
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
        #endregion Grid - Header
        dRow++;
        bool altRow = false;
        #region Process each Grid Row
        foreach (GridViewRow gvRow in gv.Rows)
        {
            dColT = dCol;
            #region Go through Row Cells
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
                    if (gv.HeaderRow.Cells[i].Text == "Caller's Zip Code"
                        || gv.HeaderRow.Cells[i].Text == "Date of Call"
                        || gv.HeaderRow.Cells[i].Text == "Time of Call"
                        || gv.HeaderRow.Cells[i].Text == "Zip Code"
                        )
                    {
                        cl.Style.NumberFormat.Format = "@";
                        cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    else if (gv.HeaderRow.Cells[i].Text == "Date")
                    {
                        cl.Style.NumberFormat.Format = "MM/dd/yyyy";
                    }
                    else if (gv.HeaderRow.Cells[i].Text == "Average Handle Time")
                    {
                        cl.Style.NumberFormat.Format = "hh:mm:ss";
                        cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    if (gv.HeaderRow.Cells[i].Text == "Conversion Rate"
                        || gv.HeaderRow.Cells[i].Text == "CC Conversion Rate"
                        )
                    {
                        cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }

                    cl.Value = cntrls;
                }
                else
                {
                    if (gvRow.Cells[i].Text != "&nbsp;")
                    {
                        //
                        if (gv.HeaderRow.Cells[i].Text == "Amount")
                        {
                            cl.Style.NumberFormat.Format = "$#,##0.00";
                            cl.Value = gvRow.Cells[i].Text;
                        }
                        else if (gv.HeaderRow.Cells[i].Text == "Date")
                        {
                            cl.Style.NumberFormat.Format = "MM/dd/yyyy hh:mm";
                            cl.Value = gvRow.Cells[i].Text;
                        }
                        else if (gv.HeaderRow.Cells[i].Text == "Caller's Zip Code"
                            || gv.HeaderRow.Cells[i].Text == "Date of Call"
                            || gv.HeaderRow.Cells[i].Text == "Time of Call"
                            || gv.HeaderRow.Cells[i].Text == "Zip Code"
                            )
                        {
                            cl.Style.NumberFormat.Format = "@";
                            cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            cl.Value = gvRow.Cells[i].Text;
                        }
                        else
                        {
                            cl.Value = gvRow.Cells[i].Text;
                        }
                    }
                }
                if (altRow) { cl.Style.Fill.BackgroundColor = XLColor.White; } else { cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke; }
                cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;
                cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dColT++;
            }
            #endregion Go through Row Cells
            dRow++;
            if (altRow) altRow = false; else altRow = true;
        }
        GridViewRow gvFRow = gv.FooterRow;
        if (gvFRow.Cells.Count > 0)
        {
            if (gvFRow.Cells[0].Text.Length > 0 && gvFRow.Cells[0].Text != "&nbsp;")
            {
                dColT = dCol;
                #region Go through Row Cells
                for (int i = 0; i < gvFRow.Cells.Count; i++)
                {
                    cl = ws.Cell(dRow, dColT);
                    if (gvFRow.Cells[i].HasControls())
                    {
                        string cntrls = "";
                        foreach (Control c in gvFRow.Cells[i].Controls)
                        {
                            if (c.GetType() == typeof(Label))
                            {
                                cntrls = ((Label)c).Text;
                            }
                        }
                        cl.Value = cntrls;
                    }
                    else
                    {
                        if (gvFRow.Cells[i].Text != "&nbsp;")
                        {
                            cl.Value = gvFRow.Cells[i].Text;
                        }
                    }
                    cl.Style.Font.Bold = true;
                    cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    if (altRow) { cl.Style.Fill.BackgroundColor = XLColor.White; } else { cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke; }
                    cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;
                    dColT++;
                }
                #endregion Go through Row Cells
                dRow++;
                if (altRow) altRow = false; else altRow = true;

            }
        }
        #endregion Process each Grid Row
        #region Grid -Process Grid 2
        if (gv2 != null)
        {
            //dRow++;
            #region Grid - Header
            dRow = sRow + 2; dCol = dColT + 1; dColT = dColT + 1;
            //dRow = dRow + 2; dCol = sCol; dColT = dCol;
            foreach (TableCell cell in gv2.HeaderRow.Cells)
            {
                ws.Cell(dRow, dColT).Value = cell.Text;
                ws.Cell(dRow, dColT).Style.Font.Bold = true;
                ws.Cell(dRow, dColT).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Cell(dRow, dColT).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(dRow, dColT).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                ws.Cell(dRow, dColT).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(dRow, dColT).Style.Border.OutsideBorderColor = XLColor.DarkGray;
                ws.Cell(dRow, dColT).Style.Alignment.WrapText = true;
                dColT++;
            }
            #endregion Grid - Header
            dRow++;
            #region Process each Grid Row
            foreach (GridViewRow gvRow in gv2.Rows)
            {
                dColT = dCol;
                #region Go through Row Cells
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
                        if (gv2.HeaderRow.Cells[i].Text == "Caller's Zip Code"
                            || gv2.HeaderRow.Cells[i].Text == "Date of Call"
                            || gv2.HeaderRow.Cells[i].Text == "Time of Call"
                            || gv2.HeaderRow.Cells[i].Text == "Zip Code"
                            )
                        {
                            cl.Style.NumberFormat.Format = "@";
                            cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else if (gv2.HeaderRow.Cells[i].Text == "Date")
                        {
                            cl.Style.NumberFormat.Format = "MM/dd/yyyy";
                        }
                        else if (gv2.HeaderRow.Cells[i].Text == "Average Handle Time")
                        {
                            cl.Style.NumberFormat.Format = "hh:mm:ss";
                            cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        if (gv2.HeaderRow.Cells[i].Text == "Conversion Rate"
                            || gv2.HeaderRow.Cells[i].Text == "CC Conversion Rate"
                            )
                        {
                            cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }

                        cl.Value = cntrls;
                    }
                    else
                    {
                        if (gvRow.Cells[i].Text != "&nbsp;")
                        {
                            //
                            if (gv2.HeaderRow.Cells[i].Text == "Amount")
                            {
                                cl.Style.NumberFormat.Format = "$#,##0.00";
                                cl.Value = gvRow.Cells[i].Text;
                            }
                            else if (gv2.HeaderRow.Cells[i].Text == "Date")
                            {
                                cl.Style.NumberFormat.Format = "MM/dd/yyyy hh:mm";
                                cl.Value = gvRow.Cells[i].Text;
                            }
                            else if (gv2.HeaderRow.Cells[i].Text == "Caller's Zip Code"
                                || gv2.HeaderRow.Cells[i].Text == "Date of Call"
                                || gv2.HeaderRow.Cells[i].Text == "Time of Call"
                                || gv2.HeaderRow.Cells[i].Text == "Zip Code"
                                )
                            {
                                cl.Style.NumberFormat.Format = "@";
                                cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                cl.Value = gvRow.Cells[i].Text;
                            }
                            else
                            {
                                cl.Value = gvRow.Cells[i].Text;
                            }
                        }
                    }
                    if (altRow) { cl.Style.Fill.BackgroundColor = XLColor.White; } else { cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke; }
                    cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;
                    cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    dColT++;
                }
                #endregion Go through Row Cells
                dRow++;
                if (altRow) altRow = false; else altRow = true;
            }
            gvFRow = gv2.FooterRow;
            if (gvFRow.Cells.Count > 0)
            {
                if (gvFRow.Cells[0].Text.Length > 0 && gvFRow.Cells[0].Text != "&nbsp;")
                {
                    dColT = dCol;
                    #region Go through Row Cells
                    for (int i = 0; i < gvFRow.Cells.Count; i++)
                    {
                        cl = ws.Cell(dRow, dColT);
                        if (gvFRow.Cells[i].HasControls())
                        {
                            string cntrls = "";
                            foreach (Control c in gvFRow.Cells[i].Controls)
                            {
                                if (c.GetType() == typeof(Label))
                                {
                                    cntrls = ((Label)c).Text;
                                }
                            }
                            cl.Value = cntrls;
                        }
                        else
                        {
                            if (gvFRow.Cells[i].Text != "&nbsp;")
                            {
                                cl.Value = gvFRow.Cells[i].Text;
                            }
                        }
                        cl.Style.Font.Bold = true;
                        cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        if (altRow) { cl.Style.Fill.BackgroundColor = XLColor.White; } else { cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke; }
                        cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;
                        dColT++;
                    }
                    #endregion Go through Row Cells
                    dRow++;
                    if (altRow) altRow = false; else altRow = true;
                }
            }
            #endregion Process each Grid Row

        }
        #endregion Grid -Process Grid 2

        #endregion Grid - Call Dispositions
        #region Wrap Up - Save/Download the File
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        ws.Columns().AdjustToContents();
        // Total Width: 40
        if (type == "CALL_HANDLING")
        {
            ws.Column(1).Width = 12;
            ws.Column(2).Width = 8;
            ws.Column(3).Width = 10;
            ws.Column(4).Width = 10;
        }
        else
        {
            ws.Columns(1, 3).Width = 11;
            ws.Column(4).Width = 7;
            //8.43 * 4 == 33.72
        }
        //ws.Rows().AdjustToContents();
        ws.ShowGridLines = false;
        // fileName = "Dashboard-Reporting-" + fileName;
        // fileName += DateTime.Now.ToString("-yyyyMMdd-HHmmss");
        DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
        if (type == "CALL_HANDLING" || type == "MASTER_FILE")
        {
            fileName += dtTo.ToString("-MMyy");
        }
        else
        {
            fileName += dtTo.ToString("-MMddyy");
        }

        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}{1}.xlsx", fileName.Replace(" ", "_"), ""));

        using (MemoryStream memoryStream = new MemoryStream())
        {
            wb.SaveAs(memoryStream);
            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
            memoryStream.Close();
        }

        HttpContext.Current.Response.End();
        #endregion Wrap Up - Save/Download the File
    }
    protected void Dashboard_Data_Export_CSV_DRTV(GridView gv, String fileName)
    {



        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".csv");
        Response.Charset = "";
        Response.ContentType = "application/text";

        // gv.AllowPaging = false;
        // gv.DataBind();

        StringBuilder sb = new StringBuilder();
        for (int k = 0; k < gv.Columns.Count; k++)
        {
            //add separator
            if (k == 0) sb.Append(gv.Columns[k].HeaderText); else sb.Append(',' + gv.Columns[k].HeaderText);
        }
        //append new line
        sb.Append("\r\n");
        for (int i = 0; i < gv.Rows.Count; i++)
        {
            for (int k = 0; k < gv.Columns.Count; k++)
            {
                String cValue = "";
                if (gv.Rows[i].Cells[k].HasControls())
                {
                    cValue = "ctrl";
                    foreach (Control c in gv.Rows[i].Cells[k].Controls)
                    {
                        if (c.GetType() == typeof(Label))
                        {
                            cValue = ((Label)c).Text;
                        }
                    }
                }
                else
                {
                    // &nbsp;
                    if (gv.Rows[i].Cells[k].Text != "&nbsp;")
                        cValue = gv.Rows[i].Cells[k].Text;
                }

                if (k == 0) sb.Append(cValue); else sb.Append(',' + cValue);
            }
            //append new line
            sb.Append("\r\n");
        }
        Response.Output.Write(sb.ToString());
        Response.Flush();
        Response.End();
    }
    protected String report_drtv_disposition(String disposition, String type, String src)
    {
        string rtrn = "";
        string dsp = "";
        string rev = "";
        if (disposition.Length > 0)
        {
            switch (disposition.Trim())
            {
                #region DRTV Switch
                case "Wants More Sustainer Information": if (src == "drtv") { dsp = "1"; rev = ""; } else { dsp = "16"; rev = ""; } break;
                case "Donation": if (src == "drtv") { dsp = "2"; rev = "2R"; } else { dsp = "17"; rev = "17R"; } break;
                case "Sustainer": if (src == "drtv") { dsp = "3"; rev = "3R"; } else { dsp = "18"; rev = "18R"; } break;
                case "Pledge": if (src == "drtv") { dsp = "5"; rev = "5R"; } else { dsp = "19"; rev = "19R"; } break; // If Pldge and AMOUNT == 0 then map to B019
                case "Pledge [Sustainer]": if (src == "drtv") { dsp = "6"; rev = "6R"; } else { dsp = "20"; rev = "20R"; } break;
                #endregion DRTV Switch

                case "OTO EFT": dsp = "7"; rev = "7R"; break;
                case "MD MD EFT": dsp = "8"; rev = "8R"; break;

                case "Training": dsp = "B010"; rev = ""; break;
                case "Complaint on Policies": dsp = "B018"; rev = ""; break;
                case "No Whisper": dsp = "B006"; rev = ""; break;
                case "Hung Up": dsp = "B007"; rev = ""; break;
                case "Pledge [No Address]": dsp = "B019"; rev = ""; break;
                case "Information Only": dsp = "B019"; rev = ""; break;
                case "Referred": dsp = "NA"; rev = ""; break;
                case "Wrong Number": dsp = "B016"; rev = ""; break;
                case "Research": dsp = "NA"; rev = ""; break;

                case "Wanted to Donate Blood/Questions": dsp = "B004"; rev = ""; break;
                case "Wanted to Volunteer": dsp = "B004"; rev = ""; break;
                case "Wanted to Donate Goods": dsp = "B015"; rev = ""; break;
                case "Wanted to Sponsor an Event": dsp = "B015"; rev = ""; break;
                case "Wanted Information on Red Cross": dsp = "B019"; rev = ""; break;
                case "Wanted Donation/Receipt Info": dsp = "B002"; rev = ""; break;
                case "Needed Local Chapter Info": dsp = "B001"; rev = ""; break;
                case "Needed Help": dsp = "B001"; rev = ""; break;
                case "Needed to Locate Military Personnel": dsp = "B001"; rev = ""; break;
                case "Needed Media Inquiry": dsp = "B019"; rev = ""; break;
                case "Wanted Class/Course Information ": dsp = "B019"; rev = ""; break;
                case "Want Information on Vehicle Donation ": dsp = "B019"; rev = ""; break;
                case "Wants Explanation of Different Funds": dsp = "B019"; rev = ""; break;
                case "Wanted to Donate Planned Gift": dsp = "B002"; rev = ""; break;
                case "Wants to be Removed From Mailing List": dsp = "B014"; rev = ""; break;

                case "Info About International Services": dsp = "B019"; rev = ""; break;
                case "Wanted Course/Class Information": dsp = "B019"; rev = ""; break;
                case "Wanted Information About Use of Funds": dsp = "B019"; rev = ""; break;
                case "Want Information On Vehicle Donation": dsp = "B019"; rev = ""; break;

                default: dsp = "n/a"; rev = "n/a"; break;
            }
        }
        if (type == "dsp") rtrn = dsp;
        else rtrn = rev;

        return rtrn;
    }
    public DateTime dtOffSetAdd(DateTime dt)
    {
        // Convert based on Users Offset
        Int32 dtOffSet = ghFunctions.dtUserOffSet;
        return dt.AddHours(dtOffSet);
    }
    protected String report_drtv_callhandling_conversion(String count_one, String count_two)
    {
        String rtrn = "0";
        Double cOne = 0;
        Double cTwo = 0;
        try
        {
            if (Double.TryParse(count_one, out cOne) && Double.TryParse(count_two, out cTwo))
            {
                if (cTwo > 0 && cOne > 0)
                {
                    rtrn = String.Format("{0:P}", cTwo / cOne);
                }
                else
                {
                    rtrn = String.Format("{0:P}", 0);
                }
            }

        }
        catch
        {
            rtrn = "-1";
        }

        return rtrn;
    }
    protected String report_drtv_callhandling_nondoncalls(String count_calls, String count_donations)
    {
        String rtrn = "0";
        Int32 calls = 0;
        Int32 donations = 0;
        try
        {
            if (Int32.TryParse(count_calls, out calls) && Int32.TryParse(count_donations, out donations))
            {
                rtrn = String.Format("{0}", calls - donations);
            }

        }
        catch
        {
            rtrn = "0";
        }

        return rtrn;
    }
    protected String report_get_five9id_from_arcid(String callid)
    {
        String rtrn_five9_callid = "";
        #region SQL Connection - DE
        using (SqlConnection con = new SqlConnection(sqlStrDE))
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
TOP 1
[if].[callid] [five9_callid]
FROM [dbo].[interactions_arc] [ia] WITH(NOLOCK)
JOIN [dbo].[interactions_five9] [if] WITH(NOLOCK) ON [if].[companyid] = [ia].[companyid] AND [if].[interactionid] = [ia].[interactionid]
WHERE 1=1
AND [ia].[companyid] = 3
AND [ia].[callid] = @sp_arc_callid
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = 3;
                cmd.Parameters.Add("@sp_arc_callid", SqlDbType.Int).Value = callid;
                #endregion SQL Parameters
                #region SQL Processing
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            rtrn_five9_callid = sqlRdr["five9_callid"].ToString();
                        }
                    }
                }
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection - DE
        return rtrn_five9_callid;

    }

    /// <summary>
    /// This functions should be put into their own file
    /// 
    /// </summary>
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
    protected void print_sql(SqlCommand cmd)
    {
        print_sql(cmd, sqlPrint, "append");
    }
    protected void print_sql(SqlCommand cmd, String type)
    {
        ghFunctions.print_sql(cmd, sqlPrint, type);
        //print_sql(cmd, sqlPrint, type);
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
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrInt))
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
                    cmd.Parameters.Add("@sp_username", SqlDbType.VarChar, 100).Value = this.Page.User.Identity.Name;
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
                using (SqlConnection con = new SqlConnection(sqlStrInt))
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
                        cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = userid;
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