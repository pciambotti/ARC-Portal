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
public partial class Dashboard_Other : System.Web.UI.Page
{
    private String sqlStr = Connection.GetConnectionString("MiddleWare", "");
    private String sqlStrInt = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    private Int32 countCalls = 0;
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Dashboard";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
        if (Connection.GetDBMode() == "Stage")
        {
            sqlStrInt = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            #region Chart Image
            if (Session["imgNameDash"] == null || !IsPostBack)
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
                Session["imgNameDash"] = imgName;
            }
            #endregion Chart Image
            //DropDown_Clients();
            //Client_Selected();
            #region Refresh Timer
            gvCR_tglTimer.Checked = true;
            gvCountReport_Timer1.Enabled = false;
            gvCR_lblTimer.Text = "stopped";
            gvCR_lblTimer.ForeColor = System.Drawing.Color.Red;

            if (!gvCountReport_Timer1.Enabled) { ScriptManager.RegisterClientScriptBlock(gvCountReport_Timer, this.Page.GetType(), "RefreshCountdown", "RefreshCountdownStart('gvCR_refreshCountdown',0);", true); }
            //GridView_Refresh_Data(0, this.Page.User.Identity.Name, gvCountReport, null);
            #endregion Refresh Timer
        }
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
                fMark = new XLMarker { ColumnId = sCol + 4, RowId = sRow + 12 };
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

        Cell_Format_01(ws, sRow + 2, sCol, lblAnswerPerf, cntAnswerPerf, perAnswerPerf);

        Cell_Format_01(ws, sRow + 5, sCol, lblTotalCalls, cntTotalCalls, perTotalCalls);

        Cell_Format_01(ws, sRow + 8, sCol, lblTotalAnswered, cntTotalAnswered, perTotalAnswered);

        Cell_Format_01(ws, sRow + 11, sCol, lblTotalAbandoned, cntTotalAbandoned, perTotalAbandoned);

        Cell_Format_02(ws, sRow + 14, sCol, lblSpeedAnswer, avgSpeedAnswer);

        Cell_Format_02(ws, sRow + 17, sCol, lblTalkTime, avgTalkTime);

        Cell_Format_02(ws, sRow + 17, sCol, lblAbandonedTime, avgAbandonedTime);

        sCol = sCol + 4;
        ws.Cell(sRow, sCol).Value = "Abandon Intervals";
        ws.Range(sRow, sCol, sRow, sCol + 1).Merge();
        ws.Cell(sRow, sCol).Style.Font.Bold = true;
        ws.Cell(sRow, sCol).Style.Font.FontSize = 12;

        Cell_Format_01(ws, sRow + 2, sCol, lblAbandoned90, cntAbandoned90, perAbandoned90);

        Cell_Format_01(ws, sRow + 5, sCol, lblAbandoned120, cntAbandoned120, perAbandoned120);

        Cell_Format_01(ws, sRow + 8, sCol, lblAbandoned120p, cntAbandoned120p, perAbandoned120p);

        ws.Cell(sRow + 12, sCol).Value = "Answer Intervals";
        ws.Range(sRow + 12, sCol, sRow + 12, sCol + 1).Merge();
        ws.Cell(sRow + 12, sCol).Style.Font.Bold = true;
        ws.Cell(sRow + 12, sCol).Style.Font.FontSize = 12;

        Cell_Format_01(ws, sRow + 14, sCol, lblAnswered90, cntAnswered90, perAnswered90);

        Cell_Format_01(ws, sRow + 17, sCol, lblAnswered120, cntAnswered120, perAnswered120);

        Cell_Format_01(ws, sRow + 20, sCol, lblAnswered120p, cntAnswered120p, perAnswered120p);
        #endregion Table Grids
        ws.Range(sRow, 1, sRow + 22, sCol + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        sCol = sCol + 4;
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
        #region Wrap Up - Save/Download the File
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        //ws.Cell(sRow + 1, sCol + 3).Value = "Done";

        ws.Rows().AdjustToContents();
        ws.Columns().AdjustToContents();
        ws.Column(13).Width = 20;
        ws.Column(14).Width = 20;
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
    protected void Cell_Format_01(IXLWorksheet ws, Int32 row, Int32 col, Label lbl, Label lbl2, Label lbl3)
    {
        // ws.Cell(row, col)
        ws.Cell(row, col).Value = lbl.Text;
        ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        ws.Cell(row, col + 1).Value = "%";
        ws.Cell(row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        ws.Range(row, col, row, col + 1).Style.Font.Bold = true;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
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
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        Dashboard_Refresh();
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
            if (ddlCallCampaigns.SelectedIndex == -1)
            {
                rpMessage.Text = "You must select at least 1 campaign to continue.";
            }
            else
            {
                DateTime dt = DateTime.UtcNow;
                rpMessage.Text = "";
                //DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                //DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                //DateTime dtFrom = DateTime.UtcNow.Date.AddHours(-7);
                //DateTime dtTo = dtFrom.AddHours(23).AddMinutes(59).AddSeconds(59);
                //dtStartDate.Text = DateTime.Now.Date.ToString("MM/dd/yyyy");
                //dtEndDate.Text = DateTime.Now.Date.ToString("MM/dd/yyyy");
                ////lblMessage.Text = "Date Range: " + dtFrom.ToString("yyyy-MM-dd HH:mm:ss") + " to " + dtTo.ToString("yyyy-MM-dd HH:mm:ss");

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

                    Data_Call_Dispositions();
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
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                String cmdText = "";
                #region Build cmdText
                cmdText = "";
                cmdText += @"SELECT
                        COUNT([f].[CallId]) [total_calls]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] < 90000 THEN 1
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] < 90 THEN 1
		                        ELSE NULL
	                        END
                        ) [answered_90]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] > 90000 AND [f].[QueueTime] <= 120000 THEN 1
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] > 90 AND [f].[QueueTime] <= 120 THEN 1
		                        ELSE NULL
	                        END
                        ) [answered_120]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] > 120000 THEN 1
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] > 120 THEN 1
		                        ELSE NULL
	                        END
                        ) [answered_120p]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] < 90000 THEN 1
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] < 90 THEN 1
		                        ELSE NULL
	                        END
                        ) [abandon_90]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] > 90000 AND [f].[QueueTime] <= 120000 THEN 1
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] > 90 AND [f].[QueueTime] <= 120 THEN 1
		                        ELSE NULL
	                        END
                        ) [abandon_120]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] > 120000 THEN 1
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] > 120 THEN 1
		                        ELSE NULL
	                        END
                        ) [abandon_120p]
                        ,[dbo].[ConvertMilliSecondsToHHMMss] (AVG(CAST(((CASE WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] > 1000 THEN [f].[QueueTime] WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] < 1000 THEN [f].[QueueTime] * 1000 END)) AS BIGINT))) [avgQueueTime] 
                        ,[dbo].[ConvertMilliSecondsToHHMMss] (AVG(CAST(((CASE  WHEN [f].[disposition] <> 'Abandon' AND [f].[TalkTime] > 1000 THEN [f].[TalkTime] WHEN [f].[disposition] <> 'Abandon' AND [f].[TalkTime] < 1000 THEN [f].[TalkTime] * 1000 END)) AS BIGINT))) [avgTalkTime] 
                        ,[dbo].[ConvertMilliSecondsToHHMMss] (AVG(CAST(((CASE WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] > 1000 THEN [f].[QueueTime] WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] < 1000 THEN [f].[QueueTime] * 1000 END)) AS BIGINT))) [avgAbandonTime] 
                        FROM [MiddleWare].[dbo].[Five9Qlikview] [f] WITH(NOLOCK)
                        ";
                cmdText += "WHERE [f].[ClientId]=1\r";
                cmdText += "AND [f].[calldate] BETWEEN @sp_date_start AND @sp_date_end\r";
                cmdText += "AND [f].[campaign] IN (SELECT [CampaignName] FROM @sp_campaigns)";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                DataTable clientCampaigns = dt_Client_Campaigns();
                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "dbo.CampaignTableType";
                #endregion SQL Parameters
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

                            perTotalCalls.Text = String.Format("{0:P0}", GetPercent(totalCalls, totalCalls));
                            perTotalAnswered.Text = String.Format("{0:P0}", GetPercent(totalAnswered, totalCalls));
                            perTotalAbandoned.Text = String.Format("{0:P0}", GetPercent(totalAbandoned, totalCalls));
                            //return 

                            cntAnswerPerf.Text = answered_90.ToString();
                            perAnswerPerf.Text = String.Format("{0:P0}", GetPercent(answered_90, totalAnswered));
                            cntSupportLevel.Value = (GetPercent(answered_90, totalAnswered) * 100).ToString();

                            cntAnswered90.Text = answered_90.ToString();
                            perAnswered90.Text = String.Format("{0:P0}", GetPercent(answered_90, totalAnswered));
                            cntAnswered120.Text = answered_120.ToString();
                            perAnswered120.Text = String.Format("{0:P0}", GetPercent(answered_120, totalAnswered));
                            cntAnswered120p.Text = answered_120p.ToString();
                            perAnswered120p.Text = String.Format("{0:P0}", GetPercent(answered_120p, totalAnswered));

                            cntAbandoned90.Text = abandon_90.ToString();
                            perAbandoned90.Text = String.Format("{0:P0}", GetPercent(abandon_90, totalAbandoned));
                            cntAbandoned120.Text = abandon_120.ToString();
                            perAbandoned120.Text = String.Format("{0:P0}", GetPercent(abandon_120, totalAbandoned));
                            cntAbandoned120p.Text = abandon_120p.ToString();
                            perAbandoned120p.Text = String.Format("{0:P0}", GetPercent(abandon_120p, totalAbandoned));


                            avgSpeedAnswer.Text = String.IsNullOrEmpty(sqlRdr["avgQueueTime"].ToString()) ? "00:00:00" : sqlRdr["avgQueueTime"].ToString();
                            avgTalkTime.Text = String.IsNullOrEmpty(sqlRdr["avgTalkTime"].ToString()) ? "00:00:00" : sqlRdr["avgTalkTime"].ToString();
                            avgAbandonedTime.Text = String.IsNullOrEmpty(sqlRdr["avgAbandonTime"].ToString()) ? "00:00:00" : sqlRdr["avgAbandonTime"].ToString();
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
    protected DataTable dt_Client_Campaigns()
    {
        // We create a table variable
        // To pass the campaigns selected
        // Need to see if this breaks with large selection

        DataTable clientCampaigns = new DataTable();// = CategoriesDataTable.GetChanges(DataRowState.Added);
        clientCampaigns.Columns.Add(new DataColumn("CampaignName", typeof(String)));
        DataRow dr;
        foreach (ListItem li in ddlCallCampaigns.Items)
        {
            if (li.Selected)
            {
                dr = clientCampaigns.NewRow(); dr["CampaignName"] = li.Text; clientCampaigns.Rows.Add(dr);
            }
        }
        return clientCampaigns;
    }
    protected void Data_Call_Dispositions()
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
                String cmdText = "";
                #region Build cmdText
                cmdText = "";
                cmdText += "SELECT\r";
                cmdText += "[f].[Disposition]\r";
                cmdText += ",COUNT([f].[CallId]) [count]\r";
                cmdText += "FROM [MiddleWare].[dbo].[Five9Qlikview] [f] WITH(NOLOCK)\r";
                cmdText += "WHERE [f].[ClientId] = 1\r";
                cmdText += "AND [f].[calldate] BETWEEN @sp_date_start AND @sp_date_end\r";
                cmdText += "AND [f].[campaign] IN (SELECT [CampaignName] FROM @sp_campaigns)";
                cmdText += "GROUP BY [f].[Disposition]\r";
                cmdText += "ORDER BY [f].[Disposition]\r";
                #endregion Build cmdText

                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();

                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                DataTable clientCampaigns = dt_Client_Campaigns();
                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "dbo.CampaignTableType";
                #endregion SQL Parameters

                #region SQL Processing - GridView
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                int dCount = DataTable_Row_Sum_Values(dt, 1);
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
    protected Int32 DataTable_Row_Sum_Values(DataTable dt, Int32 col)
    {
        int dtSum = 0;
        foreach (DataRow r in dt.Rows)
        {
            dtSum += Convert.ToInt32(r[col].ToString());
        }
        return dtSum;
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
    protected void Client_Index_Changed(object sender, EventArgs e)
    {
        Client_Selected();
    }
    protected void Client_Selected()
    {
        int clientid = 0;
        clientid = Convert.ToInt32(ddlCallClients.SelectedValue);
        DropDown_Campaigns(clientid);
        lblClientCampaigns.Text = "Campaigns Loaded: " + ddlCallCampaigns.Items.Count.ToString();
        #region Reset the Dashboard
        // This is done to prevent the user from causing an export error
        // Since the chart images are saved based on the client id
        pnlCCPerformance.Visible = false;
        pnlIntervalAbandon.Visible = false;
        pnlIntervalAnswer.Visible = false;
        btnExportFull.Visible = false;
        gvCallDispositions.DataBind();
        UpdatePanel1.Update();
        #endregion Reset the Dashboard
    }
    protected void DropDown_Clients()
    {
        ListBox ddl = ddlCallClients;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                cmd.CommandText = "[dbo].[sp_dashboard_get_clients]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_clientid", "0"));
                #endregion SQL Parameters

                #region SQL Processing - DropDown
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                ddl.DataTextField = "Text";
                ddl.DataValueField = "Value";

                ddl.Items.Clear();
                ddl.DataSource = dt;
                ddl.DataBind();

                ddl.SelectedIndex = 2;

                #endregion SQL Processing - DropDown

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void DropDown_Campaigns(int clientid)
    {
        ListBox ddl = ddlCallCampaigns;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                cmd.CommandText = "[dbo].[sp_dashboard_get_client_campaigns]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_clientid", clientid));
                #endregion SQL Parameters

                #region SQL Processing - DropDown
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                ddl.DataTextField = "Text";
                ddl.DataValueField = "Value";

                ddl.Items.Clear();
                ddl.DataSource = dt;
                ddl.DataBind();

                // Select all Items
                foreach (ListItem li in ddl.Items)
                {
                    li.Selected = true;
                }
                #endregion SQL Processing - DropDown

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GoTo_Selective_Report(object sender, EventArgs e)
    {
        Response.Redirect("~/Dashboard_Other.aspx");
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